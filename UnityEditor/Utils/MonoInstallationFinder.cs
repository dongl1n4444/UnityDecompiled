namespace UnityEditor.Utils
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal class MonoInstallationFinder
    {
        public const string MonoBleedingEdgeInstallation = "MonoBleedingEdge";
        public const string MonoInstallation = "Mono";

        public static string GetEtcDirectory(string monoInstallation) => 
            Path.Combine(GetMonoInstallation(monoInstallation), Path.Combine("etc", "mono"));

        public static string GetFrameWorksFolder()
        {
            string str = FileUtil.NiceWinPath(EditorApplication.applicationPath);
            if ((Application.platform != RuntimePlatform.WindowsEditor) && (Application.platform == RuntimePlatform.OSXEditor))
            {
                return Path.Combine(str, "Contents");
            }
            return Path.Combine(Path.GetDirectoryName(str), "Data");
        }

        public static string GetMonoBleedingEdgeInstallation() => 
            GetMonoInstallation("MonoBleedingEdge");

        public static string GetMonoInstallation() => 
            GetMonoInstallation("Mono");

        public static string GetMonoInstallation(string monoName) => 
            Path.Combine(GetFrameWorksFolder(), monoName);

        public static string GetProfileDirectory(string profile) => 
            Path.Combine(GetMonoInstallation(), Path.Combine("lib", Path.Combine("mono", profile)));

        public static string GetProfileDirectory(string profile, string monoInstallation) => 
            Path.Combine(GetMonoInstallation(monoInstallation), Path.Combine("lib", Path.Combine("mono", profile)));

        public static string GetProfilesDirectory(string monoInstallation) => 
            Path.Combine(GetMonoInstallation(monoInstallation), Path.Combine("lib", "mono"));
    }
}

