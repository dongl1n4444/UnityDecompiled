namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Angular limits on the rotation of a Rigidbody2D object around a HingeJoint2D.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct JointAngleLimits2D
    {
        private float m_LowerAngle;
        private float m_UpperAngle;
        /// <summary>
        /// <para>Lower angular limit of rotation.</para>
        /// </summary>
        public float min
        {
            get => 
                this.m_LowerAngle;
            set
            {
                this.m_LowerAngle = value;
            }
        }
        /// <summary>
        /// <para>Upper angular limit of rotation.</para>
        /// </summary>
        public float max
        {
            get => 
                this.m_UpperAngle;
            set
            {
                this.m_UpperAngle = value;
            }
        }
    }
}

