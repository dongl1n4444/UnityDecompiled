namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshDataInstance
    {
        private int m_Handle;
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
        public void Remove()
        {
            NavMesh.RemoveNavMeshDataInternal(this.id);
        }

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

