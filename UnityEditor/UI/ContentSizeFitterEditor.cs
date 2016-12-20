namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the ContentSizeFitter Component.
    /// </para>
    /// </summary>
    [CustomEditor(typeof(ContentSizeFitter), true), CanEditMultipleObjects]
    public class ContentSizeFitterEditor : SelfControllerEditor
    {
        private SerializedProperty m_HorizontalFit;
        private SerializedProperty m_VerticalFit;

        protected virtual void OnEnable()
        {
            this.m_HorizontalFit = base.serializedObject.FindProperty("m_HorizontalFit");
            this.m_VerticalFit = base.serializedObject.FindProperty("m_VerticalFit");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_HorizontalFit, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_VerticalFit, true, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}

