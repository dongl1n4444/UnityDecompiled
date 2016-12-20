namespace UnityEditor.Android
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    internal class AndroidFileLocator
    {
        private static readonly Regex DEFAULT_IGNORE_PATTERN = new Regex(@"(?:^\._)|(?:^\.DS_Store)", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string[] Find(string searchPattern)
        {
            List<string> result = new List<string>();
            Find(searchPattern, result, false);
            return result.ToArray();
        }

        public static bool Find(string searchPattern, List<string> result)
        {
            return Find(searchPattern, result, false);
        }

        public static bool Find(string searchPattern, List<string> result, bool findFirst)
        {
            return Find(searchPattern, result, findFirst, 0x100);
        }

        public static bool Find(string searchPattern, List<string> result, bool findFirst, int maxdepth)
        {
            return Find(searchPattern, DEFAULT_IGNORE_PATTERN, result, findFirst, maxdepth);
        }

        public static bool Find(string searchPattern, Regex ignorePattern, List<string> result, bool findFirst, int maxdepth)
        {
            if (maxdepth < 0)
            {
                return false;
            }
            char[] anyOf = new char[] { '/', '\\' };
            char[] chArray2 = new char[] { '*', '?' };
            int startIndex = searchPattern.IndexOfAny(chArray2);
            if (startIndex >= 0)
            {
                int length = searchPattern.IndexOfAny(anyOf, startIndex);
                if (length == -1)
                {
                    length = searchPattern.Length;
                }
                string str = searchPattern.Substring(length);
                string path = searchPattern.Substring(0, length);
                string directoryName = Path.GetDirectoryName(path);
                if ("" == directoryName)
                {
                    directoryName = Directory.GetCurrentDirectory();
                }
                DirectoryInfo info = new DirectoryInfo(directoryName);
                if (!info.Exists)
                {
                    return false;
                }
                string fileName = Path.GetFileName(path);
                foreach (FileSystemInfo info2 in info.GetFileSystemInfos(fileName))
                {
                    if (Find(info2.FullName + str, result, findFirst, maxdepth - 1) && findFirst)
                    {
                        return true;
                    }
                }
            }
            else if (File.Exists(searchPattern))
            {
                if (!ignorePattern.IsMatch(Path.GetFileName(searchPattern)))
                {
                    result.Add(searchPattern);
                }
            }
            else if (Directory.Exists(searchPattern) && !ignorePattern.IsMatch(Path.GetDirectoryName(searchPattern)))
            {
                result.Add(searchPattern);
            }
            return (result.Count > 0);
        }
    }
}

