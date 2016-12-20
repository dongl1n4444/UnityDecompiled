namespace UnityEngine.iOS
{
    using System;

    /// <summary>
    /// <para>iOS device generation.</para>
    /// </summary>
    public enum DeviceGeneration
    {
        /// <summary>
        /// <para>iPad, first generation.</para>
        /// </summary>
        iPad1Gen = 7,
        /// <summary>
        /// <para>iPad, second generation.</para>
        /// </summary>
        iPad2Gen = 10,
        /// <summary>
        /// <para>iPad, third generation.</para>
        /// </summary>
        iPad3Gen = 12,
        /// <summary>
        /// <para>iPad, fourth generation.</para>
        /// </summary>
        iPad4Gen = 0x10,
        /// <summary>
        /// <para>iPad Air, fifth generation.</para>
        /// </summary>
        [Obsolete("Please use iPadAir1 instead.")]
        iPad5Gen = 0x13,
        /// <summary>
        /// <para>iPad Air.</para>
        /// </summary>
        iPadAir1 = 0x13,
        /// <summary>
        /// <para>iPad Air 2.</para>
        /// </summary>
        iPadAir2 = 0x18,
        /// <summary>
        /// <para>iPadMini, first generation.</para>
        /// </summary>
        iPadMini1Gen = 15,
        /// <summary>
        /// <para>iPadMini Retina, second generation.</para>
        /// </summary>
        iPadMini2Gen = 20,
        /// <summary>
        /// <para>iPad Mini 3.</para>
        /// </summary>
        iPadMini3Gen = 0x17,
        /// <summary>
        /// <para>iPad Mini, fourth generation.</para>
        /// </summary>
        iPadMini4Gen = 0x1c,
        /// <summary>
        /// <para>iPad Pro 9.7", first generation.</para>
        /// </summary>
        iPadPro10Inch1Gen = 30,
        /// <summary>
        /// <para>iPad Pro, first generation.</para>
        /// </summary>
        iPadPro1Gen = 0x1b,
        /// <summary>
        /// <para>Yet unknown iPad.</para>
        /// </summary>
        iPadUnknown = 0x2712,
        /// <summary>
        /// <para>iPhone, first generation.</para>
        /// </summary>
        iPhone = 1,
        /// <summary>
        /// <para>iPhone, second generation.</para>
        /// </summary>
        iPhone3G = 2,
        /// <summary>
        /// <para>iPhone, third generation.</para>
        /// </summary>
        iPhone3GS = 3,
        /// <summary>
        /// <para>iPhone, fourth generation.</para>
        /// </summary>
        iPhone4 = 8,
        /// <summary>
        /// <para>iPhone, fifth generation.</para>
        /// </summary>
        iPhone4S = 11,
        /// <summary>
        /// <para>iPhone5.</para>
        /// </summary>
        iPhone5 = 13,
        /// <summary>
        /// <para>iPhone 5C.</para>
        /// </summary>
        iPhone5C = 0x11,
        /// <summary>
        /// <para>iPhone 5S.</para>
        /// </summary>
        iPhone5S = 0x12,
        /// <summary>
        /// <para>iPhone 6.</para>
        /// </summary>
        iPhone6 = 0x15,
        /// <summary>
        /// <para>iPhone 6 plus.</para>
        /// </summary>
        iPhone6Plus = 0x16,
        /// <summary>
        /// <para>iPhone 6S.</para>
        /// </summary>
        iPhone6S = 0x19,
        /// <summary>
        /// <para>iPhone 6S Plus.</para>
        /// </summary>
        iPhone6SPlus = 0x1a,
        /// <summary>
        /// <para>iPhone 7.</para>
        /// </summary>
        iPhone7 = 0x1f,
        /// <summary>
        /// <para>iPhone 7 Plus.</para>
        /// </summary>
        iPhone7Plus = 0x20,
        /// <summary>
        /// <para>iPhone SE, first generation.</para>
        /// </summary>
        iPhoneSE1Gen = 0x1d,
        /// <summary>
        /// <para>Yet unknown iPhone.</para>
        /// </summary>
        iPhoneUnknown = 0x2711,
        /// <summary>
        /// <para>iPod Touch, first generation.</para>
        /// </summary>
        iPodTouch1Gen = 4,
        /// <summary>
        /// <para>iPod Touch, second generation.</para>
        /// </summary>
        iPodTouch2Gen = 5,
        /// <summary>
        /// <para>iPod Touch, third generation.</para>
        /// </summary>
        iPodTouch3Gen = 6,
        /// <summary>
        /// <para>iPod Touch, fourth generation.</para>
        /// </summary>
        iPodTouch4Gen = 9,
        /// <summary>
        /// <para>iPod Touch, fifth generation.</para>
        /// </summary>
        iPodTouch5Gen = 14,
        /// <summary>
        /// <para>Yet unknown iPod Touch.</para>
        /// </summary>
        iPodTouchUnknown = 0x2713,
        Unknown = 0
    }
}

