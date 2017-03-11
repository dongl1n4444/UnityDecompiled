namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>An asset that produces a specific IRenderPipeline.</para>
    /// </summary>
    public abstract class RenderPipelineAsset : ScriptableObject, IRenderPipelineAsset
    {
        private readonly List<IRenderPipeline> m_CreatedPipelines = new List<IRenderPipeline>();

        protected RenderPipelineAsset()
        {
        }

        /// <summary>
        /// <para>Create a IRenderPipeline specific to this asset.</para>
        /// </summary>
        /// <returns>
        /// <para>Created pipeline.</para>
        /// </returns>
        public IRenderPipeline CreatePipeline()
        {
            IRenderPipeline item = this.InternalCreatePipeline();
            if (item != null)
            {
                this.m_CreatedPipelines.Add(item);
            }
            return item;
        }

        /// <summary>
        /// <para>Destroys all cached data and created IRenderLoop's.</para>
        /// </summary>
        public void DestroyCreatedInstances()
        {
            foreach (IRenderPipeline pipeline in this.m_CreatedPipelines)
            {
                pipeline.Dispose();
            }
            this.m_CreatedPipelines.Clear();
        }

        /// <summary>
        /// <para>Create a IRenderPipeline specific to this asset.</para>
        /// </summary>
        /// <returns>
        /// <para>Created pipeline.</para>
        /// </returns>
        protected abstract IRenderPipeline InternalCreatePipeline();
        private void OnDisable()
        {
            this.DestroyCreatedInstances();
        }

        private void OnValidate()
        {
            this.DestroyCreatedInstances();
        }
    }
}

