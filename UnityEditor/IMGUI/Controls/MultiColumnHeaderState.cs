namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// <para>State used by the MultiColumnHeader.</para>
    /// </summary>
    [Serializable]
    public class MultiColumnHeaderState
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

        /// <summary>
        /// <para>Checks if the source state can transfer its serialized data to the destination state.</para>
        /// </summary>
        /// <param name="source">State that have serialized data to be transfered to the destination state.</param>
        /// <param name="destination">Destination state.</param>
        /// <returns>
        /// <para>Returns true if the source state have the same number of columns as the destination state.</para>
        /// </returns>
        public static bool CanOverwriteSerializedFields(MultiColumnHeaderState source, MultiColumnHeaderState destination)
        {
            if ((source == null) || (destination == null))
            {
                return false;
            }
            if ((source.m_Columns == null) || (destination.m_Columns == null))
            {
                return false;
            }
            return (source.m_Columns.Length == destination.m_Columns.Length);
        }

        /// <summary>
        /// <para>Overwrites the seralized fields from the source state to the destination state.</para>
        /// </summary>
        /// <param name="source">State that have serialized data to be transfered to the destination state.</param>
        /// <param name="destination">Destination state.</param>
        public static void OverwriteSerializedFields(MultiColumnHeaderState source, MultiColumnHeaderState destination)
        {
            if (!CanOverwriteSerializedFields(source, destination))
            {
                Debug.LogError("MultiColumnHeaderState: Not able to overwrite serialized fields");
            }
            else
            {
                destination.m_VisibleColumns = source.m_VisibleColumns.ToArray<int>();
                destination.m_SortedColumnIndex = source.m_SortedColumnIndex;
                destination.m_PreviousSortedColumnIndex = source.m_PreviousSortedColumnIndex;
                for (int i = 0; i < destination.m_Columns.Length; i++)
                {
                    destination.m_Columns[i].width = source.m_Columns[i].width;
                    destination.m_Columns[i].sortedAscending = source.m_Columns[i].sortedAscending;
                }
            }
        }

        /// <summary>
        /// <para>The array of column states used by the MultiColumnHeader class.</para>
        /// </summary>
        public Column[] columns =>
            this.m_Columns;

        /// <summary>
        /// <para>When a new column is set for sorting the previous sorted column is stored.</para>
        /// </summary>
        public int previousSortedColumnIndex =>
            this.m_PreviousSortedColumnIndex;

        /// <summary>
        /// <para>The main sorted column. This column will show the sorting arrow in the column header.</para>
        /// </summary>
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

        /// <summary>
        /// <para>This is the array of currently visible column indicies.</para>
        /// </summary>
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

        /// <summary>
        /// <para>Returns the sum of all the widths of the visible columns in the visibleColumns array.</para>
        /// </summary>
        public float widthOfAllVisibleColumns =>
            Enumerable.Sum<int>(this.visibleColumns, (Func<int, float>) (t => this.columns[t].width));

        /// <summary>
        /// <para>Column state.</para>
        /// </summary>
        [Serializable]
        public class Column
        {
            /// <summary>
            /// <para>Option to allow/disallow hiding the column from the context menu.</para>
            /// </summary>
            [NonSerialized]
            public bool allowToggleVisibility = true;
            /// <summary>
            /// <para>Option to allow the column to resize automatically when resizing the entire MultiColumnHeader.</para>
            /// </summary>
            [NonSerialized]
            public bool autoResize = true;
            /// <summary>
            /// <para>Is sorting enabled for this column. If false, left-clicking this column header has no effect.</para>
            /// </summary>
            [NonSerialized]
            public bool canSort = true;
            /// <summary>
            /// <para>If this is set then it is used for the context menu for toggling visibility, if not set then the ::headerContent is used.</para>
            /// </summary>
            [NonSerialized]
            public string contextMenuText;
            /// <summary>
            /// <para>This is the GUIContent that will be rendered in the column header.</para>
            /// </summary>
            [NonSerialized]
            public GUIContent headerContent = new GUIContent();
            /// <summary>
            /// <para>Alignment of the header content.</para>
            /// </summary>
            [NonSerialized]
            public TextAlignment headerTextAlignment = TextAlignment.Left;
            /// <summary>
            /// <para>Maximum width of the column.</para>
            /// </summary>
            [NonSerialized]
            public float maxWidth = 1000000f;
            /// <summary>
            /// <para>Minimum width of the column.</para>
            /// </summary>
            [NonSerialized]
            public float minWidth = 20f;
            /// <summary>
            /// <para>Value that controls if this column is sorted ascending or descending.</para>
            /// </summary>
            [SerializeField]
            public bool sortedAscending;
            /// <summary>
            /// <para>Alignment of the sorting arrow.</para>
            /// </summary>
            [NonSerialized]
            public TextAlignment sortingArrowAlignment = TextAlignment.Center;
            /// <summary>
            /// <para>The width of the column.</para>
            /// </summary>
            [SerializeField]
            public float width = 50f;
        }
    }
}

