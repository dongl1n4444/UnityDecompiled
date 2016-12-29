namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Structure to store the state of a color transition on a Selectable.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ColorBlock : IEquatable<ColorBlock>
    {
        [FormerlySerializedAs("normalColor"), SerializeField]
        private Color m_NormalColor;
        [FormerlySerializedAs("highlightedColor"), FormerlySerializedAs("m_SelectedColor"), SerializeField]
        private Color m_HighlightedColor;
        [FormerlySerializedAs("pressedColor"), SerializeField]
        private Color m_PressedColor;
        [FormerlySerializedAs("disabledColor"), SerializeField]
        private Color m_DisabledColor;
        [Range(1f, 5f), SerializeField]
        private float m_ColorMultiplier;
        [FormerlySerializedAs("fadeDuration"), SerializeField]
        private float m_FadeDuration;
        /// <summary>
        /// <para>Normal Color.</para>
        /// </summary>
        public Color normalColor
        {
            get => 
                this.m_NormalColor;
            set
            {
                this.m_NormalColor = value;
            }
        }
        /// <summary>
        /// <para>Highlighted Color.</para>
        /// </summary>
        public Color highlightedColor
        {
            get => 
                this.m_HighlightedColor;
            set
            {
                this.m_HighlightedColor = value;
            }
        }
        /// <summary>
        /// <para>Pressed Color.</para>
        /// </summary>
        public Color pressedColor
        {
            get => 
                this.m_PressedColor;
            set
            {
                this.m_PressedColor = value;
            }
        }
        /// <summary>
        /// <para>Disabled Color.</para>
        /// </summary>
        public Color disabledColor
        {
            get => 
                this.m_DisabledColor;
            set
            {
                this.m_DisabledColor = value;
            }
        }
        /// <summary>
        /// <para>Multiplier applied to colors (allows brightening greater then base color).</para>
        /// </summary>
        public float colorMultiplier
        {
            get => 
                this.m_ColorMultiplier;
            set
            {
                this.m_ColorMultiplier = value;
            }
        }
        /// <summary>
        /// <para>How long a color transition should take.</para>
        /// </summary>
        public float fadeDuration
        {
            get => 
                this.m_FadeDuration;
            set
            {
                this.m_FadeDuration = value;
            }
        }
        /// <summary>
        /// <para>Simple getter for the default ColorBlock.</para>
        /// </summary>
        public static ColorBlock defaultColorBlock =>
            new ColorBlock { 
                m_NormalColor=(Color) new Color32(0xff, 0xff, 0xff, 0xff),
                m_HighlightedColor=(Color) new Color32(0xf5, 0xf5, 0xf5, 0xff),
                m_PressedColor=(Color) new Color32(200, 200, 200, 0xff),
                m_DisabledColor=(Color) new Color32(200, 200, 200, 0x80),
                colorMultiplier=1f,
                fadeDuration=0.1f
            };
        public override bool Equals(object obj) => 
            ((obj is ColorBlock) && this.Equals((ColorBlock) obj));

        public bool Equals(ColorBlock other) => 
            (((((this.normalColor == other.normalColor) && (this.highlightedColor == other.highlightedColor)) && ((this.pressedColor == other.pressedColor) && (this.disabledColor == other.disabledColor))) && (this.colorMultiplier == other.colorMultiplier)) && (this.fadeDuration == other.fadeDuration));

        public static bool operator ==(ColorBlock point1, ColorBlock point2) => 
            point1.Equals(point2);

        public static bool operator !=(ColorBlock point1, ColorBlock point2) => 
            !point1.Equals(point2);

        public override int GetHashCode() => 
            base.GetHashCode();
    }
}

