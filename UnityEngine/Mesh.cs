namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>A class that allows creating or modifying meshes from scripts.</para>
    /// </summary>
    public sealed class Mesh : UnityEngine.Object
    {
        /// <summary>
        /// <para>Creates an empty Mesh.</para>
        /// </summary>
        public Mesh()
        {
            Internal_Create(this);
        }

        /// <summary>
        /// <para>Adds a new blend shape frame.</para>
        /// </summary>
        /// <param name="shapeName">Name of the blend shape to add a frame to.</param>
        /// <param name="frameWeight">Weight for the frame being added.</param>
        /// <param name="deltaVertices">Delta vertices for the frame being added.</param>
        /// <param name="deltaNormals">Delta normals for the frame being added.</param>
        /// <param name="deltaTangents">Delta tangents for the frame being added.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
        private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
        {
            if (!this.canAccess)
            {
                this.PrintErrorCantAccessMeshForIndices();
                return false;
            }
            if ((submesh < 0) || (submesh >= this.subMeshCount))
            {
                if (errorAboutTriangles)
                {
                    this.PrintErrorBadSubmeshIndexTriangles();
                }
                else
                {
                    this.PrintErrorBadSubmeshIndexIndices();
                }
                return false;
            }
            return true;
        }

        private bool CheckCanAccessSubmeshIndices(int submesh) => 
            this.CheckCanAccessSubmesh(submesh, false);

        private bool CheckCanAccessSubmeshTriangles(int submesh) => 
            this.CheckCanAccessSubmesh(submesh, true);

        [ExcludeFromDocs]
        public void Clear()
        {
            bool keepVertexLayout = true;
            this.Clear(keepVertexLayout);
        }

        /// <summary>
        /// <para>Clears all vertex data and all triangle indices.</para>
        /// </summary>
        /// <param name="keepVertexLayout"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Clear([UnityEngine.Internal.DefaultValue("true")] bool keepVertexLayout);
        /// <summary>
        /// <para>Clears all blend shapes from Mesh.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ClearBlendShapes();
        [ExcludeFromDocs]
        public void CombineMeshes(CombineInstance[] combine)
        {
            bool useMatrices = true;
            bool mergeSubMeshes = true;
            this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
        }

        [ExcludeFromDocs]
        public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
        {
            bool useMatrices = true;
            this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
        }

        /// <summary>
        /// <para>Combines several Meshes into this Mesh.</para>
        /// </summary>
        /// <param name="combine">Descriptions of the Meshes to combine.</param>
        /// <param name="mergeSubMeshes">Defines whether Meshes should be combined into a single sub-Mesh.</param>
        /// <param name="useMatrices">Defines whether the transforms supplied in the CombineInstance array should be used or ignored.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CombineMeshes(CombineInstance[] combine, [UnityEngine.Internal.DefaultValue("true")] bool mergeSubMeshes, [UnityEngine.Internal.DefaultValue("true")] bool useMatrices);
        internal static int DefaultDimensionForChannel(InternalShaderChannel channel)
        {
            if ((channel == InternalShaderChannel.Vertex) || (channel == InternalShaderChannel.Normal))
            {
                return 3;
            }
            if ((channel >= InternalShaderChannel.TexCoord0) && (channel <= InternalShaderChannel.TexCoord3))
            {
                return 2;
            }
            if ((channel != InternalShaderChannel.Tangent) && (channel != InternalShaderChannel.Color))
            {
                throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
            }
            return 4;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Array ExtractArrayFromList(object list);
        private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel) => 
            this.GetAllocArrayFromChannel<T>(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel));

        private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim)
        {
            if (this.canAccess)
            {
                if (this.HasChannel(channel))
                {
                    return (T[]) this.GetAllocArrayFromChannelImpl(channel, format, dim);
                }
            }
            else
            {
                this.PrintErrorCantAccessMesh(channel);
            }
            return new T[0];
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Array GetAllocArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values);
        /// <summary>
        /// <para>Returns the frame count for a blend shape.</para>
        /// </summary>
        /// <param name="shapeIndex">The shape index to get frame count from.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetBlendShapeFrameCount(int shapeIndex);
        /// <summary>
        /// <para>Retreives deltaVertices, deltaNormals and deltaTangents of a blend shape frame.</para>
        /// </summary>
        /// <param name="shapeIndex">The shape index of the frame.</param>
        /// <param name="frameIndex">The frame index to get the weight from.</param>
        /// <param name="deltaVertices">Delta vertices output array for the frame being retreived.</param>
        /// <param name="deltaNormals">Delta normals output array for the frame being retreived.</param>
        /// <param name="deltaTangents">Delta tangents output array for the frame being retreived.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
        /// <summary>
        /// <para>Returns the weight of a blend shape frame.</para>
        /// </summary>
        /// <param name="shapeIndex">The shape index of the frame.</param>
        /// <param name="frameIndex">The frame index to get the weight from.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);
        /// <summary>
        /// <para>Returns index of BlendShape by given name.</para>
        /// </summary>
        /// <param name="blendShapeName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetBlendShapeIndex(string blendShapeName);
        /// <summary>
        /// <para>Returns name of BlendShape by given index.</para>
        /// </summary>
        /// <param name="shapeIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetBlendShapeName(int shapeIndex);
        /// <summary>
        /// <para>Returns the index buffer for the sub-Mesh.</para>
        /// </summary>
        /// <param name="submesh"></param>
        public int[] GetIndices(int submesh) => 
            (!this.CheckCanAccessSubmeshIndices(submesh) ? new int[0] : this.GetIndicesImpl(submesh));

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int[] GetIndicesImpl(int submesh);
        /// <summary>
        /// <para>Retrieves a native (underlying graphics API) pointer to the index buffer.</para>
        /// </summary>
        /// <returns>
        /// <para>Pointer to the underlying graphics API index buffer.</para>
        /// </returns>
        public IntPtr GetNativeIndexBufferPtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeIndexBufferPtr(this, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Retrieves a native (underlying graphics API) pointer to the vertex buffer.</para>
        /// </summary>
        /// <param name="bufferIndex">Which vertex buffer to get (some Meshes might have more than one). See vertexBufferCount.</param>
        /// <returns>
        /// <para>Pointer to the underlying graphics API vertex buffer.</para>
        /// </returns>
        public IntPtr GetNativeVertexBufferPtr(int bufferIndex)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeVertexBufferPtr(this, bufferIndex, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Gets the topology of a sub-Mesh.</para>
        /// </summary>
        /// <param name="submesh"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MeshTopology GetTopology(int submesh);
        /// <summary>
        /// <para>Returns the triangle list for the sub-Mesh.</para>
        /// </summary>
        /// <param name="submesh"></param>
        public int[] GetTriangles(int submesh) => 
            (!this.CheckCanAccessSubmeshTriangles(submesh) ? new int[0] : this.GetTrianglesImpl(submesh));

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int[] GetTrianglesImpl(int submesh);
        internal InternalShaderChannel GetUVChannel(int uvIndex)
        {
            if ((uvIndex < 0) || (uvIndex > 3))
            {
                throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
            }
            return (InternalShaderChannel) (3 + uvIndex);
        }

        public void GetUVs(int channel, List<Vector2> uvs)
        {
            this.GetUVsImpl<Vector2>(channel, uvs, 2);
        }

        public void GetUVs(int channel, List<Vector3> uvs)
        {
            this.GetUVsImpl<Vector3>(channel, uvs, 3);
        }

        public void GetUVs(int channel, List<Vector4> uvs)
        {
            this.GetUVsImpl<Vector4>(channel, uvs, 4);
        }

        private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
        {
            if (uvs == null)
            {
                throw new ArgumentException("The result uvs list cannot be null", "uvs");
            }
            if ((uvIndex < 0) || (uvIndex > 3))
            {
                throw new ArgumentException("The uv index is invalid (must be in [0..3]", "uvIndex");
            }
            uvs.Clear();
            InternalShaderChannel uVChannel = this.GetUVChannel(uvIndex);
            if (!this.canAccess)
            {
                this.PrintErrorCantAccessMesh(uVChannel);
            }
            else if (this.HasChannel(uVChannel))
            {
                if (this.vertexCount > uvs.Capacity)
                {
                    uvs.Capacity = this.vertexCount;
                }
                this.GetUVsInternal<T>(uvs, uvIndex, dim);
            }
        }

        private void GetUVsInternal<T>(List<T> uvs, int uvIndex, int dim)
        {
            InternalShaderChannel uVChannel = this.GetUVChannel(uvIndex);
            ResizeList(uvs, this.vertexCount);
            this.GetArrayFromChannelImpl(uVChannel, InternalVertexChannelType.Float, dim, ExtractArrayFromList(uvs));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool HasChannel(InternalShaderChannel channel);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetNativeIndexBufferPtr(Mesh self, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetNativeVertexBufferPtr(Mesh self, int bufferIndex, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Create([Writable] Mesh mono);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_bounds(ref Bounds value);
        /// <summary>
        /// <para>Optimize mesh for frequent updates.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void MarkDynamic();
        /// <summary>
        /// <para>Optimizes the Mesh for display.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("This method is no longer supported (UnityUpgradable)", true), EditorBrowsable(EditorBrowsableState.Never)]
        public extern void Optimize();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void PrintErrorBadSubmeshIndexIndices();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void PrintErrorBadSubmeshIndexTriangles();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void PrintErrorCantAccessMesh(InternalShaderChannel channel);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void PrintErrorCantAccessMeshForIndices();
        /// <summary>
        /// <para>Recalculate the bounding volume of the Mesh from the vertices.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RecalculateBounds();
        /// <summary>
        /// <para>Recalculates the normals of the Mesh from the triangles and vertices.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RecalculateNormals();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ResizeList(object list, int size);
        private int SafeLength(Array values) => 
            ((values == null) ? 0 : values.Length);

        private int SafeLength<T>(List<T> values) => 
            ((values == null) ? 0 : values.Count);

        private void SetArrayForChannel<T>(InternalShaderChannel channel, T[] values)
        {
            this.SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), values, this.SafeLength(values));
        }

        private void SetArrayForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, T[] values)
        {
            this.SetSizedArrayForChannel(channel, format, dim, values, this.SafeLength(values));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetArrayForChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int arraySize);
        public void SetColors(List<Color> inColors)
        {
            this.SetListForChannel<Color>(InternalShaderChannel.Color, inColors);
        }

        public void SetColors(List<Color32> inColors)
        {
            this.SetListForChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, inColors);
        }

        /// <summary>
        /// <para>Sets the index buffer for the sub-Mesh.</para>
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="topology"></param>
        /// <param name="submesh"></param>
        /// <param name="calculateBounds"></param>
        [ExcludeFromDocs]
        public void SetIndices(int[] indices, MeshTopology topology, int submesh)
        {
            bool calculateBounds = true;
            this.SetIndices(indices, topology, submesh, calculateBounds);
        }

        public void SetIndices(int[] indices, MeshTopology topology, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
        {
            if (this.CheckCanAccessSubmeshIndices(submesh))
            {
                this.SetIndicesImpl(submesh, topology, indices, this.SafeLength(indices), calculateBounds);
            }
        }

        [ExcludeFromDocs]
        private void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize)
        {
            bool calculateBounds = true;
            this.SetIndicesImpl(submesh, topology, indices, arraySize, calculateBounds);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds);
        private void SetListForChannel<T>(InternalShaderChannel channel, List<T> values)
        {
            this.SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), ExtractArrayFromList(values), this.SafeLength<T>(values));
        }

        private void SetListForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, List<T> values)
        {
            this.SetSizedArrayForChannel(channel, format, dim, ExtractArrayFromList(values), this.SafeLength<T>(values));
        }

        public void SetNormals(List<Vector3> inNormals)
        {
            this.SetListForChannel<Vector3>(InternalShaderChannel.Normal, inNormals);
        }

        private void SetSizedArrayForChannel(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int valuesCount)
        {
            if (this.canAccess)
            {
                this.SetArrayForChannelImpl(channel, format, dim, values, valuesCount);
            }
            else
            {
                this.PrintErrorCantAccessMesh(channel);
            }
        }

        public void SetTangents(List<Vector4> inTangents)
        {
            this.SetListForChannel<Vector4>(InternalShaderChannel.Tangent, inTangents);
        }

        [ExcludeFromDocs]
        public void SetTriangles(List<int> triangles, int submesh)
        {
            bool calculateBounds = true;
            this.SetTriangles(triangles, submesh, calculateBounds);
        }

        /// <summary>
        /// <para>Sets the triangle list for the sub-Mesh.</para>
        /// </summary>
        /// <param name="triangles"></param>
        /// <param name="submesh"></param>
        /// <param name="calculateBounds"></param>
        [ExcludeFromDocs]
        public void SetTriangles(int[] triangles, int submesh)
        {
            bool calculateBounds = true;
            this.SetTriangles(triangles, submesh, calculateBounds);
        }

        public void SetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
        {
            if (this.CheckCanAccessSubmeshTriangles(submesh))
            {
                this.SetTrianglesImpl(submesh, ExtractArrayFromList(triangles), this.SafeLength<int>(triangles), calculateBounds);
            }
        }

        public void SetTriangles(int[] triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
        {
            if (this.CheckCanAccessSubmeshTriangles(submesh))
            {
                this.SetTrianglesImpl(submesh, triangles, this.SafeLength(triangles), calculateBounds);
            }
        }

        [ExcludeFromDocs]
        private void SetTrianglesImpl(int submesh, Array triangles, int arraySize)
        {
            bool calculateBounds = true;
            this.SetTrianglesImpl(submesh, triangles, arraySize, calculateBounds);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetTrianglesImpl(int submesh, Array triangles, int arraySize, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds);
        public void SetUVs(int channel, List<Vector2> uvs)
        {
            this.SetUvsImpl<Vector2>(channel, 2, uvs);
        }

        public void SetUVs(int channel, List<Vector3> uvs)
        {
            this.SetUvsImpl<Vector3>(channel, 3, uvs);
        }

        public void SetUVs(int channel, List<Vector4> uvs)
        {
            this.SetUvsImpl<Vector4>(channel, 4, uvs);
        }

        private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs)
        {
            if ((uvIndex < 0) || (uvIndex > 3))
            {
                Debug.LogError("The uv index is invalid (must be in [0..3]");
            }
            else
            {
                this.SetListForChannel<T>(this.GetUVChannel(uvIndex), InternalVertexChannelType.Float, dim, uvs);
            }
        }

        public void SetVertices(List<Vector3> inVertices)
        {
            this.SetListForChannel<Vector3>(InternalShaderChannel.Vertex, inVertices);
        }

        /// <summary>
        /// <para>Upload previously done Mesh modifications to the graphics API.</para>
        /// </summary>
        /// <param name="markNoLongerReadable">Frees up system memory copy of mesh data when set to true.</param>
        /// <param name="markNoLogerReadable"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void UploadMeshData(bool markNoLogerReadable);

        /// <summary>
        /// <para>The bind poses. The bind pose at each index refers to the bone with the same index.</para>
        /// </summary>
        public Matrix4x4[] bindposes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns BlendShape count on this mesh.</para>
        /// </summary>
        public int blendShapeCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The bone weights of each vertex.</para>
        /// </summary>
        public BoneWeight[] boneWeights { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The bounding volume of the mesh.</para>
        /// </summary>
        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_bounds(ref value);
            }
        }

        internal bool canAccess { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Vertex colors of the Mesh.</para>
        /// </summary>
        public Color[] colors
        {
            get => 
                this.GetAllocArrayFromChannel<Color>(InternalShaderChannel.Color);
            set
            {
                this.SetArrayForChannel<Color>(InternalShaderChannel.Color, value);
            }
        }

        /// <summary>
        /// <para>Vertex colors of the Mesh.</para>
        /// </summary>
        public Color32[] colors32
        {
            get => 
                this.GetAllocArrayFromChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1);
            set
            {
                this.SetArrayForChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, value);
            }
        }

        /// <summary>
        /// <para>Returns state of the Read/Write Enabled checkbox when model was imported.</para>
        /// </summary>
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The normals of the Mesh.</para>
        /// </summary>
        public Vector3[] normals
        {
            get => 
                this.GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Normal);
            set
            {
                this.SetArrayForChannel<Vector3>(InternalShaderChannel.Normal, value);
            }
        }

        /// <summary>
        /// <para>The number of sub-Meshes. Every Material has a separate triangle list.</para>
        /// </summary>
        public int subMeshCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The tangents of the Mesh.</para>
        /// </summary>
        public Vector4[] tangents
        {
            get => 
                this.GetAllocArrayFromChannel<Vector4>(InternalShaderChannel.Tangent);
            set
            {
                this.SetArrayForChannel<Vector4>(InternalShaderChannel.Tangent, value);
            }
        }

        /// <summary>
        /// <para>An array containing all triangles in the Mesh.</para>
        /// </summary>
        public int[] triangles
        {
            get
            {
                if (this.canAccess)
                {
                    return this.GetTrianglesImpl(-1);
                }
                this.PrintErrorCantAccessMeshForIndices();
                return new int[0];
            }
            set
            {
                if (this.canAccess)
                {
                    this.SetTrianglesImpl(-1, value, this.SafeLength(value));
                }
                else
                {
                    this.PrintErrorCantAccessMeshForIndices();
                }
            }
        }

        /// <summary>
        /// <para>The base texture coordinates of the Mesh.</para>
        /// </summary>
        public Vector2[] uv
        {
            get => 
                this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord0);
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord0, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property Mesh.uv1 has been deprecated. Use Mesh.uv2 instead (UnityUpgradable) -> uv2", true)]
        public Vector2[] uv1
        {
            get => 
                null;
            set
            {
            }
        }

        /// <summary>
        /// <para>The second texture coordinate set of the mesh, if present.</para>
        /// </summary>
        public Vector2[] uv2
        {
            get => 
                this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord1);
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord1, value);
            }
        }

        /// <summary>
        /// <para>The third texture coordinate set of the mesh, if present.</para>
        /// </summary>
        public Vector2[] uv3
        {
            get => 
                this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord2);
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord2, value);
            }
        }

        /// <summary>
        /// <para>The fourth texture coordinate set of the mesh, if present.</para>
        /// </summary>
        public Vector2[] uv4
        {
            get => 
                this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord3);
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord3, value);
            }
        }

        /// <summary>
        /// <para>Get the number of vertex buffers present in the Mesh. (Read Only)</para>
        /// </summary>
        public int vertexBufferCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the number of vertices in the Mesh (Read Only).</para>
        /// </summary>
        public int vertexCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns a copy of the vertex positions or assigns a new vertex positions array.</para>
        /// </summary>
        public Vector3[] vertices
        {
            get => 
                this.GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Vertex);
            set
            {
                this.SetArrayForChannel<Vector3>(InternalShaderChannel.Vertex, value);
            }
        }

        internal enum InternalShaderChannel
        {
            Vertex,
            Normal,
            Color,
            TexCoord0,
            TexCoord1,
            TexCoord2,
            TexCoord3,
            Tangent
        }

        internal enum InternalVertexChannelType
        {
            Color = 2,
            Float = 0
        }
    }
}

