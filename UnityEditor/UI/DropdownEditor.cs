namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom editor for the Dropdown component.</para>
    /// </summary>
    [CustomEditor(typeof(Dropdown), true), CanEditMultipleObjects]
    public class DropdownEditor : SelectableEditor
    {
        private SerializedProperty m_CaptionImage;
        private SerializedProperty m_CaptionText;
        private SerializedProperty m_ItemImage;
        private SerializedProperty m_ItemText;
        private SerializedProperty m_OnSelectionChanged;
        private SerializedProperty m_Options;
        private SerializedProperty m_Template;
        private SerializedProperty m_Value;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Template = base.serializedObject.FindProperty("m_Template");
            this.m_CaptionText = base.serializedObject.FindProperty("m_CaptionText");
            this.m_CaptionImage = base.serializedObject.FindProperty("m_CaptionImage");
            this.m_ItemText = base.serializedObject.FindProperty("m_ItemText");
            this.m_ItemImage = base.serializedObject.FindProperty("m_ItemImage");
            this.m_OnSelectionChanged = base.serializedObject.FindProperty("m_OnValueChanged");
            this.m_Value = base.serializedObject.FindProperty("m_Value");
            this.m_Options = base.serializedObject.FindProperty("m_Options");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Template, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_CaptionText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_CaptionImage, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ItemText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ItemImage, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Value, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Options, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_OnSelectionChanged, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

