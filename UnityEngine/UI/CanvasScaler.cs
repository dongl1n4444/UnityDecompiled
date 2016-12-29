namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>The Canvas Scaler component is used for controlling the overall scale and pixel density of UI elements in the Canvas. This scaling affects everything under the Canvas, including font sizes and image borders.</para>
    /// </summary>
    [RequireComponent(typeof(Canvas)), ExecuteInEditMode, AddComponentMenu("Layout/Canvas Scaler", 0x65)]
    public class CanvasScaler : UIBehaviour
    {
        private const float kLogBase = 2f;
        private Canvas m_Canvas;
        [Tooltip("The pixels per inch to use for sprites that have a 'Pixels Per Unit' setting that matches the 'Reference Pixels Per Unit' setting."), SerializeField]
        protected float m_DefaultSpriteDPI = 96f;
        [Tooltip("The amount of pixels per unit to use for dynamically created bitmaps in the UI, such as Text."), SerializeField]
        protected float m_DynamicPixelsPerUnit = 1f;
        [Tooltip("The DPI to assume if the screen DPI is not known."), SerializeField]
        protected float m_FallbackScreenDPI = 96f;
        [Tooltip("Determines if the scaling is using the width or height as reference, or a mix in between."), Range(0f, 1f), SerializeField]
        protected float m_MatchWidthOrHeight = 0f;
        [Tooltip("The physical unit to specify positions and sizes in."), SerializeField]
        protected Unit m_PhysicalUnit = Unit.Points;
        [NonSerialized]
        private float m_PrevReferencePixelsPerUnit = 100f;
        [NonSerialized]
        private float m_PrevScaleFactor = 1f;
        [Tooltip("If a sprite has this 'Pixels Per Unit' setting, then one pixel in the sprite will cover one unit in the UI."), SerializeField]
        protected float m_ReferencePixelsPerUnit = 100f;
        [Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the UI will be scaled up, and if it's smaller, the UI will be scaled down. This is done in accordance with the Screen Match Mode."), SerializeField]
        protected Vector2 m_ReferenceResolution = new Vector2(800f, 600f);
        [Tooltip("Scales all UI elements in the Canvas by this factor."), SerializeField]
        protected float m_ScaleFactor = 1f;
        [Tooltip("A mode used to scale the canvas area if the aspect ratio of the current resolution doesn't fit the reference resolution."), SerializeField]
        protected ScreenMatchMode m_ScreenMatchMode = ScreenMatchMode.MatchWidthOrHeight;
        [Tooltip("Determines how UI elements in the Canvas are scaled."), SerializeField]
        private ScaleMode m_UiScaleMode = ScaleMode.ConstantPixelSize;

        protected CanvasScaler()
        {
        }

        /// <summary>
        /// <para>Method that handles calculations of canvas scaling.</para>
        /// </summary>
        protected virtual void Handle()
        {
            if ((this.m_Canvas != null) && this.m_Canvas.isRootCanvas)
            {
                if (this.m_Canvas.renderMode == RenderMode.WorldSpace)
                {
                    this.HandleWorldCanvas();
                }
                else
                {
                    switch (this.m_UiScaleMode)
                    {
                        case ScaleMode.ConstantPixelSize:
                            this.HandleConstantPixelSize();
                            break;

                        case ScaleMode.ScaleWithScreenSize:
                            this.HandleScaleWithScreenSize();
                            break;

                        case ScaleMode.ConstantPhysicalSize:
                            this.HandleConstantPhysicalSize();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// <para>Handles canvas scaling for a constant physical size.</para>
        /// </summary>
        protected virtual void HandleConstantPhysicalSize()
        {
            float dpi = Screen.dpi;
            float num2 = (dpi != 0f) ? dpi : this.m_FallbackScreenDPI;
            float num3 = 1f;
            switch (this.m_PhysicalUnit)
            {
                case Unit.Centimeters:
                    num3 = 2.54f;
                    break;

                case Unit.Millimeters:
                    num3 = 25.4f;
                    break;

                case Unit.Inches:
                    num3 = 1f;
                    break;

                case Unit.Points:
                    num3 = 72f;
                    break;

                case Unit.Picas:
                    num3 = 6f;
                    break;
            }
            this.SetScaleFactor(num2 / num3);
            this.SetReferencePixelsPerUnit((this.m_ReferencePixelsPerUnit * num3) / this.m_DefaultSpriteDPI);
        }

        /// <summary>
        /// <para>Handles canvas scaling for a constant pixel size.</para>
        /// </summary>
        protected virtual void HandleConstantPixelSize()
        {
            this.SetScaleFactor(this.m_ScaleFactor);
            this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
        }

        /// <summary>
        /// <para>Handles canvas scaling that scales with the screen size.</para>
        /// </summary>
        protected virtual void HandleScaleWithScreenSize()
        {
            Vector2 vector = new Vector2((float) Screen.width, (float) Screen.height);
            int targetDisplay = this.m_Canvas.targetDisplay;
            if ((targetDisplay > 0) && (targetDisplay < Display.displays.Length))
            {
                Display display = Display.displays[targetDisplay];
                vector = new Vector2((float) display.renderingWidth, (float) display.renderingHeight);
            }
            float scaleFactor = 0f;
            switch (this.m_ScreenMatchMode)
            {
                case ScreenMatchMode.MatchWidthOrHeight:
                {
                    float a = Mathf.Log(vector.x / this.m_ReferenceResolution.x, 2f);
                    float b = Mathf.Log(vector.y / this.m_ReferenceResolution.y, 2f);
                    float p = Mathf.Lerp(a, b, this.m_MatchWidthOrHeight);
                    scaleFactor = Mathf.Pow(2f, p);
                    break;
                }
                case ScreenMatchMode.Expand:
                    scaleFactor = Mathf.Min((float) (vector.x / this.m_ReferenceResolution.x), (float) (vector.y / this.m_ReferenceResolution.y));
                    break;

                case ScreenMatchMode.Shrink:
                    scaleFactor = Mathf.Max((float) (vector.x / this.m_ReferenceResolution.x), (float) (vector.y / this.m_ReferenceResolution.y));
                    break;
            }
            this.SetScaleFactor(scaleFactor);
            this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
        }

        /// <summary>
        /// <para>Handles canvas scaling for world canvas.</para>
        /// </summary>
        protected virtual void HandleWorldCanvas()
        {
            this.SetScaleFactor(this.m_DynamicPixelsPerUnit);
            this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.SetScaleFactor(1f);
            this.SetReferencePixelsPerUnit(100f);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Canvas = base.GetComponent<Canvas>();
            this.Handle();
        }

        protected override void OnValidate()
        {
            this.m_ScaleFactor = Mathf.Max(0.01f, this.m_ScaleFactor);
            this.m_DefaultSpriteDPI = Mathf.Max(1f, this.m_DefaultSpriteDPI);
        }

        /// <summary>
        /// <para>Sets the referencePixelsPerUnit on the Canvas.</para>
        /// </summary>
        /// <param name="referencePixelsPerUnit"></param>
        protected void SetReferencePixelsPerUnit(float referencePixelsPerUnit)
        {
            if (referencePixelsPerUnit != this.m_PrevReferencePixelsPerUnit)
            {
                this.m_Canvas.referencePixelsPerUnit = referencePixelsPerUnit;
                this.m_PrevReferencePixelsPerUnit = referencePixelsPerUnit;
            }
        }

        /// <summary>
        /// <para>Sets the scale factor on the canvas.</para>
        /// </summary>
        /// <param name="scaleFactor">The scale factor to use.</param>
        protected void SetScaleFactor(float scaleFactor)
        {
            if (scaleFactor != this.m_PrevScaleFactor)
            {
                this.m_Canvas.scaleFactor = scaleFactor;
                this.m_PrevScaleFactor = scaleFactor;
            }
        }

        /// <summary>
        /// <para>Handles per-frame checking if the canvas scaling needs to be updated.</para>
        /// </summary>
        protected virtual void Update()
        {
            this.Handle();
        }

        /// <summary>
        /// <para>The pixels per inch to use for sprites that have a 'Pixels Per Unit' setting that matches the 'Reference Pixels Per Unit' setting.</para>
        /// </summary>
        public float defaultSpriteDPI
        {
            get => 
                this.m_DefaultSpriteDPI;
            set
            {
                this.m_DefaultSpriteDPI = Mathf.Max(1f, value);
            }
        }

        /// <summary>
        /// <para>The amount of pixels per unit to use for dynamically created bitmaps in the UI, such as Text.</para>
        /// </summary>
        public float dynamicPixelsPerUnit
        {
            get => 
                this.m_DynamicPixelsPerUnit;
            set
            {
                this.m_DynamicPixelsPerUnit = value;
            }
        }

        /// <summary>
        /// <para>The DPI to assume if the screen DPI is not known.</para>
        /// </summary>
        public float fallbackScreenDPI
        {
            get => 
                this.m_FallbackScreenDPI;
            set
            {
                this.m_FallbackScreenDPI = value;
            }
        }

        /// <summary>
        /// <para>Setting to scale the Canvas to match the width or height of the reference resolution, or a combination.</para>
        /// </summary>
        public float matchWidthOrHeight
        {
            get => 
                this.m_MatchWidthOrHeight;
            set
            {
                this.m_MatchWidthOrHeight = value;
            }
        }

        /// <summary>
        /// <para>The physical unit to specify positions and sizes in.</para>
        /// </summary>
        public Unit physicalUnit
        {
            get => 
                this.m_PhysicalUnit;
            set
            {
                this.m_PhysicalUnit = value;
            }
        }

        /// <summary>
        /// <para>If a sprite has this 'Pixels Per Unit' setting, then one pixel in the sprite will cover one unit in the UI.</para>
        /// </summary>
        public float referencePixelsPerUnit
        {
            get => 
                this.m_ReferencePixelsPerUnit;
            set
            {
                this.m_ReferencePixelsPerUnit = value;
            }
        }

        /// <summary>
        /// <para>The resolution the UI layout is designed for.</para>
        /// </summary>
        public Vector2 referenceResolution
        {
            get => 
                this.m_ReferenceResolution;
            set
            {
                this.m_ReferenceResolution = value;
                if ((this.m_ReferenceResolution.x > -1E-05f) && (this.m_ReferenceResolution.x < 1E-05f))
                {
                    this.m_ReferenceResolution.x = 1E-05f * Mathf.Sign(this.m_ReferenceResolution.x);
                }
                if ((this.m_ReferenceResolution.y > -1E-05f) && (this.m_ReferenceResolution.y < 1E-05f))
                {
                    this.m_ReferenceResolution.y = 1E-05f * Mathf.Sign(this.m_ReferenceResolution.y);
                }
            }
        }

        /// <summary>
        /// <para>Scales all UI elements in the Canvas by this factor.</para>
        /// </summary>
        public float scaleFactor
        {
            get => 
                this.m_ScaleFactor;
            set
            {
                this.m_ScaleFactor = Mathf.Max(0.01f, value);
            }
        }

        /// <summary>
        /// <para>A mode used to scale the canvas area if the aspect ratio of the current resolution doesn't fit the reference resolution.</para>
        /// </summary>
        public ScreenMatchMode screenMatchMode
        {
            get => 
                this.m_ScreenMatchMode;
            set
            {
                this.m_ScreenMatchMode = value;
            }
        }

        /// <summary>
        /// <para>Determines how UI elements in the Canvas are scaled.</para>
        /// </summary>
        public ScaleMode uiScaleMode
        {
            get => 
                this.m_UiScaleMode;
            set
            {
                this.m_UiScaleMode = value;
            }
        }

        /// <summary>
        /// <para>Determines how UI elements in the Canvas are scaled.</para>
        /// </summary>
        public enum ScaleMode
        {
            ConstantPixelSize,
            ScaleWithScreenSize,
            ConstantPhysicalSize
        }

        /// <summary>
        /// <para>Scale the canvas area with the width as reference, the height as reference, or something in between.</para>
        /// </summary>
        public enum ScreenMatchMode
        {
            MatchWidthOrHeight,
            Expand,
            Shrink
        }

        /// <summary>
        /// <para>Settings used to specify a physical unit.</para>
        /// </summary>
        public enum Unit
        {
            Centimeters,
            Millimeters,
            Inches,
            Points,
            Picas
        }
    }
}

