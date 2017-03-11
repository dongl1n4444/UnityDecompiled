namespace UnityScript.Steps
{
    using Boo.Lang;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Runtime;
    using System;
    using System.Linq;
    using UnityScript;
    using UnityScript.Core;

    [Serializable]
    public class ApplySemantics : AbstractVisitorCompilerStep
    {
        internal bool $ModuleContainsOnlyTypeDefinitions$closure$129(TypeMember m) => 
            (m is TypeDefinition);

        public void AddScriptBaseType(ClassDefinition klass)
        {
            klass.BaseTypes.Add(this.CodeBuilder.CreateTypeReference(this.UnityScriptParameters.ScriptBaseType));
        }

        public Constructor ConstructorFromMethod(Method method)
        {
            Constructor constructor;
            Constructor constructor1 = constructor = new Constructor();
            ParameterDeclarationCollection collection1 = constructor.Parameters = method.Parameters;
            Block block1 = constructor.Body = method.Body;
            AttributeCollection collection2 = constructor.Attributes = method.Attributes;
            TypeMemberModifiers modifiers1 = constructor.Modifiers = method.Modifiers;
            LexicalInfo info1 = constructor.LexicalInfo = method.LexicalInfo;
            SourceLocation location1 = constructor.EndSourceLocation = method.EndSourceLocation;
            return constructor;
        }

        public Method ExistingMainMethodOn(TypeDefinition typeDef) => 
            typeDef.Members.OfType<Method>().FirstOrDefault<Method>(new Func<Method, bool>(this.IsMainMethod));

        public ClassDefinition FindOrCreateScriptClass(Module module)
        {
            ClassDefinition definition2;
            foreach (ClassDefinition definition in module.Members.OfType<ClassDefinition>())
            {
                if (definition.Name == module.Name)
                {
                    module.Members.Remove(definition);
                    if (definition.IsPartial)
                    {
                        this.AddScriptBaseType(definition);
                    }
                    definition.Annotate("UserDefined");
                    return definition;
                }
            }
            ClassDefinition definition1 = definition2 = new ClassDefinition(module.LexicalInfo);
            string text1 = definition2.Name = module.Name;
            SourceLocation location1 = definition2.EndSourceLocation = module.EndSourceLocation;
            int num1 = (int) (definition2.IsSynthetic = true);
            ClassDefinition klass = definition2;
            this.AddScriptBaseType(klass);
            return klass;
        }

        public bool IsConstructorMethod(TypeDefinition toType, TypeMember member) => 
            ((member.NodeType == NodeType.Method) ? (toType.Name == member.Name) : false);

        public bool IsMainMethod(Method m)
        {
            if (m == null)
            {
            }
            bool flag2 = m.Name == this.ScriptMainMethod;
            if (!flag2)
            {
                return flag2;
            }
            return (m.Parameters.Count == 0);
        }

        public void MakeItPartial(ClassDefinition global)
        {
            global.Modifiers |= TypeMemberModifiers.Partial;
        }

        public bool ModuleContainsOnlyTypeDefinitions(Module module)
        {
            if (module.Members.IsEmpty)
            {
            }
            if (!module.Members.All<TypeMember>(new Func<TypeMember, bool>(this.$ModuleContainsOnlyTypeDefinitions$closure$129)))
            {
            }
            bool isEmpty = module.Globals.IsEmpty;
            if (!isEmpty)
            {
                return isEmpty;
            }
            return module.Attributes.IsEmpty;
        }

        public void MoveAttributes(TypeDefinition fromType, TypeDefinition toType)
        {
            toType.Attributes.AddRange(fromType.Attributes);
            fromType.Attributes.Clear();
        }

        public TypeMember MovedMember(TypeDefinition toType, TypeMember member) => 
            (toType.ContainsAnnotation("UserDefined") ? (this.IsConstructorMethod(toType, member) ? this.ConstructorFromMethod((Method) member) : member) : member);

        public void MoveGlobalStatementsToMainMethodOf(TypeDefinition script, Module module)
        {
            Method item = this.ExistingMainMethodOn(script);
            if (item == null)
            {
                item = this.NewMainMethodFor(module);
                script.Members.Add(item);
            }
            else
            {
                this.Warnings.Add(UnityScriptWarnings.ScriptMainMethodIsImplicitlyDefined(item.LexicalInfo, item.Name));
            }
            item.Body.Statements.AddRange(module.Globals.Statements);
            module.Globals.Statements.Clear();
        }

        public void MoveMembers(TypeDefinition fromType, TypeDefinition toType)
        {
            List list = new List();
            foreach (TypeMember member in fromType.Members)
            {
                if (!(member is TypeDefinition))
                {
                    TypeMember item = this.MovedMember(toType, member);
                    toType.Members.Add(item);
                    this.Visit(item);
                    list.Add(member);
                }
                else
                {
                    this.Visit(member);
                }
            }
            foreach (object obj2 in list)
            {
                if (!(obj2 is TypeMember))
                {
                }
                fromType.Members.Remove((TypeMember) RuntimeServices.Coerce(obj2, typeof(TypeMember)));
            }
        }

        public Method NewMainMethodFor(Module module)
        {
            Method method;
            Method method1 = method = new Method();
            LexicalInfo info1 = method.LexicalInfo = module.Globals.IsEmpty ? module.LexicalInfo : ApplySemanticsModule.Copy(module.Globals.Statements[0].LexicalInfo);
            string text1 = method.Name = this.ScriptMainMethod;
            int num1 = (int) (method.Modifiers = TypeMemberModifiers.Virtual | TypeMemberModifiers.Public);
            SourceLocation location1 = method.EndSourceLocation = module.EndSourceLocation;
            return method;
        }

        public override void OnModule(Module module)
        {
            this.SetUpDefaultImports(module);
            if (!this.ModuleContainsOnlyTypeDefinitions(module))
            {
                ClassDefinition global = this.FindOrCreateScriptClass(module);
                this.MakeItPartial(global);
                this.MoveMembers(module, global);
                this.SetUpMainMethod(module, global);
                this.MoveAttributes(module, global);
                module.Members.Add(global);
                UtilitiesModule.SetScriptClass(this.Context, global);
            }
        }

        public override void Run()
        {
            this.Visit(this.CompileUnit);
        }

        public void SetUpDefaultImports(Module module)
        {
            foreach (string str in this.UnityScriptParameters.Imports)
            {
                module.Imports.Add(new Import(str));
            }
        }

        public void SetUpMainMethod(Module module, TypeDefinition script)
        {
            this.TransformGlobalVariablesIntoFields(module, script);
            this.MoveGlobalStatementsToMainMethodOf(script, module);
        }

        public void TransformGlobalVariablesIntoFields(Module module, TypeDefinition script)
        {
            if (this.UnityScriptParameters.GlobalVariablesBecomeFields)
            {
                module.Globals.Accept(new DeclareGlobalVariables((ClassDefinition) script));
            }
        }

        public string ScriptMainMethod =>
            this.UnityScriptParameters.ScriptMainMethod;

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) base._context.Parameters);

        [Serializable]
        public class DeclareGlobalVariables : DepthFirstTransformer
        {
            protected ClassDefinition _class;

            public DeclareGlobalVariables(ClassDefinition cd)
            {
                this._class = cd;
            }

            public override bool EnterBlock(Block node) => 
                (node.ParentNode is Method);

            public override void OnDeclarationStatement(DeclarationStatement node)
            {
                if (!node.ContainsAnnotation("PrivateScope"))
                {
                    Field field;
                    Field field1 = field = new Field(LexicalInfo.Empty);
                    int num1 = (int) (field.Modifiers = TypeMemberModifiers.Public);
                    string text1 = field.Name = "$";
                    TypeReference reference1 = field.Type = TypeReference.Lift(node.Declaration.Type);
                    Expression expression1 = field.Initializer = Expression.Lift(node.Initializer);
                    int num2 = (int) (field.IsVolatile = false);
                    string text2 = field.Name = CodeSerializer.LiftName(node.Declaration.Name);
                    Field item = field;
                    item.LexicalInfo = node.LexicalInfo;
                    this._class.Members.Add(item);
                    this.RemoveCurrentNode();
                }
            }
        }
    }
}

