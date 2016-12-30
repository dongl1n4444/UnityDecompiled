namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>The MultiColumnHeader is a general purpose class that e.g can be used with the TreeView to create multi-column tree views and list views.</para>
    /// </summary>
    public class MultiColumnHeader
    {
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MultiColumnHeaderState.Column, float> <>f__am$cache1;
        private bool m_CanSort = true;
        private Rect[] m_ColumnRects;
        private float m_DividerWidth = 6f;
        private GUIView m_GUIView;
        private float m_Height = DefaultGUI.defaultHeight;
        private Rect m_PreviousRect;
        private bool m_ResizeToFit = false;
        private MultiColumnHeaderState m_State;

        public event HeaderCallback sortingChanged;

        public event HeaderCallback visibleColumnsChanged;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="state">Column header state and Column state.</param>
        public MultiColumnHeader(MultiColumnHeaderState state)
        {
            this.m_State = state;
        }

        /// <summary>
        /// <para>Override this method to extend the default context menu items shown when context clicking the header area.</para>
        /// </summary>
        /// <param name="menu">Context menu shown.</param>
        protected virtual void AddColumnHeaderContextMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Resize to Fit"), false, new GenericMenu.MenuFunction(this.ResizeToFit));
            menu.AddSeparator("");
            for (int i = 0; i < this.state.columns.Length; i++)
            {
                MultiColumnHeaderState.Column column = this.state.columns[i];
                string text = string.IsNullOrEmpty(column.contextMenuText) ? column.headerContent.text : column.contextMenuText;
                if (column.allowToggleVisibility)
                {
                    menu.AddItem(new GUIContent(text), this.state.visibleColumns.Contains<int>(i), new GenericMenu.MenuFunction2(this.ToggleVisibility), i);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(text));
                }
            }
        }

        protected virtual void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
        {
            if (this.state.sortedColumnIndex == columnIndex)
            {
                column.sortedAscending = !column.sortedAscending;
            }
            else
            {
                this.state.sortedColumnIndex = columnIndex;
            }
            this.OnSortingChanged();
        }

        protected virtual void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            if (this.canSort && column.canSort)
            {
                this.SortingButton(column, headerRect, columnIndex);
            }
            GUIStyle style = this.GetStyle(column.headerTextAlignment);
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            Rect position = new Rect(headerRect.x, (headerRect.yMax - singleLineHeight) - DefaultGUI.labelSpaceFromBottom, headerRect.width, singleLineHeight);
            GUI.Label(position, column.headerContent, style);
        }

        private void DetectSizeChanges(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.m_PreviousRect.width > 0f)
                {
                    float deltaWidth = Mathf.Round(rect.width - this.m_PreviousRect.width);
                    if (deltaWidth != 0f)
                    {
                        float fixedWidth = GUI.skin.verticalScrollbar.fixedWidth;
                        if (((rect.width - fixedWidth) > this.state.widthOfAllVisibleColumns) || (deltaWidth < 0f))
                        {
                            this.ResizeColumnsWidthsProportionally(deltaWidth);
                        }
                    }
                }
                this.m_PreviousRect = rect;
            }
        }

        private void DoContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            this.AddColumnHeaderContextMenuItems(menu);
            menu.ShowAsContext();
        }

        internal virtual void DrawColumnResizing(Rect headerRect, MultiColumnHeaderState.Column column)
        {
            headerRect.y++;
            headerRect.width--;
            headerRect.height -= 2f;
            EditorGUI.DrawRect(headerRect, new Color(0.5f, 0.5f, 0.5f, 0.1f));
        }

        internal virtual void DrawDivider(Rect dividerRect, MultiColumnHeaderState.Column column)
        {
            EditorGUI.DrawRect(dividerRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        internal virtual Rect GetArrowRect(MultiColumnHeaderState.Column column, Rect headerRect)
        {
            float fixedWidth = DefaultStyles.arrowStyle.fixedWidth;
            float y = headerRect.y;
            float f = 0f;
            switch (column.sortingArrowAlignment)
            {
                case TextAlignment.Left:
                    f = headerRect.x + DefaultStyles.columnHeader.padding.left;
                    break;

                case TextAlignment.Center:
                    f = (headerRect.x + (headerRect.width * 0.5f)) - (fixedWidth * 0.5f);
                    break;

                case TextAlignment.Right:
                    f = (headerRect.xMax - DefaultStyles.columnHeader.padding.right) - fixedWidth;
                    break;

                default:
                    Debug.LogError("Unhandled enum");
                    break;
            }
            return new Rect(Mathf.Round(f), y, fixedWidth, 16f);
        }

        /// <summary>
        /// <para>Calculates a cell rect for a column and row using the visibleColumnIndex and rowRect parameters.</para>
        /// </summary>
        /// <param name="visibleColumnIndex"></param>
        /// <param name="rowRect"></param>
        public Rect GetCellRect(int visibleColumnIndex, Rect rowRect)
        {
            Rect columnRect = this.GetColumnRect(visibleColumnIndex);
            columnRect.y = rowRect.y;
            columnRect.height = rowRect.height;
            return columnRect;
        }

        /// <summary>
        /// <para>Returns the column data for a given column index.</para>
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>
        /// <para>Column data.</para>
        /// </returns>
        public MultiColumnHeaderState.Column GetColumn(int columnIndex)
        {
            if ((columnIndex < 0) || (columnIndex >= this.state.columns.Length))
            {
                throw new ArgumentOutOfRangeException("columnIndex", $"columnIndex {columnIndex} is not valid when the current column count is {this.state.columns.Length}");
            }
            return this.state.columns[columnIndex];
        }

        /// <summary>
        /// <para>Returns the header column Rect for a given visible column index.</para>
        /// </summary>
        /// <param name="visibleColumnIndex">Index of a visible column.</param>
        public Rect GetColumnRect(int visibleColumnIndex)
        {
            if ((visibleColumnIndex < 0) || (visibleColumnIndex >= this.m_ColumnRects.Length))
            {
                throw new ArgumentException($"The provided visibleColumnIndex is invalid. Ensure the index ({visibleColumnIndex}) is within the number of visible columns ({this.m_ColumnRects.Length})", "visibleColumnIndex");
            }
            return this.m_ColumnRects[visibleColumnIndex];
        }

        private GUIStyle GetStyle(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Left:
                    return DefaultStyles.columnHeader;

                case TextAlignment.Center:
                    return DefaultStyles.columnHeaderCenterAligned;

                case TextAlignment.Right:
                    return DefaultStyles.columnHeaderRightAligned;
            }
            return DefaultStyles.columnHeader;
        }

        /// <summary>
        /// <para>Convert from column index to visible column index.</para>
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>
        /// <para>Visible column index.</para>
        /// </returns>
        public int GetVisibleColumnIndex(int columnIndex)
        {
            for (int i = 0; i < this.state.visibleColumns.Length; i++)
            {
                if (this.state.visibleColumns[i] == columnIndex)
                {
                    return i;
                }
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = t => t.ToString();
            }
            string str = string.Join(", ", Enumerable.Select<int, string>(this.state.visibleColumns, <>f__am$cache0).ToArray<string>());
            throw new ArgumentException($"Invalid columnIndex: {columnIndex}. The index is not part of the current visible columns: {str}", "columnIndex");
        }

        /// <summary>
        /// <para>Check if a column is currently visible in the MultiColumnHeader.</para>
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        public bool IsColumnVisible(int columnIndex)
        {
            <IsColumnVisible>c__AnonStorey0 storey = new <IsColumnVisible>c__AnonStorey0 {
                columnIndex = columnIndex
            };
            return Enumerable.Any<int>(this.state.visibleColumns, new Func<int, bool>(storey.<>m__0));
        }

        /// <summary>
        /// <para>Check the sorting direction state for a column.</para>
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>
        /// <para>True if sorted ascending.</para>
        /// </returns>
        public bool IsSortedAscending(int columnIndex) => 
            this.GetColumn(columnIndex).sortedAscending;

        /// <summary>
        /// <para>Render and handle input for the MultiColumnHeader at the given rect.</para>
        /// </summary>
        /// <param name="xScroll">Horizontal scroll offset.</param>
        /// <param name="rect">Rect where the MultiColumnHeader is drawn in.</param>
        public virtual void OnGUI(Rect rect, float xScroll)
        {
            Event current = Event.current;
            if (this.m_GUIView == null)
            {
                this.m_GUIView = GUIView.current;
            }
            this.DetectSizeChanges(rect);
            if (this.m_ResizeToFit && (current.type == EventType.Repaint))
            {
                this.m_ResizeToFit = false;
                this.ResizeColumnsWidthsProportionally((rect.width - GUI.skin.verticalScrollbar.fixedWidth) - this.state.widthOfAllVisibleColumns);
            }
            GUIClip.Push(rect, new Vector2(-xScroll, 0f), Vector2.zero, false);
            Rect totalHeaderRect = new Rect(0f, 0f, rect.width, rect.height);
            float widthOfAllVisibleColumns = this.state.widthOfAllVisibleColumns;
            float width = ((totalHeaderRect.width <= widthOfAllVisibleColumns) ? widthOfAllVisibleColumns : totalHeaderRect.width) + GUI.skin.verticalScrollbar.fixedWidth;
            Rect position = new Rect(0f, 0f, width, totalHeaderRect.height);
            GUI.Label(position, GUIContent.none, DefaultStyles.background);
            if ((current.type == EventType.ContextClick) && position.Contains(current.mousePosition))
            {
                current.Use();
                this.DoContextMenu();
            }
            this.UpdateColumnHeaderRects(totalHeaderRect);
            for (int i = 0; i < this.state.visibleColumns.Length; i++)
            {
                bool flag;
                int index = this.state.visibleColumns[i];
                MultiColumnHeaderState.Column column = this.state.columns[index];
                Rect headerRect = this.m_ColumnRects[i];
                Rect dividerRect = new Rect(headerRect.xMax - 1f, headerRect.y + 4f, 1f, headerRect.height - 8f);
                Rect rect6 = new Rect(dividerRect.x - (this.m_DividerWidth * 0.5f), totalHeaderRect.y, this.m_DividerWidth, totalHeaderRect.height);
                column.width = EditorGUI.WidthResizer(rect6, column.width, column.minWidth, column.maxWidth, out flag);
                if (flag && (current.type == EventType.Repaint))
                {
                    this.DrawColumnResizing(headerRect, column);
                }
                this.DrawDivider(dividerRect, column);
                this.ColumnHeaderGUI(column, headerRect, index);
            }
            GUIClip.Pop();
        }

        /// <summary>
        /// <para>Called when sorting changes and dispatches the sortingChanged event.</para>
        /// </summary>
        protected virtual void OnSortingChanged()
        {
            if (this.sortingChanged != null)
            {
                this.sortingChanged(this);
            }
        }

        /// <summary>
        /// <para>Called when the number of visible column changes and dispatches the visibleColumnsChanged event.</para>
        /// </summary>
        protected virtual void OnVisibleColumnsChanged()
        {
            if (this.visibleColumnsChanged != null)
            {
                this.visibleColumnsChanged(this);
            }
        }

        /// <summary>
        /// <para>Requests the window which contains the MultiColumnHeader to repaint.</para>
        /// </summary>
        public void Repaint()
        {
            if (this.m_GUIView != null)
            {
                this.m_GUIView.Repaint();
            }
        }

        private void ResizeColumnsWidthsProportionally(float deltaWidth)
        {
            List<MultiColumnHeaderState.Column> list = null;
            foreach (int num in this.state.visibleColumns)
            {
                MultiColumnHeaderState.Column item = this.state.columns[num];
                if ((item.autoResize && ((deltaWidth <= 0f) || (item.width < item.maxWidth))) && ((deltaWidth >= 0f) || (item.width > item.minWidth)))
                {
                    if (list == null)
                    {
                        list = new List<MultiColumnHeaderState.Column>();
                    }
                    list.Add(item);
                }
            }
            if (list != null)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = x => x.width;
                }
                float num3 = Enumerable.Sum<MultiColumnHeaderState.Column>(list, <>f__am$cache1);
                foreach (MultiColumnHeaderState.Column column2 in list)
                {
                    column2.width += deltaWidth * (column2.width / num3);
                    column2.width = Mathf.Clamp(column2.width, column2.minWidth, column2.maxWidth);
                }
            }
        }

        /// <summary>
        /// <para>Resizes the column widths of the columns that have auto-resize enabled to make all the columns fit to the width of the MultiColumnHeader render rect.</para>
        /// </summary>
        public void ResizeToFit()
        {
            this.m_ResizeToFit = true;
            this.Repaint();
        }

        /// <summary>
        /// <para>Change sort direction for a given column.</para>
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="sortAscending">Direction of the sorting.</param>
        public void SetSortDirection(int columnIndex, bool sortAscending)
        {
            MultiColumnHeaderState.Column column = this.GetColumn(columnIndex);
            if (column.sortedAscending != sortAscending)
            {
                column.sortedAscending = sortAscending;
                this.OnSortingChanged();
            }
        }

        /// <summary>
        /// <para>Set both the curent sorting column and its sorting direction.</para>
        /// </summary>
        /// <param name="columnIndex">Column to sort.</param>
        /// <param name="sortAscending">Sorting direction for the column specified.</param>
        public void SetSorting(int columnIndex, bool sortAscending)
        {
            bool flag = false;
            if (this.state.sortedColumnIndex != columnIndex)
            {
                this.state.sortedColumnIndex = columnIndex;
                flag = true;
            }
            MultiColumnHeaderState.Column column = this.GetColumn(columnIndex);
            if (column.sortedAscending != sortAscending)
            {
                column.sortedAscending = sortAscending;
                flag = true;
            }
            if (flag)
            {
                this.OnSortingChanged();
            }
        }

        protected void SortingButton(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            if (EditorGUI.Button(headerRect, GUIContent.none, GUIStyle.none))
            {
                this.ColumnHeaderClicked(column, columnIndex);
            }
            if ((columnIndex == this.state.sortedColumnIndex) && (Event.current.type == EventType.Repaint))
            {
                Rect arrowRect = this.GetArrowRect(column, headerRect);
                Matrix4x4 matrix = GUI.matrix;
                if (column.sortedAscending)
                {
                    GUIUtility.RotateAroundPivot(180f, arrowRect.center - new Vector2(0f, 1f));
                }
                GUI.Label(arrowRect, "▾", DefaultStyles.arrowStyle);
                if (column.sortedAscending)
                {
                    GUI.matrix = matrix;
                }
            }
        }

        /// <summary>
        /// <para>Method for toggling the visibility of a column.</para>
        /// </summary>
        /// <param name="columnIndex">Toggle visibility for this column.</param>
        protected virtual void ToggleVisibility(int columnIndex)
        {
            List<int> list = new List<int>(this.state.visibleColumns);
            if (list.Contains(columnIndex))
            {
                list.Remove(columnIndex);
            }
            else
            {
                list.Add(columnIndex);
                list.Sort();
            }
            this.state.visibleColumns = list.ToArray();
            this.Repaint();
            this.OnVisibleColumnsChanged();
        }

        private void ToggleVisibility(object userData)
        {
            this.ToggleVisibility((int) userData);
        }

        private void UpdateColumnHeaderRects(Rect totalHeaderRect)
        {
            if ((this.m_ColumnRects == null) || (this.m_ColumnRects.Length != this.state.visibleColumns.Length))
            {
                this.m_ColumnRects = new Rect[this.state.visibleColumns.Length];
            }
            Rect rect = totalHeaderRect;
            for (int i = 0; i < this.state.visibleColumns.Length; i++)
            {
                int index = this.state.visibleColumns[i];
                MultiColumnHeaderState.Column column = this.state.columns[index];
                if (i > 0)
                {
                    rect.x += rect.width;
                }
                rect.width = column.width;
                this.m_ColumnRects[i] = rect;
            }
        }

        /// <summary>
        /// <para>Use this property to control whether sorting is enabled for all the columns.</para>
        /// </summary>
        public bool canSort
        {
            get => 
                this.m_CanSort;
            set
            {
                this.m_CanSort = value;
                this.height = this.m_Height;
            }
        }

        /// <summary>
        /// <para>Customizable height of the multi column header.</para>
        /// </summary>
        public float height
        {
            get => 
                this.m_Height;
            set
            {
                this.m_Height = value;
            }
        }

        /// <summary>
        /// <para>The index of the previous sorted column (if any).</para>
        /// </summary>
        public int previousSortedColumnIndex =>
            this.state.previousSortedColumnIndex;

        /// <summary>
        /// <para>The index of the column that is set to be the primary sorting column. This is the column that shows the sorting arrow above the header text.</para>
        /// </summary>
        public int sortedColumnIndex
        {
            get => 
                this.state.sortedColumnIndex;
            set
            {
                if (value != this.state.sortedColumnIndex)
                {
                    this.state.sortedColumnIndex = value;
                    this.OnSortingChanged();
                }
            }
        }

        /// <summary>
        /// <para>This is the state of the MultiColumnHeader.</para>
        /// </summary>
        public MultiColumnHeaderState state
        {
            get => 
                this.m_State;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("state", "MultiColumnHeader state is not allowed to be null");
                }
                this.m_State = value;
            }
        }

        [CompilerGenerated]
        private sealed class <IsColumnVisible>c__AnonStorey0
        {
            internal int columnIndex;

            internal bool <>m__0(int t) => 
                (t == this.columnIndex);
        }

        /// <summary>
        /// <para>Default GUI methods and properties for the MultiColumnHeader class.</para>
        /// </summary>
        public static class DefaultGUI
        {
            /// <summary>
            /// <para>Margin that can be used by clients of the MultiColumnHeader to control spacing between content in multiple columns.</para>
            /// </summary>
            public static float columnContentMargin =>
                3f;

            /// <summary>
            /// <para>Default height of the header.</para>
            /// </summary>
            public static float defaultHeight =>
                27f;

            internal static float labelSpaceFromBottom =>
                3f;

            /// <summary>
            /// <para>This height is the minium height the header can have and can only be used if sorting is disabled.</para>
            /// </summary>
            public static float minimumHeight =>
                20f;
        }

        /// <summary>
        /// <para>Default styles used by the MultiColumnHeader class.</para>
        /// </summary>
        public static class DefaultStyles
        {
            internal static GUIStyle arrowStyle;
            /// <summary>
            /// <para>Style used for rendering the background of the header.</para>
            /// </summary>
            public static GUIStyle background = new GUIStyle("ProjectBrowserTopBarBg");
            /// <summary>
            /// <para>Style used for left aligned header text.</para>
            /// </summary>
            public static GUIStyle columnHeader;
            /// <summary>
            /// <para>Style used for centered header text.</para>
            /// </summary>
            public static GUIStyle columnHeaderCenterAligned;
            /// <summary>
            /// <para>Style used for right aligned header text.</para>
            /// </summary>
            public static GUIStyle columnHeaderRightAligned;

            static DefaultStyles()
            {
                background.fixedHeight = 0f;
                background.border = new RectOffset(3, 3, 3, 3);
                columnHeader = new GUIStyle(EditorStyles.label);
                columnHeader.alignment = TextAnchor.MiddleLeft;
                columnHeader.padding = new RectOffset(4, 4, 0, 0);
                Color textColor = columnHeader.normal.textColor;
                textColor.a = 0.8f;
                columnHeader.normal.textColor = textColor;
                columnHeaderRightAligned = new GUIStyle(columnHeader);
                columnHeaderRightAligned.alignment = TextAnchor.MiddleRight;
                columnHeaderCenterAligned = new GUIStyle(columnHeader);
                columnHeaderCenterAligned.alignment = TextAnchor.MiddleCenter;
                arrowStyle = new GUIStyle(EditorStyles.label);
                arrowStyle.padding = new RectOffset();
                arrowStyle.fixedWidth = 13f;
                arrowStyle.alignment = TextAnchor.UpperCenter;
            }
        }

        /// <summary>
        /// <para>Delegate used for events from the MultiColumnHeader.</para>
        /// </summary>
        /// <param name="multiColumnHeader">The MultiColumnHeader that dispatched this event.</param>
        public delegate void HeaderCallback(MultiColumnHeader multiColumnHeader);
    }
}

