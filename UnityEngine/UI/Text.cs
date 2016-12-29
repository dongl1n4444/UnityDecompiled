namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>The default Graphic to draw font data to screen.</para>
    /// </summary>
    [AddComponentMenu("UI/Text", 10)]
    public class Text : MaskableGraphic, ILayoutElement
    {
        [NonSerialized]
        protected bool m_DisableFontTextureRebuiltCallback = false;
        [SerializeField]
        private FontData m_FontData = FontData.defaultFontData;
        private Font m_LastTrackedFont;
        private readonly UIVertex[] m_TempVerts = new UIVertex[4];
        [TextArea(3, 10), SerializeField]
        protected string m_Text = string.Empty;
        private TextGenerator m_TextCache;
        private TextGenerator m_TextCacheForLayout;
        protected static Material s_DefaultText = null;

        protected Text()
        {
            base.useLegacyMeshGeneration = false;
        }

        internal void AssignDefaultFont()
        {
            this.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual void CalculateLayoutInputVertical()
        {
        }

        /// <summary>
        /// <para>Called by the [FontUpdateTracker] when the texture associated with a font is modified.</para>
        /// </summary>
        public void FontTextureChanged()
        {
            if ((this != null) && !this.m_DisableFontTextureRebuiltCallback)
            {
                this.cachedTextGenerator.Invalidate();
                if (this.IsActive())
                {
                    if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
                    {
                        this.UpdateGeometry();
                    }
                    else
                    {
                        this.SetAllDirty();
                    }
                }
            }
        }

        /// <summary>
        /// <para>Convenience function to populate the generation setting for the text.</para>
        /// </summary>
        /// <param name="extents">The extents the text can draw in.</param>
        /// <returns>
        /// <para>Generated settings.</para>
        /// </returns>
        public TextGenerationSettings GetGenerationSettings(Vector2 extents)
        {
            TextGenerationSettings settings = new TextGenerationSettings {
                generationExtents = extents
            };
            if ((this.font != null) && this.font.dynamic)
            {
                settings.fontSize = this.m_FontData.fontSize;
                settings.resizeTextMinSize = this.m_FontData.minSize;
                settings.resizeTextMaxSize = this.m_FontData.maxSize;
            }
            settings.textAnchor = this.m_FontData.alignment;
            settings.alignByGeometry = this.m_FontData.alignByGeometry;
            settings.scaleFactor = this.pixelsPerUnit;
            settings.color = this.color;
            settings.font = this.font;
            settings.pivot = base.rectTransform.pivot;
            settings.richText = this.m_FontData.richText;
            settings.lineSpacing = this.m_FontData.lineSpacing;
            settings.fontStyle = this.m_FontData.fontStyle;
            settings.resizeTextForBestFit = this.m_FontData.bestFit;
            settings.updateBounds = false;
            settings.horizontalOverflow = this.m_FontData.horizontalOverflow;
            settings.verticalOverflow = this.m_FontData.verticalOverflow;
            return settings;
        }

        /// <summary>
        /// <para>Convenience function to determine the vector offset of the anchor.</para>
        /// </summary>
        /// <param name="anchor"></param>
        public static Vector2 GetTextAnchorPivot(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    return new Vector2(0f, 1f);

                case TextAnchor.UpperCenter:
                    return new Vector2(0.5f, 1f);

                case TextAnchor.UpperRight:
                    return new Vector2(1f, 1f);

                case TextAnchor.MiddleLeft:
                    return new Vector2(0f, 0.5f);

                case TextAnchor.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);

                case TextAnchor.MiddleRight:
                    return new Vector2(1f, 0.5f);

                case TextAnchor.LowerLeft:
                    return new Vector2(0f, 0f);

                case TextAnchor.LowerCenter:
                    return new Vector2(0.5f, 0f);

                case TextAnchor.LowerRight:
                    return new Vector2(1f, 0f);
            }
            return Vector2.zero;
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            FontUpdateTracker.UntrackText(this);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.cachedTextGenerator.Invalidate();
            FontUpdateTracker.TrackText(this);
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (this.font != null)
            {
                this.m_DisableFontTextureRebuiltCallback = true;
                Vector2 size = base.rectTransform.rect.size;
                TextGenerationSettings generationSettings = this.GetGenerationSettings(size);
                this.cachedTextGenerator.PopulateWithErrors(this.text, generationSettings, base.gameObject);
                IList<UIVertex> verts = this.cachedTextGenerator.verts;
                float num = 1f / this.pixelsPerUnit;
                int num2 = verts.Count - 4;
                UIVertex vertex = verts[0];
                UIVertex vertex2 = verts[0];
                Vector2 point = (Vector2) (new Vector2(vertex.position.x, vertex2.position.y) * num);
                point = base.PixelAdjustPoint(point) - point;
                toFill.Clear();
                if (point != Vector2.zero)
                {
                    for (int i = 0; i < num2; i++)
                    {
                        int index = i & 3;
                        this.m_TempVerts[index] = verts[i];
                        this.m_TempVerts[index].position = (Vector3) (this.m_TempVerts[index].position * num);
                        this.m_TempVerts[index].position.x += point.x;
                        this.m_TempVerts[index].position.y += point.y;
                        if (index == 3)
                        {
                            toFill.AddUIVertexQuad(this.m_TempVerts);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < num2; j++)
                    {
                        int num6 = j & 3;
                        this.m_TempVerts[num6] = verts[j];
                        this.m_TempVerts[num6].position = (Vector3) (this.m_TempVerts[num6].position * num);
                        if (num6 == 3)
                        {
                            toFill.AddUIVertexQuad(this.m_TempVerts);
                        }
                    }
                }
                this.m_DisableFontTextureRebuiltCallback = false;
            }
        }

        public override void OnRebuildRequested()
        {
            FontUpdateTracker.UntrackText(this);
            FontUpdateTracker.TrackText(this);
            this.cachedTextGenerator.Invalidate();
            base.OnRebuildRequested();
        }

        protected override void OnValidate()
        {
            if (!this.IsActive())
            {
                base.OnValidate();
            }
            else
            {
                if (this.m_FontData.font != this.m_LastTrackedFont)
                {
                    Font font = this.m_FontData.font;
                    this.m_FontData.font = this.m_LastTrackedFont;
                    FontUpdateTracker.UntrackText(this);
                    this.m_FontData.font = font;
                    FontUpdateTracker.TrackText(this);
                    this.m_LastTrackedFont = font;
                }
                base.OnValidate();
            }
        }

        protected override void Reset()
        {
            this.AssignDefaultFont();
        }

        protected override void UpdateGeometry()
        {
            if (this.font != null)
            {
                base.UpdateGeometry();
            }
        }

        /// <summary>
        /// <para>Use the extents of glyph geometry to perform horizontal alignment rather than glyph metrics.</para>
        /// </summary>
        public bool alignByGeometry
        {
            get => 
                this.m_FontData.alignByGeometry;
            set
            {
                if (this.m_FontData.alignByGeometry != value)
                {
                    this.m_FontData.alignByGeometry = value;
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>The positioning of the text reliative to its RectTransform.</para>
        /// </summary>
        public TextAnchor alignment
        {
            get => 
                this.m_FontData.alignment;
            set
            {
                if (this.m_FontData.alignment != value)
                {
                    this.m_FontData.alignment = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>The cached TextGenerator used when generating visible Text.</para>
        /// </summary>
        public TextGenerator cachedTextGenerator
        {
            get
            {
                TextGenerator textCache;
                if (this.m_TextCache != null)
                {
                    textCache = this.m_TextCache;
                }
                else
                {
                    textCache = this.m_TextCache = (this.m_Text.Length == 0) ? new TextGenerator() : new TextGenerator(this.m_Text.Length);
                }
                return textCache;
            }
        }

        /// <summary>
        /// <para>The cached TextGenerator used when determine Layout.</para>
        /// </summary>
        public TextGenerator cachedTextGeneratorForLayout
        {
            get
            {
                TextGenerator textCacheForLayout;
                if (this.m_TextCacheForLayout != null)
                {
                    textCacheForLayout = this.m_TextCacheForLayout;
                }
                else
                {
                    textCacheForLayout = this.m_TextCacheForLayout = new TextGenerator();
                }
                return textCacheForLayout;
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float flexibleHeight =>
            -1f;

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float flexibleWidth =>
            -1f;

        /// <summary>
        /// <para>The Font used by the text.</para>
        /// </summary>
        public Font font
        {
            get => 
                this.m_FontData.font;
            set
            {
                if (this.m_FontData.font != value)
                {
                    FontUpdateTracker.UntrackText(this);
                    this.m_FontData.font = value;
                    FontUpdateTracker.TrackText(this);
                    this.m_LastTrackedFont = value;
                    this.SetAllDirty();
                }
            }
        }

        /// <summary>
        /// <para>The size that the Font should render at.</para>
        /// </summary>
        public int fontSize
        {
            get => 
                this.m_FontData.fontSize;
            set
            {
                if (this.m_FontData.fontSize != value)
                {
                    this.m_FontData.fontSize = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>FontStyle used by the text.</para>
        /// </summary>
        public FontStyle fontStyle
        {
            get => 
                this.m_FontData.fontStyle;
            set
            {
                if (this.m_FontData.fontStyle != value)
                {
                    this.m_FontData.fontStyle = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>Horizontal overflow mode.</para>
        /// </summary>
        public HorizontalWrapMode horizontalOverflow
        {
            get => 
                this.m_FontData.horizontalOverflow;
            set
            {
                if (this.m_FontData.horizontalOverflow != value)
                {
                    this.m_FontData.horizontalOverflow = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual int layoutPriority =>
            0;

        /// <summary>
        /// <para>Line spacing, specified as a factor of font line height. A value of 1 will produce normal line spacing.</para>
        /// </summary>
        public float lineSpacing
        {
            get => 
                this.m_FontData.lineSpacing;
            set
            {
                if (this.m_FontData.lineSpacing != value)
                {
                    this.m_FontData.lineSpacing = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>The Texture that comes from the Font.</para>
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (((this.font != null) && (this.font.material != null)) && (this.font.material.mainTexture != null))
                {
                    return this.font.material.mainTexture;
                }
                if (base.m_Material != null)
                {
                    return base.m_Material.mainTexture;
                }
                return base.mainTexture;
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float minHeight =>
            0f;

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float minWidth =>
            0f;

        /// <summary>
        /// <para>(Read Only) Provides information about how fonts are scale to the screen.</para>
        /// </summary>
        public float pixelsPerUnit
        {
            get
            {
                Canvas canvas = base.canvas;
                if (canvas == null)
                {
                    return 1f;
                }
                if ((this.font == null) || this.font.dynamic)
                {
                    return canvas.scaleFactor;
                }
                if ((this.m_FontData.fontSize <= 0) || (this.font.fontSize <= 0))
                {
                    return 1f;
                }
                return (((float) this.font.fontSize) / ((float) this.m_FontData.fontSize));
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float preferredHeight
        {
            get
            {
                TextGenerationSettings generationSettings = this.GetGenerationSettings(new Vector2(base.GetPixelAdjustedRect().size.x, 0f));
                return (this.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.pixelsPerUnit);
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float preferredWidth
        {
            get
            {
                TextGenerationSettings generationSettings = this.GetGenerationSettings(Vector2.zero);
                return (this.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.pixelsPerUnit);
            }
        }

        /// <summary>
        /// <para>Should the text be allowed to auto resized.</para>
        /// </summary>
        public bool resizeTextForBestFit
        {
            get => 
                this.m_FontData.bestFit;
            set
            {
                if (this.m_FontData.bestFit != value)
                {
                    this.m_FontData.bestFit = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>The maximum size the text is allowed to be. 1 = infinitly large.</para>
        /// </summary>
        public int resizeTextMaxSize
        {
            get => 
                this.m_FontData.maxSize;
            set
            {
                if (this.m_FontData.maxSize != value)
                {
                    this.m_FontData.maxSize = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>The minimum size the text is allowed to be.</para>
        /// </summary>
        public int resizeTextMinSize
        {
            get => 
                this.m_FontData.minSize;
            set
            {
                if (this.m_FontData.minSize != value)
                {
                    this.m_FontData.minSize = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>Whether this Text will support rich text.</para>
        /// </summary>
        public bool supportRichText
        {
            get => 
                this.m_FontData.richText;
            set
            {
                if (this.m_FontData.richText != value)
                {
                    this.m_FontData.richText = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>The string value this text will display.</para>
        /// </summary>
        public virtual string text
        {
            get => 
                this.m_Text;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (!string.IsNullOrEmpty(this.m_Text))
                    {
                        this.m_Text = "";
                        this.SetVerticesDirty();
                    }
                }
                else if (this.m_Text != value)
                {
                    this.m_Text = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// <para>Vertical overflow mode.</para>
        /// </summary>
        public VerticalWrapMode verticalOverflow
        {
            get => 
                this.m_FontData.verticalOverflow;
            set
            {
                if (this.m_FontData.verticalOverflow != value)
                {
                    this.m_FontData.verticalOverflow = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }
    }
}

