namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The HingeJoint groups together 2 rigid bodies, constraining them to move like connected by a hinge.</para>
    /// </summary>
    public sealed class HingeJoint : Joint
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_limits(out JointLimits value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_motor(out JointMotor value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_spring(out JointSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_limits(ref JointLimits value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_motor(ref JointMotor value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_spring(ref JointSpring value);

        /// <summary>
        /// <para>The current angle in degrees of the joint relative to its rest position. (Read Only)</para>
        /// </summary>
        public float angle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Limit of angular rotation (in degrees) on the hinge joint.</para>
        /// </summary>
        public JointLimits limits
        {
            get
            {
                JointLimits limits;
                this.INTERNAL_get_limits(out limits);
                return limits;
            }
            set
            {
                this.INTERNAL_set_limits(ref value);
            }
        }

        /// <summary>
        /// <para>The motor will apply a force up to a maximum force to achieve the target velocity in degrees per second.</para>
        /// </summary>
        public JointMotor motor
        {
            get
            {
                JointMotor motor;
                this.INTERNAL_get_motor(out motor);
                return motor;
            }
            set
            {
                this.INTERNAL_set_motor(ref value);
            }
        }

        /// <summary>
        /// <para>The spring attempts to reach a target angle by adding spring and damping forces.</para>
        /// </summary>
        public JointSpring spring
        {
            get
            {
                JointSpring spring;
                this.INTERNAL_get_spring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_spring(ref value);
            }
        }

        /// <summary>
        /// <para>Enables the joint's limits. Disabled by default.</para>
        /// </summary>
        public bool useLimits { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables the joint's motor. Disabled by default.</para>
        /// </summary>
        public bool useMotor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables the joint's spring. Disabled by default.</para>
        /// </summary>
        public bool useSpring { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The angular velocity of the joint in degrees per second.</para>
        /// </summary>
        public float velocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

