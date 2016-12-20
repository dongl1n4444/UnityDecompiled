namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class FileMirroring
    {
        [CompilerGenerated]
        private static Func<string, string, bool> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<string, string, bool> <>f__mg$cache1;

        private static bool AreFilesIdentical(string filePath1, string filePath2)
        {
            using (FileStream stream = File.OpenRead(filePath1))
            {
                using (FileStream stream2 = File.OpenRead(filePath2))
                {
                    int num;
                    if (stream.Length != stream2.Length)
                    {
                        return false;
                    }
                    byte[] buffer = new byte[0x10000];
                    byte[] buffer2 = new byte[0x10000];
                    while ((num = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream2.Read(buffer2, 0, buffer2.Length);
                        for (int i = 0; i < num; i++)
                        {
                            if (buffer[i] != buffer2[i])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static bool CanSkipCopy(string from, string to)
        {
            if (!File.Exists(to))
            {
                return false;
            }
            return AreFilesIdentical(from, to);
        }

        private static void DeleteFileOrDirectory(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static FileEntryType FileEntryTypeFor(string fileEntry)
        {
            if (File.Exists(fileEntry))
            {
                return FileEntryType.File;
            }
            if (Directory.Exists(fileEntry))
            {
                return FileEntryType.Directory;
            }
            return FileEntryType.NotExisting;
        }

        public static void MirrorFile(string from, string to)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<string, string, bool>(null, (IntPtr) CanSkipCopy);
            }
            MirrorFile(from, to, <>f__mg$cache0);
        }

        public static void MirrorFile(string from, string to, Func<string, string, bool> comparer)
        {
            if (!comparer.Invoke(from, to))
            {
                if (!File.Exists(from))
                {
                    DeleteFileOrDirectory(to);
                }
                else
                {
                    string directoryName = Path.GetDirectoryName(to);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    File.Copy(from, to, true);
                }
            }
        }

        public static void MirrorFolder(string from, string to)
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<string, string, bool>(null, (IntPtr) CanSkipCopy);
            }
            MirrorFolder(from, to, <>f__mg$cache1);
        }

        public static void MirrorFolder(string from, string to, Func<string, string, bool> comparer)
        {
            <MirrorFolder>c__AnonStorey0 storey = new <MirrorFolder>c__AnonStorey0 {
                to = to,
                from = from
            };
            storey.from = Path.GetFullPath(storey.from);
            storey.to = Path.GetFullPath(storey.to);
            if (!Directory.Exists(storey.from))
            {
                if (Directory.Exists(storey.to))
                {
                    Directory.Delete(storey.to, true);
                }
            }
            else
            {
                if (!Directory.Exists(storey.to))
                {
                    Directory.CreateDirectory(storey.to);
                }
                IEnumerable<string> first = Enumerable.Select<string, string>(Directory.GetFileSystemEntries(storey.to), new Func<string, string>(storey, (IntPtr) this.<>m__0));
                IEnumerable<string> second = Enumerable.Select<string, string>(Directory.GetFileSystemEntries(storey.from), new Func<string, string>(storey, (IntPtr) this.<>m__1));
                IEnumerable<string> enumerable3 = Enumerable.Except<string>(first, second);
                foreach (string str in enumerable3)
                {
                    DeleteFileOrDirectory(Path.Combine(storey.to, str));
                }
                foreach (string str2 in second)
                {
                    string fileEntry = Path.Combine(storey.from, str2);
                    string str4 = Path.Combine(storey.to, str2);
                    FileEntryType type = FileEntryTypeFor(fileEntry);
                    FileEntryType type2 = FileEntryTypeFor(str4);
                    if ((type == FileEntryType.File) && (type2 == FileEntryType.Directory))
                    {
                        DeleteFileOrDirectory(str4);
                    }
                    if (type == FileEntryType.Directory)
                    {
                        if (type2 == FileEntryType.File)
                        {
                            DeleteFileOrDirectory(str4);
                        }
                        if (type2 != FileEntryType.Directory)
                        {
                            Directory.CreateDirectory(str4);
                        }
                        MirrorFolder(fileEntry, str4);
                    }
                    if (type == FileEntryType.File)
                    {
                        MirrorFile(fileEntry, str4, comparer);
                    }
                }
            }
        }

        private static string StripPrefix(string s, string prefix)
        {
            return s.Substring(prefix.Length + 1);
        }

        [CompilerGenerated]
        private sealed class <MirrorFolder>c__AnonStorey0
        {
            internal string from;
            internal string to;

            internal string <>m__0(string s)
            {
                return FileMirroring.StripPrefix(s, this.to);
            }

            internal string <>m__1(string s)
            {
                return FileMirroring.StripPrefix(s, this.from);
            }
        }

        private enum FileEntryType
        {
            File,
            Directory,
            NotExisting
        }
    }
}

