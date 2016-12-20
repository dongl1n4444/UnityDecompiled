namespace UnityScript.Macros
{
    using Boo.Lang.Compiler.Ast;
    using System;

    [Serializable]
    public class CaseStatement : CustomStatement
    {
        public Block Body;
        public ExpressionCollection Expressions;
    }
}

