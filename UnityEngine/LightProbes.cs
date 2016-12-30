namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Stores light probes for the scene.</para>
    /// </summary>
    public sealed class LightProbes : UnityEngine.Object
    {
        [Obsolete("Use GetInterpolatedProbe instead.", true)]
        public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
        {
        }

        public static void GetInterpolatedProbe(Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe)
        {
            INTERNAL_CALL_GetInterpolatedProbe(ref position, renderer, out probe);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetInterpolatedProbe(ref Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe);

        /// <summary>
        /// <para>Coefficients of baked light probes.</para>
        /// </summary>
        public SphericalHarmonicsL2[] bakedProbes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of cells space is divided into (Read Only).</para>
        /// </summary>
        public int cellCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("Use bakedProbes instead.", true)]
        public float[] coefficients
        {
            get => 
                new float[0];
            set
            {
            }
        }

        /// <summary>
        /// <para>The number of light probes (Read Only).</para>
        /// </summary>
        public int count { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Positions of the baked light probes (Read Only).</para>
        /// </summary>
        public Vector3[] positions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

