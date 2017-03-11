namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Joint that keeps two Rigidbody2D objects a fixed distance apart.</para>
    /// </summary>
    public sealed class DistanceJoint2D : AnchoredJoint2D
    {
        /// <summary>
        /// <para>Should the distance be calculated automatically?</para>
        /// </summary>
        public bool autoConfigureDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The distance separating the two ends of the joint.</para>
        /// </summary>
        public float distance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether to maintain a maximum distance only or not.  If not then the absolute distance will be maintained instead.</para>
        /// </summary>
        public bool maxDistanceOnly { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

