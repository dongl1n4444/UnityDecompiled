namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Settings for RenderLoop.DrawRenderers.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DrawRendererSettings
    {
        /// <summary>
        /// <para>How to sort objects during rendering.</para>
        /// </summary>
        public DrawRendererSortSettings sorting;
        /// <summary>
        /// <para>Which shader pass to use.</para>
        /// </summary>
        public ShaderPassName shaderPassName;
        /// <summary>
        /// <para>Which subset of visible objects to render.</para>
        /// </summary>
        public InputFilter inputFilter;
        /// <summary>
        /// <para>What kind of per-object data to setup during rendering.</para>
        /// </summary>
        public RendererConfiguration rendererConfiguration;
        /// <summary>
        /// <para>Other flags controlling object rendering.</para>
        /// </summary>
        public DrawRendererFlags flags;
        private IntPtr _cullResults;
        /// <summary>
        /// <para>Create a draw settings struct.</para>
        /// </summary>
        /// <param name="cullResults">Culling results to use.</param>
        /// <param name="camera">Camera to use. Camera's transparency sort mode is used to determine whether to use orthographic or distance based sorting.</param>
        /// <param name="shaderPassName">Shader pass to use.</param>
        public DrawRendererSettings(CullResults cullResults, Camera camera, ShaderPassName shaderPassName)
        {
            this._cullResults = cullResults.cullResults;
            this.shaderPassName = shaderPassName;
            this.rendererConfiguration = RendererConfiguration.None;
            this.flags = DrawRendererFlags.EnableInstancing;
            this.inputFilter = InputFilter.Default();
            InitializeSortSettings(camera, out this.sorting);
        }

        /// <summary>
        /// <para>Culling results to use.</para>
        /// </summary>
        public CullResults cullResults
        {
            set
            {
                this._cullResults = value.cullResults;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void InitializeSortSettings(Camera camera, out DrawRendererSortSettings sortSettings);
    }
}

