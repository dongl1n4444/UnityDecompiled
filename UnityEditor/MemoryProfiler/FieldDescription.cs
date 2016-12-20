namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Description of a field of a managed type.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct FieldDescription
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal int m_Offset;
        [SerializeField]
        internal int m_TypeIndex;
        [SerializeField]
        internal bool m_IsStatic;
        /// <summary>
        /// <para>Name of this field.</para>
        /// </summary>
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        /// <summary>
        /// <para>Offset of this field.</para>
        /// </summary>
        public int offset
        {
            get
            {
                return this.m_Offset;
            }
        }
        /// <summary>
        /// <para>The typeindex into PackedMemorySnapshot.typeDescriptions of the type this field belongs to.</para>
        /// </summary>
        public int typeIndex
        {
            get
            {
                return this.m_TypeIndex;
            }
        }
        /// <summary>
        /// <para>Is this field static?</para>
        /// </summary>
        public bool isStatic
        {
            get
            {
                return this.m_IsStatic;
            }
        }
    }
}

