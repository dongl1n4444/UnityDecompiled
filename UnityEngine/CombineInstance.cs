﻿namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Struct used to describe meshes to be combined using Mesh.CombineMeshes.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CombineInstance
    {
        private int m_MeshInstanceID;
        private int m_SubMeshIndex;
        private Matrix4x4 m_Transform;
        /// <summary>
        /// <para>Mesh to combine.</para>
        /// </summary>
        public Mesh mesh
        {
            get => 
                this.InternalGetMesh(this.m_MeshInstanceID);
            set
            {
                this.m_MeshInstanceID = (value == null) ? 0 : value.GetInstanceID();
            }
        }
        /// <summary>
        /// <para>Submesh index of the mesh.</para>
        /// </summary>
        public int subMeshIndex
        {
            get => 
                this.m_SubMeshIndex;
            set
            {
                this.m_SubMeshIndex = value;
            }
        }
        /// <summary>
        /// <para>Matrix to transform the mesh with before combining.</para>
        /// </summary>
        public Matrix4x4 transform
        {
            get => 
                this.m_Transform;
            set
            {
                this.m_Transform = value;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Mesh InternalGetMesh(int instanceID);
    }
}

