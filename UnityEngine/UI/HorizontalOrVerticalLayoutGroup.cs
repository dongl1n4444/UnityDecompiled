namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Abstract base class for HorizontalLayoutGroup and VerticalLayoutGroup.</para>
    /// </summary>
    public abstract class HorizontalOrVerticalLayoutGroup : LayoutGroup
    {
        [SerializeField]
        protected bool m_ChildControlHeight = true;
        [SerializeField]
        protected bool m_ChildControlWidth = true;
        [SerializeField]
        protected bool m_ChildForceExpandHeight = true;
        [SerializeField]
        protected bool m_ChildForceExpandWidth = true;
        [SerializeField]
        protected float m_Spacing = 0f;

        protected HorizontalOrVerticalLayoutGroup()
        {
        }

        /// <summary>
        /// <para>Calculate the layout element properties for this layout element along the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        protected void CalcAlongAxis(int axis, bool isVertical)
        {
            float num = (axis != 0) ? ((float) base.padding.vertical) : ((float) base.padding.horizontal);
            bool controlSize = (axis != 0) ? this.m_ChildControlHeight : this.m_ChildControlWidth;
            bool childForceExpand = (axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth;
            float b = num;
            float num3 = num;
            float num4 = 0f;
            bool flag3 = isVertical ^ (axis == 1);
            for (int i = 0; i < base.rectChildren.Count; i++)
            {
                float num6;
                float num7;
                float num8;
                RectTransform child = base.rectChildren[i];
                this.GetChildSizes(child, axis, controlSize, childForceExpand, out num6, out num7, out num8);
                if (flag3)
                {
                    b = Mathf.Max(num6 + num, b);
                    num3 = Mathf.Max(num7 + num, num3);
                    num4 = Mathf.Max(num8, num4);
                }
                else
                {
                    b += num6 + this.spacing;
                    num3 += num7 + this.spacing;
                    num4 += num8;
                }
            }
            if (!flag3 && (base.rectChildren.Count > 0))
            {
                b -= this.spacing;
                num3 -= this.spacing;
            }
            num3 = Mathf.Max(b, num3);
            base.SetLayoutInputForAxis(b, num3, num4, axis);
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0f;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }
            if (childForceExpand)
            {
                flexible = Mathf.Max(flexible, 1f);
            }
        }

        protected override void Reset()
        {
            base.Reset();
            this.m_ChildControlWidth = false;
            this.m_ChildControlHeight = false;
        }

        /// <summary>
        /// <para>Set the positions and sizes of the child layout elements for the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        protected void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            float num = base.rectTransform.rect.size[axis];
            bool controlSize = (axis != 0) ? this.m_ChildControlHeight : this.m_ChildControlWidth;
            bool childForceExpand = (axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth;
            float alignmentOnAxis = base.GetAlignmentOnAxis(axis);
            if (isVertical ^ (axis == 1))
            {
                float num3 = num - ((axis != 0) ? ((float) base.padding.vertical) : ((float) base.padding.horizontal));
                for (int i = 0; i < base.rectChildren.Count; i++)
                {
                    float num5;
                    float num6;
                    float num7;
                    RectTransform child = base.rectChildren[i];
                    this.GetChildSizes(child, axis, controlSize, childForceExpand, out num5, out num6, out num7);
                    float requiredSpaceWithoutPadding = Mathf.Clamp(num3, num5, (num7 <= 0f) ? num6 : num);
                    float startOffset = base.GetStartOffset(axis, requiredSpaceWithoutPadding);
                    if (controlSize)
                    {
                        base.SetChildAlongAxis(child, axis, startOffset, requiredSpaceWithoutPadding);
                    }
                    else
                    {
                        float num10 = (requiredSpaceWithoutPadding - child.sizeDelta[axis]) * alignmentOnAxis;
                        base.SetChildAlongAxis(child, axis, startOffset + num10);
                    }
                }
            }
            else
            {
                float pos = (axis != 0) ? ((float) base.padding.top) : ((float) base.padding.left);
                if ((base.GetTotalFlexibleSize(axis) == 0f) && (base.GetTotalPreferredSize(axis) < num))
                {
                    pos = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - ((axis != 0) ? ((float) base.padding.vertical) : ((float) base.padding.horizontal)));
                }
                float t = 0f;
                if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
                {
                    t = Mathf.Clamp01((num - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
                }
                float num13 = 0f;
                if ((num > base.GetTotalPreferredSize(axis)) && (base.GetTotalFlexibleSize(axis) > 0f))
                {
                    num13 = (num - base.GetTotalPreferredSize(axis)) / base.GetTotalFlexibleSize(axis);
                }
                for (int j = 0; j < base.rectChildren.Count; j++)
                {
                    float num15;
                    float num16;
                    float num17;
                    RectTransform transform2 = base.rectChildren[j];
                    this.GetChildSizes(transform2, axis, controlSize, childForceExpand, out num15, out num16, out num17);
                    float size = Mathf.Lerp(num15, num16, t) + (num17 * num13);
                    if (controlSize)
                    {
                        base.SetChildAlongAxis(transform2, axis, pos, size);
                    }
                    else
                    {
                        float num19 = (size - transform2.sizeDelta[axis]) * alignmentOnAxis;
                        base.SetChildAlongAxis(transform2, axis, pos + num19);
                    }
                    pos += size + this.spacing;
                }
            }
        }

        /// <summary>
        /// <para>Should the layout group control the heights of the children?</para>
        /// </summary>
        public bool childControlHeight
        {
            get
            {
                return this.m_ChildControlHeight;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_ChildControlHeight, value);
            }
        }

        /// <summary>
        /// <para>Should the layout group control the widths of the children?</para>
        /// </summary>
        public bool childControlWidth
        {
            get
            {
                return this.m_ChildControlWidth;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_ChildControlWidth, value);
            }
        }

        /// <summary>
        /// <para>Whether to force the children to expand to fill additional available vertical space.</para>
        /// </summary>
        public bool childForceExpandHeight
        {
            get
            {
                return this.m_ChildForceExpandHeight;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
            }
        }

        /// <summary>
        /// <para>Whether to force the children to expand to fill additional available horizontal space.</para>
        /// </summary>
        public bool childForceExpandWidth
        {
            get
            {
                return this.m_ChildForceExpandWidth;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
            }
        }

        /// <summary>
        /// <para>The spacing to use between layout elements in the layout group.</para>
        /// </summary>
        public float spacing
        {
            get
            {
                return this.m_Spacing;
            }
            set
            {
                base.SetProperty<float>(ref this.m_Spacing, value);
            }
        }
    }
}

