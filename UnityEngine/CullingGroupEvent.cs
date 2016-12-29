namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Provides information about the current and previous states of one sphere in a CullingGroup.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CullingGroupEvent
    {
        private int m_Index;
        private byte m_PrevState;
        private byte m_ThisState;
        private const byte kIsVisibleMask = 0x80;
        private const byte kDistanceMask = 0x7f;
        /// <summary>
        /// <para>The index of the sphere that has changed.</para>
        /// </summary>
        public int index =>
            this.m_Index;
        /// <summary>
        /// <para>Was the sphere considered visible by the most recent culling pass?</para>
        /// </summary>
        public bool isVisible =>
            ((this.m_ThisState & 0x80) != 0);
        /// <summary>
        /// <para>Was the sphere visible before the most recent culling pass?</para>
        /// </summary>
        public bool wasVisible =>
            ((this.m_PrevState & 0x80) != 0);
        /// <summary>
        /// <para>Did this sphere change from being invisible to being visible in the most recent culling pass?</para>
        /// </summary>
        public bool hasBecomeVisible =>
            (this.isVisible && !this.wasVisible);
        /// <summary>
        /// <para>Did this sphere change from being visible to being invisible in the most recent culling pass?</para>
        /// </summary>
        public bool hasBecomeInvisible =>
            (!this.isVisible && this.wasVisible);
        /// <summary>
        /// <para>The current distance band index of the sphere, after the most recent culling pass.</para>
        /// </summary>
        public int currentDistance =>
            (this.m_ThisState & 0x7f);
        /// <summary>
        /// <para>The distance band index of the sphere before the most recent culling pass.</para>
        /// </summary>
        public int previousDistance =>
            (this.m_PrevState & 0x7f);
    }
}

