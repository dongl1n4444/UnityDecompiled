namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Structure describing device location.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LocationInfo
    {
        private double m_Timestamp;
        private float m_Latitude;
        private float m_Longitude;
        private float m_Altitude;
        private float m_HorizontalAccuracy;
        private float m_VerticalAccuracy;
        /// <summary>
        /// <para>Geographical device location latitude.</para>
        /// </summary>
        public float latitude =>
            this.m_Latitude;
        /// <summary>
        /// <para>Geographical device location latitude.</para>
        /// </summary>
        public float longitude =>
            this.m_Longitude;
        /// <summary>
        /// <para>Geographical device location altitude.</para>
        /// </summary>
        public float altitude =>
            this.m_Altitude;
        /// <summary>
        /// <para>Horizontal accuracy of the location.</para>
        /// </summary>
        public float horizontalAccuracy =>
            this.m_HorizontalAccuracy;
        /// <summary>
        /// <para>Vertical accuracy of the location.</para>
        /// </summary>
        public float verticalAccuracy =>
            this.m_VerticalAccuracy;
        /// <summary>
        /// <para>Timestamp (in seconds since 1970) when location was last time updated.</para>
        /// </summary>
        public double timestamp =>
            this.m_Timestamp;
    }
}

