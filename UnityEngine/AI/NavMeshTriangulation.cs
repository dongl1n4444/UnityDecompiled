namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>Contains data describing a triangulation of a navmesh.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode, MovedFrom("UnityEngine")]
    public struct NavMeshTriangulation
    {
        /// <summary>
        /// <para>Vertices for the navmesh triangulation.</para>
        /// </summary>
        public Vector3[] vertices;
        /// <summary>
        /// <para>Triangle indices for the navmesh triangulation.</para>
        /// </summary>
        public int[] indices;
        /// <summary>
        /// <para>NavMesh area indices for the navmesh triangulation.</para>
        /// </summary>
        public int[] areas;
        /// <summary>
        /// <para>NavMeshLayer values for the navmesh triangulation.</para>
        /// </summary>
        [Obsolete("Use areas instead.")]
        public int[] layers
        {
            get
            {
                return this.areas;
            }
        }
    }
}

