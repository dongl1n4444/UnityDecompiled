namespace UnityEditor.WindowsStandalone
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class WindowsStandaloneBuildWindowExtension : DesktopStandaloneBuildWindowExtension
    {
        private GUIContent m_CopyPdbFiles = EditorGUIUtility.TextContent("Copy PDB files|Copy pdb files to final destination, contains debugging symbols");

        public override void ShowPlatformBuildOptions()
        {
            base.ShowPlatformBuildOptions();
            UserBuildSettings.copyPDBFiles = EditorGUILayout.Toggle(this.m_CopyPdbFiles, UserBuildSettings.copyPDBFiles, new GUILayoutOption[0]);
        }
    }
}

