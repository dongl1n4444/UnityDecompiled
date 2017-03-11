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
        private static Func<UnityEngine.Object, GameObject> <>f__am$cache0;
        private const string kDisplayChartingKey = "MeshRendererEditor.Lighting.ShowChartingSettings";
        private const string kDisplayLightingKey = "MeshRendererEditor.Lighting.ShowSettings";
        private const string kDisplayLightmapKey = "MeshRendererEditor.Lighting.ShowLightmapSettings";
        private SerializedObject m_GameObjectsSerializedObject;
        private SerializedProperty m_GameObjectStaticFlags;
        private LightingSettingsInspector m_Lighting;
        private SerializedProperty m_Materials;

        private void InitializeLightingFields()
        {
            this.m_Lighting = new LightingSettingsInspector(base.serializedObject);
            this.m_Lighting.showSettings = EditorPrefs.GetBool("MeshRendererEditor.Lighting.ShowSettings", false);
            this.m_Lighting.showChartingSettings = SessionState.GetBool("MeshRendererEditor.Lighting.ShowChartingSettings", true);
            this.m_Lighting.showLightmapSettings = SessionState.GetBool("MeshRendererEditor.Lighting.ShowLightmapSettings", true);
        }

        private void LightingFieldsGUI()
        {
            bool showSettings = this.m_Lighting.showSettings;
            bool showChartingSettings = this.m_Lighting.showChartingSettings;
            bool showLightmapSettings = this.m_Lighting.showLightmapSettings;
            if (this.m_Lighting.Begin())
            {
                base.RenderProbeFields();
                this.m_Lighting.RenderMeshSettings(true);
            }
            this.m_Lighting.End();
            if (this.m_Lighting.showSettings != showSettings)
            {
                EditorPrefs.SetBool("MeshRendererEditor.Lighting.ShowSettings", this.m_Lighting.showSettings);
            }
            if (this.m_Lighting.showChartingSettings != showChartingSettings)
            {
                SessionState.SetBool("MeshRendererEditor.Lighting.ShowChartingSettings", this.m_Lighting.showChartingSettings);
            }
            if (this.m_Lighting.showLightmapSettings != showLightmapSettings)
            {
                SessionState.SetBool("MeshRendererEditor.Lighting.ShowLightmapSettings", this.m_Lighting.showLightmapSettings);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Materials = base.serializedObject.FindProperty("m_Materials");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = t => ((MeshRenderer) t).gameObject;
            }
            this.m_GameObjectsSerializedObject = new SerializedObject(Enumerable.Select<UnityEngine.Object, GameObject>(base.targets, <>f__am$cache0).ToArray<GameObject>());
            this.m_GameObjectStaticFlags = this.m_GameObjectsSerializedObject.FindProperty("m_StaticEditorFlags");
            base.InitializeProbeFields();
            this.InitializeLightingFields();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.LightingFieldsGUI();
            bool flag = false;
            if (!this.m_Materials.hasMultipleDifferentValues)
            {
                MeshFilter component = ((MeshRenderer) base.serializedObject.targetObject).GetComponent<MeshFilter>();
                flag = ((component != null) && (component.sharedMesh != null)) && (this.m_Materials.arraySize > component.sharedMesh.subMeshCount);
            }
            EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
            if (!this.m_Materials.hasMultipleDifferentValues && flag)
            {
                EditorGUILayout.HelpBox(Styles.MaterialWarning, MessageType.Warning, true);
            }
            if (ShaderUtil.MaterialsUseInstancingShader(this.m_Materials))
            {
                this.m_GameObjectsSerializedObject.Update();
                if (!this.m_GameObjectStaticFlags.hasMultipleDifferentValues && ((this.m_GameObjectStaticFlags.intValue & 4) != 0))
                {
                    EditorGUILayout.HelpBox(Styles.StaticBatchingWarning, MessageType.Warning, true);
                }
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private class Styles
        {
            public static readonly string MaterialWarning = "This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.";
            public static readonly string StaticBatchingWarning = "This renderer is statically batched and uses an instanced shader at the same time. Instancing will be disabled in such a case. Consider disabling static batching if you want it to be instanced.";
        }
    }
}

