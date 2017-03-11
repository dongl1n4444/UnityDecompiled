namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal static class DiagnosticSwitchPreferences
    {
        [CompilerGenerated]
        private static Comparison<DiagnosticSwitch> <>f__am$cache0;
        private const uint kMaxRangeForSlider = 10;
        private static string s_FilterString = string.Empty;
        private static readonly Resources s_Resources = new Resources();
        private static Vector2 s_ScrollOffset;

        private static bool DisplaySwitch(DiagnosticSwitch diagnosticSwitch)
        {
            GUIContent label = new GUIContent(diagnosticSwitch.name, diagnosticSwitch.name + "\n\n" + diagnosticSwitch.description);
            bool flag = !object.Equals(diagnosticSwitch.value, diagnosticSwitch.persistentValue);
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope(new GUILayoutOption[0]))
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect position = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label, options);
                Rect rect2 = new Rect(position.x, position.y, position.height, position.height);
                position.xMin += rect2.width + 3f;
                if (flag && (Event.current.type == EventType.Repaint))
                {
                    GUI.DrawTexture(rect2, s_Resources.smallWarningIcon);
                }
                if (diagnosticSwitch.value is bool)
                {
                    diagnosticSwitch.persistentValue = EditorGUI.Toggle(position, label, (bool) diagnosticSwitch.persistentValue);
                }
                else if (diagnosticSwitch.enumInfo != null)
                {
                    if (diagnosticSwitch.enumInfo.isFlags)
                    {
                        int num = 0;
                        foreach (int num2 in diagnosticSwitch.enumInfo.values)
                        {
                            num |= num2;
                        }
                        string[] names = diagnosticSwitch.enumInfo.names;
                        int[] values = diagnosticSwitch.enumInfo.values;
                        if (diagnosticSwitch.enumInfo.values[0] == 0)
                        {
                            names = new string[names.Length - 1];
                            values = new int[values.Length - 1];
                            Array.Copy(diagnosticSwitch.enumInfo.names, 1, names, 0, names.Length);
                            Array.Copy(diagnosticSwitch.enumInfo.values, 1, values, 0, values.Length);
                        }
                        diagnosticSwitch.persistentValue = EditorGUI.MaskFieldInternal(position, label, (int) diagnosticSwitch.persistentValue, names, values, EditorStyles.popup) & num;
                    }
                    else
                    {
                        diagnosticSwitch.persistentValue = EditorGUI.IntPopup(position, label, (int) diagnosticSwitch.persistentValue, diagnosticSwitch.enumInfo.guiNames, diagnosticSwitch.enumInfo.values);
                    }
                }
                else if (diagnosticSwitch.value is uint)
                {
                    uint minValue = (uint) diagnosticSwitch.minValue;
                    uint maxValue = (uint) diagnosticSwitch.maxValue;
                    if ((((maxValue - minValue) <= 10) && ((maxValue - minValue) > 0)) && ((minValue < 0x7fffffff) && (maxValue < 0x7fffffff)))
                    {
                        diagnosticSwitch.persistentValue = (uint) EditorGUI.IntSlider(position, label, (int) ((uint) diagnosticSwitch.persistentValue), (int) minValue, (int) maxValue);
                    }
                    else
                    {
                        diagnosticSwitch.persistentValue = (uint) EditorGUI.IntField(position, label, (int) ((uint) diagnosticSwitch.persistentValue));
                    }
                }
                else if (diagnosticSwitch.value is int)
                {
                    int leftValue = (int) diagnosticSwitch.minValue;
                    int rightValue = (int) diagnosticSwitch.maxValue;
                    if ((((rightValue - leftValue) <= 10L) && ((rightValue - leftValue) > 0)) && ((leftValue < 0x7fffffff) && (rightValue < 0x7fffffff)))
                    {
                        diagnosticSwitch.persistentValue = EditorGUI.IntSlider(position, label, (int) diagnosticSwitch.persistentValue, leftValue, rightValue);
                    }
                    else
                    {
                        diagnosticSwitch.persistentValue = EditorGUI.IntField(position, label, (int) diagnosticSwitch.persistentValue);
                    }
                }
                else if (diagnosticSwitch.value is string)
                {
                    diagnosticSwitch.persistentValue = EditorGUI.TextField(position, label, (string) diagnosticSwitch.persistentValue);
                }
                else
                {
                    GUIStyle style = new GUIStyle {
                        normal = { textColor = Color.red }
                    };
                    EditorGUI.LabelField(position, label, new GUIContent("Unsupported type: " + diagnosticSwitch.value.GetType().Name), style);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Debug.SetDiagnosticSwitch(diagnosticSwitch.name, diagnosticSwitch.persistentValue, true);
            }
            return flag;
        }

        private static void DoTopBar()
        {
            using (new EditorGUILayout.HorizontalScope(s_Resources.title, new GUILayoutOption[0]))
            {
                GUILayout.FlexibleSpace();
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(200f) };
                s_FilterString = GUILayout.TextField(s_FilterString, EditorStyles.toolbarSearchField, options);
                if (GUILayout.Button(GUIContent.none, !string.IsNullOrEmpty(s_FilterString) ? EditorStyles.toolbarSearchFieldCancelButton : EditorStyles.toolbarSearchFieldCancelButtonEmpty, new GUILayoutOption[0]))
                {
                    s_FilterString = string.Empty;
                }
            }
        }

        [PreferenceItem("Diagnostics")]
        public static void OnGUI()
        {
            List<DiagnosticSwitch> results = new List<DiagnosticSwitch>();
            Debug.GetDiagnosticSwitches(results);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (a, b) => Comparer<string>.Default.Compare(a.name, b.name);
            }
            results.Sort(<>f__am$cache0);
            DoTopBar();
            bool flag = false;
            using (EditorGUILayout.VerticalScrollViewScope scope = new EditorGUILayout.VerticalScrollViewScope(s_ScrollOffset, false, GUI.skin.verticalScrollbar, s_Resources.scrollArea, new GUILayoutOption[0]))
            {
                string filterString = s_FilterString.ToLowerInvariant().Trim();
                for (int i = 0; i < results.Count; i++)
                {
                    if (PassesFilter(results[i], filterString))
                    {
                        flag |= DisplaySwitch(results[i]);
                    }
                }
                s_ScrollOffset = scope.scrollPosition;
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(40f) };
            Rect position = GUILayoutUtility.GetRect(s_Resources.restartNeededWarning, EditorStyles.helpBox, options);
            if (flag)
            {
                EditorGUI.HelpBox(position, s_Resources.restartNeededWarning.text, MessageType.Warning);
            }
        }

        private static bool PassesFilter(DiagnosticSwitch diagnosticSwitch, string filterString) => 
            ((string.IsNullOrEmpty(s_FilterString) || diagnosticSwitch.name.ToLowerInvariant().Contains(filterString)) || diagnosticSwitch.description.ToLowerInvariant().Contains(filterString));

        private class Resources
        {
            public GUIContent restartNeededWarning = new GUIContent("Some settings will not take effect until you restart Unity.");
            public GUIStyle scrollArea = "OL Box";
            public Texture2D smallWarningIcon = EditorGUIUtility.LoadIconRequired("console.warnicon.sml");
            public GUIStyle title = "OL Title";
        }
    }
}

