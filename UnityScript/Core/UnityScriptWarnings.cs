namespace UnityScript.Core
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using System;
    using System.Text;

    public static class UnityScriptWarnings
    {
        public static CompilerWarning BitwiseOperatorWithBooleanOperands(LexicalInfo location, string expectedOperator, string actualOperator)
        {
            return CreateWarning("UCW0003", location, new StringBuilder("WARNING: Bitwise operation '").Append(actualOperator).Append("' on boolean values won't shortcut. Did you mean '").Append(expectedOperator).Append("'?").ToString());
        }

        public static CompilerWarning CannotStartCoroutineFromStaticFunction(LexicalInfo location, string coroutineName)
        {
            return CreateWarning("UCW0004", location, new StringBuilder("WARNING: Coroutine '").Append(coroutineName).Append("' cannot be automatically started from a static function.").ToString());
        }

        private static CompilerWarning CreateWarning(string code, LexicalInfo location, string message)
        {
            return new CompilerWarning(location, message, code);
        }

        public static CompilerWarning ScriptMainMethodIsImplicitlyDefined(LexicalInfo location, string functionName)
        {
            return CreateWarning("UCW0002", location, new StringBuilder("WARNING: Function '").Append(functionName).Append("()' is already implicitly defined to contain global script code. Global code will be merged at the end. Either rename the function, change its signature or remove any global code if that's not what you intended.").ToString());
        }

        public static CompilerWarning VirtualKeywordHasNoEffect(LexicalInfo location)
        {
            return CreateWarning("UCW0100", location, "WARNING: 'virtual' keyword has no effect and it has been deprecated. Functions are virtual by default.");
        }
    }
}

