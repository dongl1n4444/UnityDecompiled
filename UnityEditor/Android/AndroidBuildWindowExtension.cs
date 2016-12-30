namespace UnityEditor.Android
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class AndroidBuildWindowExtension : DefaultBuildWindowExtension
    {
        private readonly GUIContent[] buildTypeStrings = new GUIContent[] { EditorGUIUtility.TextContent("Debug"), EditorGUIUtility.TextContent("Development"), EditorGUIUtility.TextContent("Release") };
        private GUIContent createProject = EditorGUIUtility.TextContent("Export Project");
        private readonly GUIContent[] exportTargetStrings;
        private readonly GUIContent m_BuildType = EditorGUIUtility.TextContent("Build Type");
        private readonly bool m_IsDeveloperBuild = Unsupported.IsDeveloperBuild();
        private GUIContent mobileTextureSubtarget = EditorGUIUtility.TextContent("Texture Compression");
        private MobileTextureSubtarget[] mobileTextureSubtargets = new MobileTextureSubtarget[] { MobileTextureSubtarget.Generic };
        private GUIContent[] mobileTextureSubtargetStrings = new GUIContent[] { EditorGUIUtility.TextContent("Don't override"), EditorGUIUtility.TextContent("DXT (Tegra)"), EditorGUIUtility.TextContent("PVRTC (PowerVR)"), EditorGUIUtility.TextContent("ATC (Adreno)"), EditorGUIUtility.TextContent("ETC (default)"), EditorGUIUtility.TextContent("ETC2 (GLES 3.0)"), EditorGUIUtility.TextContent("ASTC") };
        private GUIContent targetTypeString = EditorGUIUtility.TextContent("Build System");

        public AndroidBuildWindowExtension()
        {
            List<GUIContent> list = new List<GUIContent> {
                EditorGUIUtility.TextContent("Internal (Default)"),
                EditorGUIUtility.TextContent("Gradle (New)"),
                EditorGUIUtility.TextContent("ADT (Legacy)")
            };
            if (this.m_IsDeveloperBuild)
            {
                list.Add(EditorGUIUtility.TextContent("Visual Studio (Experimental)"));
            }
            this.exportTargetStrings = list.ToArray();
        }

        public override bool EnabledBuildAndRunButton() => 
            !EditorUserBuildSettings.exportAsGoogleAndroidProject;

        public override bool ShouldDrawDevelopmentPlayerCheckbox() => 
            (!this.m_IsDeveloperBuild || ((EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.VisualStudio) && EditorUserBuildSettings.installInBuildFolder));

        public override void ShowPlatformBuildOptions()
        {
            int index = Array.IndexOf<MobileTextureSubtarget>(this.mobileTextureSubtargets, EditorUserBuildSettings.androidBuildSubtarget);
            if (index == -1)
            {
                index = 0;
            }
            index = EditorGUILayout.Popup(this.mobileTextureSubtarget, index, this.mobileTextureSubtargetStrings, new GUILayoutOption[0]);
            EditorUserBuildSettings.androidBuildSubtarget = this.mobileTextureSubtargets[index];
            int androidBuildSystem = (int) EditorUserBuildSettings.androidBuildSystem;
            if ((androidBuildSystem < 0) || (androidBuildSystem >= this.exportTargetStrings.Length))
            {
                androidBuildSystem = 0;
            }
            EditorUserBuildSettings.androidBuildSystem = (AndroidBuildSystem) EditorGUILayout.Popup(this.targetTypeString, androidBuildSystem, this.exportTargetStrings, new GUILayoutOption[0]);
            bool installInBuildFolder = EditorUserBuildSettings.installInBuildFolder;
            if (EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.VisualStudio)
            {
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            }
            else if ((EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Internal) || installInBuildFolder)
            {
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            }
            else if (EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.ADT)
            {
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            }
            GUI.enabled = (EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Gradle) && !installInBuildFolder;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = EditorGUILayout.Toggle(this.createProject, EditorUserBuildSettings.exportAsGoogleAndroidProject, new GUILayoutOption[0]);
            GUI.enabled = true;
            if (!this.ShouldDrawDevelopmentPlayerCheckbox())
            {
                int androidBuildType = (int) EditorUserBuildSettings.androidBuildType;
                if ((androidBuildType < 0) || (androidBuildType >= this.buildTypeStrings.Length))
                {
                    androidBuildType = !EditorUserBuildSettings.development ? 2 : 1;
                }
                EditorUserBuildSettings.androidBuildType = (AndroidBuildType) EditorGUILayout.Popup(this.m_BuildType, androidBuildType, this.buildTypeStrings, new GUILayoutOption[0]);
                EditorUserBuildSettings.development = EditorUserBuildSettings.androidBuildType != AndroidBuildType.Release;
            }
        }
    }
}

