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
        internal int m_NativeBaseTypeArrayIndex;
        /// <summary>
        /// <para>Name of this C++ unity type.</para>
        /// </summary>
        public string name =>
            this.m_Name;
        [Obsolete("PackedNativeType.baseClassId is obsolete. Use PackedNativeType.nativeBaseTypeArrayIndex instead (UnityUpgradable) -> nativeBaseTypeArrayIndex")]
        public int baseClassId =>
            this.m_NativeBaseTypeArrayIndex;
        /// <summary>
        /// <para>The index used to obtain the native C++ base class description from the PackedMemorySnapshot.nativeTypes array.</para>
        /// </summary>
        public int nativeBaseTypeArrayIndex =>
            this.m_NativeBaseTypeArrayIndex;
    }
}

