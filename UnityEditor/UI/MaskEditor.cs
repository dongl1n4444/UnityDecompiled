namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the Mask component.</para>
    /// </summary>
    [CustomEditor(typeof(UnityEngine.UI.Mask), true), CanEditMultipleObjects]
    public class MaskEditor : Editor
    {
        private SerializedProperty m_ShowMaskGraphic;

        protected virtual void OnEnable()
        {
            this.m_ShowMaskGraphic = base.serializedObject.FindProperty("m_ShowMaskGraphic");
        }

        public override void OnInspectorGUI()
        {
            Graphic component = (base.target as UnityEngine.UI.Mask).GetComponent<Graphic>();
            if ((component != null) && !component.IsActive())
            {
                EditorGUILayout.HelpBox("Masking disabled due to Graphic component being disabled.", MessageType.Warning);
            }
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_ShowMaskGraphic, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

