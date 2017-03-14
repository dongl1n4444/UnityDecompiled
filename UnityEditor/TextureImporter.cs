namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Build;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Texture importer lets you modify Texture2D import settings from editor scripts.</para>
    /// </summary>
    public sealed class TextureImporter : AssetImporter
    {
        /// <summary>
        /// <para>Clear specific target platform settings.</para>
        /// </summary>
        /// <param name="platform">The platform whose settings are to be cleared (see below).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearPlatformTextureSettings(string platform);
        /// <summary>
        /// <para>Does textures source image have alpha channel.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool DoesSourceTextureHaveAlpha();
        /// <summary>
        /// <para>Does textures source image have RGB channels.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("DoesSourceTextureHaveColor always returns true in Unity."), GeneratedByOldBindingsGenerator]
        public extern bool DoesSourceTextureHaveColor();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern TextureImporterFormat FormatFromTextureParameters(TextureImporterSettings settings, TextureImporterPlatformSettings platformSettings, bool doesTextureContainAlpha, bool sourceWasHDR, BuildTarget destinationPlatform);
        /// <summary>
        /// <para>Getter for the flag that allows Alpha splitting on the imported texture when needed (for example ETC1 compression for textures with transparency).</para>
        /// </summary>
        /// <returns>
        /// <para>True if the importer allows alpha split on the imported texture, False otherwise.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use UnityEditor.TextureImporter.GetPlatformTextureSettings() instead."), GeneratedByOldBindingsGenerator]
        public extern bool GetAllowsAlphaSplitting();
        /// <summary>
        /// <para>Returns the TextureImporterFormat that would be automatically chosen for this platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>
        /// <para>Format chosen by the system for the provided platform, TextureImporterFormat.Automatic if the platform does not exist.</para>
        /// </returns>
        public TextureImporterFormat GetAutomaticFormat(string platform)
        {
            TextureImporterSettings dest = new TextureImporterSettings();
            this.ReadTextureSettings(dest);
            TextureImporterPlatformSettings platformTextureSettings = this.GetPlatformTextureSettings(platform);
            List<BuildPlatform> validPlatforms = BuildPlatforms.instance.GetValidPlatforms();
            foreach (BuildPlatform platform2 in validPlatforms)
            {
                if (platform2.name == platform)
                {
                    return FormatFromTextureParameters(dest, platformTextureSettings, this.DoesSourceTextureHaveAlpha(), this.IsSourceTextureHDR(), platform2.defaultTarget);
                }
            }
            return TextureImporterFormat.Automatic;
        }

        /// <summary>
        /// <para>Get the default platform specific texture settings.</para>
        /// </summary>
        /// <returns>
        /// <para>A TextureImporterPlatformSettings structure containing the default platform parameters.</para>
        /// </returns>
        public TextureImporterPlatformSettings GetDefaultPlatformTextureSettings() => 
            this.GetPlatformTextureSettings(TextureImporterInspector.s_DefaultPlatformName);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern string GetImportWarnings();
        /// <summary>
        /// <para>Get platform specific texture settings.</para>
        /// </summary>
        /// <param name="platform">The platform whose settings are required (see below).</param>
        /// <returns>
        /// <para>A TextureImporterPlatformSettings structure containing the platform parameters.</para>
        /// </returns>
        public TextureImporterPlatformSettings GetPlatformTextureSettings(string platform)
        {
            TextureImporterPlatformSettings dest = new TextureImporterPlatformSettings();
            this.Internal_GetPlatformTextureSettings(platform, dest);
            return dest;
        }

        public bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat)
        {
            int compressionQuality = 0;
            bool flag = false;
            return this.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out flag);
        }

        public bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat, out int compressionQuality)
        {
            bool flag = false;
            return this.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out flag);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat, out int compressionQuality, out bool etc1AlphaSplitEnabled);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void GetWidthAndHeight(ref int width, ref int height);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_spriteBorder(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_spritePivot(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void Internal_GetPlatformTextureSettings(string platform, TextureImporterPlatformSettings dest);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_spriteBorder(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_spritePivot(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsETC1SupportedByBuildTarget(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool IsSourceTextureHDR();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsTextureFormatETC1Compression(TextureFormat fmt);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ReadTextureImportInstructions(BuildTarget target, out TextureFormat desiredFormat, out ColorSpace colorSpace, out int compressionQuality);
        /// <summary>
        /// <para>Read texture settings into TextureImporterSettings class.</para>
        /// </summary>
        /// <param name="dest"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ReadTextureSettings(TextureImporterSettings dest);
        /// <summary>
        /// <para>Setter for the flag that allows Alpha splitting on the imported texture when needed (for example ETC1 compression for textures with transparency).</para>
        /// </summary>
        /// <param name="flag"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings() instead."), GeneratedByOldBindingsGenerator]
        public extern void SetAllowsAlphaSplitting(bool flag);
        /// <summary>
        /// <para>Set specific target platform settings.</para>
        /// </summary>
        /// <param name="platformSettings">Structure containing the platform settings.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPlatformTextureSettings(TextureImporterPlatformSettings platformSettings);
        [Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings(TextureImporterPlatformSettings) instead."), ExcludeFromDocs]
        public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat)
        {
            bool allowsAlphaSplit = false;
            this.SetPlatformTextureSettings(platform, maxTextureSize, textureFormat, allowsAlphaSplit);
        }

        /// <summary>
        /// <para>Set specific target platform settings.</para>
        /// </summary>
        /// <param name="platform">The platforms whose settings are to be changed (see below).</param>
        /// <param name="maxTextureSize">Maximum texture width/height in pixels.</param>
        /// <param name="textureFormat">Data format for the texture.</param>
        /// <param name="compressionQuality">Value from 0..100, with 0, 50 and 100 being respectively Fast, Normal, Best quality options in the texture importer UI. For Crunch texture formats, this roughly corresponds to JPEG quality levels.</param>
        /// <param name="allowsAlphaSplit">Allows splitting of imported texture into RGB+A so that ETC1 compression can be applied (Android only, and works only on textures that are a part of some atlas).</param>
        [Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings(TextureImporterPlatformSettings) instead.")]
        public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat, [DefaultValue("false")] bool allowsAlphaSplit)
        {
            TextureImporterPlatformSettings dest = new TextureImporterPlatformSettings();
            this.Internal_GetPlatformTextureSettings(platform, dest);
            dest.overridden = true;
            dest.maxTextureSize = maxTextureSize;
            dest.format = textureFormat;
            dest.allowsAlphaSplitting = allowsAlphaSplit;
            this.SetPlatformTextureSettings(dest);
        }

        /// <summary>
        /// <para>Set specific target platform settings.</para>
        /// </summary>
        /// <param name="platform">The platforms whose settings are to be changed (see below).</param>
        /// <param name="maxTextureSize">Maximum texture width/height in pixels.</param>
        /// <param name="textureFormat">Data format for the texture.</param>
        /// <param name="compressionQuality">Value from 0..100, with 0, 50 and 100 being respectively Fast, Normal, Best quality options in the texture importer UI. For Crunch texture formats, this roughly corresponds to JPEG quality levels.</param>
        /// <param name="allowsAlphaSplit">Allows splitting of imported texture into RGB+A so that ETC1 compression can be applied (Android only, and works only on textures that are a part of some atlas).</param>
        [Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings(TextureImporterPlatformSettings) instead.")]
        public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat, int compressionQuality, bool allowsAlphaSplit)
        {
            TextureImporterPlatformSettings dest = new TextureImporterPlatformSettings();
            this.Internal_GetPlatformTextureSettings(platform, dest);
            dest.overridden = true;
            dest.maxTextureSize = maxTextureSize;
            dest.format = textureFormat;
            dest.compressionQuality = compressionQuality;
            dest.allowsAlphaSplitting = allowsAlphaSplit;
            this.SetPlatformTextureSettings(dest);
        }

        /// <summary>
        /// <para>Set texture importers settings from TextureImporterSettings class.</para>
        /// </summary>
        /// <param name="src"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetTextureSettings(TextureImporterSettings src);

        /// <summary>
        /// <para>Allows alpha splitting on relevant platforms for this texture.</para>
        /// </summary>
        public bool allowAlphaSplitting { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If the provided alpha channel is transparency, enable this to prefilter the color to avoid filtering artifacts.</para>
        /// </summary>
        public bool alphaIsTransparency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Select how the alpha of the imported texture is generated.</para>
        /// </summary>
        public TextureImporterAlphaSource alphaSource { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Anisotropic filtering level of the texture.</para>
        /// </summary>
        public int anisoLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Keep texture borders the same when generating mipmaps?</para>
        /// </summary>
        public bool borderMipmap { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Quality of Texture Compression in the range [0..100].</para>
        /// </summary>
        public int compressionQuality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Convert heightmap to normal map?</para>
        /// </summary>
        public bool convertToNormalmap { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("correctGamma Property deprecated. Mipmaps are always generated in linear space.")]
        public bool correctGamma { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Use crunched compression when available.</para>
        /// </summary>
        public bool crunchedCompression { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal static string defaultPlatformName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Fade out mip levels to gray color?</para>
        /// </summary>
        public bool fadeout { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Filtering mode of the texture.</para>
        /// </summary>
        public UnityEngine.FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Cubemap generation mode.</para>
        /// </summary>
        public TextureImporterGenerateCubemap generateCubemap { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should mip maps be generated with gamma correction?</para>
        /// </summary>
        [Obsolete("generateMipsInLinearSpace Property deprecated. Mipmaps are always generated in linear space.")]
        public bool generateMipsInLinearSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Generate alpha channel from intensity?</para>
        /// </summary>
        [Obsolete("Use UnityEditor.TextureImporter.alphaSource instead.")]
        public bool grayscaleToAlpha
        {
            get => 
                (this.alphaSource == TextureImporterAlphaSource.FromGrayScale);
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
        public float heightmapScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is texture data readable from scripts.</para>
        /// </summary>
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is this texture a lightmap?</para>
        /// </summary>
        [Obsolete("lightmap Property deprecated. Check [[TextureImporterSettings.textureType]] instead. Getter will work as expected. Setter will set textureType to Lightmap if true, nothing otherwise.")]
        public bool lightmap
        {
            get => 
                (this.textureType == TextureImporterType.Lightmap);
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

        /// <summary>
        /// <para>Is texture storing non-color data?</para>
        /// </summary>
        [Obsolete("linearTexture Property deprecated. Use sRGBTexture instead.")]
        public bool linearTexture
        {
            get => 
                !this.sRGBTexture;
            set
            {
                this.sRGBTexture = !value;
            }
        }

        /// <summary>
        /// <para>Maximum texture size.</para>
        /// </summary>
        public int maxTextureSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mip map bias of the texture.</para>
        /// </summary>
        public float mipMapBias { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Generate mip maps for the texture?</para>
        /// </summary>
        public bool mipmapEnabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mip level where texture is faded out completely.</para>
        /// </summary>
        public int mipmapFadeDistanceEnd { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mip level where texture begins to fade out.</para>
        /// </summary>
        public int mipmapFadeDistanceStart { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mipmap filtering mode.</para>
        /// </summary>
        public TextureImporterMipFilter mipmapFilter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is this texture a normal map?</para>
        /// </summary>
        [Obsolete("normalmap Property deprecated. Check [[TextureImporterSettings.textureType]] instead. Getter will work as expected. Setter will set textureType to NormalMap if true, nothing otherwise.")]
        public bool normalmap
        {
            get => 
                (this.textureType == TextureImporterType.NormalMap);
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
        public TextureImporterNormalFilter normalmapFilter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Scaling mode for non power of two textures.</para>
        /// </summary>
        public TextureImporterNPOTScale npotScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns true if this TextureImporter is setup for Sprite packing.</para>
        /// </summary>
        public bool qualifiesForSpritePacking { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Border sizes of the generated sprites.</para>
        /// </summary>
        public Vector4 spriteBorder
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_spriteBorder(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_spriteBorder(ref value);
            }
        }

        /// <summary>
        /// <para>Selects Single or Manual import mode for Sprite textures.</para>
        /// </summary>
        public SpriteImportMode spriteImportMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Selects the Sprite packing tag.</para>
        /// </summary>
        public string spritePackingTag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The point in the Sprite object's coordinate space where the graphic is located.</para>
        /// </summary>
        public Vector2 spritePivot
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_spritePivot(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_spritePivot(ref value);
            }
        }

        /// <summary>
        /// <para>The number of pixels in the sprite that correspond to one unit in world space.</para>
        /// </summary>
        public float spritePixelsPerUnit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Scale factor for mapping pixels in the graphic to units in world space.</para>
        /// </summary>
        [Obsolete("Use spritePixelsPerUnit property instead.")]
        public float spritePixelsToUnits { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Array representing the sections of the atlas corresponding to individual sprite graphics.</para>
        /// </summary>
        public SpriteMetaData[] spritesheet { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is texture storing color data?</para>
        /// </summary>
        public bool sRGBTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Compression of imported texture.</para>
        /// </summary>
        public TextureImporterCompression textureCompression { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Format of imported texture.</para>
        /// </summary>
        [Obsolete("textureFormat is not longer accessible at the TextureImporter level. For old 'simple' formats use the textureCompression property for the equivalent automatic choice (Uncompressed for TrueColor, Compressed and HQCommpressed for 16 bits). For platform specific formats use the [[PlatformTextureSettings]] API. Using this setter will setup various parameters to match the new automatic system as well possible. Getter will return the last value set.")]
        public TextureImporterFormat textureFormat { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shape of imported texture.</para>
        /// </summary>
        public TextureImporterShape textureShape { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Which type of texture are we dealing with here.</para>
        /// </summary>
        public TextureImporterType textureType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture coordinate wrapping mode.</para>
        /// </summary>
        public TextureWrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture U coordinate wrapping mode.</para>
        /// </summary>
        public TextureWrapMode wrapModeU { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture V coordinate wrapping mode.</para>
        /// </summary>
        public TextureWrapMode wrapModeV { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture W coordinate wrapping mode for Texture3D.</para>
        /// </summary>
        public TextureWrapMode wrapModeW { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

