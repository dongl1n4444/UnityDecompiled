namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal static class RenderPipelineManager
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static IRenderPipeline <currentPipeline>k__BackingField;
        private static IRenderPipelineAsset s_CurrentPipelineAsset;

        [RequiredByNativeCode]
        internal static void CleanupRenderPipeline()
        {
            if (s_CurrentPipelineAsset != null)
            {
                s_CurrentPipelineAsset.DestroyCreatedInstances();
            }
            s_CurrentPipelineAsset = null;
            currentPipeline = null;
        }

        [RequiredByNativeCode]
        private static bool DoRenderLoop_Internal(IRenderPipelineAsset pipe, Camera[] cameras, IntPtr loopPtr)
        {
            if (!PrepareRenderPipeline(pipe))
            {
                return false;
            }
            ScriptableRenderContext renderContext = new ScriptableRenderContext();
            renderContext.Initialize(loopPtr);
            currentPipeline.Render(renderContext, cameras);
            return true;
        }

        private static bool PrepareRenderPipeline(IRenderPipelineAsset pipe)
        {
            if (s_CurrentPipelineAsset != pipe)
            {
                if (s_CurrentPipelineAsset != null)
                {
                    CleanupRenderPipeline();
                }
                s_CurrentPipelineAsset = pipe;
            }
            if ((s_CurrentPipelineAsset != null) && ((currentPipeline == null) || currentPipeline.disposed))
            {
                currentPipeline = s_CurrentPipelineAsset.CreatePipeline();
            }
            return (s_CurrentPipelineAsset != null);
        }

        public static IRenderPipeline currentPipeline
        {
            [CompilerGenerated]
            get => 
                <currentPipeline>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <currentPipeline>k__BackingField = value;
            }
        }
    }
}

