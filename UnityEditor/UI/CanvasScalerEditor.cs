namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the CanvasScaler component.</para>
    /// </summary>
    [CustomEditor(typeof(CanvasScaler), true), CanEditMultipleObjects]
    public class CanvasScalerEditor : Editor
    {
        private const int kSliderEndpointLabelsHeight = 12;
        private SerializedProperty m_DefaultSpriteDPI;
        private SerializedProperty m_DynamicPixelsPerUnit;
        private SerializedProperty m_FallbackScreenDPI;
        private SerializedProperty m_MatchWidthOrHeight;
        private SerializedProperty m_PhysicalUnit;
        private SerializedProperty m_ReferencePixelsPerUnit;
        private SerializedProperty m_ReferenceResolution;
        private SerializedProperty m_ScaleFactor;
        private SerializedProperty m_ScreenMatchMode;
        private SerializedProperty m_UiScaleMode;
        private static Styles s_Styles;

        private static void DualLabeledSlider(Rect position, SerializedProperty property, GUIContent mainLabel, GUIContent labelLeft, GUIContent labelRight)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect rect = position;
            position.y += 12f;
            position.xMin += EditorGUIUtility.labelWidth;
            position.xMax -= EditorGUIUtility.fieldWidth;
            GUI.Label(position, labelLeft, s_Styles.leftAlignedLabel);
            GUI.Label(position, labelRight, s_Styles.rightAlignedLabel);
            EditorGUI.PropertyField(rect, property, mainLabel);
        }

        protected virtual void OnEnable()
        {
            this.m_UiScaleMode = base.serializedObject.FindProperty("m_UiScaleMode");
            this.m_ScaleFactor = base.serializedObject.FindProperty("m_ScaleFactor");
            this.m_ReferenceResolution = base.serializedObject.FindProperty("m_ReferenceResolution");
            this.m_ScreenMatchMode = base.serializedObject.FindProperty("m_ScreenMatchMode");
            this.m_MatchWidthOrHeight = base.serializedObject.FindProperty("m_MatchWidthOrHeight");
            this.m_PhysicalUnit = base.serializedObject.FindProperty("m_PhysicalUnit");
            this.m_FallbackScreenDPI = base.serializedObject.FindProperty("m_FallbackScreenDPI");
            this.m_DefaultSpriteDPI = base.serializedObject.FindProperty("m_DefaultSpriteDPI");
            this.m_DynamicPixelsPerUnit = base.serializedObject.FindProperty("m_DynamicPixelsPerUnit");
            this.m_ReferencePixelsPerUnit = base.serializedObject.FindProperty("m_ReferencePixelsPerUnit");
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            bool flag = true;
            bool flag2 = false;
            bool flag3 = (base.target as CanvasScaler).GetComponent<Canvas>().renderMode == UnityEngine.RenderMode.WorldSpace;
            for (int i = 0; i < base.targets.Length; i++)
            {
                Canvas component = (base.targets[i] as CanvasScaler).GetComponent<Canvas>();
                if (!component.isRootCanvas)
                {
                    flag = false;
                    break;
                }
                if ((flag3 && (component.renderMode != UnityEngine.RenderMode.WorldSpace)) || (!flag3 && (component.renderMode == UnityEngine.RenderMode.WorldSpace)))
                {
                    flag2 = true;
                    break;
                }
            }
            if (!flag)
            {
                EditorGUILayout.HelpBox("Non-root Canvases will not be scaled.", MessageType.Warning);
            }
            else
            {
                base.serializedObject.Update();
                EditorGUI.showMixedValue = flag2;
                using (new EditorGUI.DisabledScope(flag3 || flag2))
                {
                    if (flag3 || flag2)
                    {
                        string[] displayedOptions = new string[] { "World" };
                        EditorGUILayout.Popup(s_Styles.uiScaleModeContent.text, 0, displayedOptions, new GUILayoutOption[0]);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(this.m_UiScaleMode, s_Styles.uiScaleModeContent, new GUILayoutOption[0]);
                    }
                }
                EditorGUI.showMixedValue = false;
                if (!flag2 && (flag3 || !this.m_UiScaleMode.hasMultipleDifferentValues))
                {
                    EditorGUILayout.Space();
                    if (flag3)
                    {
                        EditorGUILayout.PropertyField(this.m_DynamicPixelsPerUnit, new GUILayoutOption[0]);
                    }
                    else if (this.m_UiScaleMode.enumValueIndex == 0)
                    {
                        EditorGUILayout.PropertyField(this.m_ScaleFactor, new GUILayoutOption[0]);
                    }
                    else if (this.m_UiScaleMode.enumValueIndex == 1)
                    {
                        EditorGUILayout.PropertyField(this.m_ReferenceResolution, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_ScreenMatchMode, new GUILayoutOption[0]);
                        if ((this.m_ScreenMatchMode.enumValueIndex == 0) && !this.m_ScreenMatchMode.hasMultipleDifferentValues)
                        {
                            DualLabeledSlider(EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 12f, new GUILayoutOption[0]), this.m_MatchWidthOrHeight, s_Styles.matchContent, s_Styles.widthContent, s_Styles.heightContent);
                        }
                    }
                    else if (this.m_UiScaleMode.enumValueIndex == 2)
                    {
                        EditorGUILayout.PropertyField(this.m_PhysicalUnit, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_FallbackScreenDPI, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_DefaultSpriteDPI, new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_ReferencePixelsPerUnit, new GUILayoutOption[0]);
                }
                base.serializedObject.ApplyModifiedProperties();
            }
        }

        private class Styles
        {
            public GUIContent heightContent = new GUIContent("Height");
            public GUIStyle leftAlignedLabel = new GUIStyle(EditorStyles.label);
            public GUIContent matchContent = new GUIContent("Match");
            public GUIStyle rightAlignedLabel = new GUIStyle(EditorStyles.label);
            public GUIContent uiScaleModeContent = new GUIContent("UI Scale Mode");
            public GUIContent widthContent = new GUIContent("Width");

            public Styles()
            {
                this.rightAlignedLabel.alignment = TextAnchor.MiddleRight;
            }
        }
    }
}

