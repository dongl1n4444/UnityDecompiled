namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(RenderSettings))]
    internal class LightingEditor : Editor
    {
        private const string kShowAmbient = "ShowAmbient";
        private const string kShowReflections = "ShowReflections";
        private const string kShowSkybox = "ShowSkybox";
        protected SerializedProperty m_AmbientEquatorColor;
        protected SerializedProperty m_AmbientGroundColor;
        protected SerializedProperty m_AmbientIntensity;
        protected SerializedProperty m_AmbientSkyColor;
        protected SerializedProperty m_AmbientSource;
        private bool m_bShowAmbient;
        private bool m_bShowReflections;
        private bool m_bShowSkybox;
        protected SerializedProperty m_CustomReflection;
        protected SerializedProperty m_DefaultReflectionMode;
        protected SerializedProperty m_DefaultReflectionResolution;
        protected SerializedObject m_LightmapSettings;
        protected SerializedProperty m_ReflectionBounces;
        protected SerializedProperty m_ReflectionCompression;
        protected SerializedProperty m_ReflectionIntensity;
        protected SerializedProperty m_SkyboxMaterial;
        protected SerializedProperty m_Sun;

        private void DrawGUI()
        {
            Material objectReferenceValue = this.m_SkyboxMaterial.objectReferenceValue as Material;
            this.m_bShowSkybox = EditorGUILayout.Foldout(this.m_bShowSkybox, Styles.env_skybox_top);
            if (this.m_bShowSkybox)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_SkyboxMaterial, Styles.env_skybox_mat, new GUILayoutOption[0]);
                if ((objectReferenceValue != null) && !EditorMaterialUtility.IsBackgroundMaterial(objectReferenceValue))
                {
                    EditorGUILayout.HelpBox(Styles.skyboxWarning.text, MessageType.Warning);
                }
                EditorGUILayout.PropertyField(this.m_Sun, Styles.env_skybox_sun, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            this.m_bShowAmbient = EditorGUILayout.Foldout(this.m_bShowAmbient, Styles.env_amb_top);
            if (this.m_bShowAmbient)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.IntPopup(this.m_AmbientSource, Styles.kFullAmbientSource, Styles.kFullAmbientSourceValues, Styles.env_amb_src, new GUILayoutOption[0]);
                switch (((AmbientMode) this.m_AmbientSource.intValue))
                {
                    case AmbientMode.Trilight:
                    {
                        EditorGUI.BeginChangeCheck();
                        Color color = EditorGUILayout.ColorField(Styles.ambientUp, this.m_AmbientSkyColor.colorValue, true, false, true, ColorPicker.defaultHDRConfig, new GUILayoutOption[0]);
                        Color color2 = EditorGUILayout.ColorField(Styles.ambientMid, this.m_AmbientEquatorColor.colorValue, true, false, true, ColorPicker.defaultHDRConfig, new GUILayoutOption[0]);
                        Color color3 = EditorGUILayout.ColorField(Styles.ambientDown, this.m_AmbientGroundColor.colorValue, true, false, true, ColorPicker.defaultHDRConfig, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.m_AmbientSkyColor.colorValue = color;
                            this.m_AmbientEquatorColor.colorValue = color2;
                            this.m_AmbientGroundColor.colorValue = color3;
                        }
                        break;
                    }
                    case AmbientMode.Flat:
                    {
                        EditorGUI.BeginChangeCheck();
                        Color color4 = EditorGUILayout.ColorField(Styles.ambient, this.m_AmbientSkyColor.colorValue, true, false, true, ColorPicker.defaultHDRConfig, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.m_AmbientSkyColor.colorValue = color4;
                        }
                        break;
                    }
                    case AmbientMode.Skybox:
                        if (objectReferenceValue == null)
                        {
                            EditorGUI.BeginChangeCheck();
                            Color color5 = EditorGUILayout.ColorField(Styles.ambient, this.m_AmbientSkyColor.colorValue, true, false, true, ColorPicker.defaultHDRConfig, new GUILayoutOption[0]);
                            if (EditorGUI.EndChangeCheck())
                            {
                                this.m_AmbientSkyColor.colorValue = color5;
                            }
                        }
                        EditorGUILayout.Slider(this.m_AmbientIntensity, 0f, 8f, Styles.env_amb_int, new GUILayoutOption[0]);
                        break;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            this.m_bShowReflections = EditorGUILayout.Foldout(this.m_bShowReflections, Styles.env_refl_top);
            if (this.m_bShowReflections)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_DefaultReflectionMode, Styles.env_refl_src, new GUILayoutOption[0]);
                switch (((DefaultReflectionMode) this.m_DefaultReflectionMode.intValue))
                {
                    case DefaultReflectionMode.FromSkybox:
                    {
                        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
                        EditorGUILayout.IntPopup(this.m_DefaultReflectionResolution, Styles.defaultReflectionSizes, Styles.defaultReflectionSizesValues, Styles.env_refl_res, options);
                        break;
                    }
                    case DefaultReflectionMode.Custom:
                        EditorGUILayout.PropertyField(this.m_CustomReflection, Styles.customReflection, new GUILayoutOption[0]);
                        break;
                }
                EditorGUILayout.PropertyField(this.m_ReflectionCompression, Styles.env_refl_cmp, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_ReflectionIntensity, 0f, 1f, Styles.env_refl_int, new GUILayoutOption[0]);
                EditorGUILayout.IntSlider(this.m_ReflectionBounces, 1, 5, Styles.env_refl_bnc, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        public virtual void OnDisable()
        {
            SessionState.SetBool("ShowSkybox", this.m_bShowSkybox);
            SessionState.SetBool("ShowAmbient", this.m_bShowAmbient);
            SessionState.SetBool("ShowReflections", this.m_bShowReflections);
        }

        public virtual void OnEnable()
        {
            this.m_Sun = base.serializedObject.FindProperty("m_Sun");
            this.m_AmbientSource = base.serializedObject.FindProperty("m_AmbientMode");
            this.m_AmbientSkyColor = base.serializedObject.FindProperty("m_AmbientSkyColor");
            this.m_AmbientEquatorColor = base.serializedObject.FindProperty("m_AmbientEquatorColor");
            this.m_AmbientGroundColor = base.serializedObject.FindProperty("m_AmbientGroundColor");
            this.m_AmbientIntensity = base.serializedObject.FindProperty("m_AmbientIntensity");
            this.m_ReflectionIntensity = base.serializedObject.FindProperty("m_ReflectionIntensity");
            this.m_ReflectionBounces = base.serializedObject.FindProperty("m_ReflectionBounces");
            this.m_SkyboxMaterial = base.serializedObject.FindProperty("m_SkyboxMaterial");
            this.m_DefaultReflectionMode = base.serializedObject.FindProperty("m_DefaultReflectionMode");
            this.m_DefaultReflectionResolution = base.serializedObject.FindProperty("m_DefaultReflectionResolution");
            this.m_CustomReflection = base.serializedObject.FindProperty("m_CustomReflection");
            this.m_LightmapSettings = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
            this.m_ReflectionCompression = this.m_LightmapSettings.FindProperty("m_LightmapEditorSettings.m_ReflectionCompression");
            this.m_bShowSkybox = SessionState.GetBool("ShowSkybox", true);
            this.m_bShowAmbient = SessionState.GetBool("ShowAmbient", true);
            this.m_bShowReflections = SessionState.GetBool("ShowReflections", true);
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.m_LightmapSettings.Update();
            this.DrawGUI();
            base.serializedObject.ApplyModifiedProperties();
            this.m_LightmapSettings.ApplyModifiedProperties();
        }

        internal static class Styles
        {
            public static readonly GUIContent ambient = EditorGUIUtility.TextContent("Ambient Color|Controls the color of the ambient light contributed to the Scene.");
            public static readonly GUIContent ambientDown = EditorGUIUtility.TextContent("Ground Color|Controls the color of light emitted from the ground of the Scene.");
            public static readonly GUIContent ambientMid = EditorGUIUtility.TextContent("Equator Color|Controls the color of light emitted from the sides of the Scene.");
            public static readonly GUIContent ambientUp = EditorGUIUtility.TextContent("Sky Color|Controls the color of light emitted from the sky in the Scene.");
            public static readonly GUIContent createLight = EditorGUIUtility.TextContent("Create Light");
            public static readonly GUIContent customReflection = EditorGUIUtility.TextContent("Cubemap|Specifies the custom cube map used for reflection effects in the Scene.");
            public static GUIContent[] defaultReflectionSizes = Enumerable.Select<int, GUIContent>(defaultReflectionSizesValues, new Func<int, GUIContent>(LightingEditor.Styles.<defaultReflectionSizes>m__0)).ToArray<GUIContent>();
            public static int[] defaultReflectionSizesValues = new int[] { 0x80, 0x100, 0x200, 0x400 };
            public static readonly GUIContent env_amb_int = EditorGUIUtility.TextContent("Intensity|Controls the brightness of the skybox lighting in the Scene.");
            public static readonly GUIContent env_amb_src = EditorGUIUtility.TextContent("Source|Specifies whether to use a skybox, gradient, or color for ambient light contributed to the Scene.");
            public static readonly GUIContent env_amb_top = EditorGUIUtility.TextContent("Distant Environment Lighting");
            public static readonly GUIContent env_refl_bnc = EditorGUIUtility.TextContent("Bounces|Controls how many times a reflection includes other reflections. A value of 1 results in the Scene being rendered once so mirrored reflections will be black. A value of 2 results in mirrored reflections being visible in the Scene.");
            public static readonly GUIContent env_refl_cmp = EditorGUIUtility.TextContent("Compression|Controls how Unity compresses the reflection cube map. Options are Auto, Compressed, and Uncompressed. Auto compresses the cube map if the compression format is suitable.");
            public static readonly GUIContent env_refl_int = EditorGUIUtility.TextContent("Intensity|Controls how much the skybox or custom cubemap reflection affects the Scene.");
            public static readonly GUIContent env_refl_res = EditorGUIUtility.TextContent("Resolution|Controls the resolution for the cube map assigned to the skybox material for reflection effects in the Scene.");
            public static readonly GUIContent env_refl_src = EditorGUIUtility.TextContent("Source|Specifies whether to use the skybox or a custom cube map for reflection effects in the Scene.");
            public static readonly GUIContent env_refl_top = EditorGUIUtility.TextContent("Reflections");
            public static readonly GUIContent env_skybox_mat = EditorGUIUtility.TextContent("Material|Specifies the material that is used to simulate the sky or other distant background in the Scene.");
            public static readonly GUIContent env_skybox_sun = EditorGUIUtility.TextContent("Sun Source|Specifies the directional light that is used to indicate the direction of the sun when a procedural skybox is used.  If set to None, the brightest directional light in the Scene is used to represent the sun.");
            public static readonly GUIContent env_skybox_top = EditorGUIUtility.TextContent("Skybox");
            public static readonly GUIContent[] kFullAmbientSource = new GUIContent[] { EditorGUIUtility.TextContent("Skybox"), EditorGUIUtility.TextContent("Gradient"), EditorGUIUtility.TextContent("Color") };
            public static readonly int[] kFullAmbientSourceValues;
            public static readonly GUIContent skyboxWarning = EditorGUIUtility.TextContent("Shader of this material does not support skybox rendering.");

            static Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 3;
                kFullAmbientSourceValues = numArray1;
            }

            [CompilerGenerated]
            private static GUIContent <defaultReflectionSizes>m__0(int n) => 
                new GUIContent(n.ToString());
        }
    }
}

