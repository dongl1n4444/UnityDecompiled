namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CustomEditor(typeof(MeshRenderer)), CanEditMultipleObjects]
    internal class MeshRendererEditor : RendererEditorBase
    {
        [CompilerGenerated]
        private static Func<Object, GameObject> <>f__am$cache0;
        private SerializedProperty m_CastShadows;
        private SerializedObject m_GameObjectsSerializedObject;
        private SerializedProperty m_GameObjectStaticFlags;
        private LightingSettingsInspector m_Lighting;
        private SerializedProperty m_Materials;
        private SerializedProperty m_MotionVectors;
        private SerializedProperty m_ReceiveShadows;

        private void InitializeLightingFields()
        {
            this.m_Lighting = new LightingSettingsInspector();
            this.m_Lighting.ShowSettings = EditorPrefs.GetBool(Styles.DisplayLightingKey, false);
            this.m_Lighting.ShowChartingSettings = SessionState.GetBool(Styles.DisplayChartingKey, true);
            this.m_Lighting.ShowLightmapSettings = SessionState.GetBool(Styles.DisplayLightmapKey, true);
        }

        private void LightingFieldsGUI()
        {
            bool showSettings = this.m_Lighting.ShowSettings;
            bool showChartingSettings = this.m_Lighting.ShowChartingSettings;
            bool showLightmapSettings = this.m_Lighting.ShowLightmapSettings;
            if (this.m_Lighting.Begin())
            {
                base.RenderProbeFields();
                EditorGUILayout.PropertyField(this.m_CastShadows, Styles.CastShadows, true, new GUILayoutOption[0]);
                bool disabled = SceneView.IsUsingDeferredRenderingPath();
                using (new EditorGUI.DisabledScope(disabled))
                {
                    EditorGUILayout.PropertyField(this.m_ReceiveShadows, Styles.ReceiveShadows, true, new GUILayoutOption[0]);
                }
                EditorGUILayout.PropertyField(this.m_MotionVectors, true, new GUILayoutOption[0]);
                this.m_Lighting.RenderMeshSettings(base.serializedObject);
            }
            this.m_Lighting.End();
            if (this.m_Lighting.ShowSettings != showSettings)
            {
                EditorPrefs.SetBool(Styles.DisplayLightingKey, this.m_Lighting.ShowSettings);
            }
            if (this.m_Lighting.ShowChartingSettings != showChartingSettings)
            {
                SessionState.SetBool(Styles.DisplayChartingKey, this.m_Lighting.ShowChartingSettings);
            }
            if (this.m_Lighting.ShowLightmapSettings != showLightmapSettings)
            {
                SessionState.SetBool(Styles.DisplayLightmapKey, this.m_Lighting.ShowLightmapSettings);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_CastShadows = base.serializedObject.FindProperty("m_CastShadows");
            this.m_ReceiveShadows = base.serializedObject.FindProperty("m_ReceiveShadows");
            this.m_MotionVectors = base.serializedObject.FindProperty("m_MotionVectors");
            this.m_Materials = base.serializedObject.FindProperty("m_Materials");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = t => ((MeshRenderer) t).gameObject;
            }
            this.m_GameObjectsSerializedObject = new SerializedObject(Enumerable.Select<Object, GameObject>(base.targets, <>f__am$cache0).ToArray<GameObject>());
            this.m_GameObjectStaticFlags = this.m_GameObjectsSerializedObject.FindProperty("m_StaticEditorFlags");
            base.InitializeProbeFields();
            this.InitializeLightingFields();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.LightingFieldsGUI();
            bool flag = false;
            SerializedProperty materialsArray = base.serializedObject.FindProperty("m_Materials");
            if (!materialsArray.hasMultipleDifferentValues)
            {
                MeshFilter component = ((MeshRenderer) base.serializedObject.targetObject).GetComponent<MeshFilter>();
                flag = ((component != null) && (component.sharedMesh != null)) && (materialsArray.arraySize > component.sharedMesh.subMeshCount);
            }
            EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
            if (!this.m_Materials.hasMultipleDifferentValues && flag)
            {
                EditorGUILayout.HelpBox("This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.", MessageType.Warning, true);
            }
            if (ShaderUtil.MaterialsUseInstancingShader(materialsArray))
            {
                this.m_GameObjectsSerializedObject.Update();
                if (!this.m_GameObjectStaticFlags.hasMultipleDifferentValues && ((this.m_GameObjectStaticFlags.intValue & 4) != 0))
                {
                    EditorGUILayout.HelpBox("This renderer is statically batched and uses an instanced shader at the same time. Instancing will be disabled in such a case. Consider disabling static batching if you want it to be instanced.", MessageType.Warning, true);
                }
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private static class Styles
        {
            public static readonly GUIContent CastShadows = EditorGUIUtility.TextContent("Cast Shadows");
            public static readonly string DisplayChartingKey = "MeshRendererEditor.Lighting.ShowChartingSettings";
            public static readonly string DisplayLightingKey = "MeshRendererEditor.Lighting.ShowSettings";
            public static readonly string DisplayLightmapKey = "MeshRendererEditor.Lighting.ShowLightmapSettings";
            public static readonly GUIContent ReceiveShadows = EditorGUIUtility.TextContent("Receive Shadows");
        }
    }
}

