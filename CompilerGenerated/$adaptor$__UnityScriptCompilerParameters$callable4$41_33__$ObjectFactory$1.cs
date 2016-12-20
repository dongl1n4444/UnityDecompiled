namespace CompilerGenerated
{
    using Boo.Lang.Environments;
    using System;

    [Serializable]
    internal sealed class $adaptor$__UnityScriptCompilerParameters$callable4$41_33__$ObjectFactory$1
    {
        protected __UnityScriptCompilerParameters$callable4$41_33__ $from;

        public $adaptor$__UnityScriptCompilerParameters$callable4$41_33__$ObjectFactory$1(__UnityScriptCompilerParameters$callable4$41_33__ from)
        {
            this.$from = from;
        }

        public static ObjectFactory Adapt(__UnityScriptCompilerParameters$callable4$41_33__ from)
        {
            return new ObjectFactory(new $adaptor$__UnityScriptCompilerParameters$callable4$41_33__$ObjectFactory$1(from).Invoke);
        }

        public object Invoke()
        {
            return this.$from();
        }
    }
}

