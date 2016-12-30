namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class for texture handling. Contains functionality that is common to both Texture2D and RenderTexture classes.</para>
    /// </summary>
    [UsedByNativeCode]
    public class Texture : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Obsolete("Use GetNativeTexturePtr instead.")]
        public extern int GetNativeTextureID();
        /// <summary>
        /// <para>Retrieve a native (underlying graphics API) pointer to the texture resource.</para>
        /// </summary>
        /// <returns>
        /// <para>Pointer to an underlying graphics API texture resource.</para>
        /// </returns>
        public IntPtr GetNativeTexturePtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeTexturePtr(this, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetNativeTexturePtr(Texture self, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_texelSize(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern TextureDimension Internal_GetDimension(Texture t);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetHeight(Texture t);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetWidth(Texture t);
        /// <summary>
        /// <para>Sets Anisotropic limits.</para>
        /// </summary>
        /// <param name="forcedMin"></param>
        /// <param name="globalMax"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax);

        /// <summary>
        /// <para>Anisotropic filtering level of the texture.</para>
        /// </summary>
        public int anisoLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static AnisotropicFiltering anisotropicFiltering { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Dimensionality (type) of the texture (Read Only).</para>
        /// </summary>
        public virtual TextureDimension dimension
        {
            get => 
                Internal_GetDimension(this);
            set
            {
                throw new Exception("not implemented");
            }
        }

        /// <summary>
        /// <para>Filtering mode of the texture.</para>
        /// </summary>
        public FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Height of the texture in pixels. (Read Only)</para>
        /// </summary>
        public virtual int height
        {
            get => 
                Internal_GetHeight(this);
            set
            {
                throw new Exception("not implemented");
            }
        }

        public static int masterTextureLimit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mip map bias of the texture.</para>
        /// </summary>
        public float mipMapBias { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public Vector2 texelSize
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_texelSize(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Width of the texture in pixels. (Read Only)</para>
        /// </summary>
        public virtual int width
        {
            get => 
                Internal_GetWidth(this);
            set
            {
                throw new Exception("not implemented");
            }
        }

        /// <summary>
        /// <para>Wrap mode (Repeat or Clamp) of the texture.</para>
        /// </summary>
        public TextureWrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

