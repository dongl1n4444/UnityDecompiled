namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Stores settings of a TextureImporter.</para>
    /// </summary>
    [Serializable]
    public sealed class TextureImporterSettings
    {
        [SerializeField]
        private int m_Alignment;
        [SerializeField]
        private int m_AlphaIsTransparency;
        [SerializeField]
        private int m_AlphaSource;
        [SerializeField]
        private int m_Aniso;
        [SerializeField]
        private int m_BorderMipMap;
        [SerializeField]
        private int m_CompressionQuality;
        [SerializeField]
        private int m_CompressionQualitySet;
        [SerializeField]
        private int m_ConvertToNormalMap;
        [SerializeField]
        private int m_CubemapConvolution;
        [SerializeField]
        private float m_CubemapConvolutionExponent;
        [SerializeField]
        private int m_CubemapConvolutionSteps;
        [SerializeField]
        private int m_EnableMipMap;
        [SerializeField]
        private int m_FadeOut;
        [SerializeField]
        private int m_FilterMode;
        [SerializeField]
        private int m_GenerateCubemap;
        [SerializeField]
        private int m_GrayScaleToAlpha;
        [SerializeField]
        private float m_HeightScale;
        [SerializeField]
        private int m_IsReadable;
        [SerializeField]
        private int m_Lightmap;
        [SerializeField]
        private int m_LinearTexture;
        [SerializeField]
        private int m_MaxTextureSize;
        [SerializeField]
        private int m_MaxTextureSizeSet;
        [SerializeField]
        private float m_MipBias;
        [SerializeField]
        private int m_MipMapFadeDistanceEnd;
        [SerializeField]
        private int m_MipMapFadeDistanceStart;
        [SerializeField]
        private int m_MipMapMode;
        [SerializeField]
        private int m_NormalMap;
        [SerializeField]
        private int m_NormalMapFilter;
        [SerializeField]
        private int m_NPOTScale;
        [SerializeField]
        private int m_RGBM;
        [SerializeField]
        private int m_SeamlessCubemap;
        [SerializeField]
        private Vector4 m_SpriteBorder;
        [SerializeField]
        private uint m_SpriteExtrude;
        [SerializeField]
        private int m_SpriteMeshType;
        [SerializeField]
        private int m_SpriteMode;
        [SerializeField]
        private Vector2 m_SpritePivot;
        [SerializeField]
        private float m_SpritePixelsToUnits;
        [SerializeField]
        private float m_SpriteTessellationDetail;
        [SerializeField]
        private int m_sRGBTexture;
        [SerializeField]
        private int m_TextureFormat;
        [SerializeField]
        private int m_TextureFormatSet;
        [SerializeField]
        private int m_TextureShape;
        [SerializeField]
        private int m_TextureType;
        [SerializeField]
        private int m_WrapMode;

        public void ApplyTextureType(TextureImporterType type)
        {
            Internal_ApplyTextureType(this, type);
        }

        /// <summary>
        /// <para>Configure parameters to import a texture for a purpose of type, as described TextureImporterType|here.</para>
        /// </summary>
        /// <param name="type">Texture type. See TextureImporterType.</param>
        /// <param name="applyAll">If false, change only specific properties. Exactly which, depends on type.</param>
        [Obsolete("ApplyTextureType(TextureImporterType, bool) is deprecated, use ApplyTextureType(TextureImporterType)")]
        public void ApplyTextureType(TextureImporterType type, bool applyAll)
        {
            Internal_ApplyTextureType(this, type);
        }

        /// <summary>
        /// <para>Copy parameters into another TextureImporterSettings object.</para>
        /// </summary>
        /// <param name="target">TextureImporterSettings object to copy settings to.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CopyTo(TextureImporterSettings target);
        /// <summary>
        /// <para>Test texture importer settings for equality.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Equal(TextureImporterSettings a, TextureImporterSettings b);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_ApplyTextureType(TextureImporterSettings s, TextureImporterType type);

        /// <summary>
        /// <para>If the provided alpha channel is transparency, enable this to dilate the color to avoid filtering artifacts on the edges.</para>
        /// </summary>
        public bool alphaIsTransparency
        {
            get
            {
                return (this.m_AlphaIsTransparency != 0);
            }
            set
            {
                this.m_AlphaIsTransparency = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Select how the alpha of the imported texture is generated.</para>
        /// </summary>
        public TextureImporterAlphaSource alphaSource
        {
            get
            {
                return (TextureImporterAlphaSource) this.m_AlphaSource;
            }
            set
            {
                this.m_AlphaSource = (int) value;
            }
        }

        /// <summary>
        /// <para>Anisotropic filtering level of the texture.</para>
        /// </summary>
        public int aniso
        {
            get
            {
                return this.m_Aniso;
            }
            set
            {
                this.m_Aniso = value;
            }
        }

        /// <summary>
        /// <para>Enable this to avoid colors seeping out to the edge of the lower Mip levels. Used for light cookies.</para>
        /// </summary>
        public bool borderMipmap
        {
            get
            {
                return (this.m_BorderMipMap != 0);
            }
            set
            {
                this.m_BorderMipMap = !value ? 0 : 1;
            }
        }

        [Obsolete("Texture compression can only be overridden on a per platform basis. See [[TextureImporter.compressionQuality]] for Default platform or [[TextureImporterPlatformSettings]]")]
        public int compressionQuality
        {
            get
            {
                return this.m_CompressionQuality;
            }
            set
            {
                this.m_CompressionQuality = value;
                this.compressionQualitySet = 1;
            }
        }

        private int compressionQualitySet
        {
            get
            {
                return this.m_CompressionQualitySet;
            }
            set
            {
                this.m_CompressionQualitySet = value;
            }
        }

        /// <summary>
        /// <para>Convert heightmap to normal map?</para>
        /// </summary>
        public bool convertToNormalMap
        {
            get
            {
                return (this.m_ConvertToNormalMap != 0);
            }
            set
            {
                this.m_ConvertToNormalMap = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Convolution mode.</para>
        /// </summary>
        public TextureImporterCubemapConvolution cubemapConvolution
        {
            get
            {
                return (TextureImporterCubemapConvolution) this.m_CubemapConvolution;
            }
            set
            {
                this.m_CubemapConvolution = (int) value;
            }
        }

        /// <summary>
        /// <para>Defines how fast Phong exponent wears off in mip maps. Higher value will apply less blur to high resolution mip maps.</para>
        /// </summary>
        [Obsolete("Not used anymore. The right values are automatically picked by the importer.")]
        public float cubemapConvolutionExponent
        {
            get
            {
                return this.m_CubemapConvolutionExponent;
            }
            set
            {
                this.m_CubemapConvolutionExponent = value;
            }
        }

        /// <summary>
        /// <para>Defines how many different Phong exponents to store in mip maps. Higher value will give better transition between glossy and rough reflections, but will need higher texture resolution.</para>
        /// </summary>
        [Obsolete("Not used anymore. The right values are automatically picked by the importer.")]
        public int cubemapConvolutionSteps
        {
            get
            {
                return this.m_CubemapConvolutionSteps;
            }
            set
            {
                this.m_CubemapConvolutionSteps = value;
            }
        }

        /// <summary>
        /// <para>Fade out mip levels to gray color?</para>
        /// </summary>
        public bool fadeOut
        {
            get
            {
                return (this.m_FadeOut != 0);
            }
            set
            {
                this.m_FadeOut = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Filtering mode of the texture.</para>
        /// </summary>
        public FilterMode filterMode
        {
            get
            {
                return (FilterMode) this.m_FilterMode;
            }
            set
            {
                this.m_FilterMode = (int) value;
            }
        }

        /// <summary>
        /// <para>Cubemap generation mode.</para>
        /// </summary>
        public TextureImporterGenerateCubemap generateCubemap
        {
            get
            {
                return (TextureImporterGenerateCubemap) this.m_GenerateCubemap;
            }
            set
            {
                this.m_GenerateCubemap = (int) value;
            }
        }

        [Obsolete("Texture mips are now always generated in linear space")]
        public bool generateMipsInLinearSpace
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Generate alpha channel from intensity?</para>
        /// </summary>
        [Obsolete("Use UnityEditor.TextureImporter.alphaSource instead")]
        public bool grayscaleToAlpha
        {
            get
            {
                return (this.alphaSource == TextureImporterAlphaSource.FromGrayScale);
            }
            set
            {
                if (value)
                {
                    this.alphaSource = TextureImporterAlphaSource.FromGrayScale;
                }
                else
                {
                    this.alphaSource = TextureImporterAlphaSource.FromInput;
                }
            }
        }

        /// <summary>
        /// <para>Amount of bumpyness in the heightmap.</para>
        /// </summary>
        public float heightmapScale
        {
            get
            {
                return this.m_HeightScale;
            }
            set
            {
                this.m_HeightScale = value;
            }
        }

        [Obsolete("Check importer.textureType against TextureImporterType.Lightmap instead. Getter will work as expected. Setter will set textureType to Lightmap if true, nothing otherwise.")]
        public bool lightmap
        {
            get
            {
                return (this.textureType == TextureImporterType.Lightmap);
            }
            set
            {
                if (value)
                {
                    this.textureType = TextureImporterType.Lightmap;
                }
                else
                {
                    this.textureType = TextureImporterType.Default;
                }
            }
        }

        [Obsolete("Use sRGBTexture instead")]
        public bool linearTexture
        {
            get
            {
                return !this.sRGBTexture;
            }
            set
            {
                this.sRGBTexture = !value;
            }
        }

        [Obsolete("Texture max size can only be overridden on a per platform basis. See [[TextureImporter.maxTextureSize]] for Default platform or [[TextureImporterPlatformSettings]]")]
        public int maxTextureSize
        {
            get
            {
                return this.m_MaxTextureSize;
            }
            set
            {
                this.m_MaxTextureSize = value;
                this.maxTextureSizeSet = 1;
            }
        }

        private int maxTextureSizeSet
        {
            get
            {
                return this.m_MaxTextureSizeSet;
            }
            set
            {
                this.m_MaxTextureSizeSet = value;
            }
        }

        /// <summary>
        /// <para>Mip map bias of the texture.</para>
        /// </summary>
        public float mipmapBias
        {
            get
            {
                return this.m_MipBias;
            }
            set
            {
                this.m_MipBias = value;
            }
        }

        /// <summary>
        /// <para>Generate mip maps for the texture?</para>
        /// </summary>
        public bool mipmapEnabled
        {
            get
            {
                return (this.m_EnableMipMap != 0);
            }
            set
            {
                this.m_EnableMipMap = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Mip level where texture is faded out to gray completely.</para>
        /// </summary>
        public int mipmapFadeDistanceEnd
        {
            get
            {
                return this.m_MipMapFadeDistanceEnd;
            }
            set
            {
                this.m_MipMapFadeDistanceEnd = value;
            }
        }

        /// <summary>
        /// <para>Mip level where texture begins to fade out to gray.</para>
        /// </summary>
        public int mipmapFadeDistanceStart
        {
            get
            {
                return this.m_MipMapFadeDistanceStart;
            }
            set
            {
                this.m_MipMapFadeDistanceStart = value;
            }
        }

        /// <summary>
        /// <para>Mipmap filtering mode.</para>
        /// </summary>
        public TextureImporterMipFilter mipmapFilter
        {
            get
            {
                return (TextureImporterMipFilter) this.m_MipMapMode;
            }
            set
            {
                this.m_MipMapMode = (int) value;
            }
        }

        [Obsolete("Check importer.textureType against TextureImporterType.NormalMap instead. Getter will work as expected. Setter will set textureType to NormalMap if true, nothing otherwise")]
        public bool normalMap
        {
            get
            {
                return (this.textureType == TextureImporterType.NormalMap);
            }
            set
            {
                if (value)
                {
                    this.textureType = TextureImporterType.NormalMap;
                }
                else
                {
                    this.textureType = TextureImporterType.Default;
                }
            }
        }

        /// <summary>
        /// <para>Normal map filtering mode.</para>
        /// </summary>
        public TextureImporterNormalFilter normalMapFilter
        {
            get
            {
                return (TextureImporterNormalFilter) this.m_NormalMapFilter;
            }
            set
            {
                this.m_NormalMapFilter = (int) value;
            }
        }

        /// <summary>
        /// <para>Scaling mode for non power of two textures.</para>
        /// </summary>
        public TextureImporterNPOTScale npotScale
        {
            get
            {
                return (TextureImporterNPOTScale) this.m_NPOTScale;
            }
            set
            {
                this.m_NPOTScale = (int) value;
            }
        }

        /// <summary>
        /// <para>Is texture data readable from scripts.</para>
        /// </summary>
        public bool readable
        {
            get
            {
                return (this.m_IsReadable != 0);
            }
            set
            {
                this.m_IsReadable = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>RGBM encoding mode for HDR textures in TextureImporter.</para>
        /// </summary>
        [Obsolete("RGBM is no longer a user's choice but has become an implementation detail hidden to the user.")]
        public TextureImporterRGBMMode rgbm
        {
            get
            {
                return (TextureImporterRGBMMode) this.m_RGBM;
            }
            set
            {
                this.m_RGBM = (int) value;
            }
        }

        public bool seamlessCubemap
        {
            get
            {
                return (this.m_SeamlessCubemap != 0);
            }
            set
            {
                this.m_SeamlessCubemap = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Edge-relative alignment of the sprite graphic.</para>
        /// </summary>
        public int spriteAlignment
        {
            get
            {
                return this.m_Alignment;
            }
            set
            {
                this.m_Alignment = value;
            }
        }

        /// <summary>
        /// <para>Border sizes of the generated sprites.</para>
        /// </summary>
        public Vector4 spriteBorder
        {
            get
            {
                return this.m_SpriteBorder;
            }
            set
            {
                this.m_SpriteBorder = value;
            }
        }

        /// <summary>
        /// <para>The number of blank pixels to leave between the edge of the graphic and the mesh.</para>
        /// </summary>
        public uint spriteExtrude
        {
            get
            {
                return this.m_SpriteExtrude;
            }
            set
            {
                this.m_SpriteExtrude = value;
            }
        }

        public SpriteMeshType spriteMeshType
        {
            get
            {
                return (SpriteMeshType) this.m_SpriteMeshType;
            }
            set
            {
                this.m_SpriteMeshType = (int) value;
            }
        }

        /// <summary>
        /// <para>Sprite texture import mode.</para>
        /// </summary>
        public int spriteMode
        {
            get
            {
                return this.m_SpriteMode;
            }
            set
            {
                this.m_SpriteMode = value;
            }
        }

        /// <summary>
        /// <para>Pivot point of the Sprite relative to its graphic's rectangle.</para>
        /// </summary>
        public Vector2 spritePivot
        {
            get
            {
                return this.m_SpritePivot;
            }
            set
            {
                this.m_SpritePivot = value;
            }
        }

        /// <summary>
        /// <para>The number of pixels in the sprite that correspond to one unit in world space.</para>
        /// </summary>
        public float spritePixelsPerUnit
        {
            get
            {
                return this.m_SpritePixelsToUnits;
            }
            set
            {
                this.m_SpritePixelsToUnits = value;
            }
        }

        /// <summary>
        /// <para>Scale factor between pixels in the sprite graphic and world space units.</para>
        /// </summary>
        [Obsolete("Use spritePixelsPerUnit property instead.")]
        public float spritePixelsToUnits
        {
            get
            {
                return this.m_SpritePixelsToUnits;
            }
            set
            {
                this.m_SpritePixelsToUnits = value;
            }
        }

        /// <summary>
        /// <para>The tessellation detail to be used for generating the mesh for the associated sprite if the SpriteMode is set to Single. For Multiple sprites, use the SpriteEditor to specify the value per sprite.
        /// Valid values are in the range [0-1], with higher values generating a tighter mesh. A default of -1 will allow Unity to determine the value automatically.</para>
        /// </summary>
        public float spriteTessellationDetail
        {
            get
            {
                return this.m_SpriteTessellationDetail;
            }
            set
            {
                this.m_SpriteTessellationDetail = value;
            }
        }

        /// <summary>
        /// <para>Is texture storing color data?</para>
        /// </summary>
        public bool sRGBTexture
        {
            get
            {
                return (this.m_sRGBTexture != 0);
            }
            set
            {
                this.m_sRGBTexture = !value ? 0 : 1;
            }
        }

        [Obsolete("Texture format can only be overridden on a per platform basis. See [[TextureImporterPlatformSettings]]")]
        public TextureImporterFormat textureFormat
        {
            get
            {
                return (TextureImporterFormat) this.m_TextureFormat;
            }
            set
            {
                this.m_TextureFormat = (int) this.textureFormat;
                this.textureFormatSet = 1;
            }
        }

        private int textureFormatSet
        {
            get
            {
                return this.m_TextureFormatSet;
            }
            set
            {
                this.m_TextureFormatSet = value;
            }
        }

        /// <summary>
        /// <para>Shape of imported texture.</para>
        /// </summary>
        public TextureImporterShape textureShape
        {
            get
            {
                return (TextureImporterShape) this.m_TextureShape;
            }
            set
            {
                this.m_TextureShape = (int) value;
            }
        }

        /// <summary>
        /// <para>Which type of texture are we dealing with here.</para>
        /// </summary>
        public TextureImporterType textureType
        {
            get
            {
                return (TextureImporterType) this.m_TextureType;
            }
            set
            {
                this.m_TextureType = (int) value;
            }
        }

        /// <summary>
        /// <para>Wrap mode (Repeat or Clamp) of the texture.</para>
        /// </summary>
        public TextureWrapMode wrapMode
        {
            get
            {
                return (TextureWrapMode) this.m_WrapMode;
            }
            set
            {
                this.m_WrapMode = (int) value;
            }
        }
    }
}

