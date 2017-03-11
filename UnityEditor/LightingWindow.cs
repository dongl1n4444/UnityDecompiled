namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Lighting", icon="Lighting")]
    internal class LightingWindow : EditorWindow
    {
        public const float kButtonWidth = 150f;
        private const string kGlobalIlluminationUnityManualPage = "file:///unity/Manual/GlobalIllumination.html";
        public LightingWindowLightingTab m_LightingTab;
        private LightingWindowLightmapPreviewTab m_LightmapPreviewTab;
        private Mode m_Mode = Mode.LightingSettings;
        private LightingWindowObjectTab m_ObjectTab;
        private PreviewResizer m_PreviewResizer = new PreviewResizer();
        private Vector2 m_ScrollPositionLighting = Vector2.zero;
        private Vector2 m_ScrollPositionOutputMaps = Vector2.zero;
        private LightModeValidator.Stats m_Stats;
        private float m_ToolbarPadding = -1f;
        private static string[] s_BakeModeOptions = new string[] { "Bake Reflection Probes", "Clear Baked Data" };
        private static bool s_IsVisible = false;

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
            bool enabled = GUI.enabled;
            GUI.enabled &= !EditorApplication.isPlayingOrWillChangePlaymode;
            bool flag2 = LightModeUtil.Get().IsWorkflowAuto();
            if ((Lightmapping.lightingDataAsset != null) && !Lightmapping.lightingDataAsset.isValid)
            {
                EditorGUILayout.HelpBox(Lightmapping.lightingDataAsset.validityErrorMessage, MessageType.Warning);
            }
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            flag2 = GUILayout.Toggle(flag2, Styles.ContinuousBakeLabel, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                LightModeUtil.Get().SetWorkflow(flag2);
            }
            using (new EditorGUI.DisabledScope(flag2))
            {
                if (flag2 || !Lightmapping.isRunning)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(180f) };
                    if (EditorGUI.ButtonWithDropdownList(Styles.BuildLabel, s_BakeModeOptions, new GenericMenu.MenuFunction2(this.BakeDropDownCallback), options))
                    {
                        this.DoBake();
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    if ((LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer) && LightModeUtil.Get().AreBakedLightmapsEnabled())
                    {
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(150f) };
                        if (GUILayout.Button("Force Stop", optionArray2))
                        {
                            Lightmapping.ForceStop();
                        }
                    }
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(150f) };
                    if (GUILayout.Button("Cancel", optionArray3))
                    {
                        Lightmapping.Cancel();
                        UsabilityAnalytics.Track("/LightMapper/Cancel");
                    }
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = enabled;
        }

        [UnityEditor.MenuItem("Window/Lighting/Settings", false, 0x832)]
        private static void CreateLightingWindow()
        {
            LightingWindow window = EditorWindow.GetWindow<LightingWindow>();
            window.minSize = new Vector2(320f, 390f);
            window.Show();
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

        private void DrawSettingsGUI()
        {
            Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.titleSettingsIcon);
            Rect position = GUILayoutUtility.GetRect(vector.x, vector.y);
            if (EditorGUI.DropdownButton(position, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.iconButton))
            {
                GUIContent[] options = new GUIContent[] { new GUIContent("Reset") };
                EditorUtility.DisplayCustomMenu(position, options, -1, new EditorUtility.SelectMenuItemFunction(this.ResetSettings), null);
            }
        }

        private void ModeToggle()
        {
            float width = base.position.width - (this.toolbarPadding * 2f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            this.m_Mode = (Mode) GUILayout.Toolbar((int) this.m_Mode, Styles.ModeToggles, "LargeButton", options);
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
            this.m_LightingTab.OnDisable();
            this.m_ObjectTab.OnDisable();
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            this.m_LightingTab = new LightingWindowLightingTab();
            this.m_LightingTab.OnEnable();
            this.m_LightmapPreviewTab = new LightingWindowLightmapPreviewTab();
            this.m_ObjectTab = new LightingWindowObjectTab();
            this.m_ObjectTab.OnEnable(this);
            base.autoRepaintOnSceneChange = false;
            this.m_PreviewResizer.Init("LightmappingPreview");
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            base.Repaint();
        }

        private void OnGUI()
        {
            LightModeUtil.Get().Load();
            EditorGUIUtility.labelWidth = 130f;
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(this.toolbarPadding);
            this.ModeToggle();
            GUILayout.FlexibleSpace();
            this.DrawHelpGUI();
            if (this.m_Mode == Mode.LightingSettings)
            {
                this.DrawSettingsGUI();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            switch (this.m_Mode)
            {
                case Mode.LightingSettings:
                    this.m_ScrollPositionLighting = EditorGUILayout.BeginScrollView(this.m_ScrollPositionLighting, new GUILayoutOption[0]);
                    this.m_LightingTab.OnGUI();
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.Space();
                    break;

                case Mode.OutputMaps:
                    this.m_ScrollPositionOutputMaps = EditorGUILayout.BeginScrollView(this.m_ScrollPositionOutputMaps, new GUILayoutOption[0]);
                    this.m_LightmapPreviewTab.Maps();
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.Space();
                    break;
            }
            this.Buttons();
            this.Summary();
            this.PreviewSection();
            if (LightModeUtil.Get().Flush())
            {
                InspectorWindow.RepaintAllInspectors();
            }
        }

        private void OnInspectorUpdate()
        {
            base.Repaint();
        }

        private void OnSelectionChange()
        {
            this.m_LightmapPreviewTab.UpdateLightmapSelection();
            base.Repaint();
        }

        private void PreviewSection()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            EditorGUILayout.BeginHorizontal(GUIContent.none, "preToolbar", options);
            GUILayout.FlexibleSpace();
            GUI.Label(GUILayoutUtility.GetLastRect(), (this.m_Mode != Mode.LightingSettings) ? "Preview" : "Statistics", "preToolbar2");
            EditorGUILayout.EndHorizontal();
            switch (this.m_Mode)
            {
                case Mode.LightingSettings:
                {
                    float height = this.m_PreviewResizer.ResizeHandle(base.position, 195f, 240f, 17f);
                    Rect r = new Rect(0f, base.position.height - height, base.position.width, height);
                    if (height > 0f)
                    {
                        this.m_LightingTab.StatisticsPreview(r);
                    }
                    break;
                }
                case Mode.OutputMaps:
                {
                    float num2 = this.m_PreviewResizer.ResizeHandle(base.position, 100f, 250f, 17f);
                    Rect rect4 = new Rect(0f, base.position.height - num2, base.position.width, num2);
                    if (num2 > 0f)
                    {
                        this.m_LightmapPreviewTab.LightmapPreview(rect4);
                    }
                    break;
                }
                case Mode.ObjectSettings:
                {
                    Rect rect7 = new Rect(0f, 180f, base.position.width, base.position.height - 180f);
                    if (Selection.activeGameObject != null)
                    {
                        this.m_ObjectTab.ObjectPreview(rect7);
                    }
                    break;
                }
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

        private void Summary()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            long bytes = 0L;
            int num2 = 0;
            Dictionary<Vector2, int> dictionary = new Dictionary<Vector2, int>();
            bool flag = false;
            bool flag2 = false;
            foreach (LightmapData data in LightmapSettings.lightmaps)
            {
                if (data.lightmapColor != null)
                {
                    num2++;
                    Vector2 key = new Vector2((float) data.lightmapColor.width, (float) data.lightmapColor.height);
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
                    bytes += TextureUtil.GetStorageMemorySizeLong(data.lightmapColor);
                    if (data.lightmapDir != null)
                    {
                        bytes += TextureUtil.GetStorageMemorySizeLong(data.lightmapDir);
                        flag = true;
                    }
                    if (data.shadowMask != null)
                    {
                        bytes += TextureUtil.GetStorageMemorySizeLong(data.shadowMask);
                        flag2 = true;
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(num2);
            builder.Append(!flag ? " non-directional" : " directional");
            builder.Append(" lightmap");
            if (num2 != 1)
            {
                builder.Append("s");
            }
            if (flag2)
            {
                builder.Append(" with shadowmask");
                if (num2 != 1)
                {
                    builder.Append("s");
                }
            }
            bool flag3 = true;
            foreach (KeyValuePair<Vector2, int> pair in dictionary)
            {
                builder.Append(!flag3 ? ", " : ": ");
                flag3 = false;
                if (pair.Value > 1)
                {
                    builder.Append(pair.Value);
                    builder.Append("x");
                }
                builder.Append(pair.Key.x);
                builder.Append("x");
                builder.Append(pair.Key.y);
                builder.Append("px");
            }
            builder.Append(" ");
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(builder.ToString(), Styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(EditorUtility.FormatBytes(bytes), Styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.Label((num2 != 0) ? "" : "No Lightmaps", Styles.labelStyle, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label("Occupied texels: " + InternalEditorUtility.CountToString(Lightmapping.occupiedTexelCount), Styles.labelStyle, new GUILayoutOption[0]);
                if (Lightmapping.isRunning)
                {
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    int num8 = 0;
                    int num9 = 0;
                    int num10 = 0;
                    int length = LightmapSettings.lightmaps.Length;
                    for (int i = 0; i < length; i++)
                    {
                        LightmapConvergence lightmapConvergence = Lightmapping.GetLightmapConvergence(i);
                        if (!lightmapConvergence.IsValid())
                        {
                            num10++;
                        }
                        else if (Lightmapping.GetVisibleTexelCount(i) > 0L)
                        {
                            num4++;
                            if (lightmapConvergence.IsConverged())
                            {
                                num5++;
                            }
                            else
                            {
                                num6++;
                            }
                        }
                        else
                        {
                            num7++;
                            if (lightmapConvergence.IsConverged())
                            {
                                num8++;
                            }
                            else
                            {
                                num9++;
                            }
                        }
                    }
                    EditorGUILayout.LabelField("Lightmaps in view: " + num4, Styles.labelStyle, new GUILayoutOption[0]);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("converged: " + num5, Styles.labelStyle, new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("not converged: " + num6, Styles.labelStyle, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Lightmaps out of view: " + num7, Styles.labelStyle, new GUILayoutOption[0]);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("converged: " + num8, Styles.labelStyle, new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("not converged: " + num9, Styles.labelStyle, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                }
                float lightmapBakeTimeTotal = Lightmapping.GetLightmapBakeTimeTotal();
                float lightmapBakePerformanceTotal = Lightmapping.GetLightmapBakePerformanceTotal();
                if (lightmapBakePerformanceTotal >= 0.0)
                {
                    GUILayout.Label("Bake performance: " + lightmapBakePerformanceTotal.ToString("0.00") + " mrays/sec", Styles.labelStyle, new GUILayoutOption[0]);
                }
                if (!Lightmapping.isRunning)
                {
                    float lightmapBakeTimeRaw = Lightmapping.GetLightmapBakeTimeRaw();
                    if (lightmapBakeTimeTotal >= 0.0)
                    {
                        int num16 = (int) lightmapBakeTimeTotal;
                        int num17 = num16 / 0xe10;
                        num16 -= 0xe10 * num17;
                        int num18 = num16 / 60;
                        num16 -= 60 * num18;
                        int num19 = num16;
                        int num20 = (int) lightmapBakeTimeRaw;
                        int num21 = num20 / 0xe10;
                        num20 -= 0xe10 * num21;
                        int num22 = num20 / 60;
                        num20 -= 60 * num22;
                        int num23 = num20;
                        int num24 = Math.Max(0, (int) (lightmapBakeTimeTotal - lightmapBakeTimeRaw));
                        int num25 = num24 / 0xe10;
                        num24 -= 0xe10 * num25;
                        int num26 = num24 / 60;
                        num24 -= 60 * num26;
                        int num27 = num24;
                        GUILayout.Label("Total bake time: " + num17.ToString("0") + ":" + num18.ToString("00") + ":" + num19.ToString("00"), Styles.labelStyle, new GUILayoutOption[0]);
                        if (Unsupported.IsDeveloperBuild())
                        {
                            GUILayout.Label("(Raw bake time: " + num21.ToString("0") + ":" + num22.ToString("00") + ":" + num23.ToString("00") + ", overhead:" + num25.ToString("0") + ":" + num26.ToString("00") + ":" + num27.ToString("00") + ")", Styles.labelStyle, new GUILayoutOption[0]);
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
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
            LightingSettings,
            OutputMaps,
            ObjectSettings
        }

        private static class Styles
        {
            public static readonly GUIContent BuildLabel = EditorGUIUtility.TextContent("Generate Lighting|Generates the lightmap data for the current master scene.  This lightmap data (for realtime and baked global illumination) is stored in the GI Cache. For GI Cache settings see the Preferences panel.");
            public static readonly GUIContent ContinuousBakeLabel = EditorGUIUtility.TextContent("Auto Generate|Automatically generates lighting data in the Scene when any changes are made to the lighting systems.");
            public static readonly GUIStyle labelStyle = EditorStyles.wordWrappedMiniLabel;
            public static readonly GUIContent[] ModeToggles = new GUIContent[] { EditorGUIUtility.TextContent("Scene"), EditorGUIUtility.TextContent("Global maps"), EditorGUIUtility.TextContent("Object maps") };
        }
    }
}

