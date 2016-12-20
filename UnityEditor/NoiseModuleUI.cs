namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class NoiseModuleUI : ModuleUI
    {
        private const int k_PreviewSize = 0x60;
        private SerializedProperty m_Damping;
        private SerializedProperty m_Frequency;
        private SerializedProperty m_OctaveMultiplier;
        private SerializedProperty m_Octaves;
        private SerializedProperty m_OctaveScale;
        private SerializedProperty m_Quality;
        private SerializedProperty m_RemapEnabled;
        private SerializedMinMaxCurve m_RemapX;
        private SerializedMinMaxCurve m_RemapY;
        private SerializedMinMaxCurve m_RemapZ;
        private SerializedMinMaxCurve m_ScrollSpeed;
        private SerializedProperty m_SeparateAxes;
        private SerializedMinMaxCurve m_StrengthX;
        private SerializedMinMaxCurve m_StrengthY;
        private SerializedMinMaxCurve m_StrengthZ;
        private GUIStyle previewTextureStyle;
        private static Texture2D s_PreviewTexture;
        private static bool s_PreviewTextureDirty = true;
        private static Texts s_Texts;

        public NoiseModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "NoiseModule", displayName)
        {
            base.m_ToolTip = "Add noise/turbulence to particle movement.";
        }

        protected override void Init()
        {
            if (this.m_StrengthX == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_StrengthX = new SerializedMinMaxCurve(this, s_Texts.x, "strength", ModuleUI.kUseSignedRange);
                this.m_StrengthY = new SerializedMinMaxCurve(this, s_Texts.y, "strengthY", ModuleUI.kUseSignedRange);
                this.m_StrengthZ = new SerializedMinMaxCurve(this, s_Texts.z, "strengthZ", ModuleUI.kUseSignedRange);
                this.m_SeparateAxes = base.GetProperty("separateAxes");
                this.m_Damping = base.GetProperty("damping");
                this.m_Frequency = base.GetProperty("frequency");
                this.m_Octaves = base.GetProperty("octaves");
                this.m_OctaveMultiplier = base.GetProperty("octaveMultiplier");
                this.m_OctaveScale = base.GetProperty("octaveScale");
                this.m_Quality = base.GetProperty("quality");
                this.m_ScrollSpeed = new SerializedMinMaxCurve(this, s_Texts.scrollSpeed, "scrollSpeed", ModuleUI.kUseSignedRange);
                this.m_ScrollSpeed.m_AllowRandom = false;
                this.m_RemapX = new SerializedMinMaxCurve(this, s_Texts.x, "remap", ModuleUI.kUseSignedRange);
                this.m_RemapY = new SerializedMinMaxCurve(this, s_Texts.y, "remapY", ModuleUI.kUseSignedRange);
                this.m_RemapZ = new SerializedMinMaxCurve(this, s_Texts.z, "remapZ", ModuleUI.kUseSignedRange);
                this.m_RemapX.m_AllowRandom = false;
                this.m_RemapY.m_AllowRandom = false;
                this.m_RemapZ.m_AllowRandom = false;
                this.m_RemapX.m_AllowConstant = false;
                this.m_RemapY.m_AllowConstant = false;
                this.m_RemapZ.m_AllowConstant = false;
                this.m_RemapEnabled = base.GetProperty("remapEnabled");
                if (s_PreviewTexture == null)
                {
                    s_PreviewTexture = new Texture2D(0x60, 0x60, TextureFormat.RGBA32, false, true);
                    s_PreviewTexture.name = "ParticleNoisePreview";
                    s_PreviewTexture.filterMode = FilterMode.Bilinear;
                    s_PreviewTexture.hideFlags = HideFlags.HideAndDontSave;
                    s_Texts.previewTexture.image = s_PreviewTexture;
                }
                s_PreviewTextureDirty = true;
                this.previewTextureStyle = new GUIStyle(ParticleSystemStyles.Get().label);
                this.previewTextureStyle.alignment = TextAnchor.LowerCenter;
                this.previewTextureStyle.imagePosition = ImagePosition.ImageAbove;
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            if (s_PreviewTextureDirty)
            {
                base.m_ParticleSystemUI.m_ParticleSystem.GenerateNoisePreviewTexture(s_PreviewTexture);
                s_PreviewTextureDirty = false;
            }
            bool flag = base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner is ParticleSystemInspector;
            if (flag)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
            }
            EditorGUI.BeginChangeCheck();
            bool flag2 = ModuleUI.GUIToggle(s_Texts.separateAxes, this.m_SeparateAxes, new GUILayoutOption[0]);
            bool flag3 = EditorGUI.EndChangeCheck();
            EditorGUI.BeginChangeCheck();
            if (flag3)
            {
                if (flag2)
                {
                    this.m_StrengthX.RemoveCurveFromEditor();
                    this.m_RemapX.RemoveCurveFromEditor();
                }
                else
                {
                    this.m_StrengthX.RemoveCurveFromEditor();
                    this.m_StrengthY.RemoveCurveFromEditor();
                    this.m_StrengthZ.RemoveCurveFromEditor();
                    this.m_RemapX.RemoveCurveFromEditor();
                    this.m_RemapY.RemoveCurveFromEditor();
                    this.m_RemapZ.RemoveCurveFromEditor();
                }
            }
            MinMaxCurveState state = this.m_StrengthX.state;
            this.m_StrengthY.state = state;
            this.m_StrengthZ.state = state;
            state = this.m_RemapX.state;
            this.m_RemapY.state = state;
            this.m_RemapZ.state = state;
            if (flag2)
            {
                this.m_StrengthX.m_DisplayName = s_Texts.x;
                base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_StrengthX, s_Texts.y, this.m_StrengthY, s_Texts.z, this.m_StrengthZ, null, new GUILayoutOption[0]);
            }
            else
            {
                this.m_StrengthX.m_DisplayName = s_Texts.strength;
                ModuleUI.GUIMinMaxCurve(s_Texts.strength, this.m_StrengthX, new GUILayoutOption[0]);
            }
            ModuleUI.GUIFloat(s_Texts.frequency, this.m_Frequency, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.scrollSpeed, this.m_ScrollSpeed, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.damping, this.m_Damping, new GUILayoutOption[0]);
            int num = ModuleUI.GUIInt(s_Texts.octaves, this.m_Octaves, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(num == 1))
            {
                ModuleUI.GUIFloat(s_Texts.octaveMultiplier, this.m_OctaveMultiplier, new GUILayoutOption[0]);
                ModuleUI.GUIFloat(s_Texts.octaveScale, this.m_OctaveScale, new GUILayoutOption[0]);
            }
            ModuleUI.GUIPopup(s_Texts.quality, this.m_Quality, s_Texts.qualityDropdown, new GUILayoutOption[0]);
            bool flag4 = ModuleUI.GUIToggle(s_Texts.remap, this.m_RemapEnabled, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(!flag4))
            {
                if (flag2)
                {
                    this.m_RemapX.m_DisplayName = s_Texts.x;
                    base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_RemapX, s_Texts.y, this.m_RemapY, s_Texts.z, this.m_RemapZ, null, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_RemapX.m_DisplayName = s_Texts.remap;
                    ModuleUI.GUIMinMaxCurve(s_Texts.remapCurve, this.m_RemapX, new GUILayoutOption[0]);
                }
            }
            if (flag)
            {
                GUILayout.EndVertical();
            }
            if ((EditorGUI.EndChangeCheck() || (this.m_ScrollSpeed.scalar.floatValue > 0f)) || (flag4 || flag3))
            {
                s_PreviewTextureDirty = true;
                base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) };
            GUILayout.Label(s_Texts.previewTexture, this.previewTextureStyle, options);
            if (flag)
            {
                GUILayout.EndHorizontal();
            }
        }

        private class Texts
        {
            public GUIContent damping = EditorGUIUtility.TextContent("Damping|If enabled, strength is proportional to frequency.");
            public GUIContent frequency = EditorGUIUtility.TextContent("Frequency|Low values create soft, smooth noise, and high values create rapidly changing noise.");
            public GUIContent octaveMultiplier = EditorGUIUtility.TextContent("Octave Multiplier|When combining each octave, scale the intensity by this amount.");
            public GUIContent octaves = EditorGUIUtility.TextContent("Octaves|Layers of noise that combine to produce final noise (Adding octaves increases the performance cost substantially!)");
            public GUIContent octaveScale = EditorGUIUtility.TextContent("Octave Scale|When combining each octave, zoom in by this amount.");
            public GUIContent previewTexture = EditorGUIUtility.TextContent("Preview");
            public GUIContent quality = EditorGUIUtility.TextContent("Quality|Generate 1D, 2D or 3D noise.");
            public string[] qualityDropdown = new string[] { "Low (1D)", "Medium (2D)", "High (3D)" };
            public GUIContent remap = EditorGUIUtility.TextContent("Remap|Remap the final noise values into a new range.");
            public GUIContent remapCurve = EditorGUIUtility.TextContent("Remap Curve");
            public GUIContent scrollSpeed = EditorGUIUtility.TextContent("Scroll Speed|Scroll the noise map over the particle system.");
            public GUIContent separateAxes = EditorGUIUtility.TextContent("Separate Axes|If enabled, you can control the noise separately for each axis.");
            public GUIContent strength = EditorGUIUtility.TextContent("Strength|How strong the overall noise effect is.");
            public GUIContent x = EditorGUIUtility.TextContent("X");
            public GUIContent y = EditorGUIUtility.TextContent("Y");
            public GUIContent z = EditorGUIUtility.TextContent("Z");
        }
    }
}

