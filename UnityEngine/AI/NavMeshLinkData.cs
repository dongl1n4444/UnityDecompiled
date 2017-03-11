namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshLinkData
    {
        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;
        private float m_CostModifier;
        private int m_Bidirectional;
        private float m_Width;
        private int m_Area;
        private int m_AgentTypeID;
        public Vector3 startPosition
        {
            get => 
                this.m_StartPosition;
            set
            {
                this.m_StartPosition = value;
            }
        }
        public Vector3 endPosition
        {
            get => 
                this.m_EndPosition;
            set
            {
                this.m_EndPosition = value;
            }
        }
        public float costModifier
        {
            get => 
                this.m_CostModifier;
            set
            {
                this.m_CostModifier = value;
            }
        }
        public bool bidirectional
        {
            get => 
                (this.m_Bidirectional != 0);
            set
            {
                this.m_Bidirectional = !value ? 0 : 1;
            }
        }
        public float width
        {
            get => 
                this.m_Width;
            set
            {
                this.m_Width = value;
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
        public int agentTypeID
        {
            get => 
                this.m_AgentTypeID;
            set
            {
                this.m_AgentTypeID = value;
            }
        }
    }
}

