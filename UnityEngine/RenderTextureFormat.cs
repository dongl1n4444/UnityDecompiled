namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Format of a RenderTexture.</para>
    /// </summary>
    public enum RenderTextureFormat
    {
        /// <summary>
        /// <para>Color render texture format, 1 bit for Alpha channel, 5 bits for Red, Green and Blue channels.</para>
        /// </summary>
        ARGB1555 = 6,
        /// <summary>
        /// <para>Color render texture format. 10 bits for colors, 2 bits for alpha.</para>
        /// </summary>
        ARGB2101010 = 8,
        /// <summary>
        /// <para>Color render texture format, 8 bits per channel.</para>
        /// </summary>
        ARGB32 = 0,
        /// <summary>
        /// <para>Color render texture format, 4 bit per channel.</para>
        /// </summary>
        ARGB4444 = 5,
        /// <summary>
        /// <para>Four color render texture format, 16 bits per channel, fixed point, unsigned normalized.</para>
        /// </summary>
        ARGB64 = 10,
        /// <summary>
        /// <para>Color render texture format, 32 bit floating point per channel.</para>
        /// </summary>
        ARGBFloat = 11,
        /// <summary>
        /// <para>Color render texture format, 16 bit floating point per channel.</para>
        /// </summary>
        ARGBHalf = 2,
        /// <summary>
        /// <para>Four channel (ARGB) render texture format, 32 bit signed integer per channel.</para>
        /// </summary>
        ARGBInt = 0x11,
        /// <summary>
        /// <para>Color render texture format, 8 bits per channel.</para>
        /// </summary>
        BGRA32 = 20,
        /// <summary>
        /// <para>Default color render texture format: will be chosen accordingly to Frame Buffer format and Platform.</para>
        /// </summary>
        Default = 7,
        /// <summary>
        /// <para>Default HDR color render texture format: will be chosen accordingly to Frame Buffer format and Platform.</para>
        /// </summary>
        DefaultHDR = 9,
        /// <summary>
        /// <para>A depth render texture format.</para>
        /// </summary>
        Depth = 1,
        /// <summary>
        /// <para>Scalar (R) render texture format, 8 bit fixed point.</para>
        /// </summary>
        R8 = 0x10,
        /// <summary>
        /// <para>Scalar (R) render texture format, 32 bit floating point.</para>
        /// </summary>
        RFloat = 14,
        /// <summary>
        /// <para>Two color (RG) render texture format, 16 bits per channel, fixed point, unsigned normalized.</para>
        /// </summary>
        RG32 = 0x17,
        /// <summary>
        /// <para>Color render texture format. R and G channels are 11 bit floating point, B channel is 10 bit floating point.</para>
        /// </summary>
        RGB111110Float = 0x16,
        /// <summary>
        /// <para>Color render texture format.</para>
        /// </summary>
        RGB565 = 4,
        /// <summary>
        /// <para>Four channel (RGBA) render texture format, 16 bit unsigned integer per channel.</para>
        /// </summary>
        RGBAUShort = 0x18,
        /// <summary>
        /// <para>Two color (RG) render texture format, 32 bit floating point per channel.</para>
        /// </summary>
        RGFloat = 12,
        /// <summary>
        /// <para>Two color (RG) render texture format, 16 bit floating point per channel.</para>
        /// </summary>
        RGHalf = 13,
        /// <summary>
        /// <para>Two channel (RG) render texture format, 32 bit signed integer per channel.</para>
        /// </summary>
        RGInt = 0x12,
        /// <summary>
        /// <para>Scalar (R) render texture format, 16 bit floating point.</para>
        /// </summary>
        RHalf = 15,
        /// <summary>
        /// <para>Scalar (R) render texture format, 32 bit signed integer.</para>
        /// </summary>
        RInt = 0x13,
        /// <summary>
        /// <para>A native shadowmap render texture format.</para>
        /// </summary>
        Shadowmap = 3
    }
}

