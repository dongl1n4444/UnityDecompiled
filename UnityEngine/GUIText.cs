namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A text string displayed in a GUI.</para>
    /// </summary>
    public sealed class GUIText : GUIElement
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_SetPixelOffset(GUIText self, ref Vector2 p);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_GetPixelOffset(out Vector2 output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_color(ref Color value);
        private void Internal_SetPixelOffset(Vector2 p)
        {
            INTERNAL_CALL_Internal_SetPixelOffset(this, ref p);
        }

        /// <summary>
        /// <para>The alignment of the text.</para>
        /// </summary>
        public TextAlignment alignment { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The anchor of the text.</para>
        /// </summary>
        public TextAnchor anchor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The color used to render the text.</para>
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
        /// <para>The font used for the text.</para>
        /// </summary>
        public Font font { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The font size to use (for dynamic fonts).</para>
        /// </summary>
        public int fontSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The font style to use (for dynamic fonts).</para>
        /// </summary>
        public FontStyle fontStyle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The line spacing multiplier.</para>
        /// </summary>
        public float lineSpacing { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The Material to use for rendering.</para>
        /// </summary>
        public Material material { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The pixel offset of the text.</para>
        /// </summary>
        public Vector2 pixelOffset
        {
            get
            {
                Vector2 vector;
                this.Internal_GetPixelOffset(out vector);
                return vector;
            }
            set
            {
                this.Internal_SetPixelOffset(value);
            }
        }

        /// <summary>
        /// <para>Enable HTML-style tags for Text Formatting Markup.</para>
        /// </summary>
        public bool richText { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The tab width multiplier.</para>
        /// </summary>
        public float tabSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The text to display.</para>
        /// </summary>
        public string text { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

