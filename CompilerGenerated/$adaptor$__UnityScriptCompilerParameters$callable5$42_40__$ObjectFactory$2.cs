namespace CompilerGenerated
{
    using Boo.Lang.Environments;
    using System;

    [Serializable]
    internal sealed class $adaptor$__UnityScriptCompilerParameters$callable5$42_40__$ObjectFactory$2
    {
        protected __UnityScriptCompilerParameters$callable5$42_40__ $from;

        public $adaptor$__UnityScriptCompilerParameters$callable5$42_40__$ObjectFactory$2(__UnityScriptCompilerParameters$callable5$42_40__ from)
        {
            this.$from = from;
        }

        public static ObjectFactory Adapt(__UnityScriptCompilerParameters$callable5$42_40__ from) => 
            new ObjectFactory(new $adaptor$__UnityScriptCompilerParameters$callable5$42_40__$ObjectFactory$2(from).Invoke);

        public object Invoke() => 
            this.$from();
    }
}

