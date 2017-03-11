namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>A 2D rectangular mask that allows for clipping / masking of areas outside the mask.</para>
    /// </summary>
    [AddComponentMenu("UI/Rect Mask 2D", 13), ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class RectMask2D : UIBehaviour, IClipper, ICanvasRaycastFilter
    {
        [NonSerialized]
        private List<RectMask2D> m_Clippers = new List<RectMask2D>();
        [NonSerialized]
        private HashSet<IClippable> m_ClipTargets = new HashSet<IClippable>();
        [NonSerialized]
        private bool m_ForceClip;
        [NonSerialized]
        private Rect m_LastClipRectCanvasSpace;
        [NonSerialized]
        private bool m_LastValidClipRect;
        [NonSerialized]
        private RectTransform m_RectTransform;
        [NonSerialized]
        private bool m_ShouldRecalculateClipRects;
        [NonSerialized]
        private readonly RectangularVertexClipper m_VertexClipper = new RectangularVertexClipper();

        protected RectMask2D()
        {
        }

        /// <summary>
        /// <para>Add a [IClippable]] to be tracked by the mask.</para>
        /// </summary>
        /// <param name="clippable"></param>
        public void AddClippable(IClippable clippable)
        {
            if (clippable != null)
            {
                this.m_ShouldRecalculateClipRects = true;
                if (!this.m_ClipTargets.Contains(clippable))
                {
                    this.m_ClipTargets.Add(clippable);
                }
                this.m_ForceClip = true;
            }
        }

        /// <summary>
        /// <para>See:ICanvasRaycastFilter.</para>
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="eventCamera"></param>
        public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) => 
            (!base.isActiveAndEnabled || RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera));

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            this.m_ShouldRecalculateClipRects = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.m_ClipTargets.Clear();
            this.m_Clippers.Clear();
            ClipperRegistry.Unregister(this);
            MaskUtilities.Notify2DMaskStateChanged(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_ShouldRecalculateClipRects = true;
            ClipperRegistry.Register(this);
            MaskUtilities.Notify2DMaskStateChanged(this);
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            this.m_ShouldRecalculateClipRects = true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.m_ShouldRecalculateClipRects = true;
            if (this.IsActive())
            {
                MaskUtilities.Notify2DMaskStateChanged(this);
            }
        }

        /// <summary>
        /// <para>See: IClipper.PerformClipping.</para>
        /// </summary>
        public virtual void PerformClipping()
        {
            if (this.m_ShouldRecalculateClipRects)
            {
                MaskUtilities.GetRectMasksForClip(this, this.m_Clippers);
                this.m_ShouldRecalculateClipRects = false;
            }
            bool validRect = true;
            Rect rect = Clipping.FindCullAndClipWorldRect(this.m_Clippers, out validRect);
            bool flag2 = rect != this.m_LastClipRectCanvasSpace;
            if (flag2 || this.m_ForceClip)
            {
                foreach (IClippable clippable in this.m_ClipTargets)
                {
                    clippable.SetClipRect(rect, validRect);
                }
                this.m_LastClipRectCanvasSpace = rect;
                this.m_LastValidClipRect = validRect;
            }
            foreach (IClippable clippable2 in this.m_ClipTargets)
            {
                MaskableGraphic graphic = clippable2 as MaskableGraphic;
                if (((graphic == null) || graphic.canvasRenderer.hasMoved) || flag2)
                {
                    clippable2.Cull(this.m_LastClipRectCanvasSpace, this.m_LastValidClipRect);
                }
            }
        }

        /// <summary>
        /// <para>Remove an IClippable from being tracked by the mask.</para>
        /// </summary>
        /// <param name="clippable"></param>
        public void RemoveClippable(IClippable clippable)
        {
            if (clippable != null)
            {
                this.m_ShouldRecalculateClipRects = true;
                clippable.SetClipRect(new Rect(), false);
                this.m_ClipTargets.Remove(clippable);
                this.m_ForceClip = true;
            }
        }

        /// <summary>
        /// <para>Get the Rect for the mask in canvas space.</para>
        /// </summary>
        public Rect canvasRect
        {
            get
            {
                Canvas c = null;
                List<Canvas> results = ListPool<Canvas>.Get();
                base.gameObject.GetComponentsInParent<Canvas>(false, results);
                if (results.Count > 0)
                {
                    c = results[results.Count - 1];
                }
                ListPool<Canvas>.Release(results);
                return this.m_VertexClipper.GetCanvasRect(this.rectTransform, c);
            }
        }

        /// <summary>
        /// <para>Get the RectTransform for the mask.</para>
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                RectTransform rectTransform;
                if (this.m_RectTransform != null)
                {
                    rectTransform = this.m_RectTransform;
                }
                else
                {
                    rectTransform = this.m_RectTransform = base.GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }
    }
}

