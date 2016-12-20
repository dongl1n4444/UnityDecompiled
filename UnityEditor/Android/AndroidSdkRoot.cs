namespace UnityEditor.Android
{
    using Microsoft.Win32;
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AndroidSdkRoot
    {
        internal static string Browse(string sdkPath)
        {
            string title = "Select Android SDK root folder";
            string str2 = sdkPath;
            if (string.IsNullOrEmpty(str2))
            {
                try
                {
                    str2 = GuessLocation();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Exception while trying to guess Android SDK location");
                    Console.WriteLine(exception);
                }
            }
            if (InternalEditorUtility.inBatchMode)
            {
                return str2;
            }
            do
            {
                sdkPath = EditorUtility.OpenFolderPanel(title, str2, "");
                if (sdkPath.Length == 0)
                {
                    return "";
                }
            }
            while (!IsSdkDir(sdkPath));
            return sdkPath;
        }

        private static string GuessLocation()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrEmpty(environmentVariable) && IsSdkDir(environmentVariable))
            {
                return environmentVariable;
            }
            environmentVariable = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            if (!string.IsNullOrEmpty(environmentVariable) && IsSdkDir(environmentVariable))
            {
                return environmentVariable;
            }
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                {
                    environmentVariable = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Android SDK Tools", "Path", "").ToString();
                    if (!string.IsNullOrEmpty(environmentVariable) && IsSdkDir(environmentVariable))
                    {
                        return environmentVariable;
                    }
                    environmentVariable = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Android SDK Tools", "Path", "").ToString();
                    if (!string.IsNullOrEmpty(environmentVariable) && IsSdkDir(environmentVariable))
                    {
                        return environmentVariable;
                    }
                    string str3 = Environment.GetEnvironmentVariable("ProgramFiles");
                    if (!string.IsNullOrEmpty(str3) && Directory.Exists(str3))
                    {
                        return str3;
                    }
                    break;
                }
                case RuntimePlatform.OSXEditor:
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

        internal static bool IsSdkDir(string path)
        {
            return ((Directory.Exists(Path.Combine(path, "platform-tools")) || File.Exists(Path.Combine(Path.Combine(path, "tools"), "android"))) || File.Exists(Path.Combine(Path.Combine(path, "tools"), "android.bat")));
        }

        public static string DownloadUrl
        {
            get
            {
                return "https://developer.android.com/sdk/index.html#Other";
            }
        }
    }
}

