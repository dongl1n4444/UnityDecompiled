namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A structure describing the webcam device.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct WebCamDevice
    {
        internal string m_Name;
        internal int m_Flags;
        /// <summary>
        /// <para>A human-readable name of the device. Varies across different systems.</para>
        /// </summary>
        public string name =>
            this.m_Name;
        /// <summary>
        /// <para>True if camera faces the same direction a screen does, false otherwise.</para>
        /// </summary>
        public bool isFrontFacing =>
            ((this.m_Flags & 1) == 1);
    }
}

