namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Custom Render Textures are an extension to Render Textures, enabling you to render directly to the Texture using a Shader.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class CustomRenderTexture : RenderTexture
    {
        /// <summary>
        /// <para>Create a new Custom Render Texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="readWrite"></param>
        public CustomRenderTexture(int width, int height) : base(width, height, 0)
        {
        }

        /// <summary>
        /// <para>Create a new Custom Render Texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="readWrite"></param>
        public CustomRenderTexture(int width, int height, RenderTextureFormat format) : base(width, height, 0, format)
        {
        }

        /// <summary>
        /// <para>Create a new Custom Render Texture.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="readWrite"></param>
        public CustomRenderTexture(int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite) : base(width, height, 0, format, readWrite)
        {
        }

        /// <summary>
        /// <para>Clear all Update Zones.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearUpdateZones();
        private void EnableCubemapFace(CubemapFace face, bool value)
        {
            uint cubemapFaceMask = this.cubemapFaceMask;
            uint num2 = ((uint) 1) << face;
            if (value)
            {
                cubemapFaceMask |= num2;
            }
            else
            {
                cubemapFaceMask &= ~num2;
            }
            this.cubemapFaceMask = cubemapFaceMask;
        }

        public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
        {
            this.GetUpdateZonesInternal(updateZones);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void GetUpdateZonesInternal(object updateZones);
        /// <summary>
        /// <para>Triggers an initialization of the Custom Render Texture.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Initialize();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_initializationColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_initializationColor(ref Color value);
        private bool IsCubemapFaceEnabled(CubemapFace face) => 
            ((this.cubemapFaceMask & (((int) 1) << face)) != 0L);

        /// <summary>
        /// <para>Setup the list of Update Zones for the Custom Render Texture.</para>
        /// </summary>
        /// <param name="updateZones"></param>
        public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
        {
            if (updateZones == null)
            {
                throw new ArgumentNullException("updateZones");
            }
            this.SetUpdateZonesInternal(updateZones);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones);
        [ExcludeFromDocs]
        public void Update()
        {
            int count = 1;
            this.Update(count);
        }

        /// <summary>
        /// <para>Triggers the update of the Custom Render Texture.</para>
        /// </summary>
        /// <param name="count">Number of upate pass to perform.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Update([DefaultValue("1")] int count);

        /// <summary>
        /// <para>Bitfield that allows to enable or disable update on each of the cubemap faces. Order from least significant bit is +X, -X, +Y, -Y, +Z, -Z.</para>
        /// </summary>
        public uint cubemapFaceMask { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If true, the Custom Render Texture is double buffered so that you can access it during its own update. otherwise the Custom Render Texture will be not be double buffered.</para>
        /// </summary>
        public bool doubleBuffered { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Color with which the Custom Render Texture is initialized. This parameter will be ignored if an initializationMaterial is set.</para>
        /// </summary>
        public Color initializationColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_initializationColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_initializationColor(ref value);
            }
        }

        /// <summary>
        /// <para>Material with which the Custom Render Texture is initialized. Initialization texture and color are ignored if this parameter is set.</para>
        /// </summary>
        public Material initializationMaterial { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specify how the texture should be initialized.</para>
        /// </summary>
        public CustomRenderTextureUpdateMode initializationMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specify if the texture should be initialized with a Texture and a Color or a Material.</para>
        /// </summary>
        public CustomRenderTextureInitializationSource initializationSource { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture with which the Custom Render Texture is initialized (multiplied by the initialization color). This parameter will be ignored if an initializationMaterial is set.</para>
        /// </summary>
        public Texture initializationTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Material with which the content of the Custom Render Texture is updated.</para>
        /// </summary>
        public Material material { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shader Pass used to update the Custom Render Texture.</para>
        /// </summary>
        public int shaderPass { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specify how the texture should be updated.</para>
        /// </summary>
        public CustomRenderTextureUpdateMode updateMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Space in which the update zones are expressed (Normalized or Pixel space).</para>
        /// </summary>
        public CustomRenderTextureUpdateZoneSpace updateZoneSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If true, Update zones will wrap around the border of the Custom Render Texture. Otherwise, Update zones will be clamped at the border of the Custom Render Texture.</para>
        /// </summary>
        public bool wrapUpdateZones { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

