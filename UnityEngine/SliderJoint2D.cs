namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Joint that restricts the motion of a Rigidbody2D object to a single line.</para>
    /// </summary>
    public sealed class SliderJoint2D : AnchoredJoint2D
    {
        /// <summary>
        /// <para>Gets the motor force of the joint given the specified timestep.</para>
        /// </summary>
        /// <param name="timeStep">The time to calculate the motor force for.</param>
        public float GetMotorForce(float timeStep) => 
            INTERNAL_CALL_GetMotorForce(this, timeStep);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_GetMotorForce(SliderJoint2D self, float timeStep);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_limits(out JointTranslationLimits2D value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_motor(out JointMotor2D value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_limits(ref JointTranslationLimits2D value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_motor(ref JointMotor2D value);

        /// <summary>
        /// <para>The angle of the line in space (in degrees).</para>
        /// </summary>
        public float angle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the angle be calculated automatically?</para>
        /// </summary>
        public bool autoConfigureAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The current joint speed.</para>
        /// </summary>
        public float jointSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The current joint translation.</para>
        /// </summary>
        public float jointTranslation { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Restrictions on how far the joint can slide in each direction along the line.</para>
        /// </summary>
        public JointTranslationLimits2D limits
        {
            get
            {
                JointTranslationLimits2D limitsd;
                this.INTERNAL_get_limits(out limitsd);
                return limitsd;
            }
            set
            {
                this.INTERNAL_set_limits(ref value);
            }
        }

        /// <summary>
        /// <para>Gets the state of the joint limit.</para>
        /// </summary>
        public JointLimitState2D limitState { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Parameters for a motor force that is applied automatically to the Rigibody2D along the line.</para>
        /// </summary>
        public JointMotor2D motor
        {
            get
            {
                JointMotor2D motord;
                this.INTERNAL_get_motor(out motord);
                return motord;
            }
            set
            {
                this.INTERNAL_set_motor(ref value);
            }
        }

        /// <summary>
        /// <para>The angle (in degrees) referenced between the two bodies used as the constraint for the joint.</para>
        /// </summary>
        public float referenceAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Should motion limits be used?</para>
        /// </summary>
        public bool useLimits { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should a motor force be applied automatically to the Rigidbody2D?</para>
        /// </summary>
        public bool useMotor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

