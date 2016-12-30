namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A collection of parameters that impact lightmap and realtime GI computations.</para>
    /// </summary>
    public sealed class LightmapParameters : Object
    {
        public LightmapParameters()
        {
            Internal_CreateLightmapParameters(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateLightmapParameters([Writable] LightmapParameters self);

        /// <summary>
        /// <para>The maximum number of times to supersample a texel to reduce aliasing.</para>
        /// </summary>
        public int antiAliasingSamples { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximum number of times to supersample a texel to reduce aliasing in AO.</para>
        /// </summary>
        public int AOAntiAliasingSamples { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of rays to cast for computing ambient occlusion.</para>
        /// </summary>
        public int AOQuality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The percentage of rays shot from a ray origin that must hit front faces to be considered usable.</para>
        /// </summary>
        public float backFaceTolerance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>BakedLightmapTag is an integer that affects the assignment to baked lightmaps. Objects with different values for bakedLightmapTag are guaranteed to not be assigned to the same lightmap even if the other baking parameters are the same.</para>
        /// </summary>
        public int bakedLightmapTag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The radius (in texels) of the post-processing filter that blurs baked direct lighting.</para>
        /// </summary>
        public int blurRadius { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls the resolution at which Enlighten stores and can transfer input light.</para>
        /// </summary>
        public float clusterResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of rays used for lights with an area. Allows for accurate soft shadowing.</para>
        /// </summary>
        public int directLightQuality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("edgeStitching is deprecated. Use stitchEdges instead.")]
        public float edgeStitching
        {
            get => 
                (!this.stitchEdges ? 0f : 1f);
            set
            {
                this.stitchEdges = !(value == 0f);
            }
        }

        /// <summary>
        /// <para>The amount of data used for realtime GI texels. Specifies how detailed view of the scene a texel has. Small values mean more averaged out lighting.</para>
        /// </summary>
        public int irradianceBudget { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of rays to cast for computing irradiance form factors.</para>
        /// </summary>
        public int irradianceQuality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If enabled, the object appears transparent during GlobalIllumination lighting calculations.</para>
        /// </summary>
        public bool isTransparent { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Maximum size of gaps that can be ignored for GI (multiplier on pixel size).</para>
        /// </summary>
        public float modellingTolerance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The texel resolution per meter used for realtime lightmaps. This value is multiplied by LightmapEditorSettings.resolution.</para>
        /// </summary>
        public float resolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether pairs of edges should be stitched together.</para>
        /// </summary>
        public bool stitchEdges { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>System tag is an integer identifier. It lets you force an object into a different Enlighten system even though all the other parameters are the same.</para>
        /// </summary>
        public int systemTag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

