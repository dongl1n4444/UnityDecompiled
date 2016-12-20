namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Timing and other statistics from the VR subsystem.</para>
    /// </summary>
    public static class VRStats
    {
        /// <summary>
        /// <para>Total GPU time utilized last frame as measured by the VR subsystem.</para>
        /// </summary>
        public static float gpuTimeLastFrame { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

