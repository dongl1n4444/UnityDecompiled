namespace UnityEngine.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>List of graphics commands to execute.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class CommandBuffer : IDisposable
    {
        internal IntPtr m_Ptr = IntPtr.Zero;

        /// <summary>
        /// <para>Create a new empty command buffer.</para>
        /// </summary>
        public CommandBuffer()
        {
            InitBuffer(this);
        }

        /// <summary>
        /// <para>Add a "blit into a render texture" command.</para>
        /// </summary>
        /// <param name="source">Source texture or render target to blit from.</param>
        /// <param name="dest">Destination to blit into.</param>
        /// <param name="mat">Material to use.</param>
        /// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
        public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            this.Blit_Identifier(ref source, ref dest, null, -1);
        }

        /// <summary>
        /// <para>Add a "blit into a render texture" command.</para>
        /// </summary>
        /// <param name="source">Source texture or render target to blit from.</param>
        /// <param name="dest">Destination to blit into.</param>
        /// <param name="mat">Material to use.</param>
        /// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
        public void Blit(Texture source, RenderTargetIdentifier dest)
        {
            this.Blit_Texture(source, ref dest, null, -1);
        }

        /// <summary>
        /// <para>Add a "blit into a render texture" command.</para>
        /// </summary>
        /// <param name="source">Source texture or render target to blit from.</param>
        /// <param name="dest">Destination to blit into.</param>
        /// <param name="mat">Material to use.</param>
        /// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
        public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat)
        {
            this.Blit_Identifier(ref source, ref dest, mat, -1);
        }

        /// <summary>
        /// <para>Add a "blit into a render texture" command.</para>
        /// </summary>
        /// <param name="source">Source texture or render target to blit from.</param>
        /// <param name="dest">Destination to blit into.</param>
        /// <param name="mat">Material to use.</param>
        /// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
        public void Blit(Texture source, RenderTargetIdentifier dest, Material mat)
        {
            this.Blit_Texture(source, ref dest, mat, -1);
        }

        /// <summary>
        /// <para>Add a "blit into a render texture" command.</para>
        /// </summary>
        /// <param name="source">Source texture or render target to blit from.</param>
        /// <param name="dest">Destination to blit into.</param>
        /// <param name="mat">Material to use.</param>
        /// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
        public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass)
        {
            this.Blit_Identifier(ref source, ref dest, mat, pass);
        }

        /// <summary>
        /// <para>Add a "blit into a render texture" command.</para>
        /// </summary>
        /// <param name="source">Source texture or render target to blit from.</param>
        /// <param name="dest">Destination to blit into.</param>
        /// <param name="mat">Material to use.</param>
        /// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
        public void Blit(Texture source, RenderTargetIdentifier dest, Material mat, int pass)
        {
            this.Blit_Texture(source, ref dest, mat, pass);
        }

        [ExcludeFromDocs]
        private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest)
        {
            int pass = -1;
            Material mat = null;
            this.Blit_Identifier(ref source, ref dest, mat, pass);
        }

        [ExcludeFromDocs]
        private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat)
        {
            int pass = -1;
            this.Blit_Identifier(ref source, ref dest, mat, pass);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Blit_Texture(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass);
        /// <summary>
        /// <para>Clear all commands in the buffer.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Clear();
        [ExcludeFromDocs]
        public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor)
        {
            float depth = 1f;
            INTERNAL_CALL_ClearRenderTarget(this, clearDepth, clearColor, ref backgroundColor, depth);
        }

        /// <summary>
        /// <para>Adds a "clear render target" command.</para>
        /// </summary>
        /// <param name="clearDepth">Should clear depth buffer?</param>
        /// <param name="clearColor">Should clear color buffer?</param>
        /// <param name="backgroundColor">Color to clear with.</param>
        /// <param name="depth">Depth to clear with (default is 1.0).</param>
        public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, [DefaultValue("1.0f")] float depth)
        {
            INTERNAL_CALL_ClearRenderTarget(this, clearDepth, clearColor, ref backgroundColor, depth);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            this.ReleaseBuffer();
            this.m_Ptr = IntPtr.Zero;
        }

        [ExcludeFromDocs]
        public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material)
        {
            MaterialPropertyBlock properties = null;
            int shaderPass = -1;
            int submeshIndex = 0;
            this.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
        }

        [ExcludeFromDocs]
        public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex)
        {
            MaterialPropertyBlock properties = null;
            int shaderPass = -1;
            this.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
        }

        [ExcludeFromDocs]
        public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass)
        {
            MaterialPropertyBlock properties = null;
            this.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
        }

        /// <summary>
        /// <para>Add a "draw mesh" command.</para>
        /// </summary>
        /// <param name="mesh">Mesh to draw.</param>
        /// <param name="matrix">Transformation matrix to use.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="submeshIndex">Which subset of the mesh to render.</param>
        /// <param name="shaderPass">Which pass of the shader to use (default is -1, which renders all passes).</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass, [DefaultValue("null")] MaterialPropertyBlock properties)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }
            if ((submeshIndex < 0) || (submeshIndex >= mesh.subMeshCount))
            {
                submeshIndex = Mathf.Clamp(submeshIndex, 0, mesh.subMeshCount - 1);
                Debug.LogWarning($"submeshIndex out of range. Clampped to {submeshIndex}.");
            }
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            this.Internal_DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
        }

        [ExcludeFromDocs]
        public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices)
        {
            MaterialPropertyBlock properties = null;
            int length = matrices.Length;
            this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, length, properties);
        }

        [ExcludeFromDocs]
        public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count)
        {
            MaterialPropertyBlock properties = null;
            this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, properties);
        }

        /// <summary>
        /// <para>Add a "draw mesh with instancing" command.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="shaderPass">Which pass of the shader to use, or -1 which renders all passes.</param>
        /// <param name="matrices">The array of object transformation matrices.</param>
        /// <param name="count">The number of instances to be drawn.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, [DefaultValue("matrices.Length")] int count, [DefaultValue("null")] MaterialPropertyBlock properties)
        {
            if (!SystemInfo.supportsInstancing)
            {
                throw new InvalidOperationException("DrawMeshInstanced is not supported.");
            }
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }
            if ((submeshIndex < 0) || (submeshIndex >= mesh.subMeshCount))
            {
                throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
            }
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            if (matrices == null)
            {
                throw new ArgumentNullException("matrices");
            }
            if ((count < 0) || (count > Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length)))
            {
                throw new ArgumentOutOfRangeException("count", $"Count must be in the range of 0 to {Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length)}.");
            }
            if (count > 0)
            {
                this.Internal_DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, properties);
            }
        }

        [ExcludeFromDocs]
        public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount)
        {
            MaterialPropertyBlock properties = null;
            int instanceCount = 1;
            this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
        }

        [ExcludeFromDocs]
        public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount)
        {
            MaterialPropertyBlock properties = null;
            this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
        }

        /// <summary>
        /// <para>Add a "draw procedural geometry" command.</para>
        /// </summary>
        /// <param name="matrix">Transformation matrix to use.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="shaderPass">Which pass of the shader to use (or -1 for all passes).</param>
        /// <param name="topology">Topology of the procedural geometry.</param>
        /// <param name="vertexCount">Vertex count to render.</param>
        /// <param name="instanceCount">Instance count to render.</param>
        /// <param name="properties">Additional material properties to apply just before rendering. See MaterialPropertyBlock.</param>
        public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount, [DefaultValue("null")] MaterialPropertyBlock properties)
        {
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            this.Internal_DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
        }

        [ExcludeFromDocs]
        public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
        {
            MaterialPropertyBlock properties = null;
            int argsOffset = 0;
            this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
        }

        [ExcludeFromDocs]
        public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
        {
            MaterialPropertyBlock properties = null;
            this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
        }

        /// <summary>
        /// <para>Add a "draw procedural geometry" command.</para>
        /// </summary>
        /// <param name="matrix">Transformation matrix to use.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="shaderPass">Which pass of the shader to use (or -1 for all passes).</param>
        /// <param name="topology">Topology of the procedural geometry.</param>
        /// <param name="properties">Additional material properties to apply just before rendering. See MaterialPropertyBlock.</param>
        /// <param name="bufferWithArgs">Buffer with draw arguments.</param>
        /// <param name="argsOffset">Byte offset where in the buffer the draw arguments are.</param>
        public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties)
        {
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            if (bufferWithArgs == null)
            {
                throw new ArgumentNullException("bufferWithArgs");
            }
            this.Internal_DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
        }

        [ExcludeFromDocs]
        public void DrawRenderer(Renderer renderer, Material material)
        {
            int shaderPass = -1;
            int submeshIndex = 0;
            this.DrawRenderer(renderer, material, submeshIndex, shaderPass);
        }

        [ExcludeFromDocs]
        public void DrawRenderer(Renderer renderer, Material material, int submeshIndex)
        {
            int shaderPass = -1;
            this.DrawRenderer(renderer, material, submeshIndex, shaderPass);
        }

        /// <summary>
        /// <para>Add a "draw renderer" command.</para>
        /// </summary>
        /// <param name="renderer">Renderer to draw.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="submeshIndex">Which subset of the mesh to render.</param>
        /// <param name="shaderPass">Which pass of the shader to use (default is -1, which renders all passes).</param>
        public void DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }
            if (submeshIndex < 0)
            {
                submeshIndex = Mathf.Max(submeshIndex, 0);
                Debug.LogWarning($"submeshIndex out of range. Clampped to {submeshIndex}.");
            }
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            this.Internal_DrawRenderer(renderer, material, submeshIndex, shaderPass);
        }

        ~CommandBuffer()
        {
            this.Dispose(false);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            FilterMode point = FilterMode.Point;
            int depthBuffer = 0;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, point, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            FilterMode point = FilterMode.Point;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, point, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            int antiAliasing = 1;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
        }

        /// <summary>
        /// <para>Add a "get a temporary render texture" command.</para>
        /// </summary>
        /// <param name="nameID">Shader property name for this texture.</param>
        /// <param name="width">Width in pixels, or -1 for "camera pixel width".</param>
        /// <param name="height">Height in pixels, or -1 for "camera pixel height".</param>
        /// <param name="depthBuffer">Depth buffer bits (0, 16 or 24).</param>
        /// <param name="filter">Texture filtering mode (default is Point).</param>
        /// <param name="format">Format of the render texture (default is ARGB32).</param>
        /// <param name="readWrite">Color space conversion mode.</param>
        /// <param name="antiAliasing">Anti-aliasing (default is no anti-aliasing).</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void GetTemporaryRT(int nameID, int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("FilterMode.Point")] FilterMode filter, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void InitBuffer(CommandBuffer buf);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ClearRenderTarget(CommandBuffer self, bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_DrawMesh(CommandBuffer self, Mesh mesh, ref Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_DrawProcedural(CommandBuffer self, ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_DrawProceduralIndirect(CommandBuffer self, ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetGlobalColor(CommandBuffer self, int nameID, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetGlobalMatrix(CommandBuffer self, int nameID, ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetGlobalVector(CommandBuffer self, int nameID, ref Vector4 value);
        private void Internal_DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
        {
            INTERNAL_CALL_Internal_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties);
        private void Internal_DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties)
        {
            INTERNAL_CALL_Internal_DrawProcedural(this, ref matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
        }

        private void Internal_DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
        {
            INTERNAL_CALL_Internal_DrawProceduralIndirect(this, ref matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
        }

        [ExcludeFromDocs]
        private void Internal_DrawRenderer(Renderer renderer, Material material)
        {
            int shaderPass = -1;
            int submeshIndex = 0;
            this.Internal_DrawRenderer(renderer, material, submeshIndex, shaderPass);
        }

        [ExcludeFromDocs]
        private void Internal_DrawRenderer(Renderer renderer, Material material, int submeshIndex)
        {
            int shaderPass = -1;
            this.Internal_DrawRenderer(renderer, material, submeshIndex, shaderPass);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass);
        /// <summary>
        /// <para>Send a user-defined event to a native code plugin.</para>
        /// </summary>
        /// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
        /// <param name="eventID">User defined id to send to the callback.</param>
        public void IssuePluginEvent(IntPtr callback, int eventID)
        {
            if (callback == IntPtr.Zero)
            {
                throw new ArgumentException("Null callback specified.");
            }
            this.IssuePluginEventInternal(callback, eventID);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void IssuePluginEventInternal(IntPtr callback, int eventID);
        public void Release()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void ReleaseBuffer();
        /// <summary>
        /// <para>Add a "release a temporary render texture" command.</para>
        /// </summary>
        /// <param name="nameID">Shader property name for this texture.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ReleaseTemporaryRT(int nameID);
        /// <summary>
        /// <para>Add a "set global shader buffer property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetGlobalBuffer(int nameID, ComputeBuffer value);
        /// <summary>
        /// <para>Add a "set global shader buffer property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalBuffer(string name, ComputeBuffer value)
        {
            this.SetGlobalBuffer(Shader.PropertyToID(name), value);
        }

        /// <summary>
        /// <para>Add a "set global shader color property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalColor(int nameID, Color value)
        {
            INTERNAL_CALL_SetGlobalColor(this, nameID, ref value);
        }

        /// <summary>
        /// <para>Add a "set global shader color property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalColor(string name, Color value)
        {
            this.SetGlobalColor(Shader.PropertyToID(name), value);
        }

        /// <summary>
        /// <para>Add a "set global shader float property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetGlobalFloat(int nameID, float value);
        /// <summary>
        /// <para>Add a "set global shader float property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalFloat(string name, float value)
        {
            this.SetGlobalFloat(Shader.PropertyToID(name), value);
        }

        public void SetGlobalFloatArray(int nameID, List<float> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetGlobalFloatArrayListImpl(nameID, values);
        }

        /// <summary>
        /// <para>Add a "set global shader float array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetGlobalFloatArray(int nameID, float[] values);
        public void SetGlobalFloatArray(string propertyName, List<float> values)
        {
            this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
        }

        /// <summary>
        /// <para>Add a "set global shader float array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public void SetGlobalFloatArray(string propertyName, float[] values)
        {
            this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetGlobalFloatArrayListImpl(int nameID, object values);
        /// <summary>
        /// <para>Add a "set global shader matrix property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalMatrix(int nameID, Matrix4x4 value)
        {
            INTERNAL_CALL_SetGlobalMatrix(this, nameID, ref value);
        }

        /// <summary>
        /// <para>Add a "set global shader matrix property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalMatrix(string name, Matrix4x4 value)
        {
            this.SetGlobalMatrix(Shader.PropertyToID(name), value);
        }

        public void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
        {
            this.SetGlobalMatrixArrayListImpl(nameID, values);
        }

        /// <summary>
        /// <para>Add a "set global shader matrix array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetGlobalMatrixArray(int nameID, Matrix4x4[] values);
        public void SetGlobalMatrixArray(string propertyName, List<Matrix4x4> values)
        {
            this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
        }

        /// <summary>
        /// <para>Add a "set global shader matrix array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
        {
            this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetGlobalMatrixArrayListImpl(int nameID, object values);
        /// <summary>
        /// <para>Add a "set global shader texture property" command, referencing a RenderTexture.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalTexture(int nameID, RenderTargetIdentifier value)
        {
            this.SetGlobalTexture_Impl(nameID, ref value);
        }

        /// <summary>
        /// <para>Add a "set global shader texture property" command, referencing a RenderTexture.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalTexture(string name, RenderTargetIdentifier value)
        {
            this.SetGlobalTexture(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetGlobalTexture_Impl(int nameID, ref RenderTargetIdentifier rt);
        /// <summary>
        /// <para>Add a "set global shader vector property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalVector(int nameID, Vector4 value)
        {
            INTERNAL_CALL_SetGlobalVector(this, nameID, ref value);
        }

        /// <summary>
        /// <para>Add a "set global shader vector property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalVector(string name, Vector4 value)
        {
            this.SetGlobalVector(Shader.PropertyToID(name), value);
        }

        public void SetGlobalVectorArray(int nameID, List<Vector4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetGlobalVectorArrayListImpl(nameID, values);
        }

        /// <summary>
        /// <para>Add a "set global shader vector array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetGlobalVectorArray(int nameID, Vector4[] values);
        public void SetGlobalVectorArray(string propertyName, List<Vector4> values)
        {
            this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
        }

        /// <summary>
        /// <para>Add a "set global shader vector array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public void SetGlobalVectorArray(string propertyName, Vector4[] values)
        {
            this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetGlobalVectorArrayListImpl(int nameID, object values);
        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier rt)
        {
            this.SetRenderTarget_Single(ref rt, 0, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel)
        {
            this.SetRenderTarget_Single(ref rt, mipLevel, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, 0, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth)
        {
            this.SetRenderTarget_Multiple(colors, ref depth);
        }

        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace)
        {
            this.SetRenderTarget_Single(ref rt, mipLevel, cubemapFace, 0);
        }

        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, mipLevel, CubemapFace.Unknown, 0);
        }

        public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace, int depthSlice)
        {
            this.SetRenderTarget_Single(ref rt, mipLevel, cubemapFace, depthSlice);
        }

        /// <summary>
        /// <para>Add a "set active render target" command.</para>
        /// </summary>
        /// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
        /// <param name="color">Render target to set as a color buffer.</param>
        /// <param name="colors">Render targets to set as color buffers (MRT).</param>
        /// <param name="depth">Render target to set as a depth buffer.</param>
        /// <param name="mipLevel">The mip level of the render target to render into.</param>
        /// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, mipLevel, cubemapFace, 0);
        }

        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, mipLevel, cubemapFace, depthSlice);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetRenderTarget_ColDepth(ref RenderTargetIdentifier color, ref RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetRenderTarget_Multiple(RenderTargetIdentifier[] color, ref RenderTargetIdentifier depth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetRenderTarget_Single(ref RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace, int depthSlice);
        /// <summary>
        /// <para>Add a "set shadow sampling mode" command.</para>
        /// </summary>
        /// <param name="shadowmap">Shadowmap render target to change the sampling mode on.</param>
        /// <param name="mode">New sampling mode.</param>
        public void SetShadowSamplingMode(RenderTargetIdentifier shadowmap, ShadowSamplingMode mode)
        {
            this.SetShadowSamplingMode_Impl(ref shadowmap, mode);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetShadowSamplingMode_Impl(ref RenderTargetIdentifier shadowmap, ShadowSamplingMode mode);

        /// <summary>
        /// <para>Name of this command buffer.</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Size of this command buffer in bytes (Read Only).</para>
        /// </summary>
        public int sizeInBytes { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

