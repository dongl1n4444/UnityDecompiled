namespace UnityEditor.MemoryProfiler
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>PackedMemorySnapshot is a compact representation of a memory snapshot that a player has sent through the profiler connection.</para>
    /// </summary>
    [Serializable]
    public class PackedMemorySnapshot
    {
        [SerializeField]
        internal Connection[] m_Connections = null;
        [SerializeField]
        internal PackedGCHandle[] m_GcHandles = null;
        [SerializeField]
        internal MemorySection[] m_ManagedHeapSections = null;
        [SerializeField]
        internal MemorySection[] m_ManagedStacks = null;
        [SerializeField]
        internal PackedNativeUnityEngineObject[] m_NativeObjects = null;
        [SerializeField]
        internal PackedNativeType[] m_NativeTypes = null;
        [SerializeField]
        internal TypeDescription[] m_TypeDescriptions = null;
        [SerializeField]
        internal VirtualMachineInformation m_VirtualMachineInformation = new VirtualMachineInformation();

        internal PackedMemorySnapshot()
        {
        }

        /// <summary>
        /// <para>Connections is an array of from,to pairs that describe which things are keeping which other things alive.</para>
        /// </summary>
        public Connection[] connections
        {
            get
            {
                return this.m_Connections;
            }
        }

        /// <summary>
        /// <para>All GC handles in use in the memorysnapshot.</para>
        /// </summary>
        public PackedGCHandle[] gcHandles
        {
            get
            {
                return this.m_GcHandles;
            }
        }

        /// <summary>
        /// <para>Array of actual managed heap memory sections.</para>
        /// </summary>
        public MemorySection[] managedHeapSections
        {
            get
            {
                return this.m_ManagedHeapSections;
            }
        }

        /// <summary>
        /// <para>All native C++ objects that were loaded at time of the snapshot.</para>
        /// </summary>
        public PackedNativeUnityEngineObject[] nativeObjects
        {
            get
            {
                return this.m_NativeObjects;
            }
        }

        /// <summary>
        /// <para>Descriptions of all the C++ unity types the profiled player knows about.</para>
        /// </summary>
        public PackedNativeType[] nativeTypes
        {
            get
            {
                return this.m_NativeTypes;
            }
        }

        /// <summary>
        /// <para>Descriptions of all the managed types that were known to the virtual machine when the snapshot was taken.</para>
        /// </summary>
        public TypeDescription[] typeDescriptions
        {
            get
            {
                return this.m_TypeDescriptions;
            }
        }

        /// <summary>
        /// <para>Information about the virtual machine running executing the managade code inside the player.</para>
        /// </summary>
        public VirtualMachineInformation virtualMachineInformation
        {
            get
            {
                return this.m_VirtualMachineInformation;
            }
        }
    }
}

