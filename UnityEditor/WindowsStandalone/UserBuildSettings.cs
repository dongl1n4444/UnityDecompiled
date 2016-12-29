namespace UnityEditor.WindowsStandalone
{
    using System;
    using UnityEditor;

    public static class UserBuildSettings
    {
        private static readonly string kSettingCopyPDBFiles = "CopyPDBFiles";

        public static bool copyPDBFiles
        {
            get => 
                (EditorUserBuildSettings.GetPlatformSettings(DesktopStandaloneUserBuildSettings.PlatformName, kSettingCopyPDBFiles).ToLower() == "true");
            set
            {
                EditorUserBuildSettings.SetPlatformSettings(DesktopStandaloneUserBuildSettings.PlatformName, kSettingCopyPDBFiles, value.ToString().ToLower());
            }
        }
    }
}

