using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>A class that allows creating or modifying meshes from scripts.</para>
	/// </summary>
	public sealed class Mesh : Object
	{
		/// <summary>
		///   <para>Returns state of the Read/Write Enabled checkbox when model was imported.</para>
		/// </summary>
		public extern bool isReadable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool canAccess
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns a copy of the vertex positions or assigns a new vertex positions array.</para>
		/// </summary>
		public extern Vector3[] vertices
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The normals of the mesh.</para>
		/// </summary>
		public extern Vector3[] normals
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The tangents of the mesh.</para>
		/// </summary>
		public extern Vector4[] tangents
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The base texture coordinates of the mesh.</para>
		/// </summary>
		public extern Vector2[] uv
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The second texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public extern Vector2[] uv2
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The third texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public extern Vector2[] uv3
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The fourth texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public extern Vector2[] uv4
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bounding volume of the mesh.</para>
		/// </summary>
		public Bounds bounds
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_bounds(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_bounds(ref value);
			}
		}

		/// <summary>
		///   <para>Vertex colors of the mesh.</para>
		/// </summary>
		public extern Color[] colors
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Vertex colors of the mesh.</para>
		/// </summary>
		public extern Color32[] colors32
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>An array containing all triangles in the mesh.</para>
		/// </summary>
		public extern int[] triangles
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the number of vertices in the mesh (Read Only).</para>
		/// </summary>
		public extern int vertexCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number of submeshes. Every material has a separate triangle list.</para>
		/// </summary>
		public extern int subMeshCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bone weights of each vertex.</para>
		/// </summary>
		public extern BoneWeight[] boneWeights
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bind poses. The bind pose at each index refers to the bone with the same index.</para>
		/// </summary>
		public extern Matrix4x4[] bindposes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns BlendShape count on this mesh.</para>
		/// </summary>
		public extern int blendShapeCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property Mesh.uv1 has been deprecated. Use Mesh.uv2 instead (UnityUpgradable) -> uv2", true)]
		public Vector2[] uv1
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>Creates an empty mesh.</para>
		/// </summary>
		public Mesh()
		{
			Mesh.Internal_Create(this);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Mesh mono);

		/// <summary>
		///   <para>Clears all vertex data and all triangle indices.</para>
		/// </summary>
		/// <param name="keepVertexLayout"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear([UnityEngine.Internal.DefaultValue("true")] bool keepVertexLayout);

		[ExcludeFromDocs]
		public void Clear()
		{
			bool keepVertexLayout = true;
			this.Clear(keepVertexLayout);
		}

		public void SetVertices(List<Vector3> inVertices)
		{
			this.SetVerticesInternal(inVertices);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVerticesInternal(object vertices);

		public void SetNormals(List<Vector3> inNormals)
		{
			this.SetNormalsInternal(inNormals);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetNormalsInternal(object normals);

		public void SetTangents(List<Vector4> inTangents)
		{
			this.SetTangentsInternal(inTangents);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTangentsInternal(object tangents);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Array ExtractListData(object list);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetUVsInternal(Array uvs, int channel, int dim, int arraySize);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetUVsInternal(Array uvs, int channel, int dim);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool CheckCanAccessUVChannel(int channel);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResizeList(object list, int size);

		private void GetUVsImpl<T>(int channel, List<T> uvs, int dim)
		{
			if (uvs == null)
			{
				throw new ArgumentException("The result uvs list cannot be null");
			}
			if (this.CheckCanAccessUVChannel(channel))
			{
				int vertexCount = this.vertexCount;
				if (vertexCount > uvs.Capacity)
				{
					uvs.Capacity = vertexCount;
				}
				Mesh.ResizeList(uvs, vertexCount);
				this.GetUVsInternal(Mesh.ExtractListData(uvs), channel, dim);
			}
			else
			{
				uvs.Clear();
			}
		}

		public void SetUVs(int channel, List<Vector2> uvs)
		{
			this.SetUVsInternal(Mesh.ExtractListData(uvs), channel, 2, uvs.Count);
		}

		public void SetUVs(int channel, List<Vector3> uvs)
		{
			this.SetUVsInternal(Mesh.ExtractListData(uvs), channel, 3, uvs.Count);
		}

		public void SetUVs(int channel, List<Vector4> uvs)
		{
			this.SetUVsInternal(Mesh.ExtractListData(uvs), channel, 4, uvs.Count);
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_bounds(ref Bounds value);

		public void SetColors(List<Color> inColors)
		{
			this.SetColorsInternal(inColors);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColorsInternal(object colors);

		public void SetColors(List<Color32> inColors)
		{
			this.SetColors32Internal(inColors);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColors32Internal(object colors);

		/// <summary>
		///   <para>Recalculate the bounding volume of the mesh from the vertices.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateBounds();

		/// <summary>
		///   <para>Recalculates the normals of the mesh from the triangles and vertices.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateNormals();

		/// <summary>
		///   <para>Optimizes the mesh for display.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Optimize();

		/// <summary>
		///   <para>Returns the triangle list for the submesh.</para>
		/// </summary>
		/// <param name="submesh"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[] GetTriangles(int submesh);

		/// <summary>
		///   <para>Sets the triangle list for the submesh.</para>
		/// </summary>
		/// <param name="inTriangles"></param>
		/// <param name="submesh"></param>
		/// <param name="triangles"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTriangles(int[] triangles, int submesh);

		public void SetTriangles(List<int> inTriangles, int submesh)
		{
			this.SetTrianglesInternal(inTriangles, submesh);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTrianglesInternal(object triangles, int submesh);

		/// <summary>
		///   <para>Returns the index buffer for the submesh.</para>
		/// </summary>
		/// <param name="submesh"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[] GetIndices(int submesh);

		/// <summary>
		///   <para>Sets the index buffer for the submesh.</para>
		/// </summary>
		/// <param name="indices"></param>
		/// <param name="topology"></param>
		/// <param name="submesh"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetIndices(int[] indices, MeshTopology topology, int submesh);

		/// <summary>
		///   <para>Gets the topology of a submesh.</para>
		/// </summary>
		/// <param name="submesh"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MeshTopology GetTopology(int submesh);

		/// <summary>
		///   <para>Combines several meshes into this mesh.</para>
		/// </summary>
		/// <param name="combine">Descriptions of the meshes to combine.</param>
		/// <param name="mergeSubMeshes">Should all meshes be combined into a single submesh?</param>
		/// <param name="useMatrices">Should the transforms supplied in the CombineInstance array be used or ignored?</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CombineMeshes(CombineInstance[] combine, [UnityEngine.Internal.DefaultValue("true")] bool mergeSubMeshes, [UnityEngine.Internal.DefaultValue("true")] bool useMatrices);

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
		{
			bool useMatrices = true;
			this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
		}

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine)
		{
			bool useMatrices = true;
			bool mergeSubMeshes = true;
			this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
		}

		/// <summary>
		///   <para>Optimize mesh for frequent updates.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MarkDynamic();

		/// <summary>
		///   <para>Upload previously done mesh modifications to the graphics API.</para>
		/// </summary>
		/// <param name="markNoLogerReadable">Frees up system memory copy of mesh data when set to true.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UploadMeshData(bool markNoLogerReadable);

		/// <summary>
		///   <para>Returns index of BlendShape by given name.</para>
		/// </summary>
		/// <param name="blendShapeName"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetBlendShapeIndex(string blendShapeName);

		/// <summary>
		///   <para>Returns name of BlendShape by given index.</para>
		/// </summary>
		/// <param name="shapeIndex"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetBlendShapeName(int shapeIndex);

		/// <summary>
		///   <para>Returns the frame count for a blend shape.</para>
		/// </summary>
		/// <param name="shapeIndex">The shape index to get frame count from.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetBlendShapeFrameCount(int shapeIndex);

		/// <summary>
		///   <para>Returns the weight of a blend shape frame.</para>
		/// </summary>
		/// <param name="shapeIndex">The shape index of the frame.</param>
		/// <param name="frameIndex">The frame index to get the weight from.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);

		/// <summary>
		///   <para>Retreives deltaVertices, deltaNormals and deltaTangents of a blend shape frame.</para>
		/// </summary>
		/// <param name="shapeIndex">The shape index of the frame.</param>
		/// <param name="frameIndex">The frame index to get the weight from.</param>
		/// <param name="deltaVertices">Delta vertices output array for the frame being retreived.</param>
		/// <param name="deltaNormals">Delta normals output array for the frame being retreived.</param>
		/// <param name="deltaTangents">Delta tangents output array for the frame being retreived.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

		/// <summary>
		///   <para>Clears all blend shapes from Mesh.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearBlendShapes();

		/// <summary>
		///   <para>Adds a new blend shape frame.</para>
		/// </summary>
		/// <param name="shapeName">Name of the blend shape to add a frame to.</param>
		/// <param name="frameWeight">Weight for the frame being added.</param>
		/// <param name="deltaVertices">Delta vertices for the frame being added.</param>
		/// <param name="deltaNormals">Delta normals for the frame being added.</param>
		/// <param name="deltaTangents">Delta tangents for the frame being added.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
	}
}
