namespace CompilerGenerated
{
    using System;
    using UnityScript.Lang;

    [Serializable]
    internal sealed class $adaptor$__ExpandoServices$callable0$60_29__$Predicate$0
    {
        protected __ExpandoServices$callable0$60_29__ $from;

        public $adaptor$__ExpandoServices$callable0$60_29__$Predicate$0(__ExpandoServices$callable0$60_29__ from)
        {
            this.$from = from;
        }

        public static Predicate<Expando> Adapt(__ExpandoServices$callable0$60_29__ from) => 
            new Predicate<Expando>(new $adaptor$__ExpandoServices$callable0$60_29__$Predicate$0(from).Invoke);

        public bool Invoke(Expando obj) => 
            this.$from(obj);
    }
}

