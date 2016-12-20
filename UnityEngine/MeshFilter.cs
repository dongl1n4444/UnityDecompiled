namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A class to access the Mesh of the.</para>
    /// </summary>
    public sealed class MeshFilter : Component
    {
        /// <summary>
        /// <para>Returns the instantiated Mesh assigned to the mesh filter.</para>
        /// </summary>
        public Mesh mesh { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the shared mesh of the mesh filter.</para>
        /// </summary>
        public Mesh sharedMesh { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

