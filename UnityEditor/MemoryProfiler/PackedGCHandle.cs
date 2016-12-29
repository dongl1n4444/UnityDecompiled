namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>A description of a GC handle used by the virtual machine.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PackedGCHandle
    {
        [SerializeField]
        internal ulong m_Target;
        /// <summary>
        /// <para>The address of the managed object that the GC handle is referencing.</para>
        /// </summary>
        public ulong target =>
            this.m_Target;
    }
}

