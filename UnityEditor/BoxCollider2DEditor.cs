namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(BoxCollider2D)), CanEditMultipleObjects]
    internal class BoxCollider2DEditor : Collider2DEditorBase
    {
        private readonly BoxEditor m_BoxEditor = new BoxEditor(true, s_BoxHash, true);
        private SerializedProperty m_Size;
        private static readonly int s_BoxHash = "BoxCollider2DEditor".GetHashCode();

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_BoxEditor.OnDisable();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_BoxEditor.OnEnable();
            this.m_BoxEditor.SetAlwaysDisplayHandles(true);
            this.m_Size = base.serializedObject.FindProperty("m_Size");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                BoxCollider2D target = (BoxCollider2D) base.target;
                Vector3 offset = (Vector3) target.offset;
                Vector3 size = (Vector3) target.size;
                if (this.m_BoxEditor.OnSceneGUI(target.transform, Handles.s_ColliderHandleColor, ref offset, ref size))
                {
                    Undo.RecordObject(target, "Modify collider");
                    target.offset = offset;
                    target.size = size;
                }
            }
        }
    }
}

