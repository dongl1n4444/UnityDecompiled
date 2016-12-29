namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A component that will render to the screen after all normal rendering has completed when attached to a Canvas. Designed for GUI application.</para>
    /// </summary>
    public sealed class CanvasRenderer : Component
    {
        public static  event OnRequestRebuild onRequestRebuild;

        public static void AddUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents)
        {
            SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, normals, tangents);
        }

        /// <summary>
        /// <para>Remove all cached vertices.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Clear();
        public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indicies)
        {
            CreateUIVertexStreamInternal(verts, positions, colors, uv0S, uv1S, normals, tangents, indicies);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CreateUIVertexStreamInternal(object verts, object positions, object colors, object uv0S, object uv1S, object normals, object tangents, object indicies);
        /// <summary>
        /// <para>Disables rectangle clipping for this CanvasRenderer.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DisableRectClipping();
        /// <summary>
        /// <para>Enables rect clipping on the CanvasRendered. Geometry outside of the specified rect will be clipped (not rendered).</para>
        /// </summary>
        /// <param name="rect"></param>
        public void EnableRectClipping(Rect rect)
        {
            INTERNAL_CALL_EnableRectClipping(this, ref rect);
        }

        /// <summary>
        /// <para>Get the current alpha of the renderer.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetAlpha();
        /// <summary>
        /// <para>Get the current color of the renderer.</para>
        /// </summary>
        public Color GetColor()
        {
            Color color;
            INTERNAL_CALL_GetColor(this, out color);
            return color;
        }

        /// <summary>
        /// <para>Gets the current Material assigned to the CanvasRenderer.</para>
        /// </summary>
        /// <param name="index">The material index to retrieve (0 if this parameter is omitted).</param>
        /// <returns>
        /// <para>Result.</para>
        /// </returns>
        public Material GetMaterial() => 
            this.GetMaterial(0);

        /// <summary>
        /// <para>Gets the current Material assigned to the CanvasRenderer.</para>
        /// </summary>
        /// <param name="index">The material index to retrieve (0 if this parameter is omitted).</param>
        /// <returns>
        /// <para>Result.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Material GetMaterial(int index);
        /// <summary>
        /// <para>Gets the current Material assigned to the CanvasRenderer. Used internally for masking.</para>
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Material GetPopMaterial(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_EnableRectClipping(CanvasRenderer self, ref Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetColor(CanvasRenderer self, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetColor(CanvasRenderer self, ref Color color);
        [RequiredByNativeCode]
        private static void RequestRefresh()
        {
            if (onRequestRebuild != null)
            {
                onRequestRebuild();
            }
        }

        /// <summary>
        /// <para>Set the alpha of the renderer. Will be multiplied with the UIVertex alpha and the Canvas alpha.</para>
        /// </summary>
        /// <param name="alpha">Alpha.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetAlpha(float alpha);
        /// <summary>
        /// <para>The Alpha Texture that will be passed to the Shader under the _AlphaTex property.</para>
        /// </summary>
        /// <param name="texture">The Texture to be passed.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetAlphaTexture(Texture texture);
        /// <summary>
        /// <para>Set the color of the renderer. Will be multiplied with the UIVertex color and the Canvas color.</para>
        /// </summary>
        /// <param name="color">Renderer multiply color.</param>
        public void SetColor(Color color)
        {
            INTERNAL_CALL_SetColor(this, ref color);
        }

        /// <summary>
        /// <para>Set the material for the canvas renderer. If a texture is specified then it will be used as the 'MainTex' instead of the material's 'MainTex'.
        /// See Also: CanvasRenderer.SetMaterialCount, CanvasRenderer.SetTexture.</para>
        /// </summary>
        /// <param name="material">Material for rendering.</param>
        /// <param name="texture">Material texture overide.</param>
        /// <param name="index">Material index.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetMaterial(Material material, int index);
        /// <summary>
        /// <para>Set the material for the canvas renderer. If a texture is specified then it will be used as the 'MainTex' instead of the material's 'MainTex'.
        /// See Also: CanvasRenderer.SetMaterialCount, CanvasRenderer.SetTexture.</para>
        /// </summary>
        /// <param name="material">Material for rendering.</param>
        /// <param name="texture">Material texture overide.</param>
        /// <param name="index">Material index.</param>
        public void SetMaterial(Material material, Texture texture)
        {
            this.materialCount = Math.Max(1, this.materialCount);
            this.SetMaterial(material, 0);
            this.SetTexture(texture);
        }

        /// <summary>
        /// <para>Sets the Mesh used by this renderer.</para>
        /// </summary>
        /// <param name="mesh"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetMesh(Mesh mesh);
        /// <summary>
        /// <para>Set the material for the canvas renderer. Used internally for masking.</para>
        /// </summary>
        /// <param name="material"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetPopMaterial(Material material, int index);
        /// <summary>
        /// <para>Sets the texture used by this renderer's material.</para>
        /// </summary>
        /// <param name="texture"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTexture(Texture texture);
        [Obsolete("UI System now uses meshes. Generate a mesh and use 'SetMesh' instead")]
        public void SetVertices(List<UIVertex> vertices)
        {
            this.SetVertices(vertices.ToArray(), vertices.Count);
        }

        /// <summary>
        /// <para>Set the vertices for the UIRenderer.</para>
        /// </summary>
        /// <param name="vertices">Array of vertices to set.</param>
        /// <param name="size">Number of vertices to set.</param>
        [Obsolete("UI System now uses meshes. Generate a mesh and use 'SetMesh' instead")]
        public void SetVertices(UIVertex[] vertices, int size)
        {
            Mesh mesh = new Mesh();
            List<Vector3> inVertices = new List<Vector3>();
            List<Color32> inColors = new List<Color32>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector2> list4 = new List<Vector2>();
            List<Vector3> inNormals = new List<Vector3>();
            List<Vector4> inTangents = new List<Vector4>();
            List<int> list7 = new List<int>();
            for (int i = 0; i < size; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    inVertices.Add(vertices[i + j].position);
                    inColors.Add(vertices[i + j].color);
                    uvs.Add(vertices[i + j].uv0);
                    list4.Add(vertices[i + j].uv1);
                    inNormals.Add(vertices[i + j].normal);
                    inTangents.Add(vertices[i + j].tangent);
                }
                list7.Add(i);
                list7.Add(i + 1);
                list7.Add(i + 2);
                list7.Add(i + 2);
                list7.Add(i + 3);
                list7.Add(i);
            }
            mesh.SetVertices(inVertices);
            mesh.SetColors(inColors);
            mesh.SetNormals(inNormals);
            mesh.SetTangents(inTangents);
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(1, list4);
            mesh.SetIndices(list7.ToArray(), MeshTopology.Triangles, 0);
            this.SetMesh(mesh);
            UnityEngine.Object.DestroyImmediate(mesh);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SplitIndiciesStreamsInternal(object verts, object indicies);
        public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indicies)
        {
            SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, normals, tangents);
            SplitIndiciesStreamsInternal(verts, indicies);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SplitUIVertexStreamsInternal(object verts, object positions, object colors, object uv0S, object uv1S, object normals, object tangents);

        /// <summary>
        /// <para>Depth of the renderer relative to the root canvas.</para>
        /// </summary>
        public int absoluteDepth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Indicates whether geometry emitted by this renderer is ignored.</para>
        /// </summary>
        public bool cull { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>True if any change has occured that would invalidate the positions of generated geometry.</para>
        /// </summary>
        public bool hasMoved { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Enable 'render stack' pop draw call.</para>
        /// </summary>
        public bool hasPopInstruction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>True if rect clipping has been enabled on this renderer.
        /// See Also: CanvasRenderer.EnableRectClipping, CanvasRenderer.DisableRectClipping.</para>
        /// </summary>
        public bool hasRectClipping { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is the UIRenderer a mask component.</para>
        /// </summary>
        [Obsolete("isMask is no longer supported. See EnableClipping for vertex clipping configuration")]
        public bool isMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of materials usable by this renderer.</para>
        /// </summary>
        public int materialCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of materials usable by this renderer. Used internally for masking.</para>
        /// </summary>
        public int popMaterialCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Depth of the renderer realative to the parent canvas.</para>
        /// </summary>
        public int relativeDepth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public delegate void OnRequestRebuild();
    }
}

