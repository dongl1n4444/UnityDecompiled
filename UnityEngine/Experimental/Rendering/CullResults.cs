namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Culling results (visible objects, lights, reflection probes).</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct CullResults
    {
        /// <summary>
        /// <para>Array of visible lights.</para>
        /// </summary>
        public VisibleLight[] visibleLights;
        /// <summary>
        /// <para>Array of visible reflection probes.</para>
        /// </summary>
        public VisibleReflectionProbe[] visibleReflectionProbes;
        internal IntPtr cullResults;
        public static bool GetCullingParameters(Camera camera, out CullingParameters cullingParameters) => 
            GetCullingParameters_Internal(camera, out cullingParameters, sizeof(CullingParameters));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetCullingParameters_Internal(Camera camera, out CullingParameters cullingParameters, int managedCullingParametersSize);
        internal static void Internal_Cull(ref CullingParameters parameters, RenderLoop renderLoop, out CullResults results)
        {
            INTERNAL_CALL_Internal_Cull(ref parameters, ref renderLoop, out results);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_Cull(ref CullingParameters parameters, ref RenderLoop renderLoop, out CullResults results);
        public static CullResults Cull(ref CullingParameters parameters, RenderLoop renderLoop)
        {
            CullResults results;
            Internal_Cull(ref parameters, renderLoop, out results);
            return results;
        }

        public static bool Cull(Camera camera, RenderLoop renderLoop, out CullResults results)
        {
            CullingParameters parameters;
            results.cullResults = IntPtr.Zero;
            results.visibleLights = null;
            results.visibleReflectionProbes = null;
            if (!GetCullingParameters(camera, out parameters))
            {
                return false;
            }
            results = Cull(ref parameters, renderLoop);
            return true;
        }

        public bool GetShadowCasterBounds(int lightIndex, out Bounds outBounds) => 
            GetShadowCasterBounds(this.cullResults, lightIndex, out outBounds);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetShadowCasterBounds(IntPtr cullResults, int lightIndex, out Bounds bounds);
        /// <summary>
        /// <para>Gets the number of per-object light indices.</para>
        /// </summary>
        /// <returns>
        /// <para>The number of per-object light indices.</para>
        /// </returns>
        public int GetLightIndicesCount() => 
            GetLightIndicesCount(this.cullResults);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int GetLightIndicesCount(IntPtr cullResults);
        /// <summary>
        /// <para>Fills a compute buffer with per-object light indices.</para>
        /// </summary>
        /// <param name="computeBuffer">The compute buffer object to fill.</param>
        public void FillLightIndices(ComputeBuffer computeBuffer)
        {
            FillLightIndices(this.cullResults, computeBuffer);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void FillLightIndices(IntPtr cullResults, ComputeBuffer computeBuffer);
        public bool ComputeSpotShadowMatricesAndCullingPrimitives(int activeLightIndex, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData) => 
            ComputeSpotShadowMatricesAndCullingPrimitives(this.cullResults, activeLightIndex, out viewMatrix, out projMatrix, out shadowSplitData);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool ComputeSpotShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);
        public bool ComputePointShadowMatricesAndCullingPrimitives(int activeLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData) => 
            ComputePointShadowMatricesAndCullingPrimitives(this.cullResults, activeLightIndex, cubemapFace, fovBias, out viewMatrix, out projMatrix, out shadowSplitData);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool ComputePointShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);
        public bool ComputeDirectionalShadowMatricesAndCullingPrimitives(int activeLightIndex, int splitIndex, int splitCount, Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData) => 
            ComputeDirectionalShadowMatricesAndCullingPrimitives(this.cullResults, activeLightIndex, splitIndex, splitCount, splitRatio, shadowResolution, shadowNearPlaneOffset, out viewMatrix, out projMatrix, out shadowSplitData);

        private static bool ComputeDirectionalShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, int splitIndex, int splitCount, Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData) => 
            INTERNAL_CALL_ComputeDirectionalShadowMatricesAndCullingPrimitives(cullResults, activeLightIndex, splitIndex, splitCount, ref splitRatio, shadowResolution, shadowNearPlaneOffset, out viewMatrix, out projMatrix, out shadowSplitData);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_ComputeDirectionalShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, int splitIndex, int splitCount, ref Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);
    }
}

