namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngineInternal;

    internal class LightingWindowBakeSettings
    {
        private const string kShowDynamicLightsSettingsKey = "ShowDynamicLightsSettings";
        private const string kShowGeneralGISettingsKey = "ShowGeneralGISettings";
        private const string kShowStationaryLightsSettingsKey = "ShowStationaryLightsSettings";
        private const string kShowUISettings = "ShowUISettings";
        private SerializedProperty m_AlbedoBoost;
        private SerializedProperty m_AO;
        private SerializedProperty m_AOMaxDistance;
        private SerializedProperty m_BakeResolution;
        private SerializedProperty m_BounceScale;
        private SerializedProperty m_CompAOExponent;
        private SerializedProperty m_CompAOExponentDirect;
        private SerializedProperty m_EnvironmentLightingMode;
        private SerializedProperty m_FinalGather;
        private SerializedProperty m_FinalGatherFiltering;
        private SerializedProperty m_FinalGatherRayCount;
        private SerializedProperty m_IndirectOutputScale;
        private SerializedProperty m_LightmapDirectionalMode;
        private SerializedProperty m_LightmapParameters;
        private Object m_LightmapSettings;
        private SerializedObject m_LightmapSettingsSO;
        private SerializedProperty m_LightmapSize;
        private LightModeUtil m_LightModeUtil = LightModeUtil.Get();
        private readonly LightProbeVisualisationGUI m_LightProbeVisualisationGUI = new LightProbeVisualisationGUI();
        private SerializedProperty m_Padding;
        private SerializedObject m_RenderSettingsSO;
        private SerializedProperty m_Resolution;
        private SerializedProperty m_RuntimeCPUUsage;
        private bool m_ShowDevOptions = false;
        private AnimBool m_ShowDisabledBakedGIInfo = new AnimBool();
        private bool m_ShowDynamicLightsSettings = true;
        private bool m_ShowGeneralGISettings = true;
        private AnimBool m_ShowStationaryInfo = new AnimBool();
        private bool m_ShowStationaryLightsSettings = true;
        private bool m_ShowUISettings = false;
        private SerializedProperty m_SubtractiveShadowColor;
        private SerializedProperty m_TextureCompression;
        private SerializedProperty m_UpdateThreshold;

        private void DeveloperBuildSettingsGUI()
        {
            if (Unsupported.IsDeveloperBuild())
            {
                this.m_ShowDevOptions = EditorGUILayout.FoldoutTitlebar(this.m_ShowDevOptions, GUIContent.Temp("Debug [internal]"), true);
                if (this.m_ShowDevOptions)
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    Lightmapping.concurrentJobsType = (Lightmapping.ConcurrentJobsType) EditorGUILayout.IntPopup(Styles.ConcurrentJobs, (int) Lightmapping.concurrentJobsType, Styles.ConcurrentJobsTypeStrings, Styles.ConcurrentJobsTypeValues, new GUILayoutOption[0]);
                    Lightmapping.enlightenForceUpdates = EditorGUILayout.Toggle(Styles.ForceUpdates, Lightmapping.enlightenForceUpdates, new GUILayoutOption[0]);
                    Lightmapping.enlightenForceWhiteAlbedo = EditorGUILayout.Toggle(Styles.ForceWhiteAlbedo, Lightmapping.enlightenForceWhiteAlbedo, new GUILayoutOption[0]);
                    Lightmapping.filterMode = (FilterMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), Lightmapping.filterMode, new GUILayoutOption[0]);
                    EditorGUILayout.Slider(this.m_BounceScale, 0f, 10f, Styles.BounceScale, new GUILayoutOption[0]);
                    EditorGUILayout.Slider(this.m_UpdateThreshold, 0f, 1f, Styles.UpdateThreshold, new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
                    if (GUILayout.Button("Clear disk cache", options))
                    {
                        Lightmapping.Clear();
                        Lightmapping.ClearDiskCache();
                    }
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(120f) };
                    if (GUILayout.Button("Print state to console", optionArray2))
                    {
                        Lightmapping.PrintStateToConsole();
                    }
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(120f) };
                    if (GUILayout.Button("Reset albedo/emissive", optionArray3))
                    {
                        GIDebugVisualisation.ResetRuntimeInputTextures();
                    }
                    GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(120f) };
                    if (GUILayout.Button("Reset environment", optionArray4))
                    {
                        DynamicGI.UpdateEnvironment();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
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

        private void DynamicLightingGUI()
        {
            this.m_ShowDynamicLightsSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowDynamicLightsSettings, Styles.DynamicLightsLabel, true);
            if (this.m_ShowDynamicLightsSettings)
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
                using (new EditorGUI.DisabledScope(!flag))
                {
                    EditorGUILayout.IntPopup(this.m_RuntimeCPUUsage, Styles.RuntimeCPUUsageStrings, Styles.RuntimeCPUUsageValues, Styles.RuntimeCPUUsage, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        private void GeneralGISettingsGUI()
        {
            this.m_ShowGeneralGISettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowGeneralGISettings, Styles.GeneralGILabel, true);
            if (this.m_ShowGeneralGISettings)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!LightModeUtil.Get().IsAnyGIEnabled()))
                {
                    DrawResolutionField(this.m_Resolution, Styles.IndirectResolution);
                    if (LightModeUtil.Get().IsAnyGIEnabled())
                    {
                        int num;
                        bool ambientMode = LightModeUtil.Get().GetAmbientMode(out num);
                        using (new EditorGUI.DisabledScope(!ambientMode))
                        {
                            int[] numArray1 = new int[2];
                            numArray1[1] = 1;
                            int[] optionValues = numArray1;
                            if (ambientMode)
                            {
                                this.m_EnvironmentLightingMode.intValue = EditorGUILayout.IntPopup(Styles.AmbientMode, this.m_EnvironmentLightingMode.intValue, Styles.AmbientModes, optionValues, new GUILayoutOption[0]);
                            }
                            else
                            {
                                EditorGUILayout.IntPopup(Styles.AmbientMode, num, Styles.AmbientModes, optionValues, new GUILayoutOption[0]);
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
                    LightingWindowObjectTab.LightmapParametersGUI(this.m_LightmapParameters, Styles.DefaultLightmapParameters, true);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
            }
        }

        private void InitSettings()
        {
            Object renderSettings = RenderSettings.GetRenderSettings();
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
            this.m_AO = obj5.FindProperty("m_LightmapEditorSettings.m_AO");
            this.m_AOMaxDistance = obj5.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance");
            this.m_CompAOExponent = obj5.FindProperty("m_LightmapEditorSettings.m_CompAOExponent");
            this.m_CompAOExponentDirect = obj5.FindProperty("m_LightmapEditorSettings.m_CompAOExponentDirect");
            this.m_TextureCompression = obj5.FindProperty("m_LightmapEditorSettings.m_TextureCompression");
            this.m_FinalGather = obj5.FindProperty("m_LightmapEditorSettings.m_FinalGather");
            this.m_FinalGatherRayCount = obj5.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount");
            this.m_FinalGatherFiltering = obj5.FindProperty("m_LightmapEditorSettings.m_FinalGatherFiltering");
            this.m_LightmapSize = obj5.FindProperty("m_LightmapEditorSettings.m_TextureWidth");
            this.m_EnvironmentLightingMode = obj5.FindProperty("m_GISettings.m_EnvironmentLightingMode");
            this.m_RuntimeCPUUsage = obj5.FindProperty("m_RuntimeCPUUsage");
            this.m_BounceScale = obj5.FindProperty("m_GISettings.m_BounceScale");
            this.m_UpdateThreshold = obj5.FindProperty("m_GISettings.m_TemporalCoherenceThreshold");
        }

        public void OnDisable()
        {
            SessionState.SetBool("ShowGeneralGISettings", this.m_ShowGeneralGISettings);
            SessionState.SetBool("ShowDynamicLightsSettings", this.m_ShowDynamicLightsSettings);
            SessionState.SetBool("ShowStationaryLightsSettings", this.m_ShowStationaryLightsSettings);
            SessionState.SetBool("ShowUISettings", this.m_ShowUISettings);
            this.m_LightmapSettingsSO.Dispose();
            this.m_LightmapSettings = null;
            this.m_RenderSettingsSO.Dispose();
            this.m_ShowDisabledBakedGIInfo.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowStationaryInfo.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public void OnEnable()
        {
            this.InitSettings();
            this.m_ShowGeneralGISettings = SessionState.GetBool("ShowGeneralGISettings", true);
            this.m_ShowDynamicLightsSettings = SessionState.GetBool("ShowDynamicLightsSettings", true);
            this.m_ShowStationaryLightsSettings = SessionState.GetBool("ShowStationaryLightsSettings", true);
            this.m_ShowUISettings = SessionState.GetBool("ShowUISettings", false);
            this.m_ShowDisabledBakedGIInfo = new AnimBool(!LightModeUtil.Get().AreBakedLightmapsEnabled());
            this.m_ShowDisabledBakedGIInfo.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowStationaryInfo = new AnimBool(LightModeUtil.Get().AreBakedLightmapsEnabled());
            this.m_ShowStationaryInfo.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public void OnGUI()
        {
            if (this.m_LightmapSettings == null)
            {
                this.InitSettings();
            }
            this.m_LightmapSettingsSO.UpdateIfRequiredOrScript();
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200f;
            this.DynamicLightingGUI();
            this.StationaryLightingGUI();
            this.GeneralGISettingsGUI();
            this.m_LightProbeVisualisationGUI.OnGUI();
            this.UISettingsGUI();
            this.DeveloperBuildSettingsGUI();
            EditorGUIUtility.labelWidth = labelWidth;
            this.m_LightmapSettingsSO.ApplyModifiedProperties();
        }

        private void Repaint()
        {
            InspectorWindow.RepaintAllInspectors();
        }

        private void StationaryLightingGUI()
        {
            this.m_ShowDisabledBakedGIInfo.target = !LightModeUtil.Get().AreBakedLightmapsEnabled();
            this.m_ShowStationaryInfo.target = LightModeUtil.Get().AreBakedLightmapsEnabled();
            this.m_ShowStationaryLightsSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowStationaryLightsSettings, Styles.StationaryLightsLabel, true);
            if (this.m_ShowStationaryLightsSettings)
            {
                EditorGUI.indentLevel++;
                LightModeUtil.Get().DrawBakedGIElement();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowDisabledBakedGIInfo.faded))
                {
                    EditorGUILayout.HelpBox(Styles.BakedGIDisabledInfo.text, MessageType.Info);
                }
                EditorGUILayout.EndFadeGroup();
                using (new EditorGUI.DisabledScope(!LightModeUtil.Get().AreBakedLightmapsEnabled()))
                {
                    int num;
                    int num2;
                    this.m_LightModeUtil.GetModes(out num, out num2);
                    int stationaryMode = EditorGUILayout.IntPopup(Styles.StationaryLightMode, num2, Styles.StationaryModeStrings, Styles.StationaryModeValues, new GUILayoutOption[0]);
                    if (EditorGUILayout.BeginFadeGroup(this.m_ShowStationaryInfo.faded))
                    {
                        EditorGUILayout.HelpBox(Styles.HelpStringsStationary[num2].text, MessageType.Info);
                    }
                    EditorGUILayout.EndFadeGroup();
                    if (stationaryMode != num2)
                    {
                        this.m_LightModeUtil.Store(num, stationaryMode);
                    }
                    if (this.m_LightModeUtil.IsSubtractiveModeEnabled())
                    {
                        EditorGUILayout.PropertyField(this.m_SubtractiveShadowColor, Styles.SubtractiveShadowColor, new GUILayoutOption[0]);
                        this.m_RenderSettingsSO.ApplyModifiedProperties();
                        EditorGUILayout.Space();
                    }
                    DrawResolutionField(this.m_BakeResolution, Styles.LightmapResolution);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_Padding, Styles.Padding, new GUILayoutOption[0]);
                    GUILayout.Label(" texels", Styles.LabelStyle, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.IntPopup(this.m_LightmapSize, Styles.LightmapSizeStrings, Styles.LightmapSizeValues, Styles.LightmapSize, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_TextureCompression, Styles.TextureCompression, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_AO, Styles.AO, new GUILayoutOption[0]);
                    if (this.m_AO.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(this.m_AOMaxDistance, Styles.AOMaxDistance, new GUILayoutOption[0]);
                        if (this.m_AOMaxDistance.floatValue < 0f)
                        {
                            this.m_AOMaxDistance.floatValue = 0f;
                        }
                        EditorGUILayout.Slider(this.m_CompAOExponent, 0f, 10f, Styles.AmbientOcclusion, new GUILayoutOption[0]);
                        EditorGUILayout.Slider(this.m_CompAOExponentDirect, 0f, 10f, Styles.AmbientOcclusionDirect, new GUILayoutOption[0]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(this.m_FinalGather, Styles.FinalGather, new GUILayoutOption[0]);
                    if (this.m_FinalGather.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(this.m_FinalGatherRayCount, Styles.FinalGatherRayCount, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_FinalGatherFiltering, Styles.FinalGatherFiltering, new GUILayoutOption[0]);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        private void UISettingsGUI()
        {
            this.m_ShowUISettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowUISettings, Styles.UISettings, true);
            if (this.m_ShowUISettings)
            {
                EditorGUI.indentLevel++;
                LightModeUtil.Get().DrawUIFlags();
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        private class Styles
        {
            public static readonly GUIContent AlbedoBoost;
            public static readonly GUIContent AmbientMode;
            public static readonly GUIContent[] AmbientModes;
            public static readonly GUIContent AmbientOcclusion;
            public static readonly GUIContent AmbientOcclusionDirect;
            public static readonly GUIContent AO;
            public static readonly GUIContent AOMaxDistance;
            public static readonly GUIContent BakedGIDisabledInfo;
            public static readonly GUIContent BounceScale;
            public static readonly GUIContent ConcurrentJobs;
            public static readonly GUIContent[] ConcurrentJobsTypeStrings;
            public static readonly int[] ConcurrentJobsTypeValues;
            public static readonly GUIContent DefaultLightmapParameters;
            public static readonly GUIContent DynamicLightsLabel;
            public static readonly GUIContent FinalGather;
            public static readonly GUIContent FinalGatherFiltering;
            public static readonly GUIContent FinalGatherRayCount;
            public static readonly GUIContent ForceUpdates;
            public static readonly GUIContent ForceWhiteAlbedo;
            public static readonly GUIContent GeneralGILabel;
            public static readonly GUIContent[] HelpStringsStationary;
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
            public static readonly GUIContent NoDirectionalInSM2AndGLES2;
            public static readonly GUIContent Padding;
            public static readonly GUIContent RuntimeCPUUsage;
            public static readonly GUIContent[] RuntimeCPUUsageStrings = new GUIContent[] { EditorGUIUtility.TextContent("Low (default)"), EditorGUIUtility.TextContent("Medium"), EditorGUIUtility.TextContent("High"), EditorGUIUtility.TextContent("Unlimited") };
            public static readonly int[] RuntimeCPUUsageValues = new int[] { 0x19, 50, 0x4b, 100 };
            public static readonly GUIContent StationaryLightMode;
            public static readonly GUIContent StationaryLightsLabel;
            public static readonly GUIContent[] StationaryModeStrings;
            public static readonly int[] StationaryModeValues;
            public static readonly GUIContent SubtractiveShadowColor;
            public static readonly GUIContent TextureCompression;
            public static readonly GUIContent UISettings;
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
                StationaryModeValues = new int[] { 0, 1, 2, 3 };
                StationaryModeStrings = new GUIContent[] { EditorGUIUtility.TextContent("Baked Indirect"), EditorGUIUtility.TextContent("Distance Shadowmask"), EditorGUIUtility.TextContent("Shadowmask"), EditorGUIUtility.TextContent("Subtractive") };
                HelpStringsStationary = new GUIContent[] { EditorGUIUtility.TextContent("Stationary lights contribute realtime direct light and use lightmaps and Light Probes for indirect light. Realtime shadows are used up to the Shadow Distance quality setting, beyond which static objects use shadow masks and dynamic objects use Light Probes."), EditorGUIUtility.TextContent("Stationary lights contribute realtime direct light and use lightmaps and Light Probes for indirect light. Static objects cast and receive shadows using shadowmasks, and receive realtime shadows. Dynamic objects cast and receive realtime shadows and shadows from Light Probes."), EditorGUIUtility.TextContent("Stationary lights contribute direct light in realtime while indirect light is baked into lightmaps and Light Probes. Shadows are handled with realtime shadows."), EditorGUIUtility.TextContent("Stationary lights contribute indirect light through lightmaps and Light Probes. Direct light is handled with lightmaps for static objects and in realtime for dynamic objects. Realtime and Light Probe shadows are used for dynamic objects while static objects receive approximate shadows cast by the main light.") };
                AmbientModes = new GUIContent[] { EditorGUIUtility.TextContent("Realtime"), EditorGUIUtility.TextContent("Baked") };
                BounceScale = EditorGUIUtility.TextContent("Bounce Scale|Multiplier for indirect lighting. Use with care.");
                UpdateThreshold = EditorGUIUtility.TextContent("Update Threshold|Threshold for updating realtime GI. A lower value causes more frequent updates (default 1.0).");
                AlbedoBoost = EditorGUIUtility.TextContent("Bounce Boost|Controls the amount of light bounced from surfaces onto other surfaces.  A value above 1.0 will increase the amount of light bounced.");
                IndirectOutputScale = EditorGUIUtility.TextContent("Indirect Intensity|Controls the brightness of indirect light stored in realtime and baked lightmaps.  A value above 1.0 will increase the intensity of indirect light while a value less than 1.0 will reduce indirect light intensity.");
                LightmapDirectionalMode = EditorGUIUtility.TextContent("Directional Mode|Controls whether baked and realtime lightmaps will store directional lighting information from the lighting environment.  Options are Directional, Non-Directional, and Directional Specular.");
                DefaultLightmapParameters = EditorGUIUtility.TextContent("Lightmap Parameters|Allows the adjustment of advanced parameters that affect the process of generating a lightmap for an object using global illumination.");
                RuntimeCPUUsage = EditorGUIUtility.TextContent("CPU Usage|How much CPU usage to assign to the final lighting calculations at runtime. Increasing this makes the system react faster to changes in lighting at a cost of using more CPU time.");
                DynamicLightsLabel = EditorGUIUtility.TextContent("Dynamic Lighting|Precompute Realtime indirect lighting for dynamic lights and static objects. In this mode dynamics lights, ambient lighting, materials of static objects (including emission) will generate indirect lighting at runtime. Only static objects are blocking and bouncing light, dynamic objects receive indirect lighting via light probes.");
                StationaryLightsLabel = EditorGUIUtility.TextContent("Stationary Lighting|Bake Global Illumination for stationary lights and static objects. May bake both direct and/or indirect lighting based on settings. Only static objects are blocking and bouncing light, dynamic objects receive baked lighting via light probes.");
                GeneralGILabel = EditorGUIUtility.TextContent("Global Illumination|Settings that apply to both Global Illumination modes (Precomputed Realtime and Baked).");
                NoDirectionalInSM2AndGLES2 = EditorGUIUtility.TextContent("Directional lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0. They will fallback to Non-Directional lightmaps.");
                ConcurrentJobs = EditorGUIUtility.TextContent("Concurrent Jobs|The amount of simultaneously scheduled jobs.");
                ForceWhiteAlbedo = EditorGUIUtility.TextContent("Force White Albedo|Force white albedo during lighting calculations.");
                ForceUpdates = EditorGUIUtility.TextContent("Force Updates|Force continuous updates of runtime indirect lighting calculations.");
                LabelStyle = EditorStyles.wordWrappedMiniLabel;
                IndirectResolution = EditorGUIUtility.TextContent("Indirect Resolution|Sets the resolution in texels that are used per unit for objects being lit by indirect lighting.  The larger the value, the more significant the impact will be on the time it takes to bake the lighting.");
                LightmapResolution = EditorGUIUtility.TextContent("Lightmap Resolution|Sets the resolution in texels that are used per unit for objects being lit by baked global illumination. Larger values will result in increased time to calculate the baked lighting.");
                Padding = EditorGUIUtility.TextContent("Lightmap Padding|Sets the separation in texels between shapes in the baked lightmap.");
                LightmapSize = EditorGUIUtility.TextContent("Lightmap Size|Sets the resolution of the full lightmap Texture in pixels. Values are squared, so a setting of 1024 will produce a 1024x1024 pixel sized lightmap.");
                TextureCompression = EditorGUIUtility.TextContent("Compress Lightmaps|Controls whether the baked lightmap is compressed or not. When enabled, baked lightmaps are compressed to reduce required storage space but some artifacting may be present due to compression.");
                AO = EditorGUIUtility.TextContent("Ambient Occlusion|Specifies whether to include ambient occlusion or not in the baked lightmap result. Enabling this results in simulating the soft shadows that occur in cracks and crevices of objects when indirect light is reflected onto them.");
                AmbientOcclusion = EditorGUIUtility.TextContent("Indirect Contrast|Adjusts the contrast of ambient occlusion applied to indirect lighting. The larger the value, the more contrast is applied to the ambient occlusion for indirect lighting. This effect is physically accurate.");
                AmbientOcclusionDirect = EditorGUIUtility.TextContent("Direct Contribution|Adjusts the contrast of ambient occlusion applied to the direct lighting. The larger the value is, the more contrast is applied to the ambient occlusion for direct lighting. This effect is not physically accurate.");
                AOMaxDistance = EditorGUIUtility.TextContent("Max Distance|Controls how far rays are cast in order to determine if an object is occluded or not. A larger value produces longer rays and contributes more shadows to the lightmap, while a smaller value produces shorter rays that contribute shadows only when objects are very close to one another. A value of 0 casts an infinitely long ray that has no maximum distance.");
                FinalGather = EditorGUIUtility.TextContent("Final Gather|Specifies whether the final light bounce of the global illumination calculation is calculated at the same resolution as the baked lightmap. When enabled, visual quality is improved at the cost of additional time required to bake the lighting.");
                FinalGatherRayCount = EditorGUIUtility.TextContent("Ray Count|Controls the number of rays emitted for every final gather point.");
                FinalGatherFiltering = EditorGUIUtility.TextContent("Denoising|Controls whether a denoising filter is applied to the final gather output.");
                SubtractiveShadowColor = EditorGUIUtility.TextContent("Realtime Shadow Color|The color used for mixing realtime shadows with baked lightmaps in Subtractive lighting mode. The color defines the darkest point of the realtime shadow.");
                StationaryLightMode = EditorGUIUtility.TextContent("Stationary Lighting Mode|Specifies which Scene lighting mode will be used for all Stationary lights in the Scene. Options are Baked Indirect, Distance Shadowmask, Shadowmask and Subtractive.");
                UISettings = EditorGUIUtility.TextContent("UI settings");
                AmbientMode = EditorGUIUtility.TextContent("Ambient Mode|Specifies the global illumination mode that should be used for handling ambient light in the scene. Options are Realtime or Baked. This property is not needed unless both Realtime Global Illumination and Baked Global Illumination are enabled for the scene.");
                UseRealtimeGI = EditorGUIUtility.TextContent("Use Realtime Global Illumination|Controls whether dynamic lights in the Scene contribute indirect light. If enabled, Dynamic lights contribute indirect light. If disabled, Dynamic lights only contribute direct light. This can be disabled on a per-light basis in the light component Inspector by setting Indirect Multiplier to 0.");
                BakedGIDisabledInfo = EditorGUIUtility.TextContent("All Static and Stationary lights in the Scene are currently being overridden to Dynamic light modes. Enable Baked Global Illumination to allow the use of Static and Stationary light modes.");
            }

            [CompilerGenerated]
            private static GUIContent <LightmapSizeStrings>m__0(int x) => 
                new GUIContent(x.ToString());
        }
    }
}

