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
            foreach (Local local in this.CurrentMethodNode.Locals)
            {
                InternalLocal entity = (InternalLocal) this.GetEntity(local);
                if (!entity.IsPrivateScope)
                {
                    field = builder.AddPublicField(entity.Name, entity.Type);
                    this.SetEvaluationContextField(local, (InternalField) this.GetEntity(field));
                }
            }
            foreach (ParameterDeclaration declaration in this.CurrentMethodNode.Parameters)
            {
                InternalParameter parameter = (InternalParameter) this.GetEntity(declaration);
                field = builder.AddPublicField(parameter.Name, parameter.Type);
                this.SetEvaluationContextField(declaration, (InternalField) this.GetEntity(field));
            }
        }

        public void ChainConstructorsFromBaseType(BooClassBuilder builder)
        {
            foreach (IConstructor constructor in builder.Entity.BaseType.GetConstructors())
            {
                ExpressionStatement stmt = this.CodeBuilder.CreateSuperConstructorInvocation(constructor);
                MethodInvocationExpression expression = stmt.Expression as MethodInvocationExpression;
                BooMethodBuilder builder2 = builder.AddConstructor();
                int index = 0;
                IParameter[] parameters = constructor.GetParameters();
                int length = parameters.Length;
                while (index < length)
                {
                    ParameterDeclaration parameter = builder2.AddParameter(parameters[index].Name, parameters[index].Type);
                    expression.Arguments.Add(this.CodeBuilder.CreateReference(parameter));
                    index++;
                }
                builder2.Body.Add(stmt);
            }
        }

        public MemberReferenceExpression CreateEvaluationContextFieldReference(InternalField field) => 
            this.CodeBuilder.CreateMemberReference(this.CreateEvaluationContextReference(), field);

        public ReferenceExpression CreateEvaluationContextReference() => 
            this.CodeBuilder.CreateReference(this._evaluationContextLocal);

        public MethodInvocationExpression CreateEvaluatorInvocation(MethodInvocationExpression node) => 
            this.CodeBuilder.CreateMethodInvocation(Evaluator_Eval, this.CreateEvaluationContextReference(), node.Arguments[0]);

        public IType DefineEvaluationContext()
        {
            string[] components = new string[] { "EvaluationContext" };
            BooClassBuilder builder = this.CodeBuilder.CreateClass(this.Context.GetUniqueName(components), TypeMemberModifiers.Public);
            builder.AddBaseType(this.Map(typeof(EvaluationContext)));
            this.ChainConstructorsFromBaseType(builder);
            this.AddLocalVariablesAsFields(builder);
            this.CurrentTypeNode.Members.Add(builder.ClassDefinition);
            return builder.Entity;
        }

        public Expression EvaluationDomainProviderReference() => 
            (!this._currentMethod.IsStatic ? ((Expression) this.CodeBuilder.CreateSelfReference(this.CurrentType)) : ((Expression) this.CodeBuilder.CreateReference(My<EvaluationDomainProviderImplementor>.Instance.StaticEvaluationDomainProviderFor((ClassDefinition) this.CurrentTypeNode))));

        public InternalField GetEvaluationContextField(Node node)
        {
            object obj1 = node["EvaluationContextField"];
            if (!(obj1 is InternalField))
            {
            }
            return (InternalField) RuntimeServices.Coerce(obj1, typeof(InternalField));
        }

        public bool IsEvalInvocation(MethodInvocationExpression node) => 
            (node.Target.Entity == UnityScriptTypeSystem.UnityScriptEval);

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
                    this.Error(node, UnityScriptCompilerErrors.EvalHasBeenDisabled(node.Target.LexicalInfo, this.UnityScriptParameters.DisableEval));
                }
                else
                {
                    this.ReplaceEvalByEvaluatorEval(node);
                }
            }
        }

        public IType Map(Type type) => 
            this.TypeSystemServices.Map(type);

        public override void OnConstructor(Constructor node)
        {
            this.OnMethod(node);
        }

        public override void OnMethod(Method node)
        {
            if (EvalAnnotation.IsMarked(node))
            {
                this._currentMethod = (InternalMethod) this.GetEntity(node);
                IType evaluationContextType = this.DefineEvaluationContext();
                Block stmt = this.PrepareEvaluationContextInitialization(evaluationContextType);
                this.Visit(node.Body);
                node.Body.Insert(0, stmt);
            }
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            IInternalEntity entity = node.Entity as IInternalEntity;
            if (entity != null)
            {
                InternalField evaluationContextField = this.GetEvaluationContextField(entity.Node);
                if (evaluationContextField != null)
                {
                    node.ParentNode.Replace(node, this.CreateEvaluationContextFieldReference(evaluationContextField));
                }
            }
        }

        public Block PrepareEvaluationContextInitialization(IType evaluationContextType)
        {
            this._evaluationContextLocal = this.CodeBuilder.DeclareTempLocal(this.CurrentMethodNode, evaluationContextType);
            Block block = new Block();
            block.Add(this.CodeBuilder.CreateAssignment(this.CreateEvaluationContextReference(), this.CodeBuilder.CreateConstructorInvocation(UtilitiesModule.ConstructorTakingNArgumentsFor(evaluationContextType, 1), this.EvaluationDomainProviderReference())));
            foreach (ParameterDeclaration declaration in this.CurrentMethodNode.Parameters)
            {
                block.Add(this.CodeBuilder.CreateAssignment(this.CreateEvaluationContextFieldReference(this.GetEvaluationContextField(declaration)), this.CodeBuilder.CreateReference(declaration)));
            }
            return block;
        }

        public void ReplaceEvalByEvaluatorEval(MethodInvocationExpression node)
        {
            node.ParentNode.Replace(node, this.CreateEvaluatorInvocation(node));
        }

        public override void Run()
        {
            if (this.Errors.Count <= 0)
            {
                this.Visit(this.CompileUnit);
            }
        }

        public void SetEvaluationContextField(Node node, InternalField field)
        {
            node["EvaluationContextField"] = field;
        }

        public Block CurrentMethodBody =>
            this.CurrentMethodNode.Body;

        public Method CurrentMethodNode =>
            this._currentMethod.Method;

        public IType CurrentType =>
            this._currentMethod.DeclaringType;

        public TypeDefinition CurrentTypeNode =>
            this.CurrentMethodNode.DeclaringType;

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) base._context.Parameters);
    }
}

