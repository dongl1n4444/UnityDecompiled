namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;

    internal class LightingSettingsInspector
    {
        private int m_PreviousSelection;
        private GUIContent m_SelectedObjectPreviewTexture;
        private bool m_ShowBakedLM = false;
        private bool m_ShowChartingSettings = true;
        private AnimBool m_ShowClampedSize = new AnimBool();
        private bool m_ShowLightmapSettings = true;
        private bool m_ShowRealtimeLM = false;
        private bool m_ShowSettings = false;
        private ZoomableArea m_ZoomablePreview;
        private static Styles s_Styles;

        public bool Begin()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_ShowSettings = EditorGUILayout.Foldout(this.m_ShowSettings, s_Styles.Lighting);
            if (!this.m_ShowSettings)
            {
                return false;
            }
            EditorGUI.indentLevel++;
            return true;
        }

        public void End()
        {
            if (this.m_ShowSettings)
            {
                EditorGUI.indentLevel--;
            }
        }

        private static bool HasNormals(Renderer renderer)
        {
            Mesh sharedMesh = null;
            if (renderer is MeshRenderer)
            {
                MeshFilter component = renderer.GetComponent<MeshFilter>();
                if (component != null)
                {
                    sharedMesh = component.sharedMesh;
                }
            }
            else if (renderer is SkinnedMeshRenderer)
            {
                sharedMesh = (renderer as SkinnedMeshRenderer).sharedMesh;
            }
            return ((sharedMesh != null) && InternalMeshUtil.HasNormals(sharedMesh));
        }

        private static bool isBuiltIn(SerializedProperty prop)
        {
            if (prop.objectReferenceValue != null)
            {
                LightmapParameters objectReferenceValue = prop.objectReferenceValue as LightmapParameters;
                return (objectReferenceValue.hideFlags == HideFlags.NotEditable);
            }
            return false;
        }

        public static bool LightmapParametersGUI(SerializedProperty prop, GUIContent content)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Default-Medium");
            string text = "Edit...";
            if (isBuiltIn(prop))
            {
                text = "View";
            }
            bool flag = false;
            if (prop.objectReferenceValue == null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(text, EditorStyles.miniButton, options))
                    {
                        Selection.activeObject = null;
                        flag = true;
                    }
                }
            }
            else
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button(text, EditorStyles.miniButton, optionArray2))
                {
                    Selection.activeObject = prop.objectReferenceValue;
                    flag = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            return flag;
        }

        private float LightmapScaleGUI(SerializedObject so, float lodScale)
        {
            SerializedProperty property = so.FindProperty("m_ScaleInLightmap");
            float num = lodScale * property.floatValue;
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            EditorGUI.BeginProperty(controlRect, s_Styles.ScaleInLightmap, property);
            EditorGUI.BeginChangeCheck();
            num = EditorGUI.FloatField(controlRect, s_Styles.ScaleInLightmap, num);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Mathf.Max((float) (num / lodScale), (float) 0f);
            }
            EditorGUI.EndProperty();
            return num;
        }

        private void LightmapStaticSettings(SerializedObject so)
        {
            bool flag = (so.FindProperty("m_StaticEditorFlags").intValue & 1) != 0;
            bool flagValue = EditorGUILayout.Toggle(s_Styles.LightmapStatic, flag, new GUILayoutOption[0]);
            if (flag != flagValue)
            {
                SceneModeUtility.SetStaticFlags(so.targetObjects, 1, flagValue);
                so.ApplyModifiedProperties();
            }
        }

        private void RendererUVSettings(SerializedObject so)
        {
            EditorGUI.indentLevel++;
            SerializedProperty property = so.FindProperty("m_PreserveUVs");
            bool flag = !property.boolValue;
            bool flag2 = EditorGUILayout.Toggle(s_Styles.OptimizeRealtimeUVs, flag, new GUILayoutOption[0]);
            if (flag2 != flag)
            {
                property.boolValue = !flag2;
            }
            EditorGUI.indentLevel++;
            bool boolValue = property.boolValue;
            using (new EditorGUI.DisabledScope(boolValue))
            {
                SerializedProperty property2 = so.FindProperty("m_AutoUVMaxDistance");
                EditorGUILayout.PropertyField(property2, s_Styles.AutoUVMaxDistance, new GUILayoutOption[0]);
                if (property2.floatValue < 0f)
                {
                    property2.floatValue = 0f;
                }
                EditorGUILayout.Slider(so.FindProperty("m_AutoUVMaxAngle"), 0f, 180f, s_Styles.AutoUVMaxAngle, new GUILayoutOption[0]);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(so.FindProperty("m_IgnoreNormalsForChartDetection"), s_Styles.IgnoreNormalsForChartDetection, new GUILayoutOption[0]);
            EditorGUILayout.IntPopup(so.FindProperty("m_MinimumChartSize"), s_Styles.MinimumChartSizeStrings, s_Styles.MinimumChartSizeValues, s_Styles.MinimumChartSize, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
        }

        public void RenderMeshSettings(SerializedObject so)
        {
            GameObject[] objArray;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Type[] types = new Type[] { typeof(MeshRenderer), typeof(SkinnedMeshRenderer) };
            Renderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Renderer>(out objArray, types);
            if (objArray.Length != 0)
            {
                if (!LightModeUtil.Get().IsAnyGIEnabled())
                {
                    EditorGUILayout.HelpBox(s_Styles.GINotEnabledInfo.text, MessageType.Info);
                }
                else
                {
                    SerializedObject obj2 = new SerializedObject(objArray);
                    this.LightmapStaticSettings(obj2);
                    if ((obj2.FindProperty("m_StaticEditorFlags").intValue & 1) != 0)
                    {
                        this.m_ShowChartingSettings = EditorGUILayout.Foldout(this.m_ShowChartingSettings, s_Styles.UVCharting);
                        if (this.m_ShowChartingSettings)
                        {
                            this.RendererUVSettings(so);
                        }
                        this.m_ShowLightmapSettings = EditorGUILayout.Foldout(this.m_ShowLightmapSettings, s_Styles.LightmapSettings);
                        if (this.m_ShowLightmapSettings)
                        {
                            EditorGUI.indentLevel++;
                            float lightmapLODLevelScale = LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[0]);
                            for (int i = 1; i < selectedObjectsOfType.Length; i++)
                            {
                                if (!Mathf.Approximately(lightmapLODLevelScale, LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[i])))
                                {
                                    lightmapLODLevelScale = 1f;
                                }
                            }
                            float lightmapScale = this.LightmapScaleGUI(so, lightmapLODLevelScale) * LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[0]);
                            float cachedMeshSurfaceArea = InternalMeshUtil.GetCachedMeshSurfaceArea((MeshRenderer) selectedObjectsOfType[0]);
                            this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedMeshSurfaceArea);
                            EditorGUILayout.PropertyField(so.FindProperty("m_ImportantGI"), s_Styles.ImportantGI, new GUILayoutOption[0]);
                            LightmapParametersGUI(so.FindProperty("m_LightmapParameters"), s_Styles.LightmapParameters);
                            this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, s_Styles.Atlas);
                            if (this.m_ShowBakedLM)
                            {
                                this.ShowAtlasGUI(so);
                            }
                            this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, s_Styles.RealtimeLM);
                            if (this.m_ShowRealtimeLM)
                            {
                                this.ShowRealtimeLMGUI(so, selectedObjectsOfType[0]);
                            }
                            EditorGUI.indentLevel--;
                        }
                        if (LightmapEditorSettings.HasZeroAreaMesh(selectedObjectsOfType[0]))
                        {
                            EditorGUILayout.HelpBox(s_Styles.ZeroAreaPackingMesh.text, MessageType.Warning);
                        }
                        if (LightmapEditorSettings.HasClampedResolution(selectedObjectsOfType[0]))
                        {
                            EditorGUILayout.HelpBox(s_Styles.ClampedPackingResolution.text, MessageType.Warning);
                        }
                        if (!HasNormals(selectedObjectsOfType[0]))
                        {
                            EditorGUILayout.HelpBox(s_Styles.NoNormalsNoLightmapping.text, MessageType.Warning);
                        }
                        obj2.ApplyModifiedProperties();
                        so.ApplyModifiedProperties();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(s_Styles.LightmapInfoBox.text, MessageType.Info);
                    }
                }
            }
        }

        public void RenderTerrainSettings(SerializedObject so)
        {
            GameObject[] objArray;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Terrain[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out objArray, new Type[0]);
            if (objArray.Length != 0)
            {
                SerializedObject obj2 = new SerializedObject(objArray);
                this.LightmapStaticSettings(obj2);
                bool flag = (obj2.FindProperty("m_StaticEditorFlags").intValue & 1) != 0;
                if (flag)
                {
                    this.m_ShowLightmapSettings = EditorGUILayout.Foldout(this.m_ShowLightmapSettings, s_Styles.LightmapSettings);
                    if (this.m_ShowLightmapSettings)
                    {
                        EditorGUI.indentLevel++;
                        using (new EditorGUI.DisabledScope(!flag))
                        {
                            if (GUI.enabled)
                            {
                                this.ShowTerrainChunks(selectedObjectsOfType);
                            }
                            float lightmapScale = this.LightmapScaleGUI(so, 1f);
                            TerrainData terrainData = selectedObjectsOfType[0].terrainData;
                            float cachedSurfaceArea = (terrainData == null) ? 0f : (terrainData.size.x * terrainData.size.z);
                            this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedSurfaceArea);
                            LightmapParametersGUI(so.FindProperty("m_LightmapParameters"), s_Styles.LightmapParameters);
                            if ((GUI.enabled && (selectedObjectsOfType.Length == 1)) && (selectedObjectsOfType[0].terrainData != null))
                            {
                                this.ShowBakePerformanceWarning(so, selectedObjectsOfType[0]);
                            }
                            this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, s_Styles.Atlas);
                            if (this.m_ShowBakedLM)
                            {
                                this.ShowAtlasGUI(so);
                            }
                            this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, s_Styles.RealtimeLM);
                            if (this.m_ShowRealtimeLM)
                            {
                                this.ShowRealtimeLMGUI(selectedObjectsOfType[0]);
                            }
                            obj2.ApplyModifiedProperties();
                            so.ApplyModifiedProperties();
                        }
                        EditorGUI.indentLevel--;
                    }
                    GUILayout.Space(10f);
                }
                else
                {
                    EditorGUILayout.HelpBox(s_Styles.TerrainLightmapInfoBox.text, MessageType.Info);
                }
            }
        }

        private void ShowAtlasGUI(SerializedObject so)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(s_Styles.AtlasIndex, new GUIContent(so.FindProperty("m_LightmapIndex").intValue.ToString()), new GUILayoutOption[0]);
            EditorGUILayout.LabelField(s_Styles.AtlasTilingX, new GUIContent(so.FindProperty("m_LightmapTilingOffset.x").floatValue.ToString()), new GUILayoutOption[0]);
            EditorGUILayout.LabelField(s_Styles.AtlasTilingY, new GUIContent(so.FindProperty("m_LightmapTilingOffset.y").floatValue.ToString()), new GUILayoutOption[0]);
            EditorGUILayout.LabelField(s_Styles.AtlasOffsetX, new GUIContent(so.FindProperty("m_LightmapTilingOffset.z").floatValue.ToString()), new GUILayoutOption[0]);
            EditorGUILayout.LabelField(s_Styles.AtlasOffsetY, new GUIContent(so.FindProperty("m_LightmapTilingOffset.w").floatValue.ToString()), new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
        }

        private void ShowBakePerformanceWarning(SerializedObject so, Terrain terrain)
        {
            LightmapParameters parameters;
            float x = terrain.terrainData.size.x;
            float z = terrain.terrainData.size.z;
            LightmapParameters objectReferenceValue = (LightmapParameters) so.FindProperty("m_LightmapParameters").objectReferenceValue;
            if (objectReferenceValue != null)
            {
                parameters = objectReferenceValue;
            }
            else
            {
                parameters = new LightmapParameters();
            }
            float num3 = (x * parameters.resolution) * LightmapEditorSettings.realtimeResolution;
            float num4 = (z * parameters.resolution) * LightmapEditorSettings.realtimeResolution;
            if ((num3 > 512f) || (num4 > 512f))
            {
                EditorGUILayout.HelpBox(s_Styles.ResolutionTooHighWarning.text, MessageType.Warning);
            }
            float num5 = num3 * parameters.clusterResolution;
            float num6 = num4 * parameters.clusterResolution;
            float num7 = ((float) terrain.terrainData.heightmapResolution) / num5;
            float num8 = ((float) terrain.terrainData.heightmapResolution) / num6;
            if ((num7 > 51.2f) || (num8 > 51.2f))
            {
                EditorGUILayout.HelpBox(s_Styles.ResolutionTooLowWarning.text, MessageType.Warning);
            }
        }

        private void ShowClampedSizeInLightmapGUI(float lightmapScale, float cachedSurfaceArea)
        {
            float num = (Mathf.Sqrt(cachedSurfaceArea) * LightmapEditorSettings.bakeResolution) * lightmapScale;
            float num2 = Math.Min(LightmapEditorSettings.maxAtlasWidth, LightmapEditorSettings.maxAtlasHeight);
            this.m_ShowClampedSize.target = num > num2;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowClampedSize.faded))
            {
                EditorGUILayout.HelpBox(s_Styles.ClampedSize.text, MessageType.Info);
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void ShowRealtimeLMGUI(Terrain terrain)
        {
            int num;
            int num2;
            int num3;
            int num4;
            EditorGUI.indentLevel++;
            if (LightmapEditorSettings.GetTerrainSystemResolution(terrain, out num, out num2, out num3, out num4))
            {
                string text = num.ToString() + "x" + num2.ToString();
                if ((num3 > 1) || (num4 > 1))
                {
                    text = text + $" ({num3}x{num4} chunks)";
                }
                EditorGUILayout.LabelField(s_Styles.RealtimeLMResolution, new GUIContent(text), new GUILayoutOption[0]);
            }
            EditorGUI.indentLevel--;
        }

        private void ShowRealtimeLMGUI(SerializedObject so, Renderer renderer)
        {
            Hash128 hash;
            Hash128 hash2;
            int num;
            int num2;
            Hash128 hash3;
            int num3;
            int num4;
            EditorGUI.indentLevel++;
            if (LightmapEditorSettings.GetInstanceHash(renderer, out hash))
            {
                EditorGUILayout.LabelField(s_Styles.RealtimeLMInstanceHash, new GUIContent(hash.ToString()), new GUILayoutOption[0]);
            }
            if (LightmapEditorSettings.GetGeometryHash(renderer, out hash2))
            {
                EditorGUILayout.LabelField(s_Styles.RealtimeLMGeometryHash, new GUIContent(hash2.ToString()), new GUILayoutOption[0]);
            }
            if (LightmapEditorSettings.GetInstanceResolution(renderer, out num, out num2))
            {
                EditorGUILayout.LabelField(s_Styles.RealtimeLMInstanceResolution, new GUIContent(num.ToString() + "x" + num2.ToString()), new GUILayoutOption[0]);
            }
            if (LightmapEditorSettings.GetInputSystemHash(renderer, out hash3))
            {
                EditorGUILayout.LabelField(s_Styles.RealtimeLMInputSystemHash, new GUIContent(hash3.ToString()), new GUILayoutOption[0]);
            }
            if (LightmapEditorSettings.GetSystemResolution(renderer, out num3, out num4))
            {
                EditorGUILayout.LabelField(s_Styles.RealtimeLMResolution, new GUIContent(num3.ToString() + "x" + num4.ToString()), new GUILayoutOption[0]);
            }
            EditorGUI.indentLevel--;
        }

        private void ShowTerrainChunks(Terrain[] terrains)
        {
            int num = 0;
            int num2 = 0;
            foreach (Terrain terrain in terrains)
            {
                int numChunksX = 0;
                int numChunksY = 0;
                Lightmapping.GetTerrainGIChunks(terrain, ref numChunksX, ref numChunksY);
                if ((num == 0) && (num2 == 0))
                {
                    num = numChunksX;
                    num2 = numChunksY;
                }
                else if ((num != numChunksX) || (num2 != numChunksY))
                {
                    num = num2 = 0;
                    break;
                }
            }
            if ((num * num2) > 1)
            {
                EditorGUILayout.HelpBox($"Terrain is chunked up into {num * num2} instances for baking.", MessageType.None);
            }
        }

        public bool ShowChartingSettings
        {
            get => 
                this.m_ShowChartingSettings;
            set
            {
                this.m_ShowChartingSettings = value;
            }
        }

        public bool ShowLightmapSettings
        {
            get => 
                this.m_ShowLightmapSettings;
            set
            {
                this.m_ShowLightmapSettings = value;
            }
        }

        public bool ShowSettings
        {
            get => 
                this.m_ShowSettings;
            set
            {
                this.m_ShowSettings = value;
            }
        }

        private class Styles
        {
            public GUIContent Atlas = EditorGUIUtility.TextContent("Baked Lightmap");
            public GUIContent AtlasIndex = EditorGUIUtility.TextContent("Lightmap Index");
            public GUIContent AtlasOffsetX = EditorGUIUtility.TextContent("Offset X");
            public GUIContent AtlasOffsetY = EditorGUIUtility.TextContent("Offset Y");
            public GUIContent AtlasTilingX = EditorGUIUtility.TextContent("Tiling X");
            public GUIContent AtlasTilingY = EditorGUIUtility.TextContent("Tiling Y");
            public GUIContent AutoUVMaxAngle = EditorGUIUtility.TextContent("Max Angle|Enlighten automatically generates simplified UVs by merging UV charts. Charts will only be simplified if the angle between the charts is smaller than this value.");
            public GUIContent AutoUVMaxDistance = EditorGUIUtility.TextContent("Max Distance|Enlighten automatically generates simplified UVs by merging UV charts. Charts will only be simplified if the worldspace distance between the charts is smaller than this value.");
            public GUIContent ChunkSize = EditorGUIUtility.TextContent("Chunk Size");
            public GUIContent ClampedPackingResolution = EditorGUIUtility.TextContent("Object's size in the realtime lightmap has reached the maximum size. If you need higher resolution for this object, divide it into smaller meshes.");
            public GUIContent ClampedSize = EditorGUIUtility.TextContent("Object's size in lightmap has reached the max atlas size.|If you need higher resolution for this object, divide it into smaller meshes or set higher max atlas size via the LightmapEditorSettings class.");
            public GUIContent EmptySelection = EditorGUIUtility.TextContent("Select a Light, Mesh Renderer or a Terrain from the scene.");
            public GUIContent GINotEnabledInfo = EditorGUIUtility.TextContent("No lightmapping settings to display (neither baked nor realtime GI is enabled).");
            public GUIContent IgnoreNormalsForChartDetection = EditorGUIUtility.TextContent("Ignore Normals|Do not compare normals when detecting charts for realtime GI. This can be necessary when using hand authored UVs to avoid unnecessary chart splits.");
            public GUIContent ImportantGI = EditorGUIUtility.TextContent("Prioritize Illumination|Make all other objects dependent upon this object. Useful for objects that will be strongly emissive to make sure that other objects will be illuminated by them.");
            public GUIContent Lighting = new GUIContent(EditorGUIUtility.TextContent("Lighting").text);
            public GUIContent LightmapInfoBox = EditorGUIUtility.TextContent("To enable generation of lightmaps for this Mesh Renderer, please enable the 'Lightmap Static' property.");
            public GUIContent LightmapParameters = EditorGUIUtility.TextContent("Lightmap Parameters|Lets you configure per instance lightmapping parameters. Objects will be automatically grouped by unique parameter sets.");
            public GUIContent LightmapSettings = EditorGUIUtility.TextContent("Lightmap Settings");
            public GUIContent LightmapStatic = EditorGUIUtility.TextContent("Lightmap Static");
            public GUIContent MinimumChartSize = EditorGUIUtility.TextContent("Min Chart Size|Directionality is generated at half resolution so in order to stitch properly at least 4x4 texels are needed in a chart so that a gradient can be generated on all sides of the chart. If stitching is not needed set this value to 2 in order to save texels for better performance at runtime and a faster lighting build.");
            public GUIContent[] MinimumChartSizeStrings = new GUIContent[] { EditorGUIUtility.TextContent("2 (Minimum)"), EditorGUIUtility.TextContent("4 (Stitchable)") };
            public int[] MinimumChartSizeValues = new int[] { 2, 4 };
            public GUIContent NoNormalsNoLightmapping = EditorGUIUtility.TextContent("Mesh used by the renderer doesn't have normals. Normals are needed for lightmapping.");
            public GUIContent OptimizeRealtimeUVs = EditorGUIUtility.TextContent("Optimize Realtime UVs|Specifies whether Unity will use the meshes existing UVs or generate a new set of UVs for Realtime Global Illumination. When enabled, the existing UVs on the mesh get ignored and a new set of UVs are auto-generated. When disabled, the existing UVs on the mesh get used and no new UVs are auto-generated for lighting purposes.");
            public GUIContent RealtimeLM = EditorGUIUtility.TextContent("Realtime Lightmap");
            public GUIContent RealtimeLMGeometryHash = EditorGUIUtility.TextContent("Geometry Hash|The hash of the realtime GI geometry that the renderer is using.");
            public GUIContent RealtimeLMInputSystemHash = EditorGUIUtility.TextContent("System Hash|The hash of the realtime system that the renderer belongs to.");
            public GUIContent RealtimeLMInstanceHash = EditorGUIUtility.TextContent("Instance Hash|The hash of the realtime GI instance.");
            public GUIContent RealtimeLMInstanceResolution = EditorGUIUtility.TextContent("Instance Resolution|The resolution in texels of the realtime lightmap packed instance.");
            public GUIContent RealtimeLMResolution = EditorGUIUtility.TextContent("System Resolution|The resolution in texels of the realtime lightmap that this renderer belongs to.");
            public GUIContent ResolutionTooHighWarning = EditorGUIUtility.TextContent("Precompute/indirect resolution for this terrain is probably too high. Use a lower realtime/indirect resolution setting in the Lighting window or assign LightmapParameters that use a lower resolution setting. Otherwise it may take a very long time to bake and memory consumption during and after the bake may be very high.");
            public GUIContent ResolutionTooLowWarning = EditorGUIUtility.TextContent("Precompute/indirect resolution for this terrain is probably too low. If the Clustering stage takes a long time, try using a higher realtime/indirect resolution setting in the Lighting window or assign LightmapParameters that use a higher resolution setting.");
            public GUIContent ScaleInLightmap = EditorGUIUtility.TextContent("Scale In Lightmap|Object's surface multiplied by this value determines it's size in the lightmap. 0 - don't lightmap this object.");
            public GUIContent TerrainLightmapInfoBox = EditorGUIUtility.TextContent("To enable generation of lightmaps for this Mesh Renderer, please enable the 'Lightmap Static' property.");
            public GUIContent TerrainLightmapSize = EditorGUIUtility.TextContent("Lightmap Size|Defines the size of the lightmap that will be used only by this terrain.");
            public GUIContent UVCharting = EditorGUIUtility.TextContent("UV Charting Control");
            public GUIContent ZeroAreaPackingMesh = EditorGUIUtility.TextContent("Mesh used by the renderer has zero UV or surface area. Non zero area is required for lightmapping.");
        }
    }
}

