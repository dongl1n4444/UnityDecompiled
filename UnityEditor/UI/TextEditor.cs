namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the Text Component.</para>
    /// </summary>
    [CustomEditor(typeof(Text), true), CanEditMultipleObjects]
    public class TextEditor : GraphicEditor
    {
        private SerializedProperty m_FontData;
        private SerializedProperty m_Text;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Text = base.serializedObject.FindProperty("m_Text");
            this.m_FontData = base.serializedObject.FindProperty("m_FontData");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Text, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_FontData, new GUILayoutOption[0]);
            base.AppearanceControlsGUI();
            base.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

