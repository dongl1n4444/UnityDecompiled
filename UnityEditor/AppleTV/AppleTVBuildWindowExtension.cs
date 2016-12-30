namespace UnityEditor.AppleTV
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class AppleTVBuildWindowExtension : DefaultBuildWindowExtension
    {
        private static readonly GUIContent[] kOptionDescriptions;
        private static readonly iOSBuildType[] kOptionsOrder;

        static AppleTVBuildWindowExtension()
        {
            iOSBuildType[] typeArray1 = new iOSBuildType[2];
            typeArray1[0] = iOSBuildType.Release;
            kOptionsOrder = typeArray1;
            kOptionDescriptions = new GUIContent[] { EditorGUIUtility.TextContent("Release"), EditorGUIUtility.TextContent("Debug") };
        }

        public override void ShowInternalPlatformBuildOptions()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            EditorUserBuildSettings.symlinkTrampoline = GUILayout.Toggle(EditorUserBuildSettings.symlinkTrampoline, "Symlink Trampoline", options);
        }

        public override void ShowPlatformBuildOptions()
        {
            iOSBuildType iOSBuildConfigType = EditorUserBuildSettings.iOSBuildConfigType;
            EditorUserBuildSettings.iOSBuildConfigType = PlayerSettingsEditor.BuildEnumPopup<iOSBuildType>(EditorGUIUtility.TextContent("Run in Xcode as"), iOSBuildConfigType, kOptionsOrder, kOptionDescriptions);
        }
    }
}

