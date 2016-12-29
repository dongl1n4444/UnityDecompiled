namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler.TypeSystem;
    using System;

    [Serializable]
    public class ActiveScriptEntity : EvaluationContextEntity
    {
        protected int _scriptId;

        public ActiveScriptEntity(int scriptId, IMember @delegate) : base(@delegate)
        {
            this._scriptId = scriptId;
        }

        public int Script =>
            this._scriptId;
    }
}

