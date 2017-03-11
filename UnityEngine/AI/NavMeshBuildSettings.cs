namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The NavMeshBuildSettings struct allows you to specify a collection of settings which describe the dimensions and limitations of a particular agent type.</para>
    /// </summary>
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
        /// <summary>
        /// <para>The agent type ID the NavMesh will be baked for.</para>
        /// </summary>
        public int agentTypeID
        {
            get => 
                this.m_AgentTypeID;
            set
            {
                this.m_AgentTypeID = value;
            }
        }
        /// <summary>
        /// <para>The radius of the agent for baking in world units.</para>
        /// </summary>
        public float agentRadius
        {
            get => 
                this.m_AgentRadius;
            set
            {
                this.m_AgentRadius = value;
            }
        }
        /// <summary>
        /// <para>The height of the agent for baking in world units.</para>
        /// </summary>
        public float agentHeight
        {
            get => 
                this.m_AgentHeight;
            set
            {
                this.m_AgentHeight = value;
            }
        }
        /// <summary>
        /// <para>The maximum slope angle which is walkable (angle in degrees).</para>
        /// </summary>
        public float agentSlope
        {
            get => 
                this.m_AgentSlope;
            set
            {
                this.m_AgentSlope = value;
            }
        }
        /// <summary>
        /// <para>The maximum vertical step size an agent can take.</para>
        /// </summary>
        public float agentClimb
        {
            get => 
                this.m_AgentClimb;
            set
            {
                this.m_AgentClimb = value;
            }
        }
        /// <summary>
        /// <para>Enables overriding the default voxel size. See Also: voxelSize.</para>
        /// </summary>
        public bool overrideVoxelSize
        {
            get => 
                (this.m_OverrideVoxelSize != 0);
            set
            {
                this.m_OverrideVoxelSize = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Sets the voxel size in world length units.</para>
        /// </summary>
        public float voxelSize
        {
            get => 
                this.m_VoxelSize;
            set
            {
                this.m_VoxelSize = value;
            }
        }
        /// <summary>
        /// <para>Enables overriding the default tile size. See Also: tileSize.</para>
        /// </summary>
        public bool overrideTileSize
        {
            get => 
                (this.m_OverrideTileSize != 0);
            set
            {
                this.m_OverrideTileSize = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Sets the tile size in voxel units.</para>
        /// </summary>
        public int tileSize
        {
            get => 
                this.m_TileSize;
            set
            {
                this.m_TileSize = value;
            }
        }
        /// <summary>
        /// <para>Validates the properties of NavMeshBuildSettings.</para>
        /// </summary>
        /// <param name="buildBounds">Describes the volume to build NavMesh for.</param>
        /// <returns>
        /// <para>The list of violated constraints.</para>
        /// </returns>
        public string[] ValidationReport(Bounds buildBounds) => 
            InternalValidationReport(this, buildBounds);

        private static string[] InternalValidationReport(NavMeshBuildSettings buildSettings, Bounds buildBounds) => 
            INTERNAL_CALL_InternalValidationReport(ref buildSettings, ref buildBounds);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string[] INTERNAL_CALL_InternalValidationReport(ref NavMeshBuildSettings buildSettings, ref Bounds buildBounds);
    }
}

