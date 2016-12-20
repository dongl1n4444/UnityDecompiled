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

        public static LexicalInfo Copy(LexicalInfo li)
        {
            return new LexicalInfo(li.get_FileName(), li.get_Line(), li.get_Column());
        }
    }
}

