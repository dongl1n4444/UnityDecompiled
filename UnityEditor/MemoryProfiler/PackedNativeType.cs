namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>A description of a C++ unity type.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PackedNativeType
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal int m_BaseClassId;
        /// <summary>
        /// <para>Name of this C++ unity type.</para>
        /// </summary>
        public string name =>
            this.m_Name;
        /// <summary>
        /// <para>ClassId of the base class of this C++ class.</para>
        /// </summary>
        public int baseClassId =>
            this.m_BaseClassId;
    }
}

