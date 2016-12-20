namespace UnityEngine.VR.WSA
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class SpatialMappingBaseInspector : Editor
    {
        private SerializedProperty m_FreezeUpdatesProp = null;
        private SerializedProperty m_HalfBoxExtentsProp = null;
        private SerializedProperty m_LodProp = null;
        private SerializedProperty m_NumUpdatesBeforeRemovalProp = null;
        private SerializedProperty m_SecondsBetweenUpdatesProp = null;
        private SpatialMappingBase m_SMBase = null;
        private SerializedProperty m_SphereRadiusProp = null;
        private SerializedProperty m_SurfaceParentProp = null;
        private SerializedProperty m_VolumeProp = null;
        private static readonly GUIContent s_BoundingVolumeLabelContent = new GUIContent("Bounding Volume Type", "The shape of the bounds for the observed region.");
        private static readonly GUIContent s_BoxExtentsInMetersLabelContent = new GUIContent("Half Extents In Meters", "The extents of the observation volume in meters.");
        private static readonly GUIContent s_FreezeUpdatesLabelContent = new GUIContent("Freeze Updates");
        private static readonly GUIContent s_GeneralSettingsLabelContent = new GUIContent("General Settings");
        private static readonly GUIContent s_LODLabelContent = new GUIContent("Level Of Detail", "The quality of the resulting mesh. Lower is better for performance and physics while higher will look more accurate and is better for rendering.");
        private static readonly GUIContent s_RadiusInMetersLabelContent = new GUIContent("Radius In Meters", "The radius of the observation sphere volume in meters.");
        private static readonly GUIContent s_RemovalUpdateCountLabelContent = new GUIContent("Removal Update Count", "Total number of updates before a surface is removed when it is no longer in the surface observer's bounding volume.");
        private static readonly GUIContent s_RemoveSurfacesLabelContent = new GUIContent("Remove Surfaces Immediately");
        private static readonly GUIContent s_SurfaceParentLabelContent = new GUIContent("Surface Parent", "All surface mesh GameObjects will be children of the surface parent.  If no surface parent has been assigned, a surface parent will be generated.");
        private static readonly GUIContent s_TimeBetweenUpdatesLabelContent = new GUIContent("Time Between Updates", "Time, in seconds, to wait between spatial mapping updates.");

        protected virtual void OnEnable()
        {
            this.m_SMBase = base.target as SpatialMappingBase;
            this.m_SurfaceParentProp = base.serializedObject.FindProperty("m_SurfaceParent");
            this.m_FreezeUpdatesProp = base.serializedObject.FindProperty("m_FreezeUpdates");
            this.m_SecondsBetweenUpdatesProp = base.serializedObject.FindProperty("m_SecondsBetweenUpdates");
            this.m_NumUpdatesBeforeRemovalProp = base.serializedObject.FindProperty("m_NumUpdatesBeforeRemoval");
            this.m_LodProp = base.serializedObject.FindProperty("m_LodType");
            this.m_VolumeProp = base.serializedObject.FindProperty("m_VolumeType");
            this.m_SphereRadiusProp = base.serializedObject.FindProperty("m_SphereRadius");
            this.m_HalfBoxExtentsProp = base.serializedObject.FindProperty("m_HalfBoxExtents");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(s_GeneralSettingsLabelContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SurfaceParentProp, s_SurfaceParentLabelContent, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_FreezeUpdatesProp, s_FreezeUpdatesLabelContent, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SecondsBetweenUpdatesProp, s_TimeBetweenUpdatesLabelContent, new GUILayoutOption[0]);
            if (this.m_NumUpdatesBeforeRemovalProp.intValue <= 0)
            {
                Rect controlRect = EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]);
                EditorGUI.BeginProperty(controlRect, s_RemoveSurfacesLabelContent, this.m_NumUpdatesBeforeRemovalProp);
                if (EditorGUI.Toggle(controlRect, s_RemoveSurfacesLabelContent, this.m_SMBase.numUpdatesBeforeRemoval <= 0))
                {
                    this.m_NumUpdatesBeforeRemovalProp.intValue = 0;
                }
                else
                {
                    this.m_NumUpdatesBeforeRemovalProp.intValue = 1;
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_NumUpdatesBeforeRemovalProp, s_RemovalUpdateCountLabelContent, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_LodProp, s_LODLabelContent, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_VolumeProp, s_BoundingVolumeLabelContent, new GUILayoutOption[0]);
            if (this.m_VolumeProp.enumValueIndex == 0)
            {
                Rect totalPosition = EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]);
                EditorGUI.BeginProperty(totalPosition, s_RadiusInMetersLabelContent, this.m_SphereRadiusProp);
                float floatValue = this.m_SphereRadiusProp.floatValue;
                this.m_SphereRadiusProp.floatValue = EditorGUI.FloatField(totalPosition, s_RadiusInMetersLabelContent, this.m_SphereRadiusProp.floatValue);
                if (this.m_SphereRadiusProp.floatValue < Mathf.Epsilon)
                {
                    this.m_SphereRadiusProp.floatValue = floatValue;
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_HalfBoxExtentsProp, s_BoxExtentsInMetersLabelContent, new GUILayoutOption[0]);
            }
        }
    }
}

