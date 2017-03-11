namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>The instance is returned when adding NavMesh data.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshDataInstance
    {
        private int m_Handle;
        /// <summary>
        /// <para>True if the NavMesh data is added to the navigation system - otherwise false (Read Only).</para>
        /// </summary>
        public bool valid =>
            ((this.m_Handle != 0) && NavMesh.IsValidNavMeshDataHandle(this.m_Handle));
        internal int id
        {
            get => 
                this.m_Handle;
            set
            {
                this.m_Handle = value;
            }
        }
        /// <summary>
        /// <para>Removes this instance from the NavMesh system.</para>
        /// </summary>
        public void Remove()
        {
            NavMesh.RemoveNavMeshDataInternal(this.id);
        }

        /// <summary>
        /// <para>Get or set the owning Object.</para>
        /// </summary>
        public UnityEngine.Object owner
        {
            get => 
                NavMesh.InternalGetOwner(this.id);
            set
            {
                int ownerID = (value == null) ? 0 : value.GetInstanceID();
                if (!NavMesh.InternalSetOwner(this.id, ownerID))
                {
                    Debug.LogError("Cannot set 'owner' on an invalid NavMeshDataInstance");
                }
            }
        }
    }
}

