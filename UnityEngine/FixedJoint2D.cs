namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Connects two Rigidbody2D together at their anchor points using a configurable spring.</para>
    /// </summary>
    public sealed class FixedJoint2D : AnchoredJoint2D
    {
        /// <summary>
        /// <para>The amount by which the spring force is reduced in proportion to the movement speed.</para>
        /// </summary>
        public float dampingRatio { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The frequency at which the spring oscillates around the distance between the objects.</para>
        /// </summary>
        public float frequency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The angle referenced between the two bodies used as the constraint for the joint.</para>
        /// </summary>
        public float referenceAngle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

