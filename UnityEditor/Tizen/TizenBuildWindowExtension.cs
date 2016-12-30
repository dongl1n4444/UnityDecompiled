namespace UnityEditor.Tizen
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class TizenBuildWindowExtension : DefaultBuildWindowExtension
    {
        private GUIContent mobileTextureSubtarget = EditorGUIUtility.TextContent("Texture Compression");
        private MobileTextureSubtarget[] mobileTextureSubtargets = new MobileTextureSubtarget[] { MobileTextureSubtarget.Generic };
        private GUIContent[] mobileTextureSubtargetStrings = new GUIContent[] { EditorGUIUtility.TextContent("Don't override"), EditorGUIUtility.TextContent("ATC"), EditorGUIUtility.TextContent("ETC (default)"), EditorGUIUtility.TextContent("ETC2 (GLES 3.0)"), EditorGUIUtility.TextContent("ASTC") };

        public override void ShowPlatformBuildOptions()
        {
            int index = Array.IndexOf<MobileTextureSubtarget>(this.mobileTextureSubtargets, EditorUserBuildSettings.tizenBuildSubtarget);
            if (index == -1)
            {
                index = 0;
            }
            index = EditorGUILayout.Popup(this.mobileTextureSubtarget, index, this.mobileTextureSubtargetStrings, new GUILayoutOption[0]);
            EditorUserBuildSettings.tizenBuildSubtarget = this.mobileTextureSubtargets[index];
        }
    }
}

