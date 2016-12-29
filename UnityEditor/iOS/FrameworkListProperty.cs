namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class FrameworkListProperty : DefaultPluginImporterExtension.Property
    {
        private List<string> m_defaultFrameworks;
        private List<string> m_frequentFrameworks;
        private bool m_isDefaultVisible;
        private SortedDictionary<string, bool> m_isFrequentEnabled;
        private SortedDictionary<string, bool> m_isRareEnabled;
        private bool m_isRareVisible;
        private List<string> m_rareFrameworks;
        private HashSet<string> m_userFrameworks;

        internal FrameworkListProperty(string name, string key, List<string> defaultFrameworks, List<string> frequentFrameworks, List<string> rareFrameworks, string platformName) : base(name, key, "", platformName)
        {
            this.m_isFrequentEnabled = new SortedDictionary<string, bool>();
            this.m_isRareEnabled = new SortedDictionary<string, bool>();
            this.m_userFrameworks = new HashSet<string>();
            this.m_defaultFrameworks = null;
            this.m_frequentFrameworks = null;
            this.m_rareFrameworks = null;
            this.m_defaultFrameworks = defaultFrameworks;
            this.m_frequentFrameworks = frequentFrameworks;
            this.m_rareFrameworks = rareFrameworks;
            this.ResetValues();
        }

        private static GUIContent BuildFrameworksLabel(string label, int enabled) => 
            EditorGUIUtility.TextContent($"{label} ({enabled} enabled)");

        private GUIContent GetDefaultFrameworksLabel() => 
            BuildFrameworksLabel("Default frameworks", this.m_defaultFrameworks.Count);

        private GUIContent GetRareFrameworksLabel()
        {
            int enabled = 0;
            foreach (KeyValuePair<string, bool> pair in this.m_isRareEnabled)
            {
                if (pair.Value)
                {
                    enabled++;
                }
            }
            return BuildFrameworksLabel("Rarely used frameworks", enabled);
        }

        internal override void OnGUI(PluginImporterInspector inspector)
        {
            bool flag = false;
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            GUILayout.Label("Framework dependencies", EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            foreach (KeyValuePair<string, bool> pair in this.m_isFrequentEnabled)
            {
                this.m_isFrequentEnabled[pair.Key] = GUILayout.Toggle(pair.Value, pair.Key, new GUILayoutOption[0]);
            }
            if (EditorGUI.EndChangeCheck())
            {
                flag = true;
            }
            this.m_isDefaultVisible = EditorGUILayout.Foldout(this.m_isDefaultVisible, this.GetDefaultFrameworksLabel(), true);
            if (this.m_isDefaultVisible)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(true))
                {
                    foreach (string str in this.m_defaultFrameworks)
                    {
                        GUILayout.Toggle(true, str, new GUILayoutOption[0]);
                    }
                }
                EditorGUI.indentLevel--;
            }
            this.m_isRareVisible = EditorGUILayout.Foldout(this.m_isRareVisible, this.GetRareFrameworksLabel(), true);
            if (this.m_isRareVisible)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                foreach (KeyValuePair<string, bool> pair2 in this.m_isRareEnabled)
                {
                    this.m_isRareEnabled[pair2.Key] = GUILayout.Toggle(pair2.Value, pair2.Key, new GUILayoutOption[0]);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    flag = true;
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.EndVertical();
            this.UpdateValue();
            GUI.changed = flag;
        }

        internal override void Reset(PluginImporterInspector inspector)
        {
            base.Reset(inspector);
            string platformData = inspector.importer.GetPlatformData(base.platformName, base.key);
            this.ResetValues();
            char[] separator = new char[] { ';' };
            string[] strArray = platformData.Split(separator);
            foreach (string str2 in strArray)
            {
                if (this.m_isFrequentEnabled.ContainsKey(str2))
                {
                    this.m_isFrequentEnabled[str2] = true;
                }
                else if (this.m_isRareEnabled.ContainsKey(str2))
                {
                    this.m_isRareEnabled[str2] = true;
                }
                else if (str2 != "")
                {
                    this.m_userFrameworks.Add(str2);
                }
            }
        }

        internal void ResetValues()
        {
            this.m_userFrameworks.Clear();
            foreach (string str in this.m_frequentFrameworks)
            {
                this.m_isFrequentEnabled[str] = false;
            }
            foreach (string str2 in this.m_rareFrameworks)
            {
                this.m_isRareEnabled[str2] = false;
            }
        }

        internal void UpdateValue()
        {
            base.value = "";
            foreach (KeyValuePair<string, bool> pair in this.m_isFrequentEnabled)
            {
                if (pair.Value)
                {
                    base.value = base.value + pair.Key.Replace(";", "") + ";";
                }
            }
            foreach (KeyValuePair<string, bool> pair2 in this.m_isRareEnabled)
            {
                if (pair2.Value)
                {
                    base.value = base.value + pair2.Key.Replace(";", "") + ";";
                }
            }
            foreach (string str in this.m_userFrameworks)
            {
                base.value = base.value + str.Replace(";", "") + ";";
            }
        }
    }
}

