namespace CompilerGenerated
{
    using Boo.Lang.Environments;
    using System;

    [Serializable]
    internal sealed class $adaptor$__UnityScriptCompilerParameters$callable3$40_30__$ObjectFactory$0
    {
        protected __UnityScriptCompilerParameters$callable3$40_30__ $from;

        public $adaptor$__UnityScriptCompilerParameters$callable3$40_30__$ObjectFactory$0(__UnityScriptCompilerParameters$callable3$40_30__ from)
        {
            this.$from = from;
        }

        public static ObjectFactory Adapt(__UnityScriptCompilerParameters$callable3$40_30__ from)
        {
            return new ObjectFactory(new $adaptor$__UnityScriptCompilerParameters$callable3$40_30__$ObjectFactory$0(from).Invoke);
        }

        public object Invoke()
        {
            return this.$from();
        }
    }
}

