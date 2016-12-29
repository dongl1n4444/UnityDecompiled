﻿namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>A utility class that can aid in the generation of meshes for the UI.</para>
    /// </summary>
    public class VertexHelper : IDisposable
    {
        private List<Color32> m_Colors;
        private List<int> m_Indices;
        private List<Vector3> m_Normals;
        private List<Vector3> m_Positions;
        private List<Vector4> m_Tangents;
        private List<Vector2> m_Uv0S;
        private List<Vector2> m_Uv1S;
        private static readonly Vector3 s_DefaultNormal = Vector3.back;
        private static readonly Vector4 s_DefaultTangent = new Vector4(1f, 0f, 0f, -1f);

        public VertexHelper()
        {
            this.m_Positions = ListPool<Vector3>.Get();
            this.m_Colors = ListPool<Color32>.Get();
            this.m_Uv0S = ListPool<Vector2>.Get();
            this.m_Uv1S = ListPool<Vector2>.Get();
            this.m_Normals = ListPool<Vector3>.Get();
            this.m_Tangents = ListPool<Vector4>.Get();
            this.m_Indices = ListPool<int>.Get();
        }

        public VertexHelper(Mesh m)
        {
            this.m_Positions = ListPool<Vector3>.Get();
            this.m_Colors = ListPool<Color32>.Get();
            this.m_Uv0S = ListPool<Vector2>.Get();
            this.m_Uv1S = ListPool<Vector2>.Get();
            this.m_Normals = ListPool<Vector3>.Get();
            this.m_Tangents = ListPool<Vector4>.Get();
            this.m_Indices = ListPool<int>.Get();
            this.m_Positions.AddRange(m.vertices);
            this.m_Colors.AddRange(m.colors32);
            this.m_Uv0S.AddRange(m.uv);
            this.m_Uv1S.AddRange(m.uv2);
            this.m_Normals.AddRange(m.normals);
            this.m_Tangents.AddRange(m.tangents);
            this.m_Indices.AddRange(m.GetIndices(0));
        }

        /// <summary>
        /// <para>Add a triangle to the buffer.</para>
        /// </summary>
        /// <param name="idx0">Index 0.</param>
        /// <param name="idx1">Index 1.</param>
        /// <param name="idx2">Index 2.</param>
        public void AddTriangle(int idx0, int idx1, int idx2)
        {
            this.m_Indices.Add(idx0);
            this.m_Indices.Add(idx1);
            this.m_Indices.Add(idx2);
        }

        /// <summary>
        /// <para>Add a quad to the stream.</para>
        /// </summary>
        /// <param name="verts">4 Vertices representing the quad.</param>
        public void AddUIVertexQuad(UIVertex[] verts)
        {
            int currentVertCount = this.currentVertCount;
            for (int i = 0; i < 4; i++)
            {
                this.AddVert(verts[i].position, verts[i].color, verts[i].uv0, verts[i].uv1, verts[i].normal, verts[i].tangent);
            }
            this.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            this.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }

        public void AddUIVertexStream(List<UIVertex> verts, List<int> indices)
        {
            if (verts != null)
            {
                CanvasRenderer.AddUIVertexStream(verts, this.m_Positions, this.m_Colors, this.m_Uv0S, this.m_Uv1S, this.m_Normals, this.m_Tangents);
            }
            if (indices != null)
            {
                this.m_Indices.AddRange(indices);
            }
        }

        public void AddUIVertexTriangleStream(List<UIVertex> verts)
        {
            if (verts != null)
            {
                CanvasRenderer.SplitUIVertexStreams(verts, this.m_Positions, this.m_Colors, this.m_Uv0S, this.m_Uv1S, this.m_Normals, this.m_Tangents, this.m_Indices);
            }
        }

        /// <summary>
        /// <para>Add a single vertex to the stream.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="uv0"></param>
        /// <param name="uv1"></param>
        /// <param name="normal"></param>
        /// <param name="tangent"></param>
        /// <param name="v"></param>
        public void AddVert(UIVertex v)
        {
            this.AddVert(v.position, v.color, v.uv0, v.uv1, v.normal, v.tangent);
        }

        /// <summary>
        /// <para>Add a single vertex to the stream.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="uv0"></param>
        /// <param name="uv1"></param>
        /// <param name="normal"></param>
        /// <param name="tangent"></param>
        /// <param name="v"></param>
        public void AddVert(Vector3 position, Color32 color, Vector2 uv0)
        {
            this.AddVert(position, color, uv0, Vector2.zero, s_DefaultNormal, s_DefaultTangent);
        }

        /// <summary>
        /// <para>Add a single vertex to the stream.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="uv0"></param>
        /// <param name="uv1"></param>
        /// <param name="normal"></param>
        /// <param name="tangent"></param>
        /// <param name="v"></param>
        public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1, Vector3 normal, Vector4 tangent)
        {
            this.m_Positions.Add(position);
            this.m_Colors.Add(color);
            this.m_Uv0S.Add(uv0);
            this.m_Uv1S.Add(uv1);
            this.m_Normals.Add(normal);
            this.m_Tangents.Add(tangent);
        }

        /// <summary>
        /// <para>Clear all vertices from the stream.</para>
        /// </summary>
        public void Clear()
        {
            this.m_Positions.Clear();
            this.m_Colors.Clear();
            this.m_Uv0S.Clear();
            this.m_Uv1S.Clear();
            this.m_Normals.Clear();
            this.m_Tangents.Clear();
            this.m_Indices.Clear();
        }

        /// <summary>
        /// <para>Cleanup allocated memory.</para>
        /// </summary>
        public void Dispose()
        {
            ListPool<Vector3>.Release(this.m_Positions);
            ListPool<Color32>.Release(this.m_Colors);
            ListPool<Vector2>.Release(this.m_Uv0S);
            ListPool<Vector2>.Release(this.m_Uv1S);
            ListPool<Vector3>.Release(this.m_Normals);
            ListPool<Vector4>.Release(this.m_Tangents);
            ListPool<int>.Release(this.m_Indices);
            this.m_Positions = null;
            this.m_Colors = null;
            this.m_Uv0S = null;
            this.m_Uv1S = null;
            this.m_Normals = null;
            this.m_Tangents = null;
            this.m_Indices = null;
        }

        /// <summary>
        /// <para>Fill the given mesh with the stream data.</para>
        /// </summary>
        /// <param name="mesh"></param>
        public void FillMesh(Mesh mesh)
        {
            mesh.Clear();
            if (this.m_Positions.Count >= 0xfde8)
            {
                throw new ArgumentException("Mesh can not have more than 65000 vertices");
            }
            mesh.SetVertices(this.m_Positions);
            mesh.SetColors(this.m_Colors);
            mesh.SetUVs(0, this.m_Uv0S);
            mesh.SetUVs(1, this.m_Uv1S);
            mesh.SetNormals(this.m_Normals);
            mesh.SetTangents(this.m_Tangents);
            mesh.SetTriangles(this.m_Indices, 0);
            mesh.RecalculateBounds();
        }

        public void GetUIVertexStream(List<UIVertex> stream)
        {
            if (stream != null)
            {
                CanvasRenderer.CreateUIVertexStream(stream, this.m_Positions, this.m_Colors, this.m_Uv0S, this.m_Uv1S, this.m_Normals, this.m_Tangents, this.m_Indices);
            }
        }

        public void PopulateUIVertex(ref UIVertex vertex, int i)
        {
            vertex.position = this.m_Positions[i];
            vertex.color = this.m_Colors[i];
            vertex.uv0 = this.m_Uv0S[i];
            vertex.uv1 = this.m_Uv1S[i];
            vertex.normal = this.m_Normals[i];
            vertex.tangent = this.m_Tangents[i];
        }

        /// <summary>
        /// <para>Set a UIVertex at the given index.</para>
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="i"></param>
        public void SetUIVertex(UIVertex vertex, int i)
        {
            this.m_Positions[i] = vertex.position;
            this.m_Colors[i] = vertex.color;
            this.m_Uv0S[i] = vertex.uv0;
            this.m_Uv1S[i] = vertex.uv1;
            this.m_Normals[i] = vertex.normal;
            this.m_Tangents[i] = vertex.tangent;
        }

        /// <summary>
        /// <para>Get the number of indices set on the VertexHelper.</para>
        /// </summary>
        public int currentIndexCount =>
            this.m_Indices.Count;

        /// <summary>
        /// <para>Current number of vertices in the buffer.</para>
        /// </summary>
        public int currentVertCount =>
            this.m_Positions.Count;
    }
}

