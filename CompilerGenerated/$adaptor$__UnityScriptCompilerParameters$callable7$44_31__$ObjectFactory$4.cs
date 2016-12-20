namespace CompilerGenerated
{
    using Boo.Lang.Environments;
    using System;

    [Serializable]
    internal sealed class $adaptor$__UnityScriptCompilerParameters$callable7$44_31__$ObjectFactory$4
    {
        protected __UnityScriptCompilerParameters$callable7$44_31__ $from;

        public $adaptor$__UnityScriptCompilerParameters$callable7$44_31__$ObjectFactory$4(__UnityScriptCompilerParameters$callable7$44_31__ from)
        {
            this.$from = from;
        }

        public static ObjectFactory Adapt(__UnityScriptCompilerParameters$callable7$44_31__ from)
        {
            return new ObjectFactory(new $adaptor$__UnityScriptCompilerParameters$callable7$44_31__$ObjectFactory$4(from).Invoke);
        }

        public object Invoke()
        {
            return this.$from();
        }
    }
}

