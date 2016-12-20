namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Compressed texture format for target build platform.</para>
    /// </summary>
    public enum MobileTextureSubtarget
    {
        Generic,
        DXT,
        PVRTC,
        ATC,
        ETC,
        ETC2,
        ASTC
    }
}

