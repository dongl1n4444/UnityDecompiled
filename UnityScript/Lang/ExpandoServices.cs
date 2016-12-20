namespace UnityScript.Lang
{
    using CompilerGenerated;
    using System;
    using System.Collections.Generic;

    public static class ExpandoServices
    {
        [NonSerialized]
        private static List<Expando> _expandos = new List<Expando>();

        internal static bool $Purge$closure$5(Expando e)
        {
            return (e.Target == null);
        }

        public static Expando GetExpandoFor(object o)
        {
            $GetExpandoFor$locals$15 s$ = new $GetExpandoFor$locals$15 {
                $o = o
            };
            List<Expando> list = _expandos;
            lock (list)
            {
                Purge();
                return _expandos.Find($adaptor$__ExpandoServices$callable0$60_29__$Predicate$0.Adapt(new __ExpandoServices$callable0$60_29__(new $GetExpandoFor$closure$4(s$).Invoke)));
            }
        }

        public static object GetExpandoProperty(object target, string name)
        {
            Expando expandoFor = GetExpandoFor(target);
            return ((expandoFor != null) ? expandoFor[name] : null);
        }

        public static Expando GetOrCreateExpandoFor(object o)
        {
            List<Expando> list = _expandos;
            lock (list)
            {
                Expando expandoFor = GetExpandoFor(o);
                if (expandoFor == null)
                {
                    expandoFor = new Expando(o);
                    _expandos.Add(expandoFor);
                }
                return expandoFor;
            }
        }

        public static void Purge()
        {
            List<Expando> list = _expandos;
            lock (list)
            {
                _expandos.RemoveAll($adaptor$__ExpandoServices$callable0$60_29__$Predicate$0.Adapt(new __ExpandoServices$callable0$60_29__(ExpandoServices.$Purge$closure$5)));
            }
        }

        public static object SetExpandoProperty(object target, string name, object value)
        {
            GetOrCreateExpandoFor(target)[name] = value;
            return value;
        }

        public static int ExpandoObjectCount
        {
            get
            {
                Purge();
                return _expandos.Count;
            }
        }

        [Serializable]
        internal class $GetExpandoFor$closure$4
        {
            internal ExpandoServices.$GetExpandoFor$locals$15 $$locals$16;

            public $GetExpandoFor$closure$4(ExpandoServices.$GetExpandoFor$locals$15 $$locals$16)
            {
                this.$$locals$16 = $$locals$16;
            }

            public bool Invoke(Expando e)
            {
                return (e.Target == this.$$locals$16.$o);
            }
        }

        [Serializable]
        internal class $GetExpandoFor$locals$15
        {
            internal object $o;
        }
    }
}

