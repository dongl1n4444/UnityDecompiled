namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    internal class TreeViewController
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<int> <contextClickItemCallback>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private System.Action <contextClickOutsideItemsCallback>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ITreeViewDataSource <data>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <deselectOnUnhandledMouseDown>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Action<int[], bool> <dragEndedCallback>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private ITreeViewDragging <dragging>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private System.Action <expandedStateChanged>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private ITreeViewGUI <gui>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GUIStyle <horizontalScrollbarStyle>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<int> <itemDoubleClickedCallback>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private System.Action <keyboardInputCallback>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Action<int, Rect> <onGUIRowCallback>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Action<Vector2> <scrollChanged>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> <searchChanged>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<int[]> <selectionChangedCallback>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private TreeViewState <state>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private GUIStyle <verticalScrollbarStyle>k__BackingField;
        internal const string kExpansionAnimationPrefKey = "TreeViewExpansionAnimation";
        private const float kSpaceForScrollBar = 16f;
        private bool m_AllowRenameOnMouseUp = true;
        private Rect m_ContentRect;
        private List<int> m_DragSelection = new List<int>();
        private readonly TreeViewItemExpansionAnimator m_ExpansionAnimator = new TreeViewItemExpansionAnimator();
        private AnimFloat m_FramingAnimFloat;
        private bool m_GrabKeyboardFocus;
        private GUIView m_GUIView;
        private bool m_HadFocusLastEvent;
        private int m_KeyboardControlID;
        private bool m_StopIteratingItems;
        private Rect m_TotalRect;
        private bool m_UseExpansionAnimation = EditorPrefs.GetBool("TreeViewExpansionAnimation", true);
        private bool m_UseScrollView = true;
        private Rect m_VisibleRect;

        public TreeViewController(EditorWindow editorWindow, TreeViewState treeViewState)
        {
            this.m_GUIView = (editorWindow == null) ? GUIView.current : editorWindow.m_Parent;
            this.state = treeViewState;
        }

        private void AnimatedScrollChanged()
        {
            this.Repaint();
            this.state.scrollPos.y = this.m_FramingAnimFloat.value;
        }

        public bool BeginNameEditing(float delay)
        {
            if (this.state.selectedIDs.Count == 0)
            {
                return false;
            }
            IList<TreeViewItem> rows = this.data.GetRows();
            TreeViewItem item = null;
            using (List<int>.Enumerator enumerator = this.state.selectedIDs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <BeginNameEditing>c__AnonStorey0 storey = new <BeginNameEditing>c__AnonStorey0 {
                        id = enumerator.Current
                    };
                    TreeViewItem item2 = Enumerable.FirstOrDefault<TreeViewItem>(rows, new Func<TreeViewItem, bool>(storey.<>m__0));
                    if (item == null)
                    {
                        item = item2;
                    }
                    else if (item2 != null)
                    {
                        return false;
                    }
                }
            }
            return (((item != null) && this.data.IsRenamingItemAllowed(item)) && this.gui.BeginRename(item, delay));
        }

        private void ChangeExpandedState(TreeViewItem item, bool expand, bool includeChildren)
        {
            if (includeChildren)
            {
                this.data.SetExpandedWithChildren(item, expand);
            }
            else
            {
                this.data.SetExpanded(item, expand);
            }
        }

        private void ChangeFolding(int[] ids, bool expand)
        {
            if (ids.Length == 1)
            {
                this.ChangeFoldingForSingleItem(ids[0], expand);
            }
            else if (ids.Length > 1)
            {
                this.ChangeFoldingForMultipleItems(ids, expand);
            }
        }

        private void ChangeFoldingForMultipleItems(int[] ids, bool expand)
        {
            HashSet<int> other = new HashSet<int>();
            foreach (int num in ids)
            {
                int num3;
                TreeViewItem itemAndRowIndex = this.GetItemAndRowIndex(num, out num3);
                if ((itemAndRowIndex != null) && (this.data.IsExpandable(itemAndRowIndex) && (this.data.IsExpanded(itemAndRowIndex) != expand)))
                {
                    other.Add(num);
                }
            }
            if (Event.current.alt)
            {
                foreach (int num4 in other)
                {
                    this.data.SetExpandedWithChildren(num4, expand);
                }
            }
            else
            {
                HashSet<int> source = new HashSet<int>(this.data.GetExpandedIDs());
                if (expand)
                {
                    source.UnionWith(other);
                }
                else
                {
                    source.ExceptWith(other);
                }
                this.data.SetExpandedIDs(source.ToArray<int>());
            }
        }

        private void ChangeFoldingForSingleItem(int id, bool expand)
        {
            int num;
            TreeViewItem itemAndRowIndex = this.GetItemAndRowIndex(id, out num);
            if (itemAndRowIndex != null)
            {
                if (this.data.IsExpandable(itemAndRowIndex) && (this.data.IsExpanded(itemAndRowIndex) != expand))
                {
                    this.UserInputChangedExpandedState(itemAndRowIndex, num, expand);
                }
                else
                {
                    this.expansionAnimator.SkipAnimating();
                    if (expand)
                    {
                        this.HandleFastExpand(itemAndRowIndex, num);
                    }
                    else
                    {
                        this.HandleFastCollapse(itemAndRowIndex, num);
                    }
                }
            }
        }

        private void ChangeScrollValue(float targetScrollPos, bool animated)
        {
            if (this.m_UseExpansionAnimation && animated)
            {
                this.m_FramingAnimFloat.value = this.state.scrollPos.y;
                this.m_FramingAnimFloat.target = targetScrollPos;
                this.m_FramingAnimFloat.speed = 3f;
            }
            else
            {
                this.state.scrollPos.y = targetScrollPos;
            }
        }

        private void DoItemGUI(TreeViewItem item, int row, float rowWidth, bool hasFocus)
        {
            if ((row < 0) || (row >= this.data.rowCount))
            {
                UnityEngine.Debug.LogError(string.Concat(new object[] { "Invalid. Org row: ", row, " Num rows ", this.data.rowCount }));
            }
            else
            {
                bool selected = this.IsItemDragSelectedOrSelected(item);
                Rect rowRect = this.gui.GetRowRect(row, rowWidth);
                if (this.animatingExpansion)
                {
                    rowRect = this.m_ExpansionAnimator.OnBeginRowGUI(row, rowRect);
                }
                if (this.animatingExpansion)
                {
                    this.m_ExpansionAnimator.OnRowGUI(row);
                }
                this.gui.OnRowGUI(rowRect, item, row, selected, hasFocus);
                if (this.animatingExpansion)
                {
                    this.m_ExpansionAnimator.OnEndRowGUI(row);
                }
                if (this.onGUIRowCallback != null)
                {
                    float contentIndent = this.gui.GetContentIndent(item);
                    Rect rect2 = new Rect(rowRect.x + contentIndent, rowRect.y, rowRect.width - contentIndent, rowRect.height);
                    this.onGUIRowCallback(item.id, rect2);
                }
                this.HandleUnusedMouseEventsForItem(rowRect, item, row);
            }
        }

        public void EndNameEditing(bool acceptChanges)
        {
            if (this.state.renameOverlay.IsRenaming())
            {
                this.state.renameOverlay.EndRename(acceptChanges);
                this.gui.EndRename();
            }
        }

        public void EndPing()
        {
            this.gui.EndPingItem();
        }

        private void EnsureRowIsVisible(int row, bool animated)
        {
            if (row >= 0)
            {
                float num = (this.m_VisibleRect.height <= 0f) ? this.m_TotalRect.height : this.m_VisibleRect.height;
                Rect rectForFraming = this.gui.GetRectForFraming(row);
                float y = rectForFraming.y;
                float targetScrollPos = rectForFraming.yMax - num;
                if (this.state.scrollPos.y < targetScrollPos)
                {
                    this.ChangeScrollValue(targetScrollPos, animated);
                }
                else if (this.state.scrollPos.y > y)
                {
                    this.ChangeScrollValue(y, animated);
                }
            }
        }

        private void ExpandedStateHasChanged()
        {
            this.m_StopIteratingItems = true;
        }

        private void ExpansionAnimationEnded(TreeViewAnimationInput setup)
        {
            if (!setup.expanding)
            {
                this.ChangeExpandedState(setup.item, false, setup.includeChildren);
            }
        }

        public TreeViewItem FindItem(int id) => 
            this.data.FindItem(id);

        public void Frame(int id, bool frame, bool ping)
        {
            this.Frame(id, frame, ping, false);
        }

        public void Frame(int id, bool frame, bool ping, bool animated)
        {
            float topPixelOfRow = -1f;
            if (frame)
            {
                this.data.RevealItem(id);
                int row = this.data.GetRow(id);
                if (row >= 0)
                {
                    topPixelOfRow = this.GetTopPixelOfRow(row);
                    this.EnsureRowIsVisible(row, animated);
                }
            }
            if (ping)
            {
                int num3 = this.data.GetRow(id);
                if ((topPixelOfRow == -1f) && (num3 >= 0))
                {
                    topPixelOfRow = this.GetTopPixelOfRow(num3);
                }
                if (((topPixelOfRow >= 0f) && (num3 >= 0)) && (num3 < this.data.rowCount))
                {
                    TreeViewItem item = this.data.GetItem(num3);
                    float num4 = (this.GetContentSize().y <= this.m_TotalRect.height) ? 0f : -16f;
                    this.gui.BeginPingItem(item, topPixelOfRow, this.m_TotalRect.width + num4);
                }
            }
        }

        private float GetAnimationDuration(float height) => 
            ((height <= 60f) ? ((height * 0.07f) / 60f) : 0.07f);

        public Vector2 GetContentSize() => 
            this.gui.GetTotalSize();

        private bool GetFirstAndLastSelected(List<TreeViewItem> items, out int firstIndex, out int lastIndex)
        {
            firstIndex = -1;
            lastIndex = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (this.state.selectedIDs.Contains(items[i].id))
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }
                    lastIndex = i;
                }
            }
            return ((firstIndex != -1) && (lastIndex != -1));
        }

        internal static int GetIndexOfID(IList<TreeViewItem> items, int id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        private TreeViewItem GetItemAndRowIndex(int id, out int row)
        {
            row = this.data.GetRow(id);
            if (row == -1)
            {
                return null;
            }
            return this.data.GetItem(row);
        }

        internal static int GetItemControlID(TreeViewItem item) => 
            (((item == null) ? 0 : item.id) + 0x989680);

        private int GetLastChildRowUnder(int row)
        {
            IList<TreeViewItem> rows = this.data.GetRows();
            int depth = rows[row].depth;
            for (int i = row + 1; i < rows.Count; i++)
            {
                if (rows[i].depth <= depth)
                {
                    return (i - 1);
                }
            }
            return (rows.Count - 1);
        }

        private List<int> GetNewSelection(TreeViewItem clickedItem, bool keepMultiSelection, bool useShiftAsActionKey)
        {
            IList<TreeViewItem> rows = this.data.GetRows();
            List<int> allInstanceIDs = new List<int>(rows.Count);
            for (int i = 0; i < rows.Count; i++)
            {
                allInstanceIDs.Add(rows[i].id);
            }
            List<int> selectedIDs = this.state.selectedIDs;
            int lastClickedID = this.state.lastClickedID;
            bool allowMultiSelection = this.data.CanBeMultiSelected(clickedItem);
            return InternalEditorUtility.GetNewSelection(clickedItem.id, allInstanceIDs, selectedIDs, lastClickedID, keepMultiSelection, useShiftAsActionKey, allowMultiSelection);
        }

        protected virtual Rect GetRectForRows(int startRow, int endRow, float rowWidth)
        {
            Rect rowRect = this.gui.GetRowRect(startRow, rowWidth);
            Rect rect2 = this.gui.GetRowRect(endRow, rowWidth);
            return new Rect(rowRect.x, rowRect.y, rowWidth, rect2.yMax - rowRect.yMin);
        }

        public int[] GetRowIDs()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = item => item.id;
            }
            return Enumerable.Select<TreeViewItem, int>(this.data.GetRows(), <>f__am$cache0).ToArray<int>();
        }

        public int[] GetSelection() => 
            this.state.selectedIDs.ToArray();

        private float GetTopPixelOfRow(int row) => 
            this.gui.GetRowRect(row, 1f).y;

        public Rect GetTotalRect() => 
            this.m_TotalRect;

        private List<int> GetVisibleSelectedIds()
        {
            int num;
            int num2;
            this.gui.GetFirstAndLastRowVisible(out num, out num2);
            if (num2 < 0)
            {
                return new List<int>();
            }
            List<int> list2 = new List<int>(num2 - num);
            for (int i = num; i < num2; i++)
            {
                TreeViewItem item = this.data.GetItem(i);
                list2.Add(item.id);
            }
            return (from id in list2
                where this.state.selectedIDs.Contains(id)
                select id).ToList<int>();
        }

        public void GrabKeyboardFocus()
        {
            this.m_GrabKeyboardFocus = true;
        }

        private void HandleFastCollapse(TreeViewItem item, int row)
        {
            if (item.depth == 0)
            {
                for (int i = row - 1; i >= 0; i--)
                {
                    if (this.data.GetItem(i).hasChildren)
                    {
                        this.OffsetSelection(i - row);
                        break;
                    }
                }
            }
            else if (item.depth > 0)
            {
                for (int j = row - 1; j >= 0; j--)
                {
                    if (this.data.GetItem(j).depth < item.depth)
                    {
                        this.OffsetSelection(j - row);
                        break;
                    }
                }
            }
        }

        private void HandleFastExpand(TreeViewItem item, int row)
        {
            int rowCount = this.data.rowCount;
            for (int i = row + 1; i < rowCount; i++)
            {
                if (this.data.GetItem(i).hasChildren)
                {
                    this.OffsetSelection(i - row);
                    break;
                }
            }
        }

        private void HandleUnusedEvents()
        {
            EventType type = Event.current.type;
            if (type == EventType.DragUpdated)
            {
                if ((this.dragging != null) && this.m_TotalRect.Contains(Event.current.mousePosition))
                {
                    this.dragging.DragElement(null, new Rect(), false);
                    this.Repaint();
                    Event.current.Use();
                }
            }
            else if (type == EventType.DragPerform)
            {
                if ((this.dragging != null) && this.m_TotalRect.Contains(Event.current.mousePosition))
                {
                    this.m_DragSelection.Clear();
                    this.dragging.DragElement(null, new Rect(), false);
                    this.Repaint();
                    Event.current.Use();
                }
            }
            else if (type == EventType.DragExited)
            {
                if (this.dragging != null)
                {
                    this.m_DragSelection.Clear();
                    this.dragging.DragCleanup(true);
                    this.Repaint();
                }
            }
            else if (type == EventType.ContextClick)
            {
                if (this.m_TotalRect.Contains(Event.current.mousePosition) && (this.contextClickOutsideItemsCallback != null))
                {
                    this.contextClickOutsideItemsCallback();
                }
            }
            else if ((type == EventType.MouseDown) && ((this.deselectOnUnhandledMouseDown && (Event.current.button == 0)) && (this.m_TotalRect.Contains(Event.current.mousePosition) && (this.state.selectedIDs.Count > 0))))
            {
                this.SetSelection(new int[0], false);
                this.NotifyListenersThatSelectionChanged();
            }
        }

        public void HandleUnusedMouseEventsForItem(Rect rect, TreeViewItem item, int row)
        {
            int itemControlID = GetItemControlID(item);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(itemControlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (!rect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    if (Event.current.button != 0)
                    {
                        if (Event.current.button == 1)
                        {
                            bool keepMultiSelection = true;
                            this.SelectionClick(item, keepMultiSelection);
                        }
                        return;
                    }
                    GUIUtility.keyboardControl = this.m_KeyboardControlID;
                    this.Repaint();
                    if (Event.current.clickCount != 2)
                    {
                        List<int> draggedItemIDs = this.GetNewSelection(item, true, false);
                        if (((this.dragging != null) && (draggedItemIDs.Count != 0)) && this.dragging.CanStartDrag(item, draggedItemIDs, Event.current.mousePosition))
                        {
                            this.m_DragSelection = draggedItemIDs;
                            DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
                            stateObject.mouseDownPosition = Event.current.mousePosition;
                        }
                        else
                        {
                            this.m_DragSelection.Clear();
                            if (this.m_AllowRenameOnMouseUp)
                            {
                                this.m_AllowRenameOnMouseUp = (this.state.selectedIDs.Count == 1) && (this.state.selectedIDs[0] == item.id);
                            }
                            this.SelectionClick(item, false);
                        }
                        GUIUtility.hotControl = itemControlID;
                    }
                    else if (this.itemDoubleClickedCallback != null)
                    {
                        this.itemDoubleClickedCallback(item.id);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == itemControlID)
                    {
                        bool flag3 = this.m_DragSelection.Count > 0;
                        GUIUtility.hotControl = 0;
                        this.m_DragSelection.Clear();
                        current.Use();
                        if (rect.Contains(current.mousePosition))
                        {
                            Rect rect2 = this.gui.GetRenameRect(rect, row, item);
                            List<int> selectedIDs = this.state.selectedIDs;
                            if (((!this.m_AllowRenameOnMouseUp || (selectedIDs == null)) || ((selectedIDs.Count != 1) || (selectedIDs[0] != item.id))) || (!rect2.Contains(current.mousePosition) || EditorGUIUtility.HasHolddownKeyModifiers(current)))
                            {
                                if (flag3)
                                {
                                    this.SelectionClick(item, false);
                                }
                                return;
                            }
                            this.BeginNameEditing(0.5f);
                        }
                    }
                    return;

                case EventType.MouseDrag:
                    if (((GUIUtility.hotControl == itemControlID) && (this.dragging != null)) && (this.m_DragSelection.Count > 0))
                    {
                        DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
                        if (delay2.CanStartDrag() && this.dragging.CanStartDrag(item, this.m_DragSelection, delay2.mouseDownPosition))
                        {
                            this.dragging.StartDrag(item, this.m_DragSelection);
                            GUIUtility.hotControl = 0;
                        }
                        current.Use();
                    }
                    return;

                default:
                    switch (typeForControl)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                        {
                            bool firstItem = row == 0;
                            if ((this.dragging != null) && this.dragging.DragElement(item, rect, firstItem))
                            {
                                GUIUtility.hotControl = 0;
                            }
                            return;
                        }
                        case EventType.ContextClick:
                            if (rect.Contains(current.mousePosition) && (this.contextClickItemCallback != null))
                            {
                                this.contextClickItemCallback(item.id);
                            }
                            return;

                        default:
                            return;
                    }
                    break;
            }
            current.Use();
        }

        public bool HasFocus() => 
            (((this.m_GUIView == null) ? EditorGUIUtility.HasCurrentWindowKeyFocus() : this.m_GUIView.hasFocus) && (GUIUtility.keyboardControl == this.m_KeyboardControlID));

        public bool HasSelection() => 
            (this.state.selectedIDs.Count<int>() > 0);

        public void Init(Rect rect, ITreeViewDataSource data, ITreeViewGUI gui, ITreeViewDragging dragging)
        {
            this.data = data;
            this.gui = gui;
            this.dragging = dragging;
            this.m_TotalRect = rect;
            data.OnInitialize();
            gui.OnInitialize();
            if (dragging != null)
            {
                dragging.OnInitialize();
            }
            this.expandedStateChanged = (System.Action) Delegate.Combine(this.expandedStateChanged, new System.Action(this.ExpandedStateHasChanged));
            this.m_FramingAnimFloat = new AnimFloat(this.state.scrollPos.y, new UnityAction(this.AnimatedScrollChanged));
        }

        public bool IsItemDragSelectedOrSelected(TreeViewItem item) => 
            ((this.m_DragSelection.Count <= 0) ? this.state.selectedIDs.Contains(item.id) : this.m_DragSelection.Contains(item.id));

        public bool IsLastClickedPartOfRows()
        {
            IList<TreeViewItem> rows = this.data.GetRows();
            if (rows.Count == 0)
            {
                return false;
            }
            return (GetIndexOfID(rows, this.state.lastClickedID) >= 0);
        }

        public bool IsSelected(int id) => 
            this.state.selectedIDs.Contains(id);

        private void IterateVisibleItems(int firstRow, int numVisibleRows, float rowWidth, bool hasFocus)
        {
            this.m_StopIteratingItems = false;
            int num = 0;
            for (int i = 0; i < numVisibleRows; i++)
            {
                int row = firstRow + i;
                if (this.animatingExpansion)
                {
                    int endRow = this.m_ExpansionAnimator.endRow;
                    if (this.m_ExpansionAnimator.CullRow(row, this.gui))
                    {
                        num++;
                        row = endRow + num;
                    }
                    else
                    {
                        row += num;
                    }
                    if (row >= this.data.rowCount)
                    {
                        continue;
                    }
                }
                else
                {
                    float num5 = this.gui.GetRowRect(row, rowWidth).y - this.state.scrollPos.y;
                    if (num5 > this.m_TotalRect.height)
                    {
                        continue;
                    }
                }
                this.DoItemGUI(this.data.GetItem(row), row, rowWidth, hasFocus);
                if (this.m_StopIteratingItems)
                {
                    break;
                }
            }
        }

        private void KeyboardGUI()
        {
            if ((this.m_KeyboardControlID == GUIUtility.keyboardControl) && GUI.enabled)
            {
                if (this.keyboardInputCallback != null)
                {
                    this.keyboardInputCallback();
                }
                if (Event.current.type == EventType.KeyDown)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            if ((Application.platform == RuntimePlatform.OSXEditor) && this.BeginNameEditing(0f))
                            {
                                Event.current.Use();
                            }
                            return;

                        case KeyCode.UpArrow:
                            Event.current.Use();
                            this.OffsetSelection(-1);
                            return;

                        case KeyCode.DownArrow:
                            Event.current.Use();
                            this.OffsetSelection(1);
                            return;

                        case KeyCode.RightArrow:
                            this.ChangeFolding(this.state.selectedIDs.ToArray(), true);
                            Event.current.Use();
                            return;

                        case KeyCode.LeftArrow:
                            this.ChangeFolding(this.state.selectedIDs.ToArray(), false);
                            Event.current.Use();
                            return;

                        case KeyCode.Home:
                            Event.current.Use();
                            this.OffsetSelection(-1000000);
                            return;

                        case KeyCode.End:
                            Event.current.Use();
                            this.OffsetSelection(0xf4240);
                            return;

                        case KeyCode.PageUp:
                        {
                            Event.current.Use();
                            TreeViewItem fromItem = this.data.FindItem(this.state.lastClickedID);
                            if (fromItem != null)
                            {
                                int num = this.gui.GetNumRowsOnPageUpDown(fromItem, true, this.m_TotalRect.height);
                                this.OffsetSelection(-num);
                            }
                            return;
                        }
                        case KeyCode.PageDown:
                        {
                            Event.current.Use();
                            TreeViewItem item2 = this.data.FindItem(this.state.lastClickedID);
                            if (item2 != null)
                            {
                                int offset = this.gui.GetNumRowsOnPageUpDown(item2, true, this.m_TotalRect.height);
                                this.OffsetSelection(offset);
                            }
                            return;
                        }
                        case KeyCode.F2:
                            if ((Application.platform != RuntimePlatform.OSXEditor) && this.BeginNameEditing(0f))
                            {
                                Event.current.Use();
                            }
                            return;
                    }
                    if ((Event.current.keyCode > KeyCode.A) && (Event.current.keyCode < KeyCode.Z))
                    {
                    }
                }
            }
        }

        private void NewSelectionFromUserInteraction(List<int> newSelection, int itemID)
        {
            this.state.lastClickedID = itemID;
            if (!this.state.selectedIDs.SequenceEqual<int>(newSelection))
            {
                this.state.selectedIDs = newSelection;
                this.NotifyListenersThatSelectionChanged();
            }
        }

        public void NotifyListenersThatDragEnded(int[] draggedIDs, bool draggedItemsFromOwnTreeView)
        {
            if (this.dragEndedCallback != null)
            {
                this.dragEndedCallback(draggedIDs, draggedItemsFromOwnTreeView);
            }
        }

        public void NotifyListenersThatSelectionChanged()
        {
            if (this.selectionChangedCallback != null)
            {
                this.selectionChangedCallback(this.state.selectedIDs.ToArray());
            }
        }

        public void OffsetSelection(int offset)
        {
            this.expansionAnimator.SkipAnimating();
            IList<TreeViewItem> rows = this.data.GetRows();
            if (rows.Count != 0)
            {
                Event.current.Use();
                int row = Mathf.Clamp(GetIndexOfID(rows, this.state.lastClickedID) + offset, 0, rows.Count - 1);
                this.EnsureRowIsVisible(row, true);
                this.SelectionByKey(rows[row]);
            }
        }

        public void OnEvent()
        {
            this.state.renameOverlay.OnEvent();
        }

        public void OnGUI(Rect rect, int keyboardControlID)
        {
            int num;
            int num2;
            this.m_KeyboardControlID = keyboardControlID;
            Event current = Event.current;
            if (current.type == EventType.Repaint)
            {
                this.m_TotalRect = rect;
            }
            if (this.m_GUIView == null)
            {
                this.m_GUIView = GUIView.current;
            }
            if (((this.m_GUIView != null) && !this.m_GUIView.hasFocus) && this.state.renameOverlay.IsRenaming())
            {
                this.EndNameEditing(true);
            }
            if (this.m_GrabKeyboardFocus || ((current.type == EventType.MouseDown) && this.m_TotalRect.Contains(current.mousePosition)))
            {
                this.m_GrabKeyboardFocus = false;
                GUIUtility.keyboardControl = this.m_KeyboardControlID;
                this.m_AllowRenameOnMouseUp = true;
                this.Repaint();
            }
            bool hasFocus = this.HasFocus();
            if ((hasFocus != this.m_HadFocusLastEvent) && (current.type != EventType.Layout))
            {
                this.m_HadFocusLastEvent = hasFocus;
                if (hasFocus && (current.type == EventType.MouseDown))
                {
                    this.m_AllowRenameOnMouseUp = false;
                }
            }
            if (this.animatingExpansion)
            {
                this.m_ExpansionAnimator.OnBeforeAllRowsGUI();
            }
            this.data.InitIfNeeded();
            Vector2 totalSize = this.gui.GetTotalSize();
            this.m_ContentRect = new Rect(0f, 0f, totalSize.x, totalSize.y);
            if (this.m_UseScrollView)
            {
                this.state.scrollPos = GUI.BeginScrollView(this.m_TotalRect, this.state.scrollPos, this.m_ContentRect, (this.horizontalScrollbarStyle == null) ? GUI.skin.horizontalScrollbar : this.horizontalScrollbarStyle, (this.verticalScrollbarStyle == null) ? GUI.skin.verticalScrollbar : this.verticalScrollbarStyle);
            }
            else
            {
                GUI.BeginClip(this.m_TotalRect);
            }
            if (current.type == EventType.Repaint)
            {
                this.m_VisibleRect = !this.m_UseScrollView ? this.m_TotalRect : GUI.GetTopScrollView().visibleRect;
            }
            this.gui.BeginRowGUI();
            this.gui.GetFirstAndLastRowVisible(out num, out num2);
            if (num2 >= 0)
            {
                int numVisibleRows = (num2 - num) + 1;
                float rowWidth = Mathf.Max(GUIClip.visibleRect.width, this.m_ContentRect.width);
                this.IterateVisibleItems(num, numVisibleRows, rowWidth, hasFocus);
            }
            if (this.animatingExpansion)
            {
                this.m_ExpansionAnimator.OnAfterAllRowsGUI();
            }
            this.gui.EndRowGUI();
            if (this.m_UseScrollView)
            {
                GUI.EndScrollView(this.showingVerticalScrollBar);
            }
            else
            {
                GUI.EndClip();
            }
            this.HandleUnusedEvents();
            this.KeyboardGUI();
            GUIUtility.GetControlID(0x1fb41d2, FocusType.Passive);
        }

        public void ReloadData()
        {
            this.data.ReloadData();
            this.Repaint();
            this.m_StopIteratingItems = true;
        }

        public void RemoveSelection()
        {
            if (this.state.selectedIDs.Count > 0)
            {
                this.state.selectedIDs.Clear();
                this.NotifyListenersThatSelectionChanged();
            }
        }

        public void Repaint()
        {
            if (this.m_GUIView != null)
            {
                this.m_GUIView.Repaint();
            }
        }

        private void SelectionByKey(TreeViewItem itemSelected)
        {
            List<int> newSelection = this.GetNewSelection(itemSelected, false, true);
            this.NewSelectionFromUserInteraction(newSelection, itemSelected.id);
        }

        public void SelectionClick(TreeViewItem itemClicked, bool keepMultiSelection)
        {
            List<int> newSelection = this.GetNewSelection(itemClicked, keepMultiSelection, false);
            this.NewSelectionFromUserInteraction(newSelection, (itemClicked == null) ? 0 : itemClicked.id);
        }

        public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected)
        {
            this.SetSelection(selectedIDs, revealSelectionAndFrameLastSelected, false);
        }

        public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected, bool animatedFraming)
        {
            if (selectedIDs.Length > 0)
            {
                if (revealSelectionAndFrameLastSelected)
                {
                    foreach (int num in selectedIDs)
                    {
                        this.data.RevealItem(num);
                    }
                }
                this.state.selectedIDs = new List<int>(selectedIDs);
                bool flag = this.state.selectedIDs.IndexOf(this.state.lastClickedID) >= 0;
                if (!flag)
                {
                    int id = selectedIDs.Last<int>();
                    if (this.data.GetRow(id) != -1)
                    {
                        this.state.lastClickedID = id;
                        flag = true;
                    }
                    else
                    {
                        this.state.lastClickedID = 0;
                    }
                }
                if (revealSelectionAndFrameLastSelected && flag)
                {
                    this.Frame(this.state.lastClickedID, true, false, animatedFraming);
                }
            }
            else
            {
                this.state.selectedIDs.Clear();
                this.state.lastClickedID = 0;
            }
        }

        public void SetTotalRect(Rect rect)
        {
            this.m_TotalRect = rect;
        }

        public void SetUseScrollView(bool useScrollView)
        {
            this.m_UseScrollView = useScrollView;
        }

        public List<int> SortIDsInVisiblityOrder(IList<int> ids)
        {
            if (ids.Count <= 1)
            {
                return ids.ToList<int>();
            }
            IList<TreeViewItem> rows = this.data.GetRows();
            List<int> second = new List<int>();
            for (int i = 0; i < rows.Count; i++)
            {
                int id = rows[i].id;
                for (int j = 0; j < ids.Count; j++)
                {
                    if (ids[j] == id)
                    {
                        second.Add(id);
                        break;
                    }
                }
            }
            if (ids.Count != second.Count)
            {
                second.AddRange(ids.Except<int>(second));
                if (ids.Count != second.Count)
                {
                    UnityEngine.Debug.LogError(string.Concat(new object[] { "SortIDsInVisiblityOrder failed: ", ids.Count, " != ", second.Count }));
                }
            }
            return second;
        }

        public void UserInputChangedExpandedState(TreeViewItem item, int row, bool expand)
        {
            bool alt = Event.current.alt;
            if (this.useExpansionAnimation)
            {
                if (expand)
                {
                    this.ChangeExpandedState(item, true, alt);
                }
                int startRow = row + 1;
                int lastChildRowUnder = this.GetLastChildRowUnder(row);
                float width = GUIClip.visibleRect.width;
                Rect rect2 = this.GetRectForRows(startRow, lastChildRowUnder, width);
                float animationDuration = this.GetAnimationDuration(rect2.height);
                TreeViewAnimationInput setup = new TreeViewAnimationInput {
                    animationDuration = animationDuration,
                    startRow = startRow,
                    endRow = lastChildRowUnder,
                    startRowRect = this.gui.GetRowRect(startRow, width),
                    rowsRect = rect2,
                    expanding = expand,
                    includeChildren = alt,
                    animationEnded = new Action<TreeViewAnimationInput>(this.ExpansionAnimationEnded),
                    item = item,
                    treeView = this
                };
                this.expansionAnimator.BeginAnimating(setup);
            }
            else
            {
                this.ChangeExpandedState(item, expand, alt);
            }
        }

        public bool animatingExpansion =>
            (this.m_UseExpansionAnimation && this.m_ExpansionAnimator.isAnimating);

        public Action<int> contextClickItemCallback { get; set; }

        public System.Action contextClickOutsideItemsCallback { get; set; }

        public ITreeViewDataSource data { get; set; }

        public bool deselectOnUnhandledMouseDown { get; set; }

        public Action<int[], bool> dragEndedCallback { get; set; }

        public ITreeViewDragging dragging { get; set; }

        public System.Action expandedStateChanged { get; set; }

        public TreeViewItemExpansionAnimator expansionAnimator =>
            this.m_ExpansionAnimator;

        public ITreeViewGUI gui { get; set; }

        public GUIStyle horizontalScrollbarStyle { get; set; }

        public bool isDragging =>
            ((this.m_DragSelection != null) && (this.m_DragSelection.Count > 0));

        public bool isSearching =>
            !string.IsNullOrEmpty(this.state.searchString);

        public Action<int> itemDoubleClickedCallback { get; set; }

        public System.Action keyboardInputCallback { get; set; }

        public Action<int, Rect> onGUIRowCallback { get; set; }

        public Action<Vector2> scrollChanged { get; set; }

        public Action<string> searchChanged { get; set; }

        public string searchString
        {
            get => 
                this.state.searchString;
            set
            {
                if (!object.ReferenceEquals(this.state.searchString, value) && (this.state.searchString != value))
                {
                    this.state.searchString = value;
                    this.data.OnSearchChanged();
                    if (this.searchChanged != null)
                    {
                        this.searchChanged(this.state.searchString);
                    }
                }
            }
        }

        public Action<int[]> selectionChangedCallback { get; set; }

        public bool showingHorizontalScrollBar =>
            (this.m_ContentRect.width > this.m_VisibleRect.width);

        public bool showingVerticalScrollBar =>
            (this.m_ContentRect.height > this.m_VisibleRect.height);

        public TreeViewState state { get; set; }

        public bool useExpansionAnimation
        {
            get => 
                this.m_UseExpansionAnimation;
            set
            {
                this.m_UseExpansionAnimation = value;
            }
        }

        public GUIStyle verticalScrollbarStyle { get; set; }

        [CompilerGenerated]
        private sealed class <BeginNameEditing>c__AnonStorey0
        {
            internal int id;

            internal bool <>m__0(TreeViewItem i) => 
                (i.id == this.id);
        }
    }
}

