namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>An instance representing a link available for pathfinding.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshLinkInstance
    {
        private int m_Handle;
        /// <summary>
        /// <para>True if the NavMesh link is added to the navigation system - otherwise false (Read Only).</para>
        /// </summary>
        public bool valid =>
            ((this.m_Handle != 0) && NavMesh.IsValidLinkHandle(this.m_Handle));
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
        /// <para>Removes this instance from the game.</para>
        /// </summary>
        public void Remove()
        {
            NavMesh.RemoveLinkInternal(this.id);
        }

        /// <summary>
        /// <para>Get or set the owning Object.</para>
        /// </summary>
        public UnityEngine.Object owner
        {
            get => 
                NavMesh.InternalGetLinkOwner(this.id);
            set
            {
                int ownerID = (value == null) ? 0 : value.GetInstanceID();
                if (!NavMesh.InternalSetLinkOwner(this.id, ownerID))
                {
                    Debug.LogError("Cannot set 'owner' on an invalid NavMeshLinkInstance");
                }
            }
        }
    }
}

