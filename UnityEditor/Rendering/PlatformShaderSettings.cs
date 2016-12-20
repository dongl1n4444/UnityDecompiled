namespace UnityEditor.Rendering
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Used to set up shader settings, per-platorm and per-shader-hardware-tier.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Obsolete("Use TierSettings instead (UnityUpgradable) -> UnityEditor.Rendering.TierSettings", false)]
    public struct PlatformShaderSettings
    {
        /// <summary>
        /// <para>Allows you to specify whether cascaded shadow maps should be used.</para>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool cascadedShadowMaps;
        /// <summary>
        /// <para>Allows you to specify whether Reflection Probes Box Projection should be used.</para>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool reflectionProbeBoxProjection;
        /// <summary>
        /// <para>Allows you to specify whether Reflection Probes Blending should be enabled.</para>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool reflectionProbeBlending;
        /// <summary>
        /// <para>Allows you to select Standard Shader Quality.</para>
        /// </summary>
        public ShaderQuality standardShaderQuality;
    }
}

