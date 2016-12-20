namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Applies tangent forces along the surfaces of colliders.</para>
    /// </summary>
    public sealed class SurfaceEffector2D : Effector2D
    {
        /// <summary>
        /// <para>The scale of the impulse force applied while attempting to reach the surface speed.</para>
        /// </summary>
        public float forceScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The speed to be maintained along the surface.</para>
        /// </summary>
        public float speed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The speed variation (from zero to the variation) added to base speed to be applied.</para>
        /// </summary>
        public float speedVariation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should bounce be used for any contact with the surface?</para>
        /// </summary>
        public bool useBounce { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the impulse force but applied to the contact point?</para>
        /// </summary>
        public bool useContactForce { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should friction be used for any contact with the surface?</para>
        /// </summary>
        public bool useFriction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

