namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Applies forces within an area.</para>
    /// </summary>
    public sealed class AreaEffector2D : Effector2D
    {
        /// <summary>
        /// <para>The angular drag to apply to rigid-bodies.</para>
        /// </summary>
        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The linear drag to apply to rigid-bodies.</para>
        /// </summary>
        public float drag { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The angle of the force to be applied.</para>
        /// </summary>
        public float forceAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("AreaEffector2D.forceDirection has been deprecated. Use AreaEffector2D.forceAngle instead (UnityUpgradable) -> forceAngle", true)]
        public float forceDirection
        {
            get => 
                this.forceAngle;
            set
            {
                this.forceAngle = value;
            }
        }

        /// <summary>
        /// <para>The magnitude of the force to be applied.</para>
        /// </summary>
        public float forceMagnitude { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The target for where the effector applies any force.</para>
        /// </summary>
        public EffectorSelection2D forceTarget { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The variation of the magnitude of the force to be applied.</para>
        /// </summary>
        public float forceVariation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the forceAngle use global space?</para>
        /// </summary>
        public bool useGlobalAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

