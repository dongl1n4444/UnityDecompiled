namespace UnityEditor
{
    using System;
    using Unity.Bindings;

    /// <summary>
    /// <para>Build target group.</para>
    /// </summary>
    [NativeEnum(GenerateNativeType=false, Name="BuildTargetPlatformGroup", Header="Editor/Src/BuildPipeline/BuildTargetPlatformSpecific.h")]
    public enum BuildTargetGroup
    {
        /// <summary>
        /// <para>Android target.</para>
        /// </summary>
        Android = 7,
        [Obsolete("BlackBerry has been removed as of 5.4")]
        BlackBerry = 0x10,
        /// <summary>
        /// <para>Facebook target.</para>
        /// </summary>
        Facebook = 0x1a,
        /// <summary>
        /// <para>Apple iOS target.</para>
        /// </summary>
        iOS = 4,
        /// <summary>
        /// <para>OBSOLETE: Use iOS. Apple iOS target.</para>
        /// </summary>
        [Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
        iPhone = 4,
        [Obsolete("Use WSA instead")]
        Metro = 14,
        /// <summary>
        /// <para>Nintendo 3DS target.</para>
        /// </summary>
        N3DS = 0x17,
        [Obsolete("PS3 has been removed in >=5.5")]
        PS3 = 5,
        /// <summary>
        /// <para>Sony Playstation 4 target.</para>
        /// </summary>
        PS4 = 0x13,
        PSM = 20,
        /// <summary>
        /// <para>Sony PS Vita target.</para>
        /// </summary>
        PSP2 = 0x12,
        /// <summary>
        /// <para>Samsung Smart TV target.</para>
        /// </summary>
        SamsungTV = 0x16,
        /// <summary>
        /// <para>Mac/PC standalone target.</para>
        /// </summary>
        Standalone = 1,
        /// <summary>
        /// <para>Nintendo Switch target.</para>
        /// </summary>
        Switch = 0x1b,
        /// <summary>
        /// <para>Samsung Tizen target.</para>
        /// </summary>
        Tizen = 0x11,
        /// <summary>
        /// <para>Apple's tvOS target.</para>
        /// </summary>
        tvOS = 0x19,
        /// <summary>
        /// <para>Unknown target.</para>
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// <para>WebGL.</para>
        /// </summary>
        WebGL = 13,
        /// <summary>
        /// <para>Mac/PC webplayer target.</para>
        /// </summary>
        [Obsolete("WebPlayer was removed in 5.4, consider using WebGL")]
        WebPlayer = 2,
        /// <summary>
        /// <para>Nintendo Wii U target.</para>
        /// </summary>
        WiiU = 0x18,
        [Obsolete("Use WSA instead")]
        WP8 = 15,
        /// <summary>
        /// <para>Windows Store Apps target.</para>
        /// </summary>
        WSA = 14,
        [Obsolete("XBOX360 has been removed in 5.5")]
        XBOX360 = 6,
        /// <summary>
        /// <para>Microsoft Xbox One target.</para>
        /// </summary>
        XboxOne = 0x15
    }
}

