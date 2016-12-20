namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the AspectRatioFitter component.</para>
    /// </summary>
    [CustomEditor(typeof(AspectRatioFitter), true), CanEditMultipleObjects]
    public class AspectRatioFitterEditor : SelfControllerEditor
    {
        private SerializedProperty m_AspectMode;
        private SerializedProperty m_AspectRatio;

        protected virtual void OnEnable()
        {
            this.m_AspectMode = base.serializedObject.FindProperty("m_AspectMode");
            this.m_AspectRatio = base.serializedObject.FindProperty("m_AspectRatio");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_AspectMode, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AspectRatio, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}

