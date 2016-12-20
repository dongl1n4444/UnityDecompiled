namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Character Joints are mainly used for Ragdoll effects.</para>
    /// </summary>
    public sealed class CharacterJoint : Joint
    {
        [Obsolete("RotationDrive not in use for Unity 5 and assumed disabled.", true)]
        public JointDrive rotationDrive;
        [Obsolete("TargetAngularVelocity not in use for Unity 5 and assumed disabled.", true)]
        public Vector3 targetAngularVelocity;
        [Obsolete("TargetRotation not in use for Unity 5 and assumed disabled.", true)]
        public Quaternion targetRotation;

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_highTwistLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_lowTwistLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_swing1Limit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_swing2Limit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_swingAxis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_swingLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_twistLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_highTwistLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_lowTwistLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_swing1Limit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_swing2Limit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_swingAxis(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_swingLimitSpring(ref SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_twistLimitSpring(ref SoftJointLimitSpring value);

        /// <summary>
        /// <para>Brings violated constraints back into alignment even when the solver fails.</para>
        /// </summary>
        public bool enableProjection { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The upper limit around the primary axis of the character joint.</para>
        /// </summary>
        public SoftJointLimit highTwistLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_highTwistLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_highTwistLimit(ref value);
            }
        }

        /// <summary>
        /// <para>The lower limit around the primary axis of the character joint.</para>
        /// </summary>
        public SoftJointLimit lowTwistLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_lowTwistLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_lowTwistLimit(ref value);
            }
        }

        /// <summary>
        /// <para>Set the angular tolerance threshold (in degrees) for projection.</para>
        /// </summary>
        public float projectionAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the linear tolerance threshold for projection.</para>
        /// </summary>
        public float projectionDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The angular limit of rotation (in degrees) around the primary axis of the character joint.</para>
        /// </summary>
        public SoftJointLimit swing1Limit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_swing1Limit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_swing1Limit(ref value);
            }
        }

        /// <summary>
        /// <para>The angular limit of rotation (in degrees) around the primary axis of the character joint.</para>
        /// </summary>
        public SoftJointLimit swing2Limit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_swing2Limit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_swing2Limit(ref value);
            }
        }

        /// <summary>
        /// <para>The secondary axis around which the joint can rotate.</para>
        /// </summary>
        public Vector3 swingAxis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_swingAxis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_swingAxis(ref value);
            }
        }

        /// <summary>
        /// <para>The configuration of the spring attached to the swing limits of the joint.</para>
        /// </summary>
        public SoftJointLimitSpring swingLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_swingLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_swingLimitSpring(ref value);
            }
        }

        /// <summary>
        /// <para>The configuration of the spring attached to the twist limits of the joint.</para>
        /// </summary>
        public SoftJointLimitSpring twistLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_twistLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_twistLimitSpring(ref value);
            }
        }
    }
}

