﻿namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>How the joint's movement will behave along its local X axis.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct JointDrive
    {
        private float m_PositionSpring;
        private float m_PositionDamper;
        private float m_MaximumForce;
        /// <summary>
        /// <para>Whether the drive should attempt to reach position, velocity, both or nothing.</para>
        /// </summary>
        [Obsolete("JointDriveMode is obsolete")]
        public JointDriveMode mode
        {
            get => 
                JointDriveMode.None;
            set
            {
            }
        }
        /// <summary>
        /// <para>Strength of a rubber-band pull toward the defined direction. Only used if mode includes Position.</para>
        /// </summary>
        public float positionSpring
        {
            get => 
                this.m_PositionSpring;
            set
            {
                this.m_PositionSpring = value;
            }
        }
        /// <summary>
        /// <para>Resistance strength against the Position Spring. Only used if mode includes Position.</para>
        /// </summary>
        public float positionDamper
        {
            get => 
                this.m_PositionDamper;
            set
            {
                this.m_PositionDamper = value;
            }
        }
        /// <summary>
        /// <para>Amount of force applied to push the object toward the defined direction.</para>
        /// </summary>
        public float maximumForce
        {
            get => 
                this.m_MaximumForce;
            set
            {
                this.m_MaximumForce = value;
            }
        }
    }
}

