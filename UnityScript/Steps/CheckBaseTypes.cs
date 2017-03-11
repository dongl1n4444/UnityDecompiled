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
            IType entity = this.TypeEntityFor(baseType);
            if (!entity.IsClass)
            {
                this.Errors.Add(UnityScriptCompilerErrors.ClassExpected(baseType.LexicalInfo, entity.DisplayName()));
            }
        }

        public void CheckIsInterface(TypeReference baseType)
        {
            IType entity = this.TypeEntityFor(baseType);
            if (!entity.IsInterface)
            {
                this.Errors.Add(UnityScriptCompilerErrors.InterfaceExpected(baseType.LexicalInfo, entity.DisplayName()));
            }
        }

        public override void LeaveClassDefinition(ClassDefinition node)
        {
            foreach (TypeReference reference in node.BaseTypes)
            {
                this.CheckBaseTypeAnnotation(reference);
            }
        }

        public override void OnBlock(Block node)
        {
        }

        public override void Run()
        {
            this.Visit(this.CompileUnit);
        }

        public IType TypeEntityFor(TypeReference baseType) => 
            this.GetEntity(baseType);
    }
}

