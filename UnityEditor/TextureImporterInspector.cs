namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Modules;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(TextureImporter)), CanEditMultipleObjects]
    internal class TextureImporterInspector : AssetImporterInspector
    {
        internal static readonly TextureImporterFormat[] kFormatsWithCompressionSettings = new TextureImporterFormat[] { TextureImporterFormat.DXT1Crunched };
        private SerializedProperty m_Alignment;
        private SerializedProperty m_AlphaIsTransparency;
        private SerializedProperty m_AlphaSource;
        private SerializedProperty m_Aniso;
        private SerializedProperty m_BorderMipMap;
        private SerializedProperty m_ConvertToNormalMap;
        private SerializedProperty m_CubemapConvolution;
        private readonly GUIContent m_EmptyContent = new GUIContent(" ");
        private SerializedProperty m_EnableMipMap;
        private SerializedProperty m_FadeOut;
        private SerializedProperty m_FilterMode;
        private readonly int[] m_FilterModeOptions = ((int[]) Enum.GetValues(typeof(UnityEngine.FilterMode)));
        private SerializedProperty m_GenerateCubemap;
        private Dictionary<TextureInspectorGUIElement, GUIMethod> m_GUIElementMethods = new Dictionary<TextureInspectorGUIElement, GUIMethod>();
        private List<TextureInspectorGUIElement> m_GUIElementsDisplayOrder = new List<TextureInspectorGUIElement>();
        private SerializedProperty m_HeightScale;
        private string m_ImportWarning = null;
        private bool m_IsPOT = false;
        private SerializedProperty m_IsReadable;
        private SerializedProperty m_MipMapFadeDistanceEnd;
        private SerializedProperty m_MipMapFadeDistanceStart;
        private SerializedProperty m_MipMapMode;
        private SerializedProperty m_NormalMapFilter;
        private SerializedProperty m_NPOTScale;
        [SerializeField]
        internal List<TextureImportPlatformSettings> m_PlatformSettings;
        private SerializedProperty m_SeamlessCubemap;
        private bool m_ShowAdvanced = false;
        private readonly AnimBool m_ShowBumpGenerationSettings = new AnimBool();
        private readonly AnimBool m_ShowCubeMapSettings = new AnimBool();
        private readonly AnimBool m_ShowGenericSpriteSettings = new AnimBool();
        private readonly AnimBool m_ShowMipMapSettings = new AnimBool();
        private SerializedProperty m_SpriteExtrude;
        private SerializedProperty m_SpriteMeshType;
        private SerializedProperty m_SpriteMode;
        private SerializedProperty m_SpritePackingTag;
        private SerializedProperty m_SpritePivot;
        private SerializedProperty m_SpritePixelsToUnits;
        private SerializedProperty m_sRGBTexture;
        private int m_TextureHeight = 0;
        private SerializedProperty m_TextureShape;
        private SerializedProperty m_TextureType;
        private TextureInspectorTypeGUIProperties[] m_TextureTypeGUIElements = new TextureInspectorTypeGUIProperties[Enum.GetValues(typeof(TextureImporterType)).Length];
        private int m_TextureWidth = 0;
        private SerializedProperty m_WrapMode;
        public static string s_DefaultPlatformName = "DefaultTexturePlatform";
        internal static string[] s_NormalFormatStringsDefault;
        internal static int[] s_NormalFormatsValueAll;
        internal static Styles s_Styles;
        internal static string[] s_TextureFormatStringsAll;
        internal static string[] s_TextureFormatStringsAndroid;
        internal static string[] s_TextureFormatStringsApplePVR;
        internal static string[] s_TextureFormatStringsDefault;
        internal static string[] s_TextureFormatStringsSingleChannel;
        internal static string[] s_TextureFormatStringsSTV;
        internal static string[] s_TextureFormatStringsTizen;
        internal static string[] s_TextureFormatStringsWebGL;
        internal static string[] s_TextureFormatStringsWiiU;
        internal static int[] s_TextureFormatsValueAll;

        private void AlphaHandlingGUI(TextureInspectorGUIElement guiElements)
        {
            int count = 0;
            int num2 = 0;
            bool flag = CountImportersWithAlpha(base.targets, out count) && CountImportersWithHDR(base.targets, out num2);
            EditorGUI.showMixedValue = this.m_AlphaSource.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int num3 = EditorGUILayout.IntPopup(s_Styles.alphaSource, this.m_AlphaSource.intValue, s_Styles.alphaSourceOptions, s_Styles.alphaSourceValues, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_AlphaSource.intValue = num3;
            }
            bool flag2 = (flag && (this.m_AlphaSource.intValue != 0)) && (num2 == 0);
            using (new EditorGUI.DisabledScope(!flag2))
            {
                this.ToggleFromInt(this.m_AlphaIsTransparency, s_Styles.alphaIsTransparency);
            }
        }

        internal override void Apply()
        {
            SpriteEditorWindow.TextureImporterApply(base.serializedObject);
            base.Apply();
            this.SyncPlatformSettings();
            foreach (TextureImportPlatformSettings settings in this.m_PlatformSettings)
            {
                settings.Apply();
            }
        }

        private void ApplySettingsToTexture()
        {
            foreach (AssetImporter importer in base.targets)
            {
                Texture tex = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture;
                if (tex != null)
                {
                    if (this.m_Aniso.intValue != -1)
                    {
                        TextureUtil.SetAnisoLevelNoDirty(tex, this.m_Aniso.intValue);
                    }
                    if (this.m_FilterMode.intValue != -1)
                    {
                        TextureUtil.SetFilterModeNoDirty(tex, (UnityEngine.FilterMode) this.m_FilterMode.intValue);
                    }
                    if (this.m_WrapMode.intValue != -1)
                    {
                        TextureUtil.SetWrapModeNoDirty(tex, (TextureWrapMode) this.m_WrapMode.intValue);
                    }
                }
            }
            SceneView.RepaintAll();
        }

        public virtual void BuildTargetList()
        {
            BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = GetBuildPlayerValidPlatforms();
            this.m_PlatformSettings = new List<TextureImportPlatformSettings>();
            this.m_PlatformSettings.Add(new TextureImportPlatformSettings(s_DefaultPlatformName, BuildTarget.StandaloneWindows, this));
            foreach (BuildPlayerWindow.BuildPlatform platform in buildPlayerValidPlatforms)
            {
                this.m_PlatformSettings.Add(new TextureImportPlatformSettings(platform.name, platform.DefaultTarget, this));
            }
        }

        internal static string[] BuildTextureStrings(int[] texFormatValues)
        {
            string[] strArray = new string[texFormatValues.Length];
            for (int i = 0; i < texFormatValues.Length; i++)
            {
                int num2 = texFormatValues[i];
                strArray[i] = " " + TextureUtil.GetTextureFormatString((TextureFormat) num2);
            }
            return strArray;
        }

        private void BumpGUI(TextureInspectorGUIElement guiElements)
        {
            EditorGUI.BeginChangeCheck();
            this.ToggleFromInt(this.m_ConvertToNormalMap, s_Styles.generateFromBump);
            this.m_ShowBumpGenerationSettings.target = this.m_ConvertToNormalMap.intValue > 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowBumpGenerationSettings.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(this.m_HeightScale, 0f, 0.3f, s_Styles.bumpiness, new GUILayoutOption[0]);
                EditorGUILayout.Popup(this.m_NormalMapFilter, s_Styles.bumpFilteringOptions, s_Styles.bumpFiltering, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUI.EndChangeCheck())
            {
                this.SyncPlatformSettings();
            }
        }

        private void CacheSerializedProperties()
        {
            this.m_AlphaSource = base.serializedObject.FindProperty("m_AlphaUsage");
            this.m_ConvertToNormalMap = base.serializedObject.FindProperty("m_ConvertToNormalMap");
            this.m_HeightScale = base.serializedObject.FindProperty("m_HeightScale");
            this.m_NormalMapFilter = base.serializedObject.FindProperty("m_NormalMapFilter");
            this.m_GenerateCubemap = base.serializedObject.FindProperty("m_GenerateCubemap");
            this.m_SeamlessCubemap = base.serializedObject.FindProperty("m_SeamlessCubemap");
            this.m_BorderMipMap = base.serializedObject.FindProperty("m_BorderMipMap");
            this.m_NPOTScale = base.serializedObject.FindProperty("m_NPOTScale");
            this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
            this.m_sRGBTexture = base.serializedObject.FindProperty("m_sRGBTexture");
            this.m_EnableMipMap = base.serializedObject.FindProperty("m_EnableMipMap");
            this.m_MipMapMode = base.serializedObject.FindProperty("m_MipMapMode");
            this.m_FadeOut = base.serializedObject.FindProperty("m_FadeOut");
            this.m_MipMapFadeDistanceStart = base.serializedObject.FindProperty("m_MipMapFadeDistanceStart");
            this.m_MipMapFadeDistanceEnd = base.serializedObject.FindProperty("m_MipMapFadeDistanceEnd");
            this.m_Aniso = base.serializedObject.FindProperty("m_TextureSettings.m_Aniso");
            this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
            this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
            this.m_CubemapConvolution = base.serializedObject.FindProperty("m_CubemapConvolution");
            this.m_SpriteMode = base.serializedObject.FindProperty("m_SpriteMode");
            this.m_SpritePackingTag = base.serializedObject.FindProperty("m_SpritePackingTag");
            this.m_SpritePixelsToUnits = base.serializedObject.FindProperty("m_SpritePixelsToUnits");
            this.m_SpriteExtrude = base.serializedObject.FindProperty("m_SpriteExtrude");
            this.m_SpriteMeshType = base.serializedObject.FindProperty("m_SpriteMeshType");
            this.m_Alignment = base.serializedObject.FindProperty("m_Alignment");
            this.m_SpritePivot = base.serializedObject.FindProperty("m_SpritePivot");
            this.m_AlphaIsTransparency = base.serializedObject.FindProperty("m_AlphaIsTransparency");
            this.m_TextureType = base.serializedObject.FindProperty("m_TextureType");
            this.m_TextureShape = base.serializedObject.FindProperty("m_TextureShape");
        }

        private void ColorSpaceGUI(TextureInspectorGUIElement guiElements)
        {
            this.ToggleFromInt(this.m_sRGBTexture, s_Styles.sRGBTexture);
        }

        private void CookieGUI(TextureInspectorGUIElement guiElements)
        {
            CookieMode spot;
            EditorGUI.BeginChangeCheck();
            if (this.m_BorderMipMap.intValue > 0)
            {
                spot = CookieMode.Spot;
            }
            else if (this.m_TextureShape.intValue == 2)
            {
                spot = CookieMode.Point;
            }
            else
            {
                spot = CookieMode.Directional;
            }
            spot = (CookieMode) EditorGUILayout.Popup(s_Styles.cookieType, (int) spot, s_Styles.cookieOptions, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.SetCookieMode(spot);
            }
            if (spot == CookieMode.Point)
            {
                this.m_TextureShape.intValue = 2;
            }
            else
            {
                this.m_TextureShape.intValue = 1;
            }
        }

        private static bool CountImportersWithAlpha(UnityEngine.Object[] importers, out int count)
        {
            try
            {
                count = 0;
                foreach (UnityEngine.Object obj2 in importers)
                {
                    if ((obj2 as TextureImporter).DoesSourceTextureHaveAlpha())
                    {
                        count++;
                    }
                }
                return true;
            }
            catch
            {
                count = importers.Length;
                return false;
            }
        }

        private static bool CountImportersWithHDR(UnityEngine.Object[] importers, out int count)
        {
            try
            {
                count = 0;
                foreach (UnityEngine.Object obj2 in importers)
                {
                    if ((obj2 as TextureImporter).IsSourceTextureHDR())
                    {
                        count++;
                    }
                }
                return true;
            }
            catch
            {
                count = importers.Length;
                return false;
            }
        }

        private void CubemapMappingGUI(TextureInspectorGUIElement guiElements)
        {
            this.m_ShowCubeMapSettings.target = this.m_TextureShape.intValue == 2;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowCubeMapSettings.faded) && (this.m_TextureShape.intValue == 2))
            {
                using (new EditorGUI.DisabledScope(!this.m_IsPOT && (this.m_NPOTScale.intValue == 0)))
                {
                    EditorGUI.showMixedValue = this.m_GenerateCubemap.hasMultipleDifferentValues || this.m_SeamlessCubemap.hasMultipleDifferentValues;
                    EditorGUI.BeginChangeCheck();
                    int num = EditorGUILayout.IntPopup(s_Styles.cubemap, this.m_GenerateCubemap.intValue, s_Styles.cubemapOptions, s_Styles.cubemapValues2, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_GenerateCubemap.intValue = num;
                    }
                    EditorGUI.indentLevel++;
                    if (this.ShouldDisplayGUIElement(guiElements, TextureInspectorGUIElement.CubeMapConvolution))
                    {
                        EditorGUILayout.IntPopup(this.m_CubemapConvolution, s_Styles.cubemapConvolutionOptions, s_Styles.cubemapConvolutionValues, s_Styles.cubemapConvolution, new GUILayoutOption[0]);
                    }
                    this.ToggleFromInt(this.m_SeamlessCubemap, s_Styles.seamlessCubemap);
                    EditorGUI.indentLevel--;
                    EditorGUI.showMixedValue = false;
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DoGUIElements(TextureInspectorGUIElement guiElements, List<TextureInspectorGUIElement> guiElementsDisplayOrder)
        {
            foreach (TextureInspectorGUIElement element in guiElementsDisplayOrder)
            {
                if (this.ShouldDisplayGUIElement(guiElements, element) && this.m_GUIElementMethods.ContainsKey(element))
                {
                    this.m_GUIElementMethods[element](guiElements);
                }
            }
        }

        private void EnumPopup(SerializedProperty property, System.Type type, GUIContent label)
        {
            EditorGUILayout.IntPopup(property, EditorGUIUtility.TempContent(Enum.GetNames(type)), Enum.GetValues(type) as int[], label, new GUILayoutOption[0]);
        }

        public static BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms() => 
            BuildPlayerWindow.GetValidPlatforms().ToArray();

        internal TextureImporterSettings GetSerializedPropertySettings() => 
            this.GetSerializedPropertySettings(new TextureImporterSettings());

        internal TextureImporterSettings GetSerializedPropertySettings(TextureImporterSettings settings)
        {
            if (!this.m_AlphaSource.hasMultipleDifferentValues)
            {
                settings.alphaSource = (TextureImporterAlphaSource) this.m_AlphaSource.intValue;
            }
            if (!this.m_ConvertToNormalMap.hasMultipleDifferentValues)
            {
                settings.convertToNormalMap = this.m_ConvertToNormalMap.intValue > 0;
            }
            if (!this.m_HeightScale.hasMultipleDifferentValues)
            {
                settings.heightmapScale = this.m_HeightScale.floatValue;
            }
            if (!this.m_NormalMapFilter.hasMultipleDifferentValues)
            {
                settings.normalMapFilter = (TextureImporterNormalFilter) this.m_NormalMapFilter.intValue;
            }
            if (!this.m_GenerateCubemap.hasMultipleDifferentValues)
            {
                settings.generateCubemap = (TextureImporterGenerateCubemap) this.m_GenerateCubemap.intValue;
            }
            if (!this.m_CubemapConvolution.hasMultipleDifferentValues)
            {
                settings.cubemapConvolution = (TextureImporterCubemapConvolution) this.m_CubemapConvolution.intValue;
            }
            if (!this.m_SeamlessCubemap.hasMultipleDifferentValues)
            {
                settings.seamlessCubemap = this.m_SeamlessCubemap.intValue > 0;
            }
            if (!this.m_BorderMipMap.hasMultipleDifferentValues)
            {
                settings.borderMipmap = this.m_BorderMipMap.intValue > 0;
            }
            if (!this.m_NPOTScale.hasMultipleDifferentValues)
            {
                settings.npotScale = (TextureImporterNPOTScale) this.m_NPOTScale.intValue;
            }
            if (!this.m_IsReadable.hasMultipleDifferentValues)
            {
                settings.readable = this.m_IsReadable.intValue > 0;
            }
            if (!this.m_sRGBTexture.hasMultipleDifferentValues)
            {
                settings.sRGBTexture = this.m_sRGBTexture.intValue > 0;
            }
            if (!this.m_EnableMipMap.hasMultipleDifferentValues)
            {
                settings.mipmapEnabled = this.m_EnableMipMap.intValue > 0;
            }
            if (!this.m_MipMapMode.hasMultipleDifferentValues)
            {
                settings.mipmapFilter = (TextureImporterMipFilter) this.m_MipMapMode.intValue;
            }
            if (!this.m_FadeOut.hasMultipleDifferentValues)
            {
                settings.fadeOut = this.m_FadeOut.intValue > 0;
            }
            if (!this.m_MipMapFadeDistanceStart.hasMultipleDifferentValues)
            {
                settings.mipmapFadeDistanceStart = this.m_MipMapFadeDistanceStart.intValue;
            }
            if (!this.m_MipMapFadeDistanceEnd.hasMultipleDifferentValues)
            {
                settings.mipmapFadeDistanceEnd = this.m_MipMapFadeDistanceEnd.intValue;
            }
            if (!this.m_SpriteMode.hasMultipleDifferentValues)
            {
                settings.spriteMode = this.m_SpriteMode.intValue;
            }
            if (!this.m_SpritePixelsToUnits.hasMultipleDifferentValues)
            {
                settings.spritePixelsPerUnit = this.m_SpritePixelsToUnits.floatValue;
            }
            if (!this.m_SpriteExtrude.hasMultipleDifferentValues)
            {
                settings.spriteExtrude = (uint) this.m_SpriteExtrude.intValue;
            }
            if (!this.m_SpriteMeshType.hasMultipleDifferentValues)
            {
                settings.spriteMeshType = (SpriteMeshType) this.m_SpriteMeshType.intValue;
            }
            if (!this.m_Alignment.hasMultipleDifferentValues)
            {
                settings.spriteAlignment = this.m_Alignment.intValue;
            }
            if (!this.m_SpritePivot.hasMultipleDifferentValues)
            {
                settings.spritePivot = this.m_SpritePivot.vector2Value;
            }
            if (!this.m_WrapMode.hasMultipleDifferentValues)
            {
                settings.wrapMode = (TextureWrapMode) this.m_WrapMode.intValue;
            }
            if (!this.m_FilterMode.hasMultipleDifferentValues)
            {
                settings.filterMode = (UnityEngine.FilterMode) this.m_FilterMode.intValue;
            }
            if (!this.m_Aniso.hasMultipleDifferentValues)
            {
                settings.aniso = this.m_Aniso.intValue;
            }
            if (!this.m_AlphaIsTransparency.hasMultipleDifferentValues)
            {
                settings.alphaIsTransparency = this.m_AlphaIsTransparency.intValue > 0;
            }
            if (!this.m_TextureType.hasMultipleDifferentValues)
            {
                settings.textureType = (TextureImporterType) this.m_TextureType.intValue;
            }
            if (!this.m_TextureShape.hasMultipleDifferentValues)
            {
                settings.textureShape = (TextureImporterShape) this.m_TextureShape.intValue;
            }
            return settings;
        }

        internal override bool HasModified()
        {
            if (base.HasModified())
            {
                return true;
            }
            foreach (TextureImportPlatformSettings settings in this.m_PlatformSettings)
            {
                if (settings.HasChanged())
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeGUI()
        {
            TextureImporterShape shape = TextureImporterShape.TextureCube | TextureImporterShape.Texture2D;
            this.m_TextureTypeGUIElements[0] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.CubeMapping | TextureInspectorGUIElement.CubeMapConvolution | TextureInspectorGUIElement.ColorSpace | TextureInspectorGUIElement.AlphaHandling, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, shape);
            this.m_TextureTypeGUIElements[1] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.CubeMapping | TextureInspectorGUIElement.NormalMap, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, shape);
            this.m_TextureTypeGUIElements[8] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.Sprite, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.ColorSpace | TextureInspectorGUIElement.AlphaHandling | TextureInspectorGUIElement.Readable, TextureImporterShape.Texture2D);
            this.m_TextureTypeGUIElements[4] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.CubeMapping | TextureInspectorGUIElement.Cookie | TextureInspectorGUIElement.AlphaHandling, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, TextureImporterShape.TextureCube | TextureImporterShape.Texture2D);
            this.m_TextureTypeGUIElements[10] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.CubeMapping | TextureInspectorGUIElement.AlphaHandling, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, shape);
            this.m_TextureTypeGUIElements[2] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.None, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.AlphaHandling | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, TextureImporterShape.Texture2D);
            this.m_TextureTypeGUIElements[7] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.None, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.AlphaHandling | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, TextureImporterShape.Texture2D);
            this.m_TextureTypeGUIElements[6] = new TextureInspectorTypeGUIProperties(TextureInspectorGUIElement.None, TextureInspectorGUIElement.MipMaps | TextureInspectorGUIElement.Readable | TextureInspectorGUIElement.PowerOfTwo, TextureImporterShape.Texture2D);
            this.m_GUIElementMethods.Clear();
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.PowerOfTwo, new GUIMethod(this.POTScaleGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.Readable, new GUIMethod(this.ReadableGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.ColorSpace, new GUIMethod(this.ColorSpaceGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.AlphaHandling, new GUIMethod(this.AlphaHandlingGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.MipMaps, new GUIMethod(this.MipMapGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.NormalMap, new GUIMethod(this.BumpGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.Sprite, new GUIMethod(this.SpriteGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.Cookie, new GUIMethod(this.CookieGUI));
            this.m_GUIElementMethods.Add(TextureInspectorGUIElement.CubeMapping, new GUIMethod(this.CubemapMappingGUI));
            this.m_GUIElementsDisplayOrder.Clear();
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.CubeMapping);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.CubeMapConvolution);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.Cookie);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.ColorSpace);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.AlphaHandling);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.NormalMap);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.Sprite);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.PowerOfTwo);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.Readable);
            this.m_GUIElementsDisplayOrder.Add(TextureInspectorGUIElement.MipMaps);
        }

        public static bool IsCompressedDXTTextureFormat(TextureImporterFormat format) => 
            ((format == TextureImporterFormat.DXT1) || (format == TextureImporterFormat.DXT5));

        internal static bool IsGLESMobileTargetPlatform(BuildTarget target) => 
            ((((target == BuildTarget.iOS) || (target == BuildTarget.tvOS)) || ((target == BuildTarget.Android) || (target == BuildTarget.Tizen))) || (target == BuildTarget.SamsungTV));

        private static bool IsPowerOfTwo(int f) => 
            ((f & (f - 1)) == 0);

        private void MipMapGUI(TextureInspectorGUIElement guiElements)
        {
            this.ToggleFromInt(this.m_EnableMipMap, s_Styles.generateMipMaps);
            this.m_ShowMipMapSettings.target = this.m_EnableMipMap.boolValue && !this.m_EnableMipMap.hasMultipleDifferentValues;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowMipMapSettings.faded))
            {
                EditorGUI.indentLevel++;
                this.ToggleFromInt(this.m_BorderMipMap, s_Styles.borderMipMaps);
                EditorGUILayout.Popup(this.m_MipMapMode, s_Styles.mipMapFilterOptions, s_Styles.mipMapFilter, new GUILayoutOption[0]);
                this.ToggleFromInt(this.m_FadeOut, s_Styles.mipmapFadeOutToggle);
                if (this.m_FadeOut.intValue > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    float intValue = this.m_MipMapFadeDistanceStart.intValue;
                    float maxValue = this.m_MipMapFadeDistanceEnd.intValue;
                    EditorGUILayout.MinMaxSlider(s_Styles.mipmapFadeOut, ref intValue, ref maxValue, 0f, 10f, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_MipMapFadeDistanceStart.intValue = Mathf.RoundToInt(intValue);
                        this.m_MipMapFadeDistanceEnd.intValue = Mathf.RoundToInt(maxValue);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        public void OnDisable()
        {
            base.OnDisable();
            EditorPrefs.SetBool("TextureImporterShowAdvanced", this.m_ShowAdvanced);
        }

        public virtual void OnEnable()
        {
            s_DefaultPlatformName = TextureImporter.defaultPlatformName;
            this.m_ShowAdvanced = EditorPrefs.GetBool("TextureImporterShowAdvanced", this.m_ShowAdvanced);
            this.CacheSerializedProperties();
            this.m_ShowBumpGenerationSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowCubeMapSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowCubeMapSettings.value = this.m_TextureShape.intValue == 2;
            this.m_ShowGenericSpriteSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowGenericSpriteSettings.value = this.m_SpriteMode.intValue != 0;
            this.m_ShowMipMapSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowMipMapSettings.value = this.m_EnableMipMap.boolValue;
            this.InitializeGUI();
            TextureImporter target = base.target as TextureImporter;
            if (target != null)
            {
                target.GetWidthAndHeight(ref this.m_TextureWidth, ref this.m_TextureHeight);
                this.m_IsPOT = IsPowerOfTwo(this.m_TextureWidth) && IsPowerOfTwo(this.m_TextureHeight);
                if (s_TextureFormatStringsApplePVR == null)
                {
                    s_TextureFormatStringsApplePVR = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueApplePVR);
                }
                if (s_TextureFormatStringsAndroid == null)
                {
                    s_TextureFormatStringsAndroid = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueAndroid);
                }
                if (s_TextureFormatStringsTizen == null)
                {
                    s_TextureFormatStringsTizen = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueTizen);
                }
                if (s_TextureFormatStringsSTV == null)
                {
                    s_TextureFormatStringsSTV = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueSTV);
                }
                if (s_TextureFormatStringsWebGL == null)
                {
                    s_TextureFormatStringsWebGL = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueWebGL);
                }
                if (s_TextureFormatStringsWiiU == null)
                {
                    s_TextureFormatStringsWiiU = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueWiiU);
                }
                if (s_TextureFormatStringsDefault == null)
                {
                    s_TextureFormatStringsDefault = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueDefault);
                }
                if (s_NormalFormatStringsDefault == null)
                {
                    s_NormalFormatStringsDefault = BuildTextureStrings(TextureImportPlatformSettings.kNormalFormatsValueDefault);
                }
                if (s_TextureFormatStringsSingleChannel == null)
                {
                    s_TextureFormatStringsSingleChannel = BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueSingleChannel);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            bool enabled = GUI.enabled;
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_TextureType.hasMultipleDifferentValues;
            int index = EditorGUILayout.IntPopup(s_Styles.textureTypeTitle, this.m_TextureType.intValue, s_Styles.textureTypeOptions, s_Styles.textureTypeValues, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck() && (this.m_TextureType.intValue != index))
            {
                this.m_TextureType.intValue = index;
                TextureImporterSettings serializedPropertySettings = this.GetSerializedPropertySettings();
                serializedPropertySettings.ApplyTextureType((TextureImporterType) this.m_TextureType.intValue);
                this.SetSerializedPropertySettings(serializedPropertySettings);
                this.SyncPlatformSettings();
                this.ApplySettingsToTexture();
            }
            int[] array = s_Styles.textureShapeValuesDictionnary[this.m_TextureTypeGUIElements[index].shapeCaps];
            using (new EditorGUI.DisabledScope((array.Length == 1) || (this.m_TextureType.intValue == 4)))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = this.m_TextureShape.hasMultipleDifferentValues;
                int num2 = EditorGUILayout.IntPopup(s_Styles.textureShape, this.m_TextureShape.intValue, s_Styles.textureShapeOptionsDictionnary[this.m_TextureTypeGUIElements[index].shapeCaps], s_Styles.textureShapeValuesDictionnary[this.m_TextureTypeGUIElements[index].shapeCaps], new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_TextureShape.intValue = num2;
                }
            }
            if (Array.IndexOf<int>(array, this.m_TextureShape.intValue) == -1)
            {
                this.m_TextureShape.intValue = array[0];
            }
            EditorGUILayout.Space();
            if (!this.m_TextureType.hasMultipleDifferentValues)
            {
                this.DoGUIElements(this.m_TextureTypeGUIElements[index].commonElements, this.m_GUIElementsDisplayOrder);
                if (this.m_TextureTypeGUIElements[index].advancedElements != TextureInspectorGUIElement.None)
                {
                    EditorGUILayout.Space();
                    this.m_ShowAdvanced = EditorGUILayout.Foldout(this.m_ShowAdvanced, s_Styles.showAdvanced, true);
                    if (this.m_ShowAdvanced)
                    {
                        EditorGUI.indentLevel++;
                        this.DoGUIElements(this.m_TextureTypeGUIElements[index].advancedElements, this.m_GUIElementsDisplayOrder);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUILayout.Space();
            this.TextureSettingsGUI();
            this.ShowPlatformSpecificSettings();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            base.ApplyRevertGUI();
            GUILayout.EndHorizontal();
            this.UpdateImportWarning();
            if (this.m_ImportWarning != null)
            {
                EditorGUILayout.HelpBox(this.m_ImportWarning, MessageType.Warning);
            }
            GUI.enabled = enabled;
        }

        private void POTScaleGUI(TextureInspectorGUIElement guiElements)
        {
            using (new EditorGUI.DisabledScope(this.m_IsPOT))
            {
                this.EnumPopup(this.m_NPOTScale, typeof(TextureImporterNPOTScale), s_Styles.npot);
            }
        }

        private void ReadableGUI(TextureInspectorGUIElement guiElements)
        {
            this.ToggleFromInt(this.m_IsReadable, s_Styles.readWrite);
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.CacheSerializedProperties();
            this.BuildTargetList();
            this.ApplySettingsToTexture();
            SelectMainAssets(base.targets);
        }

        public static void SelectMainAssets(UnityEngine.Object[] targets)
        {
            ArrayList list = new ArrayList();
            foreach (AssetImporter importer in targets)
            {
                Texture texture = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture;
                if (texture != null)
                {
                    list.Add(texture);
                }
            }
            if (list.Count > 0)
            {
                Selection.objects = list.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[];
            }
        }

        private void SetCookieMode(CookieMode cm)
        {
            if (cm != CookieMode.Spot)
            {
                if (cm == CookieMode.Point)
                {
                    this.m_BorderMipMap.intValue = 0;
                    this.m_WrapMode.intValue = 1;
                    this.m_GenerateCubemap.intValue = 1;
                    this.m_TextureShape.intValue = 2;
                }
                else if (cm == CookieMode.Directional)
                {
                    this.m_BorderMipMap.intValue = 0;
                    this.m_WrapMode.intValue = 0;
                    this.m_GenerateCubemap.intValue = 6;
                    this.m_TextureShape.intValue = 1;
                }
            }
            else
            {
                this.m_BorderMipMap.intValue = 1;
                this.m_WrapMode.intValue = 1;
                this.m_GenerateCubemap.intValue = 6;
                this.m_TextureShape.intValue = 1;
            }
        }

        private void SetSerializedPropertySettings(TextureImporterSettings settings)
        {
            this.m_AlphaSource.intValue = (int) settings.alphaSource;
            this.m_ConvertToNormalMap.intValue = !settings.convertToNormalMap ? 0 : 1;
            this.m_HeightScale.floatValue = settings.heightmapScale;
            this.m_NormalMapFilter.intValue = (int) settings.normalMapFilter;
            this.m_GenerateCubemap.intValue = (int) settings.generateCubemap;
            this.m_CubemapConvolution.intValue = (int) settings.cubemapConvolution;
            this.m_SeamlessCubemap.intValue = !settings.seamlessCubemap ? 0 : 1;
            this.m_BorderMipMap.intValue = !settings.borderMipmap ? 0 : 1;
            this.m_NPOTScale.intValue = (int) settings.npotScale;
            this.m_IsReadable.intValue = !settings.readable ? 0 : 1;
            this.m_EnableMipMap.intValue = !settings.mipmapEnabled ? 0 : 1;
            this.m_sRGBTexture.intValue = !settings.sRGBTexture ? 0 : 1;
            this.m_MipMapMode.intValue = (int) settings.mipmapFilter;
            this.m_FadeOut.intValue = !settings.fadeOut ? 0 : 1;
            this.m_MipMapFadeDistanceStart.intValue = settings.mipmapFadeDistanceStart;
            this.m_MipMapFadeDistanceEnd.intValue = settings.mipmapFadeDistanceEnd;
            this.m_SpriteMode.intValue = settings.spriteMode;
            this.m_SpritePixelsToUnits.floatValue = settings.spritePixelsPerUnit;
            this.m_SpriteExtrude.intValue = (int) settings.spriteExtrude;
            this.m_SpriteMeshType.intValue = (int) settings.spriteMeshType;
            this.m_Alignment.intValue = settings.spriteAlignment;
            this.m_WrapMode.intValue = (int) settings.wrapMode;
            this.m_FilterMode.intValue = (int) settings.filterMode;
            this.m_Aniso.intValue = settings.aniso;
            this.m_AlphaIsTransparency.intValue = !settings.alphaIsTransparency ? 0 : 1;
            this.m_TextureType.intValue = (int) settings.textureType;
            this.m_TextureShape.intValue = (int) settings.textureShape;
        }

        private bool ShouldDisplayGUIElement(TextureInspectorGUIElement guiElements, TextureInspectorGUIElement guiElement) => 
            ((guiElements & guiElement) == guiElement);

        protected void ShowPlatformSpecificSettings()
        {
            BuildPlayerWindow.BuildPlatform[] platforms = GetBuildPlayerValidPlatforms().ToArray<BuildPlayerWindow.BuildPlatform>();
            GUILayout.Space(10f);
            int index = EditorGUILayout.BeginPlatformGrouping(platforms, s_Styles.defaultPlatform);
            TextureImportPlatformSettings platformSettings = this.m_PlatformSettings[index + 1];
            if (!platformSettings.isDefault)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = platformSettings.overriddenIsDifferent;
                bool overridden = EditorGUILayout.ToggleLeft("Override for " + platforms[index].title.text, platformSettings.overridden, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    platformSettings.SetOverriddenForAll(overridden);
                    this.SyncPlatformSettings();
                }
            }
            bool disabled = !platformSettings.isDefault && !platformSettings.allAreOverridden;
            using (new EditorGUI.DisabledScope(disabled))
            {
                ModuleManager.GetTextureImportSettingsExtension(platformSettings.m_Target).ShowImportSettings(this, platformSettings);
                this.SyncPlatformSettings();
            }
            EditorGUILayout.EndPlatformGrouping();
        }

        private void SpriteGUI(TextureInspectorGUIElement guiElements)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.IntPopup(this.m_SpriteMode, s_Styles.spriteModeOptions, new int[] { 1, 2, 3 }, s_Styles.spriteMode, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                GUIUtility.keyboardControl = 0;
            }
            EditorGUI.indentLevel++;
            this.m_ShowGenericSpriteSettings.target = this.m_SpriteMode.intValue != 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowGenericSpriteSettings.faded))
            {
                EditorGUILayout.PropertyField(this.m_SpritePackingTag, s_Styles.spritePackingTag, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_SpritePixelsToUnits, s_Styles.spritePixelsPerUnit, new GUILayoutOption[0]);
                int[] optionValues = new int[2];
                optionValues[1] = 1;
                EditorGUILayout.IntPopup(this.m_SpriteMeshType, s_Styles.spriteMeshTypeOptions, optionValues, s_Styles.spriteMeshType, new GUILayoutOption[0]);
                EditorGUILayout.IntSlider(this.m_SpriteExtrude, 0, 0x20, s_Styles.spriteExtrude, new GUILayoutOption[0]);
                if (this.m_SpriteMode.intValue == 1)
                {
                    EditorGUILayout.Popup(this.m_Alignment, s_Styles.spriteAlignmentOptions, s_Styles.spriteAlignment, new GUILayoutOption[0]);
                    if (this.m_Alignment.intValue == 9)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_SpritePivot, this.m_EmptyContent, new GUILayoutOption[0]);
                        GUILayout.EndHorizontal();
                    }
                }
                using (new EditorGUI.DisabledScope(base.targets.Length != 1))
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Sprite Editor", new GUILayoutOption[0]))
                    {
                        if (this.HasModified())
                        {
                            string message = ("Unapplied import settings for '" + ((TextureImporter) base.target).assetPath + "'.\n") + "Apply and continue to sprite editor or cancel.";
                            if (EditorUtility.DisplayDialog("Unapplied import settings", message, "Apply", "Cancel"))
                            {
                                base.ApplyAndImport();
                                SpriteEditorWindow.GetWindow();
                                GUIUtility.ExitGUI();
                            }
                        }
                        else
                        {
                            SpriteEditorWindow.GetWindow();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
        }

        private void SyncPlatformSettings()
        {
            foreach (TextureImportPlatformSettings settings in this.m_PlatformSettings)
            {
                settings.Sync();
            }
        }

        private void TextureSettingsGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_WrapMode.hasMultipleDifferentValues;
            TextureWrapMode intValue = (TextureWrapMode) this.m_WrapMode.intValue;
            if (intValue == ~TextureWrapMode.Repeat)
            {
                intValue = TextureWrapMode.Repeat;
            }
            intValue = (TextureWrapMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Wrap Mode"), intValue, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_WrapMode.intValue = (int) intValue;
            }
            if (((this.m_NPOTScale.intValue == 0) && (intValue == TextureWrapMode.Repeat)) && !ShaderUtil.hardwareSupportsFullNPOT)
            {
                bool flag = false;
                foreach (UnityEngine.Object obj2 in base.targets)
                {
                    int width = -1;
                    int height = -1;
                    ((TextureImporter) obj2).GetWidthAndHeight(ref width, ref height);
                    if (!Mathf.IsPowerOfTwo(width) || !Mathf.IsPowerOfTwo(height))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Graphics device doesn't support Repeat wrap mode on NPOT textures. Falling back to Clamp.").text, MessageType.Warning, true);
                }
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_FilterMode.hasMultipleDifferentValues;
            UnityEngine.FilterMode trilinear = (UnityEngine.FilterMode) this.m_FilterMode.intValue;
            if (trilinear == ~UnityEngine.FilterMode.Point)
            {
                if ((this.m_FadeOut.intValue > 0) || (this.m_ConvertToNormalMap.intValue > 0))
                {
                    trilinear = UnityEngine.FilterMode.Trilinear;
                }
                else
                {
                    trilinear = UnityEngine.FilterMode.Bilinear;
                }
            }
            trilinear = (UnityEngine.FilterMode) EditorGUILayout.IntPopup(s_Styles.filterMode, (int) trilinear, s_Styles.filterModeOptions, this.m_FilterModeOptions, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_FilterMode.intValue = (int) trilinear;
            }
            bool flag2 = ((this.m_FilterMode.intValue != 0) && (this.m_EnableMipMap.intValue > 0)) && (this.m_TextureShape.intValue != 2);
            using (new EditorGUI.DisabledScope(!flag2))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = this.m_Aniso.hasMultipleDifferentValues;
                int num4 = this.m_Aniso.intValue;
                if (num4 == -1)
                {
                    num4 = 1;
                }
                num4 = EditorGUILayout.IntSlider("Aniso Level", num4, 0, 0x10, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_Aniso.intValue = num4;
                }
                TextureInspector.DoAnisoGlobalSettingNote(num4);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ApplySettingsToTexture();
            }
        }

        private void ToggleFromInt(SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            int num = !EditorGUILayout.Toggle(label, property.intValue > 0, new GUILayoutOption[0]) ? 0 : 1;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = num;
            }
        }

        private void UpdateImportWarning()
        {
            this.m_ImportWarning = (base.target as TextureImporter)?.GetImportWarnings();
        }

        internal static int[] NormalFormatsValueAll
        {
            get
            {
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = GetBuildPlayerValidPlatforms();
                foreach (BuildPlayerWindow.BuildPlatform platform in buildPlayerValidPlatforms)
                {
                    BuildTarget defaultTarget = platform.DefaultTarget;
                    if (defaultTarget != BuildTarget.iOS)
                    {
                        if (defaultTarget != BuildTarget.Android)
                        {
                            if (defaultTarget == BuildTarget.Tizen)
                            {
                                goto Label_007C;
                            }
                            if (defaultTarget == BuildTarget.tvOS)
                            {
                                goto Label_0073;
                            }
                        }
                        else
                        {
                            flag2 = true;
                            flag3 = true;
                            flag = true;
                            flag4 = true;
                            flag5 = true;
                        }
                    }
                    else
                    {
                        flag2 = true;
                        flag = true;
                    }
                    continue;
                Label_0073:
                    flag2 = true;
                    flag = true;
                    continue;
                Label_007C:
                    flag = true;
                }
                List<int> list = new List<int>();
                int[] collection = new int[] { 12 };
                list.AddRange(collection);
                if (flag2)
                {
                    list.AddRange(new int[] { 30, 0x1f, 0x20, 0x21 });
                }
                if (flag3)
                {
                    int[] numArray2 = new int[] { 0x23, 0x24 };
                    list.AddRange(numArray2);
                }
                if (flag)
                {
                    int[] numArray3 = new int[] { 0x22 };
                    list.AddRange(numArray3);
                }
                if (flag4)
                {
                    list.AddRange(new int[] { 0x2d, 0x2e, 0x2f });
                }
                if (flag5)
                {
                    list.AddRange(new int[] { 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b });
                }
                list.AddRange(new int[] { 2, 13, 4, 0x1d });
                s_NormalFormatsValueAll = list.ToArray();
                return s_NormalFormatsValueAll;
            }
        }

        internal override bool showImportedObject =>
            false;

        internal SpriteImportMode spriteImportMode =>
            ((SpriteImportMode) this.m_SpriteMode.intValue);

        internal static int[] TextureFormatsValueAll
        {
            get
            {
                if (s_TextureFormatsValueAll == null)
                {
                    bool flag = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    bool flag5 = false;
                    BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = GetBuildPlayerValidPlatforms();
                    foreach (BuildPlayerWindow.BuildPlatform platform in buildPlayerValidPlatforms)
                    {
                        switch (platform.DefaultTarget)
                        {
                            case BuildTarget.SamsungTV:
                            {
                                flag = true;
                                continue;
                            }
                            case BuildTarget.tvOS:
                            {
                                flag2 = true;
                                flag5 = true;
                                continue;
                            }
                            case BuildTarget.iOS:
                                flag2 = true;
                                break;

                            case BuildTarget.Android:
                                flag2 = true;
                                flag = true;
                                flag3 = true;
                                flag4 = true;
                                flag5 = true;
                                break;

                            case BuildTarget.Tizen:
                                goto Label_00A3;
                        }
                        continue;
                    Label_00A3:
                        flag = true;
                    }
                    List<int> list = new List<int>();
                    int[] collection = new int[] { 10, 12 };
                    list.AddRange(collection);
                    if (flag)
                    {
                        list.Add(0x22);
                    }
                    if (flag2)
                    {
                        list.AddRange(new int[] { 30, 0x1f, 0x20, 0x21 });
                    }
                    if (flag3)
                    {
                        int[] numArray2 = new int[] { 0x23, 0x24 };
                        list.AddRange(numArray2);
                    }
                    if (flag4)
                    {
                        list.AddRange(new int[] { 0x2d, 0x2e, 0x2f });
                    }
                    if (flag5)
                    {
                        list.AddRange(new int[] { 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b });
                    }
                    list.AddRange(new int[] { 7, 2, 13, 3, 1, 5, 4, 0x11, 0x18, 0x19, 0x1c, 0x1d });
                    s_TextureFormatsValueAll = list.ToArray();
                }
                return s_TextureFormatsValueAll;
            }
        }

        internal TextureImporterType textureType
        {
            get
            {
                if (this.m_TextureType.hasMultipleDifferentValues)
                {
                    return TextureImporterType.Default;
                }
                return (TextureImporterType) this.m_TextureType.intValue;
            }
        }

        internal bool textureTypeHasMultipleDifferentValues =>
            this.m_TextureType.hasMultipleDifferentValues;

        private enum CookieMode
        {
            Spot,
            Directional,
            Point
        }

        private delegate void GUIMethod(TextureImporterInspector.TextureInspectorGUIElement guiElements);

        internal class Styles
        {
            public readonly GUIContent alphaIsTransparency;
            public readonly GUIContent alphaSource;
            public readonly GUIContent[] alphaSourceOptions;
            public readonly int[] alphaSourceValues;
            public readonly GUIContent borderMipMaps;
            public readonly GUIContent bumpFiltering = EditorGUIUtility.TextContent("Filtering");
            public readonly GUIContent[] bumpFilteringOptions = new GUIContent[] { EditorGUIUtility.TextContent("Sharp"), EditorGUIUtility.TextContent("Smooth") };
            public readonly GUIContent bumpiness = EditorGUIUtility.TextContent("Bumpiness");
            public readonly GUIContent compressionQuality;
            public readonly GUIContent compressionQualitySlider;
            public readonly GUIContent[] cookieOptions = new GUIContent[] { EditorGUIUtility.TextContent("Spotlight"), EditorGUIUtility.TextContent("Directional"), EditorGUIUtility.TextContent("Point") };
            public readonly GUIContent cookieType = EditorGUIUtility.TextContent("Light Type");
            public readonly GUIContent crunchedCompression;
            public readonly GUIContent cubemap = EditorGUIUtility.TextContent("Mapping");
            public readonly GUIContent cubemapConvolution = EditorGUIUtility.TextContent("Convolution Type");
            public readonly GUIContent[] cubemapConvolutionOptions = new GUIContent[] { EditorGUIUtility.TextContent("None"), EditorGUIUtility.TextContent("Specular (Glossy Reflection)|Convolve cubemap for specular reflections with varying smoothness (Glossy Reflections)."), EditorGUIUtility.TextContent("Diffuse (Irradiance)|Convolve cubemap for diffuse-only reflection (Irradiance Cubemap).") };
            public readonly int[] cubemapConvolutionValues;
            public readonly GUIContent[] cubemapOptions = new GUIContent[] { EditorGUIUtility.TextContent("Auto"), EditorGUIUtility.TextContent("6 Frames Layout (Cubic Environment)|Texture contains 6 images arranged in one of the standard cubemap layouts - cross or sequence (+x,-x, +y, -y, +z, -z). Texture can be in vertical or horizontal orientation."), EditorGUIUtility.TextContent("Latitude-Longitude Layout (Cylindrical)|Texture contains an image of a ball unwrapped such that latitude and longitude are mapped to horizontal and vertical dimensions (as on a globe)."), EditorGUIUtility.TextContent("Mirrored Ball (Spheremap)|Texture contains an image of a mirrored ball.") };
            public readonly int[] cubemapValues2 = new int[] { 6, 5, 2, 1 };
            public readonly GUIContent defaultPlatform;
            public readonly GUIContent etc1Compression;
            public readonly GUIContent filterMode = EditorGUIUtility.TextContent("Filter Mode");
            public readonly GUIContent[] filterModeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Point (no filter)"), EditorGUIUtility.TextContent("Bilinear"), EditorGUIUtility.TextContent("Trilinear") };
            public readonly GUIContent generateCubemap;
            public readonly GUIContent generateFromBump = EditorGUIUtility.TextContent("Create from Grayscale|The grayscale of the image is used as a heightmap for generating the normal map.");
            public readonly GUIContent generateMipMaps;
            public readonly GUIContent mipmapFadeOut;
            public readonly GUIContent mipmapFadeOutToggle;
            public readonly GUIContent mipMapFilter;
            public readonly GUIContent[] mipMapFilterOptions;
            public readonly GUIContent[] mobileCompressionQualityOptions;
            public readonly GUIContent npot;
            public readonly GUIContent readWrite;
            public readonly GUIContent seamlessCubemap;
            public readonly GUIContent showAdvanced;
            public readonly GUIContent spriteAlignment;
            public readonly GUIContent[] spriteAlignmentOptions;
            public readonly GUIContent spriteExtrude;
            public readonly GUIContent spriteMeshType;
            public readonly GUIContent[] spriteMeshTypeOptions;
            public readonly GUIContent spriteMode;
            public readonly GUIContent[] spriteModeOptions;
            public readonly GUIContent spritePackingTag;
            public readonly GUIContent spritePixelsPerUnit;
            public readonly GUIContent sRGBTexture;
            public readonly GUIContent textureFormat;
            public readonly GUIContent textureShape = EditorGUIUtility.TextContent("Texture Shape|What shape is this texture?");
            private readonly GUIContent textureShape2D = EditorGUIUtility.TextContent("2D|Texture is 2D.");
            private readonly GUIContent textureShapeCube = EditorGUIUtility.TextContent("Cube|Texture is a Cubemap.");
            public readonly Dictionary<TextureImporterShape, GUIContent[]> textureShapeOptionsDictionnary = new Dictionary<TextureImporterShape, GUIContent[]>();
            public readonly Dictionary<TextureImporterShape, int[]> textureShapeValuesDictionnary = new Dictionary<TextureImporterShape, int[]>();
            public readonly GUIContent[] textureTypeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Default|Texture is a normal image such as a diffuse texture or other."), EditorGUIUtility.TextContent("Normal map|Texture is a bump or normal map."), EditorGUIUtility.TextContent("Editor GUI and Legacy GUI|Texture is used for a GUI element."), EditorGUIUtility.TextContent("Sprite (2D and UI)|Texture is used for a sprite."), EditorGUIUtility.TextContent("Cursor|Texture is used for a cursor."), EditorGUIUtility.TextContent("Cookie|Texture is a cookie you put on a light."), EditorGUIUtility.TextContent("Lightmap|Texture is a lightmap."), EditorGUIUtility.TextContent("Single Channel|Texture is a one component texture.") };
            public readonly GUIContent textureTypeTitle = EditorGUIUtility.TextContent("Texture Type|What will this texture be used for?");
            public readonly int[] textureTypeValues = new int[] { 0, 1, 2, 8, 7, 4, 6, 10 };

            public Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 2;
                this.cubemapConvolutionValues = numArray1;
                this.seamlessCubemap = EditorGUIUtility.TextContent("Fixup Edge Seams|Enable if this texture is used for glossy reflections.");
                this.textureFormat = EditorGUIUtility.TextContent("Format");
                this.defaultPlatform = EditorGUIUtility.TextContent("Default");
                this.mipmapFadeOutToggle = EditorGUIUtility.TextContent("Fadeout Mip Maps");
                this.mipmapFadeOut = EditorGUIUtility.TextContent("Fade Range");
                this.readWrite = EditorGUIUtility.TextContent("Read/Write Enabled|Enable to be able to access the raw pixel data from code.");
                this.alphaSource = EditorGUIUtility.TextContent("Alpha Source|How is the alpha generated for the imported texture.");
                this.alphaSourceOptions = new GUIContent[] { EditorGUIUtility.TextContent("None|No Alpha will be used."), EditorGUIUtility.TextContent("Input Texture Alpha|Use Alpha from the input texture if one is provided."), EditorGUIUtility.TextContent("From Gray Scale|Generate Alpha from image gray scale.") };
                int[] numArray4 = new int[3];
                numArray4[1] = 1;
                numArray4[2] = 2;
                this.alphaSourceValues = numArray4;
                this.generateMipMaps = EditorGUIUtility.TextContent("Generate Mip Maps");
                this.sRGBTexture = EditorGUIUtility.TextContent("sRGB (Color Texture)|Texture content is stored in gamma space. Non-HDR color textures should enable this flag (except if used for IMGUI).");
                this.borderMipMaps = EditorGUIUtility.TextContent("Border Mip Maps");
                this.mipMapFilter = EditorGUIUtility.TextContent("Mip Map Filtering");
                this.mipMapFilterOptions = new GUIContent[] { EditorGUIUtility.TextContent("Box"), EditorGUIUtility.TextContent("Kaiser") };
                this.npot = EditorGUIUtility.TextContent("Non Power of 2|How non-power-of-two textures are scaled on import.");
                this.generateCubemap = EditorGUIUtility.TextContent("Generate Cubemap");
                this.compressionQuality = EditorGUIUtility.TextContent("Compressor Quality");
                this.compressionQualitySlider = EditorGUIUtility.TextContent("Compressor Quality|Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best)");
                this.mobileCompressionQualityOptions = new GUIContent[] { EditorGUIUtility.TextContent("Fast"), EditorGUIUtility.TextContent("Normal"), EditorGUIUtility.TextContent("Best") };
                this.spriteMode = EditorGUIUtility.TextContent("Sprite Mode");
                this.spriteModeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Single"), EditorGUIUtility.TextContent("Multiple"), EditorGUIUtility.TextContent("Polygon") };
                this.spriteMeshTypeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Full Rect"), EditorGUIUtility.TextContent("Tight") };
                this.spritePackingTag = EditorGUIUtility.TextContent("Packing Tag|Tag for the Sprite Packing system.");
                this.spritePixelsPerUnit = EditorGUIUtility.TextContent("Pixels Per Unit|How many pixels in the sprite correspond to one unit in the world.");
                this.spriteExtrude = EditorGUIUtility.TextContent("Extrude Edges|How much empty area to leave around the sprite in the generated mesh.");
                this.spriteMeshType = EditorGUIUtility.TextContent("Mesh Type|Type of sprite mesh to generate.");
                this.spriteAlignment = EditorGUIUtility.TextContent("Pivot|Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");
                this.spriteAlignmentOptions = new GUIContent[] { EditorGUIUtility.TextContent("Center"), EditorGUIUtility.TextContent("Top Left"), EditorGUIUtility.TextContent("Top"), EditorGUIUtility.TextContent("Top Right"), EditorGUIUtility.TextContent("Left"), EditorGUIUtility.TextContent("Right"), EditorGUIUtility.TextContent("Bottom Left"), EditorGUIUtility.TextContent("Bottom"), EditorGUIUtility.TextContent("Bottom Right"), EditorGUIUtility.TextContent("Custom") };
                this.alphaIsTransparency = EditorGUIUtility.TextContent("Alpha Is Transparency|If the provided alpha channel is transparency, enable this to pre-filter the color to avoid texture filtering artifacts. This is not supported for HDR textures.");
                this.etc1Compression = EditorGUIUtility.TextContent("Compress using ETC1 (split alpha channel)|Alpha for this texture will be preserved by splitting the alpha channel to another texture, and both resulting textures will be compressed using ETC1.");
                this.crunchedCompression = EditorGUIUtility.TextContent("Use Crunch Compression|Texture is crunch-compressed to save space on disk when applicable.");
                this.showAdvanced = EditorGUIUtility.TextContent("Advanced|Show advanced settings.");
                GUIContent[] contentArray = new GUIContent[] { this.textureShape2D };
                GUIContent[] contentArray2 = new GUIContent[] { this.textureShapeCube };
                GUIContent[] contentArray3 = new GUIContent[] { this.textureShape2D, this.textureShapeCube };
                this.textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D, contentArray);
                this.textureShapeOptionsDictionnary.Add(TextureImporterShape.TextureCube, contentArray2);
                this.textureShapeOptionsDictionnary.Add(TextureImporterShape.TextureCube | TextureImporterShape.Texture2D, contentArray3);
                int[] numArray = new int[] { 1 };
                int[] numArray2 = new int[] { 2 };
                int[] numArray3 = new int[] { 1, 2 };
                this.textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D, numArray);
                this.textureShapeValuesDictionnary.Add(TextureImporterShape.TextureCube, numArray2);
                this.textureShapeValuesDictionnary.Add(TextureImporterShape.TextureCube | TextureImporterShape.Texture2D, numArray3);
            }
        }

        [Flags]
        private enum TextureInspectorGUIElement
        {
            AlphaHandling = 4,
            ColorSpace = 8,
            Cookie = 0x80,
            CubeMapConvolution = 0x100,
            CubeMapping = 0x200,
            MipMaps = 0x10,
            None = 0,
            NormalMap = 0x20,
            PowerOfTwo = 1,
            Readable = 2,
            Sprite = 0x40
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TextureInspectorTypeGUIProperties
        {
            public TextureImporterInspector.TextureInspectorGUIElement commonElements;
            public TextureImporterInspector.TextureInspectorGUIElement advancedElements;
            public TextureImporterShape shapeCaps;
            public TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement _commonElements, TextureImporterInspector.TextureInspectorGUIElement _advancedElements, TextureImporterShape _shapeCaps)
            {
                this.commonElements = _commonElements;
                this.advancedElements = _advancedElements;
                this.shapeCaps = _shapeCaps;
            }
        }
    }
}

