namespace Unity.IL2CPP.StringLiterals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Portability;

    public class StringLiteralCollection : IStringLiteralProvider, IStringLiteralCollection, IDisposable
    {
        private readonly Dictionary<string, int> _stringLiterals = new Dictionary<string, int>();
        [CompilerGenerated]
        private static Func<KeyValuePair<string, int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, int>, string> <>f__am$cache1;

        public int Add(string literal)
        {
            int count;
            if (!this._stringLiterals.TryGetValue(literal, out count))
            {
                count = this._stringLiterals.Count;
                this._stringLiterals.Add(literal, count);
            }
            return count;
        }

        public void Dispose()
        {
            this._stringLiterals.Clear();
        }

        public int GetIndex(string literal) => 
            this._stringLiterals[literal];

        public ReadOnlyCollection<string> GetStringLiterals()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<string, int>, int>(null, (IntPtr) <GetStringLiterals>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<string, int>, string>(null, (IntPtr) <GetStringLiterals>m__1);
            }
            return this._stringLiterals.OrderBy<KeyValuePair<string, int>, int>(<>f__am$cache0).Select<KeyValuePair<string, int>, string>(<>f__am$cache1).ToArray<string>().AsReadOnlyPortable<string>();
        }
    }
}

