namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The NavMesh build markup allows you to control how certain objects are treated during the NavMesh build process, specifically when collecting sources for building.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshBuildMarkup
    {
        private int m_OverrideArea;
        private int m_Area;
        private int m_IgnoreFromBuild;
        private int m_InstanceID;
        /// <summary>
        /// <para>Use this to specify whether the area type of the GameObject and its children should be overridden by the area type specified in this struct.</para>
        /// </summary>
        public bool overrideArea
        {
            get => 
                (this.m_OverrideArea != 0);
            set
            {
                this.m_OverrideArea = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>The area type to use when override area is enabled.</para>
        /// </summary>
        public int area
        {
            get => 
                this.m_Area;
            set
            {
                this.m_Area = value;
            }
        }
        /// <summary>
        /// <para>Use this to specify whether the GameObject and its children should be ignored.</para>
        /// </summary>
        public bool ignoreFromBuild
        {
            get => 
                (this.m_IgnoreFromBuild != 0);
            set
            {
                this.m_IgnoreFromBuild = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Use this to specify which GameObject (including the GameObject’s children) the markup should be applied to.</para>
        /// </summary>
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

