namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngineInternal;

    internal class LightingWindowObjectTab
    {
        private static GUIContent[] kObjectPreviewTextureOptions = new GUIContent[] { EditorGUIUtility.TextContent("Charting"), EditorGUIUtility.TextContent("Albedo"), EditorGUIUtility.TextContent("Emissive"), EditorGUIUtility.TextContent("Realtime Intensity"), EditorGUIUtility.TextContent("Realtime Direction"), EditorGUIUtility.TextContent("Baked Intensity"), EditorGUIUtility.TextContent("Baked Direction") };
        private GITextureType[] kObjectPreviewTextureTypes = new GITextureType[] { GITextureType.Charting };
        private bool m_HasSeparateIndirectUV = false;
        private Editor m_LightEditor;
        private Editor m_LightmapParametersEditor;
        private int m_PreviousSelection;
        private GUIContent m_SelectedObjectPreviewTexture;
        private bool m_ShowBakedLM = false;
        private AnimBool m_ShowClampedSize = new AnimBool();
        private bool m_ShowRealtimeLM = false;
        private ZoomableArea m_ZoomablePreview;
        private static Styles s_Styles;

        private Rect CenterToRect(Rect rect, Rect to)
        {
            float num = Mathf.Clamp((float) (((float) ((int) (to.width - rect.width))) / 2f), (float) 0f, (float) 2.147484E+09f);
            float num2 = Mathf.Clamp((float) (((float) ((int) (to.height - rect.height))) / 2f), (float) 0f, (float) 2.147484E+09f);
            return new Rect(rect.x + num, rect.y + num2, rect.width, rect.height);
        }

        public bool EditLightmapParameters()
        {
            UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(LightmapParameters), UnityEditor.SelectionMode.Unfiltered);
            if (filtered.Length == 0)
            {
                return false;
            }
            EditorGUILayout.MultiSelectionObjectTitleBar(filtered);
            this.GetLightmapParametersEditor(filtered).OnInspectorGUI();
            GUILayout.Space(10f);
            return true;
        }

        public bool EditLights()
        {
            GameObject[] objArray;
            Light[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Light>(out objArray, new System.Type[0]);
            if (objArray.Length == 0)
            {
                return false;
            }
            EditorGUILayout.InspectorTitlebar(selectedObjectsOfType);
            this.GetLightEditor(selectedObjectsOfType).OnInspectorGUI();
            GUILayout.Space(10f);
            return true;
        }

        public bool EditRenderers()
        {
            GameObject[] objArray;
            System.Type[] types = new System.Type[] { typeof(MeshRenderer), typeof(SkinnedMeshRenderer) };
            Renderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Renderer>(out objArray, types);
            if (objArray.Length == 0)
            {
                return false;
            }
            EditorGUILayout.InspectorTitlebar(selectedObjectsOfType);
            SerializedObject obj2 = new SerializedObject(objArray);
            using (new EditorGUI.DisabledScope(!SceneModeUtility.StaticFlagField("Lightmap Static", obj2.FindProperty("m_StaticEditorFlags"), 1)))
            {
                SerializedObject so = new SerializedObject(selectedObjectsOfType);
                float lightmapLODLevelScale = LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[0]);
                for (int i = 1; i < selectedObjectsOfType.Length; i++)
                {
                    if (!Mathf.Approximately(lightmapLODLevelScale, LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[i])))
                    {
                        lightmapLODLevelScale = 1f;
                    }
                }
                float lightmapScale = this.LightmapScaleGUI(so, lightmapLODLevelScale) * LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[0]);
                float cachedSurfaceArea = !(selectedObjectsOfType[0] is MeshRenderer) ? InternalMeshUtil.GetCachedSkinnedMeshSurfaceArea(selectedObjectsOfType[0] as SkinnedMeshRenderer) : InternalMeshUtil.GetCachedMeshSurfaceArea(selectedObjectsOfType[0] as MeshRenderer);
                this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedSurfaceArea);
                EditorGUILayout.PropertyField(so.FindProperty("m_ImportantGI"), s_Styles.ImportantGI, new GUILayoutOption[0]);
                LightmapParametersGUI(so.FindProperty("m_LightmapParameters"), s_Styles.LightmapParameters, true);
                GUILayout.Space(10f);
                this.RendererUVSettings(so);
                GUILayout.Space(10f);
                this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, s_Styles.Atlas, true);
                if (this.m_ShowBakedLM)
                {
                    this.ShowAtlasGUI(so);
                }
                this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, s_Styles.RealtimeLM, true);
                if (this.m_ShowRealtimeLM)
                {
                    this.ShowRealtimeLMGUI(so, selectedObjectsOfType[0]);
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
            GUILayout.Space(10f);
            return true;
        }

        public bool EditTerrains()
        {
            GameObject[] objArray;
            Terrain[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out objArray, new System.Type[0]);
            if (objArray.Length == 0)
            {
                return false;
            }
            EditorGUILayout.InspectorTitlebar(selectedObjectsOfType);
            SerializedObject obj2 = new SerializedObject(objArray);
            using (new EditorGUI.DisabledScope(!SceneModeUtility.StaticFlagField("Lightmap Static", obj2.FindProperty("m_StaticEditorFlags"), 1)))
            {
                if (GUI.enabled)
                {
                    this.ShowTerrainChunks(selectedObjectsOfType);
                }
                SerializedObject so = new SerializedObject(selectedObjectsOfType.ToArray<Terrain>());
                float lightmapScale = this.LightmapScaleGUI(so, 1f);
                TerrainData terrainData = selectedObjectsOfType[0].terrainData;
                float cachedSurfaceArea = (terrainData == null) ? 0f : (terrainData.size.x * terrainData.size.z);
                this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedSurfaceArea);
                LightmapParametersGUI(so.FindProperty("m_LightmapParameters"), s_Styles.LightmapParameters, true);
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
            GUILayout.Space(10f);
            return true;
        }

        private Editor GetLightEditor(Light[] lights)
        {
            Editor.CreateCachedEditor(lights, typeof(LightEditor), ref this.m_LightEditor);
            return this.m_LightEditor;
        }

        private Editor GetLightmapParametersEditor(UnityEngine.Object[] lights)
        {
            Editor.CreateCachedEditor(lights, typeof(LightmapParametersEditor), ref this.m_LightmapParametersEditor);
            return this.m_LightmapParametersEditor;
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
            return InternalMeshUtil.HasNormals(sharedMesh);
        }

        private static bool isBuiltIn(SerializedProperty prop)
        {
            if (prop.objectReferenceValue != null)
            {
                LightmapParameters objectReferenceValue = prop.objectReferenceValue as LightmapParameters;
                return (objectReferenceValue.hideFlags == HideFlags.NotEditable);
            }
            return true;
        }

        public static bool LightmapParametersGUI(SerializedProperty prop, GUIContent content, bool advancedParameters)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (advancedParameters)
            {
                EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Default scene parameter");
            }
            else
            {
                EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Default-Medium");
            }
            string text = "Edit...";
            if (isBuiltIn(prop))
            {
                text = "View";
            }
            bool flag = false;
            if (prop.objectReferenceValue == null)
            {
                SerializedProperty property = new SerializedObject(LightmapEditorSettings.GetLightmapSettings()).FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
                using (new EditorGUI.DisabledScope(property == null))
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(text, EditorStyles.miniButton, options))
                    {
                        Selection.activeObject = property.objectReferenceValue;
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

        public void ObjectPreview(Rect r)
        {
            if (r.height > 0f)
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                List<Texture2D> list = new List<Texture2D>();
                foreach (GITextureType type in this.kObjectPreviewTextureTypes)
                {
                    list.Add(LightmapVisualizationUtility.GetGITexture(type));
                }
                if (list.Count != 0)
                {
                    if (this.m_ZoomablePreview == null)
                    {
                        this.m_ZoomablePreview = new ZoomableArea(true);
                        this.m_ZoomablePreview.hRangeMin = 0f;
                        this.m_ZoomablePreview.vRangeMin = 0f;
                        this.m_ZoomablePreview.hRangeMax = 1f;
                        this.m_ZoomablePreview.vRangeMax = 1f;
                        this.m_ZoomablePreview.SetShownHRange(0f, 1f);
                        this.m_ZoomablePreview.SetShownVRange(0f, 1f);
                        this.m_ZoomablePreview.uniformScale = true;
                        this.m_ZoomablePreview.scaleWithWindow = true;
                    }
                    GUI.Box(r, "", "PreBackground");
                    Rect position = new Rect(r);
                    position.y++;
                    position.height = 18f;
                    GUI.Box(position, "", EditorStyles.toolbar);
                    Rect rect2 = new Rect(r);
                    rect2.y++;
                    rect2.height = 18f;
                    rect2.width = 120f;
                    Rect to = new Rect(r);
                    to.yMin += rect2.height;
                    to.yMax -= 14f;
                    to.width -= 11f;
                    int index = Array.IndexOf<GUIContent>(kObjectPreviewTextureOptions, this.m_SelectedObjectPreviewTexture);
                    if (index < 0)
                    {
                        index = 0;
                    }
                    index = EditorGUI.Popup(rect2, index, kObjectPreviewTextureOptions, EditorStyles.toolbarPopup);
                    if (index >= kObjectPreviewTextureOptions.Length)
                    {
                        index = 0;
                    }
                    this.m_SelectedObjectPreviewTexture = kObjectPreviewTextureOptions[index];
                    LightmapType lightmapType = ((this.kObjectPreviewTextureTypes[index] != GITextureType.Baked) && (this.kObjectPreviewTextureTypes[index] != GITextureType.BakedDirectional)) ? LightmapType.DynamicLightmap : LightmapType.StaticLightmap;
                    bool flag = ((this.kObjectPreviewTextureTypes[index] == GITextureType.Baked) || (this.kObjectPreviewTextureTypes[index] == GITextureType.BakedDirectional)) && (LightmapSettings.lightmapsMode == LightmapsMode.SeparateDirectional);
                    if (flag)
                    {
                        GUIContent content = GUIContent.Temp("Indirect");
                        Rect rect4 = rect2;
                        rect4.x += rect2.width;
                        rect4.width = EditorStyles.toolbarButton.CalcSize(content).x;
                        this.m_HasSeparateIndirectUV = GUI.Toggle(rect4, this.m_HasSeparateIndirectUV, content.text, EditorStyles.toolbarButton);
                    }
                    switch (Event.current.type)
                    {
                        case EventType.ValidateCommand:
                        case EventType.ExecuteCommand:
                            if (Event.current.commandName == "FrameSelected")
                            {
                                Vector4 lightmapTilingOffset = LightmapVisualizationUtility.GetLightmapTilingOffset(lightmapType);
                                Vector2 lhs = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
                                Vector2 vector4 = lhs + new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
                                lhs = Vector2.Max(lhs, Vector2.zero);
                                vector4 = Vector2.Min(vector4, Vector2.one);
                                float num3 = 1f - lhs.y;
                                lhs.y = 1f - vector4.y;
                                vector4.y = num3;
                                Rect rect5 = new Rect(lhs.x, lhs.y, vector4.x - lhs.x, vector4.y - lhs.y);
                                rect5.x -= Mathf.Clamp(rect5.height - rect5.width, 0f, float.MaxValue) / 2f;
                                rect5.y -= Mathf.Clamp(rect5.width - rect5.height, 0f, float.MaxValue) / 2f;
                                float num4 = Mathf.Max(rect5.width, rect5.height);
                                rect5.height = num4;
                                rect5.width = num4;
                                if (flag && this.m_HasSeparateIndirectUV)
                                {
                                    rect5.x += 0.5f;
                                }
                                this.m_ZoomablePreview.shownArea = rect5;
                                Event.current.Use();
                            }
                            break;

                        case EventType.Repaint:
                        {
                            Texture2D texture = list[index];
                            if ((texture != null) && (Event.current.type == EventType.Repaint))
                            {
                                Rect rect = new Rect(0f, 0f, (float) texture.width, (float) texture.height);
                                rect = this.ResizeRectToFit(rect, to);
                                rect = this.CenterToRect(rect, to);
                                rect = this.ScaleRectByZoomableArea(rect, this.m_ZoomablePreview);
                                Rect rect7 = new Rect(rect);
                                rect7.x += 3f;
                                rect7.y += to.y + 20f;
                                Rect drawableArea = new Rect(to);
                                drawableArea.y += rect2.height + 3f;
                                float num5 = drawableArea.y - 14f;
                                rect7.y -= num5;
                                drawableArea.y -= num5;
                                UnityEngine.FilterMode filterMode = texture.filterMode;
                                texture.filterMode = UnityEngine.FilterMode.Point;
                                GITextureType textureType = this.kObjectPreviewTextureTypes[index];
                                bool drawSpecularUV = flag && this.m_HasSeparateIndirectUV;
                                LightmapVisualizationUtility.DrawTextureWithUVOverlay(texture, Selection.activeGameObject, drawableArea, rect7, textureType, drawSpecularUV);
                                texture.filterMode = filterMode;
                            }
                            break;
                        }
                    }
                    if (this.m_PreviousSelection != Selection.activeInstanceID)
                    {
                        this.m_PreviousSelection = Selection.activeInstanceID;
                        this.m_ZoomablePreview.SetShownHRange(0f, 1f);
                        this.m_ZoomablePreview.SetShownVRange(0f, 1f);
                    }
                    Rect rect9 = new Rect(r);
                    rect9.yMin += rect2.height;
                    this.m_ZoomablePreview.rect = rect9;
                    this.m_ZoomablePreview.BeginViewGUI();
                    this.m_ZoomablePreview.EndViewGUI();
                    GUILayoutUtility.GetRect(r.width, r.height);
                }
            }
        }

        public void ObjectSettings()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            System.Type[] types = new System.Type[] { typeof(Light), typeof(Renderer), typeof(Terrain) };
            SceneModeUtility.SearchBar(types);
            EditorGUILayout.Space();
            bool flag = false;
            flag |= this.EditRenderers();
            flag |= this.EditLightmapParameters();
            flag |= this.EditLights();
            if (!(flag | this.EditTerrains()))
            {
                GUILayout.Label(s_Styles.EmptySelection, EditorStyles.helpBox, new GUILayoutOption[0]);
            }
        }

        public void OnDisable()
        {
            UnityEngine.Object.DestroyImmediate(this.m_LightEditor);
            UnityEngine.Object.DestroyImmediate(this.m_LightmapParametersEditor);
        }

        public void OnEnable(EditorWindow window)
        {
            this.m_ShowClampedSize.value = false;
            this.m_ShowClampedSize.valueChanged.AddListener(new UnityAction(window.Repaint));
        }

        private void RendererUVSettings(SerializedObject so)
        {
            SerializedProperty property = so.FindProperty("m_PreserveUVs");
            EditorGUILayout.PropertyField(property, s_Styles.PreserveUVs, new GUILayoutOption[0]);
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
            EditorGUILayout.PropertyField(so.FindProperty("m_IgnoreNormalsForChartDetection"), s_Styles.IgnoreNormalsForChartDetection, new GUILayoutOption[0]);
            EditorGUILayout.IntPopup(so.FindProperty("m_MinimumChartSize"), s_Styles.MinimumChartSizeStrings, s_Styles.MinimumChartSizeValues, s_Styles.MinimumChartSize, new GUILayoutOption[0]);
        }

        private Rect ResizeRectToFit(Rect rect, Rect to)
        {
            float a = to.width / rect.width;
            float b = to.height / rect.height;
            float num3 = Mathf.Min(a, b);
            float width = (int) Mathf.Round(rect.width * num3);
            return new Rect(rect.x, rect.y, width, (int) Mathf.Round(rect.height * num3));
        }

        private Rect ScaleRectByZoomableArea(Rect rect, ZoomableArea zoomableArea)
        {
            float num = -(zoomableArea.shownArea.x / zoomableArea.shownArea.width) * rect.width;
            float num2 = ((zoomableArea.shownArea.y - (1f - zoomableArea.shownArea.height)) / zoomableArea.shownArea.height) * rect.height;
            float width = rect.width / zoomableArea.shownArea.width;
            return new Rect(rect.x + num, rect.y + num2, width, rect.height / zoomableArea.shownArea.height);
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
                EditorGUILayout.HelpBox("Baking resolution for this terrain probably is TOO HIGH. Try use a lower resolution parameter set otherwise it may take long or even infinite time to bake and memory consumption during baking may get greatly increased as well.", MessageType.Warning);
            }
            float num5 = num3 * parameters.clusterResolution;
            float num6 = num4 * parameters.clusterResolution;
            float num7 = ((float) terrain.terrainData.heightmapResolution) / num5;
            float num8 = ((float) terrain.terrainData.heightmapResolution) / num6;
            if ((num7 > 51.2f) || (num8 > 51.2f))
            {
                EditorGUILayout.HelpBox("Baking resolution for this terrain probably is TOO LOW. If it takes long time in Clustering stage, try use a higher resolution parameter set.", MessageType.Warning);
            }
        }

        private void ShowClampedSizeInLightmapGUI(float lightmapScale, float cachedSurfaceArea)
        {
            float num = (Mathf.Sqrt(cachedSurfaceArea) * LightmapEditorSettings.bakeResolution) * lightmapScale;
            float num2 = Math.Min(LightmapEditorSettings.maxAtlasWidth, LightmapEditorSettings.maxAtlasHeight);
            this.m_ShowClampedSize.target = num > num2;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowClampedSize.faded))
            {
                GUILayout.Label(s_Styles.ClampedSize, EditorStyles.helpBox, new GUILayoutOption[0]);
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
                GUILayout.Label($"Terrain is chunked up into {num * num2} instances for baking.", EditorStyles.helpBox, new GUILayoutOption[0]);
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
            public GUIContent AutoUVMaxAngle = EditorGUIUtility.TextContent("Auto UV Max Angle|Enlighten automatically generates simplified UVs by merging UV charts. Charts will only be simplified if the angle between the charts is smaller than this value.");
            public GUIContent AutoUVMaxDistance = EditorGUIUtility.TextContent("Auto UV Max Distance|Enlighten automatically generates simplified UVs by merging UV charts. Charts will only be simplified if the worldspace distance between the charts is smaller than this value.");
            public GUIContent ChunkSize = EditorGUIUtility.TextContent("Chunk Size");
            public GUIContent ClampedPackingResolution = EditorGUIUtility.TextContent("Object's size in the realtime lightmap has reached the maximum size.|If you need higher resolution for this object, divide it into smaller meshes.");
            public GUIContent ClampedSize = EditorGUIUtility.TextContent("Object's size in lightmap has reached the max atlas size.|If you need higher resolution for this object, divide it into smaller meshes or set higher max atlas size via the LightmapEditorSettings class.");
            public GUIContent EmptySelection = EditorGUIUtility.TextContent("Select a Light, Mesh Renderer or a Terrain from the scene.");
            public GUIContent IgnoreNormalsForChartDetection = EditorGUIUtility.TextContent("Ignore Normals|Do not compare normals when detecting charts for realtime GI. This can be necessary when using hand authored UVs to avoid unnecessary chart splits.");
            public GUIContent ImportantGI = EditorGUIUtility.TextContent("Important GI|Make all other objects dependent upon this object. Useful for objects that will be strongly emissive to make sure that other objects will be illuminated by it.");
            public GUIContent LightmapParameters = EditorGUIUtility.TextContent("Advanced Parameters|Lets you configure per instance lightmapping parameters. Objects will be automatically grouped by unique parameter sets.");
            public GUIContent MinimumChartSize = EditorGUIUtility.TextContent("Min Chart Size|Directionality is generated at half resolution so in order to stitch properly at least 4x4 texels are needed in a chart so that a gradient can be generated on all sides of the chart. If stitching is not needed set this value to 2 in order to save texels for better performance at runtime and a faster lighting build.");
            public GUIContent[] MinimumChartSizeStrings = new GUIContent[] { EditorGUIUtility.TextContent("2 (Minimum)"), EditorGUIUtility.TextContent("4 (Stitchable)") };
            public int[] MinimumChartSizeValues = new int[] { 2, 4 };
            public GUIContent NoNormalsNoLightmapping = EditorGUIUtility.TextContent("Mesh used by the renderer doesn't have normals. Normals are needed for lightmapping.");
            public GUIContent PreserveUVs = EditorGUIUtility.TextContent("Preserve UVs|Preserve the incoming lightmap UVs when generating realtime GI UVs. The incoming UVs are packed but charts are not scaled or merged. This is necessary for correct edge stitching of axis aligned chart edges.");
            public GUIContent RealtimeLM = EditorGUIUtility.TextContent("Realtime Lightmap");
            public GUIContent RealtimeLMGeometryHash = EditorGUIUtility.TextContent("Geometry Hash|The hash of the realtime GI geometry that the renderer is using.");
            public GUIContent RealtimeLMInputSystemHash = EditorGUIUtility.TextContent("System Hash|The hash of the realtime system that the renderer belongs to.");
            public GUIContent RealtimeLMInstanceHash = EditorGUIUtility.TextContent("Instance Hash|The hash of the realtime GI instance.");
            public GUIContent RealtimeLMInstanceResolution = EditorGUIUtility.TextContent("Instance Resolution|The resolution in texels of the realtime lightmap packed instance.");
            public GUIContent RealtimeLMResolution = EditorGUIUtility.TextContent("System Resolution|The resolution in texels of the realtime lightmap that this renderer belongs to.");
            public GUIContent ScaleInLightmap = EditorGUIUtility.TextContent("Scale In Lightmap|Object's surface multiplied by this value determines it's size in the lightmap. 0 - don't lightmap this object.");
            public GUIContent TerrainLightmapSize = EditorGUIUtility.TextContent("Lightmap Size|Defines the size of the lightmap that will be used only by this terrain.");
            public GUIContent ZeroAreaPackingMesh = EditorGUIUtility.TextContent("Mesh used by the renderer has zero UV or surface area. Non zero area is required for lightmapping.");
        }
    }
}

