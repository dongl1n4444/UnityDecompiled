namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(EdgeCollider2D))]
    internal class EdgeCollider2DEditor : Collider2DEditorBase
    {
        private SerializedProperty m_Points;
        private PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();
        private bool m_ShowColliderInfo;

        protected override void OnEditEnd()
        {
            this.m_PolyUtility.StopEditing();
        }

        protected override void OnEditStart()
        {
            this.m_PolyUtility.StartEditing(base.target as Collider2D);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Points = base.serializedObject.FindProperty("m_Points");
            this.m_Points.isExpanded = false;
        }

        public override void OnInspectorGUI()
        {
            base.BeginColliderInspector();
            base.OnInspectorGUI();
            if (base.targets.Length == 1)
            {
                EditorGUI.BeginDisabledGroup(base.editingCollider);
                EditorGUILayout.PropertyField(this.m_Points, true, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
            }
            base.EndColliderInspector();
            base.FinalizeInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                this.m_PolyUtility.OnSceneGUI();
            }
        }
    }
}

