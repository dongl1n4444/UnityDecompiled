namespace UnityEditor
{
    using System;

    [Serializable]
    internal class CurveEditorSettings
    {
        public bool allowDeleteLastKeyInCurve = false;
        public bool allowDraggingCurvesAndRegions = true;
        public float hTickLabelOffset = 0f;
        private bool m_HRangeLocked;
        private float m_HRangeMax = float.PositiveInfinity;
        private float m_HRangeMin = float.NegativeInfinity;
        private bool m_HSlider = true;
        private TickStyle m_HTickStyle = new TickStyle();
        private bool m_ScaleWithWindow = true;
        private bool m_VRangeLocked;
        private float m_VRangeMax = float.PositiveInfinity;
        private float m_VRangeMin = float.NegativeInfinity;
        private bool m_VSlider = true;
        private TickStyle m_VTickStyle = new TickStyle();
        internal RectangleToolFlags rectangleToolFlags = RectangleToolFlags.NoRectangleTool;
        public bool showAxisLabels = true;
        public bool showWrapperPopups = false;
        public bool undoRedoSelection = false;
        public bool useFocusColors = false;
        public EditorGUIUtility.SkinnedColor wrapColor = new EditorGUIUtility.SkinnedColor(new Color(1f, 1f, 1f, 0.5f), new Color(0.65f, 0.65f, 0.65f, 0.5f));

        public bool hasUnboundedRanges =>
            ((((this.m_HRangeMin == float.NegativeInfinity) || (this.m_HRangeMax == float.PositiveInfinity)) || (this.m_VRangeMin == float.NegativeInfinity)) || (this.m_VRangeMax == float.PositiveInfinity));

        internal bool hRangeLocked
        {
            get => 
                this.m_HRangeLocked;
            set
            {
                this.m_HRangeLocked = value;
            }
        }

        public float hRangeMax
        {
            get => 
                this.m_HRangeMax;
            set
            {
                this.m_HRangeMax = value;
            }
        }

        public float hRangeMin
        {
            get => 
                this.m_HRangeMin;
            set
            {
                this.m_HRangeMin = value;
            }
        }

        public bool hSlider
        {
            get => 
                this.m_HSlider;
            set
            {
                this.m_HSlider = value;
            }
        }

        internal TickStyle hTickStyle
        {
            get => 
                this.m_HTickStyle;
            set
            {
                this.m_HTickStyle = value;
            }
        }

        internal bool scaleWithWindow
        {
            get => 
                this.m_ScaleWithWindow;
            set
            {
                this.m_ScaleWithWindow = value;
            }
        }

        internal bool vRangeLocked
        {
            get => 
                this.m_VRangeLocked;
            set
            {
                this.m_VRangeLocked = value;
            }
        }

        public float vRangeMax
        {
            get => 
                this.m_VRangeMax;
            set
            {
                this.m_VRangeMax = value;
            }
        }

        public float vRangeMin
        {
            get => 
                this.m_VRangeMin;
            set
            {
                this.m_VRangeMin = value;
            }
        }

        public bool vSlider
        {
            get => 
                this.m_VSlider;
            set
            {
                this.m_VSlider = value;
            }
        }

        internal TickStyle vTickStyle
        {
            get => 
                this.m_VTickStyle;
            set
            {
                this.m_VTickStyle = value;
            }
        }

        internal enum RectangleToolFlags
        {
            NoRectangleTool,
            MiniRectangleTool,
            FullRectangleTool
        }
    }
}

