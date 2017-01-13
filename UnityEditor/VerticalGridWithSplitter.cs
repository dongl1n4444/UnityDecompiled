namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class VerticalGridWithSplitter
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <bottomMargin>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <fixedWidth>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Vector2 <itemSize>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <leftMargin>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <minHorizontalSpacing>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <rightMargin>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <topMargin>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <verticalSpacing>k__BackingField;
        private int m_Columns = 1;
        private float m_CurrentSplitHeight;
        private float m_Height;
        private float m_HorizontalSpacing;
        private float m_LastSplitUpdate;
        private int m_Rows;
        private int m_SplitAfterRow;
        private float m_TargetSplitHeight;

        public Rect CalcRect(int itemIdx, float yOffset)
        {
            float num = Mathf.Floor((float) (itemIdx / this.columns));
            float num2 = itemIdx - (num * this.columns);
            return new Rect((num2 * (this.itemSize.x + this.horizontalSpacing)) + this.leftMargin, ((num * (this.itemSize.y + this.verticalSpacing)) + this.topMargin) + yOffset, this.itemSize.x, this.itemSize.y);
        }

        public Rect CalcSplitRect(int splitIndex, float yOffset) => 
            new Rect(0f, 0f, 0f, 0f);

        public void CloseSplit()
        {
            this.m_TargetSplitHeight = 0f;
        }

        public int GetMaxVisibleItems(float height)
        {
            int num = (int) Mathf.Ceil(((height - this.topMargin) - this.bottomMargin) / (this.itemSize.y + this.verticalSpacing));
            return (num * this.columns);
        }

        public void InitNumRowsAndColumns(int itemCount, int maxNumRows)
        {
            this.m_Columns = (int) Mathf.Floor(((this.fixedWidth - this.leftMargin) - this.rightMargin) / (this.itemSize.x + this.minHorizontalSpacing));
            this.m_Columns = Mathf.Max(this.m_Columns, 1);
            this.m_HorizontalSpacing = 0f;
            if (this.m_Columns > 1)
            {
                this.m_HorizontalSpacing = (this.fixedWidth - (((this.m_Columns * this.itemSize.x) + this.leftMargin) + this.rightMargin)) / ((float) (this.m_Columns - 1));
            }
            this.m_Rows = Mathf.Min(maxNumRows, (int) Mathf.Ceil(((float) itemCount) / ((float) this.m_Columns)));
            this.m_Height = (((this.m_Rows * (this.itemSize.y + this.verticalSpacing)) - this.verticalSpacing) + this.topMargin) + this.bottomMargin;
        }

        public void OpenSplit(int splitAfterRowIndex, int numItems)
        {
            int num = (int) Mathf.Ceil(((float) numItems) / ((float) this.m_Columns));
            float num2 = (((num * (this.itemSize.y + this.verticalSpacing)) - this.verticalSpacing) + this.topMargin) + this.bottomMargin;
            this.m_SplitAfterRow = splitAfterRowIndex;
            this.m_TargetSplitHeight = num2;
            this.m_LastSplitUpdate = Time.realtimeSinceStartup;
        }

        public void ResetSplit()
        {
            this.m_SplitAfterRow = -1;
            this.m_CurrentSplitHeight = 0f;
            this.m_LastSplitUpdate = -1f;
            this.m_TargetSplitHeight = 0f;
        }

        public bool UpdateSplitAnimationOnGUI()
        {
            if (this.m_SplitAfterRow != -1)
            {
                float num = Time.realtimeSinceStartup - this.m_LastSplitUpdate;
                this.m_CurrentSplitHeight = num * this.m_TargetSplitHeight;
                this.m_LastSplitUpdate = Time.realtimeSinceStartup;
                if ((this.m_CurrentSplitHeight != this.m_TargetSplitHeight) && (Event.current.type == EventType.Repaint))
                {
                    this.m_CurrentSplitHeight = Mathf.MoveTowards(this.m_CurrentSplitHeight, this.m_TargetSplitHeight, 0.03f);
                    if ((this.m_CurrentSplitHeight == 0f) && (this.m_TargetSplitHeight == 0f))
                    {
                        this.ResetSplit();
                    }
                    return true;
                }
            }
            return false;
        }

        public float bottomMargin { get; set; }

        public int columns =>
            this.m_Columns;

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

        public float verticalSpacing { get; set; }
    }
}

