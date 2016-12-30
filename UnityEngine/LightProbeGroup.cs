namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Light Probe Group.</para>
    /// </summary>
    public sealed class LightProbeGroup : Behaviour
    {
        /// <summary>
        /// <para>Editor only function to access and modify probe positions.</para>
        /// </summary>
        public Vector3[] probePositions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

