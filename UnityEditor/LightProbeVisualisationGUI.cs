namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LightProbeVisualisationGUI
    {
        private GUIContent LightProbeVisualisationTitle = EditorGUIUtility.TextContent("Light Probe Visualization");
        private bool m_ShowProbeDebugModeOptions = false;

        public void OnGUI()
        {
            this.m_ShowProbeDebugModeOptions = EditorGUILayout.FoldoutTitlebar(this.m_ShowProbeDebugModeOptions, this.LightProbeVisualisationTitle, true);
            if (this.m_ShowProbeDebugModeOptions)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                LightProbeVisualization.lightProbeVisualizationMode = (LightProbeVisualization.LightProbeVisualizationMode) EditorGUILayout.EnumPopup(LightProbeVisualization.lightProbeVisualizationMode, new GUILayoutOption[0]);
                EditorGUILayout.LabelField("Display:", new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                LightProbeVisualization.showInterpolationWeights = EditorGUILayout.Toggle("Weights", LightProbeVisualization.showInterpolationWeights, new GUILayoutOption[0]);
                LightProbeVisualization.showOcclusions = EditorGUILayout.Toggle("Occlusions", LightProbeVisualization.showOcclusions, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorApplication.SetSceneRepaintDirty();
                }
                EditorGUILayout.Space();
            }
        }
    }
}

