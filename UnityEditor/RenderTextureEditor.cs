namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(RenderTexture)), CanEditMultipleObjects]
    internal class RenderTextureEditor : TextureInspector
    {
        private SerializedProperty m_AntiAliasing;
        private SerializedProperty m_AutoGeneratesMipmaps;
        private SerializedProperty m_ColorFormat;
        private SerializedProperty m_Depth;
        private SerializedProperty m_DepthFormat;
        private SerializedProperty m_Dimension;
        private SerializedProperty m_EnableMipmaps;
        private SerializedProperty m_Height;
        private SerializedProperty m_sRGB;
        private SerializedProperty m_Width;
        private const GUIElements s_AllGUIElements = (GUIElements.RenderTargetAAGUI | GUIElements.RenderTargetDepthGUI);
        private static Styles s_Styles = null;

        public override string GetInfoString()
        {
            RenderTexture target = base.target as RenderTexture;
            string str = target.width + "x" + target.height;
            if (target.dimension == TextureDimension.Tex3D)
            {
                str = str + "x" + target.volumeDepth;
            }
            if (!target.isPowerOfTwo)
            {
                str = str + "(NPOT)";
            }
            if (QualitySettings.desiredColorSpace == ColorSpace.Linear)
            {
                bool flag = IsHDRFormat(target.format);
                bool flag2 = target.sRGB && !flag;
                str = str + " " + (!flag2 ? "Linear" : "sRGB");
            }
            return ((str + "  " + target.format) + "  " + EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySizeLong(target)));
        }

        public static bool IsHDRFormat(RenderTextureFormat format) => 
            (((((format == RenderTextureFormat.ARGBHalf) || (format == RenderTextureFormat.RGB111110Float)) || ((format == RenderTextureFormat.RGFloat) || (format == RenderTextureFormat.ARGBFloat))) || (((format == RenderTextureFormat.ARGBFloat) || (format == RenderTextureFormat.RFloat)) || (format == RenderTextureFormat.RGHalf))) || (format == RenderTextureFormat.RHalf));

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Width = base.serializedObject.FindProperty("m_Width");
            this.m_Height = base.serializedObject.FindProperty("m_Height");
            this.m_Depth = base.serializedObject.FindProperty("m_VolumeDepth");
            this.m_AntiAliasing = base.serializedObject.FindProperty("m_AntiAliasing");
            this.m_ColorFormat = base.serializedObject.FindProperty("m_ColorFormat");
            this.m_DepthFormat = base.serializedObject.FindProperty("m_DepthFormat");
            this.m_EnableMipmaps = base.serializedObject.FindProperty("m_MipMap");
            this.m_AutoGeneratesMipmaps = base.serializedObject.FindProperty("m_GenerateMips");
            this.m_Dimension = base.serializedObject.FindProperty("m_Dimension");
            this.m_sRGB = base.serializedObject.FindProperty("m_SRGB");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.OnRenderTextureGUI(GUIElements.RenderTargetAAGUI | GUIElements.RenderTargetDepthGUI);
            base.serializedObject.ApplyModifiedProperties();
        }

        protected void OnRenderTextureGUI(GUIElements guiElements)
        {
            GUI.changed = false;
            bool disabled = this.m_Dimension.intValue == 3;
            EditorGUILayout.IntPopup(this.m_Dimension, styles.dimensionStrings, styles.dimensionValues, styles.dimension, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(styles.size, EditorStyles.popup);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
            EditorGUILayout.DelayedIntField(this.m_Width, GUIContent.none, options);
            GUILayout.Label(styles.cross, new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
            EditorGUILayout.DelayedIntField(this.m_Height, GUIContent.none, optionArray2);
            if (disabled)
            {
                GUILayout.Label(styles.cross, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
                EditorGUILayout.DelayedIntField(this.m_Depth, GUIContent.none, optionArray3);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if ((guiElements & GUIElements.RenderTargetAAGUI) != GUIElements.RenderTargetNoneGUI)
            {
                EditorGUILayout.IntPopup(this.m_AntiAliasing, styles.renderTextureAntiAliasing, styles.renderTextureAntiAliasingValues, styles.antiAliasing, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_ColorFormat, styles.colorFormat, new GUILayoutOption[0]);
            if ((guiElements & GUIElements.RenderTargetDepthGUI) != GUIElements.RenderTargetNoneGUI)
            {
                EditorGUILayout.PropertyField(this.m_DepthFormat, styles.depthBuffer, new GUILayoutOption[0]);
            }
            bool flag2 = IsHDRFormat((RenderTextureFormat) this.m_ColorFormat.intValue);
            using (new EditorGUI.DisabledScope(flag2))
            {
                EditorGUILayout.PropertyField(this.m_sRGB, styles.sRGBTexture, new GUILayoutOption[0]);
            }
            using (new EditorGUI.DisabledScope(disabled))
            {
                EditorGUILayout.PropertyField(this.m_EnableMipmaps, styles.enableMipmaps, new GUILayoutOption[0]);
                using (new EditorGUI.DisabledScope(!this.m_EnableMipmaps.boolValue))
                {
                    EditorGUILayout.PropertyField(this.m_AutoGeneratesMipmaps, styles.autoGeneratesMipmaps, new GUILayoutOption[0]);
                }
            }
            if (disabled)
            {
                EditorGUILayout.HelpBox("3D RenderTextures do not support Mip Maps.", MessageType.Info);
            }
            RenderTexture target = base.target as RenderTexture;
            if (GUI.changed && (target != null))
            {
                target.Release();
            }
            EditorGUILayout.Space();
            base.DoWrapModePopup();
            base.DoFilterModePopup();
            using (new EditorGUI.DisabledScope(this.RenderTextureHasDepth()))
            {
                base.DoAnisoLevelSlider();
            }
            if (this.RenderTextureHasDepth())
            {
                base.m_Aniso.intValue = 0;
                EditorGUILayout.HelpBox("RenderTextures with depth must have an Aniso Level of 0.", MessageType.Info);
            }
        }

        private bool RenderTextureHasDepth() => 
            (TextureUtil.IsDepthRTFormat((RenderTextureFormat) this.m_ColorFormat.enumValueIndex) || (this.m_DepthFormat.enumValueIndex != 0));

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        [Flags]
        protected enum GUIElements
        {
            RenderTargetAAGUI = 4,
            RenderTargetDepthGUI = 2,
            RenderTargetNoneGUI = 0
        }

        private class Styles
        {
            public readonly GUIContent antiAliasing = EditorGUIUtility.TextContent("Anti-Aliasing|Number of anti-aliasing samples.");
            public readonly GUIContent autoGeneratesMipmaps = EditorGUIUtility.TextContent("Auto generate Mip Maps|This render texture automatically generate its Mip Maps.");
            public readonly GUIContent colorFormat = EditorGUIUtility.TextContent("Color Format|Format of the color buffer.");
            public readonly GUIContent cross = EditorGUIUtility.TextContent("x");
            public readonly GUIContent depthBuffer = EditorGUIUtility.TextContent("Depth Buffer|Format of the depth buffer.");
            public readonly GUIContent dimension = EditorGUIUtility.TextContent("Dimension|Is the texture 2D, Cube or 3D?");
            public readonly GUIContent[] dimensionStrings = new GUIContent[] { EditorGUIUtility.TextContent("2D"), EditorGUIUtility.TextContent("Cube"), EditorGUIUtility.TextContent("3D") };
            public readonly int[] dimensionValues = new int[] { 2, 4, 3 };
            public readonly GUIContent enableMipmaps = EditorGUIUtility.TextContent("Enable Mip Maps|This render texture will have Mip Maps.");
            public readonly GUIContent[] renderTextureAntiAliasing = new GUIContent[] { new GUIContent("None"), new GUIContent("2 samples"), new GUIContent("4 samples"), new GUIContent("8 samples") };
            public readonly int[] renderTextureAntiAliasingValues = new int[] { 1, 2, 4, 8 };
            public readonly GUIContent size = EditorGUIUtility.TextContent("Size|Size of the render texture in pixels.");
            public readonly GUIContent sRGBTexture = EditorGUIUtility.TextContent("sRGB (Color RenderTexture)|RenderTexture content is stored in gamma space. Non-HDR color textures should enable this flag.");
        }
    }
}

