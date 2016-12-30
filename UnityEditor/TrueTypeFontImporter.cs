namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>AssetImporter for importing Fonts.</para>
    /// </summary>
    public sealed class TrueTypeFontImporter : AssetImporter
    {
        /// <summary>
        /// <para>Create an editable copy of the font asset at path.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Font GenerateEditableFont(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool IsFormatSupported();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern Font[] LookupFallbackFontReferences(string[] _names);

        /// <summary>
        /// <para>Calculation mode for determining font's ascent.</para>
        /// </summary>
        public AscentCalculationMode ascentCalculationMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Border pixels added to character images for padding. This is useful if you want to render text using a shader which needs to render outside of the character area (like an outline shader).</para>
        /// </summary>
        public int characterPadding { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Spacing between character images in the generated texture in pixels. This is useful if you want to render text using a shader which samples pixels outside of the character area (like an outline shader).</para>
        /// </summary>
        public int characterSpacing { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A custom set of characters to be included in the Font Texture.</para>
        /// </summary>
        public string customCharacters { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>An array of font names, to be used when includeFontData is set to false.</para>
        /// </summary>
        public string[] fontNames { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>References to other fonts to be used looking for fallbacks.</para>
        /// </summary>
        public Font[] fontReferences { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Font rendering mode to use for this font.</para>
        /// </summary>
        public FontRenderingMode fontRenderingMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("FontRenderModes are no longer supported.", true)]
        private int fontRenderMode
        {
            get => 
                0;
            set
            {
            }
        }

        /// <summary>
        /// <para>Font size to use for importing the characters.</para>
        /// </summary>
        public int fontSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Use this to adjust which characters should be imported.</para>
        /// </summary>
        public FontTextureCase fontTextureCase { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The internal font name of the TTF file.</para>
        /// </summary>
        public string fontTTFName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>If this is enabled, the actual font will be embedded into the asset for Dynamic fonts.</para>
        /// </summary>
        public bool includeFontData { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Per-Font styles are no longer supported. Set the style in the rendering component, or import a styled version of the font.", true)]
        private FontStyle style
        {
            get => 
                FontStyle.Normal;
            set
            {
            }
        }

        [Obsolete("use2xBehaviour is deprecated. Use ascentCalculationMode instead")]
        private bool use2xBehaviour
        {
            get => 
                (this.ascentCalculationMode == AscentCalculationMode.Legacy2x);
            set
            {
                if (value)
                {
                    this.ascentCalculationMode = AscentCalculationMode.Legacy2x;
                }
                else if (this.ascentCalculationMode == AscentCalculationMode.Legacy2x)
                {
                    this.ascentCalculationMode = AscentCalculationMode.FaceAscender;
                }
            }
        }
    }
}

