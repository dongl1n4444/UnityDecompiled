namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Extension]
    public static class ExtensionMethods
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;

        [Extension]
        public static ReadOnlyHashSet<T> AsReadOnly<T>(HashSet<T> set)
        {
            return new ReadOnlyHashSet<T>(set);
        }

        [Extension]
        public static ReadOnlyDictionary<T, K> AsReadOnly<T, K>(IDictionary<T, K> dictionary)
        {
            return new ReadOnlyDictionary<T, K>(dictionary);
        }

        [Extension]
        public static IEnumerable<string> InQuotes(IEnumerable<string> inputs)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, string>(null, (IntPtr) <InQuotes>m__0);
            }
            return Enumerable.Select<string, string>(inputs, <>f__am$cache0);
        }

        [Extension]
        public static string InQuotes(string input)
        {
            return ("\"" + input + "\"");
        }

        [Extension]
        public static IEnumerable<string> PrefixedWith(IEnumerable<string> inputs, string prefix)
        {
            <PrefixedWith>c__AnonStorey0 storey = new <PrefixedWith>c__AnonStorey0 {
                prefix = prefix
            };
            return Enumerable.Select<string, string>(inputs, new Func<string, string>(storey, (IntPtr) this.<>m__0));
        }

        [Extension]
        public static string SeparateWithSpaces(IEnumerable<string> inputs)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (string str in inputs)
            {
                if (!flag)
                {
                    builder.Append(" ");
                }
                flag = false;
                builder.Append(str);
            }
            return builder.ToString();
        }

        [CompilerGenerated]
        private sealed class <PrefixedWith>c__AnonStorey0
        {
            internal string prefix;

            internal string <>m__0(string input)
            {
                return (this.prefix + input);
            }
        }
    }
}

