namespace UnityEditor
{
    using System;

    public enum GameViewSizeGroupType
    {
        Android = 3,
        HMD = 9,
        iOS = 2,
        N3DS = 8,
        [Obsolete("PS3 has been removed in 5.5", false)]
        PS3 = 4,
        Standalone = 0,
        Tizen = 6,
        [Obsolete("WebPlayer has been removed in 5.4", false)]
        WebPlayer = 1,
        WiiU = 5,
        [Obsolete("Windows Phone 8 was removed in 5.3", false)]
        WP8 = 7
    }
}

