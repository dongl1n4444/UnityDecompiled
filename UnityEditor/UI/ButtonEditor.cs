namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the Button Component.</para>
    /// </summary>
    [CustomEditor(typeof(Button), true), CanEditMultipleObjects]
    public class ButtonEditor : SelectableEditor
    {
        private SerializedProperty m_OnClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_OnClickProperty = base.serializedObject.FindProperty("m_OnClick");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_OnClickProperty, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

