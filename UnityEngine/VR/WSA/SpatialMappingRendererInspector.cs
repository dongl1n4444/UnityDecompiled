namespace UnityEngine.VR.WSA
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(SpatialMappingRenderer))]
    public class SpatialMappingRendererInspector : SpatialMappingBaseInspector
    {
        private SerializedProperty m_OcculsionMaterialProp = null;
        private SerializedProperty m_RenderStateProp = null;
        private SpatialMappingRenderer m_SMRenderer = null;
        private SerializedProperty m_VisualMaterialProp = null;
        private static readonly GUIContent s_CustomMaterialLabelContent = new GUIContent("Visual Material", "The visual material is intended to be used for the purpose of visualizing the surfaces.");
        private static readonly string s_NoMaterialInUseMsg = "No material is in use.  Nothing will be rendered.";
        private static readonly string s_OcclusionMaterialInUseMsg = "The occlusion render state will use the occlusion material.";
        private static readonly GUIContent s_OcclusionMaterialLabelContent = new GUIContent("Occlusion Material", "The occlusion material is intended to occlude holograms that should be hidden from the user.");
        private static readonly GUIContent s_RenderSettingsLabelContent = new GUIContent("Render Settings");
        private static readonly GUIContent s_RenderStateLabelContent = new GUIContent("Render State", "This field specifies the material that should be applied to all surfaces.");
        private static readonly string s_VariableOcclusionMaterial = "m_OcclusionMaterial";
        private static readonly string s_VariableVisualMaterial = "m_VisualMaterial";
        private static readonly string s_VisualizationMaterialInUseMsg = "The visualization render state will use the visualization material.";

        private void CacheSerializedProperties()
        {
            this.m_RenderStateProp = base.serializedObject.FindProperty("m_CurrentRenderState");
            this.m_OcculsionMaterialProp = base.serializedObject.FindProperty(s_VariableOcclusionMaterial);
            this.m_VisualMaterialProp = base.serializedObject.FindProperty(s_VariableVisualMaterial);
        }

        private void ManageRenderSettings()
        {
            EditorGUILayout.LabelField(s_RenderSettingsLabelContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_RenderStateProp, s_RenderStateLabelContent, new GUILayoutOption[0]);
            if (this.m_SMRenderer.renderState == SpatialMappingRenderer.RenderState.Occlusion)
            {
                EditorGUILayout.HelpBox(s_OcclusionMaterialInUseMsg, MessageType.Info);
            }
            else if (this.m_SMRenderer.renderState == SpatialMappingRenderer.RenderState.Visualization)
            {
                EditorGUILayout.HelpBox(s_VisualizationMaterialInUseMsg, MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(s_NoMaterialInUseMsg, MessageType.Info);
            }
            EditorGUILayout.PropertyField(this.m_OcculsionMaterialProp, s_OcclusionMaterialLabelContent, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_VisualMaterialProp, s_CustomMaterialLabelContent, new GUILayoutOption[0]);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_SMRenderer = base.target as SpatialMappingRenderer;
            this.CacheSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.ManageRenderSettings();
            EditorGUILayout.Separator();
            base.OnInspectorGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

