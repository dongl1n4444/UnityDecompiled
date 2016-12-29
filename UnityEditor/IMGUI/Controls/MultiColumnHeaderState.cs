namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Linq;
    using UnityEngine;

    [Serializable]
    internal class MultiColumnHeaderState
    {
        [SerializeField]
        private Column[] m_Columns;
        [SerializeField]
        private int m_PreviousSortedColumnIndex;
        [SerializeField]
        private int m_SortedColumnIndex;
        [SerializeField]
        private int[] m_VisibleColumns;

        public MultiColumnHeaderState(Column[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentException("columns are no allowed to be null", "columns");
            }
            if (columns.Length == 0)
            {
                throw new ArgumentException("columns array should at least have one column: it is empty", "columns");
            }
            this.m_Columns = columns;
            this.m_SortedColumnIndex = -1;
            this.m_PreviousSortedColumnIndex = -1;
            this.m_VisibleColumns = new int[this.m_Columns.Length];
            for (int i = 0; i < this.m_Columns.Length; i++)
            {
                this.m_VisibleColumns[i] = i;
            }
        }

        public Column[] columns =>
            this.m_Columns;

        public int previousSortedColumnIndex =>
            this.m_PreviousSortedColumnIndex;

        public int sortedColumnIndex
        {
            get => 
                this.m_SortedColumnIndex;
            set
            {
                if (value != this.m_SortedColumnIndex)
                {
                    this.m_PreviousSortedColumnIndex = this.m_SortedColumnIndex;
                }
                this.m_SortedColumnIndex = value;
            }
        }

        public int[] visibleColumns
        {
            get => 
                this.m_VisibleColumns;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("visibleColumns should not be set to null");
                }
                if (value.Length == 0)
                {
                    throw new ArgumentException("visibleColumns should should not be set to an empty array. At least one visible column is required.");
                }
                this.m_VisibleColumns = value;
            }
        }

        public float widthOfAllVisibleColumns =>
            Enumerable.Sum<int>(this.visibleColumns, new Func<int, float>(this, (IntPtr) this.<get_widthOfAllVisibleColumns>m__0));

        [Serializable]
        public class Column
        {
            [SerializeField]
            public bool allowToggleVisibility = true;
            [SerializeField]
            public bool autoResize;
            [SerializeField]
            public string headerText;
            [SerializeField]
            public TextAlignment headerTextAlignment = TextAlignment.Left;
            [SerializeField]
            public float maxWidth;
            [SerializeField]
            public float minWidth;
            [SerializeField]
            public bool sortedAscending;
            [SerializeField]
            public TextAlignment sortingArrowAlignment = TextAlignment.Center;
            [SerializeField]
            public float width;
        }
    }
}

