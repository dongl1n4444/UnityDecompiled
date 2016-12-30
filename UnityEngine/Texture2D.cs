namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class for texture handling.</para>
    /// </summary>
    public sealed class Texture2D : Texture
    {
        /// <summary>
        /// <para>Create a new empty texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Texture2D(int width, int height)
        {
            Internal_Create(this, width, height, TextureFormat.RGBA32, true, false, IntPtr.Zero);
        }

        /// <summary>
        /// <para>Create a new empty texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="mipmap"></param>
        public Texture2D(int width, int height, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, width, height, format, mipmap, false, IntPtr.Zero);
        }

        /// <summary>
        /// <para>See Also: SetPixel, SetPixels, Apply functions.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="mipmap"></param>
        /// <param name="linear"></param>
        public Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear)
        {
            Internal_Create(this, width, height, format, mipmap, linear, IntPtr.Zero);
        }

        internal Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
        {
            Internal_Create(this, width, height, format, mipmap, linear, nativeTex);
        }

        [ExcludeFromDocs]
        public void Apply()
        {
            bool makeNoLongerReadable = false;
            bool updateMipmaps = true;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        [ExcludeFromDocs]
        public void Apply(bool updateMipmaps)
        {
            bool makeNoLongerReadable = false;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        /// <summary>
        /// <para>Actually apply all previous SetPixel and SetPixels changes.</para>
        /// </summary>
        /// <param name="updateMipmaps">When set to true, mipmap levels are recalculated.</param>
        /// <param name="makeNoLongerReadable">When set to true, system memory copy of a texture is released.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);
        /// <summary>
        /// <para>Compress texture into DXT format.</para>
        /// </summary>
        /// <param name="highQuality"></param>
        public void Compress(bool highQuality)
        {
            INTERNAL_CALL_Compress(this, highQuality);
        }

        /// <summary>
        /// <para>Creates Unity Texture out of externally created native texture object.</para>
        /// </summary>
        /// <param name="nativeTex">Native 2D texture object.</param>
        /// <param name="width">Width of texture in pixels.</param>
        /// <param name="height">Height of texture in pixels.</param>
        /// <param name="format">Format of underlying texture object.</param>
        /// <param name="mipmap">Does the texture have mipmaps?</param>
        /// <param name="linear">Is texture using linear color space?</param>
        public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex) => 
            new Texture2D(width, height, format, mipmap, linear, nativeTex);

        [ExcludeFromDocs]
        public byte[] EncodeToEXR()
        {
            EXRFlags none = EXRFlags.None;
            return this.EncodeToEXR(none);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern byte[] EncodeToEXR([DefaultValue("EXRFlags.None")] EXRFlags flags);
        /// <summary>
        /// <para>Encodes this texture into JPG format.</para>
        /// </summary>
        /// <param name="quality">JPG quality to encode with, 1..100 (default 75).</param>
        public byte[] EncodeToJPG() => 
            this.EncodeToJPG(0x4b);

        /// <summary>
        /// <para>Encodes this texture into JPG format.</para>
        /// </summary>
        /// <param name="quality">JPG quality to encode with, 1..100 (default 75).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern byte[] EncodeToJPG(int quality);
        /// <summary>
        /// <para>Encodes this texture into PNG format.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern byte[] EncodeToPNG();
        public static bool GenerateAtlas(Vector2[] sizes, int padding, int atlasSize, List<Rect> results)
        {
            if (sizes == null)
            {
                throw new ArgumentException("sizes array can not be null");
            }
            if (results == null)
            {
                throw new ArgumentException("results list cannot be null");
            }
            if (padding < 0)
            {
                throw new ArgumentException("padding can not be negative");
            }
            if (atlasSize <= 0)
            {
                throw new ArgumentException("atlas size must be positive");
            }
            results.Clear();
            if (sizes.Length == 0)
            {
                return true;
            }
            GenerateAtlasInternal(sizes, padding, atlasSize, results);
            return (results.Count != 0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GenerateAtlasInternal(Vector2[] sizes, int padding, int atlasSize, object resultList);
        /// <summary>
        /// <para>Returns pixel color at coordinates (x, y).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Color GetPixel(int x, int y)
        {
            Color color;
            INTERNAL_CALL_GetPixel(this, x, y, out color);
            return color;
        }

        /// <summary>
        /// <para>Returns filtered pixel color at normalized coordinates (u, v).</para>
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public Color GetPixelBilinear(float u, float v)
        {
            Color color;
            INTERNAL_CALL_GetPixelBilinear(this, u, v, out color);
            return color;
        }

        [ExcludeFromDocs]
        public Color[] GetPixels()
        {
            int miplevel = 0;
            return this.GetPixels(miplevel);
        }

        /// <summary>
        /// <para>Get a block of pixel colors.</para>
        /// </summary>
        /// <param name="miplevel"></param>
        public Color[] GetPixels([DefaultValue("0")] int miplevel)
        {
            int blockWidth = this.width >> miplevel;
            if (blockWidth < 1)
            {
                blockWidth = 1;
            }
            int blockHeight = this.height >> miplevel;
            if (blockHeight < 1)
            {
                blockHeight = 1;
            }
            return this.GetPixels(0, 0, blockWidth, blockHeight, miplevel);
        }

        [ExcludeFromDocs]
        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
        {
            int miplevel = 0;
            return this.GetPixels(x, y, blockWidth, blockHeight, miplevel);
        }

        /// <summary>
        /// <para>Get a block of pixel colors.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <param name="miplevel"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public Color32[] GetPixels32()
        {
            int miplevel = 0;
            return this.GetPixels32(miplevel);
        }

        /// <summary>
        /// <para>Get a block of pixel colors in Color32 format.</para>
        /// </summary>
        /// <param name="miplevel"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);
        /// <summary>
        /// <para>Get raw data from a texture.</para>
        /// </summary>
        /// <returns>
        /// <para>Raw texture data as a byte array.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern byte[] GetRawTextureData();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Compress(Texture2D self, bool highQuality);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPixel(Texture2D self, int x, int y, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPixelBilinear(Texture2D self, float u, float v, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ReadPixels(Texture2D self, ref Rect source, int destX, int destY, bool recalculateMipMaps);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetPixel(Texture2D self, int x, int y, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create([Writable] Texture2D mono, int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool Internal_ResizeWH(int width, int height);
        [ExcludeFromDocs]
        public bool LoadImage(byte[] data)
        {
            bool markNonReadable = false;
            return this.LoadImage(data, markNonReadable);
        }

        /// <summary>
        /// <para>Loads PNG/JPG image byte array into a texture.</para>
        /// </summary>
        /// <param name="data">The byte array containing the image data to load.</param>
        /// <param name="markNonReadable">Set to false by default, pass true to optionally mark the texture as non-readable.</param>
        /// <returns>
        /// <para>Returns true if the data can be loaded, false otherwise.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool LoadImage(byte[] data, [DefaultValue("false")] bool markNonReadable);
        /// <summary>
        /// <para>Fills texture pixels with raw preformatted data.</para>
        /// </summary>
        /// <param name="data">Byte array to initialize texture pixels with.</param>
        /// <param name="size">Size of data in bytes.</param>
        public void LoadRawTextureData(byte[] data)
        {
            this.LoadRawTextureData_ImplArray(data);
        }

        /// <summary>
        /// <para>Fills texture pixels with raw preformatted data.</para>
        /// </summary>
        /// <param name="data">Byte array to initialize texture pixels with.</param>
        /// <param name="size">Size of data in bytes.</param>
        public void LoadRawTextureData(IntPtr data, int size)
        {
            this.LoadRawTextureData_ImplPointer(data, size);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void LoadRawTextureData_ImplArray(byte[] data);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void LoadRawTextureData_ImplPointer(IntPtr data, int size);
        [ExcludeFromDocs]
        public Rect[] PackTextures(Texture2D[] textures, int padding)
        {
            bool makeNoLongerReadable = false;
            int maximumAtlasSize = 0x800;
            return this.PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
        }

        [ExcludeFromDocs]
        public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
        {
            bool makeNoLongerReadable = false;
            return this.PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
        }

        /// <summary>
        /// <para>Packs multiple Textures into a texture atlas.</para>
        /// </summary>
        /// <param name="textures">Array of textures to pack into the atlas.</param>
        /// <param name="padding">Padding in pixels between the packed textures.</param>
        /// <param name="maximumAtlasSize">Maximum size of the resulting texture.</param>
        /// <param name="makeNoLongerReadable">Should the texture be marked as no longer readable?</param>
        /// <returns>
        /// <para>An array of rectangles containing the UV coordinates in the atlas for each input texture, or null if packing fails.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Rect[] PackTextures(Texture2D[] textures, int padding, [DefaultValue("2048")] int maximumAtlasSize, [DefaultValue("false")] bool makeNoLongerReadable);
        [ExcludeFromDocs]
        public void ReadPixels(Rect source, int destX, int destY)
        {
            bool recalculateMipMaps = true;
            INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
        }

        /// <summary>
        /// <para>Read pixels from screen into the saved texture data.</para>
        /// </summary>
        /// <param name="source">Rectangular region of the view to read from. Pixels are read from current render target.</param>
        /// <param name="destX">Horizontal pixel position in the texture to place the pixels that are read.</param>
        /// <param name="destY">Vertical pixel position in the texture to place the pixels that are read.</param>
        /// <param name="recalculateMipMaps">Should the texture's mipmaps be recalculated after reading?</param>
        public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
        {
            INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
        }

        /// <summary>
        /// <para>Resizes the texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public bool Resize(int width, int height) => 
            this.Internal_ResizeWH(width, height);

        /// <summary>
        /// <para>Resizes the texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="hasMipMap"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool Resize(int width, int height, TextureFormat format, bool hasMipMap);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetAllPixels32(Color32[] colors, int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel);
        /// <summary>
        /// <para>Sets pixel color at coordinates (x,y).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            INTERNAL_CALL_SetPixel(this, x, y, ref color);
        }

        [ExcludeFromDocs]
        public void SetPixels(Color[] colors)
        {
            int miplevel = 0;
            this.SetPixels(colors, miplevel);
        }

        /// <summary>
        /// <para>Set a block of pixel colors.</para>
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="miplevel"></param>
        public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
        {
            int blockWidth = this.width >> miplevel;
            if (blockWidth < 1)
            {
                blockWidth = 1;
            }
            int blockHeight = this.height >> miplevel;
            if (blockHeight < 1)
            {
                blockHeight = 1;
            }
            this.SetPixels(0, 0, blockWidth, blockHeight, colors, miplevel);
        }

        [ExcludeFromDocs]
        public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
        {
            int miplevel = 0;
            this.SetPixels(x, y, blockWidth, blockHeight, colors, miplevel);
        }

        /// <summary>
        /// <para>Set a block of pixel colors.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <param name="colors"></param>
        /// <param name="miplevel"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SetPixels32(Color32[] colors)
        {
            int miplevel = 0;
            this.SetPixels32(colors, miplevel);
        }

        /// <summary>
        /// <para>Set a block of pixel colors.</para>
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="miplevel"></param>
        public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
        {
            this.SetAllPixels32(colors, miplevel);
        }

        [ExcludeFromDocs]
        public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
        {
            int miplevel = 0;
            this.SetPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
        }

        /// <summary>
        /// <para>Set a block of pixel colors.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <param name="colors"></param>
        /// <param name="miplevel"></param>
        public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
        {
            this.SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
        }

        /// <summary>
        /// <para>Updates Unity texture to use different native texture object.</para>
        /// </summary>
        /// <param name="nativeTex">Native 2D texture object.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void UpdateExternalTexture(IntPtr nativeTex);

        public bool alphaIsTransparency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Get a small texture with all black pixels.</para>
        /// </summary>
        public static Texture2D blackTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The format of the pixel data in the texture (Read Only).</para>
        /// </summary>
        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>How many mipmap levels are in this texture (Read Only).</para>
        /// </summary>
        public int mipmapCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Get a small texture with all white pixels.</para>
        /// </summary>
        public static Texture2D whiteTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Flags used to control the encoding to an EXR file.</para>
        /// </summary>
        [Flags]
        public enum EXRFlags
        {
            /// <summary>
            /// <para>This texture will use Wavelet compression. This is best used for grainy images.</para>
            /// </summary>
            CompressPIZ = 8,
            /// <summary>
            /// <para>The texture will use RLE (Run Length Encoding) EXR compression format (similar to Targa RLE compression).</para>
            /// </summary>
            CompressRLE = 4,
            /// <summary>
            /// <para>The texture will use the EXR ZIP compression format.</para>
            /// </summary>
            CompressZIP = 2,
            /// <summary>
            /// <para>No flag. This will result in an uncompressed 16-bit float EXR file.</para>
            /// </summary>
            None = 0,
            /// <summary>
            /// <para>The texture will be exported as a 32-bit float EXR file (default is 16-bit).</para>
            /// </summary>
            OutputAsFloat = 1
        }
    }
}

