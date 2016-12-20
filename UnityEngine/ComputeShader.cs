namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Compute Shader asset.</para>
    /// </summary>
    public sealed class ComputeShader : UnityEngine.Object
    {
        /// <summary>
        /// <para>Execute a compute shader.</para>
        /// </summary>
        /// <param name="kernelIndex">Which kernel to execute. A single compute shader asset can have multiple kernel entry points.</param>
        /// <param name="threadGroupsX">Number of work groups in the X dimension.</param>
        /// <param name="threadGroupsY">Number of work groups in the Y dimension.</param>
        /// <param name="threadGroupsZ">Number of work groups in the Z dimension.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Dispatch(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);
        [ExcludeFromDocs]
        public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer)
        {
            uint argsOffset = 0;
            this.DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
        }

        /// <summary>
        /// <para>Execute a compute shader.</para>
        /// </summary>
        /// <param name="kernelIndex">Which kernel to execute. A single compute shader asset can have multiple kernel entry points.</param>
        /// <param name="argsBuffer">Buffer with dispatch arguments.</param>
        /// <param name="argsOffset">Byte offset where in the buffer the dispatch arguments are.</param>
        public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
        {
            if (argsBuffer == null)
            {
                throw new ArgumentNullException("argsBuffer");
            }
            if (argsBuffer.m_Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("argsBuffer");
            }
            this.Internal_DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
        }

        /// <summary>
        /// <para>Find ComputeShader kernel index.</para>
        /// </summary>
        /// <param name="name">Name of kernel function.</param>
        /// <returns>
        /// <para>Kernel index, or -1 if not found.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int FindKernel(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void GetKernelThreadGroupSizes(int kernelIndex, out uint x, out uint y, out uint z);
        /// <summary>
        /// <para>Checks whether a shader contains a given kernel.</para>
        /// </summary>
        /// <param name="name">The name of the kernel to look for.</param>
        /// <returns>
        /// <para>True if the kernel is found, false otherwise.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool HasKernel(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetVector(ComputeShader self, int nameID, ref Vector4 val);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, uint argsOffset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetFloats(int nameID, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetInts(int nameID, int[] values);
        /// <summary>
        /// <para>Set a bool parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        public void SetBool(int nameID, bool val)
        {
            this.SetInt(nameID, !val ? 0 : 1);
        }

        /// <summary>
        /// <para>Set a bool parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        public void SetBool(string name, bool val)
        {
            this.SetInt(name, !val ? 0 : 1);
        }

        /// <summary>
        /// <para>Sets an input or output compute buffer.</para>
        /// </summary>
        /// <param name="kernelIndex">For which kernel the buffer is being set. See FindKernel.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="buffer">Buffer to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetBuffer(int kernelIndex, int nameID, ComputeBuffer buffer);
        /// <summary>
        /// <para>Sets an input or output compute buffer.</para>
        /// </summary>
        /// <param name="kernelIndex">For which kernel the buffer is being set. See FindKernel.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="buffer">Buffer to set.</param>
        public void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer)
        {
            this.SetBuffer(kernelIndex, Shader.PropertyToID(name), buffer);
        }

        /// <summary>
        /// <para>Set a float parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetFloat(int nameID, float val);
        /// <summary>
        /// <para>Set a float parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        public void SetFloat(string name, float val)
        {
            this.SetFloat(Shader.PropertyToID(name), val);
        }

        /// <summary>
        /// <para>Set multiple consecutive float parameters at once.</para>
        /// </summary>
        /// <param name="name">Array variable name in the shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Value array to set.</param>
        public void SetFloats(int nameID, params float[] values)
        {
            this.Internal_SetFloats(nameID, values);
        }

        /// <summary>
        /// <para>Set multiple consecutive float parameters at once.</para>
        /// </summary>
        /// <param name="name">Array variable name in the shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Value array to set.</param>
        public void SetFloats(string name, params float[] values)
        {
            this.Internal_SetFloats(Shader.PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Set an integer parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetInt(int nameID, int val);
        /// <summary>
        /// <para>Set an integer parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        public void SetInt(string name, int val)
        {
            this.SetInt(Shader.PropertyToID(name), val);
        }

        /// <summary>
        /// <para>Set multiple consecutive integer parameters at once.</para>
        /// </summary>
        /// <param name="name">Array variable name in the shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Value array to set.</param>
        public void SetInts(int nameID, params int[] values)
        {
            this.Internal_SetInts(nameID, values);
        }

        /// <summary>
        /// <para>Set multiple consecutive integer parameters at once.</para>
        /// </summary>
        /// <param name="name">Array variable name in the shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Value array to set.</param>
        public void SetInts(string name, params int[] values)
        {
            this.Internal_SetInts(Shader.PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Set a texture parameter.</para>
        /// </summary>
        /// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="texture">Texture to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTexture(int kernelIndex, int nameID, Texture texture);
        /// <summary>
        /// <para>Set a texture parameter.</para>
        /// </summary>
        /// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="texture">Texture to set.</param>
        public void SetTexture(int kernelIndex, string name, Texture texture)
        {
            this.SetTexture(kernelIndex, Shader.PropertyToID(name), texture);
        }

        /// <summary>
        /// <para>Set a texture parameter from a global texture property.</para>
        /// </summary>
        /// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="globalTextureName">Global texture property to assign to shader.</param>
        /// <param name="globalTextureNameID">Property name ID, use Shader.PropertyToID to get it.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTextureFromGlobal(int kernelIndex, int nameID, int globalTextureNameID);
        /// <summary>
        /// <para>Set a texture parameter from a global texture property.</para>
        /// </summary>
        /// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Name of the buffer variable in shader code.</param>
        /// <param name="globalTextureName">Global texture property to assign to shader.</param>
        /// <param name="globalTextureNameID">Property name ID, use Shader.PropertyToID to get it.</param>
        public void SetTextureFromGlobal(int kernelIndex, string name, string globalTextureName)
        {
            this.SetTextureFromGlobal(kernelIndex, Shader.PropertyToID(name), Shader.PropertyToID(globalTextureName));
        }

        /// <summary>
        /// <para>Set a vector parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        public void SetVector(int nameID, Vector4 val)
        {
            INTERNAL_CALL_SetVector(this, nameID, ref val);
        }

        /// <summary>
        /// <para>Set a vector parameter.</para>
        /// </summary>
        /// <param name="name">Variable name in shader code.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="val">Value to set.</param>
        public void SetVector(string name, Vector4 val)
        {
            this.SetVector(Shader.PropertyToID(name), val);
        }
    }
}

