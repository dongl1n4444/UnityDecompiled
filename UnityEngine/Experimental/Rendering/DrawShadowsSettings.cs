﻿namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Settings for RenderLoop.DrawShadows.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct DrawShadowsSettings
    {
        private IntPtr _cullResults;
        /// <summary>
        /// <para>The index of the shadow-casting light to be rendered.</para>
        /// </summary>
        public int lightIndex;
        /// <summary>
        /// <para>The split data.</para>
        /// </summary>
        public ShadowSplitData splitData;
        /// <summary>
        /// <para>Create a shadow settings object.</para>
        /// </summary>
        /// <param name="cullResults">The cull results for this light.</param>
        /// <param name="lightIndex">The light index.</param>
        public DrawShadowsSettings(CullResults cullResults, int lightIndex)
        {
            this._cullResults = cullResults.cullResults;
            this.lightIndex = lightIndex;
            this.splitData.cullingPlaneCount = 0;
            this.splitData.cullingSphere = Vector4.zero;
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
    }
}

