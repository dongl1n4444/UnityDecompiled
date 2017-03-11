namespace UnityEditor
{
    using System;
    using Unity.Bindings;

    /// <summary>
    /// <para>Target build platform.</para>
    /// </summary>
    [NativeEnum(Name="BuildTargetPlatform", Header="Runtime/Serialize/SerializationMetaFlags.h")]
    public enum BuildTarget
    {
        /// <summary>
        /// <para>Build an Android .apk standalone app.</para>
        /// </summary>
        Android = 13,
        [Obsolete("BlackBerry has been removed in 5.4")]
        BB10 = -1,
        [Obsolete("BlackBerry has been removed in 5.4")]
        BlackBerry = 0x1c,
        /// <summary>
        /// <para>Build an iOS player.</para>
        /// </summary>
        iOS = 9,
        /// <summary>
        /// <para>OBSOLETE: Use iOS. Build an iOS player.</para>
        /// </summary>
        [Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
        iPhone = -1,
        [Obsolete("Use WSAPlayer instead (UnityUpgradable) -> WSAPlayer", true)]
        MetroPlayer = -1,
        /// <summary>
        /// <para>Build to Nintendo 3DS platform.</para>
        /// </summary>
        N3DS = 0x23,
        NoTarget = -2,
        [Obsolete("PS3 has been removed in >=5.5")]
        PS3 = 10,
        /// <summary>
        /// <para>Build a PS4 Standalone.</para>
        /// </summary>
        PS4 = 0x1f,
        PSM = 0x20,
        /// <summary>
        /// <para>Build a PS Vita Standalone.</para>
        /// </summary>
        PSP2 = 30,
        /// <summary>
        /// <para>Build to Samsung Smart TV platform.</para>
        /// </summary>
        SamsungTV = 0x22,
        /// <summary>
        /// <para>Build a Linux standalone.</para>
        /// </summary>
        StandaloneLinux = 0x11,
        /// <summary>
        /// <para>Build a Linux 64-bit standalone.</para>
        /// </summary>
        StandaloneLinux64 = 0x18,
        /// <summary>
        /// <para>Build a Linux universal standalone.</para>
        /// </summary>
        StandaloneLinuxUniversal = 0x19,
        /// <summary>
        /// <para>Build an OS X standalone (Intel only).</para>
        /// </summary>
        StandaloneOSXIntel = 4,
        /// <summary>
        /// <para>Build an OSX Intel 64-bit standalone.</para>
        /// </summary>
        StandaloneOSXIntel64 = 0x1b,
        /// <summary>
        /// <para>Build a universal OSX standalone.</para>
        /// </summary>
        StandaloneOSXUniversal = 2,
        /// <summary>
        /// <para>Build a Windows standalone.</para>
        /// </summary>
        StandaloneWindows = 5,
        /// <summary>
        /// <para>Build a Windows 64-bit standalone.</para>
        /// </summary>
        StandaloneWindows64 = 0x13,
        /// <summary>
        /// <para>Build a Nintendo Switch player.</para>
        /// </summary>
        Switch = 0x26,
        /// <summary>
        /// <para>Build a Tizen player.</para>
        /// </summary>
        Tizen = 0x1d,
        /// <summary>
        /// <para>Build to Apple's tvOS platform.</para>
        /// </summary>
        tvOS = 0x25,
        /// <summary>
        /// <para>WebGL.</para>
        /// </summary>
        WebGL = 20,
        /// <summary>
        /// <para>Build a web player. (This build target is deprecated. Building for web player will no longer be supported in future versions of Unity.)</para>
        /// </summary>
        [Obsolete("WebPlayer has been removed in 5.4")]
        WebPlayer = 6,
        /// <summary>
        /// <para>Build a streamed web player.</para>
        /// </summary>
        [Obsolete("WebPlayerStreamed has been removed in 5.4")]
        WebPlayerStreamed = 7,
        /// <summary>
        /// <para>Build a Wii U standalone.</para>
        /// </summary>
        WiiU = 0x24,
        [Obsolete("Use WSAPlayer with Windows Phone 8.1 selected")]
        WP8Player = 0x1a,
        /// <summary>
        /// <para>Build an Windows Store Apps player.</para>
        /// </summary>
        WSAPlayer = 0x15,
        [Obsolete("XBOX360 has been removed in 5.5")]
        XBOX360 = 11,
        /// <summary>
        /// <para>Build a Xbox One Standalone.</para>
        /// </summary>
        XboxOne = 0x21
    }
}

