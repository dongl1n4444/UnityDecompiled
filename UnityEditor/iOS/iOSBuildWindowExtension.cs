namespace UnityEditor.iOS
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class iOSBuildWindowExtension : DefaultBuildWindowExtension
    {
        private static readonly GUIContent[] kOptionDescriptions;
        private static readonly iOSBuildType[] kOptionsOrder;

        static iOSBuildWindowExtension()
        {
            iOSBuildType[] typeArray1 = new iOSBuildType[2];
            typeArray1[0] = iOSBuildType.Release;
            kOptionsOrder = typeArray1;
            kOptionDescriptions = new GUIContent[] { EditorGUIUtility.TextContent("Release"), EditorGUIUtility.TextContent("Debug") };
        }

        public override bool EnabledBuildAndRunButton()
        {
            return (!Unsupported.IsDeveloperBuild() || PostProcessiPhonePlayer.CheckIfPlayerLibIsBuilt());
        }

        public override bool EnabledBuildButton()
        {
            return ((!Unsupported.IsDeveloperBuild() || EditorUserBuildSettings.symlinkLibraries) || PostProcessiPhonePlayer.CheckIfPlayerLibIsBuilt());
        }

        public override void ShowInternalPlatformBuildOptions()
        {
            if (PostProcessiPhonePlayer.CheckIfPlayerLibIsBuilt())
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                EditorUserBuildSettings.symlinkTrampoline = GUILayout.Toggle(EditorUserBuildSettings.symlinkTrampoline, "Symlink Trampoline", options);
            }
            else
            {
                EditorGUILayout.HelpBox("Selected static library variant is not yet built.", MessageType.Warning);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button("Build Player Library", optionArray2))
                {
                    PostProcessiPhonePlayer.BuildPlayerLib();
                }
            }
        }

        public override void ShowPlatformBuildOptions()
        {
            int iOSBuildConfigType = (int) EditorUserBuildSettings.iOSBuildConfigType;
            EditorUserBuildSettings.iOSBuildConfigType = (iOSBuildType) PlayerSettingsEditor.BuildEnumPopup<iOSBuildType>(EditorGUIUtility.TextContent("Run in Xcode as"), iOSBuildConfigType, kOptionsOrder, kOptionDescriptions);
        }
    }
}

