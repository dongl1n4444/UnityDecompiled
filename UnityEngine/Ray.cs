namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Representation of rays.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Ray
    {
        private Vector3 m_Origin;
        private Vector3 m_Direction;
        /// <summary>
        /// <para>Creates a ray starting at origin along direction.</para>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.m_Origin = origin;
            this.m_Direction = direction.normalized;
        }

        /// <summary>
        /// <para>The origin point of the ray.</para>
        /// </summary>
        public Vector3 origin
        {
            get => 
                this.m_Origin;
            set
            {
                this.m_Origin = value;
            }
        }
        /// <summary>
        /// <para>The direction of the ray.</para>
        /// </summary>
        public Vector3 direction
        {
            get => 
                this.m_Direction;
            set
            {
                this.m_Direction = value.normalized;
            }
        }
        /// <summary>
        /// <para>Returns a point at distance units along the ray.</para>
        /// </summary>
        /// <param name="distance"></param>
        public Vector3 GetPoint(float distance) => 
            (this.m_Origin + ((Vector3) (this.m_Direction * distance)));

        /// <summary>
        /// <para>Returns a nicely formatted string for this ray.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.m_Origin, this.m_Direction };
            return UnityString.Format("Origin: {0}, Dir: {1}", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for this ray.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.m_Origin.ToString(format), this.m_Direction.ToString(format) };
            return UnityString.Format("Origin: {0}, Dir: {1}", args);
        }
    }
}

