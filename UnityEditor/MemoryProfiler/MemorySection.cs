namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>A dump of a piece of memory from the player that's being profiled.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct MemorySection
    {
        [SerializeField]
        internal byte[] m_Bytes;
        [SerializeField]
        internal ulong m_StartAddress;
        /// <summary>
        /// <para>The actual bytes of the memory dump.</para>
        /// </summary>
        public byte[] bytes =>
            this.m_Bytes;
        /// <summary>
        /// <para>The start address of this piece of memory.</para>
        /// </summary>
        public ulong startAddress =>
            this.m_StartAddress;
    }
}

