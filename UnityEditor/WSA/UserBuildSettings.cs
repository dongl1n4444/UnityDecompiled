namespace UnityEditor.WSA
{
    using System;
    using UnityEditor;

    public static class UserBuildSettings
    {
        private static readonly string kSettingsCopyReferences = "CopyReferences";

        public static bool copyReferences
        {
            get
            {
                return (EditorUserBuildSettings.GetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WSAPlayer), kSettingsCopyReferences).ToLower() == "true");
            }
            set
            {
                EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WSAPlayer), kSettingsCopyReferences, value.ToString().ToLower());
            }
        }
    }
}

