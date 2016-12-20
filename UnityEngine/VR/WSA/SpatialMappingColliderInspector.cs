namespace UnityEngine.VR.WSA
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(SpatialMappingCollider))]
    public class SpatialMappingColliderInspector : SpatialMappingBaseInspector
    {
        private SerializedProperty m_EnableCollisionsProp = null;
        private SerializedProperty m_LayerProp = null;
        private SerializedProperty m_PhysicMaterialProp = null;
        private static readonly GUIContent s_ColliderSettingsLabelContent = new GUIContent("Collider Settings");
        private static readonly GUIContent s_EnableCollisionsLabelContent = new GUIContent("Enable Collisions");
        private static readonly GUIContent s_MeshLayerLabelContent = new GUIContent("Mesh Layer");
        private static readonly GUIContent s_PhysicMaterialLabelContent = new GUIContent("Physic Material");

        private void CacheSerializedProperties()
        {
            this.m_EnableCollisionsProp = base.serializedObject.FindProperty("m_EnableCollisions");
            this.m_LayerProp = base.serializedObject.FindProperty("m_Layer");
            this.m_PhysicMaterialProp = base.serializedObject.FindProperty("m_Material");
        }

        private void ManageColliderSettings()
        {
            EditorGUILayout.LabelField(s_ColliderSettingsLabelContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_EnableCollisionsProp, s_EnableCollisionsLabelContent, new GUILayoutOption[0]);
            Rect controlRect = EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]);
            EditorGUI.BeginProperty(controlRect, s_MeshLayerLabelContent, this.m_LayerProp);
            this.m_LayerProp.intValue = EditorGUI.LayerField(controlRect, s_MeshLayerLabelContent, this.m_LayerProp.intValue);
            EditorGUI.EndProperty();
            EditorGUILayout.PropertyField(this.m_PhysicMaterialProp, s_PhysicMaterialLabelContent, new GUILayoutOption[0]);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.CacheSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.ManageColliderSettings();
            EditorGUILayout.Separator();
            base.OnInspectorGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

