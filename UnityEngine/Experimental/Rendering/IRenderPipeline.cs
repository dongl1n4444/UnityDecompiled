namespace UnityEngine.Experimental.Rendering
{
    using System;
    using UnityEngine;

    public interface IRenderPipeline : IDisposable
    {
        /// <summary>
        /// <para>Defines custom rendering for this RenderPipeline.</para>
        /// </summary>
        /// <param name="renderContext">Structure that holds the rendering commands for this loop.</param>
        /// <param name="cameras">Cameras to render.</param>
        void Render(ScriptableRenderContext renderContext, Camera[] cameras);

        /// <summary>
        /// <para>When the IRenderPipeline is invalid or destroyed this returns true.</para>
        /// </summary>
        bool disposed { get; }
    }
}

