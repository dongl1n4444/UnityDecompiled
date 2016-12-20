namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Supported iOS deployment versions.</para>
    /// </summary>
    [Obsolete("targetOSVersion is obsolete, use targetOSVersionString", false)]
    public enum iOSTargetOSVersion
    {
        /// <summary>
        /// <para>iOS 4.0.</para>
        /// </summary>
        iOS_4_0 = 10,
        /// <summary>
        /// <para>iOS 4.1.</para>
        /// </summary>
        iOS_4_1 = 12,
        /// <summary>
        /// <para>iOS 4.2.</para>
        /// </summary>
        iOS_4_2 = 14,
        /// <summary>
        /// <para>iOS 4.3.</para>
        /// </summary>
        iOS_4_3 = 0x10,
        /// <summary>
        /// <para>iOS 5.0.</para>
        /// </summary>
        iOS_5_0 = 0x12,
        /// <summary>
        /// <para>iOS 5.1.</para>
        /// </summary>
        iOS_5_1 = 20,
        /// <summary>
        /// <para>iOS 6.0.</para>
        /// </summary>
        iOS_6_0 = 0x16,
        /// <summary>
        /// <para>iOS 7.0.</para>
        /// </summary>
        iOS_7_0 = 0x18,
        /// <summary>
        /// <para>iOS 7.1.</para>
        /// </summary>
        iOS_7_1 = 0x1a,
        /// <summary>
        /// <para>iOS 8.0.</para>
        /// </summary>
        iOS_8_0 = 0x1c,
        /// <summary>
        /// <para>iOS 8.1.</para>
        /// </summary>
        iOS_8_1 = 30,
        /// <summary>
        /// <para>Unknown iOS version, managed by user.</para>
        /// </summary>
        Unknown = 0x3e7
    }
}

