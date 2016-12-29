namespace UnityEditor.Tizen
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class TizenPreferenceWindowExtension : IPreferenceWindowExtension
    {
        private string m_TizenSdkPath = string.Empty;

        public bool HasExternalApplications() => 
            true;

        public void ReadPreferences()
        {
            this.m_TizenSdkPath = EditorPrefs.GetString("TizenSdkRoot");
        }

        public void ShowExternalApplications()
        {
            GUIStyle followingStyle = "MiniPopup";
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel("Tizen SDK Location", followingStyle);
            string text = !string.IsNullOrEmpty(this.m_TizenSdkPath) ? this.m_TizenSdkPath : "Browse...";
            GUIContent content = new GUIContent(text);
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(GUIContent.none, followingStyle), content, FocusType.Passive, followingStyle))
            {
                string str2 = TizenSdkRoot.Browse(this.m_TizenSdkPath);
                if (!string.IsNullOrEmpty(str2))
                {
                    this.m_TizenSdkPath = str2;
                    this.WritePreferences();
                    this.ReadPreferences();
                }
            }
            GUILayout.EndHorizontal();
        }

        public void WritePreferences()
        {
            EditorPrefs.SetString("TizenSdkRoot", this.m_TizenSdkPath);
        }
    }
}

