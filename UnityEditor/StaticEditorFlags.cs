namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Static Editor Flags.</para>
    /// </summary>
    [Flags]
    public enum StaticEditorFlags
    {
        /// <summary>
        /// <para>Consider for static batching.</para>
        /// </summary>
        BatchingStatic = 4,
        /// <summary>
        /// <para>Considered static for lightmapping.</para>
        /// </summary>
        LightmapStatic = 1,
        /// <summary>
        /// <para>Considered static for navigation.</para>
        /// </summary>
        NavigationStatic = 8,
        /// <summary>
        /// <para>Considered static for occlusion.</para>
        /// </summary>
        OccludeeStatic = 0x10,
        /// <summary>
        /// <para>Considered static for occlusion.</para>
        /// </summary>
        OccluderStatic = 2,
        /// <summary>
        /// <para>Auto-generate OffMeshLink.</para>
        /// </summary>
        OffMeshLinkGeneration = 0x20,
        /// <summary>
        /// <para>Consider static for reflection probe.</para>
        /// </summary>
        ReflectionProbeStatic = 0x40
    }
}

