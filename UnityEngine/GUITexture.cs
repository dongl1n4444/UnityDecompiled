namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A texture image used in a 2D GUI.</para>
    /// </summary>
    public sealed class GUITexture : GUIElement
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_pixelInset(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_pixelInset(ref Rect value);

        /// <summary>
        /// <para>The border defines the number of pixels from the edge that are not affected by scale.</para>
        /// </summary>
        public RectOffset border { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The color of the GUI texture.</para>
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
        /// <para>Pixel inset used for pixel adjustments for size and position.</para>
        /// </summary>
        public Rect pixelInset
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_pixelInset(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_pixelInset(ref value);
            }
        }

        /// <summary>
        /// <para>The texture used for drawing.</para>
        /// </summary>
        public Texture texture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

