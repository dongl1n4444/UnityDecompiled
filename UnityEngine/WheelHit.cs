namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Contact information for the wheel, reported by WheelCollider.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WheelHit
    {
        private Vector3 m_Point;
        private Vector3 m_Normal;
        private Vector3 m_ForwardDir;
        private Vector3 m_SidewaysDir;
        private float m_Force;
        private float m_ForwardSlip;
        private float m_SidewaysSlip;
        private Collider m_Collider;
        /// <summary>
        /// <para>The other Collider the wheel is hitting.</para>
        /// </summary>
        public Collider collider
        {
            get => 
                this.m_Collider;
            set
            {
                this.m_Collider = value;
            }
        }
        /// <summary>
        /// <para>The point of contact between the wheel and the ground.</para>
        /// </summary>
        public Vector3 point
        {
            get => 
                this.m_Point;
            set
            {
                this.m_Point = value;
            }
        }
        /// <summary>
        /// <para>The normal at the point of contact.</para>
        /// </summary>
        public Vector3 normal
        {
            get => 
                this.m_Normal;
            set
            {
                this.m_Normal = value;
            }
        }
        /// <summary>
        /// <para>The direction the wheel is pointing in.</para>
        /// </summary>
        public Vector3 forwardDir
        {
            get => 
                this.m_ForwardDir;
            set
            {
                this.m_ForwardDir = value;
            }
        }
        /// <summary>
        /// <para>The sideways direction of the wheel.</para>
        /// </summary>
        public Vector3 sidewaysDir
        {
            get => 
                this.m_SidewaysDir;
            set
            {
                this.m_SidewaysDir = value;
            }
        }
        /// <summary>
        /// <para>The magnitude of the force being applied for the contact.</para>
        /// </summary>
        public float force
        {
            get => 
                this.m_Force;
            set
            {
                this.m_Force = value;
            }
        }
        /// <summary>
        /// <para>Tire slip in the rolling direction. Acceleration slip is negative, braking slip is positive.</para>
        /// </summary>
        public float forwardSlip
        {
            get => 
                this.m_ForwardSlip;
            set
            {
                this.m_Force = this.m_ForwardSlip;
            }
        }
        /// <summary>
        /// <para>Tire slip in the sideways direction.</para>
        /// </summary>
        public float sidewaysSlip
        {
            get => 
                this.m_SidewaysSlip;
            set
            {
                this.m_SidewaysSlip = value;
            }
        }
    }
}

