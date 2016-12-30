namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>The Editor for the HorizontalOrVerticalLayoutGroup class.</para>
    /// </summary>
    [CustomEditor(typeof(HorizontalOrVerticalLayoutGroup), true), CanEditMultipleObjects]
    public class HorizontalOrVerticalLayoutGroupEditor : Editor
    {
        private SerializedProperty m_ChildAlignment;
        private SerializedProperty m_ChildControlHeight;
        private SerializedProperty m_ChildControlWidth;
        private SerializedProperty m_ChildForceExpandHeight;
        private SerializedProperty m_ChildForceExpandWidth;
        private SerializedProperty m_Padding;
        private SerializedProperty m_Spacing;

        protected virtual void OnEnable()
        {
            this.m_Padding = base.serializedObject.FindProperty("m_Padding");
            this.m_Spacing = base.serializedObject.FindProperty("m_Spacing");
            this.m_ChildAlignment = base.serializedObject.FindProperty("m_ChildAlignment");
            this.m_ChildControlWidth = base.serializedObject.FindProperty("m_ChildControlWidth");
            this.m_ChildControlHeight = base.serializedObject.FindProperty("m_ChildControlHeight");
            this.m_ChildForceExpandWidth = base.serializedObject.FindProperty("m_ChildForceExpandWidth");
            this.m_ChildForceExpandHeight = base.serializedObject.FindProperty("m_ChildForceExpandHeight");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Padding, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Spacing, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ChildAlignment, true, new GUILayoutOption[0]);
            Rect position = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), -1, new GUIContent("Control Child Size"));
            position.width = Mathf.Max((float) 50f, (float) ((position.width - 4f) / 3f));
            EditorGUIUtility.labelWidth = 50f;
            this.ToggleLeft(position, this.m_ChildControlWidth, new GUIContent("Width"));
            position.x += position.width + 2f;
            this.ToggleLeft(position, this.m_ChildControlHeight, new GUIContent("Height"));
            EditorGUIUtility.labelWidth = 0f;
            position = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), -1, new GUIContent("Child Force Expand"));
            position.width = Mathf.Max((float) 50f, (float) ((position.width - 4f) / 3f));
            EditorGUIUtility.labelWidth = 50f;
            this.ToggleLeft(position, this.m_ChildForceExpandWidth, new GUIContent("Width"));
            position.x += position.width + 2f;
            this.ToggleLeft(position, this.m_ChildForceExpandHeight, new GUIContent("Height"));
            EditorGUIUtility.labelWidth = 0f;
            base.serializedObject.ApplyModifiedProperties();
        }

        private void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            bool boolValue = property.boolValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            boolValue = EditorGUI.ToggleLeft(position, label, boolValue);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = !property.hasMultipleDifferentValues ? !property.boolValue : true;
            }
            EditorGUI.showMixedValue = false;
        }
    }
}

