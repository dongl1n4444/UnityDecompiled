namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class LightingWindowLightingTab
    {
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache0;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache1;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache2;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache3;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache4;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache5;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache6;
        [CompilerGenerated]
        private static LightModeValidator.GetComponent <>f__mg$cache7;
        private const string kShowEnvironment = "ShowEnvironment";
        private LightingWindowBakeSettings m_BakeSettings;
        private Editor m_FogEditor;
        private Editor m_LightingEditor;
        private Editor m_OtherRenderingEditor;
        private Object m_RenderSettings = null;
        private bool m_ShowEnvironment = true;
        private LightModeValidator.Stats m_Stats;

        private void ClearCachedProperties()
        {
            if (this.m_LightingEditor != null)
            {
                Object.DestroyImmediate(this.m_LightingEditor);
                this.m_LightingEditor = null;
            }
            if (this.m_FogEditor != null)
            {
                Object.DestroyImmediate(this.m_FogEditor);
                this.m_FogEditor = null;
            }
            if (this.m_OtherRenderingEditor != null)
            {
                Object.DestroyImmediate(this.m_OtherRenderingEditor);
                this.m_OtherRenderingEditor = null;
            }
        }

        public void OnDisable()
        {
            this.m_BakeSettings.OnDisable();
            this.ClearCachedProperties();
        }

        public void OnEnable()
        {
            this.m_BakeSettings = new LightingWindowBakeSettings();
            this.m_BakeSettings.OnEnable();
            this.m_ShowEnvironment = SessionState.GetBool("ShowEnvironment", true);
        }

        public void OnGUI()
        {
            EditorGUIUtility.hierarchyMode = true;
            this.m_ShowEnvironment = EditorGUILayout.FoldoutTitlebar(this.m_ShowEnvironment, Styles.Environment, true);
            if (this.m_ShowEnvironment)
            {
                EditorGUILayout.Space();
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200f;
                EditorGUI.indentLevel++;
                this.lightingEditor.OnInspectorGUI();
                this.otherRenderingEditor.OnInspectorGUI();
                this.fogEditor.OnInspectorGUI();
                EditorGUILayout.Space();
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel--;
            }
            this.m_BakeSettings.OnGUI();
        }

        public void StatisticsPreview(Rect r)
        {
            <StatisticsPreview>c__AnonStorey1 storey = new <StatisticsPreview>c__AnonStorey1 {
                $this = this
            };
            GUI.Box(r, "", "PreBackground");
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(r.height) };
            EditorGUILayout.BeginScrollView(Vector2.zero, options);
            bool flag = LightModeUtil.Get().UpdateStatistics();
            if (flag)
            {
                LightModeUtil.Get().AnalyzeScene(ref this.m_Stats);
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(Styles.StatisticsWarning, MessageType.Info);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            storey.bMarkReceivers = LightModeUtil.Get().AreBakedLightmapsEnabled() && (this.m_Stats.receiverMask == LightModeValidator.Receivers.None);
            storey.bMarkEmitters = LightModeUtil.Get().AreBakedLightmapsEnabled() && ((this.m_Stats.emitterMask & LightModeValidator.Emitters.Baked) == LightModeValidator.Emitters.None);
            storey.defCol = GUI.contentColor;
            storey.emitterWarning = "Lightmaps are enabled but there are no static lights or baked emissives in the scene, and Ambient Mode is set to Realtime.";
            storey.receiverWarning = "Lightmaps are enabled but there are no Lightmap Static meshes or Light Probes in the scene.";
            using (new EditorGUI.DisabledScope(!flag))
            {
                using (new TableScoper(1, new TableScoper.drawDelegate(storey.<>m__0)))
                {
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }

        private Editor fogEditor
        {
            get
            {
                if ((this.m_FogEditor == null) || (this.m_FogEditor.target == null))
                {
                    Editor.CreateCachedEditor(this.renderSettings, typeof(FogEditor), ref this.m_FogEditor);
                }
                return this.m_FogEditor;
            }
        }

        private Editor lightingEditor
        {
            get
            {
                if ((this.m_LightingEditor == null) || (this.m_LightingEditor.target == null))
                {
                    Editor.CreateCachedEditor(this.renderSettings, typeof(LightingEditor), ref this.m_LightingEditor);
                }
                return this.m_LightingEditor;
            }
        }

        private Editor otherRenderingEditor
        {
            get
            {
                if ((this.m_OtherRenderingEditor == null) || (this.m_OtherRenderingEditor.target == null))
                {
                    Editor.CreateCachedEditor(this.renderSettings, typeof(OtherRenderingEditor), ref this.m_OtherRenderingEditor);
                }
                return this.m_OtherRenderingEditor;
            }
        }

        private Object renderSettings
        {
            get
            {
                if (this.m_RenderSettings == null)
                {
                    this.m_RenderSettings = RenderSettings.GetRenderSettings();
                }
                return this.m_RenderSettings;
            }
        }

        [CompilerGenerated]
        private sealed class <StatisticsPreview>c__AnonStorey1
        {
            internal LightingWindowLightingTab $this;
            internal bool bMarkEmitters;
            internal bool bMarkReceivers;
            internal Color defCol;
            internal string emitterWarning;
            internal string receiverWarning;

            internal void <>m__0()
            {
                <StatisticsPreview>c__AnonStorey0 storey = new <StatisticsPreview>c__AnonStorey0 {
                    <>f__ref$1 = this
                };
                storey.opts_name = new GUILayoutOption[] { GUILayout.MinWidth(80f), GUILayout.MaxWidth(150f) };
                storey.opts = new GUILayoutOption[] { GUILayout.MinWidth(65f), GUILayout.MaxWidth(65f) };
                if (LightingWindowLightingTab.Styles.StatsTableHeader == null)
                {
                    LightingWindowLightingTab.Styles.StatsTableHeader = new GUIStyle(EditorStyles.boldLabel);
                    LightingWindowLightingTab.Styles.StatsTableContent = new GUIStyle(EditorStyles.label);
                    LightingWindowLightingTab.Styles.StatsTableHeader.alignment = TextAnchor.MiddleLeft;
                    LightingWindowLightingTab.Styles.StatsTableContent.alignment = TextAnchor.MiddleLeft;
                }
                GUIStyle statsTableHeader = LightingWindowLightingTab.Styles.StatsTableHeader;
                storey.st = LightingWindowLightingTab.Styles.StatsTableContent;
                using (new EditorGUILayout.HorizontalScope(new GUILayoutOption[0]))
                {
                    EditorGUILayout.LabelField(GUIContent.Temp("Category"), EditorStyles.boldLabel, storey.opts_name);
                    EditorGUILayout.LabelField(GUIContent.Temp("Enabled"), statsTableHeader, storey.opts);
                    EditorGUILayout.LabelField(GUIContent.Temp("Active"), statsTableHeader, storey.opts);
                    EditorGUILayout.LabelField(GUIContent.Temp("Inactive"), statsTableHeader, storey.opts);
                }
                LightingWindowLightingTab.DrawStats stats = new LightingWindowLightingTab.DrawStats(storey.<>m__0);
                if (LightingWindowLightingTab.<>f__mg$cache0 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache0 = new LightModeValidator.GetComponent(LightModeValidator.GetDynamicLights);
                }
                stats("Dynamic Lights", (int) this.$this.m_Stats.enabled.dynamicLightsCount, (int) this.$this.m_Stats.active.dynamicLightsCount, (int) this.$this.m_Stats.inactive.dynamicLightsCount, false, null, LightingWindowLightingTab.<>f__mg$cache0);
                if (LightingWindowLightingTab.<>f__mg$cache1 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache1 = new LightModeValidator.GetComponent(LightModeValidator.GetStaticLights);
                }
                stats("Stationary Lights", (int) this.$this.m_Stats.enabled.staticLightsCount, (int) this.$this.m_Stats.active.staticLightsCount, (int) this.$this.m_Stats.inactive.staticLightsCount, this.bMarkEmitters, this.emitterWarning, LightingWindowLightingTab.<>f__mg$cache1);
                if (LightingWindowLightingTab.<>f__mg$cache2 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache2 = new LightModeValidator.GetComponent(LightModeValidator.GetDynamicMeshes);
                }
                stats("Dynamic Meshes", (int) this.$this.m_Stats.enabled.dynamicMeshesCount, (int) this.$this.m_Stats.active.dynamicMeshesCount, (int) this.$this.m_Stats.inactive.dynamicMeshesCount, false, null, LightingWindowLightingTab.<>f__mg$cache2);
                if (LightingWindowLightingTab.<>f__mg$cache3 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache3 = new LightModeValidator.GetComponent(LightModeValidator.GetStaticMeshes);
                }
                stats("Static Meshes", (int) this.$this.m_Stats.enabled.staticMeshesCount, (int) this.$this.m_Stats.active.staticMeshesCount, (int) this.$this.m_Stats.inactive.staticMeshesCount, this.bMarkReceivers, this.receiverWarning, LightingWindowLightingTab.<>f__mg$cache3);
                if (LightingWindowLightingTab.<>f__mg$cache4 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache4 = new LightModeValidator.GetComponent(LightModeValidator.GetRealtimeEmissive);
                }
                stats("Realtime Emissive Materials", (int) this.$this.m_Stats.enabled.staticMeshesRealtimeEmissive, (int) this.$this.m_Stats.active.staticMeshesRealtimeEmissive, (int) this.$this.m_Stats.inactive.staticMeshesRealtimeEmissive, false, null, LightingWindowLightingTab.<>f__mg$cache4);
                if (LightingWindowLightingTab.<>f__mg$cache5 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache5 = new LightModeValidator.GetComponent(LightModeValidator.GetBakedEmissive);
                }
                stats("Baked Emissive Materials", (int) this.$this.m_Stats.enabled.staticMeshesBakedEmissive, (int) this.$this.m_Stats.active.staticMeshesBakedEmissive, (int) this.$this.m_Stats.inactive.staticMeshesBakedEmissive, this.bMarkEmitters, this.emitterWarning, LightingWindowLightingTab.<>f__mg$cache5);
                if (LightingWindowLightingTab.<>f__mg$cache6 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache6 = new LightModeValidator.GetComponent(LightModeValidator.GetLightProbeGroups);
                }
                stats("Light Probe Groups", (int) this.$this.m_Stats.enabled.lightProbeGroupsCount, (int) this.$this.m_Stats.active.lightProbeGroupsCount, (int) this.$this.m_Stats.inactive.lightProbeGroupsCount, this.bMarkReceivers, this.receiverWarning, LightingWindowLightingTab.<>f__mg$cache6);
                if (LightingWindowLightingTab.<>f__mg$cache7 == null)
                {
                    LightingWindowLightingTab.<>f__mg$cache7 = new LightModeValidator.GetComponent(LightModeValidator.GetReflectionProbes);
                }
                stats("Reflection Probes", (int) this.$this.m_Stats.enabled.reflectionProbesCount, (int) this.$this.m_Stats.active.reflectionProbesCount, (int) this.$this.m_Stats.inactive.reflectionProbesCount, false, null, LightingWindowLightingTab.<>f__mg$cache7);
            }

            private sealed class <StatisticsPreview>c__AnonStorey0
            {
                internal LightingWindowLightingTab.<StatisticsPreview>c__AnonStorey1 <>f__ref$1;
                internal GUILayoutOption[] opts;
                internal GUILayoutOption[] opts_name;
                internal GUIStyle st;

                internal void <>m__0(string label, int enabled, int active, int inactive, bool bWarning, string strWarning, LightModeValidator.GetComponent del)
                {
                    using (new EditorGUILayout.HorizontalScope(new GUILayoutOption[0]))
                    {
                        EditorGUILayout.LabelField(label, this.opts_name);
                        Rect lastRect = GUILayoutUtility.GetLastRect();
                        EditorGUILayout.LabelField(enabled.ToString(), this.st, this.opts);
                        lastRect.xMax = GUILayoutUtility.GetLastRect().xMax;
                        EditorGUILayout.LabelField(active.ToString(), this.st, this.opts);
                        EditorGUILayout.LabelField(inactive.ToString(), this.st, this.opts);
                        GUI.contentColor = this.<>f__ref$1.defCol;
                    }
                    if ((strWarning != null) && bWarning)
                    {
                        EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), GUIContent.Temp("", strWarning));
                    }
                }
            }
        }

        private delegate void DrawStats(string label, int enabled, int active, int inactive, bool bWarning, string strWarning, LightModeValidator.GetComponent del);

        private class Styles
        {
            public static readonly GUIContent Environment = EditorGUIUtility.TextContent("Environment");
            public static readonly string StatisticsWarning = "Statistics are not updated during play mode. This behavior can be changed via Settings -> UI Settings (Beta) -> \"Don't update statistics\"";
            public static GUIStyle StatsTableContent = null;
            public static GUIStyle StatsTableHeader = null;
        }
    }
}

