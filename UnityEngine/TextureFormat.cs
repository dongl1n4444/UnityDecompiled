namespace UnityEngine
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// <para>Format used when creating textures from scripts.</para>
    /// </summary>
    public enum TextureFormat
    {
        /// <summary>
        /// <para>Alpha-only texture format.</para>
        /// </summary>
        Alpha8 = 1,
        /// <summary>
        /// <para>Color with alpha texture format, 8-bits per channel.</para>
        /// </summary>
        ARGB32 = 5,
        /// <summary>
        /// <para>A 16 bits/pixel texture format. Texture stores color with an alpha channel.</para>
        /// </summary>
        ARGB4444 = 2,
        /// <summary>
        /// <para>ASTC (10x10 pixel block in 128 bits) compressed RGB texture format.</para>
        /// </summary>
        ASTC_RGB_10x10 = 0x34,
        /// <summary>
        /// <para>ASTC (12x12 pixel block in 128 bits) compressed RGB texture format.</para>
        /// </summary>
        ASTC_RGB_12x12 = 0x35,
        /// <summary>
        /// <para>ASTC (4x4 pixel block in 128 bits) compressed RGB texture format.</para>
        /// </summary>
        ASTC_RGB_4x4 = 0x30,
        /// <summary>
        /// <para>ASTC (5x5 pixel block in 128 bits) compressed RGB texture format.</para>
        /// </summary>
        ASTC_RGB_5x5 = 0x31,
        /// <summary>
        /// <para>ASTC (6x6 pixel block in 128 bits) compressed RGB texture format.</para>
        /// </summary>
        ASTC_RGB_6x6 = 50,
        /// <summary>
        /// <para>ASTC (8x8 pixel block in 128 bits) compressed RGB texture format.</para>
        /// </summary>
        ASTC_RGB_8x8 = 0x33,
        /// <summary>
        /// <para>ASTC (10x10 pixel block in 128 bits) compressed RGBA texture format.</para>
        /// </summary>
        ASTC_RGBA_10x10 = 0x3a,
        /// <summary>
        /// <para>ASTC (12x12 pixel block in 128 bits) compressed RGBA texture format.</para>
        /// </summary>
        ASTC_RGBA_12x12 = 0x3b,
        /// <summary>
        /// <para>ASTC (4x4 pixel block in 128 bits) compressed RGBA texture format.</para>
        /// </summary>
        ASTC_RGBA_4x4 = 0x36,
        /// <summary>
        /// <para>ASTC (5x5 pixel block in 128 bits) compressed RGBA texture format.</para>
        /// </summary>
        ASTC_RGBA_5x5 = 0x37,
        /// <summary>
        /// <para>ASTC (6x6 pixel block in 128 bits) compressed RGBA texture format.</para>
        /// </summary>
        ASTC_RGBA_6x6 = 0x38,
        /// <summary>
        /// <para>ASTC (8x8 pixel block in 128 bits) compressed RGBA texture format.</para>
        /// </summary>
        ASTC_RGBA_8x8 = 0x39,
        /// <summary>
        /// <para>ATC (ATITC) 4 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ATC_RGB4 = 0x23,
        /// <summary>
        /// <para>ATC (ATITC) 8 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ATC_RGBA8 = 0x24,
        /// <summary>
        /// <para>Compressed one channel (R) texture format.</para>
        /// </summary>
        BC4 = 0x1a,
        /// <summary>
        /// <para>Compressed two-channel (RG) texture format.</para>
        /// </summary>
        BC5 = 0x1b,
        /// <summary>
        /// <para>HDR compressed color texture format.</para>
        /// </summary>
        BC6H = 0x18,
        /// <summary>
        /// <para>High quality compressed color texture format.</para>
        /// </summary>
        BC7 = 0x19,
        /// <summary>
        /// <para>Color with alpha texture format, 8-bits per channel.</para>
        /// </summary>
        BGRA32 = 14,
        /// <summary>
        /// <para>Compressed color texture format.</para>
        /// </summary>
        DXT1 = 10,
        /// <summary>
        /// <para>Compressed color texture format with Crunch compression for small storage sizes.</para>
        /// </summary>
        DXT1Crunched = 0x1c,
        /// <summary>
        /// <para>Compressed color with alpha channel texture format.</para>
        /// </summary>
        DXT5 = 12,
        /// <summary>
        /// <para>Compressed color with alpha channel texture format with Crunch compression for small storage sizes.</para>
        /// </summary>
        DXT5Crunched = 0x1d,
        /// <summary>
        /// <para>ETC2  EAC (GL ES 3.0) 4 bitspixel compressed unsigned single-channel texture format.</para>
        /// </summary>
        EAC_R = 0x29,
        /// <summary>
        /// <para>ETC2  EAC (GL ES 3.0) 4 bitspixel compressed signed single-channel texture format.</para>
        /// </summary>
        EAC_R_SIGNED = 0x2a,
        /// <summary>
        /// <para>ETC2  EAC (GL ES 3.0) 8 bitspixel compressed unsigned dual-channel (RG) texture format.</para>
        /// </summary>
        EAC_RG = 0x2b,
        /// <summary>
        /// <para>ETC2  EAC (GL ES 3.0) 8 bitspixel compressed signed dual-channel (RG) texture format.</para>
        /// </summary>
        EAC_RG_SIGNED = 0x2c,
        /// <summary>
        /// <para>ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ETC_RGB4 = 0x22,
        /// <summary>
        /// <para>ETC 4 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ETC_RGB4_3DS = 60,
        /// <summary>
        /// <para>ETC 4 bitspixel RGB + 4 bitspixel Alpha compressed texture format.</para>
        /// </summary>
        ETC_RGBA8_3DS = 0x3d,
        /// <summary>
        /// <para>ETC2 (GL ES 3.0) 4 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ETC2_RGB = 0x2d,
        /// <summary>
        /// <para>ETC2 (GL ES 3.0) 4 bits/pixel RGB+1-bit alpha texture format.</para>
        /// </summary>
        ETC2_RGBA1 = 0x2e,
        /// <summary>
        /// <para>ETC2 (GL ES 3.0) 8 bits/pixel compressed RGBA texture format.</para>
        /// </summary>
        ETC2_RGBA8 = 0x2f,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member TextureFormat.PVRTC_2BPP_RGB has been deprecated. Use PVRTC_RGB2 instead (UnityUpgradable) -> PVRTC_RGB2", true)]
        PVRTC_2BPP_RGB = -127,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member TextureFormat.PVRTC_2BPP_RGBA has been deprecated. Use PVRTC_RGBA2 instead (UnityUpgradable) -> PVRTC_RGBA2", true)]
        PVRTC_2BPP_RGBA = -127,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member TextureFormat.PVRTC_4BPP_RGB has been deprecated. Use PVRTC_RGB4 instead (UnityUpgradable) -> PVRTC_RGB4", true)]
        PVRTC_4BPP_RGB = -127,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member TextureFormat.PVRTC_4BPP_RGBA has been deprecated. Use PVRTC_RGBA4 instead (UnityUpgradable) -> PVRTC_RGBA4", true)]
        PVRTC_4BPP_RGBA = -127,
        /// <summary>
        /// <para>PowerVR (iOS) 2 bits/pixel compressed color texture format.</para>
        /// </summary>
        PVRTC_RGB2 = 30,
        /// <summary>
        /// <para>PowerVR (iOS) 4 bits/pixel compressed color texture format.</para>
        /// </summary>
        PVRTC_RGB4 = 0x20,
        /// <summary>
        /// <para>PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format.</para>
        /// </summary>
        PVRTC_RGBA2 = 0x1f,
        /// <summary>
        /// <para>PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format.</para>
        /// </summary>
        PVRTC_RGBA4 = 0x21,
        /// <summary>
        /// <para>A 16 bit color texture format that only has a red channel.</para>
        /// </summary>
        R16 = 9,
        /// <summary>
        /// <para>Scalar (R) texture format, 32 bit floating point.</para>
        /// </summary>
        RFloat = 0x12,
        /// <summary>
        /// <para>Color texture format, 8-bits per channel.</para>
        /// </summary>
        RGB24 = 3,
        /// <summary>
        /// <para>A 16 bit color texture format.</para>
        /// </summary>
        RGB565 = 7,
        /// <summary>
        /// <para>RGB HDR format, with 9 bit mantissa per channel and a 5 bit shared exponent.</para>
        /// </summary>
        RGB9e5Float = 0x16,
        /// <summary>
        /// <para>Color with alpha texture format, 8-bits per channel.</para>
        /// </summary>
        RGBA32 = 4,
        /// <summary>
        /// <para>Color and alpha  texture format, 4 bit per channel.</para>
        /// </summary>
        RGBA4444 = 13,
        /// <summary>
        /// <para>RGB color and alpha texture format,  32-bit floats per channel.</para>
        /// </summary>
        RGBAFloat = 20,
        /// <summary>
        /// <para>RGB color and alpha texture format, 16 bit floating point per channel.</para>
        /// </summary>
        RGBAHalf = 0x11,
        /// <summary>
        /// <para>Two color (RG)  texture format, 32 bit floating point per channel.</para>
        /// </summary>
        RGFloat = 0x13,
        /// <summary>
        /// <para>Two color (RG)  texture format, 16 bit floating point per channel.</para>
        /// </summary>
        RGHalf = 0x10,
        /// <summary>
        /// <para>Scalar (R)  texture format, 16 bit floating point.</para>
        /// </summary>
        RHalf = 15,
        /// <summary>
        /// <para>A format that uses the YUV color space and is often used for video encoding or playback.</para>
        /// </summary>
        YUY2 = 0x15
    }
}

