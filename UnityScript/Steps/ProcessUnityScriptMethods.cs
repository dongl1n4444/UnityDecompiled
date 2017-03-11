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
        protected Boo.Lang.Compiler.Ast.Module _activeModule;
        protected bool _implicit;

        public void ApplyImplicitArrayConversion(BinaryExpression node)
        {
            IType expressionType = this.GetExpressionType(node.Left);
            if (expressionType.IsArray)
            {
                IType type = this.GetExpressionType(node.Right);
                if (type == this.UnityScriptLangArray())
                {
                    node.Right = this.CodeBuilder.CreateCast(expressionType, this.CodeBuilder.CreateMethodInvocation(node.Right, this.ResolveMethod(type, "ToBuiltin"), this.CodeBuilder.CreateTypeofExpression(expressionType.ElementType)));
                }
            }
        }

        public void CheckEntryPoint(Method node)
        {
            if ((node.IsStatic && node.IsPublic) && ((node.Name == "Main") && (this.GetType(node.ReturnType) == this.TypeSystemServices.VoidType)))
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
                Expression expression1 = statement.Expression = Expression.Lift(this.EmptyEnumeratorReference);
                node.Body.Add(statement);
            }
        }

        public override IType GetGeneratorReturnType(InternalMethod generator) => 
            this.TypeSystemServices.IEnumeratorType;

        public override void Initialize(CompilerContext context)
        {
            base.Initialize(context);
            this.OptimizeNullComparisons = false;
        }

        public bool IsCompilerGenerated(ReferenceExpression reference) => 
            reference.Name.Contains("$");

        public bool IsEmptyCoroutine(Method node)
        {
            InternalMethod entity = (InternalMethod) this.GetEntity(node);
            bool flag1 = entity.ReturnType == this.GetGeneratorReturnType(entity);
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

        public bool NeedsUpdateableIteration(ForStatement node) => 
            !this.GetExpressionType(node.Iterator).IsArray;

        public override void OnForStatement(ForStatement node)
        {
            if (1 != node.Declarations.Count)
            {
                throw new AssertionFailedException("1 == len(node.Declarations)");
            }
            this.Visit(node.Iterator);
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
            if (this.Parameters.OutputType != CompilerOutputType.Library)
            {
                this.CheckEntryPoint(node);
            }
        }

        public override void OnModule(Boo.Lang.Compiler.Ast.Module module)
        {
            this.ActiveModule = module;
            base.OnModule(module);
        }

        public override void ProcessAutoLocalDeclaration(BinaryExpression node, ReferenceExpression reference)
        {
            if ((this.Strict && !this._implicit) && !this.IsCompilerGenerated(reference))
            {
                this.EmitUnknownIdentifierError(reference, reference.Name);
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
                EvalAnnotation.Mark(this.CurrentMethod);
                this.BindExpressionType(node, this.TypeSystemServices.ObjectType);
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
                if (unityScriptTypeSystem.IsScriptType(method.DeclaringType) && unityScriptTypeSystem.IsGenerator(method))
                {
                    if (this.CurrentMethod.IsStatic)
                    {
                        this.Warnings.Add(UnityScriptWarnings.CannotStartCoroutineFromStaticFunction(node.LexicalInfo, method.Name));
                    }
                    else
                    {
                        node.ParentNode.Replace(node, this.CodeBuilder.CreateMethodInvocation(this.CodeBuilder.CreateSelfReference(node.LexicalInfo, this.CurrentType), this._StartCoroutine, node));
                    }
                }
            }
        }

        public void ProcessNormalIteration(ForStatement node)
        {
            node.Iterator = this.ProcessIterator(node.Iterator, node.Declarations);
            this.VisitForStatementBlock(node);
        }

        public override void ProcessStaticallyTypedAssignment(BinaryExpression node)
        {
            this.TryToResolveAmbiguousAssignment(node);
            this.ApplyImplicitArrayConversion(node);
            if (this.ValidateAssignment(node))
            {
                this.BindExpressionType(node, this.GetExpressionType(node.Right));
            }
            else
            {
                this.Error(node);
            }
        }

        private void ProcessTypeofBuiltin(MethodInvocationExpression node)
        {
            if (node.Arguments.Count != 1)
            {
                this.Error(node, new CompilerError("UCE0001", node.Target.LexicalInfo, "'typeof' takes a single argument.", null));
            }
            else
            {
                IType entity = node.Arguments[0].Entity as IType;
                if (entity != null)
                {
                    node.ParentNode.Replace(node, this.CodeBuilder.CreateTypeofExpression(entity));
                }
                else
                {
                    node.Target = this.CodeBuilder.CreateReference(this._UnityRuntimeServices_GetTypeOf);
                    this.BindExpressionType(node, this.TypeSystemServices.TypeType);
                }
            }
        }

        public void ProcessUpdateableIteration(ForStatement node)
        {
            Expression iterator = node.Iterator;
            MethodInvocationExpression expression2 = this.CodeBuilder.CreateMethodInvocation(this._UnityRuntimeServices_GetEnumerator, node.Iterator);
            expression2.LexicalInfo = new LexicalInfo(node.Iterator.LexicalInfo);
            node.Iterator = expression2;
            this.ProcessDeclarationForIterator(node.Declarations[0], this.GetEnumeratorItemType(this.GetExpressionType(iterator)));
            this.VisitForStatementBlock(node);
            this.TransformIteration(node);
        }

        public IField ResolveUnityRuntimeField(string name) => 
            this.NameResolutionService.ResolveField(this.UnityRuntimeServicesType, name);

        public IMethod ResolveUnityRuntimeMethod(string name) => 
            this.NameResolutionService.ResolveMethod(this.UnityRuntimeServicesType, name);

        public bool ShouldDisableImplicitDowncastWarning()
        {
            bool flag1 = !this.Parameters.Strict;
            if (flag1)
            {
                return flag1;
            }
            return this.ActiveModule.ContainsAnnotation("downcast");
        }

        public void TransformIteration(ForStatement node)
        {
            string[] components = new string[] { "iterator" };
            InternalLocal local = this.CodeBuilder.DeclareLocal(this.CurrentMethod, base._context.GetUniqueName(components), this.TypeSystemServices.IEnumeratorType);
            local.IsUsed = true;
            Block newNode = new Block(node.LexicalInfo);
            newNode.Add(this.CodeBuilder.CreateAssignment(node.LexicalInfo, this.CodeBuilder.CreateReference(local), node.Iterator));
            WhileStatement stmt = new WhileStatement(node.LexicalInfo) {
                Condition = this.CodeBuilder.CreateMethodInvocation(this.CodeBuilder.CreateReference(local), this.IEnumerator_MoveNext)
            };
            MethodInvocationExpression rhs = this.CodeBuilder.CreateMethodInvocation(this.CodeBuilder.CreateReference(local), this.IEnumerator_get_Current);
            InternalLocal entity = (InternalLocal) TypeSystemServices.GetEntity(node.Declarations[0]);
            stmt.Block.Add(this.CodeBuilder.CreateAssignment(node.LexicalInfo, this.CodeBuilder.CreateReference(entity), rhs));
            stmt.Block.Add(node.Block);
            new LoopVariableUpdater(this, base._context, local, entity).Visit(node);
            newNode.Add(stmt);
            node.ParentNode.Replace(node, newNode);
        }

        public IType UnityScriptLangArray() => 
            this.TypeSystemServices.Map(typeof(UnityScript.Lang.Array));

        public void UpdateSettingsForActiveModule()
        {
            this.Parameters.Strict = Pragmas.IsEnabledOn(this.ActiveModule, "strict");
            this._implicit = Pragmas.IsEnabledOn(this.ActiveModule, "implicit");
            My<UnityDowncastPermissions>.Instance.Enabled = Pragmas.IsEnabledOn(this.ActiveModule, "downcast");
            if (this.ShouldDisableImplicitDowncastWarning())
            {
                this.Parameters.DisableWarning(this.ImplicitDowncast);
            }
            else
            {
                this.Parameters.EnableWarning(this.ImplicitDowncast);
            }
        }

        public override void VisitMemberPreservingContext(TypeMember node)
        {
            Boo.Lang.Compiler.Ast.Module enclosingModule = node.EnclosingModule;
            if (enclosingModule == this.ActiveModule)
            {
                base.VisitMemberPreservingContext(node);
            }
            else
            {
                Boo.Lang.Compiler.Ast.Module activeModule = this.ActiveModule;
                try
                {
                    this.ActiveModule = enclosingModule;
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
                            Type[] paramTypes = new Type[] { typeof(IEnumerator) };
                            this.___get__StartCoroutine_returnValue = this.NameResolutionService.ResolveMethod(this.UnityScriptTypeSystem.ScriptBaseType, "StartCoroutine", paramTypes);
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

        public Boo.Lang.Compiler.Ast.Module ActiveModule
        {
            get => 
                this._activeModule;
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
                            this.___get_EmptyEnumeratorReference_returnValue = this.CodeBuilder.CreateMemberReference(this.ResolveUnityRuntimeField("EmptyEnumerator"));
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

        public string ImplicitDowncast =>
            "BCW0028";

        public bool Strict =>
            this.Parameters.Strict;

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
                            this.___get_UnityRuntimeServicesType_returnValue = this.TypeSystemServices.Map(typeof(UnityRuntimeServices));
                            this.___get_UnityRuntimeServicesType_cached = true;
                        }
                    }
                }
                return this.___get_UnityRuntimeServicesType_returnValue;
            }
        }

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) base._context.Parameters);

        public UnityScript.TypeSystem.UnityScriptTypeSystem UnityScriptTypeSystem =>
            ((UnityScript.TypeSystem.UnityScriptTypeSystem) this.TypeSystemServices);

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
                this.Visit(node.Expression);
                if (this._found)
                {
                    Node parentNode = node.ParentNode;
                    BooCodeBuilder codeBuilder = this._context.CodeBuilder;
                    Block newNode = new Block(node.LexicalInfo);
                    newNode.Add(node);
                    newNode.Add(codeBuilder.CreateMethodInvocation(this._parent._UnityRuntimeServices_Update, codeBuilder.CreateReference(this._iteratorVariable), codeBuilder.CreateReference(this._loopVariable)));
                    parentNode.Replace(node, newNode);
                }
            }

            public override void OnReferenceExpression(ReferenceExpression node)
            {
                if (!this._found)
                {
                    IEntity entity = node.Entity;
                    this._found = entity == this._loopVariable;
                }
            }
        }
    }
}

