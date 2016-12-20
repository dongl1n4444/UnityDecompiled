namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Builders;
    using Boo.Lang.Compiler.TypeSystem.Internal;
    using Boo.Lang.Environments;
    using Boo.Lang.Runtime;
    using System;
    using System.Reflection;
    using UnityScript;
    using UnityScript.Core;
    using UnityScript.Scripting;
    using UnityScript.TypeSystem;

    [Serializable]
    public class ProcessEvalInvocations : AbstractVisitorCompilerStep
    {
        protected InternalMethod _currentMethod;
        protected InternalLocal _evaluationContextLocal;
        [NonSerialized]
        protected static readonly MethodInfo Evaluator_Eval = typeof(Evaluator).GetMethod("Eval");

        public void AddLocalVariablesAsFields(BooClassBuilder builder)
        {
            Field field;
            foreach (Local local in this.CurrentMethodNode.get_Locals())
            {
                InternalLocal entity = this.GetEntity(local);
                if (!entity.get_IsPrivateScope())
                {
                    field = builder.AddPublicField(entity.get_Name(), entity.get_Type());
                    this.SetEvaluationContextField(local, (InternalField) this.GetEntity(field));
                }
            }
            foreach (ParameterDeclaration declaration in this.CurrentMethodNode.get_Parameters())
            {
                InternalParameter parameter = this.GetEntity(declaration);
                field = builder.AddPublicField(parameter.get_Name(), parameter.get_Type());
                this.SetEvaluationContextField(declaration, (InternalField) this.GetEntity(field));
            }
        }

        public void ChainConstructorsFromBaseType(BooClassBuilder builder)
        {
            foreach (IConstructor constructor in TypeSystemExtensions.GetConstructors(builder.get_Entity().get_BaseType()))
            {
                ExpressionStatement statement = this.get_CodeBuilder().CreateSuperConstructorInvocation(constructor);
                MethodInvocationExpression expression = statement.get_Expression() as MethodInvocationExpression;
                BooMethodBuilder builder2 = builder.AddConstructor();
                int index = 0;
                IParameter[] parameters = constructor.GetParameters();
                int length = parameters.Length;
                while (index < length)
                {
                    ParameterDeclaration declaration = builder2.AddParameter(parameters[index].get_Name(), parameters[index].get_Type());
                    expression.get_Arguments().Add(this.get_CodeBuilder().CreateReference(declaration));
                    index++;
                }
                builder2.get_Body().Add(statement);
            }
        }

        public MemberReferenceExpression CreateEvaluationContextFieldReference(InternalField field)
        {
            return this.get_CodeBuilder().CreateMemberReference(this.CreateEvaluationContextReference(), field);
        }

        public ReferenceExpression CreateEvaluationContextReference()
        {
            return this.get_CodeBuilder().CreateReference(this._evaluationContextLocal);
        }

        public MethodInvocationExpression CreateEvaluatorInvocation(MethodInvocationExpression node)
        {
            return this.get_CodeBuilder().CreateMethodInvocation(Evaluator_Eval, this.CreateEvaluationContextReference(), node.get_Arguments().get_Item(0));
        }

        public IType DefineEvaluationContext()
        {
            string[] textArray1 = new string[] { "EvaluationContext" };
            BooClassBuilder builder = this.get_CodeBuilder().CreateClass(this.get_Context().GetUniqueName(textArray1), 8);
            builder.AddBaseType(this.Map(typeof(EvaluationContext)));
            this.ChainConstructorsFromBaseType(builder);
            this.AddLocalVariablesAsFields(builder);
            this.CurrentTypeNode.get_Members().Add(builder.get_ClassDefinition());
            return builder.get_Entity();
        }

        public Expression EvaluationDomainProviderReference()
        {
            return (!this._currentMethod.get_IsStatic() ? this.get_CodeBuilder().CreateSelfReference(this.CurrentType) : this.get_CodeBuilder().CreateReference(My<EvaluationDomainProviderImplementor>.Instance.StaticEvaluationDomainProviderFor((ClassDefinition) this.CurrentTypeNode)));
        }

        public InternalField GetEvaluationContextField(Node node)
        {
            object obj1 = node.get_Item("EvaluationContextField");
            if (!(obj1 is InternalField))
            {
            }
            return (InternalField) RuntimeServices.Coerce(obj1, typeof(InternalField));
        }

        public bool IsEvalInvocation(MethodInvocationExpression node)
        {
            return (node.get_Target().get_Entity() == UnityScriptTypeSystem.UnityScriptEval);
        }

        public override void LeaveClassDefinition(ClassDefinition node)
        {
            if (EvalAnnotation.IsMarked(node))
            {
                My<EvaluationDomainProviderImplementor>.Instance.ImplementIEvaluationDomainProviderOn(node);
            }
        }

        public override void LeaveMethodInvocationExpression(MethodInvocationExpression node)
        {
            if (this.IsEvalInvocation(node))
            {
                if (!string.IsNullOrEmpty(this.UnityScriptParameters.DisableEval))
                {
                    this.Error(node, UnityScriptCompilerErrors.EvalHasBeenDisabled(node.get_Target().get_LexicalInfo(), this.UnityScriptParameters.DisableEval));
                }
                else
                {
                    this.ReplaceEvalByEvaluatorEval(node);
                }
            }
        }

        public IType Map(Type type)
        {
            return this.get_TypeSystemServices().Map(type);
        }

        public override void OnConstructor(Constructor node)
        {
            this.OnMethod(node);
        }

        public override void OnMethod(Method node)
        {
            if (EvalAnnotation.IsMarked(node))
            {
                this._currentMethod = this.GetEntity(node);
                IType evaluationContextType = this.DefineEvaluationContext();
                Block block = this.PrepareEvaluationContextInitialization(evaluationContextType);
                this.Visit(node.get_Body());
                node.get_Body().Insert(0, block);
            }
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            IInternalEntity entity = node.get_Entity() as IInternalEntity;
            if (entity != null)
            {
                InternalField evaluationContextField = this.GetEvaluationContextField(entity.get_Node());
                if (evaluationContextField != null)
                {
                    node.get_ParentNode().Replace(node, this.CreateEvaluationContextFieldReference(evaluationContextField));
                }
            }
        }

        public Block PrepareEvaluationContextInitialization(IType evaluationContextType)
        {
            this._evaluationContextLocal = this.get_CodeBuilder().DeclareTempLocal(this.CurrentMethodNode, evaluationContextType);
            Block block = new Block();
            block.Add(this.get_CodeBuilder().CreateAssignment(this.CreateEvaluationContextReference(), this.get_CodeBuilder().CreateConstructorInvocation(UtilitiesModule.ConstructorTakingNArgumentsFor(evaluationContextType, 1), this.EvaluationDomainProviderReference())));
            foreach (ParameterDeclaration declaration in this.CurrentMethodNode.get_Parameters())
            {
                block.Add(this.get_CodeBuilder().CreateAssignment(this.CreateEvaluationContextFieldReference(this.GetEvaluationContextField(declaration)), this.get_CodeBuilder().CreateReference(declaration)));
            }
            return block;
        }

        public void ReplaceEvalByEvaluatorEval(MethodInvocationExpression node)
        {
            node.get_ParentNode().Replace(node, this.CreateEvaluatorInvocation(node));
        }

        public override void Run()
        {
            if (this.get_Errors().Count <= 0)
            {
                this.Visit(this.get_CompileUnit());
            }
        }

        public void SetEvaluationContextField(Node node, InternalField field)
        {
            node.set_Item("EvaluationContextField", field);
        }

        public Block CurrentMethodBody
        {
            get
            {
                return this.CurrentMethodNode.get_Body();
            }
        }

        public Method CurrentMethodNode
        {
            get
            {
                return this._currentMethod.get_Method();
            }
        }

        public IType CurrentType
        {
            get
            {
                return this._currentMethod.get_DeclaringType();
            }
        }

        public TypeDefinition CurrentTypeNode
        {
            get
            {
                return this.CurrentMethodNode.get_DeclaringType();
            }
        }

        public UnityScriptCompilerParameters UnityScriptParameters
        {
            get
            {
                return (UnityScriptCompilerParameters) base._context.get_Parameters();
            }
        }
    }
}

