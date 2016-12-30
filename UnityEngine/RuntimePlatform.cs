namespace UnityEngine
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// <para>The platform application is running. Returned by Application.platform.</para>
    /// </summary>
    public enum RuntimePlatform
    {
        /// <summary>
        /// <para>In the player on Android devices.</para>
        /// </summary>
        Android = 11,
        [Obsolete("BB10Player export is no longer supported in Unity 5.4+."), EditorBrowsable(EditorBrowsableState.Never)]
        BB10Player = 0x16,
        [Obsolete("BlackBerryPlayer export is no longer supported in Unity 5.4+.")]
        BlackBerryPlayer = 0x16,
        [Obsolete("FlashPlayer export is no longer supported in Unity 5.0+.")]
        FlashPlayer = 15,
        /// <summary>
        /// <para>In the player on the iPhone.</para>
        /// </summary>
        IPhonePlayer = 8,
        /// <summary>
        /// <para>In the Unity editor on Linux.</para>
        /// </summary>
        LinuxEditor = 0x10,
        /// <summary>
        /// <para>In the player on Linux.</para>
        /// </summary>
        LinuxPlayer = 13,
        [Obsolete("Use WSAPlayerARM instead")]
        MetroPlayerARM = 20,
        [Obsolete("Use WSAPlayerX64 instead")]
        MetroPlayerX64 = 0x13,
        [Obsolete("Use WSAPlayerX86 instead")]
        MetroPlayerX86 = 0x12,
        [Obsolete("NaCl export is no longer supported in Unity 5.0+.")]
        NaCl = 12,
        /// <summary>
        /// <para>In the Dashboard widget on Mac OS X.</para>
        /// </summary>
        OSXDashboardPlayer = 4,
        /// <summary>
        /// <para>In the Unity editor on Mac OS X.</para>
        /// </summary>
        OSXEditor = 0,
        /// <summary>
        /// <para>In the player on Mac OS X.</para>
        /// </summary>
        OSXPlayer = 1,
        /// <summary>
        /// <para>In the web player on Mac OS X.</para>
        /// </summary>
        [Obsolete("WebPlayer export is no longer supported in Unity 5.4+.")]
        OSXWebPlayer = 3,
        [Obsolete("PS3 export is no longer supported in Unity >=5.5.")]
        PS3 = 9,
        /// <summary>
        /// <para>In the player on the Playstation 4.</para>
        /// </summary>
        PS4 = 0x19,
        PSM = 0x1a,
        /// <summary>
        /// <para>In the player on the PS Vita.</para>
        /// </summary>
        PSP2 = 0x18,
        /// <summary>
        /// <para>In the player on Samsung Smart TV.</para>
        /// </summary>
        SamsungTVPlayer = 0x1c,
        /// <summary>
        /// <para>In the player on Nintendo Switch.</para>
        /// </summary>
        Switch = 0x20,
        /// <summary>
        /// <para>In the player on Tizen.</para>
        /// </summary>
        TizenPlayer = 0x17,
        /// <summary>
        /// <para>In the player on the Apple's tvOS.</para>
        /// </summary>
        tvOS = 0x1f,
        /// <summary>
        /// <para>In the player on WebGL?</para>
        /// </summary>
        WebGLPlayer = 0x11,
        /// <summary>
        /// <para>In the player on Wii U.</para>
        /// </summary>
        WiiU = 30,
        /// <summary>
        /// <para>In the Unity editor on Windows.</para>
        /// </summary>
        WindowsEditor = 7,
        /// <summary>
        /// <para>In the player on Windows.</para>
        /// </summary>
        WindowsPlayer = 2,
        /// <summary>
        /// <para>In the web player on Windows.</para>
        /// </summary>
        [Obsolete("WebPlayer export is no longer supported in Unity 5.4+.")]
        WindowsWebPlayer = 5,
        [Obsolete("Windows Phone 8 was removed in 5.3")]
        WP8Player = 0x15,
        /// <summary>
        /// <para>In the player on Windows Store Apps when CPU architecture is ARM.</para>
        /// </summary>
        WSAPlayerARM = 20,
        /// <summary>
        /// <para>In the player on Windows Store Apps when CPU architecture is X64.</para>
        /// </summary>
        WSAPlayerX64 = 0x13,
        /// <summary>
        /// <para>In the player on Windows Store Apps when CPU architecture is X86.</para>
        /// </summary>
        WSAPlayerX86 = 0x12,
        [Obsolete("Xbox360 export is no longer supported in Unity 5.5+.")]
        XBOX360 = 10,
        /// <summary>
        /// <para>In the player on Xbox One.</para>
        /// </summary>
        XboxOne = 0x1b
    }
}

