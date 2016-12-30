namespace UnityEngine.Experimental.Rendering
{
    using System;

    /// <summary>
    /// <para>What kind of per-object data to setup during rendering.</para>
    /// </summary>
    [Flags]
    public enum RendererConfiguration
    {
        /// <summary>
        /// <para>Do not setup any particular per-object data besides the transformation matrix.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Setup per-object lightmaps.</para>
        /// </summary>
        PerObjectLightmaps = 8,
        /// <summary>
        /// <para>Setup per-object light probe SH data.</para>
        /// </summary>
        PerObjectLightProbe = 1,
        /// <summary>
        /// <para>Setup per-object light probe proxy volume data.</para>
        /// </summary>
        PerObjectLightProbeProxyVolume = 4,
        /// <summary>
        /// <para>Setup per-object reflection probe data.</para>
        /// </summary>
        PerObjectReflectionProbes = 2,
        /// <summary>
        /// <para>Setup per-object light indices.</para>
        /// </summary>
        ProvideLightIndices = 0x10
    }
}

