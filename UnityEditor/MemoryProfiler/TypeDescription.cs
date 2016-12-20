namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Description of a managed type.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct TypeDescription
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal string m_Assembly;
        [SerializeField]
        internal FieldDescription[] m_Fields;
        [SerializeField]
        internal byte[] m_StaticFieldBytes;
        [SerializeField]
        internal int m_BaseOrElementTypeIndex;
        [SerializeField]
        internal int m_Size;
        [SerializeField]
        internal ulong m_TypeInfoAddress;
        [SerializeField]
        internal int m_TypeIndex;
        [SerializeField]
        internal TypeFlags m_Flags;
        /// <summary>
        /// <para>Is this type a value type? (if it's not a value type, it's a reference type)</para>
        /// </summary>
        public bool isValueType
        {
            get
            {
                return ((this.m_Flags & TypeFlags.kValueType) != TypeFlags.kNone);
            }
        }
        /// <summary>
        /// <para>Is this type an array?</para>
        /// </summary>
        public bool isArray
        {
            get
            {
                return ((this.m_Flags & TypeFlags.kArray) != TypeFlags.kNone);
            }
        }
        /// <summary>
        /// <para>If this is an arrayType, this will return the rank of the array. (1 for a 1-dimensional array, 2 for a 2-dimensional array, etc)</para>
        /// </summary>
        public int arrayRank
        {
            get
            {
                return (((int) (this.m_Flags & -65536)) >> 0x10);
            }
        }
        /// <summary>
        /// <para>Name of this type.</para>
        /// </summary>
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        /// <summary>
        /// <para>Name of the assembly this type was loaded from.</para>
        /// </summary>
        public string assembly
        {
            get
            {
                return this.m_Assembly;
            }
        }
        /// <summary>
        /// <para>An array containing descriptions of all fields of this type.</para>
        /// </summary>
        public FieldDescription[] fields
        {
            get
            {
                return this.m_Fields;
            }
        }
        /// <summary>
        /// <para>The actual contents of the bytes that store this types static fields, at the point of time when the snapshot was taken.</para>
        /// </summary>
        public byte[] staticFieldBytes
        {
            get
            {
                return this.m_StaticFieldBytes;
            }
        }
        /// <summary>
        /// <para>The base type for this type, pointed to by an index into PackedMemorySnapshot.typeDescriptions.</para>
        /// </summary>
        public int baseOrElementTypeIndex
        {
            get
            {
                return this.m_BaseOrElementTypeIndex;
            }
        }
        /// <summary>
        /// <para>Size in bytes of an instance of this type. If this type is an arraytype, this describes the amount of bytes a single element in the array will take up.</para>
        /// </summary>
        public int size
        {
            get
            {
                return this.m_Size;
            }
        }
        /// <summary>
        /// <para>The address in memory that contains the description of this type inside the virtual machine. This can be used to match managed objects in the heap to their corresponding TypeDescription,  as the first pointer of a managed object points to its type description.</para>
        /// </summary>
        public ulong typeInfoAddress
        {
            get
            {
                return this.m_TypeInfoAddress;
            }
        }
        /// <summary>
        /// <para>The typeIndex of this type.  This index is an index into the PackedMemorySnapshot.typeDescriptions array.</para>
        /// </summary>
        public int typeIndex
        {
            get
            {
                return this.m_TypeIndex;
            }
        }
        internal enum TypeFlags
        {
            kArray = 2,
            kArrayRankMask = -65536,
            kNone = 0,
            kValueType = 1
        }
    }
}

