namespace UnityEngine.Rendering
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Graphics device API type.</para>
    /// </summary>
    [UsedByNativeCode]
    public enum GraphicsDeviceType
    {
        /// <summary>
        /// <para>Direct3D 11 graphics API.</para>
        /// </summary>
        Direct3D11 = 2,
        /// <summary>
        /// <para>Direct3D 12 graphics API.</para>
        /// </summary>
        Direct3D12 = 0x12,
        /// <summary>
        /// <para>Direct3D 9 graphics API.</para>
        /// </summary>
        Direct3D9 = 1,
        /// <summary>
        /// <para>iOS Metal graphics API.</para>
        /// </summary>
        Metal = 0x10,
        /// <summary>
        /// <para>Nintendo 3DS graphics API.</para>
        /// </summary>
        N3DS = 0x13,
        /// <summary>
        /// <para>No graphics API.</para>
        /// </summary>
        Null = 4,
        /// <summary>
        /// <para>OpenGL 2.x graphics API. (deprecated, only available on Linux and MacOSX)</para>
        /// </summary>
        [Obsolete("OpenGL2 is no longer supported in Unity 5.5+")]
        OpenGL2 = 0,
        /// <summary>
        /// <para>OpenGL (Core profile - GL3 or later) graphics API.</para>
        /// </summary>
        OpenGLCore = 0x11,
        /// <summary>
        /// <para>OpenGL ES 2.0 graphics API.</para>
        /// </summary>
        OpenGLES2 = 8,
        /// <summary>
        /// <para>OpenGL ES 3.0 graphics API.</para>
        /// </summary>
        OpenGLES3 = 11,
        /// <summary>
        /// <para>PlayStation 3 graphics API.</para>
        /// </summary>
        [Obsolete("PS3 is no longer supported in Unity 5.5+")]
        PlayStation3 = 3,
        /// <summary>
        /// <para>PlayStation 4 graphics API.</para>
        /// </summary>
        PlayStation4 = 13,
        /// <summary>
        /// <para>PlayStation Mobile (PSM) graphics API.</para>
        /// </summary>
        PlayStationMobile = 15,
        /// <summary>
        /// <para>PlayStation Vita graphics API.</para>
        /// </summary>
        PlayStationVita = 12,
        /// <summary>
        /// <para>Vulkan (EXPERIMENTAL).</para>
        /// </summary>
        Vulkan = 0x15,
        [Obsolete("Xbox360 is no longer supported in Unity 5.5+")]
        Xbox360 = 6,
        /// <summary>
        /// <para>Xbox One graphics API.</para>
        /// </summary>
        XboxOne = 14
    }
}

