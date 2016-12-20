namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;

    [Serializable]
    public class ApplyDefaultVisibility : AbstractVisitorCompilerStep
    {
        public override void LeaveClassDefinition(ClassDefinition node)
        {
            this.SetPublicByDefault(node);
        }

        public override void LeaveEnumDefinition(EnumDefinition node)
        {
            this.SetPublicByDefault(node);
        }

        public override void LeaveInterfaceDefinition(InterfaceDefinition node)
        {
            this.SetPublicByDefault(node);
        }

        public override void OnConstructor(Constructor node)
        {
            this.SetPublicByDefault(node);
        }

        public override void OnField(Field node)
        {
            this.SetPublicByDefault(node);
        }

        public override void OnMethod(Method node)
        {
            if (!node.get_IsPrivate())
            {
                this.SetPublicByDefault(node);
                if (!node.get_IsFinal() && !node.get_IsStatic())
                {
                    node.set_Modifiers(node.get_Modifiers() | 0x80);
                }
            }
        }

        public override void Run()
        {
            this.Visit(this.get_CompileUnit());
        }

        public void SetPublicByDefault(TypeMember node)
        {
            if (!node.get_IsVisibilitySet())
            {
                node.set_Modifiers(node.get_Modifiers() | 8);
            }
        }
    }
}

