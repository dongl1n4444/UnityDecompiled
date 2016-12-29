namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Structure describing acceleration status of the device.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AccelerationEvent
    {
        private float x;
        private float y;
        private float z;
        private float m_TimeDelta;
        /// <summary>
        /// <para>Value of acceleration.</para>
        /// </summary>
        public Vector3 acceleration =>
            new Vector3(this.x, this.y, this.z);
        /// <summary>
        /// <para>Amount of time passed since last accelerometer measurement.</para>
        /// </summary>
        public float deltaTime =>
            this.m_TimeDelta;
    }
}

