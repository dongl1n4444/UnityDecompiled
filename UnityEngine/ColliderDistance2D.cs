namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Represents the separation or overlap of two Collider2D.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ColliderDistance2D
    {
        private Vector2 m_PointA;
        private Vector2 m_PointB;
        private Vector2 m_Normal;
        private float m_Distance;
        private int m_IsValid;
        /// <summary>
        /// <para>A point on a Collider2D that is a specific distance away from pointB.</para>
        /// </summary>
        public Vector2 pointA
        {
            get => 
                this.m_PointA;
            set
            {
                this.m_PointA = value;
            }
        }
        /// <summary>
        /// <para>A point on a Collider2D that is a specific distance away from pointA.</para>
        /// </summary>
        public Vector2 pointB
        {
            get => 
                this.m_PointB;
            set
            {
                this.m_PointB = value;
            }
        }
        /// <summary>
        /// <para>A normalized vector that points from pointB to pointA.</para>
        /// </summary>
        public Vector2 normal =>
            this.m_Normal;
        /// <summary>
        /// <para>Gets the distance between two colliders.</para>
        /// </summary>
        public float distance
        {
            get => 
                this.m_Distance;
            set
            {
                this.m_Distance = value;
            }
        }
        /// <summary>
        /// <para>Gets whether the distance represents an overlap or not.</para>
        /// </summary>
        public bool isOverlapped =>
            (this.m_Distance < 0f);
        /// <summary>
        /// <para>Gets whether the distance is valid or not.</para>
        /// </summary>
        public bool isValid
        {
            get => 
                (this.m_IsValid != 0);
            set
            {
                this.m_IsValid = !value ? 0 : 1;
            }
        }
    }
}

