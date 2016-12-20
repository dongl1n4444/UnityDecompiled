namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Joint that attempts to keep two Rigidbody2D objects a set distance apart by applying a force between them.</para>
    /// </summary>
    public sealed class SpringJoint2D : AnchoredJoint2D
    {
        /// <summary>
        /// <para>Should the distance be calculated automatically?</para>
        /// </summary>
        public bool autoConfigureDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The amount by which the spring force is reduced in proportion to the movement speed.</para>
        /// </summary>
        public float dampingRatio { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The distance the spring will try to keep between the two objects.</para>
        /// </summary>
        public float distance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The frequency at which the spring oscillates around the distance distance between the objects.</para>
        /// </summary>
        public float frequency { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

