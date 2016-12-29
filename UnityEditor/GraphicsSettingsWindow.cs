namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEditor.Rendering;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class GraphicsSettingsWindow : EditorWindow
    {
        private const float kToolbarPadding = 38f;
        private Editor m_AlwaysIncludedShadersEditor;
        private Editor m_BuiltinShadersEditor;
        private Vector2 m_ScrollPosition = Vector2.zero;
        private Editor m_ShaderPreloadEditor;
        private Editor m_ShaderStrippingEditor;
        private SettingsTab m_Tab = SettingsTab.Tiers;
        private Editor m_TierSettingsEditor;

        private void OnDisable()
        {
            Object.DestroyImmediate(this.m_TierSettingsEditor);
            this.m_TierSettingsEditor = null;
            Object.DestroyImmediate(this.m_BuiltinShadersEditor);
            this.m_BuiltinShadersEditor = null;
            Object.DestroyImmediate(this.m_AlwaysIncludedShadersEditor);
            this.m_AlwaysIncludedShadersEditor = null;
            Object.DestroyImmediate(this.m_ShaderStrippingEditor);
            this.m_ShaderStrippingEditor = null;
            Object.DestroyImmediate(this.m_ShaderPreloadEditor);
            this.m_ShaderPreloadEditor = null;
        }

        private void OnGUI()
        {
            this.TabsGUI();
            this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
            switch (this.m_Tab)
            {
                case SettingsTab.Tiers:
                    this.OnTiersGUI();
                    break;

                case SettingsTab.Shaders:
                    this.OnShadersGUI();
                    break;
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnShadersGUI()
        {
            this.alwaysIncludedShadersEditor.OnInspectorGUI();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.builtinSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.builtinShadersEditor.OnInspectorGUI();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.shaderStrippingSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.shaderStrippingEditor.OnInspectorGUI();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.shaderPreloadSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.shaderPreloadEditor.OnInspectorGUI();
        }

        private void OnTiersGUI()
        {
            this.tierSettingsEditor.OnInspectorGUI();
        }

        private void TabsGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(38f);
            float width = base.position.width - 76f;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            this.m_Tab = (SettingsTab) GUILayout.Toolbar((int) this.m_Tab, Styles.Tabs, "LargeButton", options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private Editor alwaysIncludedShadersEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(AlwaysIncludedShadersEditor), ref this.m_AlwaysIncludedShadersEditor);
                return this.m_AlwaysIncludedShadersEditor;
            }
        }

        private Editor builtinShadersEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(BuiltinShadersEditor), ref this.m_BuiltinShadersEditor);
                return this.m_BuiltinShadersEditor;
            }
        }

        private Object graphicsSettings =>
            GraphicsSettings.GetGraphicsSettings();

        private Editor shaderPreloadEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(ShaderPreloadEditor), ref this.m_ShaderPreloadEditor);
                return this.m_ShaderPreloadEditor;
            }
        }

        private Editor shaderStrippingEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(ShaderStrippingEditor), ref this.m_ShaderStrippingEditor);
                return this.m_ShaderStrippingEditor;
            }
        }

        private Editor tierSettingsEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(TierSettingsEditor), ref this.m_TierSettingsEditor);
                ((TierSettingsEditor) this.m_TierSettingsEditor).verticalLayout = true;
                return this.m_TierSettingsEditor;
            }
        }

        [CustomEditor(typeof(GraphicsSettings))]
        internal class AlwaysIncludedShadersEditor : Editor
        {
            private SerializedProperty m_AlwaysIncludedShaders;

            public void OnEnable()
            {
                this.m_AlwaysIncludedShaders = base.serializedObject.FindProperty("m_AlwaysIncludedShaders");
                this.m_AlwaysIncludedShaders.isExpanded = true;
            }

            public override void OnInspectorGUI()
            {
                base.serializedObject.Update();
                EditorGUILayout.PropertyField(this.m_AlwaysIncludedShaders, true, new GUILayoutOption[0]);
                base.serializedObject.ApplyModifiedProperties();
            }
        }

        [CustomEditor(typeof(GraphicsSettings))]
        internal class BuiltinShadersEditor : Editor
        {
            private GraphicsSettingsWindow.BuiltinShaderSettings m_Deferred;
            private GraphicsSettingsWindow.BuiltinShaderSettings m_DeferredReflections;
            private GraphicsSettingsWindow.BuiltinShaderSettings m_LegacyDeferred;
            private GraphicsSettingsWindow.BuiltinShaderSettings m_MotionVectors;
            private GraphicsSettingsWindow.BuiltinShaderSettings m_ScreenSpaceShadows;

            public void OnEnable()
            {
                this.m_Deferred = new GraphicsSettingsWindow.BuiltinShaderSettings(this.deferredString, "m_Deferred", base.serializedObject);
                this.m_DeferredReflections = new GraphicsSettingsWindow.BuiltinShaderSettings(this.deferredReflString, "m_DeferredReflections", base.serializedObject);
                this.m_ScreenSpaceShadows = new GraphicsSettingsWindow.BuiltinShaderSettings(this.screenShadowsString, "m_ScreenSpaceShadows", base.serializedObject);
                this.m_LegacyDeferred = new GraphicsSettingsWindow.BuiltinShaderSettings(this.legacyDeferredString, "m_LegacyDeferred", base.serializedObject);
                this.m_MotionVectors = new GraphicsSettingsWindow.BuiltinShaderSettings(this.motionVectorsString, "m_MotionVectors", base.serializedObject);
            }

            public override void OnInspectorGUI()
            {
                base.serializedObject.Update();
                this.m_Deferred.DoGUI();
                EditorGUI.BeginChangeCheck();
                this.m_DeferredReflections.DoGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    ShaderUtil.ReloadAllShaders();
                }
                this.m_ScreenSpaceShadows.DoGUI();
                this.m_LegacyDeferred.DoGUI();
                this.m_MotionVectors.DoGUI();
                base.serializedObject.ApplyModifiedProperties();
            }

            private string deferredReflString =>
                LocalizationDatabase.GetLocalizedString("Deferred Reflections|Shader settings for deferred reflections");

            private string deferredString =>
                LocalizationDatabase.GetLocalizedString("Deferred|Shader settings for Deferred Shading");

            private string legacyDeferredString =>
                LocalizationDatabase.GetLocalizedString("Legacy Deferred|Shader settings for Legacy (light prepass) Deferred Lighting");

            private string motionVectorsString =>
                LocalizationDatabase.GetLocalizedString("Motion Vectors|Shader for generation of Motion Vectors when the rendering camera has renderMotionVectors set to true");

            private string screenShadowsString =>
                LocalizationDatabase.GetLocalizedString("Screen Space Shadows|Shader settings for cascaded shadow maps");
        }

        internal class BuiltinShaderSettings
        {
            private readonly GUIContent m_Label;
            private readonly SerializedProperty m_Mode;
            private readonly SerializedProperty m_Shader;

            internal BuiltinShaderSettings(string label, string name, SerializedObject serializedObject)
            {
                this.m_Mode = serializedObject.FindProperty(name + ".m_Mode");
                this.m_Shader = serializedObject.FindProperty(name + ".m_Shader");
                this.m_Label = EditorGUIUtility.TextContent(label);
            }

            internal void DoGUI()
            {
                EditorGUILayout.PropertyField(this.m_Mode, this.m_Label, new GUILayoutOption[0]);
                if (this.m_Mode.intValue == 2)
                {
                    EditorGUILayout.PropertyField(this.m_Shader, new GUILayoutOption[0]);
                }
            }

            internal enum BuiltinShaderMode
            {
                None,
                Builtin,
                Custom
            }
        }

        private enum SettingsTab
        {
            Tiers,
            Shaders
        }

        [CustomEditor(typeof(GraphicsSettings))]
        internal class ShaderPreloadEditor : Editor
        {
            private SerializedProperty m_PreloadedShaders;

            public void OnEnable()
            {
                this.m_PreloadedShaders = base.serializedObject.FindProperty("m_PreloadedShaders");
                this.m_PreloadedShaders.isExpanded = true;
            }

            public override void OnInspectorGUI()
            {
                base.serializedObject.ApplyModifiedProperties();
                EditorGUILayout.PropertyField(this.m_PreloadedShaders, true, new GUILayoutOption[0]);
                EditorGUILayout.Space();
                GUILayout.Label($"Currently tracked: {ShaderUtil.GetCurrentShaderVariantCollectionShaderCount()} shaders {ShaderUtil.GetCurrentShaderVariantCollectionVariantCount()} total variants", new GUILayoutOption[0]);
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Styles.shaderPreloadSave, EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    string message = "Save shader variant collection";
                    string str2 = EditorUtility.SaveFilePanelInProject("Save Shader Variant Collection", "NewShaderVariants", "shadervariants", message, ProjectWindowUtil.GetActiveFolderPath());
                    if (!string.IsNullOrEmpty(str2))
                    {
                        ShaderUtil.SaveCurrentShaderVariantCollection(str2);
                    }
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button(Styles.shaderPreloadClear, EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    ShaderUtil.ClearCurrentShaderVariantCollection();
                }
                EditorGUILayout.EndHorizontal();
                base.serializedObject.ApplyModifiedProperties();
            }

            internal class Styles
            {
                public static readonly GUIContent shaderPreloadClear = EditorGUIUtility.TextContent("Clear|Clear currently tracked shader variant information.");
                public static readonly GUIContent shaderPreloadSave = EditorGUIUtility.TextContent("Save to asset...|Save currently tracked shaders into a Shader Variant Manifest asset.");
            }
        }

        [CustomEditor(typeof(GraphicsSettings))]
        internal class ShaderStrippingEditor : Editor
        {
            private SerializedProperty m_FogKeepExp;
            private SerializedProperty m_FogKeepExp2;
            private SerializedProperty m_FogKeepLinear;
            private SerializedProperty m_FogStripping;
            private SerializedProperty m_LightmapKeepDirCombined;
            private SerializedProperty m_LightmapKeepDirSeparate;
            private SerializedProperty m_LightmapKeepDynamicDirCombined;
            private SerializedProperty m_LightmapKeepDynamicDirSeparate;
            private SerializedProperty m_LightmapKeepDynamicPlain;
            private SerializedProperty m_LightmapKeepPlain;
            private SerializedProperty m_LightmapStripping;

            public void OnEnable()
            {
                this.m_LightmapStripping = base.serializedObject.FindProperty("m_LightmapStripping");
                this.m_LightmapKeepPlain = base.serializedObject.FindProperty("m_LightmapKeepPlain");
                this.m_LightmapKeepDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDirCombined");
                this.m_LightmapKeepDirSeparate = base.serializedObject.FindProperty("m_LightmapKeepDirSeparate");
                this.m_LightmapKeepDynamicPlain = base.serializedObject.FindProperty("m_LightmapKeepDynamicPlain");
                this.m_LightmapKeepDynamicDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDynamicDirCombined");
                this.m_LightmapKeepDynamicDirSeparate = base.serializedObject.FindProperty("m_LightmapKeepDynamicDirSeparate");
                this.m_FogStripping = base.serializedObject.FindProperty("m_FogStripping");
                this.m_FogKeepLinear = base.serializedObject.FindProperty("m_FogKeepLinear");
                this.m_FogKeepExp = base.serializedObject.FindProperty("m_FogKeepExp");
                this.m_FogKeepExp2 = base.serializedObject.FindProperty("m_FogKeepExp2");
            }

            public override void OnInspectorGUI()
            {
                base.serializedObject.Update();
                bool flag = false;
                bool flag2 = false;
                EditorGUILayout.PropertyField(this.m_LightmapStripping, Styles.lightmapModes, new GUILayoutOption[0]);
                if (this.m_LightmapStripping.intValue != 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(this.m_LightmapKeepPlain, Styles.lightmapPlain, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LightmapKeepDirCombined, Styles.lightmapDirCombined, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LightmapKeepDirSeparate, Styles.lightmapDirSeparate, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicPlain, Styles.lightmapDynamicPlain, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicDirCombined, Styles.lightmapDynamicDirCombined, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicDirSeparate, Styles.lightmapDynamicDirSeparate, new GUILayoutOption[0]);
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(Styles.lightmapFromScene, EditorStyles.miniButton, options))
                    {
                        flag = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(this.m_FogStripping, Styles.fogModes, new GUILayoutOption[0]);
                if (this.m_FogStripping.intValue != 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(this.m_FogKeepLinear, Styles.fogLinear, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_FogKeepExp, Styles.fogExp, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_FogKeepExp2, Styles.fogExp2, new GUILayoutOption[0]);
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(Styles.fogFromScene, EditorStyles.miniButton, optionArray2))
                    {
                        flag2 = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }
                base.serializedObject.ApplyModifiedProperties();
                if (flag)
                {
                    ShaderUtil.CalculateLightmapStrippingFromCurrentScene();
                }
                if (flag2)
                {
                    ShaderUtil.CalculateFogStrippingFromCurrentScene();
                }
            }

            internal class Styles
            {
                public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("Built-in shader settings");
                public static readonly GUIContent fogExp = EditorGUIUtility.TextContent("Exponential|Include support for Exponential fog.");
                public static readonly GUIContent fogExp2 = EditorGUIUtility.TextContent("Exponential Squared|Include support for Exponential Squared fog.");
                public static readonly GUIContent fogFromScene = EditorGUIUtility.TextContent("From current scene|Calculate fog modes used by the current scene.");
                public static readonly GUIContent fogLinear = EditorGUIUtility.TextContent("Linear|Include support for Linear fog.");
                public static readonly GUIContent fogModes = EditorGUIUtility.TextContent("Fog modes");
                public static readonly GUIContent lightmapDirCombined = EditorGUIUtility.TextContent("Baked Directional|Include support for baked directional lightmaps.");
                public static readonly GUIContent lightmapDirSeparate = EditorGUIUtility.TextContent("Baked Directional Specular|Include support for baked directional specular lightmaps.");
                public static readonly GUIContent lightmapDynamicDirCombined = EditorGUIUtility.TextContent("Realtime Directional|Include support for realtime directional lightmaps.");
                public static readonly GUIContent lightmapDynamicDirSeparate = EditorGUIUtility.TextContent("Realtime Directional Specular|Include support for realtime directional specular lightmaps.");
                public static readonly GUIContent lightmapDynamicPlain = EditorGUIUtility.TextContent("Realtime Non-Directional|Include support for realtime non-directional lightmaps.");
                public static readonly GUIContent lightmapFromScene = EditorGUIUtility.TextContent("From current scene|Calculate lightmap modes used by the current scene.");
                public static readonly GUIContent lightmapModes = EditorGUIUtility.TextContent("Lightmap modes");
                public static readonly GUIContent lightmapPlain = EditorGUIUtility.TextContent("Baked Non-Directional|Include support for baked non-directional lightmaps.");
                public static readonly GUIContent shaderPreloadClear = EditorGUIUtility.TextContent("Clear|Clear currently tracked shader variant information.");
                public static readonly GUIContent shaderPreloadSave = EditorGUIUtility.TextContent("Save to asset...|Save currently tracked shaders into a Shader Variant Manifest asset.");
                public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("Shader preloading");
                public static readonly GUIContent shaderSettings = EditorGUIUtility.TextContent("Platform shader settings");
                public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("Shader stripping");
            }
        }

        internal class Styles
        {
            public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("Built-in shader settings");
            public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("Shader preloading");
            public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("Shader stripping");
            public static readonly GUIContent[] Tabs = new GUIContent[] { EditorGUIUtility.TextContent("Tiers|Preliminary name."), EditorGUIUtility.TextContent("Shaders|Preliminary name.") };
        }

        [CustomEditor(typeof(GraphicsSettings))]
        internal class TierSettingsEditor : Editor
        {
            public bool verticalLayout = false;

            internal void OnFieldLabelsGUI()
            {
                EditorGUILayout.LabelField(Styles.cascadedShadowMaps, new GUILayoutOption[0]);
                EditorGUILayout.LabelField(Styles.standardShaderQuality, new GUILayoutOption[0]);
                EditorGUILayout.LabelField(Styles.reflectionProbeBoxProjection, new GUILayoutOption[0]);
                EditorGUILayout.LabelField(Styles.reflectionProbeBlending, new GUILayoutOption[0]);
                EditorGUILayout.LabelField(Styles.renderingPath, new GUILayoutOption[0]);
            }

            internal void OnGuiHorizontal(BuildTargetGroup platform)
            {
                IEnumerator enumerator = Enum.GetValues(typeof(GraphicsTier)).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        GraphicsTier current = (GraphicsTier) enumerator.Current;
                        bool flag = EditorGraphicsSettings.AreTierSettingsAutomatic(platform, current);
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        EditorGUIUtility.labelWidth = 80f;
                        EditorGUILayout.LabelField(Styles.tierName[(int) current], EditorStyles.boldLabel, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        EditorGUIUtility.labelWidth = 75f;
                        flag = EditorGUILayout.Toggle(Styles.autoSettings, flag, new GUILayoutOption[0]);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorGraphicsSettings.RegisterUndoForGraphicsSettings();
                            EditorGraphicsSettings.MakeTierSettingsAutomatic(platform, current, flag);
                            EditorGraphicsSettings.OnUpdateTierSettingsImpl(platform, true);
                        }
                        using (new EditorGUI.DisabledScope(flag))
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                            EditorGUIUtility.labelWidth = 140f;
                            this.OnFieldLabelsGUI();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                            EditorGUIUtility.labelWidth = 50f;
                            this.OnTierGUI(platform, current);
                            EditorGUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                EditorGUIUtility.labelWidth = 0f;
            }

            internal void OnGuiVertical(BuildTargetGroup platform)
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                EditorGUIUtility.labelWidth = 140f;
                EditorGUILayout.LabelField(Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.OnFieldLabelsGUI();
                EditorGUILayout.EndVertical();
                EditorGUIUtility.labelWidth = 50f;
                IEnumerator enumerator = Enum.GetValues(typeof(GraphicsTier)).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        GraphicsTier current = (GraphicsTier) enumerator.Current;
                        EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                        EditorGUILayout.LabelField(Styles.tierName[(int) current], EditorStyles.boldLabel, new GUILayoutOption[0]);
                        this.OnTierGUI(platform, current);
                        EditorGUILayout.EndVertical();
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.EndHorizontal();
            }

            public override void OnInspectorGUI()
            {
                BuildPlayerWindow.BuildPlatform[] platforms = BuildPlayerWindow.GetValidPlatforms().ToArray();
                BuildTargetGroup targetGroup = platforms[EditorGUILayout.BeginPlatformGrouping(platforms, null, GUIStyle.none)].targetGroup;
                if (this.verticalLayout)
                {
                    this.OnGuiVertical(targetGroup);
                }
                else
                {
                    this.OnGuiHorizontal(targetGroup);
                }
                EditorGUILayout.EndPlatformGrouping();
            }

            internal void OnTierGUI(BuildTargetGroup platform, GraphicsTier tier)
            {
                TierSettings tierSettings = EditorGraphicsSettings.GetTierSettings(platform, tier);
                EditorGUI.BeginChangeCheck();
                tierSettings.cascadedShadowMaps = EditorGUILayout.Toggle(tierSettings.cascadedShadowMaps, new GUILayoutOption[0]);
                tierSettings.standardShaderQuality = this.ShaderQualityPopup(tierSettings.standardShaderQuality);
                tierSettings.reflectionProbeBoxProjection = EditorGUILayout.Toggle(tierSettings.reflectionProbeBoxProjection, new GUILayoutOption[0]);
                tierSettings.reflectionProbeBlending = EditorGUILayout.Toggle(tierSettings.reflectionProbeBlending, new GUILayoutOption[0]);
                tierSettings.renderingPath = this.RenderingPathPopup(tierSettings.renderingPath);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorGraphicsSettings.RegisterUndoForGraphicsSettings();
                    EditorGraphicsSettings.SetTierSettings(platform, tier, tierSettings);
                }
            }

            internal RenderingPath RenderingPathPopup(RenderingPath rp) => 
                ((RenderingPath) EditorGUILayout.IntPopup((int) rp, Styles.renderingPathName, Styles.renderingPathValue, new GUILayoutOption[0]));

            internal ShaderQuality ShaderQualityPopup(ShaderQuality sq) => 
                ((ShaderQuality) EditorGUILayout.IntPopup((int) sq, Styles.shaderQualityName, Styles.shaderQualityValue, new GUILayoutOption[0]));

            internal class Styles
            {
                public static readonly GUIContent autoSettings;
                public static readonly GUIContent cascadedShadowMaps;
                public static readonly GUIContent empty;
                public static readonly GUIContent reflectionProbeBlending;
                public static readonly GUIContent reflectionProbeBoxProjection;
                public static readonly GUIContent renderingPath;
                public static readonly GUIContent[] renderingPathName;
                public static readonly int[] renderingPathValue;
                public static readonly GUIContent[] shaderQualityName = new GUIContent[] { new GUIContent("Low"), new GUIContent("Medium"), new GUIContent("High") };
                public static readonly int[] shaderQualityValue;
                public static readonly GUIContent standardShaderQuality;
                public static readonly GUIContent[] tierName;

                static Styles()
                {
                    int[] numArray1 = new int[3];
                    numArray1[1] = 1;
                    numArray1[2] = 2;
                    shaderQualityValue = numArray1;
                    renderingPathName = new GUIContent[] { new GUIContent("Forward"), new GUIContent("Deferred"), new GUIContent("Legacy Vertex Lit"), new GUIContent("Legacy Deferred (light prepass)") };
                    renderingPathValue = new int[] { 1, 3, 0, 2 };
                    tierName = new GUIContent[] { new GUIContent("Low (Tier1)"), new GUIContent("Medium (Tier 2)"), new GUIContent("High (Tier 3)") };
                    empty = EditorGUIUtility.TextContent("");
                    autoSettings = EditorGUIUtility.TextContent("Use Defaults");
                    cascadedShadowMaps = EditorGUIUtility.TextContent("Cascaded Shadows");
                    standardShaderQuality = EditorGUIUtility.TextContent("Standard Shader Quality");
                    reflectionProbeBoxProjection = EditorGUIUtility.TextContent("Reflection Probes Box Projection");
                    reflectionProbeBlending = EditorGUIUtility.TextContent("Reflection Probes Blending");
                    renderingPath = EditorGUIUtility.TextContent("Rendering Path");
                }
            }
        }
    }
}

