using System;
using UnityEditor;

internal static class DesktopStandaloneSettings
{
    private static readonly string kSettingCopyPDBFiles = "CopyPDBFiles";

    internal static bool CopyPDBFiles
    {
        get => 
            (EditorUserBuildSettings.GetPlatformSettings(PlatformName, kSettingCopyPDBFiles).ToLower() == "true");
        set
        {
            EditorUserBuildSettings.SetPlatformSettings(PlatformName, kSettingCopyPDBFiles, value.ToString().ToLower());
        }
    }

    internal static string PlatformName =>
        "Standalone";
}

