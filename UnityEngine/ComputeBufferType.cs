namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>ComputeBuffer type.</para>
    /// </summary>
    [Flags]
    public enum ComputeBufferType
    {
        /// <summary>
        /// <para>Append-consume ComputeBuffer type.</para>
        /// </summary>
        Append = 2,
        /// <summary>
        /// <para>ComputeBuffer with a counter.</para>
        /// </summary>
        Counter = 4,
        /// <summary>
        /// <para>Default ComputeBuffer type.</para>
        /// </summary>
        Default = 0,
        /// <summary>
        /// <para>ComputeBuffer used for Graphics.DrawProceduralIndirect or ComputeShader.DispatchIndirect.</para>
        /// </summary>
        [Obsolete("Enum member DrawIndirect has been deprecated. Use IndirectArguments instead (UnityUpgradable) -> IndirectArguments", false)]
        DrawIndirect = 0x100,
        /// <summary>
        /// <para>ComputeBuffer is attempted to be located in GPU memory.</para>
        /// </summary>
        GPUMemory = 0x200,
        /// <summary>
        /// <para>ComputeBuffer used for Graphics.DrawProceduralIndirect or ComputeShader.DispatchIndirect.</para>
        /// </summary>
        IndirectArguments = 0x100,
        /// <summary>
        /// <para>Raw ComputeBuffer type.</para>
        /// </summary>
        Raw = 1
    }
}

