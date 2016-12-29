namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Script interface for.</para>
    /// </summary>
    public sealed class Light : Behaviour
    {
        /// <summary>
        /// <para>Add a command buffer to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddCommandBuffer(LightEvent evt, CommandBuffer buffer);
        /// <summary>
        /// <para>Get command buffers to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <returns>
        /// <para>Array of command buffers.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern CommandBuffer[] GetCommandBuffers(LightEvent evt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Light[] GetLights(LightType type, int layer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_areaSize(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_areaSize(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_color(ref Color value);
        /// <summary>
        /// <para>Remove all command buffers set on this light.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveAllCommandBuffers();
        /// <summary>
        /// <para>Remove command buffer from execution at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveCommandBuffer(LightEvent evt, CommandBuffer buffer);
        /// <summary>
        /// <para>Remove command buffers from execution at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveCommandBuffers(LightEvent evt);

        [Obsolete("Use Light.bakedIndex or Light.isBaked instead.")]
        public bool alreadyLightmapped { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The size of the area light. Editor only.</para>
        /// </summary>
        public Vector2 areaSize
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_areaSize(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_areaSize(ref value);
            }
        }

        [Obsolete("light.attenuate was removed; all lights always attenuate now", true)]
        public bool attenuate
        {
            get => 
                true;
            set
            {
            }
        }

        /// <summary>
        /// <para>A unique index, used internally for identifying lights contributing to lightmaps and/or light probes.</para>
        /// </summary>
        public int bakedIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The multiplier that defines the strength of the bounce lighting.</para>
        /// </summary>
        public float bounceIntensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The color of the light.</para>
        /// </summary>
        public Color color
        {
            get
            {
                Color color;
                this.INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_color(ref value);
            }
        }

        /// <summary>
        /// <para>Number of command buffers set up on this light (Read Only).</para>
        /// </summary>
        public int commandBufferCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The cookie texture projected by the light.</para>
        /// </summary>
        public Texture cookie { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The size of a directional light's cookie.</para>
        /// </summary>
        public float cookieSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>This is used to light certain objects in the scene selectively.</para>
        /// </summary>
        public int cullingMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The to use for this light.</para>
        /// </summary>
        public Flare flare { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The Intensity of a light is multiplied with the Light color.</para>
        /// </summary>
        public float intensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is the light contribution already stored in lightmaps and/or lightprobes (Read Only).</para>
        /// </summary>
        public bool isBaked { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>This property controls whether this light only affects lightmap baking, or dynamic objects, or both.</para>
        /// </summary>
        public LightmappingMode lightmappingMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("Use QualitySettings.pixelLightCount instead.")]
        public static int pixelLightCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The range of the light.</para>
        /// </summary>
        public float range { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How to render the light.</para>
        /// </summary>
        public LightRenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Shadow mapping constant bias.</para>
        /// </summary>
        public float shadowBias { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("light.shadowConstantBias was removed, use light.shadowBias", true)]
        public float shadowConstantBias
        {
            get => 
                0f;
            set
            {
            }
        }

        /// <summary>
        /// <para>The custom resolution of the shadow map.</para>
        /// </summary>
        public int shadowCustomResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Near plane value to use for shadow frustums.</para>
        /// </summary>
        public float shadowNearPlane { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Shadow mapping normal-based bias.</para>
        /// </summary>
        public float shadowNormalBias { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("light.shadowObjectSizeBias was removed, use light.shadowBias", true)]
        public float shadowObjectSizeBias
        {
            get => 
                0f;
            set
            {
            }
        }

        /// <summary>
        /// <para>The resolution of the shadow map.</para>
        /// </summary>
        public LightShadowResolution shadowResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How this light casts shadows</para>
        /// </summary>
        public LightShadows shadows { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("Shadow softness is removed in Unity 5.0+")]
        public float shadowSoftness { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("Shadow softness is removed in Unity 5.0+")]
        public float shadowSoftnessFade { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Strength of light's shadows.</para>
        /// </summary>
        public float shadowStrength { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The angle of the light's spotlight cone in degrees.</para>
        /// </summary>
        public float spotAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The type of the light.</para>
        /// </summary>
        public LightType type { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

