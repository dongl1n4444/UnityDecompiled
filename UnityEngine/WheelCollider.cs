namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A special collider for vehicle wheels.</para>
    /// </summary>
    public sealed class WheelCollider : Collider
    {
        /// <summary>
        /// <para>Configure vehicle sub-stepping parameters.</para>
        /// </summary>
        /// <param name="speedThreshold">The speed threshold of the sub-stepping algorithm.</param>
        /// <param name="stepsBelowThreshold">Amount of simulation sub-steps when vehicle's speed is below speedThreshold.</param>
        /// <param name="stepsAboveThreshold">Amount of simulation sub-steps when vehicle's speed is above speedThreshold.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ConfigureVehicleSubsteps(float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool GetGroundHit(out WheelHit hit);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void GetWorldPose(out Vector3 pos, out Quaternion quat);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_forwardFriction(out WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_sidewaysFriction(out WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_suspensionSpring(out JointSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_center(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_forwardFriction(ref WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_sidewaysFriction(ref WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_suspensionSpring(ref JointSpring value);

        /// <summary>
        /// <para>Brake torque expressed in Newton metres.</para>
        /// </summary>
        public float brakeTorque { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The center of the wheel, measured in the object's local space.</para>
        /// </summary>
        public Vector3 center
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_center(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_center(ref value);
            }
        }

        /// <summary>
        /// <para>Application point of the suspension and tire forces measured from the base of the resting wheel.</para>
        /// </summary>
        public float forceAppPointDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Properties of tire friction in the direction the wheel is pointing in.</para>
        /// </summary>
        public WheelFrictionCurve forwardFriction
        {
            get
            {
                WheelFrictionCurve curve;
                this.INTERNAL_get_forwardFriction(out curve);
                return curve;
            }
            set
            {
                this.INTERNAL_set_forwardFriction(ref value);
            }
        }

        /// <summary>
        /// <para>Indicates whether the wheel currently collides with something (Read Only).</para>
        /// </summary>
        public bool isGrounded { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The mass of the wheel, expressed in kilograms. Must be larger than zero. Typical values would be in range (20,80).</para>
        /// </summary>
        public float mass { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Motor torque on the wheel axle expressed in Newton metres. Positive or negative depending on direction.</para>
        /// </summary>
        public float motorTorque { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The radius of the wheel, measured in local space.</para>
        /// </summary>
        public float radius { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Current wheel axle rotation speed, in rotations per minute (Read Only).</para>
        /// </summary>
        public float rpm { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Properties of tire friction in the sideways direction.</para>
        /// </summary>
        public WheelFrictionCurve sidewaysFriction
        {
            get
            {
                WheelFrictionCurve curve;
                this.INTERNAL_get_sidewaysFriction(out curve);
                return curve;
            }
            set
            {
                this.INTERNAL_set_sidewaysFriction(ref value);
            }
        }

        /// <summary>
        /// <para>The mass supported by this WheelCollider.</para>
        /// </summary>
        public float sprungMass { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Steering angle in degrees, always around the local y-axis.</para>
        /// </summary>
        public float steerAngle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Maximum extension distance of wheel suspension, measured in local space.</para>
        /// </summary>
        public float suspensionDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The parameters of wheel's suspension. The suspension attempts to reach a target position by applying a linear force and a damping force.</para>
        /// </summary>
        public JointSpring suspensionSpring
        {
            get
            {
                JointSpring spring;
                this.INTERNAL_get_suspensionSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_suspensionSpring(ref value);
            }
        }

        /// <summary>
        /// <para>The damping rate of the wheel. Must be larger than zero.</para>
        /// </summary>
        public float wheelDampingRate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

