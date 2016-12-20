namespace CompilerGenerated
{
    using Boo.Lang.Environments;
    using System;

    [Serializable]
    internal sealed class $adaptor$__UnityScriptCompilerParameters$callable6$43_34__$ObjectFactory$3
    {
        protected __UnityScriptCompilerParameters$callable6$43_34__ $from;

        public $adaptor$__UnityScriptCompilerParameters$callable6$43_34__$ObjectFactory$3(__UnityScriptCompilerParameters$callable6$43_34__ from)
        {
            this.$from = from;
        }

        public static ObjectFactory Adapt(__UnityScriptCompilerParameters$callable6$43_34__ from)
        {
            return new ObjectFactory(new $adaptor$__UnityScriptCompilerParameters$callable6$43_34__$ObjectFactory$3(from).Invoke);
        }

        public object Invoke()
        {
            return this.$from();
        }
    }
}

