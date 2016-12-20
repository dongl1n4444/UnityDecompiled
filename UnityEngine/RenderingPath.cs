namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Rendering path of a Camera.</para>
    /// </summary>
    public enum RenderingPath
    {
        /// <summary>
        /// <para>Deferred Lighting (Legacy).</para>
        /// </summary>
        DeferredLighting = 2,
        /// <summary>
        /// <para>Deferred Shading.</para>
        /// </summary>
        DeferredShading = 3,
        /// <summary>
        /// <para>Forward Rendering.</para>
        /// </summary>
        Forward = 1,
        /// <summary>
        /// <para>Use Player Settings.</para>
        /// </summary>
        UsePlayerSettings = -1,
        /// <summary>
        /// <para>Vertex Lit.</para>
        /// </summary>
        VertexLit = 0
    }
}

