namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    internal class MultiColumnHeader
    {
        [CompilerGenerated]
        private static Func<MultiColumnHeaderState.Column, float> <>f__am$cache0;
        private bool m_CanSort = true;
        private Rect[] m_ColumnRects;
        private float m_DividerWidth = 6f;
        private GUIView m_GUIView;
        private float m_Height = DefaultGUI.defaultHeight;
        private Rect m_PreviousRect;
        private MultiColumnHeaderState m_State;

        public event HeaderCallback sortingChanged;

        public event HeaderCallback visibleColumnsChanged;

        public MultiColumnHeader(MultiColumnHeaderState state)
        {
            this.m_State = state;
        }

        protected virtual void AddColumnVisibilityItems(GenericMenu menu)
        {
            for (int i = 0; i < this.state.columns.Length; i++)
            {
                MultiColumnHeaderState.Column column = this.state.columns[i];
                if (column.allowToggleVisibility)
                {
                    menu.AddItem(new GUIContent(column.headerText), Enumerable.Contains<int>(this.state.visibleColumns, i), new GenericMenu.MenuFunction2(this.ToggleVisibility), i);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(column.headerText));
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
            if (this.canSort)
            {
                this.SortingButton(column, headerRect, columnIndex);
            }
            GUIStyle columnHeader = DefaultStyles.columnHeader;
            columnHeader.alignment = this.ConvertHeaderAlignmentToTextAnchor(column.headerTextAlignment);
            GUI.Label(headerRect, column.headerText, columnHeader);
        }

        protected TextAnchor ConvertHeaderAlignmentToTextAnchor(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Left:
                    return TextAnchor.LowerLeft;

                case TextAlignment.Center:
                    return TextAnchor.LowerCenter;

                case TextAlignment.Right:
                    return TextAnchor.LowerRight;
            }
            return TextAnchor.LowerLeft;
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

        protected virtual void DoContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            this.AddColumnVisibilityItems(menu);
            menu.ShowAsContext();
        }

        protected virtual void DrawDivider(Rect dividerRect)
        {
            EditorGUI.DrawRect(dividerRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        public Rect GetCellRect(int visibleColumnIndex, Rect rowRect)
        {
            Rect columnRect = this.GetColumnRect(visibleColumnIndex);
            columnRect.y = rowRect.y;
            columnRect.height = rowRect.height;
            return columnRect;
        }

        public MultiColumnHeaderState.Column GetColumn(int columnIndex)
        {
            if ((columnIndex < 0) || (columnIndex >= this.state.columns.Length))
            {
                throw new ArgumentOutOfRangeException("columnIndex", string.Format("columnIndex {0} is not valid when the current column count is {1}", columnIndex, this.state.columns.Length));
            }
            return this.state.columns[columnIndex];
        }

        public Rect GetColumnRect(int visibleColumnIndex)
        {
            if ((visibleColumnIndex < 0) || (visibleColumnIndex >= this.m_ColumnRects.Length))
            {
                throw new ArgumentException(string.Format("The provided visibleColumnIndex is invalid. Ensure the index ({0}) is within the number of visible columns ({1})", visibleColumnIndex, this.m_ColumnRects.Length), "visibleColumnIndex");
            }
            return this.m_ColumnRects[visibleColumnIndex];
        }

        public int GetVisibleColumnIndex(int columnIndex)
        {
            for (int i = 0; i < this.state.visibleColumns.Length; i++)
            {
                if (this.state.visibleColumns[i] == columnIndex)
                {
                    return i;
                }
            }
            throw new ArgumentException("Invalid columnIndex: it is not part of the visible columns", "columnIndex");
        }

        public bool IsSortedAscending(int columnIndex)
        {
            return this.GetColumn(columnIndex).sortedAscending;
        }

        public virtual void OnGUI(Rect rect, float xScroll)
        {
            Event current = Event.current;
            if (this.m_GUIView == null)
            {
                this.m_GUIView = GUIView.current;
            }
            this.DetectSizeChanges(rect);
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
                int index = this.state.visibleColumns[i];
                MultiColumnHeaderState.Column column = this.state.columns[index];
                Rect headerRect = this.m_ColumnRects[i];
                Rect dividerRect = new Rect(headerRect.xMax - 1f, headerRect.y + 4f, 1f, headerRect.height - 8f);
                this.DrawDivider(dividerRect);
                Rect rect6 = new Rect(dividerRect.x - (this.m_DividerWidth * 0.5f), totalHeaderRect.y, this.m_DividerWidth, totalHeaderRect.height);
                column.width = EditorGUI.WidthResizer(rect6, column.width, column.minWidth, column.maxWidth);
                this.ColumnHeaderGUI(column, headerRect, index);
            }
            GUIClip.Pop();
        }

        protected virtual void OnSortingChanged()
        {
            if (this.sortingChanged != null)
            {
                this.sortingChanged(this);
            }
        }

        protected virtual void OnVisibleColumnsChanged()
        {
            if (this.visibleColumnsChanged != null)
            {
                this.visibleColumnsChanged(this);
            }
        }

        public void Repaint()
        {
            this.m_GUIView.Repaint();
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
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<MultiColumnHeaderState.Column, float>(null, (IntPtr) <ResizeColumnsWidthsProportionally>m__0);
                }
                float num3 = Enumerable.Sum<MultiColumnHeaderState.Column>(list, <>f__am$cache0);
                foreach (MultiColumnHeaderState.Column column2 in list)
                {
                    column2.width += deltaWidth * (column2.width / num3);
                    column2.width = Mathf.Clamp(column2.width, column2.minWidth, column2.maxWidth);
                }
            }
        }

        public void SetSortDirection(int columnIndex, bool sortAscending)
        {
            MultiColumnHeaderState.Column column = this.GetColumn(columnIndex);
            if (column.sortedAscending != sortAscending)
            {
                column.sortedAscending = sortAscending;
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
                Rect position = new Rect(Mathf.Round(f), y, fixedWidth, 12f);
                Matrix4x4 matrix = GUI.matrix;
                if (column.sortedAscending)
                {
                    GUIUtility.RotateAroundPivot(180f, position.center);
                }
                GUI.Label(position, "▾", DefaultStyles.arrowStyle);
                if (column.sortedAscending)
                {
                    GUI.matrix = matrix;
                }
            }
        }

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

        public bool canSort
        {
            get
            {
                return this.m_CanSort;
            }
            set
            {
                this.m_CanSort = value;
                this.height = this.m_Height;
            }
        }

        public float height
        {
            get
            {
                return this.m_Height;
            }
            set
            {
                this.m_Height = value;
                this.m_Height = Mathf.Max(this.m_Height, !this.m_CanSort ? DefaultGUI.minimumHeight : DefaultGUI.defaultHeight);
            }
        }

        public int previousSortedColumnIndex
        {
            get
            {
                return this.state.previousSortedColumnIndex;
            }
        }

        public int sortedColumnIndex
        {
            get
            {
                return this.state.sortedColumnIndex;
            }
            set
            {
                if (value != this.state.sortedColumnIndex)
                {
                    this.state.sortedColumnIndex = value;
                    this.OnSortingChanged();
                }
            }
        }

        public MultiColumnHeaderState state
        {
            get
            {
                return this.m_State;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("state", "MultiColumnHeader state is not allowed to be null");
                }
                this.m_State = value;
            }
        }

        public static class DefaultGUI
        {
            public static float defaultHeight
            {
                get
                {
                    return 27f;
                }
            }

            public static float minimumHeight
            {
                get
                {
                    return 20f;
                }
            }
        }

        public static class DefaultStyles
        {
            public static GUIStyle arrowStyle;
            public static GUIStyle background = new GUIStyle("ProjectBrowserTopBarBg");
            public static GUIStyle columnHeader;
            public static GUIStyle columnHeaderRightAligned;

            static DefaultStyles()
            {
                background.fixedHeight = 0f;
                background.border = new RectOffset(3, 3, 3, 3);
                columnHeader = new GUIStyle(EditorStyles.label);
                columnHeader.padding = new RectOffset(4, 4, 4, 4);
                Color textColor = columnHeader.normal.textColor;
                textColor.a = 0.8f;
                columnHeader.normal.textColor = textColor;
                arrowStyle = new GUIStyle(EditorStyles.label);
                arrowStyle.padding = new RectOffset();
                arrowStyle.fixedWidth = 13f;
                arrowStyle.alignment = TextAnchor.UpperCenter;
            }
        }

        public delegate void HeaderCallback(MultiColumnHeader multiColumnHeader);
    }
}

