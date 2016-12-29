namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Description of a C++ unity object in memory.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PackedNativeUnityEngineObject
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal int m_InstanceId;
        [SerializeField]
        internal int m_Size;
        [SerializeField]
        internal int m_ClassId;
        [SerializeField]
        internal HideFlags m_HideFlags;
        [SerializeField]
        internal ObjectFlags m_Flags;
        [SerializeField]
        internal long m_NativeObjectAddress;
        /// <summary>
        /// <para>Is this object persistent? (Assets are persistent, objects stored in scenes are persistent,  dynamically created objects are not)</para>
        /// </summary>
        public bool isPersistent =>
            ((this.m_Flags & ObjectFlags.IsPersistent) != ((ObjectFlags) 0));
        /// <summary>
        /// <para>Has this object has been marked as DontDestroyOnLoad?</para>
        /// </summary>
        public bool isDontDestroyOnLoad =>
            ((this.m_Flags & ObjectFlags.IsDontDestroyOnLoad) != ((ObjectFlags) 0));
        /// <summary>
        /// <para>Is this native object an internal Unity manager object?</para>
        /// </summary>
        public bool isManager =>
            ((this.m_Flags & ObjectFlags.IsManager) != ((ObjectFlags) 0));
        /// <summary>
        /// <para>Name of this object.</para>
        /// </summary>
        public string name =>
            this.m_Name;
        /// <summary>
        /// <para>InstanceId of this object.</para>
        /// </summary>
        public int instanceId =>
            this.m_InstanceId;
        /// <summary>
        /// <para>Size in bytes of this object.</para>
        /// </summary>
        public int size =>
            this.m_Size;
        /// <summary>
        /// <para>ClassId of this C++ object.  Use this classId to index into PackedMemorySnapshot.nativeTypes.</para>
        /// </summary>
        public int classId =>
            this.m_ClassId;
        /// <summary>
        /// <para>The hideFlags this native object has.</para>
        /// </summary>
        public HideFlags hideFlags =>
            this.m_HideFlags;
        /// <summary>
        /// <para>The memory address of the native C++ object. This matches the "m_CachedPtr" field of UnityEngine.Object.</para>
        /// </summary>
        public long nativeObjectAddress =>
            this.m_NativeObjectAddress;
        internal enum ObjectFlags
        {
            IsDontDestroyOnLoad = 1,
            IsManager = 4,
            IsPersistent = 2
        }
    }
}

