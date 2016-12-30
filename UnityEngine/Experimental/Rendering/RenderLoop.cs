namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class for building custom rendering loops.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderLoop
    {
        private IntPtr m_Ptr;
        /// <summary>
        /// <para>Register a custom rendering loop.</para>
        /// </summary>
        public static PrepareRenderLoop renderLoopDelegate;
        /// <summary>
        /// <para>Submit rendering loop for execution.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Submit();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DrawRenderers(ref DrawRendererSettings settings);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DrawShadows(ref DrawShadowsSettings settings);
        /// <summary>
        /// <para>Execute a custom graphics command buffer.</para>
        /// </summary>
        /// <param name="commandBuffer">Command buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ExecuteCommandBuffer(CommandBuffer commandBuffer);
        /// <summary>
        /// <para>Setup camera specific global shader variables.</para>
        /// </summary>
        /// <param name="camera">Camera to setup shader variables for.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetupCameraProperties(Camera camera);
        /// <summary>
        /// <para>Draw skybox.</para>
        /// </summary>
        /// <param name="camera">Camera to draw the skybox for.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DrawSkybox(Camera camera);
        [RequiredByNativeCode]
        private static bool PrepareRenderLoop_Internal(Camera[] cameras, IntPtr loopPtr)
        {
            if (renderLoopDelegate == null)
            {
                return false;
            }
            RenderLoop outputLoop = new RenderLoop {
                m_Ptr = loopPtr
            };
            return renderLoopDelegate(cameras, outputLoop);
        }
        /// <summary>
        /// <para>Custom render loop delegate.</para>
        /// </summary>
        /// <param name="cameras"></param>
        /// <param name="outputLoop"></param>
        public delegate bool PrepareRenderLoop(Camera[] cameras, RenderLoop outputLoop);
    }
}

