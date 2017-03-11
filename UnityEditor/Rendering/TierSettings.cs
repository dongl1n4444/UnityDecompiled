namespace UnityEditor.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

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
        /// <para>The CameraHDRMode to use for this tier.</para>
        /// </summary>
        public CameraHDRMode hdrMode;
        /// <summary>
        /// <para>Allows you to specify whether Reflection Probes Box Projection should be used.</para>
        /// </summary>
        public bool reflectionProbeBoxProjection;
        /// <summary>
        /// <para>Allows you to specify whether Reflection Probes Blending should be enabled.</para>
        /// </summary>
        public bool reflectionProbeBlending;
        /// <summary>
        /// <para>Setting this field to true enables HDR rendering for this tier. Setting it to false disables HDR rendering for this tier.
        /// See Also:</para>
        /// </summary>
        public bool hdr;
        /// <summary>
        /// <para>Allows you to specify whether Detail Normal Map should be sampled if assigned.</para>
        /// </summary>
        public bool detailNormalMap;
        /// <summary>
        /// <para>Allows you to specify whether cascaded shadow maps should be used.</para>
        /// </summary>
        public bool cascadedShadowMaps;
        /// <summary>
        /// <para>Allows you to specify whether Semitransparent Shadows should be enabled.</para>
        /// </summary>
        public bool semitransparentShadows;
        /// <summary>
        /// <para>The rendering path that should be used.</para>
        /// </summary>
        public RenderingPath renderingPath;
        /// <summary>
        /// <para>The RealtimeGICPUUsage to use for this tier.</para>
        /// </summary>
        public RealtimeGICPUUsage realtimeGICPUUsage;
    }
}

