namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(CapsuleCollider2D)), CanEditMultipleObjects]
    internal class CapsuleCollider2DEditor : Collider2DEditorBase
    {
        private static readonly int k_CapsuleHash = "CapsuleCollider2DEditor".GetHashCode();
        private readonly BoxEditor m_BoxEditor = new BoxEditor(true, k_CapsuleHash, true);
        private SerializedProperty m_Direction;
        private SerializedProperty m_Size;

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
            this.m_Direction = base.serializedObject.FindProperty("m_Direction");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                CapsuleCollider2D target = (CapsuleCollider2D) base.target;
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

