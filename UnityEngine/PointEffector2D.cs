namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Applies forces to attract/repulse against a point.</para>
    /// </summary>
    public sealed class PointEffector2D : Effector2D
    {
        /// <summary>
        /// <para>The angular drag to apply to rigid-bodies.</para>
        /// </summary>
        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The scale applied to the calculated distance between source and target.</para>
        /// </summary>
        public float distanceScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The linear drag to apply to rigid-bodies.</para>
        /// </summary>
        public float drag { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The magnitude of the force to be applied.</para>
        /// </summary>
        public float forceMagnitude { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The mode used to apply the effector force.</para>
        /// </summary>
        public EffectorForceMode2D forceMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The source which is used to calculate the centroid point of the effector.  The distance from the target is defined from this point.</para>
        /// </summary>
        public EffectorSelection2D forceSource { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The target for where the effector applies any force.</para>
        /// </summary>
        public EffectorSelection2D forceTarget { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The variation of the magnitude of the force to be applied.</para>
        /// </summary>
        public float forceVariation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

