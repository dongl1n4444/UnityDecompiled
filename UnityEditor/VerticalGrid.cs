namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class VerticalGrid
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <bottomMargin>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <fixedHorizontalSpacing>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <fixedWidth>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Vector2 <itemSize>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <leftMargin>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <minHorizontalSpacing>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <rightMargin>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <topMargin>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <useFixedHorizontalSpacing>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <verticalSpacing>k__BackingField;
        private int m_Columns = 1;
        private float m_Height;
        private float m_HorizontalSpacing;
        private int m_Rows;

        public int CalcColumns()
        {
            float num = !this.useFixedHorizontalSpacing ? this.minHorizontalSpacing : this.fixedHorizontalSpacing;
            int a = (int) Mathf.Floor(((this.fixedWidth - this.leftMargin) - this.rightMargin) / (this.itemSize.x + num));
            return Mathf.Max(a, 1);
        }

        public Rect CalcRect(int itemIdx, float yOffset)
        {
            float num = Mathf.Floor((float) (itemIdx / this.columns));
            float num2 = itemIdx - (num * this.columns);
            if (this.useFixedHorizontalSpacing)
            {
                return new Rect(this.leftMargin + (num2 * (this.itemSize.x + this.fixedHorizontalSpacing)), ((num * (this.itemSize.y + this.verticalSpacing)) + this.topMargin) + yOffset, this.itemSize.x, this.itemSize.y);
            }
            return new Rect((this.leftMargin + (this.horizontalSpacing * 0.5f)) + (num2 * (this.itemSize.x + this.horizontalSpacing)), ((num * (this.itemSize.y + this.verticalSpacing)) + this.topMargin) + yOffset, this.itemSize.x, this.itemSize.y);
        }

        public int CalcRows(int itemCount)
        {
            int num = (int) Mathf.Ceil(((float) itemCount) / ((float) this.CalcColumns()));
            if (num < 0)
            {
                return 0x7fffffff;
            }
            return num;
        }

        public int GetMaxVisibleItems(float height)
        {
            int num = (int) Mathf.Ceil(((height - this.topMargin) - this.bottomMargin) / (this.itemSize.y + this.verticalSpacing));
            return (num * this.columns);
        }

        public void InitNumRowsAndColumns(int itemCount, int maxNumRows)
        {
            if (this.useFixedHorizontalSpacing)
            {
                this.m_Columns = this.CalcColumns();
                this.m_HorizontalSpacing = this.fixedHorizontalSpacing;
                this.m_Rows = Mathf.Min(maxNumRows, this.CalcRows(itemCount));
                this.m_Height = (((this.m_Rows * (this.itemSize.y + this.verticalSpacing)) - this.verticalSpacing) + this.topMargin) + this.bottomMargin;
            }
            else
            {
                this.m_Columns = this.CalcColumns();
                this.m_HorizontalSpacing = Mathf.Max((float) 0f, (float) ((this.fixedWidth - (((this.m_Columns * this.itemSize.x) + this.leftMargin) + this.rightMargin)) / ((float) this.m_Columns)));
                this.m_Rows = Mathf.Min(maxNumRows, this.CalcRows(itemCount));
                if (this.m_Rows == 1)
                {
                    this.m_HorizontalSpacing = this.minHorizontalSpacing;
                }
                this.m_Height = (((this.m_Rows * (this.itemSize.y + this.verticalSpacing)) - this.verticalSpacing) + this.topMargin) + this.bottomMargin;
            }
        }

        public bool IsVisibleInScrollView(float scrollViewHeight, float scrollPos, float gridStartY, int maxIndex, out int startIndex, out int endIndex)
        {
            startIndex = endIndex = 0;
            float num2 = scrollPos;
            float num3 = scrollPos + scrollViewHeight;
            float num4 = gridStartY + this.topMargin;
            if (num4 > num3)
            {
                return false;
            }
            if ((num4 + this.height) < num2)
            {
                return false;
            }
            float num5 = this.itemSize.y + this.verticalSpacing;
            int num6 = Mathf.FloorToInt((num2 - num4) / num5);
            startIndex = num6 * this.columns;
            startIndex = Mathf.Clamp(startIndex, 0, maxIndex);
            int num7 = Mathf.FloorToInt((num3 - num4) / num5);
            endIndex = ((num7 + 1) * this.columns) - 1;
            endIndex = Mathf.Clamp(endIndex, 0, maxIndex);
            return true;
        }

        public override string ToString() => 
            $"VerticalGrid: rows {this.rows}, columns {this.columns}, fixedWidth {this.fixedWidth}, itemSize {this.itemSize}";

        public float bottomMargin { get; set; }

        public int columns =>
            this.m_Columns;

        public float fixedHorizontalSpacing { get; set; }

        public float fixedWidth { get; set; }

        public float height =>
            this.m_Height;

        public float horizontalSpacing =>
            this.m_HorizontalSpacing;

        public Vector2 itemSize { get; set; }

        public float leftMargin { get; set; }

        public float minHorizontalSpacing { get; set; }

        public float rightMargin { get; set; }

        public int rows =>
            this.m_Rows;

        public float topMargin { get; set; }

        public bool useFixedHorizontalSpacing { get; set; }

        public float verticalSpacing { get; set; }
    }
}

