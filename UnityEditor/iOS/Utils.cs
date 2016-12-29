namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    internal static class Utils
    {
        private static iOSDeviceDescription[] iOSDevices;
        private const int modelColumn = 5;
        private const int modelIdColumn = 3;
        private const int productIdColumn = 1;
        private const int revisionColumn = 2;
        private const int typeColumn = 4;
        private const int vendorIdColumn = 0;

        public static int CompareVersionNumbers(string v1, string v2)
        {
            if ((v1 == null) && (v2 != null))
            {
                return -1;
            }
            if ((v1 != null) && (v2 == null))
            {
                return 1;
            }
            if ((v1 == null) && (v2 == null))
            {
                return 0;
            }
            char[] separator = new char[] { '.' };
            string[] strArray = v1.Split(separator);
            char[] chArray2 = new char[] { '.' };
            string[] strArray2 = v2.Split(chArray2);
            for (int i = 0; (i < strArray.Length) && (i < strArray2.Length); i++)
            {
                if (strArray[i] != strArray2[i])
                {
                    int num3;
                    int num4;
                    bool flag = int.TryParse(strArray[i], out num3);
                    bool flag2 = int.TryParse(strArray2[i], out num4);
                    if (!flag && flag2)
                    {
                        return -1;
                    }
                    if (flag && !flag2)
                    {
                        return 1;
                    }
                    if (!flag && !flag2)
                    {
                        return string.Compare(strArray[i], strArray2[i]);
                    }
                    if (num3 != num4)
                    {
                        return (num3 - num4);
                    }
                }
            }
            return (strArray.Length - strArray2.Length);
        }

        public static void CopyRecursiveWithIgnoreList(string src, string dst, Regex[] ignoreList)
        {
            <CopyRecursiveWithIgnoreList>c__AnonStorey2 storey = new <CopyRecursiveWithIgnoreList>c__AnonStorey2 {
                dst = dst
            };
            EnumerateFilesRecursivelyWithIgnoreList(src, new EnumerateCallback(storey.<>m__0), new EnumerateCallback(storey.<>m__1), ignoreList);
        }

        public static void DeleteDirectoryIfEmpty(string path)
        {
            Regex regex = new Regex(@"\.DS_Store");
            bool flag = !Directory.GetDirectories(path).Any<string>();
            foreach (string str in Directory.GetFiles(path))
            {
                if (!regex.IsMatch(str))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                FileUtil.DeleteFileOrDirectory(path);
            }
        }

        public static void DeleteNonHiddenFilesOrDirectories(string item)
        {
            if ((item != null) && !Path.GetFileName(item).StartsWith("."))
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
                else
                {
                    foreach (string str in Directory.GetDirectories(item))
                    {
                        if (!Path.GetFileName(str).StartsWith("."))
                        {
                            DeleteNonHiddenFilesOrDirectories(str);
                        }
                    }
                    foreach (string str2 in Directory.GetFiles(item))
                    {
                        if (!Path.GetFileName(str2).StartsWith("."))
                        {
                            FileUtil.DeleteFileOrDirectory(str2);
                        }
                    }
                    if ((Directory.GetFiles(item).Length == 0) && (Directory.GetDirectories(item).Length == 0))
                    {
                        Directory.Delete(item);
                    }
                }
            }
        }

        public static void DeleteRecursiveWithMatchList(string directory, Regex[] matchList)
        {
            <DeleteRecursiveWithMatchList>c__AnonStorey4 storey = new <DeleteRecursiveWithMatchList>c__AnonStorey4 {
                matchList = matchList
            };
            EnumerateCallback fileCallback = new EnumerateCallback(storey.<>m__0);
            EnumerateFilesRecursivelyWithIgnoreList(directory, fileCallback, fileCallback, null);
        }

        public static void EnumerateFilesRecursivelyWithIgnoreList(string src, EnumerateCallback fileCallback, EnumerateCallback dirCallback, Regex[] ignoreList)
        {
            EnumerateFilesRecursivelyWithIgnoreListImpl(src, "", fileCallback, dirCallback, ignoreList);
        }

        internal static void EnumerateFilesRecursivelyWithIgnoreListImpl(string src, string curLevel, EnumerateCallback fileCallback, EnumerateCallback dirCallback, Regex[] ignoreList)
        {
            if ((ignoreList == null) || !MatchesAny(Path.GetFileName(src), ignoreList))
            {
                if (File.Exists(src))
                {
                    if (fileCallback != null)
                    {
                        fileCallback(src, curLevel);
                    }
                }
                else
                {
                    if (dirCallback != null)
                    {
                        dirCallback(src, curLevel);
                    }
                    foreach (string str in Directory.GetDirectories(src))
                    {
                        EnumerateFilesRecursivelyWithIgnoreListImpl(str, Path.Combine(curLevel, Path.GetFileName(str)), fileCallback, dirCallback, ignoreList);
                    }
                    foreach (string str2 in Directory.GetFiles(src))
                    {
                        EnumerateFilesRecursivelyWithIgnoreListImpl(str2, curLevel, fileCallback, dirCallback, ignoreList);
                    }
                }
            }
        }

        public static bool FileContentsEqual(string path1, string path2)
        {
            bool flag;
            FileInfo info = new FileInfo(path1);
            FileInfo info2 = new FileInfo(path2);
            if (!info.Exists || !info2.Exists)
            {
                return false;
            }
            if (info.Length != info2.Length)
            {
                return false;
            }
            using (FileStream stream = new FileStream(path1, System.IO.FileMode.Open))
            {
                using (FileStream stream2 = new FileStream(path2, System.IO.FileMode.Open))
                {
                    int num;
                    byte[] buffer = new byte[0x1000];
                    byte[] buffer2 = new byte[0x1000];
                Label_0071:
                    num = stream.Read(buffer, 0, 0x1000);
                    int count = stream2.Read(buffer2, 0, 0x1000);
                    if (num != count)
                    {
                        return false;
                    }
                    if (num == 0)
                    {
                        return true;
                    }
                    if (!buffer.Take<byte>(num).SequenceEqual<byte>(buffer2.Take<byte>(count)))
                    {
                        flag = false;
                    }
                    else
                    {
                        goto Label_0071;
                    }
                }
            }
            return flag;
        }

        public static bool FindiOSDevice(int vendorId, int productId, int revision, string name, out iOSDeviceDescription device)
        {
            <FindiOSDevice>c__AnonStorey0 storey = new <FindiOSDevice>c__AnonStorey0 {
                vendorId = vendorId,
                productId = productId,
                revision = revision
            };
            if (iOSDevices == null)
            {
                iOSDevices = LoadiOSDeviceDescriptions();
            }
            device = Enumerable.FirstOrDefault<iOSDeviceDescription>(iOSDevices, new Func<iOSDeviceDescription, bool>(storey, (IntPtr) this.<>m__0));
            if (device.IsValid())
            {
                return true;
            }
            storey.modelId = GuessModelId(name, storey.revision);
            device = Enumerable.FirstOrDefault<iOSDeviceDescription>(iOSDevices, new Func<iOSDeviceDescription, bool>(storey, (IntPtr) this.<>m__1));
            if (device.IsValid())
            {
                return true;
            }
            device = Enumerable.FirstOrDefault<iOSDeviceDescription>(iOSDevices, new Func<iOSDeviceDescription, bool>(storey, (IntPtr) this.<>m__2));
            return device.IsValid();
        }

        public static string GetCurrentUserName()
        {
            string environmentVariable = "user";
            if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.LinuxEditor))
            {
                return Environment.GetEnvironmentVariable("USER");
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                environmentVariable = Environment.GetEnvironmentVariable("USERNAME");
            }
            return environmentVariable;
        }

        public static string GetDeviceType(string name)
        {
            if (name.Contains("iPad"))
            {
                return "iPad";
            }
            if (name.Contains("iPhone"))
            {
                return "iPhone";
            }
            if (name.Contains("iPod"))
            {
                return "iPod";
            }
            if (name.Contains("AppleTV"))
            {
                return "AppleTV";
            }
            return null;
        }

        public static string GetOSExeExtension()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return ".exe";
            }
            return "";
        }

        public static string GetOSPathPart()
        {
            RuntimePlatform platform = Application.platform;
            if (platform != RuntimePlatform.WindowsEditor)
            {
                if (platform != RuntimePlatform.OSXEditor)
                {
                    if (platform != RuntimePlatform.LinuxEditor)
                    {
                        throw new NotSupportedException("Platform not supported");
                    }
                    return "Linux";
                }
            }
            else
            {
                return "Win";
            }
            return "OSX";
        }

        public static string GetRelativePath(string folder, string path)
        {
            if (!folder.StartsWith("/"))
            {
                folder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            }
            if (!path.StartsWith("/"))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            folder = folder.Replace(Path.DirectorySeparatorChar, '/');
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            char[] separator = new char[] { '/' };
            List<string> components = new List<string>(folder.Split(separator));
            char[] chArray2 = new char[] { '/' };
            List<string> list2 = new List<string>(path.Split(chArray2));
            NormalizePath(components);
            NormalizePath(list2);
            int num = 0;
            while (((num < components.Count) && (num < list2.Count)) && (string.Compare(components[num], list2[num], StringComparison.OrdinalIgnoreCase) == 0))
            {
                num++;
            }
            string str = "";
            for (int i = num; i < components.Count; i++)
            {
                str = Path.Combine(str, "..");
            }
            for (int j = num; j < list2.Count; j++)
            {
                str = Path.Combine(str, list2[j]);
            }
            return str;
        }

        public static string GetVariantNameFromPath(string path)
        {
            string extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
            {
                return null;
            }
            return extension.Substring(1);
        }

        public static bool GracefullyKillProcess(Process process, int waitForClose, int waitForKill)
        {
            if (!process.HasExited)
            {
                process.CloseMainWindow();
                process.WaitForExit(waitForClose);
            }
            if (!process.HasExited)
            {
                process.Kill();
                process.WaitForExit(waitForKill);
            }
            return process.HasExited;
        }

        public static string GuessModelId(string deviceName, int revision)
        {
            string str = deviceName;
            if ((revision & 0xf000) != 0)
            {
                str = str + ((revision >> 12) & 15);
            }
            str = str + ((string) ((revision >> 8) & 15)) + ",";
            if ((revision & 240) != 0)
            {
                str = str + ((revision >> 4) & 15);
            }
            if ((revision & 240) == 0)
            {
                str = str + ((revision >> 0) & 15);
            }
            return str;
        }

        public static int InstallFileWithFallbacks(string[] srcFiles, string dest)
        {
            FileUtil.DeleteFileOrDirectory(dest);
            for (int i = 0; i < srcFiles.Count<string>(); i++)
            {
                if (File.Exists(srcFiles[i]))
                {
                    FileUtil.CopyFileOrDirectory(srcFiles[i], dest);
                    return i;
                }
            }
            return -1;
        }

        public static bool IsPathExtensionOneOf(string path, string[] exts)
        {
            if ((path != null) && !Path.GetFileName(path).StartsWith("."))
            {
                string str = Path.GetExtension(path).ToLower();
                foreach (string str2 in exts)
                {
                    if (str == str2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static iOSDeviceDescription[] LoadiOSDeviceDescriptions()
        {
            List<iOSDeviceDescription> list = new List<iOSDeviceDescription>();
            string[] strArray = File.ReadAllLines(Path.Combine(UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.iOS, BuildOptions.CompressTextures), "Data/iosdevices.csv"));
            foreach (string str2 in strArray)
            {
                char[] separator = new char[] { ';' };
                string[] strArray3 = str2.Split(separator);
                if (((((strArray3.Length <= 0) || (strArray3[0].Length <= 0)) || (strArray3[0][0] != '#')) && ((strArray3.Length >= 6) && (strArray3[4].Length != 0))) && (strArray3[5].Length != 0))
                {
                    int num2;
                    int num3;
                    int num4;
                    string type = strArray3[4];
                    string model = strArray3[5];
                    string modelId = strArray3[3];
                    int.TryParse(strArray3[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2);
                    int.TryParse(strArray3[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num3);
                    int.TryParse(strArray3[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num4);
                    list.Add(new iOSDeviceDescription(num2, num3, num4, modelId, type, model));
                }
            }
            return list.ToArray();
        }

        public static string MakeShortUdid(string udid)
        {
            int length = Math.Min(6, udid.Length);
            if (length == 0)
            {
                return "";
            }
            return udid.Substring(0, length);
        }

        private static bool MatchesAny(string dst, Regex[] ignoreList)
        {
            <MatchesAny>c__AnonStorey1 storey = new <MatchesAny>c__AnonStorey1 {
                dst = dst
            };
            return Array.Exists<Regex>(ignoreList, new Predicate<Regex>(storey.<>m__0));
        }

        public static void MoveDirectoryRecursive(string source, string target, bool overwrite)
        {
            FileUtil.CopyDirectoryRecursive(source, target, overwrite);
            FileUtil.DeleteFileOrDirectory(source);
        }

        public static void MoveFileOrDirectoryIfExists(string src, string dst)
        {
            if (File.Exists(src) || Directory.Exists(src))
            {
                FileUtil.MoveFileOrDirectory(src, dst);
            }
        }

        public static void MoveRecursiveWithMatchList(string src, string dst, Regex[] matchList)
        {
            <MoveRecursiveWithMatchList>c__AnonStorey3 storey = new <MoveRecursiveWithMatchList>c__AnonStorey3 {
                matchList = matchList,
                dst = dst
            };
            EnumerateCallback fileCallback = new EnumerateCallback(storey.<>m__0);
            EnumerateFilesRecursivelyWithIgnoreList(src, fileCallback, fileCallback, null);
        }

        private static void NormalizePath(List<string> components)
        {
            int index = 0;
            while (index < components.Count)
            {
                if ((components[index] == ".") || (components[index] == ""))
                {
                    components.RemoveAt(index);
                }
                else if (components[index] == "..")
                {
                    if (index == 0)
                    {
                        throw new Exception("Path refers to parent of the root node");
                    }
                    components.RemoveRange(index - 1, 2);
                    index--;
                }
                else
                {
                    index++;
                }
            }
        }

        public static bool ParseVersion(string input, out Version version)
        {
            try
            {
                version = new Version(input);
                return true;
            }
            catch
            {
                version = new Version();
                return false;
            }
        }

        public static bool PlatformSupportsSymlinks()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return false;
            }
            return true;
        }

        public static void ReplaceFileOrDirectoryCopy(string src, string dst)
        {
            if (File.Exists(dst) || Directory.Exists(dst))
            {
                FileUtil.DeleteFileOrDirectory(dst);
            }
            FileUtil.CopyFileOrDirectory(src, dst);
        }

        public static void ReplaceFileOrDirectoryMove(string src, string dst)
        {
            if (File.Exists(dst) || Directory.Exists(dst))
            {
                FileUtil.DeleteFileOrDirectory(dst);
            }
            FileUtil.MoveFileOrDirectory(src, dst);
        }

        public static void SymlinkFileAbsolute(string src, string dst)
        {
            SymlinkFileImpl(src, dst, true);
        }

        private static void SymlinkFileImpl(string src, string dst, bool absolute)
        {
            if (!PlatformSupportsSymlinks())
            {
                throw new Exception("Can't symlink files on this platform");
            }
            if (File.Exists(dst))
            {
                File.Delete(dst);
            }
            bool flag = false;
            char[] trimChars = new char[] { '/', '\\' };
            src = Path.GetFullPath(src).TrimEnd(trimChars);
            char[] chArray2 = new char[] { '/', '\\' };
            dst = Path.GetFullPath(dst).TrimEnd(chArray2);
            if (!src.Contains("'") && !dst.Contains("'"))
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                if (!absolute)
                {
                    currentDirectory = Directory.GetParent(dst).FullName;
                    src = GetRelativePath(currentDirectory, src);
                    dst = Path.GetFileName(dst);
                }
                ProcessStartInfo info2 = new ProcessStartInfo("/bin/ln");
                string[] textArray1 = new string[] { "-s '", src, "' '", dst, "'" };
                info2.Arguments = string.Concat(textArray1);
                info2.UseShellExecute = false;
                info2.CreateNoWindow = true;
                info2.WorkingDirectory = currentDirectory;
                ProcessStartInfo info = info2;
                using (Process process = new Process())
                {
                    process.StartInfo = info;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                UnityEngine.Debug.LogWarning("Failed symlinking runtime library. Falling back to copy.");
                FileUtil.CopyFileOrDirectory(src, dst);
            }
        }

        public static void SymlinkFileRelative(string src, string dst)
        {
            SymlinkFileImpl(src, dst, false);
        }

        [CompilerGenerated]
        private sealed class <CopyRecursiveWithIgnoreList>c__AnonStorey2
        {
            internal string dst;

            internal void <>m__0(string path, string curLevel)
            {
                FileUtil.CopyFileOrDirectory(path, Path.Combine(Path.Combine(this.dst, curLevel), Path.GetFileName(path)));
            }

            internal void <>m__1(string path, string curLevel)
            {
                Directory.CreateDirectory(Path.Combine(this.dst, curLevel));
            }
        }

        [CompilerGenerated]
        private sealed class <DeleteRecursiveWithMatchList>c__AnonStorey4
        {
            internal Regex[] matchList;

            internal void <>m__0(string path, string curLevel)
            {
                if (Utils.MatchesAny(Path.GetFileName(path), this.matchList))
                {
                    FileUtil.DeleteFileOrDirectory(path);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindiOSDevice>c__AnonStorey0
        {
            internal string modelId;
            internal int productId;
            internal int revision;
            internal int vendorId;

            internal bool <>m__0(iOSDeviceDescription d) => 
                (((d.vendorId == this.vendorId) && (d.productId == this.productId)) && (d.revision == this.revision));

            internal bool <>m__1(iOSDeviceDescription d) => 
                ((d.productId == this.productId) && (d.modelId == this.modelId));

            internal bool <>m__2(iOSDeviceDescription d) => 
                (d.modelId == this.modelId);
        }

        [CompilerGenerated]
        private sealed class <MatchesAny>c__AnonStorey1
        {
            internal string dst;

            internal bool <>m__0(Regex val) => 
                val.IsMatch(this.dst);
        }

        [CompilerGenerated]
        private sealed class <MoveRecursiveWithMatchList>c__AnonStorey3
        {
            internal string dst;
            internal Regex[] matchList;

            internal void <>m__0(string path, string curLevel)
            {
                if (Utils.MatchesAny(Path.GetFileName(path), this.matchList))
                {
                    string str = Path.Combine(this.dst, curLevel);
                    if (!Directory.Exists(str))
                    {
                        Directory.CreateDirectory(str);
                    }
                    FileUtil.MoveFileOrDirectory(path, Path.Combine(str, Path.GetFileName(path)));
                }
            }
        }

        internal delegate void EnumerateCallback(string path, string curLevel);
    }
}

