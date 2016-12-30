namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>.NET API compatibility level.</para>
    /// </summary>
    public enum ApiCompatibilityLevel
    {
        /// <summary>
        /// <para>.NET 2.0.</para>
        /// </summary>
        NET_2_0 = 1,
        /// <summary>
        /// <para>.NET 2.0 Subset.</para>
        /// </summary>
        NET_2_0_Subset = 2,
        /// <summary>
        /// <para>.NET 4.6.</para>
        /// </summary>
        NET_4_6 = 3,
        /// <summary>
        /// <para>Micro profile, used by Mono scripting backend on iOS, tvOS, Android and Tizen if stripping level is set to "Use micro mscorlib".</para>
        /// </summary>
        NET_Micro = 5,
        /// <summary>
        /// <para>Web profile, used only by Samsung TV.</para>
        /// </summary>
        NET_Web = 4
    }
}

