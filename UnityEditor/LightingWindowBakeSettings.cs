namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngineInternal;

    internal class LightingWindowBakeSettings
    {
        private const string kShowGeneralLightmapSettingsKey = "ShowGeneralLightmapSettings";
        private const string kShowMixedLightsSettingsKey = "ShowMixedLightsSettings";
        private const string kShowRealtimeLightsSettingsKey = "ShowRealtimeLightsSettings";
        private SerializedProperty m_AlbedoBoost;
        private SerializedProperty m_AmbientOcclusion;
        private SerializedProperty m_AOMaxDistance;
        private SerializedProperty m_BakeBackend;
        private SerializedProperty m_BakeResolution;
        private SerializedProperty m_BounceScale;
        private SerializedProperty m_CompAOExponent;
        private SerializedProperty m_CompAOExponentDirect;
        private SerializedProperty m_FinalGather;
        private SerializedProperty m_FinalGatherFiltering;
        private SerializedProperty m_FinalGatherRayCount;
        private SerializedProperty m_IndirectOutputScale;
        private SerializedProperty m_LightmapDirectionalMode;
        private SerializedProperty m_LightmapParameters;
        private UnityEngine.Object m_LightmapSettings;
        private SerializedObject m_LightmapSettingsSO;
        private SerializedProperty m_LightmapSize;
        private LightModeUtil m_LightModeUtil = LightModeUtil.Get();
        private SerializedProperty m_Padding;
        private SerializedProperty m_PVRBounces;
        private SerializedProperty m_PVRCulling;
        private SerializedProperty m_PVRDirectSampleCount;
        private SerializedProperty m_PVRFilteringGaussRadiusAO;
        private SerializedProperty m_PVRFilteringGaussRadiusDirect;
        private SerializedProperty m_PVRFilteringGaussRadiusIndirect;
        private SerializedProperty m_PVRFilteringMode;
        private SerializedProperty m_PVRSampleCount;
        private SerializedObject m_RenderSettingsSO;
        private SerializedProperty m_Resolution;
        private bool m_ShowGeneralLightmapSettings = true;
        private bool m_ShowMixedLightsSettings = true;
        private bool m_ShowRealtimeLightsSettings = true;
        private SerializedProperty m_SubtractiveShadowColor;
        private SerializedProperty m_TextureCompression;
        private SerializedProperty m_UpdateThreshold;

        public void DeveloperBuildSettingsGUI()
        {
            if (Unsupported.IsDeveloperBuild())
            {
                Lightmapping.concurrentJobsType = (Lightmapping.ConcurrentJobsType) EditorGUILayout.IntPopup(Styles.ConcurrentJobs, (int) Lightmapping.concurrentJobsType, Styles.ConcurrentJobsTypeStrings, Styles.ConcurrentJobsTypeValues, new GUILayoutOption[0]);
                Lightmapping.enlightenForceUpdates = EditorGUILayout.Toggle(Styles.ForceUpdates, Lightmapping.enlightenForceUpdates, new GUILayoutOption[0]);
                Lightmapping.enlightenForceWhiteAlbedo = EditorGUILayout.Toggle(Styles.ForceWhiteAlbedo, Lightmapping.enlightenForceWhiteAlbedo, new GUILayoutOption[0]);
                Lightmapping.filterMode = (UnityEngine.FilterMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), Lightmapping.filterMode, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_BounceScale, 0f, 10f, Styles.BounceScale, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_UpdateThreshold, 0f, 1f, Styles.UpdateThreshold, new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(150f) };
                if (GUILayout.Button("Clear disk cache", options))
                {
                    Lightmapping.Clear();
                    Lightmapping.ClearDiskCache();
                }
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(150f) };
                if (GUILayout.Button("Print state to console", optionArray2))
                {
                    Lightmapping.PrintStateToConsole();
                }
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(150f) };
                if (GUILayout.Button("Reset albedo/emissive", optionArray3))
                {
                    GIDebugVisualisation.ResetRuntimeInputTextures();
                }
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(150f) };
                if (GUILayout.Button("Reset environment", optionArray4))
                {
                    DynamicGI.UpdateEnvironment();
                }
            }
        }

        private static void DrawResolutionField(SerializedProperty resolution, GUIContent label)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(resolution, label, new GUILayoutOption[0]);
            GUILayout.Label(" texels per unit", Styles.LabelStyle, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
        }

        private void GeneralLightmapSettingsGUI()
        {
            this.m_ShowGeneralLightmapSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowGeneralLightmapSettings, Styles.GeneralLightmapLabel, true);
            if (this.m_ShowGeneralLightmapSettings)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!LightModeUtil.Get().IsAnyGIEnabled()))
                {
                    using (new EditorGUI.DisabledScope(!LightModeUtil.Get().AreBakedLightmapsEnabled()))
                    {
                        EditorGUILayout.PropertyField(this.m_BakeBackend, Styles.BakeBackend, new GUILayoutOption[0]);
                        if (LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer)
                        {
                            EditorGUILayout.HelpBox(Styles.NoTransparencyAndLODInProgressive.text, MessageType.Warning);
                        }
                    }
                    using (new EditorGUI.DisabledScope(!LightModeUtil.Get().AreBakedLightmapsEnabled()))
                    {
                        if (LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(this.m_PVRCulling, Styles.PVRCulling, new GUILayoutOption[0]);
                            if (LightmapEditorSettings.giPathTracerSampling != LightmapEditorSettings.PathTracerSampling.Auto)
                            {
                                EditorGUILayout.PropertyField(this.m_PVRDirectSampleCount, Styles.PVRDirectSampleCount, new GUILayoutOption[0]);
                                EditorGUILayout.PropertyField(this.m_PVRSampleCount, Styles.PVRIndirectSampleCount, new GUILayoutOption[0]);
                                if ((this.m_PVRSampleCount.intValue < 10) || (this.m_PVRSampleCount.intValue > 0x186a0))
                                {
                                    this.m_PVRSampleCount.intValue = Math.Max(Math.Min(this.m_PVRSampleCount.intValue, 0x186a0), 10);
                                }
                            }
                            EditorGUILayout.IntPopup(this.m_PVRBounces, Styles.BouncesStrings, Styles.BouncesValues, Styles.PVRBounces, new GUILayoutOption[0]);
                            EditorGUILayout.PropertyField(this.m_PVRFilteringMode, Styles.PVRFilteringMode, new GUILayoutOption[0]);
                            if (this.m_PVRFilteringMode.enumValueIndex == 2)
                            {
                                EditorGUI.indentLevel++;
                                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                EditorGUILayout.IntSlider(this.m_PVRFilteringGaussRadiusDirect, 0, 5, Styles.PVRFilteringGaussRadiusDirect, new GUILayoutOption[0]);
                                GUILayout.Label(" texels", Styles.LabelStyle, new GUILayoutOption[0]);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                EditorGUILayout.IntSlider(this.m_PVRFilteringGaussRadiusIndirect, 0, 5, Styles.PVRFilteringGaussRadiusIndirect, new GUILayoutOption[0]);
                                GUILayout.Label(" texels", Styles.LabelStyle, new GUILayoutOption[0]);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                EditorGUILayout.IntSlider(this.m_PVRFilteringGaussRadiusAO, 0, 5, Styles.PVRFilteringGaussRadiusAO, new GUILayoutOption[0]);
                                GUILayout.Label(" texels", Styles.LabelStyle, new GUILayoutOption[0]);
                                GUILayout.EndHorizontal();
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.Space();
                        }
                    }
                    using (new EditorGUI.DisabledScope((LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer) && !LightModeUtil.Get().IsRealtimeGIEnabled()))
                    {
                        DrawResolutionField(this.m_Resolution, Styles.IndirectResolution);
                    }
                    using (new EditorGUI.DisabledScope(!LightModeUtil.Get().AreBakedLightmapsEnabled()))
                    {
                        DrawResolutionField(this.m_BakeResolution, Styles.LightmapResolution);
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_Padding, Styles.Padding, new GUILayoutOption[0]);
                        GUILayout.Label(" texels", Styles.LabelStyle, new GUILayoutOption[0]);
                        GUILayout.EndHorizontal();
                        EditorGUILayout.IntPopup(this.m_LightmapSize, Styles.LightmapSizeStrings, Styles.LightmapSizeValues, Styles.LightmapSize, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_TextureCompression, Styles.TextureCompression, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_AmbientOcclusion, Styles.AmbientOcclusion, new GUILayoutOption[0]);
                        if (this.m_AmbientOcclusion.boolValue)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(this.m_AOMaxDistance, Styles.AOMaxDistance, new GUILayoutOption[0]);
                            if (this.m_AOMaxDistance.floatValue < 0f)
                            {
                                this.m_AOMaxDistance.floatValue = 0f;
                            }
                            EditorGUILayout.Slider(this.m_CompAOExponent, 0f, 10f, Styles.AmbientOcclusionContribution, new GUILayoutOption[0]);
                            EditorGUILayout.Slider(this.m_CompAOExponentDirect, 0f, 10f, Styles.AmbientOcclusionContributionDirect, new GUILayoutOption[0]);
                            EditorGUI.indentLevel--;
                        }
                        if (LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.Radiosity)
                        {
                            EditorGUILayout.PropertyField(this.m_FinalGather, Styles.FinalGather, new GUILayoutOption[0]);
                            if (this.m_FinalGather.boolValue)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(this.m_FinalGatherRayCount, Styles.FinalGatherRayCount, new GUILayoutOption[0]);
                                EditorGUILayout.PropertyField(this.m_FinalGatherFiltering, Styles.FinalGatherFiltering, new GUILayoutOption[0]);
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    EditorGUILayout.IntPopup(this.m_LightmapDirectionalMode, Styles.LightmapDirectionalModeStrings, Styles.LightmapDirectionalModeValues, Styles.LightmapDirectionalMode, new GUILayoutOption[0]);
                    if (this.m_LightmapDirectionalMode.intValue == 1)
                    {
                        EditorGUILayout.HelpBox(Styles.NoDirectionalInSM2AndGLES2.text, MessageType.Warning);
                    }
                    EditorGUILayout.Slider(this.m_IndirectOutputScale, 0f, 5f, Styles.IndirectOutputScale, new GUILayoutOption[0]);
                    EditorGUILayout.Slider(this.m_AlbedoBoost, 1f, 10f, Styles.AlbedoBoost, new GUILayoutOption[0]);
                    if (LightmapParametersGUI(this.m_LightmapParameters, Styles.DefaultLightmapParameters))
                    {
                        EditorWindow.FocusWindowIfItsOpen<InspectorWindow>();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
            }
        }

        private void InitSettings()
        {
            UnityEngine.Object renderSettings = RenderSettings.GetRenderSettings();
            this.m_SubtractiveShadowColor = (this.m_RenderSettingsSO = new SerializedObject(renderSettings)).FindProperty("m_SubtractiveShadowColor");
            this.m_LightmapSettings = LightmapEditorSettings.GetLightmapSettings();
            SerializedObject obj5 = this.m_LightmapSettingsSO = new SerializedObject(this.m_LightmapSettings);
            this.m_Resolution = obj5.FindProperty("m_LightmapEditorSettings.m_Resolution");
            this.m_AlbedoBoost = obj5.FindProperty("m_GISettings.m_AlbedoBoost");
            this.m_IndirectOutputScale = obj5.FindProperty("m_GISettings.m_IndirectOutputScale");
            this.m_LightmapParameters = obj5.FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
            this.m_LightmapDirectionalMode = obj5.FindProperty("m_LightmapEditorSettings.m_LightmapsBakeMode");
            this.m_BakeResolution = obj5.FindProperty("m_LightmapEditorSettings.m_BakeResolution");
            this.m_Padding = obj5.FindProperty("m_LightmapEditorSettings.m_Padding");
            this.m_AmbientOcclusion = obj5.FindProperty("m_LightmapEditorSettings.m_AO");
            this.m_AOMaxDistance = obj5.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance");
            this.m_CompAOExponent = obj5.FindProperty("m_LightmapEditorSettings.m_CompAOExponent");
            this.m_CompAOExponentDirect = obj5.FindProperty("m_LightmapEditorSettings.m_CompAOExponentDirect");
            this.m_TextureCompression = obj5.FindProperty("m_LightmapEditorSettings.m_TextureCompression");
            this.m_FinalGather = obj5.FindProperty("m_LightmapEditorSettings.m_FinalGather");
            this.m_FinalGatherRayCount = obj5.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount");
            this.m_FinalGatherFiltering = obj5.FindProperty("m_LightmapEditorSettings.m_FinalGatherFiltering");
            this.m_LightmapSize = obj5.FindProperty("m_LightmapEditorSettings.m_TextureWidth");
            this.m_BakeBackend = obj5.FindProperty("m_LightmapEditorSettings.m_BakeBackend");
            this.m_PVRSampleCount = obj5.FindProperty("m_LightmapEditorSettings.m_PVRSampleCount");
            this.m_PVRDirectSampleCount = obj5.FindProperty("m_LightmapEditorSettings.m_PVRDirectSampleCount");
            this.m_PVRBounces = obj5.FindProperty("m_LightmapEditorSettings.m_PVRBounces");
            this.m_PVRCulling = obj5.FindProperty("m_LightmapEditorSettings.m_PVRCulling");
            this.m_PVRFilteringMode = obj5.FindProperty("m_LightmapEditorSettings.m_PVRFilteringMode");
            this.m_PVRFilteringGaussRadiusDirect = obj5.FindProperty("m_LightmapEditorSettings.m_PVRFilteringGaussRadiusDirect");
            this.m_PVRFilteringGaussRadiusIndirect = obj5.FindProperty("m_LightmapEditorSettings.m_PVRFilteringGaussRadiusIndirect");
            this.m_PVRFilteringGaussRadiusAO = obj5.FindProperty("m_LightmapEditorSettings.m_PVRFilteringGaussRadiusAO");
            this.m_BounceScale = obj5.FindProperty("m_GISettings.m_BounceScale");
            this.m_UpdateThreshold = obj5.FindProperty("m_GISettings.m_TemporalCoherenceThreshold");
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

        private static bool LightmapParametersGUI(SerializedProperty prop, GUIContent content)
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

        private void MixedLightingGUI()
        {
            this.m_ShowMixedLightsSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowMixedLightsSettings, Styles.MixedLightsLabel, true);
            if (this.m_ShowMixedLightsSettings)
            {
                EditorGUI.indentLevel++;
                LightModeUtil.Get().DrawBakedGIElement();
                if (!LightModeUtil.Get().AreBakedLightmapsEnabled())
                {
                    EditorGUILayout.HelpBox(Styles.BakedGIDisabledInfo.text, MessageType.Info);
                }
                using (new EditorGUI.DisabledScope(!LightModeUtil.Get().AreBakedLightmapsEnabled()))
                {
                    int num;
                    int num2;
                    this.m_LightModeUtil.GetModes(out num, out num2);
                    int mixedMode = EditorGUILayout.IntPopup(Styles.MixedLightMode, num2, Styles.MixedModeStrings, Styles.MixedModeValues, new GUILayoutOption[0]);
                    if ((LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer) && (mixedMode != 0))
                    {
                        EditorGUILayout.HelpBox(Styles.NoShadowMaskInProgressive.text, MessageType.Warning);
                    }
                    if (LightModeUtil.Get().AreBakedLightmapsEnabled())
                    {
                        EditorGUILayout.HelpBox(Styles.HelpStringsMixed[num2].text, MessageType.Info);
                    }
                    if (mixedMode != num2)
                    {
                        this.m_LightModeUtil.Store(num, mixedMode);
                    }
                    if (this.m_LightModeUtil.IsSubtractiveModeEnabled())
                    {
                        EditorGUILayout.PropertyField(this.m_SubtractiveShadowColor, Styles.SubtractiveShadowColor, new GUILayoutOption[0]);
                        this.m_RenderSettingsSO.ApplyModifiedProperties();
                        EditorGUILayout.Space();
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        public void OnDisable()
        {
            SessionState.SetBool("ShowGeneralLightmapSettings", this.m_ShowGeneralLightmapSettings);
            SessionState.SetBool("ShowRealtimeLightsSettings", this.m_ShowRealtimeLightsSettings);
            SessionState.SetBool("ShowMixedLightsSettings", this.m_ShowMixedLightsSettings);
            this.m_LightmapSettingsSO.Dispose();
            this.m_LightmapSettings = null;
            this.m_RenderSettingsSO.Dispose();
        }

        public void OnEnable()
        {
            this.InitSettings();
            this.m_ShowGeneralLightmapSettings = SessionState.GetBool("ShowGeneralLightmapSettings", true);
            this.m_ShowRealtimeLightsSettings = SessionState.GetBool("ShowRealtimeLightsSettings", true);
            this.m_ShowMixedLightsSettings = SessionState.GetBool("ShowMixedLightsSettings", true);
        }

        public void OnGUI()
        {
            if (this.m_LightmapSettings == null)
            {
                this.InitSettings();
            }
            this.m_LightmapSettingsSO.UpdateIfRequiredOrScript();
            this.RealtimeLightingGUI();
            this.MixedLightingGUI();
            this.GeneralLightmapSettingsGUI();
            this.m_LightmapSettingsSO.ApplyModifiedProperties();
        }

        private void RealtimeLightingGUI()
        {
            this.m_ShowRealtimeLightsSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowRealtimeLightsSettings, Styles.RealtimeLightsLabel, true);
            if (this.m_ShowRealtimeLightsSettings)
            {
                int num;
                int num2;
                EditorGUI.indentLevel++;
                this.m_LightModeUtil.GetModes(out num, out num2);
                bool flag = num == 0;
                flag = EditorGUILayout.Toggle(Styles.UseRealtimeGI, flag, new GUILayoutOption[0]);
                if (flag != (num == 0))
                {
                    this.m_LightModeUtil.Store(!flag ? 1 : 0, num2);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        private void Repaint()
        {
            InspectorWindow.RepaintAllInspectors();
        }

        private static class Styles
        {
            public static readonly GUIContent AlbedoBoost;
            public static readonly GUIContent AmbientOcclusion;
            public static readonly GUIContent AmbientOcclusionContribution;
            public static readonly GUIContent AmbientOcclusionContributionDirect;
            public static readonly GUIContent AOMaxDistance;
            public static readonly GUIContent BakeBackend;
            public static readonly GUIContent BakedGIDisabledInfo;
            public static readonly GUIContent BounceScale;
            public static readonly GUIContent[] BouncesStrings;
            public static readonly int[] BouncesValues;
            public static readonly GUIContent ConcurrentJobs;
            public static readonly GUIContent[] ConcurrentJobsTypeStrings;
            public static readonly int[] ConcurrentJobsTypeValues;
            public static readonly GUIContent DefaultLightmapParameters;
            public static readonly GUIContent FinalGather;
            public static readonly GUIContent FinalGatherFiltering;
            public static readonly GUIContent FinalGatherRayCount;
            public static readonly GUIContent ForceUpdates;
            public static readonly GUIContent ForceWhiteAlbedo;
            public static readonly GUIContent GeneralLightmapLabel;
            public static readonly GUIContent[] HelpStringsMixed;
            public static readonly GUIContent IndirectOutputScale;
            public static readonly GUIContent IndirectResolution;
            public static readonly GUIStyle LabelStyle;
            public static readonly GUIContent LightmapDirectionalMode;
            public static readonly GUIContent[] LightmapDirectionalModeStrings;
            public static readonly int[] LightmapDirectionalModeValues;
            public static readonly GUIContent LightmapResolution;
            public static readonly GUIContent LightmapSize;
            public static readonly GUIContent[] LightmapSizeStrings;
            public static readonly int[] LightmapSizeValues;
            public static readonly GUIContent MixedLightMode;
            public static readonly GUIContent MixedLightsLabel;
            public static readonly GUIContent[] MixedModeStrings;
            public static readonly int[] MixedModeValues;
            public static readonly GUIContent NoDirectionalInSM2AndGLES2;
            public static readonly GUIContent NoShadowMaskInProgressive;
            public static readonly GUIContent NoTransparencyAndLODInProgressive;
            public static readonly GUIContent Padding;
            public static readonly GUIContent PVRBounces;
            public static readonly GUIContent PVRCulling;
            public static readonly GUIContent PVRDirectSampleCount;
            public static readonly GUIContent PVRFiltering;
            public static readonly GUIContent PVRFilteringAdvanced;
            public static readonly GUIContent PVRFilteringAtrousColorSigma;
            public static readonly GUIContent PVRFilteringAtrousNormalSigma;
            public static readonly GUIContent PVRFilteringAtrousPositionSigma;
            public static readonly GUIContent PVRFilteringGaussRadiusAO;
            public static readonly GUIContent PVRFilteringGaussRadiusDirect;
            public static readonly GUIContent PVRFilteringGaussRadiusIndirect;
            public static readonly GUIContent PVRFilteringMode;
            public static readonly GUIContent PVRIndirectSampleCount;
            public static readonly GUIContent RealtimeLightsLabel;
            public static readonly GUIContent SubtractiveShadowColor;
            public static readonly GUIContent TextureCompression;
            public static readonly GUIContent UpdateThreshold;
            public static readonly GUIContent UseRealtimeGI;

            static Styles()
            {
                int[] numArray1 = new int[2];
                numArray1[1] = 1;
                LightmapDirectionalModeValues = numArray1;
                LightmapDirectionalModeStrings = new GUIContent[] { new GUIContent("Non-Directional"), new GUIContent("Directional") };
                LightmapSizeValues = new int[] { 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800, 0x1000 };
                LightmapSizeStrings = Array.ConvertAll<int, GUIContent>(LightmapSizeValues, new Converter<int, GUIContent>(LightingWindowBakeSettings.Styles.<LightmapSizeStrings>m__0));
                int[] numArray2 = new int[3];
                numArray2[1] = 1;
                numArray2[2] = 2;
                ConcurrentJobsTypeValues = numArray2;
                ConcurrentJobsTypeStrings = new GUIContent[] { new GUIContent("Min"), new GUIContent("Low"), new GUIContent("High") };
                MixedModeValues = new int[] { 0, 1, 2, 3 };
                MixedModeStrings = new GUIContent[] { EditorGUIUtility.TextContent("Baked Indirect"), EditorGUIUtility.TextContent("Distance Shadowmask"), EditorGUIUtility.TextContent("Shadowmask"), EditorGUIUtility.TextContent("Subtractive") };
                BouncesValues = new int[] { 0, 1, 2, 3, 4 };
                BouncesStrings = new GUIContent[] { EditorGUIUtility.TextContent("None"), EditorGUIUtility.TextContent("1"), EditorGUIUtility.TextContent("2"), EditorGUIUtility.TextContent("3"), EditorGUIUtility.TextContent("4") };
                HelpStringsMixed = new GUIContent[] { EditorGUIUtility.TextContent("Mixed lights provide realtime direct lighting while indirect light is baked into lightmaps and light probes."), EditorGUIUtility.TextContent("Mixed lights provide realtime direct lighting while indirect light is baked into lightmaps and light probes. Shadows are handled with realtime shadow maps up to the shadow distance quality setting."), EditorGUIUtility.TextContent("Mixed lights provide realtime direct lighting while indirect light is baked into lightmaps and light probes. Shadowmasks are used for static objects while dynamic objects are realtime up to the shadow distance quality setting."), EditorGUIUtility.TextContent("Mixed lights provide baked direct and indirect lighting for static objects. Dynamic objects receive realtime direct lighting and cast shadows on static objects using the main directional light in the scene.") };
                BounceScale = EditorGUIUtility.TextContent("Bounce Scale|Multiplier for indirect lighting. Use with care.");
                UpdateThreshold = EditorGUIUtility.TextContent("Update Threshold|Threshold for updating realtime GI. A lower value causes more frequent updates (default 1.0).");
                AlbedoBoost = EditorGUIUtility.TextContent("Albedo Boost|Controls the amount of light bounced between surfaces by intensifying the albedo of materials in the scene. Increasing this draws the albedo value towards white for indirect light computation. The default value is physically accurate.");
                IndirectOutputScale = EditorGUIUtility.TextContent("Indirect Intensity|Controls the brightness of indirect light stored in realtime and baked lightmaps. A value above 1.0 will increase the intensity of indirect light while a value less than 1.0 will reduce indirect light intensity.");
                LightmapDirectionalMode = EditorGUIUtility.TextContent("Directional Mode|Controls whether baked and realtime lightmaps will store directional lighting information from the lighting environment. Options are Directional and Non-Directional.");
                DefaultLightmapParameters = EditorGUIUtility.TextContent("Lightmap Parameters|Allows the adjustment of advanced parameters that affect the process of generating a lightmap for an object using global illumination.");
                RealtimeLightsLabel = EditorGUIUtility.TextContent("Realtime Lighting|Precompute Realtime indirect lighting for realtime lights and static objects. In this mode realtime lights, ambient lighting, materials of static objects (including emission) will generate indirect lighting at runtime. Only static objects are blocking and bouncing light, dynamic objects receive indirect lighting via light probes.");
                MixedLightsLabel = EditorGUIUtility.TextContent("Mixed Lighting|Bake Global Illumination for mixed lights and static objects. May bake both direct and/or indirect lighting based on settings. Only static objects are blocking and bouncing light, dynamic objects receive baked lighting via light probes.");
                GeneralLightmapLabel = EditorGUIUtility.TextContent("Lightmapping Settings|Settings that apply to both Global Illumination modes (Precomputed Realtime and Baked).");
                NoDirectionalInSM2AndGLES2 = EditorGUIUtility.TextContent("Directional lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0. They will fallback to Non-Directional lightmaps.");
                NoTransparencyAndLODInProgressive = EditorGUIUtility.TextContent("Experimental: Transparency and baked LOD is not yet available when baking with the Progressive lightmapping backend. This will be added soon.");
                NoShadowMaskInProgressive = EditorGUIUtility.TextContent("Experimental: Shadow mask and probe occlusion are not yet available when baking with the Progressive lightmapping backend. This will be added soon.");
                ConcurrentJobs = EditorGUIUtility.TextContent("Concurrent Jobs|The amount of simultaneously scheduled jobs.");
                ForceWhiteAlbedo = EditorGUIUtility.TextContent("Force White Albedo|Force white albedo during lighting calculations.");
                ForceUpdates = EditorGUIUtility.TextContent("Force Updates|Force continuous updates of runtime indirect lighting calculations.");
                LabelStyle = EditorStyles.wordWrappedMiniLabel;
                IndirectResolution = EditorGUIUtility.TextContent("Indirect Resolution|Sets the resolution in texels that are used per unit for objects being lit by indirect lighting. The larger the value, the more significant the impact will be on the time it takes to bake the lighting.");
                LightmapResolution = EditorGUIUtility.TextContent("Lightmap Resolution|Sets the resolution in texels that are used per unit for objects being lit by baked global illumination. Larger values will result in increased time to calculate the baked lighting.");
                Padding = EditorGUIUtility.TextContent("Lightmap Padding|Sets the separation in texels between shapes in the baked lightmap.");
                LightmapSize = EditorGUIUtility.TextContent("Lightmap Size|Sets the resolution of the full lightmap Texture in pixels. Values are squared, so a setting of 1024 will produce a 1024x1024 pixel sized lightmap.");
                TextureCompression = EditorGUIUtility.TextContent("Compress Lightmaps|Controls whether the baked lightmap is compressed or not. When enabled, baked lightmaps are compressed to reduce required storage space but some artifacting may be present due to compression.");
                AmbientOcclusion = EditorGUIUtility.TextContent("Ambient Occlusion|Specifies whether to include ambient occlusion or not in the baked lightmap result. Enabling this results in simulating the soft shadows that occur in cracks and crevices of objects when light is reflected onto them.");
                AmbientOcclusionContribution = EditorGUIUtility.TextContent("Indirect Contribution|Adjusts the contrast of ambient occlusion applied to indirect lighting. The larger the value, the more contrast is applied to the ambient occlusion for indirect lighting.");
                AmbientOcclusionContributionDirect = EditorGUIUtility.TextContent("Direct Contribution|Adjusts the contrast of ambient occlusion applied to the direct lighting. The larger the value is, the more contrast is applied to the ambient occlusion for direct lighting. This effect is not physically accurate.");
                AOMaxDistance = EditorGUIUtility.TextContent("Max Distance|Controls how far rays are cast in order to determine if an object is occluded or not. A larger value produces longer rays and contributes more shadows to the lightmap, while a smaller value produces shorter rays that contribute shadows only when objects are very close to one another. A value of 0 casts an infinitely long ray that has no maximum distance.");
                FinalGather = EditorGUIUtility.TextContent("Final Gather|Specifies whether the final light bounce of the global illumination calculation is calculated at the same resolution as the baked lightmap. When enabled, visual quality is improved at the cost of additional time required to bake the lighting.");
                FinalGatherRayCount = EditorGUIUtility.TextContent("Ray Count|Controls the number of rays emitted for every final gather point.");
                FinalGatherFiltering = EditorGUIUtility.TextContent("Denoising|Controls whether a denoising filter is applied to the final gather output.");
                SubtractiveShadowColor = EditorGUIUtility.TextContent("Realtime Shadow Color|The color used for mixing realtime shadows with baked lightmaps in Subtractive lighting mode. The color defines the darkest point of the realtime shadow.");
                MixedLightMode = EditorGUIUtility.TextContent("Lighting Mode|Specifies which Scene lighting mode will be used for all Mixed lights in the Scene. Options are Baked Indirect, Distance Shadowmask, Shadowmask and Subtractive.");
                UseRealtimeGI = EditorGUIUtility.TextContent("Realtime Global Illumination|Controls whether Realtime lights in the Scene contribute indirect light. If enabled, Realtime lights contribute both direct and indirect light. If disabled, Realtime lights only contribute direct light. This can be disabled on a per-light basis in the light component Inspector by setting Indirect Multiplier to 0.");
                BakedGIDisabledInfo = EditorGUIUtility.TextContent("All Baked and Mixed lights in the Scene are currently being overridden to Realtime light modes. Enable Baked Global Illumination to allow the use of Baked and Mixed light modes.");
                BakeBackend = EditorGUIUtility.TextContent("Lightmapper");
                PVRDirectSampleCount = EditorGUIUtility.TextContent("Direct Samples|Controls the number of samples the lightmapper will use for direct lighting calculations. Increasing this value may improve the quality of lightmaps but increases the time required for baking to complete.");
                PVRIndirectSampleCount = EditorGUIUtility.TextContent("Indirect Samples|Controls the number of samples the lightmapper will use for indirect lighting calculations. Increasing this value may improve the quality of lightmaps but increases the time required for baking to complete.");
                PVRBounces = EditorGUIUtility.TextContent("Bounces|Controls the maximum number of bounces the lightmapper will compute for indirect light.");
                PVRFilteringMode = EditorGUIUtility.TextContent("Filtering|Specifies the method used to reduce noise in baked lightmaps. Options are None, Automatic, or Advanced.");
                PVRFiltering = EditorGUIUtility.TextContent("Filtering|Choose which filter kernel to apply to the lightmap.");
                PVRFilteringAdvanced = EditorGUIUtility.TextContent("Advanced Filter Settings|Show advanced settings to configure filtering on lightmaps.");
                PVRFilteringGaussRadiusDirect = EditorGUIUtility.TextContent("Direct Radius|Controls the radius of the filter for direct light stored in the lightmap. A higher value will increase the strength of the blur, reducing noise from direct light in the lightmap.");
                PVRFilteringGaussRadiusIndirect = EditorGUIUtility.TextContent("Indirect Radius|Controls the radius of the filter for indirect light stored in the lightmap. A higher value will increase the strength of the blur, reducing noise from indirect light in the lightmap.");
                PVRFilteringGaussRadiusAO = EditorGUIUtility.TextContent("Ambient Occlusion Radius|The radius of the filter for ambient occlusion in the lightmap. A higher radius will increase the blur strength, reducing sampling noise from ambient occlusion in the lightmap.");
                PVRFilteringAtrousColorSigma = EditorGUIUtility.TextContent("Color Sigma|How to weigh the color channel in the filter edge stopping condition.");
                PVRFilteringAtrousNormalSigma = EditorGUIUtility.TextContent("Normal Sigma|How to weigh the normal channel in the filter edge stopping condition.");
                PVRFilteringAtrousPositionSigma = EditorGUIUtility.TextContent("Position Sigma|How to weigh the position channel in the filter edge stopping condition.");
                PVRCulling = EditorGUIUtility.TextContent("Prioritize View|Specifies whether the lightmapper should prioritize baking texels within the scene view. When disabled, objects outside the scene view will have the same priority as those in the scene view.");
            }

            [CompilerGenerated]
            private static GUIContent <LightmapSizeStrings>m__0(int x) => 
                new GUIContent(x.ToString());
        }
    }
}

