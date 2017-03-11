namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Defines state and drawing commands used in a custom render pipelines.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ScriptableRenderContext
    {
        private IntPtr m_Ptr;
        internal void Initialize(IntPtr ptr)
        {
            this.m_Ptr = ptr;
        }

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
    }
}

