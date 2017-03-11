namespace UnityEditor
{
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AndroidBuildSubtarget has been deprecated. Use UnityEditor.MobileTextureSubtarget instead (UnityUpgradable) -> MobileTextureSubtarget", true)]
    public enum AndroidBuildSubtarget
    {
        ASTC = -1,
        ATC = -1,
        DXT = -1,
        ETC = -1,
        ETC2 = -1,
        Generic = -1,
        PVRTC = -1
    }
}

