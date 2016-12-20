namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>A pair of from and to indices describing what thing keeps what other thing alive.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Connection
    {
        [SerializeField]
        internal int m_From;
        [SerializeField]
        internal int m_To;
        /// <summary>
        /// <para>Index into a virtual list of all GC handles, followed by all native objects.</para>
        /// </summary>
        public int from
        {
            get
            {
                return this.m_From;
            }
            set
            {
                this.m_From = value;
            }
        }
        /// <summary>
        /// <para>Index into a virtual list of all GC handles, followed by all native objects.</para>
        /// </summary>
        public int to
        {
            get
            {
                return this.m_To;
            }
            set
            {
                this.m_To = value;
            }
        }
    }
}

