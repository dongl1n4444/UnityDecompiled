namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class InternalMeshUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int CalcTriangleCount(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GetCachedMeshSurfaceArea(MeshRenderer meshRenderer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GetCachedSkinnedMeshSurfaceArea(SkinnedMeshRenderer skinnedMeshRenderer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetPrimitiveCount(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetVertexFormat(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasNormals(Mesh mesh);
    }
}

