namespace UnityScript.Scripting
{
    using Boo.Lang;
    using Boo.Lang.Runtime;
    using System;

    [Serializable]
    public class EvaluationDomain
    {
        protected Hash _cache = new Hash();

        public void CacheScript(EvaluationScriptCacheKey key, Type type)
        {
            EvaluationDomain domain = this;
            lock (domain)
            {
                this._cache[key] = type;
            }
        }

        public Type GetCachedScript(EvaluationScriptCacheKey key)
        {
            EvaluationDomain domain = this;
            lock (domain)
            {
                object obj1 = this._cache[key];
                if (!(obj1 is Type))
                {
                }
                return (Type) RuntimeServices.Coerce(obj1, typeof(Type));
            }
        }
    }
}

