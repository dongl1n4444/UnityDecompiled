namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class PBXPath
    {
        public static string Combine(string path1, string path2)
        {
            if (path2.StartsWith("/"))
            {
                return path2;
            }
            if (path1.EndsWith("/"))
            {
                return (path1 + path2);
            }
            if (path1 == "")
            {
                return path2;
            }
            if (path2 == "")
            {
                return path1;
            }
            return (path1 + "/" + path2);
        }

        public static void Combine(string path1, PBXSourceTree tree1, string path2, PBXSourceTree tree2, out string resPath, out PBXSourceTree resTree)
        {
            if (tree2 == PBXSourceTree.Group)
            {
                resPath = Combine(path1, path2);
                resTree = tree1;
            }
            else
            {
                resPath = path2;
                resTree = tree2;
            }
        }

        public static string FixSlashes(string path)
        {
            if (path == null)
            {
                return null;
            }
            return path.Replace('\\', '/');
        }

        public static string GetCurrentDirectory()
        {
            if ((Environment.OSVersion.Platform != PlatformID.MacOSX) && (Environment.OSVersion.Platform != PlatformID.Unix))
            {
                throw new Exception("PBX project compatible current directory can only obtained on OSX");
            }
            string path = FixSlashes(Directory.GetCurrentDirectory());
            if (!IsPathRooted(path))
            {
                return ("/" + path);
            }
            return path;
        }

        public static string GetDirectory(string path)
        {
            char[] trimChars = new char[] { '/' };
            path = path.TrimEnd(trimChars);
            int length = path.LastIndexOf('/');
            if (length == -1)
            {
                return "";
            }
            return path.Substring(0, length);
        }

        public static string GetFilename(string path)
        {
            int num = path.LastIndexOf('/');
            if (num == -1)
            {
                return path;
            }
            return path.Substring(num + 1);
        }

        public static string GetFullPath(string path)
        {
            if (IsPathRooted(path))
            {
                return path;
            }
            return Combine(GetCurrentDirectory(), path);
        }

        public static bool IsPathRooted(string path)
        {
            if ((path == null) || (path.Length == 0))
            {
                return false;
            }
            return (path[0] == '/');
        }

        public static string[] Split(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new string[0];
            }
            char[] separator = new char[] { '/' };
            return path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

