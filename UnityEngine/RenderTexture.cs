namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Render textures are textures that can be rendered to.</para>
    /// </summary>
    [UsedByNativeCode]
    public class RenderTexture : Texture
    {
        /// <summary>
        /// <para>Creates a new RenderTexture object.</para>
        /// </summary>
        /// <param name="width">Texture width in pixels.</param>
        /// <param name="height">Texture height in pixels.</param>
        /// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
        /// <param name="format">Texture color format.</param>
        /// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
        public RenderTexture(int width, int height, int depth)
        {
            Internal_CreateRenderTexture(this);
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = RenderTextureFormat.Default;
            Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
        }

        /// <summary>
        /// <para>Creates a new RenderTexture object.</para>
        /// </summary>
        /// <param name="width">Texture width in pixels.</param>
        /// <param name="height">Texture height in pixels.</param>
        /// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
        /// <param name="format">Texture color format.</param>
        /// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
        public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
        {
            Internal_CreateRenderTexture(this);
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = format;
            Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
        }

        /// <summary>
        /// <para>Creates a new RenderTexture object.</para>
        /// </summary>
        /// <param name="width">Texture width in pixels.</param>
        /// <param name="height">Texture height in pixels.</param>
        /// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
        /// <param name="format">Texture color format.</param>
        /// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
        public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            Internal_CreateRenderTexture(this);
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = format;
            bool sRGB = readWrite == RenderTextureReadWrite.sRGB;
            if (readWrite == RenderTextureReadWrite.Default)
            {
                sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
            }
            Internal_SetSRGBReadWrite(this, sRGB);
        }

        /// <summary>
        /// <para>Actually creates the RenderTexture.</para>
        /// </summary>
        public bool Create() => 
            INTERNAL_CALL_Create(this);

        /// <summary>
        /// <para>Discards the contents of the RenderTexture.</para>
        /// </summary>
        /// <param name="discardColor">Should the colour buffer be discarded?</param>
        /// <param name="discardDepth">Should the depth buffer be discarded?</param>
        public void DiscardContents()
        {
            INTERNAL_CALL_DiscardContents(this);
        }

        /// <summary>
        /// <para>Discards the contents of the RenderTexture.</para>
        /// </summary>
        /// <param name="discardColor">Should the colour buffer be discarded?</param>
        /// <param name="discardDepth">Should the depth buffer be discarded?</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DiscardContents(bool discardColor, bool discardDepth);
        /// <summary>
        /// <para>Generate mipmap levels of a render texture.</para>
        /// </summary>
        public void GenerateMips()
        {
            INTERNAL_CALL_GenerateMips(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetColorBuffer(out RenderBuffer res);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetDepthBuffer(out RenderBuffer res);
        /// <summary>
        /// <para>Retrieve a native (underlying graphics API) pointer to the depth buffer resource.</para>
        /// </summary>
        /// <returns>
        /// <para>Pointer to an underlying graphics API depth buffer resource.</para>
        /// </returns>
        public IntPtr GetNativeDepthBufferPtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeDepthBufferPtr(this, out ptr);
            return ptr;
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            int depthBuffer = 0;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            int antiAliasing = 1;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        /// <summary>
        /// <para>Allocate a temporary render texture.</para>
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="depthBuffer">Depth buffer bits (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
        /// <param name="format">Render texture format.</param>
        /// <param name="readWrite">Color space conversion mode.</param>
        /// <param name="antiAliasing">Anti-aliasing (1,2,4,8).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern RenderTexture GetTemporary(int width, int height, [UnityEngine.Internal.DefaultValue("0")] int depthBuffer, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing);
        [Obsolete("GetTexelOffset always returns zero now, no point in using it.")]
        public Vector2 GetTexelOffset() => 
            Vector2.zero;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Create(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_DiscardContents(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GenerateMips(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetNativeDepthBufferPtr(RenderTexture self, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsCreated(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MarkRestoreExpected(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Release(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateRenderTexture([Writable] RenderTexture rt);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern TextureDimension Internal_GetDimension(RenderTexture rt);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetHeight(RenderTexture mono);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetWidth(RenderTexture mono);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetDimension(RenderTexture rt, TextureDimension dim);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetHeight(RenderTexture mono, int width);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetSRGBReadWrite(RenderTexture mono, bool sRGB);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetWidth(RenderTexture mono, int width);
        /// <summary>
        /// <para>Is the render texture actually created?</para>
        /// </summary>
        public bool IsCreated() => 
            INTERNAL_CALL_IsCreated(this);

        /// <summary>
        /// <para>Indicate that there's a RenderTexture restore operation expected.</para>
        /// </summary>
        public void MarkRestoreExpected()
        {
            INTERNAL_CALL_MarkRestoreExpected(this);
        }

        /// <summary>
        /// <para>Releases the RenderTexture.</para>
        /// </summary>
        public void Release()
        {
            INTERNAL_CALL_Release(this);
        }

        /// <summary>
        /// <para>Release a temporary texture allocated with GetTemporary.</para>
        /// </summary>
        /// <param name="temp"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ReleaseTemporary(RenderTexture temp);
        [Obsolete("SetBorderColor is no longer supported.", true)]
        public void SetBorderColor(Color color)
        {
        }

        /// <summary>
        /// <para>Assigns this RenderTexture as a global shader property named propertyName.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetGlobalShaderProperty(string propertyName);
        /// <summary>
        /// <para>Does a RenderTexture have stencil buffer?</para>
        /// </summary>
        /// <param name="rt">Render texture, or null for main screen.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SupportsStencil(RenderTexture rt);

        /// <summary>
        /// <para>Currently active render texture.</para>
        /// </summary>
        public static RenderTexture active { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The antialiasing level for the RenderTexture.</para>
        /// </summary>
        public int antiAliasing { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mipmap levels are generated automatically when this flag is set.</para>
        /// </summary>
        public bool autoGenerateMips { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Color buffer of the render texture (Read Only).</para>
        /// </summary>
        public RenderBuffer colorBuffer
        {
            get
            {
                RenderBuffer buffer;
                this.GetColorBuffer(out buffer);
                return buffer;
            }
        }

        /// <summary>
        /// <para>The precision of the render texture's depth buffer in bits (0, 16, 24/32 are supported).</para>
        /// </summary>
        public int depth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Depth/stencil buffer of the render texture (Read Only).</para>
        /// </summary>
        public RenderBuffer depthBuffer
        {
            get
            {
                RenderBuffer buffer;
                this.GetDepthBuffer(out buffer);
                return buffer;
            }
        }

        /// <summary>
        /// <para>Dimensionality (type) of the render texture.</para>
        /// </summary>
        public override TextureDimension dimension
        {
            get => 
                Internal_GetDimension(this);
            set
            {
                Internal_SetDimension(this, value);
            }
        }

        [Obsolete("RenderTexture.enabled is always now, no need to use it")]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enable random access write into this render texture on Shader Model 5.0 level shaders.</para>
        /// </summary>
        public bool enableRandomWrite { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The color format of the render texture.</para>
        /// </summary>
        public RenderTextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Use RenderTexture.autoGenerateMips instead (UnityUpgradable) -> autoGenerateMips", false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool generateMips
        {
            get => 
                this.autoGenerateMips;
            set
            {
                this.autoGenerateMips = value;
            }
        }

        /// <summary>
        /// <para>The height of the render texture in pixels.</para>
        /// </summary>
        public override int height
        {
            get => 
                Internal_GetHeight(this);
            set
            {
                Internal_SetHeight(this, value);
            }
        }

        [Obsolete("Use RenderTexture.dimension instead.")]
        public bool isCubemap { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public bool isPowerOfTwo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If enabled, this Render Texture will be used as a Texture3D.</para>
        /// </summary>
        [Obsolete("Use RenderTexture.dimension instead.")]
        public bool isVolume { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Does this render texture use sRGB read/write conversions (Read Only).</para>
        /// </summary>
        public bool sRGB { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Render texture has mipmaps when this flag is set.</para>
        /// </summary>
        public bool useMipMap { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Volume extent of a 3D render texture.</para>
        /// </summary>
        public int volumeDepth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The width of the render texture in pixels.</para>
        /// </summary>
        public override int width
        {
            get => 
                Internal_GetWidth(this);
            set
            {
                Internal_SetWidth(this, value);
            }
        }
    }
}

