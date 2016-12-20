namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Topology of Mesh faces.</para>
    /// </summary>
    public enum MeshTopology
    {
        /// <summary>
        /// <para>Mesh is made from lines.</para>
        /// </summary>
        Lines = 3,
        /// <summary>
        /// <para>Mesh is a line strip.</para>
        /// </summary>
        LineStrip = 4,
        /// <summary>
        /// <para>Mesh is made from points.</para>
        /// </summary>
        Points = 5,
        /// <summary>
        /// <para>Mesh is made from quads.</para>
        /// </summary>
        Quads = 2,
        /// <summary>
        /// <para>Mesh is made from triangles.</para>
        /// </summary>
        Triangles = 0
    }
}

