namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;

    [Serializable]
    public class EnableRawArrayIndexing : AbstractCompilerStep
    {
        public override void Run()
        {
            foreach (Module module in this.CompileUnit.Modules)
            {
                AstAnnotations.MarkRawArrayIndexing(module);
            }
        }
    }
}

