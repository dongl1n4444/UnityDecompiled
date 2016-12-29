namespace UnityEditor.Android
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class AndroidPreferenceWindowExtension : IPreferenceWindowExtension
    {
        [CompilerGenerated]
        private static BrowseFunc <>f__am$cache0;
        [CompilerGenerated]
        private static BrowseFunc <>f__am$cache1;
        [CompilerGenerated]
        private static BrowseFunc <>f__am$cache2;
        private string m_AndroidNdkPath = string.Empty;
        private string m_AndroidSdkPath = string.Empty;
        private string m_JdkPath = string.Empty;

        public bool HasExternalApplications() => 
            true;

        public void ReadPreferences()
        {
            this.m_AndroidSdkPath = EditorPrefs.GetString("AndroidSdkRoot");
            this.m_AndroidNdkPath = EditorPrefs.GetString("AndroidNdkRoot");
            this.m_JdkPath = EditorPrefs.GetString("JdkPath");
        }

        public void ShowExternalApplications()
        {
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Android"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = path => AndroidSdkRoot.Browse(path);
            }
            this.ToolLocation("SDK", ref this.m_AndroidSdkPath, AndroidSdkRoot.DownloadUrl, <>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = path => AndroidJavaTools.BrowseForJDK(path);
            }
            this.ToolLocation("JDK", ref this.m_JdkPath, AndroidJavaTools.JDKDownloadUrl, <>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = path => AndroidNdkRoot.Browse(path);
            }
            this.ToolLocation("NDK", ref this.m_AndroidNdkPath, AndroidNdkRoot.DownloadUrl, <>f__am$cache2);
            EditorGUILayout.HelpBox($"IL2CPP requires that you have Android NDK {"r10e"} installed.
If you are not targeting IL2CPP you can leave this field empty.", MessageType.Info);
        }

        private void ToolLocation(string title, ref string pathRef, string downloadURL, BrowseFunc Browse)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(30f) };
            GUILayout.Label(title, options);
            pathRef = EditorGUILayout.TextField(pathRef, new GUILayoutOption[0]);
            if (GUILayout.Button("Browse", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                string str = Browse(pathRef);
                if (!string.IsNullOrEmpty(str))
                {
                    pathRef = str;
                    this.WritePreferences();
                    this.ReadPreferences();
                    GUI.FocusControl("");
                }
            }
            if (GUILayout.Button("Download", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                Application.OpenURL(downloadURL);
            }
            GUILayout.EndHorizontal();
        }

        public void WritePreferences()
        {
            EditorPrefs.SetString("AndroidSdkRoot", this.m_AndroidSdkPath);
            EditorPrefs.SetString("AndroidNdkRoot", this.m_AndroidNdkPath);
            EditorPrefs.SetString("JdkPath", this.m_JdkPath);
        }

        public delegate string BrowseFunc(string path);
    }
}

