namespace UnityScript.Scripting
{
    using Boo.Lang.Compiler;
    using System;

    [Serializable]
    public class CompilationErrorsException : Exception
    {
        protected CompilerErrorCollection _errors;

        public CompilationErrorsException(CompilerErrorCollection errors) : base(errors.ToString(true))
        {
            this._errors = errors;
        }

        public CompilerErrorCollection Errors =>
            this._errors;
    }
}

