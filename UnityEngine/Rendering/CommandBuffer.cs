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
        /// <para>Adds a command to begin profile sampling.</para>
        /// </summary>
        /// <param name="name">Name of the profile information used for sampling.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void BeginSample(string name);
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Blit_Texture(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass);
        /// <summary>
        /// <para>Clear all commands in the buffer.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        /// <summary>
        /// <para>Adds a command to copy ComputeBuffer counter value.</para>
        /// </summary>
        /// <param name="src">Append/consume buffer to copy the counter from.</param>
        /// <param name="dst">A buffer to copy the counter to.</param>
        /// <param name="dstOffset">Target byte offset in dst buffer.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void CopyCounterValue(ComputeBuffer src, ComputeBuffer dst, uint dstOffset);
        /// <summary>
        /// <para>Adds a command to copy a texture into another texture.</para>
        /// </summary>
        /// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public void CopyTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
        {
            this.CopyTexture_Internal(ref src, -1, -1, -1, -1, -1, -1, ref dst, -1, -1, -1, -1, 1);
        }

        /// <summary>
        /// <para>Adds a command to copy a texture into another texture.</para>
        /// </summary>
        /// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public void CopyTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
        {
            this.CopyTexture_Internal(ref src, srcElement, -1, -1, -1, -1, -1, ref dst, dstElement, -1, -1, -1, 2);
        }

        /// <summary>
        /// <para>Adds a command to copy a texture into another texture.</para>
        /// </summary>
        /// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, RenderTargetIdentifier dst, int dstElement, int dstMip)
        {
            this.CopyTexture_Internal(ref src, srcElement, srcMip, -1, -1, -1, -1, ref dst, dstElement, dstMip, -1, -1, 3);
        }

        /// <summary>
        /// <para>Adds a command to copy a texture into another texture.</para>
        /// </summary>
        /// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY)
        {
            this.CopyTexture_Internal(ref src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, ref dst, dstElement, dstMip, dstX, dstY, 4);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void CopyTexture_Internal(ref RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, ref RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY, int mode);
        /// <summary>
        /// <para>Adds a command to disable global shader keyword.</para>
        /// </summary>
        /// <param name="keyword">Shader keyword to disable.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DisableShaderKeyword(string keyword);
        /// <summary>
        /// <para>Add a command to execute a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to execute.</param>
        /// <param name="kernelIndex">Kernel index to execute, see ComputeShader.FindKernel.</param>
        /// <param name="threadGroupsX">Number of work groups in the X dimension.</param>
        /// <param name="threadGroupsY">Number of work groups in the Y dimension.</param>
        /// <param name="threadGroupsZ">Number of work groups in the Z dimension.</param>
        /// <param name="indirectBuffer">ComputeBuffer with dispatch arguments.</param>
        /// <param name="argsOffset">Byte offset indicating the location of the dispatch arguments in the buffer.</param>
        public void DispatchCompute(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset)
        {
            this.Internal_DispatchComputeIndirect(computeShader, kernelIndex, indirectBuffer, argsOffset);
        }

        /// <summary>
        /// <para>Add a command to execute a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to execute.</param>
        /// <param name="kernelIndex">Kernel index to execute, see ComputeShader.FindKernel.</param>
        /// <param name="threadGroupsX">Number of work groups in the X dimension.</param>
        /// <param name="threadGroupsY">Number of work groups in the Y dimension.</param>
        /// <param name="threadGroupsZ">Number of work groups in the Z dimension.</param>
        /// <param name="indirectBuffer">ComputeBuffer with dispatch arguments.</param>
        /// <param name="argsOffset">Byte offset indicating the location of the dispatch arguments in the buffer.</param>
        public void DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
        {
            this.Internal_DispatchCompute(computeShader, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
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
        public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs)
        {
            MaterialPropertyBlock properties = null;
            int argsOffset = 0;
            this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
        }

        [ExcludeFromDocs]
        public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset)
        {
            MaterialPropertyBlock properties = null;
            this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
        }

        /// <summary>
        /// <para>Add a "draw mesh with indirect instancing" command.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="shaderPass">Which pass of the shader to use, or -1 which renders all passes.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="bufferWithArgs">The GPU buffer containing the arguments for how many instances of this mesh to draw.</param>
        /// <param name="argsOffset">The byte offset into the buffer, where the draw arguments start.</param>
        public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties)
        {
            if (!SystemInfo.supportsInstancing)
            {
                throw new InvalidOperationException("Instancing is not supported.");
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
            if (bufferWithArgs == null)
            {
                throw new ArgumentNullException("bufferWithArgs");
            }
            this.Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
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

        /// <summary>
        /// <para>Adds a command to enable global shader keyword.</para>
        /// </summary>
        /// <param name="keyword">Shader keyword to enable.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void EnableShaderKeyword(string keyword);
        /// <summary>
        /// <para>Adds a command to begin profile sampling.</para>
        /// </summary>
        /// <param name="name">Name of the profile information used for sampling.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void EndSample(string name);
        ~CommandBuffer()
        {
            this.Dispose(false);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height)
        {
            bool enableRandomWrite = false;
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            FilterMode point = FilterMode.Point;
            int depthBuffer = 0;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, point, format, readWrite, antiAliasing, enableRandomWrite);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer)
        {
            bool enableRandomWrite = false;
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            FilterMode point = FilterMode.Point;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, point, format, readWrite, antiAliasing, enableRandomWrite);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter)
        {
            bool enableRandomWrite = false;
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format)
        {
            bool enableRandomWrite = false;
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            bool enableRandomWrite = false;
            int antiAliasing = 1;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite);
        }

        [ExcludeFromDocs]
        public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
        {
            bool enableRandomWrite = false;
            this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite);
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
        /// <param name="enableRandomWrite">Should random-write access into the texture be enabled (default is false).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void GetTemporaryRT(int nameID, int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("FilterMode.Point")] FilterMode filter, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("false")] bool enableRandomWrite);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void InitBuffer(CommandBuffer buf);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClearRenderTarget(CommandBuffer self, bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawMesh(CommandBuffer self, Mesh mesh, ref Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawProcedural(CommandBuffer self, ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawProceduralIndirect(CommandBuffer self, ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetComputeVectorParam(CommandBuffer self, ComputeShader computeShader, string name, ref Vector4 val);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetGlobalColor(CommandBuffer self, int nameID, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetGlobalMatrix(CommandBuffer self, int nameID, ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetGlobalVector(CommandBuffer self, int nameID, ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetProjectionMatrix(CommandBuffer self, ref Matrix4x4 proj);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetViewMatrix(CommandBuffer self, ref Matrix4x4 view);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetViewport(CommandBuffer self, ref Rect pixelRect);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetViewProjectionMatrices(CommandBuffer self, ref Matrix4x4 view, ref Matrix4x4 proj);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_DispatchComputeIndirect(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset);
        private void Internal_DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
        {
            INTERNAL_CALL_Internal_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetComputeFloats(ComputeShader computeShader, string name, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetComputeInts(ComputeShader computeShader, string name, int[] values);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, ref RenderTargetIdentifier rt);
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void IssuePluginEventInternal(IntPtr callback, int eventID);
        public void Release()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void ReleaseBuffer();
        /// <summary>
        /// <para>Add a "release a temporary render texture" command.</para>
        /// </summary>
        /// <param name="nameID">Shader property name for this texture.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ReleaseTemporaryRT(int nameID);
        /// <summary>
        /// <para>Adds a command to set an input or output buffer parameter on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="kernelIndex">Which kernel the buffer is being set for. See ComputeShader.FindKernel.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="buffer">Buffer to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, ComputeBuffer buffer);
        /// <summary>
        /// <para>Adds a command to set a float parameter on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="name">Name of the variable in shader code.</param>
        /// <param name="val">Value to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetComputeFloatParam(ComputeShader computeShader, string name, float val);
        /// <summary>
        /// <para>Adds a command to set multiple consecutive float parameters on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="name">Name of the variable in shader code.</param>
        /// <param name="values">Values to set.</param>
        public void SetComputeFloatParams(ComputeShader computeShader, string name, params float[] values)
        {
            this.Internal_SetComputeFloats(computeShader, name, values);
        }

        /// <summary>
        /// <para>Adds a command to set an integer parameter on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="name">Name of the variable in shader code.</param>
        /// <param name="val">Value to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetComputeIntParam(ComputeShader computeShader, string name, int val);
        /// <summary>
        /// <para>Adds a command to set multiple consecutive integer parameters on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="name">Name of the variable in shader code.</param>
        /// <param name="values">Values to set.</param>
        public void SetComputeIntParams(ComputeShader computeShader, string name, params int[] values)
        {
            this.Internal_SetComputeInts(computeShader, name, values);
        }

        /// <summary>
        /// <para>Adds a command to set a texture parameter on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="kernelIndex">Which kernel the texture is being set for. See ComputeShader.FindKernel.</param>
        /// <param name="name">Name of the texture variable in shader code.</param>
        /// <param name="rt">Texture value or identifier to set, see RenderTargetIdentifier.</param>
        public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt)
        {
            this.Internal_SetComputeTextureParam(computeShader, kernelIndex, name, ref rt);
        }

        /// <summary>
        /// <para>Adds a command to set a vector parameter on a ComputeShader.</para>
        /// </summary>
        /// <param name="computeShader">ComputeShader to set parameter for.</param>
        /// <param name="name">Name of the variable in shader code.</param>
        /// <param name="val">Value to set.</param>
        public void SetComputeVectorParam(ComputeShader computeShader, string name, Vector4 val)
        {
            INTERNAL_CALL_SetComputeVectorParam(this, computeShader, name, ref val);
        }

        /// <summary>
        /// <para>Add a "set global shader buffer property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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
        /// <para>Add a command to set global depth bias.</para>
        /// </summary>
        /// <param name="bias">Constant depth bias.</param>
        /// <param name="slopeBias">Slope-dependent depth bias.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetGlobalDepthBias(float bias, float slopeBias);
        /// <summary>
        /// <para>Add a "set global shader float property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetGlobalMatrixArrayListImpl(nameID, values);
        }

        /// <summary>
        /// <para>Add a "set global shader matrix array property" command.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetGlobalMatrixArrayListImpl(int nameID, object values);
        /// <summary>
        /// <para>Add a "set global shader texture property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalTexture(int nameID, RenderTargetIdentifier value)
        {
            this.SetGlobalTexture_Impl(nameID, ref value);
        }

        /// <summary>
        /// <para>Add a "set global shader texture property" command.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public void SetGlobalTexture(string name, RenderTargetIdentifier value)
        {
            this.SetGlobalTexture(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetGlobalVectorArrayListImpl(int nameID, object values);
        /// <summary>
        /// <para>Add a command to set the projection matrix.</para>
        /// </summary>
        /// <param name="proj">Projection (camera to clip space) matrix.</param>
        public void SetProjectionMatrix(Matrix4x4 proj)
        {
            INTERNAL_CALL_SetProjectionMatrix(this, ref proj);
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, mipLevel, CubemapFace.Unknown, 0);
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, mipLevel, cubemapFace, 0);
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
        /// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
        public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
        {
            this.SetRenderTarget_ColDepth(ref color, ref depth, mipLevel, cubemapFace, depthSlice);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetRenderTarget_ColDepth(ref RenderTargetIdentifier color, ref RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetRenderTarget_Multiple(RenderTargetIdentifier[] color, ref RenderTargetIdentifier depth);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetShadowSamplingMode_Impl(ref RenderTargetIdentifier shadowmap, ShadowSamplingMode mode);
        /// <summary>
        /// <para>Add a command to set the view matrix.</para>
        /// </summary>
        /// <param name="view">View (world to camera space) matrix.</param>
        public void SetViewMatrix(Matrix4x4 view)
        {
            INTERNAL_CALL_SetViewMatrix(this, ref view);
        }

        /// <summary>
        /// <para>Add a command to set the rendering viewport.</para>
        /// </summary>
        /// <param name="pixelRect">Viewport rectangle in pixel coordinates.</param>
        public void SetViewport(Rect pixelRect)
        {
            INTERNAL_CALL_SetViewport(this, ref pixelRect);
        }

        /// <summary>
        /// <para>Add a command to set the view and projection matrices.</para>
        /// </summary>
        /// <param name="view">View (world to camera space) matrix.</param>
        /// <param name="proj">Projection (camera to clip space) matrix.</param>
        public void SetViewProjectionMatrices(Matrix4x4 view, Matrix4x4 proj)
        {
            INTERNAL_CALL_SetViewProjectionMatrices(this, ref view, ref proj);
        }

        /// <summary>
        /// <para>Name of this command buffer.</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Size of this command buffer in bytes (Read Only).</para>
        /// </summary>
        public int sizeInBytes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

