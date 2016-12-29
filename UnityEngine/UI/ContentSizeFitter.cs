namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>Resizes a RectTransform to fit the size of its content.</para>
    /// </summary>
    [AddComponentMenu("Layout/Content Size Fitter", 0x8d), ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitter : UIBehaviour, ILayoutSelfController, ILayoutController
    {
        [SerializeField]
        protected FitMode m_HorizontalFit = FitMode.Unconstrained;
        [NonSerialized]
        private RectTransform m_Rect;
        private DrivenRectTransformTracker m_Tracker;
        [SerializeField]
        protected FitMode m_VerticalFit = FitMode.Unconstrained;

        protected ContentSizeFitter()
        {
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode mode = (axis != 0) ? this.verticalFit : this.horizontalFit;
            if (mode == FitMode.Unconstrained)
            {
                this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.None);
            }
            else
            {
                this.m_Tracker.Add(this, this.rectTransform, (axis != 0) ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDeltaX);
                if (mode == FitMode.MinSize)
                {
                    this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis, LayoutUtility.GetMinSize(this.m_Rect, axis));
                }
                else
                {
                    this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis, LayoutUtility.GetPreferredSize(this.m_Rect, axis));
                }
            }
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            this.SetDirty();
        }

        protected override void OnValidate()
        {
            this.SetDirty();
        }

        /// <summary>
        /// <para>Mark the ContentSizeFitter as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (this.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            }
        }

        /// <summary>
        /// <para>Method called by the layout system.</para>
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            this.m_Tracker.Clear();
            this.HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        /// <para>Method called by the layout system.</para>
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            this.HandleSelfFittingAlongAxis(1);
        }

        /// <summary>
        /// <para>The fit mode to use to determine the width.</para>
        /// </summary>
        public FitMode horizontalFit
        {
            get => 
                this.m_HorizontalFit;
            set
            {
                if (SetPropertyUtility.SetStruct<FitMode>(ref this.m_HorizontalFit, value))
                {
                    this.SetDirty();
                }
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (this.m_Rect == null)
                {
                    this.m_Rect = base.GetComponent<RectTransform>();
                }
                return this.m_Rect;
            }
        }

        /// <summary>
        /// <para>The fit mode to use to determine the height.</para>
        /// </summary>
        public FitMode verticalFit
        {
            get => 
                this.m_VerticalFit;
            set
            {
                if (SetPropertyUtility.SetStruct<FitMode>(ref this.m_VerticalFit, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>The size fit mode to use.</para>
        /// </summary>
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }
    }
}

