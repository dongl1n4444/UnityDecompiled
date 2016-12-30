namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Renders a Sprite for 2D graphics.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class SpriteRenderer : Renderer
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Sprite GetSprite_INTERNAL();
        internal Bounds GetSpriteBounds()
        {
            Bounds bounds;
            INTERNAL_CALL_GetSpriteBounds(this, out bounds);
            return bounds;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSpriteBounds(SpriteRenderer self, out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_size(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_size(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetSprite_INTERNAL(Sprite sprite);

        /// <summary>
        /// <para>The current threshold for Sprite Renderer tiling.</para>
        /// </summary>
        public float adaptiveModeThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Rendering color for the Sprite graphic.</para>
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
        /// <para>The current draw mode of the Sprite Renderer.</para>
        /// </summary>
        public SpriteDrawMode drawMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Flips the sprite on the X axis.</para>
        /// </summary>
        public bool flipX { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Flips the sprite on the Y axis.</para>
        /// </summary>
        public bool flipY { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal bool shouldSupportTiling { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Property to set/get the size to render when the SpriteRenderer.drawMode is set to SpriteDrawMode.NineSlice.</para>
        /// </summary>
        public Vector2 size
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_size(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_size(ref value);
            }
        }

        /// <summary>
        /// <para>The Sprite to render.</para>
        /// </summary>
        public Sprite sprite
        {
            get => 
                this.GetSprite_INTERNAL();
            set
            {
                this.SetSprite_INTERNAL(value);
            }
        }

        /// <summary>
        /// <para>The current tile mode of the Sprite Renderer.</para>
        /// </summary>
        public SpriteTileMode tileMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

