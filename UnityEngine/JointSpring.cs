namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>JointSpring is used add a spring force to HingeJoint and PhysicMaterial.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct JointSpring
    {
        /// <summary>
        /// <para>The spring forces used to reach the target position.</para>
        /// </summary>
        public float spring;
        /// <summary>
        /// <para>The damper force uses to dampen the spring.</para>
        /// </summary>
        public float damper;
        /// <summary>
        /// <para>The target position the joint attempts to reach.</para>
        /// </summary>
        public float targetPosition;
    }
}

