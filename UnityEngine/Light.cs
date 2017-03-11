namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Script interface for.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class Light : Behaviour
    {
        private int m_BakedIndex;

        /// <summary>
        /// <para>Add a command buffer to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        /// <param name="shadowPassMask">A mask specifying which shadow passes to execute the buffer for.</param>
        public void AddCommandBuffer(LightEvent evt, CommandBuffer buffer)
        {
            this.AddCommandBuffer(evt, buffer, ShadowMapPass.All);
        }

        /// <summary>
        /// <para>Add a command buffer to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        /// <param name="shadowPassMask">A mask specifying which shadow passes to execute the buffer for.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void AddCommandBuffer(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask);
        /// <summary>
        /// <para>Get command buffers to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <returns>
        /// <para>Array of command buffers.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern CommandBuffer[] GetCommandBuffers(LightEvent evt);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Light[] GetLights(LightType type, int layer);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_areaSize(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_areaSize(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_color(ref Color value);
        /// <summary>
        /// <para>Remove all command buffers set on this light.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RemoveAllCommandBuffers();
        /// <summary>
        /// <para>Remove command buffer from execution at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RemoveCommandBuffer(LightEvent evt, CommandBuffer buffer);
        /// <summary>
        /// <para>Remove command buffers from execution at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RemoveCommandBuffers(LightEvent evt);

        public bool alreadyLightmapped { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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

        [Obsolete("bakedIndex has been removed please use isBaked instead.")]
        public int bakedIndex
        {
            get => 
                this.m_BakedIndex;
            set
            {
                this.m_BakedIndex = value;
            }
        }

        /// <summary>
        /// <para>The multiplier that defines the strength of the bounce lighting.</para>
        /// </summary>
        public float bounceIntensity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        /// <para>
        /// The color temperature of the light.
        /// Correlated Color Temperature (abbreviated as CCT) is multiplied with the color filter when calculating the final color of a light source. The color temperature of the electromagnetic radiation emitted from an ideal black body is defined as its surface temperature in Kelvin. White is 6500K according to the D65 standard. Candle light is 1800K.
        /// If you want to use lightsUseCCT, lightsUseLinearIntensity has to be enabled to ensure physically correct output.
        /// See Also: GraphicsSettings.lightsUseLinearIntensity, GraphicsSettings.lightsUseCCT.
        /// </para>
        /// </summary>
        public float colorTemperature { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Number of command buffers set up on this light (Read Only).</para>
        /// </summary>
        public int commandBufferCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The cookie texture projected by the light.</para>
        /// </summary>
        public Texture cookie { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The size of a directional light's cookie.</para>
        /// </summary>
        public float cookieSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>This is used to light certain objects in the scene selectively.</para>
        /// </summary>
        public int cullingMask { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The to use for this light.</para>
        /// </summary>
        public Flare flare { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The Intensity of a light is multiplied with the Light color.</para>
        /// </summary>
        public float intensity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the light contribution already stored in lightmaps and/or lightprobes (Read Only).</para>
        /// </summary>
        public bool isBaked { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>This property describes what part of a light's contribution can be baked.</para>
        /// </summary>
        public LightmapBakeType lightmapBakeType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Light.lightmappingMode has been deprecated. Use Light.lightmapBakeType instead (UnityUpgradable) -> lightmapBakeType", true)]
        public LightmappingMode lightmappingMode
        {
            get => 
                LightmappingMode.Realtime;
            set
            {
            }
        }

        [Obsolete("Use QualitySettings.pixelLightCount instead.")]
        public static int pixelLightCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The range of the light.</para>
        /// </summary>
        public float range { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How to render the light.</para>
        /// </summary>
        public LightRenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shadow mapping constant bias.</para>
        /// </summary>
        public float shadowBias { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        public int shadowCustomResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Near plane value to use for shadow frustums.</para>
        /// </summary>
        public float shadowNearPlane { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shadow mapping normal-based bias.</para>
        /// </summary>
        public float shadowNormalBias { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        public LightShadowResolution shadowResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How this light casts shadows</para>
        /// </summary>
        public LightShadows shadows { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Shadow softness is removed in Unity 5.0+")]
        public float shadowSoftness { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Shadow softness is removed in Unity 5.0+")]
        public float shadowSoftnessFade { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Strength of light's shadows.</para>
        /// </summary>
        public float shadowStrength { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The angle of the light's spotlight cone in degrees.</para>
        /// </summary>
        public float spotAngle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The type of the light.</para>
        /// </summary>
        public LightType type { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

