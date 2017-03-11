namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A class to access the Mesh of the.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class MeshFilter : Component
    {
        /// <summary>
        /// <para>Returns the instantiated Mesh assigned to the mesh filter.</para>
        /// </summary>
        public Mesh mesh { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the shared mesh of the mesh filter.</para>
        /// </summary>
        public Mesh sharedMesh { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

