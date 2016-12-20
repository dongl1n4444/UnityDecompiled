namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Applies both force and torque to reduce both the linear and angular velocities to zero.</para>
    /// </summary>
    public sealed class FrictionJoint2D : AnchoredJoint2D
    {
        /// <summary>
        /// <para>The maximum force that can be generated when trying to maintain the friction joint constraint.</para>
        /// </summary>
        public float maxForce { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum torque that can be generated when trying to maintain the friction joint constraint.</para>
        /// </summary>
        public float maxTorque { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

