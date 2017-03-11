namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class ExtensionMethods
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;

        public static ReadOnlyHashSet<T> AsReadOnly<T>(this HashSet<T> set) => 
            new ReadOnlyHashSet<T>(set);

        public static ReadOnlyDictionary<T, K> AsReadOnly<T, K>(this IDictionary<T, K> dictionary) => 
            new ReadOnlyDictionary<T, K>(dictionary);

        public static IEnumerable<string> InQuotes(this IEnumerable<string> inputs)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = input => input.InQuotes();
            }
            return inputs.Select<string, string>(<>f__am$cache0);
        }

        public static string InQuotes(this string input) => 
            ("\"" + input + "\"");

        public static IEnumerable<string> PrefixedWith(this IEnumerable<string> inputs, string prefix)
        {
            <PrefixedWith>c__AnonStorey0 storey = new <PrefixedWith>c__AnonStorey0 {
                prefix = prefix
            };
            return inputs.Select<string, string>(new Func<string, string>(storey.<>m__0));
        }

        public static string SeparateWithSpaces(this IEnumerable<string> inputs)
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

            internal string <>m__0(string input) => 
                (this.prefix + input);
        }
    }
}

