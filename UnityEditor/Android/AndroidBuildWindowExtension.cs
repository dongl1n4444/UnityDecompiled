namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class AndroidBuildWindowExtension : DefaultBuildWindowExtension
    {
        private GUIContent createProject = EditorGUIUtility.TextContent("Export Project");
        private GUIContent[] exportTargetStrings = new GUIContent[] { EditorGUIUtility.TextContent("Internal (Default)"), EditorGUIUtility.TextContent("Gradle (New)"), EditorGUIUtility.TextContent("ADT (Legacy)") };
        private GUIContent mobileTextureSubtarget = EditorGUIUtility.TextContent("Texture Compression");
        private MobileTextureSubtarget[] mobileTextureSubtargets = new MobileTextureSubtarget[] { MobileTextureSubtarget.Generic };
        private GUIContent[] mobileTextureSubtargetStrings = new GUIContent[] { EditorGUIUtility.TextContent("Don't override"), EditorGUIUtility.TextContent("DXT (Tegra)"), EditorGUIUtility.TextContent("PVRTC (PowerVR)"), EditorGUIUtility.TextContent("ATC (Adreno)"), EditorGUIUtility.TextContent("ETC (default)"), EditorGUIUtility.TextContent("ETC2 (GLES 3.0)"), EditorGUIUtility.TextContent("ASTC") };
        private GUIContent targetTypeString = EditorGUIUtility.TextContent("Build System");

        public override bool EnabledBuildAndRunButton() => 
            !EditorUserBuildSettings.exportAsGoogleAndroidProject;

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
            if (androidBuildSystem < 0)
            {
                androidBuildSystem = 0;
            }
            EditorUserBuildSettings.androidBuildSystem = (AndroidBuildSystem) EditorGUILayout.Popup(this.targetTypeString, androidBuildSystem, this.exportTargetStrings, new GUILayoutOption[0]);
            bool installInBuildFolder = EditorUserBuildSettings.installInBuildFolder;
            if ((EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Internal) || installInBuildFolder)
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
        }
    }
}

