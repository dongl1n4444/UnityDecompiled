namespace UnityEditor.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Used to set up per-platorm per-shader-hardware-tier graphics settings.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TierSettings
    {
        /// <summary>
        /// <para>Allows you to select Standard Shader Quality.</para>
        /// </summary>
        public ShaderQuality standardShaderQuality;
        /// <summary>
        /// <para>Allows you to specify whether cascaded shadow maps should be used.</para>
        /// </summary>
        public bool cascadedShadowMaps;
        /// <summary>
        /// <para>Allows you to specify whether Reflection Probes Box Projection should be used.</para>
        /// </summary>
        public bool reflectionProbeBoxProjection;
        /// <summary>
        /// <para>Allows you to specify whether Reflection Probes Blending should be enabled.</para>
        /// </summary>
        public bool reflectionProbeBlending;
        /// <summary>
        /// <para>The rendering path that should be used.</para>
        /// </summary>
        public RenderingPath renderingPath;
    }
}

