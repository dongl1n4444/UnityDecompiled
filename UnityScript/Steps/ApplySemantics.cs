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
            klass.get_BaseTypes().Add(this.get_CodeBuilder().CreateTypeReference(this.UnityScriptParameters.ScriptBaseType));
        }

        public Constructor ConstructorFromMethod(Method method)
        {
            Constructor constructor;
            Constructor constructor1 = constructor = new Constructor();
            constructor.set_Parameters(method.get_Parameters());
            constructor.set_Body(method.get_Body());
            constructor.set_Attributes(method.get_Attributes());
            constructor.set_Modifiers(method.get_Modifiers());
            constructor.set_LexicalInfo(method.get_LexicalInfo());
            constructor.set_EndSourceLocation(method.get_EndSourceLocation());
            return constructor;
        }

        public Method ExistingMainMethodOn(TypeDefinition typeDef) => 
            typeDef.get_Members().OfType<Method>().FirstOrDefault<Method>(new Func<Method, bool>(this.IsMainMethod));

        public ClassDefinition FindOrCreateScriptClass(Module module)
        {
            ClassDefinition definition2;
            foreach (ClassDefinition definition in module.get_Members().OfType<ClassDefinition>())
            {
                if (definition.get_Name() == module.get_Name())
                {
                    module.get_Members().Remove(definition);
                    if (definition.get_IsPartial())
                    {
                        this.AddScriptBaseType(definition);
                    }
                    definition.Annotate("UserDefined");
                    return definition;
                }
            }
            ClassDefinition definition1 = definition2 = new ClassDefinition(module.get_LexicalInfo());
            definition2.set_Name(module.get_Name());
            definition2.set_EndSourceLocation(module.get_EndSourceLocation());
            definition2.set_IsSynthetic(true);
            ClassDefinition klass = definition2;
            this.AddScriptBaseType(klass);
            return klass;
        }

        public bool IsConstructorMethod(TypeDefinition toType, TypeMember member) => 
            ((member.get_NodeType() == 0x16) ? (toType.get_Name() == member.get_Name()) : false);

        public bool IsMainMethod(Method m)
        {
            if (m == null)
            {
            }
            bool flag2 = m.get_Name() == this.ScriptMainMethod;
            if (!flag2)
            {
                return flag2;
            }
            return (m.get_Parameters().Count == 0);
        }

        public void MakeItPartial(ClassDefinition global)
        {
            global.set_Modifiers(global.get_Modifiers() | 0x400);
        }

        public bool ModuleContainsOnlyTypeDefinitions(Module module)
        {
            if (module.get_Members().get_IsEmpty())
            {
            }
            if (!module.get_Members().All<TypeMember>(new Func<TypeMember, bool>(this.$ModuleContainsOnlyTypeDefinitions$closure$129)))
            {
            }
            bool flag3 = module.get_Globals().get_IsEmpty();
            if (!flag3)
            {
                return flag3;
            }
            return module.get_Attributes().get_IsEmpty();
        }

        public void MoveAttributes(TypeDefinition fromType, TypeDefinition toType)
        {
            toType.get_Attributes().AddRange(fromType.get_Attributes());
            fromType.get_Attributes().Clear();
        }

        public TypeMember MovedMember(TypeDefinition toType, TypeMember member) => 
            (toType.ContainsAnnotation("UserDefined") ? (this.IsConstructorMethod(toType, member) ? this.ConstructorFromMethod((Method) member) : member) : member);

        public void MoveGlobalStatementsToMainMethodOf(TypeDefinition script, Module module)
        {
            Method method = this.ExistingMainMethodOn(script);
            if (method == null)
            {
                method = this.NewMainMethodFor(module);
                script.get_Members().Add(method);
            }
            else
            {
                this.get_Warnings().Add(UnityScriptWarnings.ScriptMainMethodIsImplicitlyDefined(method.get_LexicalInfo(), method.get_Name()));
            }
            method.get_Body().get_Statements().AddRange(module.get_Globals().get_Statements());
            module.get_Globals().get_Statements().Clear();
        }

        public void MoveMembers(TypeDefinition fromType, TypeDefinition toType)
        {
            List list = new List();
            foreach (TypeMember member in fromType.get_Members())
            {
                if (!(member is TypeDefinition))
                {
                    TypeMember member2 = this.MovedMember(toType, member);
                    toType.get_Members().Add(member2);
                    this.Visit(member2);
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
                fromType.get_Members().Remove((TypeMember) RuntimeServices.Coerce(obj2, typeof(TypeMember)));
            }
        }

        public Method NewMainMethodFor(Module module)
        {
            Method method;
            Method method1 = method = new Method();
            method.set_LexicalInfo(module.get_Globals().get_IsEmpty() ? module.get_LexicalInfo() : ApplySemanticsModule.Copy(module.get_Globals().get_Statements().get_Item(0).get_LexicalInfo()));
            method.set_Name(this.ScriptMainMethod);
            method.set_Modifiers(0x88);
            method.set_EndSourceLocation(module.get_EndSourceLocation());
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
                module.get_Members().Add(global);
                UtilitiesModule.SetScriptClass(this.get_Context(), global);
            }
        }

        public override void Run()
        {
            this.Visit(this.get_CompileUnit());
        }

        public void SetUpDefaultImports(Module module)
        {
            foreach (string str in this.UnityScriptParameters.Imports)
            {
                module.get_Imports().Add(new Import(str));
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
                module.get_Globals().Accept(new DeclareGlobalVariables((ClassDefinition) script));
            }
        }

        public string ScriptMainMethod =>
            this.UnityScriptParameters.ScriptMainMethod;

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) base._context.get_Parameters());

        [Serializable]
        public class DeclareGlobalVariables : DepthFirstTransformer
        {
            protected ClassDefinition _class;

            public DeclareGlobalVariables(ClassDefinition cd)
            {
                this._class = cd;
            }

            public override bool EnterBlock(Block node) => 
                (node.get_ParentNode() is Method);

            public override void OnDeclarationStatement(DeclarationStatement node)
            {
                if (!node.ContainsAnnotation("PrivateScope"))
                {
                    Field field;
                    Field field1 = field = new Field(LexicalInfo.Empty);
                    field.set_Modifiers(8);
                    field.set_Name("$");
                    field.set_Type(TypeReference.Lift(node.get_Declaration().get_Type()));
                    field.set_Initializer(Expression.Lift(node.get_Initializer()));
                    field.set_IsVolatile(false);
                    field.set_Name(CodeSerializer.LiftName(node.get_Declaration().get_Name()));
                    Field field2 = field;
                    field2.set_LexicalInfo(node.get_LexicalInfo());
                    this._class.get_Members().Add(field2);
                    this.RemoveCurrentNode();
                }
            }
        }
    }
}

