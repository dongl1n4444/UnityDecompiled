namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshBuildMarkup
    {
        private int m_OverrideArea;
        private int m_Area;
        private int m_IgnoreFromBuild;
        private int m_InstanceID;
        public bool overrideArea
        {
            get => 
                (this.m_OverrideArea != 0);
            set
            {
                this.m_OverrideArea = !value ? 0 : 1;
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
        public bool ignoreFromBuild
        {
            get => 
                (this.m_IgnoreFromBuild != 0);
            set
            {
                this.m_IgnoreFromBuild = !value ? 0 : 1;
            }
        }
        public Transform root
        {
            get => 
                this.InternalGetRootGO(this.m_InstanceID);
            set
            {
                this.m_InstanceID = (value == null) ? 0 : value.GetInstanceID();
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Transform InternalGetRootGO(int instanceID);
    }
}

