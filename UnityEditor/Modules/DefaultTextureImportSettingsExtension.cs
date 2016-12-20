namespace UnityEditor.Modules
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    internal class DefaultTextureImportSettingsExtension : ITextureImportSettingsExtension
    {
        private static readonly string[] kMaxTextureSizeStrings = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192" };
        private static readonly int[] kMaxTextureSizeValues = new int[] { 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000 };
        private static readonly GUIContent kTextureCompression = EditorGUIUtility.TextContent("Compression|How will this texture be compressed?");
        private static readonly GUIContent[] kTextureCompressionOptions = new GUIContent[] { EditorGUIUtility.TextContent("None|Texture is not compressed."), EditorGUIUtility.TextContent("Low Quality|Texture compressed with low quality but high performance, high compression format."), EditorGUIUtility.TextContent("Normal Quality|Texture is compressed with a standard format."), EditorGUIUtility.TextContent("High Quality|Texture compressed with a high quality format.") };
        private static readonly int[] kTextureCompressionValues = new int[] { 0, 3, 1, 2 };
        private static readonly GUIContent maxSize = EditorGUIUtility.TextContent("Max Size|Textures larger than this will be scaled down.");

        private int EditCompressionQuality(BuildTarget target, int compression)
        {
            if ((((target == BuildTarget.iOS) || (target == BuildTarget.tvOS)) || ((target == BuildTarget.Android) || (target == BuildTarget.Tizen))) || (target == BuildTarget.SamsungTV))
            {
                int selectedIndex = 1;
                if (compression == 0)
                {
                    selectedIndex = 0;
                }
                else if (compression == 100)
                {
                    selectedIndex = 2;
                }
                switch (EditorGUILayout.Popup(TextureImporterInspector.s_Styles.compressionQuality, selectedIndex, TextureImporterInspector.s_Styles.mobileCompressionQualityOptions, new GUILayoutOption[0]))
                {
                    case 0:
                        return 0;

                    case 1:
                        return 50;

                    case 2:
                        return 100;
                }
                return 50;
            }
            compression = EditorGUILayout.IntSlider(TextureImporterInspector.s_Styles.compressionQualitySlider, compression, 0, 100, new GUILayoutOption[0]);
            return compression;
        }

        public virtual void ShowImportSettings(Editor baseEditor, TextureImportPlatformSettings platformSettings)
        {
            TextureImporterInspector inspector = baseEditor as TextureImporterInspector;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = platformSettings.overriddenIsDifferent || platformSettings.maxTextureSizeIsDifferent;
            int maxTextureSize = EditorGUILayout.IntPopup(maxSize.text, platformSettings.maxTextureSize, kMaxTextureSizeStrings, kMaxTextureSizeValues, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                platformSettings.SetMaxTextureSizeForAll(maxTextureSize);
            }
            using (new EditorGUI.DisabledScope(platformSettings.overridden && !platformSettings.isDefault))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = (platformSettings.overriddenIsDifferent || platformSettings.textureCompressionIsDifferent) || (platformSettings.overridden && !platformSettings.isDefault);
                TextureImporterCompression textureCompression = (TextureImporterCompression) EditorGUILayout.IntPopup(kTextureCompression, (int) platformSettings.textureCompression, kTextureCompressionOptions, kTextureCompressionValues, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    platformSettings.SetTextureCompressionForAll(textureCompression);
                }
            }
            int[] second = null;
            string[] strArray = null;
            bool flag = false;
            int selectedValue = 0;
            for (int i = 0; i < inspector.targets.Length; i++)
            {
                TextureImporter importer = inspector.targets[i] as TextureImporter;
                TextureImporterSettings settings = platformSettings.GetSettings(importer);
                TextureImporterType type = !inspector.textureTypeHasMultipleDifferentValues ? inspector.textureType : settings.textureType;
                int format = (int) platformSettings.format;
                int[] first = null;
                string[] strArray2 = null;
                if (platformSettings.isDefault)
                {
                    format = -1;
                    first = new int[] { -1 };
                    strArray2 = new string[] { "Auto" };
                }
                else if (!platformSettings.overridden)
                {
                    format = (int) TextureImporter.FormatFromTextureParameters(settings, platformSettings.platformTextureSettings, importer.DoesSourceTextureHaveAlpha(), importer.IsSourceTextureHDR(), platformSettings.m_Target);
                    first = new int[] { format };
                    strArray2 = new string[] { TextureUtil.GetTextureFormatString((TextureFormat) format) };
                }
                else
                {
                    switch (type)
                    {
                        case TextureImporterType.Cookie:
                        case TextureImporterType.SingleChannel:
                            first = TextureImportPlatformSettings.kTextureFormatsValueSingleChannel;
                            strArray2 = TextureImporterInspector.s_TextureFormatStringsSingleChannel;
                            goto Label_0304;
                    }
                    if (TextureImporterInspector.IsGLESMobileTargetPlatform(platformSettings.m_Target))
                    {
                        if ((platformSettings.m_Target == BuildTarget.iOS) || (platformSettings.m_Target == BuildTarget.tvOS))
                        {
                            first = TextureImportPlatformSettings.kTextureFormatsValueApplePVR;
                            strArray2 = TextureImporterInspector.s_TextureFormatStringsApplePVR;
                        }
                        else if (platformSettings.m_Target == BuildTarget.SamsungTV)
                        {
                            first = TextureImportPlatformSettings.kTextureFormatsValueSTV;
                            strArray2 = TextureImporterInspector.s_TextureFormatStringsSTV;
                        }
                        else
                        {
                            first = TextureImportPlatformSettings.kTextureFormatsValueAndroid;
                            strArray2 = TextureImporterInspector.s_TextureFormatStringsAndroid;
                        }
                    }
                    else if (type == TextureImporterType.NormalMap)
                    {
                        first = TextureImportPlatformSettings.kNormalFormatsValueDefault;
                        strArray2 = TextureImporterInspector.s_NormalFormatStringsDefault;
                    }
                    else if (platformSettings.m_Target == BuildTarget.WebGL)
                    {
                        first = TextureImportPlatformSettings.kTextureFormatsValueWebGL;
                        strArray2 = TextureImporterInspector.s_TextureFormatStringsWebGL;
                    }
                    else if (platformSettings.m_Target == BuildTarget.WiiU)
                    {
                        first = TextureImportPlatformSettings.kTextureFormatsValueWiiU;
                        strArray2 = TextureImporterInspector.s_TextureFormatStringsWiiU;
                    }
                    else
                    {
                        first = TextureImportPlatformSettings.kTextureFormatsValueDefault;
                        strArray2 = TextureImporterInspector.s_TextureFormatStringsDefault;
                    }
                }
            Label_0304:
                if (i == 0)
                {
                    second = first;
                    strArray = strArray2;
                    selectedValue = format;
                }
                else if (!Enumerable.SequenceEqual<int>(first, second) || !Enumerable.SequenceEqual<string>(strArray2, strArray))
                {
                    flag = true;
                    break;
                }
            }
            using (new EditorGUI.DisabledScope(flag || (strArray.Length == 1)))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = flag || platformSettings.textureFormatIsDifferent;
                selectedValue = EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.textureFormat, selectedValue, EditorGUIUtility.TempContent(strArray), second, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    platformSettings.SetTextureFormatForAll((TextureImporterFormat) selectedValue);
                }
                if (Array.IndexOf<int>(second, selectedValue) == -1)
                {
                    platformSettings.SetTextureFormatForAll((TextureImporterFormat) second[0]);
                }
            }
            if ((platformSettings.isDefault && (platformSettings.textureCompression != TextureImporterCompression.Uncompressed)) || (platformSettings.allAreOverridden && TextureImporterInspector.IsCompressedDXTTextureFormat((TextureImporterFormat) selectedValue)))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = platformSettings.overriddenIsDifferent || platformSettings.crunchedCompressionIsDifferent;
                bool crunched = EditorGUILayout.Toggle(TextureImporterInspector.s_Styles.crunchedCompression, platformSettings.crunchedCompression, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    platformSettings.SetCrunchedCompressionForAll(crunched);
                }
            }
            if (((platformSettings.crunchedCompression && !platformSettings.crunchedCompressionIsDifferent) && (((platformSettings.textureCompression != TextureImporterCompression.Uncompressed) || (selectedValue == 10)) || (selectedValue == 12))) || (!platformSettings.textureFormatIsDifferent && ArrayUtility.Contains<TextureImporterFormat>(TextureImporterInspector.kFormatsWithCompressionSettings, (TextureImporterFormat) selectedValue)))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = platformSettings.overriddenIsDifferent || platformSettings.compressionQualityIsDifferent;
                int quality = this.EditCompressionQuality(platformSettings.m_Target, platformSettings.compressionQuality);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    platformSettings.SetCompressionQualityForAll(quality);
                }
            }
            bool flag3 = TextureImporter.IsETC1SupportedByBuildTarget(BuildPipeline.GetBuildTargetByName(platformSettings.name));
            bool flag4 = inspector.spriteImportMode != SpriteImportMode.None;
            bool flag5 = TextureImporter.IsTextureFormatETC1Compression((TextureFormat) selectedValue);
            if ((flag3 && flag4) && flag5)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = platformSettings.overriddenIsDifferent || platformSettings.allowsAlphaSplitIsDifferent;
                bool flag6 = GUILayout.Toggle(platformSettings.allowsAlphaSplitting, TextureImporterInspector.s_Styles.etc1Compression, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    platformSettings.SetAllowsAlphaSplitForAll(flag6);
                }
            }
        }
    }
}

