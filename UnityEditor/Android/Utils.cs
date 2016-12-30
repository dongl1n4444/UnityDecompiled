namespace UnityEditor.Android
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class Utils
    {
        [CompilerGenerated]
        private static Func<char, bool> <>f__am$cache0;
        public static readonly Version DefaultVersion = new Version(0, 0, 0);

        public static Version ParseVersion(string version)
        {
        Label_0013:
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = c => c == '.';
            }
            if (Enumerable.Count<char>(version, <>f__am$cache0) < 2)
            {
                version = version + ".0";
                goto Label_0013;
            }
            return new Version(version);
        }
    }
}

