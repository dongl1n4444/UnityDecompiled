namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Light Probe Group.</para>
    /// </summary>
    public sealed class LightProbeGroup : Behaviour
    {
        /// <summary>
        /// <para>Editor only function to access and modify probe positions.</para>
        /// </summary>
        public Vector3[] probePositions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

