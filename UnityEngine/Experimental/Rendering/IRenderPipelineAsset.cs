namespace UnityEngine.Experimental.Rendering
{
    using System;

    public interface IRenderPipelineAsset
    {
        /// <summary>
        /// <para>Create a IRenderPipeline specific to this asset.</para>
        /// </summary>
        /// <returns>
        /// <para>Created pipeline.</para>
        /// </returns>
        IRenderPipeline CreatePipeline();
        /// <summary>
        /// <para>Override this method to destroy RenderPipeline cached state.</para>
        /// </summary>
        void DestroyCreatedInstances();
    }
}

