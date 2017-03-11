namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshLinkInstance
    {
        private int m_Handle;
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
        public void Remove()
        {
            NavMesh.RemoveLinkInternal(this.id);
        }

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

