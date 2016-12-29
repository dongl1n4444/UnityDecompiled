namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Layout child layout elements in a grid.</para>
    /// </summary>
    [AddComponentMenu("Layout/Grid Layout Group", 0x98)]
    public class GridLayoutGroup : LayoutGroup
    {
        [SerializeField]
        protected Vector2 m_CellSize = new Vector2(100f, 100f);
        [SerializeField]
        protected Constraint m_Constraint = Constraint.Flexible;
        [SerializeField]
        protected int m_ConstraintCount = 2;
        [SerializeField]
        protected Vector2 m_Spacing = Vector2.zero;
        [SerializeField]
        protected Axis m_StartAxis = Axis.Horizontal;
        [SerializeField]
        protected Corner m_StartCorner = Corner.UpperLeft;

        protected GridLayoutGroup()
        {
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            int num = 0;
            int num2 = 0;
            if (this.m_Constraint == Constraint.FixedColumnCount)
            {
                num = num2 = this.m_ConstraintCount;
            }
            else if (this.m_Constraint == Constraint.FixedRowCount)
            {
                num = num2 = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) this.m_ConstraintCount)) - 0.001f);
            }
            else
            {
                num = 1;
                num2 = Mathf.CeilToInt(Mathf.Sqrt((float) base.rectChildren.Count));
            }
            base.SetLayoutInputForAxis((base.padding.horizontal + ((this.cellSize.x + this.spacing.x) * num)) - this.spacing.x, (base.padding.horizontal + ((this.cellSize.x + this.spacing.x) * num2)) - this.spacing.x, -1f, 0);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            int constraintCount = 0;
            if (this.m_Constraint == Constraint.FixedColumnCount)
            {
                constraintCount = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) this.m_ConstraintCount)) - 0.001f);
            }
            else if (this.m_Constraint == Constraint.FixedRowCount)
            {
                constraintCount = this.m_ConstraintCount;
            }
            else
            {
                float x = base.rectTransform.rect.size.x;
                int num3 = Mathf.Max(1, Mathf.FloorToInt((((x - base.padding.horizontal) + this.spacing.x) + 0.001f) / (this.cellSize.x + this.spacing.x)));
                constraintCount = Mathf.CeilToInt(((float) base.rectChildren.Count) / ((float) num3));
            }
            float totalMin = (base.padding.vertical + ((this.cellSize.y + this.spacing.y) * constraintCount)) - this.spacing.y;
            base.SetLayoutInputForAxis(totalMin, totalMin, -1f, 1);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.constraintCount = this.constraintCount;
        }

        private void SetCellsAlongAxis(int axis)
        {
            if (axis == 0)
            {
                for (int i = 0; i < base.rectChildren.Count; i++)
                {
                    RectTransform rectTransform = base.rectChildren[i];
                    this.m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                    rectTransform.sizeDelta = this.cellSize;
                }
            }
            else
            {
                int num8;
                int num9;
                int num10;
                float x = base.rectTransform.rect.size.x;
                float y = base.rectTransform.rect.size.y;
                int constraintCount = 1;
                int num5 = 1;
                if (this.m_Constraint == Constraint.FixedColumnCount)
                {
                    constraintCount = this.m_ConstraintCount;
                    num5 = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) constraintCount)) - 0.001f);
                }
                else if (this.m_Constraint == Constraint.FixedRowCount)
                {
                    num5 = this.m_ConstraintCount;
                    constraintCount = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) num5)) - 0.001f);
                }
                else
                {
                    if ((this.cellSize.x + this.spacing.x) <= 0f)
                    {
                        constraintCount = 0x7fffffff;
                    }
                    else
                    {
                        constraintCount = Mathf.Max(1, Mathf.FloorToInt((((x - base.padding.horizontal) + this.spacing.x) + 0.001f) / (this.cellSize.x + this.spacing.x)));
                    }
                    if ((this.cellSize.y + this.spacing.y) <= 0f)
                    {
                        num5 = 0x7fffffff;
                    }
                    else
                    {
                        num5 = Mathf.Max(1, Mathf.FloorToInt((((y - base.padding.vertical) + this.spacing.y) + 0.001f) / (this.cellSize.y + this.spacing.y)));
                    }
                }
                int num6 = (int) (this.startCorner % Corner.LowerLeft);
                int num7 = (int) (this.startCorner / Corner.LowerLeft);
                if (this.startAxis == Axis.Horizontal)
                {
                    num8 = constraintCount;
                    num9 = Mathf.Clamp(constraintCount, 1, base.rectChildren.Count);
                    num10 = Mathf.Clamp(num5, 1, Mathf.CeilToInt(((float) base.rectChildren.Count) / ((float) num8)));
                }
                else
                {
                    num8 = num5;
                    num10 = Mathf.Clamp(num5, 1, base.rectChildren.Count);
                    num9 = Mathf.Clamp(constraintCount, 1, Mathf.CeilToInt(((float) base.rectChildren.Count) / ((float) num8)));
                }
                Vector2 vector13 = new Vector2((num9 * this.cellSize.x) + ((num9 - 1) * this.spacing.x), (num10 * this.cellSize.y) + ((num10 - 1) * this.spacing.y));
                Vector2 vector18 = new Vector2(base.GetStartOffset(0, vector13.x), base.GetStartOffset(1, vector13.y));
                for (int j = 0; j < base.rectChildren.Count; j++)
                {
                    int num12;
                    int num13;
                    if (this.startAxis == Axis.Horizontal)
                    {
                        num12 = j % num8;
                        num13 = j / num8;
                    }
                    else
                    {
                        num12 = j / num8;
                        num13 = j % num8;
                    }
                    if (num6 == 1)
                    {
                        num12 = (num9 - 1) - num12;
                    }
                    if (num7 == 1)
                    {
                        num13 = (num10 - 1) - num13;
                    }
                    base.SetChildAlongAxis(base.rectChildren[j], 0, vector18.x + ((this.cellSize[0] + this.spacing[0]) * num12), this.cellSize[0]);
                    base.SetChildAlongAxis(base.rectChildren[j], 1, vector18.y + ((this.cellSize[1] + this.spacing[1]) * num13), this.cellSize[1]);
                }
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            this.SetCellsAlongAxis(0);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            this.SetCellsAlongAxis(1);
        }

        /// <summary>
        /// <para>The size to use for each cell in the grid.</para>
        /// </summary>
        public Vector2 cellSize
        {
            get => 
                this.m_CellSize;
            set
            {
                base.SetProperty<Vector2>(ref this.m_CellSize, value);
            }
        }

        /// <summary>
        /// <para>Which constraint to use for the GridLayoutGroup.</para>
        /// </summary>
        public Constraint constraint
        {
            get => 
                this.m_Constraint;
            set
            {
                base.SetProperty<Constraint>(ref this.m_Constraint, value);
            }
        }

        /// <summary>
        /// <para>How many cells there should be along the constrained axis.</para>
        /// </summary>
        public int constraintCount
        {
            get => 
                this.m_ConstraintCount;
            set
            {
                base.SetProperty<int>(ref this.m_ConstraintCount, Mathf.Max(1, value));
            }
        }

        /// <summary>
        /// <para>The spacing to use between layout elements in the grid.</para>
        /// </summary>
        public Vector2 spacing
        {
            get => 
                this.m_Spacing;
            set
            {
                base.SetProperty<Vector2>(ref this.m_Spacing, value);
            }
        }

        /// <summary>
        /// <para>Which axis should cells be placed along first?</para>
        /// </summary>
        public Axis startAxis
        {
            get => 
                this.m_StartAxis;
            set
            {
                base.SetProperty<Axis>(ref this.m_StartAxis, value);
            }
        }

        /// <summary>
        /// <para>Which corner should the first cell be placed in?</para>
        /// </summary>
        public Corner startCorner
        {
            get => 
                this.m_StartCorner;
            set
            {
                base.SetProperty<Corner>(ref this.m_StartCorner, value);
            }
        }

        /// <summary>
        /// <para>An axis that can be horizontal or vertical.</para>
        /// </summary>
        public enum Axis
        {
            Horizontal,
            Vertical
        }

        /// <summary>
        /// <para>A constraint on either the number of columns or rows.</para>
        /// </summary>
        public enum Constraint
        {
            Flexible,
            FixedColumnCount,
            FixedRowCount
        }

        /// <summary>
        /// <para>One of the four corners in a rectangle.</para>
        /// </summary>
        public enum Corner
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight
        }
    }
}

