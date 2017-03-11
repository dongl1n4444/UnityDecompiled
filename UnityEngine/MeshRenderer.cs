namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Renders meshes inserted by the MeshFilter or TextMesh.</para>
    /// </summary>
    public sealed class MeshRenderer : Renderer
    {
        /// <summary>
        /// <para>Vertex attributes in this mesh will override or add attributes of the primary mesh in the MeshRenderer.</para>
        /// </summary>
        public Mesh additionalVertexStreams { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

