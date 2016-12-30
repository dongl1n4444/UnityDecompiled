namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NavMeshBuildDebugSettings
    {
        private int m_ShowInputGeom;
        private int m_ShowVoxels;
        private int m_ShowRegions;
        private int m_ShowRawContours;
        private int m_ShowContours;
        private int m_ShowPolyMesh;
        private int m_ShowPolyMeshDetail;
        private int m_UseFocus;
        private Vector3 m_FocusPoint;
        public bool showInputGeom
        {
            get => 
                (this.m_ShowInputGeom != 0);
            set
            {
                this.m_ShowInputGeom = !value ? 0 : 1;
            }
        }
        public bool showVoxels
        {
            get => 
                (this.m_ShowVoxels != 0);
            set
            {
                this.m_ShowVoxels = !value ? 0 : 1;
            }
        }
        public bool showRegions
        {
            get => 
                (this.m_ShowRegions != 0);
            set
            {
                this.m_ShowRegions = !value ? 0 : 1;
            }
        }
        public bool showRawContours
        {
            get => 
                (this.m_ShowRawContours != 0);
            set
            {
                this.m_ShowRawContours = !value ? 0 : 1;
            }
        }
        public bool showContours
        {
            get => 
                (this.m_ShowContours != 0);
            set
            {
                this.m_ShowContours = !value ? 0 : 1;
            }
        }
        public bool showPolyMesh
        {
            get => 
                (this.m_ShowPolyMesh != 0);
            set
            {
                this.m_ShowPolyMesh = !value ? 0 : 1;
            }
        }
        public bool showPolyMeshDetail
        {
            get => 
                (this.m_ShowPolyMeshDetail != 0);
            set
            {
                this.m_ShowPolyMeshDetail = !value ? 0 : 1;
            }
        }
        public bool useFocus
        {
            get => 
                (this.m_UseFocus != 0);
            set
            {
                this.m_UseFocus = !value ? 0 : 1;
            }
        }
        public Vector3 focusPoint
        {
            get => 
                this.m_FocusPoint;
            set
            {
                this.m_FocusPoint = value;
            }
        }
    }
}

