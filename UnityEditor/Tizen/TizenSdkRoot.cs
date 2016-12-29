namespace UnityEditor.Tizen
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal class TizenSdkRoot
    {
        internal static string Browse(string sdkPath)
        {
            string defaultName = "tizen-sdk";
            string title = "Select Tizen SDK root folder";
            string folder = !string.IsNullOrEmpty(sdkPath) ? sdkPath : GuessPerPlatform();
            do
            {
                sdkPath = EditorUtility.OpenFolderPanel(title, folder, defaultName);
                if (sdkPath.Length == 0)
                {
                    return "";
                }
            }
            while (!IsSdkDir(sdkPath));
            return sdkPath;
        }

        private static string GuessPerPlatform()
        {
            RuntimePlatform platform = Application.platform;
            if (platform != RuntimePlatform.WindowsEditor)
            {
                if (platform != RuntimePlatform.OSXEditor)
                {
                    return "";
                }
            }
            else
            {
                string str = Environment.GetEnvironmentVariable("ProgramFiles");
                string str2 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                if (!string.IsNullOrEmpty(str2))
                {
                    return str2;
                }
                if (string.IsNullOrEmpty(str))
                {
                    goto Label_0088;
                }
                return str;
            }
            string environmentVariable = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(environmentVariable))
            {
                return environmentVariable;
            }
        Label_0088:
            return "";
        }

        internal static bool IsSdkDir(string path) => 
            Directory.Exists(Path.Combine(path, "tools"));
    }
}

