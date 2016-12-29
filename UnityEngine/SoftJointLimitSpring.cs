﻿namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>The configuration of the spring attached to the joint's limits: linear and angular. Used by CharacterJoint and ConfigurableJoint.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SoftJointLimitSpring
    {
        private float m_Spring;
        private float m_Damper;
        /// <summary>
        /// <para>The stiffness of the spring limit. When stiffness is zero the limit is hard, otherwise soft.</para>
        /// </summary>
        public float spring
        {
            get => 
                this.m_Spring;
            set
            {
                this.m_Spring = value;
            }
        }
        /// <summary>
        /// <para>The damping of the spring limit. In effect when the stiffness of the sprint limit is not zero.</para>
        /// </summary>
        public float damper
        {
            get => 
                this.m_Damper;
            set
            {
                this.m_Damper = value;
            }
        }
    }
}

