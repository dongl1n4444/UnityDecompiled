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
            if (!node.IsPrivate)
            {
                this.SetPublicByDefault(node);
                if (!node.IsFinal && !node.IsStatic)
                {
                    node.Modifiers |= TypeMemberModifiers.Virtual;
                }
            }
        }

        public override void Run()
        {
            this.Visit(this.CompileUnit);
        }

        public void SetPublicByDefault(TypeMember node)
        {
            if (!node.IsVisibilitySet)
            {
                node.Modifiers |= TypeMemberModifiers.Public;
            }
        }
    }
}

