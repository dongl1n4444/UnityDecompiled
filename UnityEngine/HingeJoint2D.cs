namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Joint that allows a Rigidbody2D object to rotate around a point in space or a point on another object.</para>
    /// </summary>
    public sealed class HingeJoint2D : AnchoredJoint2D
    {
        /// <summary>
        /// <para>Gets the motor torque of the joint given the specified timestep.</para>
        /// </summary>
        /// <param name="timeStep">The time to calculate the motor torque for.</param>
        public float GetMotorTorque(float timeStep) => 
            INTERNAL_CALL_GetMotorTorque(this, timeStep);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_GetMotorTorque(HingeJoint2D self, float timeStep);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_limits(out JointAngleLimits2D value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_motor(out JointMotor2D value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_limits(ref JointAngleLimits2D value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_motor(ref JointMotor2D value);

        /// <summary>
        /// <para>The current joint angle (in degrees) with respect to the reference angle.</para>
        /// </summary>
        public float jointAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The current joint speed.</para>
        /// </summary>
        public float jointSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Limit of angular rotation (in degrees) on the joint.</para>
        /// </summary>
        public JointAngleLimits2D limits
        {
            get
            {
                JointAngleLimits2D limitsd;
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
        /// <para>Parameters for the motor force applied to the joint.</para>
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
        /// <para>Should limits be placed on the range of rotation?</para>
        /// </summary>
        public bool useLimits { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the joint be rotated automatically by a motor torque?</para>
        /// </summary>
        public bool useMotor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

