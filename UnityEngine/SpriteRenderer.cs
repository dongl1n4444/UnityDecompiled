namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Renders a Sprite for 2D graphics.</para>
    /// </summary>
    public sealed class SpriteRenderer : Renderer
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Sprite GetSprite_INTERNAL();
        internal Bounds GetSpriteBounds()
        {
            Bounds bounds;
            INTERNAL_CALL_GetSpriteBounds(this, out bounds);
            return bounds;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetSpriteBounds(SpriteRenderer self, out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetSprite_INTERNAL(Sprite sprite);

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
        /// <para>Flips the sprite on the X axis.</para>
        /// </summary>
        public bool flipX { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Flips the sprite on the Y axis.</para>
        /// </summary>
        public bool flipY { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The Sprite to render.</para>
        /// </summary>
        public Sprite sprite
        {
            get
            {
                return this.GetSprite_INTERNAL();
            }
            set
            {
                this.SetSprite_INTERNAL(value);
            }
        }
    }
}

