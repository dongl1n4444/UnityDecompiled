namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Various utilities for mesh manipulation.</para>
    /// </summary>
    public sealed class MeshUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Vector2[] ComputeTextureBoundingHull(Texture texture, int vertexCount);
        /// <summary>
        /// <para>Returns the mesh compression setting for a Mesh.</para>
        /// </summary>
        /// <param name="mesh">The mesh to get information on.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern ModelImporterMeshCompression GetMeshCompression(Mesh mesh);
        /// <summary>
        /// <para>Optimizes the mesh for GPU access.</para>
        /// </summary>
        /// <param name="mesh"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Optimize(Mesh mesh);
        /// <summary>
        /// <para>Change the mesh compression setting for a mesh.</para>
        /// </summary>
        /// <param name="mesh">The mesh to set the compression mode for.</param>
        /// <param name="compression">The compression mode to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetMeshCompression(Mesh mesh, ModelImporterMeshCompression compression);
        /// <summary>
        /// <para>Will insert per-triangle uv2 in mesh and handle vertex splitting etc.</para>
        /// </summary>
        /// <param name="src"></param>
        /// <param name="triUV"></param>
        public static void SetPerTriangleUV2(Mesh src, Vector2[] triUV)
        {
            int num = InternalMeshUtil.CalcTriangleCount(src);
            int length = triUV.Length;
            if (length != (3 * num))
            {
                Debug.LogError(string.Concat(new object[] { "mesh contains ", num, " triangles but ", length, " uvs are provided" }));
            }
            else
            {
                SetPerTriangleUV2NoCheck(src, triUV);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetPerTriangleUV2NoCheck(Mesh src, Vector2[] triUV);
    }
}

