namespace UnityScript.Steps
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Internal;
    using Boo.Lang.Environments;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections;
    using System.Reflection;
    using UnityScript;
    using UnityScript.Core;
    using UnityScript.Lang;
    using UnityScript.TypeSystem;

    [Serializable]
    public class ProcessUnityScriptMethods : ProcessMethodBodiesWithDuckTyping
    {
        private bool ___get__StartCoroutine_cached;
        private object ___get__StartCoroutine_lock = new object();
        private IMethod ___get__StartCoroutine_returnValue;
        private bool ___get__UnityRuntimeServices_GetEnumerator_cached;
        private object ___get__UnityRuntimeServices_GetEnumerator_lock = new object();
        private IMethod ___get__UnityRuntimeServices_GetEnumerator_returnValue;
        private bool ___get__UnityRuntimeServices_GetTypeOf_cached;
        private object ___get__UnityRuntimeServices_GetTypeOf_lock = new object();
        private IMethod ___get__UnityRuntimeServices_GetTypeOf_returnValue;
        private bool ___get__UnityRuntimeServices_Update_cached;
        private object ___get__UnityRuntimeServices_Update_lock = new object();
        private IMethod ___get__UnityRuntimeServices_Update_returnValue;
        private bool ___get_EmptyEnumeratorReference_cached;
        private object ___get_EmptyEnumeratorReference_lock = new object();
        private MemberReferenceExpression ___get_EmptyEnumeratorReference_returnValue;
        private bool ___get_IEnumerable_GetEnumerator_cached;
        private object ___get_IEnumerable_GetEnumerator_lock = new object();
        private MethodInfo ___get_IEnumerable_GetEnumerator_returnValue;
        private bool ___get_IEnumerator_get_Current_cached;
        private object ___get_IEnumerator_get_Current_lock = new object();
        private MethodInfo ___get_IEnumerator_get_Current_returnValue;
        private bool ___get_IEnumerator_MoveNext_cached;
        private object ___get_IEnumerator_MoveNext_lock = new object();
        private MethodInfo ___get_IEnumerator_MoveNext_returnValue;
        private bool ___get_UnityRuntimeServicesType_cached;
        private object ___get_UnityRuntimeServicesType_lock = new object();
        private IType ___get_UnityRuntimeServicesType_returnValue;
        protected Module _activeModule;
        protected bool _implicit;

        public void ApplyImplicitArrayConversion(BinaryExpression node)
        {
            IType expressionType = this.GetExpressionType(node.get_Left());
            if (expressionType.get_IsArray())
            {
                IType type2 = this.GetExpressionType(node.get_Right());
                if (type2 == this.UnityScriptLangArray())
                {
                    node.set_Right(this.get_CodeBuilder().CreateCast(expressionType, this.get_CodeBuilder().CreateMethodInvocation(node.get_Right(), this.ResolveMethod(type2, "ToBuiltin"), this.get_CodeBuilder().CreateTypeofExpression(expressionType.get_ElementType()))));
                }
            }
        }

        public void CheckEntryPoint(Method node)
        {
            if ((node.get_IsStatic() && node.get_IsPublic()) && ((node.get_Name() == "Main") && (this.GetType(node.get_ReturnType()) == this.get_TypeSystemServices().VoidType)))
            {
                ContextAnnotations.SetEntryPoint(base._context, node);
            }
        }

        public void CheckForEmptyCoroutine(Method node)
        {
            if (this.IsEmptyCoroutine(node))
            {
                ReturnStatement statement;
                ReturnStatement statement1 = statement = new ReturnStatement(LexicalInfo.Empty);
                statement.set_Expression(Expression.Lift(this.EmptyEnumeratorReference));
                node.get_Body().Add(statement);
            }
        }

        public override IType GetGeneratorReturnType(InternalMethod generator)
        {
            return this.get_TypeSystemServices().IEnumeratorType;
        }

        public override void Initialize(CompilerContext context)
        {
            base.Initialize(context);
            this.set_OptimizeNullComparisons(false);
        }

        public bool IsCompilerGenerated(ReferenceExpression reference)
        {
            return reference.get_Name().Contains("$");
        }

        public bool IsEmptyCoroutine(Method node)
        {
            InternalMethod entity = this.GetEntity(node);
            bool flag1 = entity.get_ReturnType() == this.GetGeneratorReturnType(entity);
            if (!flag1)
            {
                return flag1;
            }
            return this.HasNeitherReturnNorYield(node);
        }

        protected override Local LocalToReuseFor(Declaration d)
        {
            if (DeclarationAnnotations.ShouldForceNewVariableFor(d))
            {
                this.AssertUniqueLocal(d);
                return null;
            }
            return base.LocalToReuseFor(d);
        }

        protected override void MemberNotFound(MemberReferenceExpression node, INamespace ns)
        {
            if (this.Strict)
            {
                base.MemberNotFound(node, ns);
            }
            else
            {
                this.BindQuack(node);
            }
        }

        public bool NeedsUpdateableIteration(ForStatement node)
        {
            return !this.GetExpressionType(node.get_Iterator()).get_IsArray();
        }

        public override void OnForStatement(ForStatement node)
        {
            if (1 != node.get_Declarations().Count)
            {
                throw new AssertionFailedException("1 == len(node.Declarations)");
            }
            this.Visit(node.get_Iterator());
            if (this.NeedsUpdateableIteration(node))
            {
                this.ProcessUpdateableIteration(node);
            }
            else
            {
                this.ProcessNormalIteration(node);
            }
        }

        public override void OnMethod(Method node)
        {
            base.OnMethod(node);
            this.CheckForEmptyCoroutine(node);
            if (this.get_Parameters().get_OutputType() != 1)
            {
                this.CheckEntryPoint(node);
            }
        }

        public override void OnModule(Module module)
        {
            this.ActiveModule = module;
            base.OnModule(module);
        }

        public override void ProcessAutoLocalDeclaration(BinaryExpression node, ReferenceExpression reference)
        {
            if ((this.Strict && !this._implicit) && !this.IsCompilerGenerated(reference))
            {
                this.EmitUnknownIdentifierError(reference, reference.get_Name());
            }
            else
            {
                base.ProcessAutoLocalDeclaration(node, reference);
            }
        }

        protected override void ProcessBuiltinInvocation(MethodInvocationExpression node, BuiltinFunction function)
        {
            if (function == UnityScript.TypeSystem.UnityScriptTypeSystem.UnityScriptEval)
            {
                EvalAnnotation.Mark(this.get_CurrentMethod());
                this.BindExpressionType(node, this.get_TypeSystemServices().ObjectType);
            }
            else if (function == UnityScript.TypeSystem.UnityScriptTypeSystem.UnityScriptTypeof)
            {
                this.ProcessTypeofBuiltin(node);
            }
            else
            {
                base.ProcessBuiltinInvocation(node, function);
            }
        }

        protected override void ProcessMethodInvocation(MethodInvocationExpression node, IMethod method)
        {
            base.ProcessMethodInvocation(node, method);
            if (UtilitiesModule.IsPossibleStartCoroutineInvocationForm(node))
            {
                UnityScript.TypeSystem.UnityScriptTypeSystem unityScriptTypeSystem = this.UnityScriptTypeSystem;
                if (unityScriptTypeSystem.IsScriptType(method.get_DeclaringType()) && unityScriptTypeSystem.IsGenerator(method))
                {
                    if (this.get_CurrentMethod().get_IsStatic())
                    {
                        this.get_Warnings().Add(UnityScriptWarnings.CannotStartCoroutineFromStaticFunction(node.get_LexicalInfo(), method.get_Name()));
                    }
                    else
                    {
                        node.get_ParentNode().Replace(node, this.get_CodeBuilder().CreateMethodInvocation(this.get_CodeBuilder().CreateSelfReference(node.get_LexicalInfo(), this.get_CurrentType()), this._StartCoroutine, node));
                    }
                }
            }
        }

        public void ProcessNormalIteration(ForStatement node)
        {
            node.set_Iterator(this.ProcessIterator(node.get_Iterator(), node.get_Declarations()));
            this.VisitForStatementBlock(node);
        }

        public override void ProcessStaticallyTypedAssignment(BinaryExpression node)
        {
            this.TryToResolveAmbiguousAssignment(node);
            this.ApplyImplicitArrayConversion(node);
            if (this.ValidateAssignment(node))
            {
                this.BindExpressionType(node, this.GetExpressionType(node.get_Right()));
            }
            else
            {
                this.Error(node);
            }
        }

        private void ProcessTypeofBuiltin(MethodInvocationExpression node)
        {
            if (node.get_Arguments().get_Count() != 1)
            {
                this.Error(node, new CompilerError("UCE0001", node.get_Target().get_LexicalInfo(), "'typeof' takes a single argument.", null));
            }
            else
            {
                IType type = node.get_Arguments().get_Item(0).get_Entity() as IType;
                if (type != null)
                {
                    node.get_ParentNode().Replace(node, this.get_CodeBuilder().CreateTypeofExpression(type));
                }
                else
                {
                    node.set_Target(this.get_CodeBuilder().CreateReference(this._UnityRuntimeServices_GetTypeOf));
                    this.BindExpressionType(node, this.get_TypeSystemServices().TypeType);
                }
            }
        }

        public void ProcessUpdateableIteration(ForStatement node)
        {
            Expression expression = node.get_Iterator();
            MethodInvocationExpression expression2 = this.get_CodeBuilder().CreateMethodInvocation(this._UnityRuntimeServices_GetEnumerator, node.get_Iterator());
            expression2.set_LexicalInfo(new LexicalInfo(node.get_Iterator().get_LexicalInfo()));
            node.set_Iterator(expression2);
            this.ProcessDeclarationForIterator(node.get_Declarations().get_Item(0), this.GetEnumeratorItemType(this.GetExpressionType(expression)));
            this.VisitForStatementBlock(node);
            this.TransformIteration(node);
        }

        public IField ResolveUnityRuntimeField(string name)
        {
            return this.get_NameResolutionService().ResolveField(this.UnityRuntimeServicesType, name);
        }

        public IMethod ResolveUnityRuntimeMethod(string name)
        {
            return this.get_NameResolutionService().ResolveMethod(this.UnityRuntimeServicesType, name);
        }

        public bool ShouldDisableImplicitDowncastWarning()
        {
            bool flag1 = !this.get_Parameters().get_Strict();
            if (flag1)
            {
                return flag1;
            }
            return this.ActiveModule.ContainsAnnotation("downcast");
        }

        public void TransformIteration(ForStatement node)
        {
            string[] textArray1 = new string[] { "iterator" };
            InternalLocal iteratorVariable = this.get_CodeBuilder().DeclareLocal(this.get_CurrentMethod(), base._context.GetUniqueName(textArray1), this.get_TypeSystemServices().IEnumeratorType);
            iteratorVariable.set_IsUsed(true);
            Block block = new Block(node.get_LexicalInfo());
            block.Add(this.get_CodeBuilder().CreateAssignment(node.get_LexicalInfo(), this.get_CodeBuilder().CreateReference(iteratorVariable), node.get_Iterator()));
            WhileStatement statement = new WhileStatement(node.get_LexicalInfo());
            statement.set_Condition(this.get_CodeBuilder().CreateMethodInvocation(this.get_CodeBuilder().CreateReference(iteratorVariable), this.IEnumerator_MoveNext));
            MethodInvocationExpression expression = this.get_CodeBuilder().CreateMethodInvocation(this.get_CodeBuilder().CreateReference(iteratorVariable), this.IEnumerator_get_Current);
            InternalLocal entity = TypeSystemServices.GetEntity(node.get_Declarations().get_Item(0));
            statement.get_Block().Add(this.get_CodeBuilder().CreateAssignment(node.get_LexicalInfo(), this.get_CodeBuilder().CreateReference(entity), expression));
            statement.get_Block().Add(node.get_Block());
            new LoopVariableUpdater(this, base._context, iteratorVariable, entity).Visit(node);
            block.Add(statement);
            node.get_ParentNode().Replace(node, block);
        }

        public IType UnityScriptLangArray()
        {
            return this.get_TypeSystemServices().Map(typeof(Array));
        }

        public void UpdateSettingsForActiveModule()
        {
            this.get_Parameters().set_Strict(Pragmas.IsEnabledOn(this.ActiveModule, "strict"));
            this._implicit = Pragmas.IsEnabledOn(this.ActiveModule, "implicit");
            My<UnityDowncastPermissions>.Instance.Enabled = Pragmas.IsEnabledOn(this.ActiveModule, "downcast");
            if (this.ShouldDisableImplicitDowncastWarning())
            {
                this.get_Parameters().DisableWarning(this.ImplicitDowncast);
            }
            else
            {
                this.get_Parameters().EnableWarning(this.ImplicitDowncast);
            }
        }

        public override void VisitMemberPreservingContext(TypeMember node)
        {
            Module module = node.get_EnclosingModule();
            if (module == this.ActiveModule)
            {
                base.VisitMemberPreservingContext(node);
            }
            else
            {
                Module activeModule = this.ActiveModule;
                try
                {
                    this.ActiveModule = module;
                    base.VisitMemberPreservingContext(node);
                }
                finally
                {
                    this.ActiveModule = activeModule;
                }
            }
        }

        public IMethod _StartCoroutine
        {
            get
            {
                if (!this.___get__StartCoroutine_cached)
                {
                    lock (this.___get__StartCoroutine_lock)
                    {
                        if (!this.___get__StartCoroutine_cached)
                        {
                            Type[] typeArray1 = new Type[] { typeof(IEnumerator) };
                            this.___get__StartCoroutine_returnValue = this.get_NameResolutionService().ResolveMethod(this.UnityScriptTypeSystem.ScriptBaseType, "StartCoroutine", typeArray1);
                            this.___get__StartCoroutine_cached = true;
                        }
                    }
                }
                return this.___get__StartCoroutine_returnValue;
            }
        }

        public IMethod _UnityRuntimeServices_GetEnumerator
        {
            get
            {
                if (!this.___get__UnityRuntimeServices_GetEnumerator_cached)
                {
                    lock (this.___get__UnityRuntimeServices_GetEnumerator_lock)
                    {
                        if (!this.___get__UnityRuntimeServices_GetEnumerator_cached)
                        {
                            this.___get__UnityRuntimeServices_GetEnumerator_returnValue = this.ResolveUnityRuntimeMethod("GetEnumerator");
                            this.___get__UnityRuntimeServices_GetEnumerator_cached = true;
                        }
                    }
                }
                return this.___get__UnityRuntimeServices_GetEnumerator_returnValue;
            }
        }

        public IMethod _UnityRuntimeServices_GetTypeOf
        {
            get
            {
                if (!this.___get__UnityRuntimeServices_GetTypeOf_cached)
                {
                    lock (this.___get__UnityRuntimeServices_GetTypeOf_lock)
                    {
                        if (!this.___get__UnityRuntimeServices_GetTypeOf_cached)
                        {
                            this.___get__UnityRuntimeServices_GetTypeOf_returnValue = this.ResolveUnityRuntimeMethod("GetTypeOf");
                            this.___get__UnityRuntimeServices_GetTypeOf_cached = true;
                        }
                    }
                }
                return this.___get__UnityRuntimeServices_GetTypeOf_returnValue;
            }
        }

        public IMethod _UnityRuntimeServices_Update
        {
            get
            {
                if (!this.___get__UnityRuntimeServices_Update_cached)
                {
                    lock (this.___get__UnityRuntimeServices_Update_lock)
                    {
                        if (!this.___get__UnityRuntimeServices_Update_cached)
                        {
                            this.___get__UnityRuntimeServices_Update_returnValue = this.ResolveUnityRuntimeMethod("Update");
                            this.___get__UnityRuntimeServices_Update_cached = true;
                        }
                    }
                }
                return this.___get__UnityRuntimeServices_Update_returnValue;
            }
        }

        public Module ActiveModule
        {
            get
            {
                return this._activeModule;
            }
            set
            {
                this._activeModule = value;
                this.UpdateSettingsForActiveModule();
            }
        }

        public MemberReferenceExpression EmptyEnumeratorReference
        {
            get
            {
                if (!this.___get_EmptyEnumeratorReference_cached)
                {
                    lock (this.___get_EmptyEnumeratorReference_lock)
                    {
                        if (!this.___get_EmptyEnumeratorReference_cached)
                        {
                            this.___get_EmptyEnumeratorReference_returnValue = this.get_CodeBuilder().CreateMemberReference(this.ResolveUnityRuntimeField("EmptyEnumerator"));
                            this.___get_EmptyEnumeratorReference_cached = true;
                        }
                    }
                }
                return this.___get_EmptyEnumeratorReference_returnValue;
            }
        }

        public MethodInfo IEnumerable_GetEnumerator
        {
            get
            {
                if (!this.___get_IEnumerable_GetEnumerator_cached)
                {
                    lock (this.___get_IEnumerable_GetEnumerator_lock)
                    {
                        if (!this.___get_IEnumerable_GetEnumerator_cached)
                        {
                            this.___get_IEnumerable_GetEnumerator_returnValue = typeof(IEnumerable).GetMethod("GetEnumerator");
                            this.___get_IEnumerable_GetEnumerator_cached = true;
                        }
                    }
                }
                return this.___get_IEnumerable_GetEnumerator_returnValue;
            }
        }

        public MethodInfo IEnumerator_get_Current
        {
            get
            {
                if (!this.___get_IEnumerator_get_Current_cached)
                {
                    lock (this.___get_IEnumerator_get_Current_lock)
                    {
                        if (!this.___get_IEnumerator_get_Current_cached)
                        {
                            this.___get_IEnumerator_get_Current_returnValue = typeof(IEnumerator).GetProperty("Current").GetGetMethod();
                            this.___get_IEnumerator_get_Current_cached = true;
                        }
                    }
                }
                return this.___get_IEnumerator_get_Current_returnValue;
            }
        }

        public MethodInfo IEnumerator_MoveNext
        {
            get
            {
                if (!this.___get_IEnumerator_MoveNext_cached)
                {
                    lock (this.___get_IEnumerator_MoveNext_lock)
                    {
                        if (!this.___get_IEnumerator_MoveNext_cached)
                        {
                            this.___get_IEnumerator_MoveNext_returnValue = typeof(IEnumerator).GetMethod("MoveNext");
                            this.___get_IEnumerator_MoveNext_cached = true;
                        }
                    }
                }
                return this.___get_IEnumerator_MoveNext_returnValue;
            }
        }

        public string ImplicitDowncast
        {
            get
            {
                return "BCW0028";
            }
        }

        public bool Strict
        {
            get
            {
                return this.get_Parameters().get_Strict();
            }
        }

        public IType UnityRuntimeServicesType
        {
            get
            {
                if (!this.___get_UnityRuntimeServicesType_cached)
                {
                    lock (this.___get_UnityRuntimeServicesType_lock)
                    {
                        if (!this.___get_UnityRuntimeServicesType_cached)
                        {
                            this.___get_UnityRuntimeServicesType_returnValue = this.get_TypeSystemServices().Map(typeof(UnityRuntimeServices));
                            this.___get_UnityRuntimeServicesType_cached = true;
                        }
                    }
                }
                return this.___get_UnityRuntimeServicesType_returnValue;
            }
        }

        public UnityScriptCompilerParameters UnityScriptParameters
        {
            get
            {
                return (UnityScriptCompilerParameters) base._context.get_Parameters();
            }
        }

        public UnityScript.TypeSystem.UnityScriptTypeSystem UnityScriptTypeSystem
        {
            get
            {
                return (UnityScript.TypeSystem.UnityScriptTypeSystem) this.get_TypeSystemServices();
            }
        }

        [Serializable]
        public class LoopVariableUpdater : DepthFirstVisitor
        {
            protected CompilerContext _context;
            protected bool _found;
            protected IEntity _iteratorVariable;
            protected IEntity _loopVariable;
            protected ProcessUnityScriptMethods _parent;

            public LoopVariableUpdater(ProcessUnityScriptMethods parent, CompilerContext context, IEntity iteratorVariable, IEntity loopVariable)
            {
                this._parent = parent;
                this._context = context;
                this._iteratorVariable = iteratorVariable;
                this._loopVariable = loopVariable;
            }

            public override void OnExpressionStatement(ExpressionStatement node)
            {
                this._found = false;
                this.Visit(node.get_Expression());
                if (this._found)
                {
                    Node node2 = node.get_ParentNode();
                    BooCodeBuilder builder = this._context.get_CodeBuilder();
                    Block block = new Block(node.get_LexicalInfo());
                    block.Add(node);
                    block.Add(builder.CreateMethodInvocation(this._parent._UnityRuntimeServices_Update, builder.CreateReference(this._iteratorVariable), builder.CreateReference(this._loopVariable)));
                    node2.Replace(node, block);
                }
            }

            public override void OnReferenceExpression(ReferenceExpression node)
            {
                if (!this._found)
                {
                    IEntity entity = node.get_Entity();
                    this._found = entity == this._loopVariable;
                }
            }
        }
    }
}

