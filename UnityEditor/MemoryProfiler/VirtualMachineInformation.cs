namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Information about a virtual machine that provided a memory snapshot.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct VirtualMachineInformation
    {
        [SerializeField]
        internal int m_PointerSize;
        [SerializeField]
        internal int m_ObjectHeaderSize;
        [SerializeField]
        internal int m_ArrayHeaderSize;
        [SerializeField]
        internal int m_ArrayBoundsOffsetInHeader;
        [SerializeField]
        internal int m_ArraySizeOffsetInHeader;
        [SerializeField]
        internal int m_AllocationGranularity;
        /// <summary>
        /// <para>Size in bytes of a pointer.</para>
        /// </summary>
        public int pointerSize
        {
            get
            {
                return this.m_PointerSize;
            }
        }
        /// <summary>
        /// <para>Size in bytes of the header of each managed object.</para>
        /// </summary>
        public int objectHeaderSize
        {
            get
            {
                return this.m_ObjectHeaderSize;
            }
        }
        /// <summary>
        /// <para>Size in bytes of the header of an array object.</para>
        /// </summary>
        public int arrayHeaderSize
        {
            get
            {
                return this.m_ArrayHeaderSize;
            }
        }
        /// <summary>
        /// <para>Offset in bytes inside the object header of an array object where the bounds of the array is stored.</para>
        /// </summary>
        public int arrayBoundsOffsetInHeader
        {
            get
            {
                return this.m_ArrayBoundsOffsetInHeader;
            }
        }
        /// <summary>
        /// <para>Offset in bytes inside the object header of an array object where the size of the array is stored.</para>
        /// </summary>
        public int arraySizeOffsetInHeader
        {
            get
            {
                return this.m_ArraySizeOffsetInHeader;
            }
        }
        /// <summary>
        /// <para>Allocation granularity in bytes used by the virtual machine allocator.</para>
        /// </summary>
        public int allocationGranularity
        {
            get
            {
                return this.m_AllocationGranularity;
            }
        }
        /// <summary>
        /// <para>A version number that will change when the object layout inside the managed heap will change.</para>
        /// </summary>
        public int heapFormatVersion
        {
            get
            {
                return 0;
            }
        }
    }
}

