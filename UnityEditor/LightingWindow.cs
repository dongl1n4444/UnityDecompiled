namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngineInternal;

    [EditorWindowTitle(title="Lighting", icon="Lighting")]
    internal class LightingWindow : EditorWindow
    {
        private float kButtonWidth;
        private GUIContent[] kConcurrentJobsTypeStrings = new GUIContent[] { new GUIContent("Min"), new GUIContent("Low"), new GUIContent("High") };
        private int[] kConcurrentJobsTypeValues;
        private const string kGlobalIlluminationUnityManualPage = "file:///unity/Manual/GlobalIllumination.html";
        private GUIContent[] kMaxAtlasSizeStrings;
        private int[] kMaxAtlasSizeValues;
        private GUIContent[] kModeStrings;
        private int[] kModeValues;
        private const string kShowBakeSettingsKey = "ShowBakedLightingSettings";
        private const string kShowGeneralSettingsKey = "ShowGeneralLightingSettings";
        private const string kShowRealtimeSettingsKey = "ShowRealtimeLightingSettings";
        private Editor m_FogEditor;
        private Editor m_LightingEditor;
        private LightingWindowLightmapPreviewTab m_LightmapPreviewTab;
        private Mode m_Mode;
        private LightingWindowObjectTab m_ObjectTab;
        private Editor m_OtherRenderingEditor;
        private PreviewResizer m_PreviewResizer;
        private Vector2 m_ScrollPosition;
        private bool m_ShowBakeSettings;
        private bool m_ShowDevOptions;
        private bool m_ShowGeneralSettings;
        private AnimBool m_ShowIndirectResolution;
        private bool m_ShowRealtimeSettings;
        private float m_ToolbarPadding;
        private static string[] s_BakeModeOptions = new string[] { "Bake Reflection Probes", "Clear Baked Data" };
        private static bool s_IsVisible = false;
        private static Styles s_Styles;

        public LightingWindow()
        {
            int[] numArray1 = new int[3];
            numArray1[1] = 1;
            numArray1[2] = 2;
            this.kConcurrentJobsTypeValues = numArray1;
            this.kButtonWidth = 120f;
            this.kModeStrings = new GUIContent[] { new GUIContent("Non-Directional"), new GUIContent("Directional"), new GUIContent("Directional Specular (deprecated)") };
            int[] numArray2 = new int[3];
            numArray2[1] = 1;
            numArray2[2] = 2;
            this.kModeValues = numArray2;
            this.kMaxAtlasSizeStrings = new GUIContent[] { new GUIContent("32"), new GUIContent("64"), new GUIContent("128"), new GUIContent("256"), new GUIContent("512"), new GUIContent("1024"), new GUIContent("2048"), new GUIContent("4096") };
            this.kMaxAtlasSizeValues = new int[] { 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800, 0x1000 };
            this.m_Mode = Mode.BakeSettings;
            this.m_ScrollPosition = Vector2.zero;
            this.m_ShowIndirectResolution = new AnimBool();
            this.m_ShowDevOptions = false;
            this.m_PreviewResizer = new PreviewResizer();
            this.m_ToolbarPadding = -1f;
        }

        private void BakedGUI(SerializedObject so, bool enableRealtimeGI, SerializedProperty enableBakedGI)
        {
            this.m_ShowBakeSettings = EditorGUILayout.ToggleTitlebar(this.m_ShowBakeSettings, styles.BakedGILabel, enableBakedGI);
            if (this.m_ShowBakeSettings)
            {
                SerializedProperty resolution = so.FindProperty("m_LightmapEditorSettings.m_Resolution");
                SerializedProperty property2 = so.FindProperty("m_LightmapEditorSettings.m_BakeResolution");
                SerializedProperty property = so.FindProperty("m_LightmapEditorSettings.m_Padding");
                SerializedProperty property4 = so.FindProperty("m_LightmapEditorSettings.m_AO");
                SerializedProperty property5 = so.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance");
                SerializedProperty property6 = so.FindProperty("m_LightmapEditorSettings.m_CompAOExponent");
                SerializedProperty property7 = so.FindProperty("m_LightmapEditorSettings.m_CompAOExponentDirect");
                SerializedProperty property8 = so.FindProperty("m_LightmapEditorSettings.m_TextureCompression");
                SerializedProperty property9 = so.FindProperty("m_LightmapEditorSettings.m_DirectLightInLightProbes");
                SerializedProperty property10 = so.FindProperty("m_LightmapEditorSettings.m_FinalGather");
                SerializedProperty property11 = so.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount");
                SerializedProperty property12 = so.FindProperty("m_LightmapEditorSettings.m_FinalGatherFiltering");
                SerializedProperty property13 = so.FindProperty("m_LightmapEditorSettings.m_TextureWidth");
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!enableBakedGI.boolValue))
                {
                    DrawLightmapResolutionField(property2, styles.BakeResolution);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(property, styles.Padding, new GUILayoutOption[0]);
                    GUILayout.Label(" texels", styles.labelStyle, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(property8, s_Styles.TextureCompression, new GUILayoutOption[0]);
                    this.m_ShowIndirectResolution.target = !enableRealtimeGI;
                    if (EditorGUILayout.BeginFadeGroup(this.m_ShowIndirectResolution.faded))
                    {
                        EditorGUILayout.Space();
                        DrawLightmapResolutionField(resolution, styles.IndirectResolution);
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.EndFadeGroup();
                    EditorGUILayout.PropertyField(property4, s_Styles.AO, new GUILayoutOption[0]);
                    if (property4.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(property5, styles.AOMaxDistance, new GUILayoutOption[0]);
                        if (property5.floatValue < 0f)
                        {
                            property5.floatValue = 0f;
                        }
                        EditorGUILayout.Slider(property6, 0f, 10f, styles.AmbientOcclusion, new GUILayoutOption[0]);
                        EditorGUILayout.Slider(property7, 0f, 10f, styles.AmbientOcclusionDirect, new GUILayoutOption[0]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(property10, s_Styles.FinalGather, new GUILayoutOption[0]);
                    if (property10.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(property11, styles.FinalGatherRayCount, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(property12, styles.FinalGatherFiltering, new GUILayoutOption[0]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.IntPopup(property13, this.kMaxAtlasSizeStrings, this.kMaxAtlasSizeValues, styles.MaxAtlasSize, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(styles.LightProbesTitle, new GUILayoutOption[0]);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property9, s_Styles.DirectLightInLightProbes, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                EditorGUI.indentLevel--;
            }
        }

        private void BakeDropDownCallback(object data)
        {
            switch (((BakeMode) data))
            {
                case BakeMode.Clear:
                    this.DoClear();
                    break;

                case BakeMode.BakeReflectionProbes:
                    this.DoBakeReflectionProbes();
                    break;
            }
        }

        private void Buttons()
        {
            bool flag = Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.Iterative;
            if (flag)
            {
                EditorGUILayout.HelpBox("Baking of lightmaps is automatic because the workflow mode is set to 'Auto'. The lightmap data is stored in the GI cache.", MessageType.Info);
            }
            if ((Lightmapping.lightingDataAsset != null) && !Lightmapping.lightingDataAsset.isValid)
            {
                EditorGUILayout.HelpBox(Lightmapping.lightingDataAsset.validityErrorMessage, MessageType.Error);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            flag = GUILayout.Toggle(flag, styles.ContinuousBakeLabel, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Lightmapping.giWorkflowMode = !flag ? Lightmapping.GIWorkflowMode.OnDemand : Lightmapping.GIWorkflowMode.Iterative;
                InspectorWindow.RepaintAllInspectors();
            }
            using (new EditorGUI.DisabledScope(flag))
            {
                if (flag || !Lightmapping.isRunning)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(180f) };
                    if (EditorGUI.ButtonWithDropdownList(styles.BuildLabel, s_BakeModeOptions, new GenericMenu.MenuFunction2(this.BakeDropDownCallback), options))
                    {
                        this.DoBake();
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(this.kButtonWidth) };
                    if (GUILayout.Button("Cancel", optionArray2))
                    {
                        Lightmapping.Cancel();
                        UsabilityAnalytics.Track("/LightMapper/Cancel");
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ClearCachedProperties()
        {
            Object.DestroyImmediate(this.m_LightingEditor);
            this.m_LightingEditor = null;
            Object.DestroyImmediate(this.m_FogEditor);
            this.m_FogEditor = null;
            Object.DestroyImmediate(this.m_OtherRenderingEditor);
            this.m_OtherRenderingEditor = null;
        }

        [MenuItem("Window/Lighting", false, 0x832)]
        private static void CreateLightingWindow()
        {
            LightingWindow window = EditorWindow.GetWindow<LightingWindow>();
            window.minSize = new Vector2(300f, 360f);
            window.Show();
        }

        private void DeveloperBuildEnlightenSettings(SerializedObject so)
        {
            if (Unsupported.IsDeveloperBuild())
            {
                this.m_ShowDevOptions = EditorGUILayout.Foldout(this.m_ShowDevOptions, "Debug [internal]", true);
                if (this.m_ShowDevOptions)
                {
                    SerializedProperty property = so.FindProperty("m_GISettings.m_BounceScale");
                    SerializedProperty property2 = so.FindProperty("m_GISettings.m_TemporalCoherenceThreshold");
                    EditorGUI.indentLevel++;
                    Lightmapping.concurrentJobsType = (Lightmapping.ConcurrentJobsType) EditorGUILayout.IntPopup(styles.ConcurrentJobs, (int) Lightmapping.concurrentJobsType, this.kConcurrentJobsTypeStrings, this.kConcurrentJobsTypeValues, new GUILayoutOption[0]);
                    Lightmapping.enlightenForceUpdates = EditorGUILayout.Toggle(styles.ForceUpdates, Lightmapping.enlightenForceUpdates, new GUILayoutOption[0]);
                    Lightmapping.enlightenForceWhiteAlbedo = EditorGUILayout.Toggle(styles.ForceWhiteAlbedo, Lightmapping.enlightenForceWhiteAlbedo, new GUILayoutOption[0]);
                    Lightmapping.filterMode = (FilterMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), Lightmapping.filterMode, new GUILayoutOption[0]);
                    EditorGUILayout.Slider(property, 0f, 10f, styles.BounceScale, new GUILayoutOption[0]);
                    EditorGUILayout.Slider(property2, 0f, 1f, styles.UpdateThreshold, new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this.kButtonWidth) };
                    if (GUILayout.Button("Clear disk cache", options))
                    {
                        Lightmapping.Clear();
                        Lightmapping.ClearDiskCache();
                    }
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(this.kButtonWidth) };
                    if (GUILayout.Button("Print state to console", optionArray2))
                    {
                        Lightmapping.PrintStateToConsole();
                    }
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(this.kButtonWidth) };
                    if (GUILayout.Button("Reset albedo/emissive", optionArray3))
                    {
                        GIDebugVisualisation.ResetRuntimeInputTextures();
                    }
                    GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(this.kButtonWidth) };
                    if (GUILayout.Button("Reset environment", optionArray4))
                    {
                        DynamicGI.UpdateEnvironment();
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DoBake()
        {
            UsabilityAnalytics.Track("/LightMapper/Start");
            UsabilityAnalytics.Event("LightMapper", "Mode", LightmapSettings.lightmapsMode.ToString(), 1);
            UsabilityAnalytics.Event("LightMapper", "Button", "BakeScene", 1);
            Lightmapping.BakeAsync();
        }

        private void DoBakeReflectionProbes()
        {
            Lightmapping.BakeAllReflectionProbesSnapshots();
            UsabilityAnalytics.Track("/LightMapper/BakeAllReflectionProbesSnapshots");
        }

        private void DoClear()
        {
            Lightmapping.ClearLightingDataAsset();
            Lightmapping.Clear();
            UsabilityAnalytics.Track("/LightMapper/Clear");
        }

        private void DrawHelpGUI()
        {
            Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.helpIcon);
            if (GUI.Button(GUILayoutUtility.GetRect(vector.x, vector.y), EditorGUI.GUIContents.helpIcon, EditorStyles.iconButton))
            {
                Help.ShowHelpPage("file:///unity/Manual/GlobalIllumination.html");
            }
        }

        private static void DrawLightmapResolutionField(SerializedProperty resolution, GUIContent label)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(resolution, label, new GUILayoutOption[0]);
            GUILayout.Label(" texels per unit", styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
        }

        private void DrawSettingsGUI()
        {
            Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.titleSettingsIcon);
            Rect position = GUILayoutUtility.GetRect(vector.x, vector.y);
            if (EditorGUI.ButtonMouseDown(position, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.iconButton))
            {
                GUIContent[] options = new GUIContent[] { new GUIContent("Reset") };
                EditorUtility.DisplayCustomMenu(position, options, -1, new EditorUtility.SelectMenuItemFunction(this.ResetSettings), null);
            }
        }

        private void EnlightenBakeSettings()
        {
            SerializedObject so = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
            SerializedProperty enableRealtimeGI = so.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
            SerializedProperty enableBakedGI = so.FindProperty("m_GISettings.m_EnableBakedLightmaps");
            bool boolValue = enableRealtimeGI.boolValue;
            this.RealtimeGUI(so, enableRealtimeGI);
            this.BakedGUI(so, enableRealtimeGI.boolValue, enableBakedGI);
            this.GeneralSettingsGUI(so, enableRealtimeGI.boolValue, enableBakedGI.boolValue);
            if ((enableBakedGI.boolValue && enableRealtimeGI.boolValue) && !boolValue)
            {
                Lightmapping.ClearPrecompSetIsDone();
            }
            so.ApplyModifiedProperties();
        }

        private void GeneralSettingsGUI(SerializedObject so, bool enableRealtimeGI, bool enableBakedGI)
        {
            this.m_ShowGeneralSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowGeneralSettings, styles.GeneralGILabel);
            if (this.m_ShowGeneralSettings)
            {
                SerializedProperty property = so.FindProperty("m_GISettings.m_AlbedoBoost");
                SerializedProperty property2 = so.FindProperty("m_GISettings.m_IndirectOutputScale");
                SerializedProperty prop = so.FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
                SerializedProperty property4 = so.FindProperty("m_LightmapEditorSettings.m_LightmapsBakeMode");
                bool flag = enableBakedGI || enableRealtimeGI;
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!flag))
                {
                    EditorGUILayout.IntPopup(property4, this.kModeStrings, this.kModeValues, s_Styles.DirectionalMode, new GUILayoutOption[0]);
                    if (property4.intValue == 1)
                    {
                        EditorGUILayout.HelpBox(s_Styles.NoDirectionalInSM2AndGLES2.text, MessageType.Warning);
                    }
                    if (property4.intValue == 2)
                    {
                        EditorGUILayout.HelpBox(s_Styles.NoDirectionalSpecularInSM2AndGLES2.text, MessageType.Warning);
                    }
                    EditorGUILayout.Slider(property2, 0f, 5f, styles.IndirectOutputScale, new GUILayoutOption[0]);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Slider(property, 1f, 10f, styles.AlbedoBoost, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Lightmapping.bounceBoost = property.floatValue;
                    }
                    if (LightingWindowObjectTab.LightmapParametersGUI(prop, styles.DefaultLightmapParameters, false))
                    {
                        this.m_Mode = Mode.ObjectSettings;
                    }
                    this.DeveloperBuildEnlightenSettings(so);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void ModeToggle()
        {
            float width = base.position.width - (this.toolbarPadding * 2f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            this.m_Mode = (Mode) GUILayout.Toolbar((int) this.m_Mode, styles.ModeToggles, "LargeButton", options);
        }

        private void OnBecameInvisible()
        {
            s_IsVisible = false;
            LightmapVisualization.enabled = false;
            RepaintSceneAndGameViews();
        }

        private void OnBecameVisible()
        {
            if (!s_IsVisible)
            {
                s_IsVisible = true;
                LightmapVisualization.enabled = true;
                RepaintSceneAndGameViews();
            }
        }

        private void OnDisable()
        {
            this.ClearCachedProperties();
            this.m_ObjectTab.OnDisable();
            SessionState.SetBool("ShowRealtimeLightingSettings", this.m_ShowRealtimeSettings);
            SessionState.SetBool("ShowBakedLightingSettings", this.m_ShowBakeSettings);
            SessionState.SetBool("ShowGeneralLightingSettings", this.m_ShowGeneralSettings);
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            this.m_LightmapPreviewTab = new LightingWindowLightmapPreviewTab();
            this.m_ObjectTab = new LightingWindowObjectTab();
            this.m_ObjectTab.OnEnable(this);
            this.m_ShowRealtimeSettings = SessionState.GetBool("ShowRealtimeLightingSettings", true);
            this.m_ShowBakeSettings = SessionState.GetBool("ShowBakedLightingSettings", true);
            this.m_ShowGeneralSettings = SessionState.GetBool("ShowGeneralLightingSettings", true);
            this.UpdateAnimatedBools(true);
            base.autoRepaintOnSceneChange = true;
            this.m_PreviewResizer.Init("LightmappingPreview");
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            base.Repaint();
        }

        private void OnGUI()
        {
            this.UpdateAnimatedBools(false);
            EditorGUIUtility.labelWidth = 130f;
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(this.toolbarPadding);
            this.ModeToggle();
            GUILayout.FlexibleSpace();
            this.DrawHelpGUI();
            if (this.m_Mode == Mode.BakeSettings)
            {
                this.DrawSettingsGUI();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
            switch (this.m_Mode)
            {
                case Mode.ObjectSettings:
                    this.m_ObjectTab.ObjectSettings();
                    break;

                case Mode.BakeSettings:
                    this.lightingEditor.OnInspectorGUI();
                    this.EnlightenBakeSettings();
                    this.fogEditor.OnInspectorGUI();
                    this.otherRenderingEditor.OnInspectorGUI();
                    break;

                case Mode.Maps:
                    this.m_LightmapPreviewTab.Maps();
                    break;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
            this.Buttons();
            GUI.enabled = true;
            EditorGUILayout.Space();
            this.Summary();
            this.PreviewSection();
        }

        private void OnSelectionChange()
        {
            this.m_LightmapPreviewTab.UpdateLightmapSelection();
            if ((this.m_Mode == Mode.ObjectSettings) || (this.m_Mode == Mode.Maps))
            {
                base.Repaint();
            }
        }

        private void PreviewSection()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            EditorGUILayout.BeginHorizontal(GUIContent.none, "preToolbar", options);
            GUILayout.FlexibleSpace();
            GUI.Label(GUILayoutUtility.GetLastRect(), "Preview", "preToolbar2");
            EditorGUILayout.EndHorizontal();
            float height = this.m_PreviewResizer.ResizeHandle(base.position, 100f, 250f, 17f);
            Rect r = new Rect(0f, base.position.height - height, base.position.width, height);
            Mode mode = this.m_Mode;
            if (mode != Mode.ObjectSettings)
            {
                if (((mode != Mode.BakeSettings) && (mode == Mode.Maps)) && (height > 0f))
                {
                    this.m_LightmapPreviewTab.LightmapPreview(r);
                }
            }
            else if (Selection.activeGameObject != null)
            {
                this.m_ObjectTab.ObjectPreview(r);
            }
        }

        private void RealtimeGUI(SerializedObject so, SerializedProperty enableRealtimeGI)
        {
            this.m_ShowRealtimeSettings = EditorGUILayout.ToggleTitlebar(this.m_ShowRealtimeSettings, styles.RealtimeGILabel, enableRealtimeGI);
            if (this.m_ShowRealtimeSettings)
            {
                SerializedProperty property = so.FindProperty("m_RuntimeCPUUsage");
                SerializedProperty resolution = so.FindProperty("m_LightmapEditorSettings.m_Resolution");
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!enableRealtimeGI.boolValue))
                {
                    DrawLightmapResolutionField(resolution, styles.Resolution);
                    EditorGUILayout.IntPopup(property, styles.RuntimeCPUUsageStrings, styles.RuntimeCPUUsageValues, styles.RuntimeCPUUsage, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
            }
        }

        internal static void RepaintSceneAndGameViews()
        {
            SceneView.RepaintAll();
            GameView.RepaintAll();
        }

        private void ResetSettings(object userData, string[] options, int selected)
        {
            RenderSettings.Reset();
            LightmapEditorSettings.Reset();
            LightmapSettings.Reset();
        }

        private void SetOptions(AnimBool animBool, bool initialize, bool targetValue)
        {
            if (initialize)
            {
                animBool.value = targetValue;
                animBool.valueChanged.AddListener(new UnityAction(this.Repaint));
            }
            else
            {
                animBool.target = targetValue;
            }
        }

        private void Summary()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            int bytes = 0;
            int num2 = 0;
            Dictionary<Vector2, int> dictionary = new Dictionary<Vector2, int>();
            bool flag = false;
            foreach (LightmapData data in LightmapSettings.lightmaps)
            {
                if (data.lightmapLight != null)
                {
                    num2++;
                    Vector2 key = new Vector2((float) data.lightmapLight.width, (float) data.lightmapLight.height);
                    if (dictionary.ContainsKey(key))
                    {
                        Dictionary<Vector2, int> dictionary2;
                        Vector2 vector2;
                        (dictionary2 = dictionary)[vector2 = key] = dictionary2[vector2] + 1;
                    }
                    else
                    {
                        dictionary.Add(key, 1);
                    }
                    bytes += TextureUtil.GetStorageMemorySize(data.lightmapLight);
                    if (data.lightmapDir != null)
                    {
                        bytes += TextureUtil.GetStorageMemorySize(data.lightmapDir);
                        flag = true;
                    }
                }
            }
            object[] objArray1 = new object[] { num2, !flag ? " non-directional" : " directional", " lightmap", (num2 != 1) ? "s" : "" };
            string str = string.Concat(objArray1);
            bool flag2 = true;
            foreach (KeyValuePair<Vector2, int> pair in dictionary)
            {
                str = str + (!flag2 ? ", " : ": ");
                flag2 = false;
                if (pair.Value > 1)
                {
                    str = str + pair.Value + "x";
                }
                string str2 = str;
                object[] objArray2 = new object[] { str2, pair.Key.x, "x", pair.Key.y, "px" };
                str = string.Concat(objArray2);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(str + " ", styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(EditorUtility.FormatBytes(bytes), styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.Label((num2 != 0) ? "" : "No Lightmaps", styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void UpdateAnimatedBools(bool initialize)
        {
            this.SetOptions(this.m_ShowIndirectResolution, initialize, !Lightmapping.realtimeGI);
        }

        private Editor fogEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.renderSettings, typeof(FogEditor), ref this.m_FogEditor);
                return this.m_FogEditor;
            }
        }

        private Editor lightingEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.renderSettings, typeof(LightingEditor), ref this.m_LightingEditor);
                (this.m_LightingEditor as LightingEditor).parentWindow = this;
                return this.m_LightingEditor;
            }
        }

        private Editor otherRenderingEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.renderSettings, typeof(OtherRenderingEditor), ref this.m_OtherRenderingEditor);
                return this.m_OtherRenderingEditor;
            }
        }

        private Object renderSettings
        {
            get
            {
                return RenderSettings.GetRenderSettings();
            }
        }

        private static Styles styles
        {
            get
            {
                Styles styles;
                if (s_Styles != null)
                {
                    styles = s_Styles;
                }
                else
                {
                    styles = s_Styles = new Styles();
                }
                return styles;
            }
        }

        private float toolbarPadding
        {
            get
            {
                if (this.m_ToolbarPadding == -1f)
                {
                    Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.helpIcon);
                    this.m_ToolbarPadding = (vector.x * 2f) + 6f;
                }
                return this.m_ToolbarPadding;
            }
        }

        private enum BakeMode
        {
            BakeReflectionProbes,
            Clear
        }

        private enum Mode
        {
            ObjectSettings,
            BakeSettings,
            Maps
        }

        private class Styles
        {
            public GUIContent AlbedoBoost = EditorGUIUtility.TextContent("Bounce Boost|When light bounces off a surface it is multiplied by the albedo of this surface. This values intensifies albedo and thus affects how strong the light bounces from one surface to the next. Used for realtime and baked lightmaps.");
            public GUIContent AmbientOcclusion = EditorGUIUtility.TextContent("Indirect|Changes contrast of ambient occlusion. It is only applied to the indirect lighting which is physically correct.");
            public GUIContent AmbientOcclusionDirect = EditorGUIUtility.TextContent("Direct|Ambient occlusion on direct lighting. Not physically correct but can be used for artistic purposes.");
            public GUIContent AO = EditorGUIUtility.TextContent("Ambient Occlusion|Darkens areas where the hemisphere above is obscured. Used only for baked lightmaps. Use an SSAO effect for Realtime GI.");
            public GUIContent AOMaxDistance = EditorGUIUtility.TextContent("Max Distance|Beyond this distance a ray is considered to be unoccluded. 0 for infinitely long rays.");
            public GUIContent BakedGILabel = EditorGUIUtility.TextContent("Baked GI|Settings used in Baked Global Illumination where direct and indirect lighting for static objects is precalculated and saved (baked) into lightmaps for use at runtime. This is useful when lights are known to be static, for mobile, for low end devices and other situations where there is not enough processing power to use Precomputed Realtime GI. You can toggle on each light whether it should be included in the bake.");
            public GUIContent BakeResolution = EditorGUIUtility.TextContent("Baked Resolution|Baked lightmap resolution in texels per world unit.");
            public GUIContent BounceScale = EditorGUIUtility.TextContent("Bounce Scale|Multiplier for indirect lighting. Use with care.");
            public GUIContent BuildLabel = EditorGUIUtility.TextContent("Build|Perform the precomputation (for Precomputed Realtime GI) and/or bake (for Baked GI) for the GI modes that are currently enabled.");
            public GUIContent ConcurrentJobs = EditorGUIUtility.TextContent("Concurrent Jobs|The amount of simultaneously scheduled jobs.");
            public GUIContent ContinuousBakeLabel = EditorGUIUtility.TextContent("Auto|Automatically detects changes and builds lighting.");
            public GUIContent DefaultLightmapParameters = EditorGUIUtility.TextContent("Default Parameters|Lets you configure default lightmapping parameters for the scene. Objects will be automatically grouped by unique parameter sets.");
            public GUIContent DirectionalMode = EditorGUIUtility.TextContent("Directional Mode|Lightmaps encode incoming dominant light direction. Directional lightmaps are more expensive in terms of memory and fill rate than non-directional lightmaps.");
            public GUIContent DirectLightInLightProbes = EditorGUIUtility.TextContent("Add Direct Light|Adding direct light into light probes is useful when having fully baked lighting and dynamic objects lit with light probes. If you are using realtime lighting, you should only have indirect lighting in the light probes.");
            public GUIContent FinalGather = EditorGUIUtility.TextContent("Final Gather|Whether to use final gather. Final gather will improve visual quality significantly by using ray tracing at bake resolution for the last light bounce. This will increase bake time.");
            public GUIContent FinalGatherFiltering = EditorGUIUtility.TextContent("Denoising|Applies a denoising filter to the final gather output.");
            public GUIContent FinalGatherRayCount = EditorGUIUtility.TextContent("Ray Count|How many rays to use for final gather per bake output texel.");
            public GUIContent ForceUpdates = EditorGUIUtility.TextContent("Force Updates|Force continuous updates of runtime indirect lighting calculations.");
            public GUIContent ForceWhiteAlbedo = EditorGUIUtility.TextContent("Force White Albedo|Force white albedo during lighting calculations.");
            public GUIContent GeneralGILabel = EditorGUIUtility.TextContent("General GI|Settings that apply to both Global Illumination modes (Precomputed Realtime and Baked).");
            public GUIContent IndirectOutputScale = EditorGUIUtility.TextContent("Indirect Intensity|Scales indirect lighting. Indirect is composed of bounce, emission and ambient lighting. Changes the amount of indirect light within the scene. Used for realtime and baked lightmaps.");
            public GUIContent IndirectResolution = EditorGUIUtility.TextContent("Indirect Resolution|Indirect lightmap resolution in texels per world unit. Equivalent to the Realtime Resolution.");
            public GUIStyle labelStyle = EditorStyles.wordWrappedMiniLabel;
            public GUIContent LightProbesTitle = EditorGUIUtility.TextContent("Light Probes");
            public GUIContent MaxAtlasSize = EditorGUIUtility.TextContent("Atlas Size|The size of a lightmap.");
            public GUIContent[] ModeToggles = new GUIContent[] { EditorGUIUtility.TextContent("Object|Bake settings for the currently selected object."), EditorGUIUtility.TextContent("Scene|Global GI settings."), EditorGUIUtility.TextContent("Lightmaps|The editable list of lightmaps.") };
            public GUIContent NoDirectionalInSM2AndGLES2 = EditorGUIUtility.TextContent("Directional lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0. They will fallback to Non-Directional lightmaps.");
            public GUIContent NoDirectionalSpecularInSM2AndGLES2 = EditorGUIUtility.TextContent("Directional Specular lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0. There is currently no fallback.");
            public GUIContent Padding = EditorGUIUtility.TextContent("Baked Padding|Texel separation between shapes.");
            public GUIContent RealtimeGILabel = EditorGUIUtility.TextContent("Precomputed Realtime GI|Settings used in Precomputed Realtime Global Illumination where it is precomputed how indirect light can bounce between static objects, but the final lighting is done at runtime. Lights, ambient lighting in addition to the materials and emission of static objects can still be changed at runtime. Only static objects can affect GI by blocking and bouncing light, but non-static objects can receive bounced light via light probes.");
            public GUIContent Resolution = EditorGUIUtility.TextContent("Realtime Resolution|Realtime lightmap resolution in texels per world unit. This value is multiplied by the resolution in LightmapParameters to give the output lightmap resolution. This should generally be an order of magnitude less than what is common for baked lightmaps to keep the precompute time manageable and the performance at runtime acceptable.");
            public GUIContent RuntimeCPUUsage = EditorGUIUtility.TextContent("CPU Usage|How much CPU usage to assign to the final lighting calculations at runtime. Increasing this makes the system react faster to changes in lighting at a cost of using more CPU time.");
            public GUIContent[] RuntimeCPUUsageStrings = new GUIContent[] { EditorGUIUtility.TextContent("Low (default)"), EditorGUIUtility.TextContent("Medium"), EditorGUIUtility.TextContent("High"), EditorGUIUtility.TextContent("Unlimited") };
            public int[] RuntimeCPUUsageValues = new int[] { 0x19, 50, 0x4b, 100 };
            public GUIContent SceneViewLightmapDisplay = EditorGUIUtility.TextContent("LightmapDisplay");
            public GUIContent TextureCompression = EditorGUIUtility.TextContent("Compressed|Improves performance and lowers space requirements but might introduce artifacts.");
            public GUIContent UpdateRealtimeProbeLabel = EditorGUIUtility.TextContent("Update Realtime Probes");
            public GUIContent UpdateThreshold = EditorGUIUtility.TextContent("Update Threshold|Threshold for updating realtime GI. A lower value causes more frequent updates (default 1.0).");
        }
    }
}

