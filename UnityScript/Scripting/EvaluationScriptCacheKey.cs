namespace UnityScript.Scripting
{
    using System;

    [Serializable]
    public class EvaluationScriptCacheKey
    {
        protected string _code;
        protected Type _contextType;

        public EvaluationScriptCacheKey(Type contextType, string code)
        {
            this._contextType = contextType;
            this._code = code;
        }

        public override bool Equals(object o)
        {
            EvaluationScriptCacheKey key = o as EvaluationScriptCacheKey;
            if (key._contextType != this._contextType)
            {
            }
            return ((key != null) ? (key._code == this._code) : false);
        }

        public override int GetHashCode()
        {
            return (this._contextType.GetHashCode() ^ this._code.GetHashCode());
        }
    }
}

