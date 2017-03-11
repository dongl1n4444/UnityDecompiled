namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Timing and other statistics from the VR subsystem.</para>
    /// </summary>
    public static class VRStats
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool TryGetDroppedFrameCount(out int droppedFrameCount);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool TryGetFramePresentCount(out int framePresentCount);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool TryGetGPUTimeLastFrame(out float gpuTimeLastFrame);

        /// <summary>
        /// <para>Total GPU time utilized last frame as measured by the VR subsystem.</para>
        /// </summary>
        [Obsolete("gpuTimeLastFrame is deprecated. Use VRStats.TryGetGPUTimeLastFrame instead.")]
        public static float gpuTimeLastFrame
        {
            get
            {
                float num;
                if (TryGetGPUTimeLastFrame(out num))
                {
                    return num;
                }
                return 0f;
            }
        }
    }
}

