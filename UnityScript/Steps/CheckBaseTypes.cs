namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using System;
    using UnityScript.Core;

    [Serializable]
    public class CheckBaseTypes : AbstractVisitorCompilerStep
    {
        public void CheckBaseTypeAnnotation(TypeReference baseType)
        {
            if (BaseTypeAnnotations.HasExtends(baseType))
            {
                this.CheckIsClass(baseType);
            }
            else if (BaseTypeAnnotations.HasImplements(baseType))
            {
                this.CheckIsInterface(baseType);
            }
        }

        public void CheckIsClass(TypeReference baseType)
        {
            IType type = this.TypeEntityFor(baseType);
            if (!type.get_IsClass())
            {
                this.get_Errors().Add(UnityScriptCompilerErrors.ClassExpected(baseType.get_LexicalInfo(), EntityExtensions.DisplayName(type)));
            }
        }

        public void CheckIsInterface(TypeReference baseType)
        {
            IType type = this.TypeEntityFor(baseType);
            if (!type.get_IsInterface())
            {
                this.get_Errors().Add(UnityScriptCompilerErrors.InterfaceExpected(baseType.get_LexicalInfo(), EntityExtensions.DisplayName(type)));
            }
        }

        public override void LeaveClassDefinition(ClassDefinition node)
        {
            foreach (TypeReference reference in node.get_BaseTypes())
            {
                this.CheckBaseTypeAnnotation(reference);
            }
        }

        public override void OnBlock(Block node)
        {
        }

        public override void Run()
        {
            this.Visit(this.get_CompileUnit());
        }

        public IType TypeEntityFor(TypeReference baseType) => 
            this.GetEntity(baseType);
    }
}

