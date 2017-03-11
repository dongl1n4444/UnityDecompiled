namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using System;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope]
    public sealed class ApplySemanticsModule
    {
        private ApplySemanticsModule()
        {
        }

        public static LexicalInfo Copy(LexicalInfo li) => 
            new LexicalInfo(li.FileName, li.Line, li.Column);
    }
}

