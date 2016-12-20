namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Imported texture format for TextureImporter.</para>
    /// </summary>
    public enum TextureImporterFormat
    {
        /// <summary>
        /// <para>TextureFormat.Alpha8 texture format.</para>
        /// </summary>
        Alpha8 = 1,
        /// <summary>
        /// <para>TextureFormat.ARGB4444 texture format.</para>
        /// </summary>
        ARGB16 = 2,
        /// <summary>
        /// <para>TextureFormat.ARGB32 texture format.</para>
        /// </summary>
        ARGB32 = 5,
        /// <summary>
        /// <para>ASTC compressed RGB texture format, 10x10 block size.</para>
        /// </summary>
        ASTC_RGB_10x10 = 0x34,
        /// <summary>
        /// <para>ASTC compressed RGB texture format, 12x12 block size.</para>
        /// </summary>
        ASTC_RGB_12x12 = 0x35,
        /// <summary>
        /// <para>ASTC compressed RGB texture format, 4x4 block size.</para>
        /// </summary>
        ASTC_RGB_4x4 = 0x30,
        /// <summary>
        /// <para>ASTC compressed RGB texture format, 5x5 block size.</para>
        /// </summary>
        ASTC_RGB_5x5 = 0x31,
        /// <summary>
        /// <para>ASTC compressed RGB texture format, 6x6 block size.</para>
        /// </summary>
        ASTC_RGB_6x6 = 50,
        /// <summary>
        /// <para>ASTC compressed RGB texture format, 8x8 block size.</para>
        /// </summary>
        ASTC_RGB_8x8 = 0x33,
        /// <summary>
        /// <para>ASTC compressed RGBA texture format, 10x10 block size.</para>
        /// </summary>
        ASTC_RGBA_10x10 = 0x3a,
        /// <summary>
        /// <para>ASTC compressed RGBA texture format, 12x12 block size.</para>
        /// </summary>
        ASTC_RGBA_12x12 = 0x3b,
        /// <summary>
        /// <para>ASTC compressed RGBA texture format, 4x4 block size.</para>
        /// </summary>
        ASTC_RGBA_4x4 = 0x36,
        /// <summary>
        /// <para>ASTC compressed RGBA texture format, 5x5 block size.</para>
        /// </summary>
        ASTC_RGBA_5x5 = 0x37,
        /// <summary>
        /// <para>ASTC compressed RGBA texture format, 6x6 block size.</para>
        /// </summary>
        ASTC_RGBA_6x6 = 0x38,
        /// <summary>
        /// <para>ASTC compressed RGBA texture format, 8x8 block size.</para>
        /// </summary>
        ASTC_RGBA_8x8 = 0x39,
        /// <summary>
        /// <para>ATC (Android) 4 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ATC_RGB4 = 0x23,
        /// <summary>
        /// <para>ATC (Android) 8 bits/pixel compressed RGBA texture format.</para>
        /// </summary>
        ATC_RGBA8 = 0x24,
        /// <summary>
        /// <para>Choose texture format automatically based on the texture parameters.</para>
        /// </summary>
        Automatic = -1,
        /// <summary>
        /// <para>Choose a 16 bit format automatically.</para>
        /// </summary>
        [Obsolete("Use textureCompression property instead")]
        Automatic16bit = -2,
        /// <summary>
        /// <para>Choose a compressed format automatically.</para>
        /// </summary>
        [Obsolete("Use textureCompression property instead")]
        AutomaticCompressed = -1,
        /// <summary>
        /// <para>Choose a compressed HDR format automatically.</para>
        /// </summary>
        [Obsolete("HDR is handled automatically now")]
        AutomaticCompressedHDR = -7,
        /// <summary>
        /// <para>Choose a crunched format automatically.</para>
        /// </summary>
        [Obsolete("Use crunchedCompression property instead")]
        AutomaticCrunched = -5,
        /// <summary>
        /// <para>Choose an HDR format automatically.</para>
        /// </summary>
        [Obsolete("HDR is handled automatically now")]
        AutomaticHDR = -6,
        /// <summary>
        /// <para>Choose a Truecolor format automatically.</para>
        /// </summary>
        [Obsolete("Use textureCompression property instead")]
        AutomaticTruecolor = -3,
        /// <summary>
        /// <para>TextureFormat.BC4 compressed texture format.</para>
        /// </summary>
        BC4 = 0x1a,
        /// <summary>
        /// <para>TextureFormat.BC5 compressed texture format.</para>
        /// </summary>
        BC5 = 0x1b,
        /// <summary>
        /// <para>TextureFormat.BC6H compressed HDR texture format.</para>
        /// </summary>
        BC6H = 0x18,
        /// <summary>
        /// <para>TextureFormat.BC7 compressed texture format.</para>
        /// </summary>
        BC7 = 0x19,
        /// <summary>
        /// <para>TextureFormat.DXT1 compressed texture format.</para>
        /// </summary>
        DXT1 = 10,
        /// <summary>
        /// <para>DXT1 compressed texture format with Crunch compression for small storage sizes.</para>
        /// </summary>
        DXT1Crunched = 0x1c,
        /// <summary>
        /// <para>TextureFormat.DXT5 compressed texture format.</para>
        /// </summary>
        DXT5 = 12,
        /// <summary>
        /// <para>DXT5 compressed texture format with Crunch compression for small storage sizes.</para>
        /// </summary>
        DXT5Crunched = 0x1d,
        /// <summary>
        /// <para>ETC2EAC compressed 4 bits  pixel unsigned R texture format.</para>
        /// </summary>
        EAC_R = 0x29,
        /// <summary>
        /// <para>ETC2EAC compressed 4 bits  pixel signed R texture format.</para>
        /// </summary>
        EAC_R_SIGNED = 0x2a,
        /// <summary>
        /// <para>ETC2EAC compressed 8 bits  pixel unsigned RG texture format.</para>
        /// </summary>
        EAC_RG = 0x2b,
        /// <summary>
        /// <para>ETC2EAC compressed 4 bits  pixel signed RG texture format.</para>
        /// </summary>
        EAC_RG_SIGNED = 0x2c,
        /// <summary>
        /// <para>ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.</para>
        /// </summary>
        ETC_RGB4 = 0x22,
        /// <summary>
        /// <para>ETC2 compressed 4 bits / pixel RGB texture format.</para>
        /// </summary>
        ETC2_RGB4 = 0x2d,
        /// <summary>
        /// <para>ETC2 compressed 4 bits / pixel RGB + 1-bit alpha texture format.</para>
        /// </summary>
        ETC2_RGB4_PUNCHTHROUGH_ALPHA = 0x2e,
        /// <summary>
        /// <para>ETC2 compressed 8 bits / pixel RGBA texture format.</para>
        /// </summary>
        ETC2_RGBA8 = 0x2f,
        /// <summary>
        /// <para>PowerVR/iOS TextureFormat.PVRTC_RGB2 compressed texture format.</para>
        /// </summary>
        PVRTC_RGB2 = 30,
        /// <summary>
        /// <para>PowerVR/iOS TextureFormat.PVRTC_RGB4 compressed texture format.</para>
        /// </summary>
        PVRTC_RGB4 = 0x20,
        /// <summary>
        /// <para>PowerVR/iOS TextureFormat.PVRTC_RGBA2 compressed texture format.</para>
        /// </summary>
        PVRTC_RGBA2 = 0x1f,
        /// <summary>
        /// <para>PowerVR/iOS TextureFormat.PVRTC_RGBA4 compressed texture format.</para>
        /// </summary>
        PVRTC_RGBA4 = 0x21,
        /// <summary>
        /// <para>TextureFormat.RGB565 texture format.</para>
        /// </summary>
        RGB16 = 7,
        /// <summary>
        /// <para>TextureFormat.RGB24 texture format.</para>
        /// </summary>
        RGB24 = 3,
        /// <summary>
        /// <para>TextureFormat.RGBA4444 texture format.</para>
        /// </summary>
        RGBA16 = 13,
        /// <summary>
        /// <para>TextureFormat.RGBA32 texture format.</para>
        /// </summary>
        RGBA32 = 4,
        /// <summary>
        /// <para>TextureFormat.RGBAHalf floating point texture format.</para>
        /// </summary>
        RGBAHalf = 0x11
    }
}

