namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct NavMeshBuildSource
    {
        private Matrix4x4 m_Transform;
        private Vector3 m_Size;
        private NavMeshBuildSourceShape m_Shape;
        private int m_Area;
        private int m_InstanceID;
        private int m_ComponentID;
        public Matrix4x4 transform
        {
            get => 
                this.m_Transform;
            set
            {
                this.m_Transform = value;
            }
        }
        public Vector3 size
        {
            get => 
                this.m_Size;
            set
            {
                this.m_Size = value;
            }
        }
        public NavMeshBuildSourceShape shape
        {
            get => 
                this.m_Shape;
            set
            {
                this.m_Shape = value;
            }
        }
        public int area
        {
            get => 
                this.m_Area;
            set
            {
                this.m_Area = value;
            }
        }
        public UnityEngine.Object sourceObject
        {
            get => 
                this.InternalGetObject(this.m_InstanceID);
            set
            {
                this.m_InstanceID = (value == null) ? 0 : value.GetInstanceID();
            }
        }
        public Component component
        {
            get => 
                this.InternalGetComponent(this.m_ComponentID);
            set
            {
                this.m_ComponentID = (value == null) ? 0 : value.GetInstanceID();
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Component InternalGetComponent(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern UnityEngine.Object InternalGetObject(int instanceID);
    }
}

