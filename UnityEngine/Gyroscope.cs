namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Interface into the Gyroscope.</para>
    /// </summary>
    public sealed class Gyroscope
    {
        private int m_GyroIndex;

        internal Gyroscope(int index)
        {
            this.m_GyroIndex = index;
        }

        private static Quaternion attitude_Internal(int idx)
        {
            Quaternion quaternion;
            INTERNAL_CALL_attitude_Internal(idx, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool getEnabled_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float getUpdateInterval_Internal(int idx);
        private static Vector3 gravity_Internal(int idx)
        {
            Vector3 vector;
            INTERNAL_CALL_gravity_Internal(idx, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_attitude_Internal(int idx, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_gravity_Internal(int idx, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_rotationRate_Internal(int idx, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_rotationRateUnbiased_Internal(int idx, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_userAcceleration_Internal(int idx, out Vector3 value);
        private static Vector3 rotationRate_Internal(int idx)
        {
            Vector3 vector;
            INTERNAL_CALL_rotationRate_Internal(idx, out vector);
            return vector;
        }

        private static Vector3 rotationRateUnbiased_Internal(int idx)
        {
            Vector3 vector;
            INTERNAL_CALL_rotationRateUnbiased_Internal(idx, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void setEnabled_Internal(int idx, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void setUpdateInterval_Internal(int idx, float interval);
        private static Vector3 userAcceleration_Internal(int idx)
        {
            Vector3 vector;
            INTERNAL_CALL_userAcceleration_Internal(idx, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Returns the attitude (ie, orientation in space) of the device.</para>
        /// </summary>
        public Quaternion attitude
        {
            get
            {
                return attitude_Internal(this.m_GyroIndex);
            }
        }

        /// <summary>
        /// <para>Sets or retrieves the enabled status of this gyroscope.</para>
        /// </summary>
        public bool enabled
        {
            get
            {
                return getEnabled_Internal(this.m_GyroIndex);
            }
            set
            {
                setEnabled_Internal(this.m_GyroIndex, value);
            }
        }

        /// <summary>
        /// <para>Returns the gravity acceleration vector expressed in the device's reference frame.</para>
        /// </summary>
        public Vector3 gravity
        {
            get
            {
                return gravity_Internal(this.m_GyroIndex);
            }
        }

        /// <summary>
        /// <para>Returns rotation rate as measured by the device's gyroscope.</para>
        /// </summary>
        public Vector3 rotationRate
        {
            get
            {
                return rotationRate_Internal(this.m_GyroIndex);
            }
        }

        /// <summary>
        /// <para>Returns unbiased rotation rate as measured by the device's gyroscope.</para>
        /// </summary>
        public Vector3 rotationRateUnbiased
        {
            get
            {
                return rotationRateUnbiased_Internal(this.m_GyroIndex);
            }
        }

        /// <summary>
        /// <para>Sets or retrieves gyroscope interval in seconds.</para>
        /// </summary>
        public float updateInterval
        {
            get
            {
                return getUpdateInterval_Internal(this.m_GyroIndex);
            }
            set
            {
                setUpdateInterval_Internal(this.m_GyroIndex, value);
            }
        }

        /// <summary>
        /// <para>Returns the acceleration that the user is giving to the device.</para>
        /// </summary>
        public Vector3 userAcceleration
        {
            get
            {
                return userAcceleration_Internal(this.m_GyroIndex);
            }
        }
    }
}

