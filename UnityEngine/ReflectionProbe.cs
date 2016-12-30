namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The reflection probe is used to capture the surroundings into a texture which is passed to the shaders and used for reflections.</para>
    /// </summary>
    public sealed class ReflectionProbe : Behaviour
    {
        /// <summary>
        /// <para>Utility method to blend 2 cubemaps into a target render texture.</para>
        /// </summary>
        /// <param name="src">Cubemap to blend from.</param>
        /// <param name="dst">Cubemap to blend to.</param>
        /// <param name="blend">Blend weight.</param>
        /// <param name="target">RenderTexture which will hold the result of the blend.</param>
        /// <returns>
        /// <para>Returns trues if cubemaps were blended, false otherwise.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool BlendCubemap(Texture src, Texture dst, float blend, RenderTexture target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_backgroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_defaultTextureHDRDecodeValues(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_size(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_textureHDRDecodeValues(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_backgroundColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_center(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_size(ref Vector3 value);
        /// <summary>
        /// <para>Checks if a probe has finished a time-sliced render.</para>
        /// </summary>
        /// <param name="renderId">An integer representing the RenderID as returned by the RenderProbe method.</param>
        /// <returns>
        /// <para>
        /// True if the render has finished, false otherwise.
        /// 
        /// See Also: timeSlicingMode
        /// </para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsFinishedRendering(int renderId);
        [ExcludeFromDocs]
        public int RenderProbe()
        {
            RenderTexture targetTexture = null;
            return this.RenderProbe(targetTexture);
        }

        /// <summary>
        /// <para>Refreshes the probe's cubemap.</para>
        /// </summary>
        /// <param name="targetTexture">Target RendeTexture in which rendering should be done. Specifying null will update the probe's default texture.</param>
        /// <returns>
        /// <para>
        /// An integer representing a RenderID which can subsequently be used to check if the probe has finished rendering while rendering in time-slice mode.
        /// 
        /// See Also: IsFinishedRendering
        /// See Also: timeSlicingMode
        /// </para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int RenderProbe([DefaultValue("null")] RenderTexture targetTexture);

        /// <summary>
        /// <para>The color with which the texture of reflection probe will be cleared.</para>
        /// </summary>
        public Color backgroundColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_backgroundColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_backgroundColor(ref value);
            }
        }

        /// <summary>
        /// <para>Reference to the baked texture of the reflection probe's surrounding.</para>
        /// </summary>
        public Texture bakedTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Distance around probe used for blending (used in deferred probes).</para>
        /// </summary>
        public float blendDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The bounding volume of the reflection probe (Read Only).</para>
        /// </summary>
        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        /// <summary>
        /// <para>Should this reflection probe use box projection?</para>
        /// </summary>
        public bool boxProjection { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The center of the box area in which reflections will be applied to the objects. Measured in the probes's local space.</para>
        /// </summary>
        public Vector3 center
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_center(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_center(ref value);
            }
        }

        /// <summary>
        /// <para>How the reflection probe clears the background.</para>
        /// </summary>
        public ReflectionProbeClearFlags clearFlags { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>This is used to render parts of the reflecion probe's surrounding selectively.</para>
        /// </summary>
        public int cullingMask { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Reference to the baked texture of the reflection probe's surrounding. Use this to assign custom reflection texture.</para>
        /// </summary>
        public Texture customBakedTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture which is used outside of all reflection probes (Read Only).</para>
        /// </summary>
        public static Texture defaultTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>HDR decode values of the default reflection probe texture.</para>
        /// </summary>
        public static Vector4 defaultTextureHDRDecodeValues
        {
            get
            {
                Vector4 vector;
                INTERNAL_get_defaultTextureHDRDecodeValues(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>The far clipping plane distance when rendering the probe.</para>
        /// </summary>
        public float farClipPlane { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should this reflection probe use HDR rendering?</para>
        /// </summary>
        public bool hdr { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Reflection probe importance.</para>
        /// </summary>
        public int importance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The intensity modifier that is applied to the texture of reflection probe in the shader.</para>
        /// </summary>
        public float intensity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static int maxBakedCubemapResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static int minBakedCubemapResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Should reflection probe texture be generated in the Editor (ReflectionProbeMode.Baked) or should probe use custom specified texure (ReflectionProbeMode.Custom)?</para>
        /// </summary>
        public ReflectionProbeMode mode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The near clipping plane distance when rendering the probe.</para>
        /// </summary>
        public float nearClipPlane { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sets the way the probe will refresh.
        /// 
        /// See Also: ReflectionProbeRefreshMode.</para>
        /// </summary>
        public ReflectionProbeRefreshMode refreshMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Resolution of the underlying reflection texture in pixels.</para>
        /// </summary>
        public int resolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shadow drawing distance when rendering the probe.</para>
        /// </summary>
        public float shadowDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The size of the box area in which reflections will be applied to the objects. Measured in the probes's local space.</para>
        /// </summary>
        public Vector3 size
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_size(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_size(ref value);
            }
        }

        /// <summary>
        /// <para>Texture which is passed to the shader of the objects in the vicinity of the reflection probe (Read Only).</para>
        /// </summary>
        public Texture texture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>HDR decode values of the reflection probe texture.</para>
        /// </summary>
        public Vector4 textureHDRDecodeValues
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_textureHDRDecodeValues(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Sets this probe time-slicing mode
        /// 
        /// See Also: ReflectionProbeTimeSlicingMode.</para>
        /// </summary>
        public ReflectionProbeTimeSlicingMode timeSlicingMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("type property has been deprecated. Starting with Unity 5.4, the only supported reflection probe type is Cube.", true)]
        public ReflectionProbeType type { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

