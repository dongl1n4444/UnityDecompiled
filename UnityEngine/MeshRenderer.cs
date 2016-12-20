namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Renders meshes inserted by the MeshFilter or TextMesh.</para>
    /// </summary>
    public sealed class MeshRenderer : Renderer
    {
        /// <summary>
        /// <para>Vertex attributes in this mesh will override or add attributes of the primary mesh in the MeshRenderer.</para>
        /// </summary>
        public Mesh additionalVertexStreams { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

