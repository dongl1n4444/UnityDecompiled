namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Abstract base class to use for layout groups.</para>
    /// </summary>
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public abstract class LayoutGroup : UIBehaviour, ILayoutElement, ILayoutGroup, ILayoutController
    {
        [FormerlySerializedAs("m_Alignment"), SerializeField]
        protected TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;
        [SerializeField]
        protected RectOffset m_Padding = new RectOffset();
        [NonSerialized]
        private RectTransform m_Rect;
        [NonSerialized]
        private List<RectTransform> m_RectChildren = new List<RectTransform>();
        private Vector2 m_TotalFlexibleSize = Vector2.zero;
        private Vector2 m_TotalMinSize = Vector2.zero;
        private Vector2 m_TotalPreferredSize = Vector2.zero;
        protected DrivenRectTransformTracker m_Tracker;

        protected LayoutGroup()
        {
            if (this.m_Padding == null)
            {
                this.m_Padding = new RectOffset();
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
            this.m_RectChildren.Clear();
            List<Component> results = ListPool<Component>.Get();
            for (int i = 0; i < this.rectTransform.childCount; i++)
            {
                RectTransform child = this.rectTransform.GetChild(i) as RectTransform;
                if ((child != null) && child.gameObject.activeInHierarchy)
                {
                    child.GetComponents(typeof(ILayoutIgnorer), results);
                    if (results.Count == 0)
                    {
                        this.m_RectChildren.Add(child);
                    }
                    else
                    {
                        for (int j = 0; j < results.Count; j++)
                        {
                            ILayoutIgnorer ignorer = (ILayoutIgnorer) results[j];
                            if (!ignorer.ignoreLayout)
                            {
                                this.m_RectChildren.Add(child);
                                break;
                            }
                        }
                    }
                }
            }
            ListPool<Component>.Release(results);
            this.m_Tracker.Clear();
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public abstract void CalculateLayoutInputVertical();
        [DebuggerHidden]
        private IEnumerator DelayedSetDirty(RectTransform rectTransform) => 
            new <DelayedSetDirty>c__Iterator0 { rectTransform = rectTransform };

        /// <summary>
        /// <para>Returns the alignment on the specified axis as a fraction where 0 is lefttop, 0.5 is middle, and 1 is rightbottom.</para>
        /// </summary>
        /// <param name="axis">The axis to get alignment along. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        /// <para>The alignment as a fraction where 0 is lefttop, 0.5 is middle, and 1 is rightbottom.</para>
        /// </returns>
        protected float GetAlignmentOnAxis(int axis)
        {
            if (axis == 0)
            {
                return (((float) (this.childAlignment % TextAnchor.MiddleLeft)) * 0.5f);
            }
            return (((float) (this.childAlignment / TextAnchor.MiddleLeft)) * 0.5f);
        }

        /// <summary>
        /// <para>Returns the calculated position of the first child layout element along the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <param name="requiredSpaceWithoutPadding">The total space required on the given axis for all the layout elements including spacing and excluding padding.</param>
        /// <returns>
        /// <para>The position of the first child along the given axis.</para>
        /// </returns>
        protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float num = requiredSpaceWithoutPadding + ((axis != 0) ? ((float) this.padding.vertical) : ((float) this.padding.horizontal));
            float num2 = this.rectTransform.rect.size[axis];
            float num3 = num2 - num;
            float alignmentOnAxis = this.GetAlignmentOnAxis(axis);
            return (((axis != 0) ? ((float) this.padding.top) : ((float) this.padding.left)) + (num3 * alignmentOnAxis));
        }

        /// <summary>
        /// <para>The flexible size for the layout group on the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        /// <para>The flexible size.</para>
        /// </returns>
        protected float GetTotalFlexibleSize(int axis) => 
            this.m_TotalFlexibleSize[axis];

        /// <summary>
        /// <para>The min size for the layout group on the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        /// <para>The min size.</para>
        /// </returns>
        protected float GetTotalMinSize(int axis) => 
            this.m_TotalMinSize[axis];

        /// <summary>
        /// <para>The preferred size for the layout group on the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        /// <para>The preferred size.</para>
        /// </returns>
        protected float GetTotalPreferredSize(int axis) => 
            this.m_TotalPreferredSize[axis];

        /// <summary>
        /// <para>Callback for when properties have been changed by animation.</para>
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            this.SetDirty();
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
            base.OnRectTransformDimensionsChange();
            if (this.isRootLayoutGroup)
            {
                this.SetDirty();
            }
        }

        protected virtual void OnTransformChildrenChanged()
        {
            this.SetDirty();
        }

        protected override void OnValidate()
        {
            this.SetDirty();
        }

        protected void SetChildAlongAxis(RectTransform rect, int axis, float pos)
        {
            if (rect != null)
            {
                this.m_Tracker.Add(this, rect, DrivenTransformProperties.Anchors | ((axis != 0) ? DrivenTransformProperties.AnchoredPositionY : DrivenTransformProperties.AnchoredPositionX));
                rect.SetInsetAndSizeFromParentEdge((axis != 0) ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos, rect.sizeDelta[axis]);
            }
        }

        /// <summary>
        /// <para>Set the position and size of a child layout element along the given axis.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the child layout element.</param>
        /// <param name="axis">The axis to set the position and size along. 0 is horizontal and 1 is vertical.</param>
        /// <param name="pos">The position from the left side or top.</param>
        /// <param name="size">The size.</param>
        protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
        {
            if (rect != null)
            {
                this.m_Tracker.Add(this, rect, DrivenTransformProperties.Anchors | ((axis != 0) ? (DrivenTransformProperties.SizeDeltaY | DrivenTransformProperties.AnchoredPositionY) : (DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.AnchoredPositionX)));
                rect.SetInsetAndSizeFromParentEdge((axis != 0) ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos, size);
            }
        }

        /// <summary>
        /// <para>Mark the LayoutGroup as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (this.IsActive())
            {
                if (!CanvasUpdateRegistry.IsRebuildingLayout())
                {
                    LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
                }
                else
                {
                    base.StartCoroutine(this.DelayedSetDirty(this.rectTransform));
                }
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public abstract void SetLayoutHorizontal();
        /// <summary>
        /// <para>Used to set the calculated layout properties for the given axis.</para>
        /// </summary>
        /// <param name="totalMin">The min size for the layout group.</param>
        /// <param name="totalPreferred">The preferred size for the layout group.</param>
        /// <param name="totalFlexible">The flexible size for the layout group.</param>
        /// <param name="axis">The axis to set sizes for. 0 is horizontal and 1 is vertical.</param>
        protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
        {
            this.m_TotalMinSize[axis] = totalMin;
            this.m_TotalPreferredSize[axis] = totalPreferred;
            this.m_TotalFlexibleSize[axis] = totalFlexible;
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public abstract void SetLayoutVertical();
        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if (((((T) currentValue) != null) || (newValue != null)) && ((((T) currentValue) == null) || !currentValue.Equals(newValue)))
            {
                currentValue = newValue;
                this.SetDirty();
            }
        }

        /// <summary>
        /// <para>The alignment to use for the child layout elements in the layout group.</para>
        /// </summary>
        public TextAnchor childAlignment
        {
            get => 
                this.m_ChildAlignment;
            set
            {
                this.SetProperty<TextAnchor>(ref this.m_ChildAlignment, value);
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float flexibleHeight =>
            this.GetTotalFlexibleSize(1);

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float flexibleWidth =>
            this.GetTotalFlexibleSize(0);

        private bool isRootLayoutGroup =>
            ((base.transform.parent == null) || (base.transform.parent.GetComponent(typeof(ILayoutGroup)) == null));

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual int layoutPriority =>
            0;

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float minHeight =>
            this.GetTotalMinSize(1);

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float minWidth =>
            this.GetTotalMinSize(0);

        /// <summary>
        /// <para>The padding to add around the child layout elements.</para>
        /// </summary>
        public RectOffset padding
        {
            get => 
                this.m_Padding;
            set
            {
                this.SetProperty<RectOffset>(ref this.m_Padding, value);
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float preferredHeight =>
            this.GetTotalPreferredSize(1);

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual float preferredWidth =>
            this.GetTotalPreferredSize(0);

        protected List<RectTransform> rectChildren =>
            this.m_RectChildren;

        protected RectTransform rectTransform
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

        [CompilerGenerated]
        private sealed class <DelayedSetDirty>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal RectTransform rectTransform;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

