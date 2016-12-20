namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CustomEditor(typeof(TrailRenderer)), CanEditMultipleObjects]
    internal class TrailRendererInspector : RendererEditorBase
    {
        private SerializedProperty m_Alignment;
        private SerializedProperty m_ColorGradient;
        private LineRendererCurveEditor m_CurveEditor = new LineRendererCurveEditor();
        private string[] m_ExcludedProperties;
        private SerializedProperty m_NumCapVertices;
        private SerializedProperty m_NumCornerVertices;
        private SerializedProperty m_TextureMode;

        public void OnDisable()
        {
            this.m_CurveEditor.OnDisable();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_ExcludedProperties = new List<string> { 
                "m_LightProbeUsage",
                "m_LightProbeVolumeOverride",
                "m_ReflectionProbeUsage",
                "m_ProbeAnchor",
                "m_Parameters"
            }.ToArray();
            this.m_CurveEditor.OnEnable(base.serializedObject);
            this.m_ColorGradient = base.serializedObject.FindProperty("m_Parameters.colorGradient");
            this.m_NumCornerVertices = base.serializedObject.FindProperty("m_Parameters.numCornerVertices");
            this.m_NumCapVertices = base.serializedObject.FindProperty("m_Parameters.numCapVertices");
            this.m_Alignment = base.serializedObject.FindProperty("m_Parameters.alignment");
            this.m_TextureMode = base.serializedObject.FindProperty("m_Parameters.textureMode");
            base.InitializeProbeFields();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Editor.DrawPropertiesExcluding(base.m_SerializedObject, this.m_ExcludedProperties);
            this.m_CurveEditor.CheckCurveChangedExternally();
            this.m_CurveEditor.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_ColorGradient, Styles.colorGradient, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_NumCornerVertices, Styles.numCornerVertices, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_NumCapVertices, Styles.numCapVertices, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Alignment, Styles.alignment, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_TextureMode, Styles.textureMode, new GUILayoutOption[0]);
            base.m_Probes.OnGUI(base.targets, (Renderer) base.target, false);
            base.serializedObject.ApplyModifiedProperties();
        }

        private class Styles
        {
            public static GUIContent alignment = EditorGUIUtility.TextContent("Alignment|Trails can rotate to face their transform component or the camera.");
            public static GUIContent colorGradient = EditorGUIUtility.TextContent("Color|The gradient describing the color along the trail.");
            public static GUIContent numCapVertices = EditorGUIUtility.TextContent("End Cap Vertices|How many vertices to add at each end.");
            public static GUIContent numCornerVertices = EditorGUIUtility.TextContent("Corner Vertices|How many vertices to add for each corner.");
            public static GUIContent textureMode = EditorGUIUtility.TextContent("Texture Mode|Should the U coordinate be stretched or tiled?");
        }
    }
}

