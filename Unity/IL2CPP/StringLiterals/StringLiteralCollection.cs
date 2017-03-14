namespace Unity.IL2CPP.StringLiterals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Portability;

    public class StringLiteralCollection : IStringLiteralProvider, IStringLiteralCollection, IDisposable
    {
        private readonly Dictionary<StringMetadataToken, int> _stringLiterals = new Dictionary<StringMetadataToken, int>(new StringMetadataTokenComparer());
        [CompilerGenerated]
        private static Func<KeyValuePair<StringMetadataToken, int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<StringMetadataToken, int>, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<StringMetadataToken, int>, int> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<StringMetadataToken, int>, StringMetadataToken> <>f__am$cache3;

        public int Add(StringMetadataToken stringMetadataToken)
        {
            int count;
            if (!this._stringLiterals.TryGetValue(stringMetadataToken, out count))
            {
                count = this._stringLiterals.Count;
                this._stringLiterals.Add(stringMetadataToken, count);
            }
            return count;
        }

        public void Dispose()
        {
            this._stringLiterals.Clear();
        }

        public int GetIndex(StringMetadataToken stringMetadataToken) => 
            this._stringLiterals[stringMetadataToken];

        public ReadOnlyCollection<string> GetStringLiterals()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = kvp => kvp.Value;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = kvp => kvp.Key.Literal;
            }
            return this._stringLiterals.OrderBy<KeyValuePair<StringMetadataToken, int>, int>(<>f__am$cache0).Select<KeyValuePair<StringMetadataToken, int>, string>(<>f__am$cache1).ToArray<string>().AsReadOnlyPortable<string>();
        }

        public ReadOnlyCollection<StringMetadataToken> GetStringMetadataTokens()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = kvp => kvp.Value;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = kvp => kvp.Key;
            }
            return this._stringLiterals.OrderBy<KeyValuePair<StringMetadataToken, int>, int>(<>f__am$cache2).Select<KeyValuePair<StringMetadataToken, int>, StringMetadataToken>(<>f__am$cache3).ToArray<StringMetadataToken>().AsReadOnlyPortable<StringMetadataToken>();
        }
    }
}

