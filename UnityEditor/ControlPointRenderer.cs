namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class ControlPointRenderer
    {
        private const string kControlPointRendererMeshName = "ControlPointRendererMesh";
        private const int kMaxVertices = 0xfde8;
        private Texture2D m_Icon;
        private List<RenderChunk> m_RenderChunks = new List<RenderChunk>();
        private static Material s_Material;

        public ControlPointRenderer(Texture2D icon)
        {
            this.m_Icon = icon;
        }

        public void AddPoint(Rect rect, Color color)
        {
            RenderChunk renderChunk = this.GetRenderChunk();
            int count = renderChunk.vertices.Count;
            renderChunk.vertices.Add(new Vector3(rect.xMin, rect.yMin, 0f));
            renderChunk.vertices.Add(new Vector3(rect.xMax, rect.yMin, 0f));
            renderChunk.vertices.Add(new Vector3(rect.xMax, rect.yMax, 0f));
            renderChunk.vertices.Add(new Vector3(rect.xMin, rect.yMax, 0f));
            renderChunk.colors.Add(color);
            renderChunk.colors.Add(color);
            renderChunk.colors.Add(color);
            renderChunk.colors.Add(color);
            renderChunk.uvs.Add(new Vector2(0f, 0f));
            renderChunk.uvs.Add(new Vector2(1f, 0f));
            renderChunk.uvs.Add(new Vector2(1f, 1f));
            renderChunk.uvs.Add(new Vector2(0f, 1f));
            renderChunk.indices.Add(count);
            renderChunk.indices.Add(count + 1);
            renderChunk.indices.Add(count + 2);
            renderChunk.indices.Add(count);
            renderChunk.indices.Add(count + 2);
            renderChunk.indices.Add(count + 3);
            renderChunk.isDirty = true;
        }

        public void Clear()
        {
            for (int i = 0; i < this.m_RenderChunks.Count; i++)
            {
                RenderChunk chunk = this.m_RenderChunks[i];
                chunk.mesh.Clear();
                chunk.vertices.Clear();
                chunk.colors.Clear();
                chunk.uvs.Clear();
                chunk.indices.Clear();
                chunk.isDirty = true;
            }
        }

        private RenderChunk CreateRenderChunk()
        {
            RenderChunk item = new RenderChunk {
                mesh = new Mesh()
            };
            item.mesh.name = "ControlPointRendererMesh";
            item.mesh.hideFlags |= HideFlags.DontSave;
            item.vertices = new List<Vector3>();
            item.colors = new List<Color32>();
            item.uvs = new List<Vector2>();
            item.indices = new List<int>();
            this.m_RenderChunks.Add(item);
            return item;
        }

        public void FlushCache()
        {
            for (int i = 0; i < this.m_RenderChunks.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(this.m_RenderChunks[i].mesh);
            }
            this.m_RenderChunks.Clear();
        }

        private RenderChunk GetRenderChunk()
        {
            if (this.m_RenderChunks.Count > 0)
            {
                RenderChunk chunk = this.m_RenderChunks.Last<RenderChunk>();
                if ((chunk.vertices.Count + 4) > 0xfde8)
                {
                    chunk = this.CreateRenderChunk();
                }
                return chunk;
            }
            return this.CreateRenderChunk();
        }

        public void Render()
        {
            Material material = ControlPointRenderer.material;
            material.SetTexture("_MainTex", this.m_Icon);
            material.SetPass(0);
            for (int i = 0; i < this.m_RenderChunks.Count; i++)
            {
                RenderChunk chunk = this.m_RenderChunks[i];
                if (chunk.isDirty)
                {
                    chunk.mesh.vertices = chunk.vertices.ToArray();
                    chunk.mesh.colors32 = chunk.colors.ToArray();
                    chunk.mesh.uv = chunk.uvs.ToArray();
                    chunk.mesh.SetIndices(chunk.indices.ToArray(), MeshTopology.Triangles, 0);
                    chunk.isDirty = false;
                }
                Graphics.DrawMeshNow(chunk.mesh, Handles.matrix);
            }
        }

        public static Material material
        {
            get
            {
                if (s_Material == null)
                {
                    Shader shader = (Shader) EditorGUIUtility.LoadRequired("Editors/AnimationWindow/ControlPoint.shader");
                    s_Material = new Material(shader);
                }
                return s_Material;
            }
        }

        private class RenderChunk
        {
            public List<Color32> colors;
            public List<int> indices;
            public bool isDirty = true;
            public Mesh mesh;
            public List<Vector2> uvs;
            public List<Vector3> vertices;
        }
    }
}

