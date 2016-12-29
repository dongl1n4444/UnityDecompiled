namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class Settings
    {
        private static List<IPrefType> m_AddedPrefs = new List<IPrefType>();
        private static SortedList<string, object> m_Prefs = new SortedList<string, object>();

        internal static void Add(IPrefType value)
        {
            m_AddedPrefs.Add(value);
        }

        internal static T Get<T>(string name, T defaultValue) where T: IPrefType, new()
        {
            Load();
            if (defaultValue == null)
            {
                throw new ArgumentException("default can not be null", "defaultValue");
            }
            if (m_Prefs.ContainsKey(name))
            {
                return (T) m_Prefs[name];
            }
            string sstr = EditorPrefs.GetString(name, "");
            if (sstr == "")
            {
                Set<T>(name, defaultValue);
                return defaultValue;
            }
            defaultValue.FromUniqueString(sstr);
            Set<T>(name, defaultValue);
            return defaultValue;
        }

        private static void Load()
        {
            if (m_AddedPrefs.Any<IPrefType>())
            {
                List<IPrefType> list = new List<IPrefType>(m_AddedPrefs);
                m_AddedPrefs.Clear();
                foreach (IPrefType type in list)
                {
                    type.Load();
                }
            }
        }

        [DebuggerHidden]
        internal static IEnumerable<KeyValuePair<string, T>> Prefs<T>() where T: IPrefType => 
            new <Prefs>c__Iterator0<T> { $PC = -2 };

        internal static void Set<T>(string name, T value) where T: IPrefType
        {
            Load();
            EditorPrefs.SetString(name, value.ToUniqueString());
            m_Prefs[name] = value;
        }

        [CompilerGenerated]
        private sealed class <Prefs>c__Iterator0<T> : IEnumerable, IEnumerable<KeyValuePair<string, T>>, IEnumerator, IDisposable, IEnumerator<KeyValuePair<string, T>> where T: IPrefType
        {
            internal KeyValuePair<string, T> $current;
            internal bool $disposing;
            internal IEnumerator<KeyValuePair<string, object>> $locvar0;
            internal int $PC;
            internal KeyValuePair<string, object> <kvp>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        Settings.Load();
                        this.$locvar0 = Settings.m_Prefs.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00E9;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_00B1;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<kvp>__0 = this.$locvar0.Current;
                        if (this.<kvp>__0.Value is T)
                        {
                            this.$current = new KeyValuePair<string, T>(this.<kvp>__0.Key, (T) this.<kvp>__0.Value);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    Label_00B1:;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$PC = -1;
            Label_00E9:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Settings.<Prefs>c__Iterator0<T>();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string,T>>.GetEnumerator();

            KeyValuePair<string, T> IEnumerator<KeyValuePair<string, T>>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

