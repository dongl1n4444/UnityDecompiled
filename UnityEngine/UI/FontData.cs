namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Struct for storing Text generation settings.</para>
    /// </summary>
    [Serializable]
    public class FontData : ISerializationCallbackReceiver
    {
        [SerializeField]
        private bool m_AlignByGeometry;
        [SerializeField, FormerlySerializedAs("alignment")]
        private TextAnchor m_Alignment;
        [SerializeField]
        private bool m_BestFit;
        [SerializeField, FormerlySerializedAs("font")]
        private Font m_Font;
        [SerializeField, FormerlySerializedAs("fontSize")]
        private int m_FontSize;
        [SerializeField, FormerlySerializedAs("fontStyle")]
        private FontStyle m_FontStyle;
        [SerializeField]
        private HorizontalWrapMode m_HorizontalOverflow;
        [SerializeField]
        private float m_LineSpacing;
        [SerializeField]
        private int m_MaxSize;
        [SerializeField]
        private int m_MinSize;
        [SerializeField, FormerlySerializedAs("richText")]
        private bool m_RichText;
        [SerializeField]
        private VerticalWrapMode m_VerticalOverflow;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.m_FontSize = Mathf.Clamp(this.m_FontSize, 0, 300);
            this.m_MinSize = Mathf.Clamp(this.m_MinSize, 0, this.m_FontSize);
            this.m_MaxSize = Mathf.Clamp(this.m_MaxSize, this.m_FontSize, 300);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        /// <summary>
        /// <para>Use the extents of glyph geometry to perform horizontal alignment rather than glyph metrics.</para>
        /// </summary>
        public bool alignByGeometry
        {
            get
            {
                return this.m_AlignByGeometry;
            }
            set
            {
                this.m_AlignByGeometry = value;
            }
        }

        /// <summary>
        /// <para>How is the text aligned.</para>
        /// </summary>
        public TextAnchor alignment
        {
            get
            {
                return this.m_Alignment;
            }
            set
            {
                this.m_Alignment = value;
            }
        }

        /// <summary>
        /// <para>Is best fit used.</para>
        /// </summary>
        public bool bestFit
        {
            get
            {
                return this.m_BestFit;
            }
            set
            {
                this.m_BestFit = value;
            }
        }

        /// <summary>
        /// <para>Get a font data with sensible defaults.</para>
        /// </summary>
        public static FontData defaultFontData
        {
            get
            {
                return new FontData { 
                    m_FontSize = 14,
                    m_LineSpacing = 1f,
                    m_FontStyle = FontStyle.Normal,
                    m_BestFit = false,
                    m_MinSize = 10,
                    m_MaxSize = 40,
                    m_Alignment = TextAnchor.UpperLeft,
                    m_HorizontalOverflow = HorizontalWrapMode.Wrap,
                    m_VerticalOverflow = VerticalWrapMode.Truncate,
                    m_RichText = true,
                    m_AlignByGeometry = false
                };
            }
        }

        /// <summary>
        /// <para>Font to use.</para>
        /// </summary>
        public Font font
        {
            get
            {
                return this.m_Font;
            }
            set
            {
                this.m_Font = value;
            }
        }

        /// <summary>
        /// <para>Font size.</para>
        /// </summary>
        public int fontSize
        {
            get
            {
                return this.m_FontSize;
            }
            set
            {
                this.m_FontSize = value;
            }
        }

        /// <summary>
        /// <para>Font Style.</para>
        /// </summary>
        public FontStyle fontStyle
        {
            get
            {
                return this.m_FontStyle;
            }
            set
            {
                this.m_FontStyle = value;
            }
        }

        /// <summary>
        /// <para>Horizontal overflow mode.</para>
        /// </summary>
        public HorizontalWrapMode horizontalOverflow
        {
            get
            {
                return this.m_HorizontalOverflow;
            }
            set
            {
                this.m_HorizontalOverflow = value;
            }
        }

        /// <summary>
        /// <para>Line spacing.</para>
        /// </summary>
        public float lineSpacing
        {
            get
            {
                return this.m_LineSpacing;
            }
            set
            {
                this.m_LineSpacing = value;
            }
        }

        /// <summary>
        /// <para>Maximum text size.</para>
        /// </summary>
        public int maxSize
        {
            get
            {
                return this.m_MaxSize;
            }
            set
            {
                this.m_MaxSize = value;
            }
        }

        /// <summary>
        /// <para>Minimum text size.</para>
        /// </summary>
        public int minSize
        {
            get
            {
                return this.m_MinSize;
            }
            set
            {
                this.m_MinSize = value;
            }
        }

        /// <summary>
        /// <para>Should RichText be used?</para>
        /// </summary>
        public bool richText
        {
            get
            {
                return this.m_RichText;
            }
            set
            {
                this.m_RichText = value;
            }
        }

        /// <summary>
        /// <para>Vertical overflow mode.</para>
        /// </summary>
        public VerticalWrapMode verticalOverflow
        {
            get
            {
                return this.m_VerticalOverflow;
            }
            set
            {
                this.m_VerticalOverflow = value;
            }
        }
    }
}

