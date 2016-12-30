namespace UnityEditor.Android
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Android.Il2Cpp;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class AndroidNdkRoot
    {
        public static string Browse(string ndkPath)
        {
            string title = $"Select Android NDK {"r10e"} root folder";
            string str2 = ndkPath;
            if (string.IsNullOrEmpty(str2))
            {
                try
                {
                    str2 = GuessLocation();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Exception while trying to guess Android NDK location");
                    Console.WriteLine(exception);
                }
            }
            if (InternalEditorUtility.inBatchMode)
            {
                return str2;
            }
            do
            {
                ndkPath = EditorUtility.OpenFolderPanel(title, ndkPath, "");
                if (ndkPath.Length == 0)
                {
                    return "";
                }
            }
            while (!VerifyNdkDir(ndkPath));
            return ndkPath;
        }

        private static string GuessLocation()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ANDROID_NDK_ROOT");
            if (!string.IsNullOrEmpty(environmentVariable) && VerifyNdkDir(environmentVariable))
            {
                return environmentVariable;
            }
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                {
                    string str3 = Environment.GetEnvironmentVariable("ProgramFiles");
                    if (!string.IsNullOrEmpty(str3) && Directory.Exists(str3))
                    {
                        return str3;
                    }
                    break;
                }
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                {
                    string str4 = Environment.GetEnvironmentVariable("HOME");
                    if (!string.IsNullOrEmpty(str4) && Directory.Exists(str4))
                    {
                        return str4;
                    }
                    break;
                }
            }
            return "";
        }

        private static bool Is32BitProcessOn64BitOS()
        {
            bool flag;
            IsWow64Process(Process.GetCurrentProcess().Handle, out flag);
            return flag;
        }

        public static bool Is64BitWindows() => 
            ((Application.platform == RuntimePlatform.WindowsEditor) && ((IntPtr.Size == 8) || ((IntPtr.Size == 4) && Is32BitProcessOn64BitOS())));

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, out bool lpSystemInfo);
        public static bool VerifyNdkDir(string ndkPath)
        {
            string fileName = Path.Combine(ndkPath, "RELEASE.TXT");
            string message = "Unable to detect NDK version, please pick a different folder.";
            try
            {
                FileInfo info = new FileInfo(fileName);
                if (info.Exists && (info.Length < 0x40L))
                {
                    string str3 = File.ReadAllText(info.FullName).Trim();
                    if (str3.Contains(AndroidIl2CppNativeCodeBuilder.AndroidNdkVersionString))
                    {
                        return true;
                    }
                    message = $"NDK {str3} is incompatible with IL2CPP. IL2CPP requires {AndroidIl2CppNativeCodeBuilder.AndroidNdkVersionString}.";
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to stat '{0}'", fileName);
            }
            Version componentVersion = AndroidComponentVersion.GetComponentVersion(ndkPath);
            if (!componentVersion.Equals("0"))
            {
                message = $"NDK {componentVersion} is incompatible with IL2CPP. IL2CPP requires {AndroidIl2CppNativeCodeBuilder.AndroidNdkVersionString}.";
            }
            switch (EditorUtility.DisplayDialogComplex("Invalid NDK version", message, "Ok", "Cancel", "Download"))
            {
                case 1:
                    throw new UnityException(message);

                case 2:
                    Application.OpenURL(DownloadUrl);
                    break;
            }
            return false;
        }

        public static string DownloadUrl
        {
            get
            {
                string str = "http://dl.google.com/android/ndk/";
                string str2 = $"android-ndk-{"r10e"}";
                RuntimePlatform platform = Application.platform;
                if (platform != RuntimePlatform.OSXEditor)
                {
                    if (platform == RuntimePlatform.WindowsEditor)
                    {
                        return Path.Combine(str, str2 + (!Is64BitWindows() ? "-windows-x86.exe" : "-windows-x86_64.exe"));
                    }
                    if (platform == RuntimePlatform.LinuxEditor)
                    {
                        return Path.Combine(str, str2 + "-linux-x86_64.bin");
                    }
                }
                else
                {
                    return Path.Combine(str, str2 + "-darwin-x86_64.bin");
                }
                return "https://developer.android.com/tools/sdk/ndk/index.html";
            }
        }
    }
}

