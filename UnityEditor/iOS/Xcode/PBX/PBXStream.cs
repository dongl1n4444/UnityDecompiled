namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class PBXStream
    {
        private static bool DontNeedQuotes(string src)
        {
            if (src.Length == 0)
            {
                return false;
            }
            bool flag2 = false;
            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];
                if (((!char.IsLetterOrDigit(c) && (c != '.')) && (c != '*')) && (c != '_'))
                {
                    if (c != '/')
                    {
                        return false;
                    }
                    flag2 = true;
                }
            }
            if (flag2 && ((src.Contains("//") || src.Contains("/*")) || src.Contains("*/")))
            {
                return false;
            }
            return true;
        }

        public static string QuoteStringIfNeeded(string src)
        {
            if (DontNeedQuotes(src))
            {
                return src;
            }
            return ("\"" + src.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n") + "\"");
        }

        public static string UnquoteString(string src)
        {
            if (!src.StartsWith("\"") || !src.EndsWith("\""))
            {
                return src;
            }
            return src.Substring(1, src.Length - 2).Replace(@"\\", "嚟").Replace("\\\"", "\"").Replace(@"\n", "\n").Replace("嚟", @"\");
        }
    }
}

