namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshBuildSettings
    {
        private int m_AgentTypeID;
        private float m_AgentRadius;
        private float m_AgentHeight;
        private float m_AgentSlope;
        private float m_AgentClimb;
        private float m_LedgeDropHeight;
        private float m_MaxJumpAcrossDistance;
        private float m_MinRegionArea;
        private int m_OverrideVoxelSize;
        private float m_VoxelSize;
        private int m_OverrideTileSize;
        private int m_TileSize;
        private int m_AccuratePlacement;
        public int agentTypeID
        {
            get => 
                this.m_AgentTypeID;
            set
            {
                this.m_AgentTypeID = value;
            }
        }
        public float agentRadius
        {
            get => 
                this.m_AgentRadius;
            set
            {
                this.m_AgentRadius = value;
            }
        }
        public float agentHeight
        {
            get => 
                this.m_AgentHeight;
            set
            {
                this.m_AgentHeight = value;
            }
        }
        public float agentSlope
        {
            get => 
                this.m_AgentSlope;
            set
            {
                this.m_AgentSlope = value;
            }
        }
        public float agentClimb
        {
            get => 
                this.m_AgentClimb;
            set
            {
                this.m_AgentClimb = value;
            }
        }
        public bool overrideVoxelSize
        {
            get => 
                (this.m_OverrideVoxelSize != 0);
            set
            {
                this.m_OverrideVoxelSize = !value ? 0 : 1;
            }
        }
        public float voxelSize
        {
            get => 
                this.m_VoxelSize;
            set
            {
                this.m_VoxelSize = value;
            }
        }
        public bool overrideTileSize
        {
            get => 
                (this.m_OverrideTileSize != 0);
            set
            {
                this.m_OverrideTileSize = !value ? 0 : 1;
            }
        }
        public int tileSize
        {
            get => 
                this.m_TileSize;
            set
            {
                this.m_TileSize = value;
            }
        }
        public string[] ValidationReport(Bounds buildBounds) => 
            InternalValidationReport(this, buildBounds);

        private static string[] InternalValidationReport(NavMeshBuildSettings buildSettings, Bounds buildBounds) => 
            INTERNAL_CALL_InternalValidationReport(ref buildSettings, ref buildBounds);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string[] INTERNAL_CALL_InternalValidationReport(ref NavMeshBuildSettings buildSettings, ref Bounds buildBounds);
    }
}

