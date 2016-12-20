namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class MeshSubsetCombineUtility
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MeshContainer
        {
            public GameObject gameObject;
            public MeshSubsetCombineUtility.MeshInstance instance;
            public List<MeshSubsetCombineUtility.SubMeshInstance> subMeshInstances;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MeshInstance
        {
            public int meshInstanceID;
            public int rendererInstanceID;
            public int additionalVertexStreamsMeshInstanceID;
            public Matrix4x4 transform;
            public Vector4 lightmapScaleOffset;
            public Vector4 realtimeLightmapScaleOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SubMeshInstance
        {
            public int meshInstanceID;
            public int vertexOffset;
            public int gameObjectInstanceID;
            public int subMeshIndex;
            public Matrix4x4 transform;
        }
    }
}

