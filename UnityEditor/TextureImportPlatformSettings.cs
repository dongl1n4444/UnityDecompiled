namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    internal class TextureImportPlatformSettings
    {
        [CompilerGenerated]
        private static Func<Object, TextureImporter> <>f__am$cache0;
        public static readonly int[] kNormalFormatsValueDefault = new int[] { 12, 0x1d, 2, 4 };
        public static readonly int[] kTextureFormatsValueAndroid = new int[] { 
            10, 12, 0x1c, 0x1d, 0x22, 0x2d, 0x2e, 0x2f, 30, 0x1f, 0x20, 0x21, 0x23, 0x24, 0x30, 0x31,
            50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 7, 3, 1, 13, 4
        };
        public static readonly int[] kTextureFormatsValueApplePVR = new int[] { 
            30, 0x1f, 0x20, 0x21, 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b,
            7, 3, 1, 13, 4
        };
        public static readonly int[] kTextureFormatsValueDefault = new int[] { 10, 12, 0x1c, 0x1d, 7, 3, 1, 2, 4, 0x11, 0x1a, 0x1b, 0x18, 0x19 };
        public static readonly int[] kTextureFormatsValueSingleChannel = new int[] { 1, 0x1a };
        public static readonly int[] kTextureFormatsValueSTV = new int[] { 0x22, 7, 3, 1, 13, 4 };
        public static readonly int[] kTextureFormatsValueTizen = new int[] { 0x22, 7, 3, 1, 13, 4 };
        public static readonly int[] kTextureFormatsValueWebGL = new int[] { 10, 12, 0x1c, 0x1d, 7, 3, 1, 2, 4 };
        public static readonly int[] kTextureFormatsValueWiiU = new int[] { 10, 12, 7, 1, 4, 13 };
        [SerializeField]
        private bool m_AlphaSplitIsDifferent = false;
        [SerializeField]
        private bool m_CompressionQualityIsDifferent = false;
        [SerializeField]
        private bool m_CrunchedCompressionIsDifferent = false;
        [SerializeField]
        private bool m_HasChanged = false;
        [SerializeField]
        private TextureImporter[] m_Importers;
        [SerializeField]
        private TextureImporterInspector m_Inspector;
        [SerializeField]
        private bool m_MaxTextureSizeIsDifferent = false;
        [SerializeField]
        private bool m_OverriddenIsDifferent = false;
        [SerializeField]
        private TextureImporterPlatformSettings m_PlatformSettings = new TextureImporterPlatformSettings();
        [SerializeField]
        public BuildTarget m_Target;
        [SerializeField]
        private bool m_TextureCompressionIsDifferent = false;
        [SerializeField]
        private bool m_TextureFormatIsDifferent = false;

        public TextureImportPlatformSettings(string name, BuildTarget target, TextureImporterInspector inspector)
        {
            this.m_PlatformSettings.name = name;
            this.m_Target = target;
            this.m_Inspector = inspector;
            this.m_PlatformSettings.overridden = false;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<Object, TextureImporter>(null, (IntPtr) <TextureImportPlatformSettings>m__0);
            }
            this.m_Importers = Enumerable.ToArray<TextureImporter>(Enumerable.Select<Object, TextureImporter>(inspector.targets, <>f__am$cache0));
            for (int i = 0; i < this.importers.Length; i++)
            {
                TextureImporterPlatformSettings platformTextureSettings = this.importers[i].GetPlatformTextureSettings(name);
                if (i == 0)
                {
                    this.m_PlatformSettings = platformTextureSettings;
                }
                else
                {
                    if (platformTextureSettings.overridden != this.m_PlatformSettings.overridden)
                    {
                        this.m_OverriddenIsDifferent = true;
                    }
                    if (platformTextureSettings.format != this.m_PlatformSettings.format)
                    {
                        this.m_TextureFormatIsDifferent = true;
                    }
                    if (platformTextureSettings.maxTextureSize != this.m_PlatformSettings.maxTextureSize)
                    {
                        this.m_MaxTextureSizeIsDifferent = true;
                    }
                    if (platformTextureSettings.textureCompression != this.m_PlatformSettings.textureCompression)
                    {
                        this.m_TextureCompressionIsDifferent = true;
                    }
                    if (platformTextureSettings.compressionQuality != this.m_PlatformSettings.compressionQuality)
                    {
                        this.m_CompressionQualityIsDifferent = true;
                    }
                    if (platformTextureSettings.crunchedCompression != this.m_PlatformSettings.crunchedCompression)
                    {
                        this.m_CrunchedCompressionIsDifferent = true;
                    }
                    if (platformTextureSettings.allowsAlphaSplitting != this.m_PlatformSettings.allowsAlphaSplitting)
                    {
                        this.m_AlphaSplitIsDifferent = true;
                    }
                }
            }
            this.Sync();
        }

        [CompilerGenerated]
        private static TextureImporter <TextureImportPlatformSettings>m__0(Object x)
        {
            return (x as TextureImporter);
        }

        public void Apply()
        {
            for (int i = 0; i < this.importers.Length; i++)
            {
                TextureImporter importer = this.importers[i];
                TextureImporterPlatformSettings platformTextureSettings = importer.GetPlatformTextureSettings(this.name);
                if (!this.m_OverriddenIsDifferent)
                {
                    platformTextureSettings.overridden = this.m_PlatformSettings.overridden;
                }
                if (!this.m_TextureFormatIsDifferent)
                {
                    platformTextureSettings.format = this.m_PlatformSettings.format;
                }
                if (!this.m_MaxTextureSizeIsDifferent)
                {
                    platformTextureSettings.maxTextureSize = this.m_PlatformSettings.maxTextureSize;
                }
                if (!this.m_TextureCompressionIsDifferent)
                {
                    platformTextureSettings.textureCompression = this.m_PlatformSettings.textureCompression;
                }
                if (!this.m_CompressionQualityIsDifferent)
                {
                    platformTextureSettings.compressionQuality = this.m_PlatformSettings.compressionQuality;
                }
                if (!this.m_CrunchedCompressionIsDifferent)
                {
                    platformTextureSettings.crunchedCompression = this.m_PlatformSettings.crunchedCompression;
                }
                if (!this.m_AlphaSplitIsDifferent)
                {
                    platformTextureSettings.allowsAlphaSplitting = this.m_PlatformSettings.allowsAlphaSplitting;
                }
                importer.SetPlatformTextureSettings(platformTextureSettings);
            }
        }

        private bool GetOverridden(TextureImporter importer)
        {
            if (!this.m_OverriddenIsDifferent)
            {
                return this.overridden;
            }
            return importer.GetPlatformTextureSettings(this.name).overridden;
        }

        public TextureImporterSettings GetSettings(TextureImporter importer)
        {
            TextureImporterSettings dest = new TextureImporterSettings();
            importer.ReadTextureSettings(dest);
            this.m_Inspector.GetSerializedPropertySettings(dest);
            return dest;
        }

        public virtual bool HasChanged()
        {
            return this.m_HasChanged;
        }

        public void SetAllowsAlphaSplitForAll(bool value)
        {
            this.m_PlatformSettings.allowsAlphaSplitting = value;
            this.m_AlphaSplitIsDifferent = false;
            this.SetChanged();
        }

        public virtual void SetChanged()
        {
            this.m_HasChanged = true;
        }

        public void SetCompressionQualityForAll(int quality)
        {
            this.m_PlatformSettings.compressionQuality = quality;
            this.m_CompressionQualityIsDifferent = false;
            this.SetChanged();
        }

        public void SetCrunchedCompressionForAll(bool crunched)
        {
            this.m_PlatformSettings.crunchedCompression = crunched;
            this.m_CrunchedCompressionIsDifferent = false;
            this.SetChanged();
        }

        public void SetMaxTextureSizeForAll(int maxTextureSize)
        {
            this.m_PlatformSettings.maxTextureSize = maxTextureSize;
            this.m_MaxTextureSizeIsDifferent = false;
            this.SetChanged();
        }

        public void SetOverriddenForAll(bool overridden)
        {
            this.m_PlatformSettings.overridden = overridden;
            this.m_OverriddenIsDifferent = false;
            this.SetChanged();
        }

        public void SetTextureCompressionForAll(TextureImporterCompression textureCompression)
        {
            this.m_PlatformSettings.textureCompression = textureCompression;
            this.m_TextureCompressionIsDifferent = false;
            this.m_HasChanged = true;
        }

        public void SetTextureFormatForAll(TextureImporterFormat format)
        {
            this.m_PlatformSettings.format = format;
            this.m_TextureFormatIsDifferent = false;
            this.SetChanged();
        }

        public bool SupportsFormat(TextureImporterFormat format, TextureImporter importer)
        {
            int[] kTextureFormatsValueSTV;
            TextureImporterSettings settings = this.GetSettings(importer);
            BuildTarget target = this.m_Target;
            switch (target)
            {
                case BuildTarget.SamsungTV:
                    kTextureFormatsValueSTV = TextureImportPlatformSettings.kTextureFormatsValueSTV;
                    break;

                case BuildTarget.WiiU:
                    kTextureFormatsValueSTV = kTextureFormatsValueWiiU;
                    break;

                case BuildTarget.tvOS:
                case BuildTarget.iOS:
                    kTextureFormatsValueSTV = kTextureFormatsValueApplePVR;
                    break;

                default:
                    if (target == BuildTarget.Android)
                    {
                        kTextureFormatsValueSTV = kTextureFormatsValueAndroid;
                    }
                    else if (target == BuildTarget.Tizen)
                    {
                        kTextureFormatsValueSTV = kTextureFormatsValueTizen;
                    }
                    else
                    {
                        kTextureFormatsValueSTV = (settings.textureType != TextureImporterType.NormalMap) ? kTextureFormatsValueDefault : kNormalFormatsValueDefault;
                    }
                    break;
            }
            return kTextureFormatsValueSTV.Contains((int) format);
        }

        public void Sync()
        {
            if (!this.isDefault && (!this.overridden || this.m_OverriddenIsDifferent))
            {
                TextureImportPlatformSettings settings = this.m_Inspector.m_PlatformSettings[0];
                this.m_PlatformSettings.maxTextureSize = settings.maxTextureSize;
                this.m_MaxTextureSizeIsDifferent = settings.m_MaxTextureSizeIsDifferent;
                this.m_PlatformSettings.textureCompression = settings.textureCompression;
                this.m_TextureCompressionIsDifferent = settings.m_TextureCompressionIsDifferent;
                this.m_PlatformSettings.format = settings.format;
                this.m_TextureFormatIsDifferent = settings.m_TextureFormatIsDifferent;
                this.m_PlatformSettings.compressionQuality = settings.compressionQuality;
                this.m_CompressionQualityIsDifferent = settings.m_CompressionQualityIsDifferent;
                this.m_PlatformSettings.crunchedCompression = settings.crunchedCompression;
                this.m_CrunchedCompressionIsDifferent = settings.m_CrunchedCompressionIsDifferent;
                this.m_PlatformSettings.allowsAlphaSplitting = settings.allowsAlphaSplitting;
                this.m_AlphaSplitIsDifferent = settings.m_AlphaSplitIsDifferent;
            }
            if ((this.overridden || this.m_OverriddenIsDifferent) && (this.m_PlatformSettings.format < ~TextureImporterFormat.Automatic))
            {
                this.m_PlatformSettings.format = TextureImporter.FormatFromTextureParameters(this.GetSettings(this.importers[0]), this.m_PlatformSettings, this.importers[0].DoesSourceTextureHaveAlpha(), this.importers[0].IsSourceTextureHDR(), this.m_Target);
                this.m_TextureFormatIsDifferent = false;
                for (int i = 1; i < this.importers.Length; i++)
                {
                    TextureImporter importer = this.importers[i];
                    if (TextureImporter.FormatFromTextureParameters(this.GetSettings(importer), this.m_PlatformSettings, importer.DoesSourceTextureHaveAlpha(), importer.IsSourceTextureHDR(), this.m_Target) != this.m_PlatformSettings.format)
                    {
                        this.m_TextureFormatIsDifferent = true;
                    }
                }
            }
        }

        public bool allAreOverridden
        {
            get
            {
                return (this.isDefault || (this.overridden && !this.m_OverriddenIsDifferent));
            }
        }

        public bool allowsAlphaSplitIsDifferent
        {
            get
            {
                return this.m_AlphaSplitIsDifferent;
            }
        }

        public bool allowsAlphaSplitting
        {
            get
            {
                return this.m_PlatformSettings.allowsAlphaSplitting;
            }
        }

        public int compressionQuality
        {
            get
            {
                return this.m_PlatformSettings.compressionQuality;
            }
        }

        public bool compressionQualityIsDifferent
        {
            get
            {
                return this.m_CompressionQualityIsDifferent;
            }
        }

        public bool crunchedCompression
        {
            get
            {
                return this.m_PlatformSettings.crunchedCompression;
            }
        }

        public bool crunchedCompressionIsDifferent
        {
            get
            {
                return this.m_CrunchedCompressionIsDifferent;
            }
        }

        public TextureImporterFormat format
        {
            get
            {
                return this.m_PlatformSettings.format;
            }
        }

        public TextureImporter[] importers
        {
            get
            {
                return this.m_Importers;
            }
        }

        public bool isDefault
        {
            get
            {
                return (this.name == TextureImporterInspector.s_DefaultPlatformName);
            }
        }

        public int maxTextureSize
        {
            get
            {
                return this.m_PlatformSettings.maxTextureSize;
            }
        }

        public bool maxTextureSizeIsDifferent
        {
            get
            {
                return this.m_MaxTextureSizeIsDifferent;
            }
        }

        public string name
        {
            get
            {
                return this.m_PlatformSettings.name;
            }
        }

        public bool overridden
        {
            get
            {
                return this.m_PlatformSettings.overridden;
            }
        }

        public bool overriddenIsDifferent
        {
            get
            {
                return this.m_OverriddenIsDifferent;
            }
        }

        public TextureImporterPlatformSettings platformTextureSettings
        {
            get
            {
                return this.m_PlatformSettings;
            }
        }

        public TextureImporterCompression textureCompression
        {
            get
            {
                return this.m_PlatformSettings.textureCompression;
            }
        }

        public bool textureCompressionIsDifferent
        {
            get
            {
                return this.m_TextureCompressionIsDifferent;
            }
        }

        public bool textureFormatIsDifferent
        {
            get
            {
                return this.m_TextureFormatIsDifferent;
            }
        }
    }
}

