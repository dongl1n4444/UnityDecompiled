namespace UnityEditor.IMGUI.Controls
{
    using mscorlib;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal abstract class TreeView
    {
        private TreeViewControlDataSource m_DataSource;
        private TreeViewControlDragging m_Dragging;
        private TreeViewControlGUI m_GUI;
        private MultiColumnHeader m_MultiColumnHeader;
        private OverriddenMethods m_OverriddenMethods;
        private TreeViewController m_TreeView;
        private int m_TreeViewKeyControlID;
        private bool m_WarnedUser;

        public TreeView(TreeViewState state)
        {
            this.Init(state);
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader)
        {
            this.m_MultiColumnHeader = multiColumnHeader;
            this.Init(state);
        }

        public bool BeginRename(TreeViewItem item) => 
            this.BeginRename(item, 0f);

        public bool BeginRename(TreeViewItem item, float delay) => 
            this.m_GUI.BeginRename(item, delay);

        protected virtual void BeginRowGUI()
        {
        }

        protected abstract void BuildRootAndRows(out TreeViewItem hiddenRoot, out IList<TreeViewItem> rows);
        protected virtual bool CanBeParent(TreeViewItem item) => 
            true;

        protected virtual bool CanChangeExpandedState(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return false;
            }
            return item.hasChildren;
        }

        protected virtual bool CanMultiSelect(TreeViewItem item) => 
            true;

        protected virtual bool CanRename(TreeViewItem item) => 
            false;

        protected virtual bool CanStartDrag(CanStartDragArgs args) => 
            false;

        public void CollapseAll()
        {
            this.SetExpanded(new int[0]);
        }

        protected virtual void ContextClicked()
        {
        }

        protected virtual void ContextClickedItem(int id)
        {
        }

        public static List<TreeViewItem> CreateChildListForCollapsedParent() => 
            LazyTreeViewDataSource.CreateChildListForCollapsedParent();

        protected virtual void DoubleClickedItem(int id)
        {
        }

        public void EndRename()
        {
            this.m_GUI.EndRename();
        }

        protected virtual void EndRowGUI()
        {
        }

        public void ExpandAll()
        {
            this.SetExpandedRecursive(this.rootItem.id, true);
        }

        protected virtual void ExpandedStateChanged()
        {
        }

        private TreeViewItem FindItem(int id)
        {
            if (this.rootItem == null)
            {
                throw new InvalidOperationException("FindItem failed: root item has not been created yet");
            }
            TreeViewItem item = TreeViewUtility.FindItem(id, this.rootItem);
            if (item == null)
            {
                throw new ArgumentException($"Could not find item with id: {id}. FindItem assumes complete tree is built.");
            }
            return item;
        }

        public void FrameItem(int id)
        {
            bool animated = false;
            this.m_TreeView.Frame(id, true, false, animated);
        }

        protected virtual IList<int> GetAncestors(int id) => 
            TreeViewUtility.GetParentsAboveItem(this.FindItem(id)).ToList<int>();

        public Rect GetCellRectForTreeFoldouts(Rect rowRect)
        {
            if (this.multiColumnHeader == null)
            {
                throw new InvalidOperationException("GetCellRect can only be called when 'multiColumnHeader' has been set");
            }
            int columnIndexForTreeFoldouts = this.columnIndexForTreeFoldouts;
            int visibleColumnIndex = this.multiColumnHeader.GetVisibleColumnIndex(columnIndexForTreeFoldouts);
            return this.multiColumnHeader.GetCellRect(visibleColumnIndex, rowRect);
        }

        public float GetContentIndent(TreeViewItem item) => 
            this.m_GUI.GetContentIndent(item);

        protected virtual IList<int> GetDescendantsThatHaveChildren(int id) => 
            TreeViewUtility.GetParentsBelowItem(this.FindItem(id)).ToList<int>();

        public IList<int> GetExpanded() => 
            this.m_DataSource.GetExpandedIDs();

        public float GetFoldoutIndent(TreeViewItem item) => 
            this.m_GUI.GetFoldoutIndent(item);

        protected virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item) => 
            this.m_GUI.DefaultRenameRect(rowRect, row, item);

        public IList<TreeViewItem> GetRows() => 
            this.m_TreeView.data.GetRows();

        public IList<TreeViewItem> GetRowsFromIDs(IList<int> ids)
        {
            <GetRowsFromIDs>c__AnonStorey0 storey = new <GetRowsFromIDs>c__AnonStorey0 {
                ids = ids
            };
            return Enumerable.Where<TreeViewItem>(this.GetRows(), new Func<TreeViewItem, bool>(storey.<>m__0)).ToList<TreeViewItem>();
        }

        public IList<int> GetSelection() => 
            this.m_TreeView.GetSelection();

        protected virtual DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) => 
            DragAndDropVisualMode.None;

        public bool HasSelection() => 
            this.m_TreeView.HasSelection();

        private void Init(TreeViewState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state", "Invalid TreeViewState: it is null");
            }
            this.m_TreeView = new TreeViewController(null, state);
            this.m_DataSource = new TreeViewControlDataSource(this.m_TreeView, this);
            this.m_GUI = new TreeViewControlGUI(this.m_TreeView, this);
            this.m_Dragging = new TreeViewControlDragging(this.m_TreeView, this);
            this.m_TreeView.Init(new Rect(), this.m_DataSource, this.m_GUI, this.m_Dragging);
            this.m_TreeView.searchChanged = (Action<string>) Delegate.Combine(this.m_TreeView.searchChanged, new Action<string>(this.SearchChanged));
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
            this.m_TreeView.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TreeView.itemDoubleClickedCallback, new Action<int>(this.DoubleClickedItem));
            this.m_TreeView.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_TreeView.contextClickItemCallback, new Action<int>(this.ContextClickedItem));
            this.m_TreeView.contextClickOutsideItemsCallback = (Action) Delegate.Combine(this.m_TreeView.contextClickOutsideItemsCallback, new Action(this.ContextClicked));
            this.m_TreeView.expandedStateChanged = (Action) Delegate.Combine(this.m_TreeView.expandedStateChanged, new Action(this.ExpandedStateChanged));
            this.m_TreeViewKeyControlID = GUIUtility.GetPermanentControlID();
        }

        public bool IsExpanded(int id) => 
            this.m_DataSource.IsExpanded(id);

        public bool IsSelected(int id) => 
            this.m_TreeView.IsSelected(id);

        protected virtual void OnDrawItemBackground(ItemGUIEventArgs args)
        {
        }

        public void OnGUI(Rect rect)
        {
            if (this.ValidTreeView())
            {
                this.m_TreeView.OnEvent();
                if (this.m_MultiColumnHeader != null)
                {
                    this.TreeViewWithMultiColumnHeader(rect);
                }
                else
                {
                    this.m_TreeView.OnGUI(rect, this.m_TreeViewKeyControlID);
                }
            }
        }

        protected virtual void OnItemGUI(ItemGUIEventArgs args)
        {
            this.m_GUI.DefaultItemGUI(args);
        }

        public void Reload()
        {
            if (this.m_OverriddenMethods == null)
            {
                this.m_OverriddenMethods = new OverriddenMethods(this);
            }
            this.m_TreeView.ReloadData();
        }

        protected virtual void RenameEnded(RenameEndedArgs args)
        {
        }

        protected virtual void SearchChanged(string newSearch)
        {
        }

        protected virtual void SelectionChanged(IList<int> selectedIds)
        {
        }

        public void SetCustomRowHeights(IList<float> rowHeights)
        {
            this.m_GUI.SetRowHeights(rowHeights);
        }

        public void SetExpanded(IList<int> ids)
        {
            this.m_DataSource.SetExpandedIDs(ids.ToArray<int>());
        }

        public bool SetExpanded(int id, bool expanded) => 
            this.m_DataSource.SetExpanded(id, expanded);

        public void SetExpandedRecursive(int id, bool expanded)
        {
            this.m_DataSource.SetExpandedWithChildren(id, expanded);
        }

        public void SetSelection(IList<int> selectedIDs)
        {
            this.SetSelection(selectedIDs, TreeViewSelectionOptions.None);
        }

        public void SetSelection(IList<int> selectedIDs, TreeViewSelectionOptions options)
        {
            bool flag = (options & TreeViewSelectionOptions.FireSelectionChanged) != TreeViewSelectionOptions.None;
            bool revealSelectionAndFrameLastSelected = (options & TreeViewSelectionOptions.RevealAndFrame) != TreeViewSelectionOptions.None;
            bool animatedFraming = false;
            this.m_TreeView.SetSelection(selectedIDs.ToArray<int>(), revealSelectionAndFrameLastSelected, animatedFraming);
            if (flag)
            {
                this.m_TreeView.NotifyListenersThatSelectionChanged();
            }
        }

        protected virtual void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
        }

        private void TreeViewWithMultiColumnHeader(Rect rect)
        {
            Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_MultiColumnHeader.height);
            Rect rect3 = new Rect(rect.x, rect2.yMax, rect.width, rect.height - rect2.height);
            float xScroll = Mathf.Max(this.m_TreeView.state.scrollPos.x, 0f);
            this.m_MultiColumnHeader.OnGUI(rect2, xScroll);
            this.m_TreeView.OnGUI(rect3, this.m_TreeViewKeyControlID);
        }

        private bool ValidTreeView()
        {
            if (this.m_TreeView.data.root != null)
            {
                return true;
            }
            if (!this.m_WarnedUser)
            {
                Debug.LogError("TreeView has not been properly intialized yet (rootItem is null). Ensure to call Reload() before using the tree view");
                this.m_WarnedUser = true;
            }
            return false;
        }

        public float baseIndent
        {
            get => 
                this.m_GUI.k_BaseIndent;
            set
            {
                this.m_GUI.k_BaseIndent = value;
            }
        }

        public int columnIndexForTreeFoldouts
        {
            get => 
                this.m_GUI.columnIndexForTreeFoldouts;
            set
            {
                this.m_GUI.columnIndexForTreeFoldouts = value;
            }
        }

        public float depthIndentWidth =>
            this.m_GUI.k_IndentWidth;

        public float foldoutWidth =>
            this.m_GUI.foldoutWidth;

        public float foldoutYOffset
        {
            get => 
                this.m_GUI.foldoutYOffset;
            set
            {
                this.m_GUI.foldoutYOffset = value;
            }
        }

        public bool hasSearch =>
            !string.IsNullOrEmpty(this.searchString);

        public bool isDragging =>
            this.m_TreeView.isDragging;

        public MultiColumnHeader multiColumnHeader =>
            this.m_MultiColumnHeader;

        public TreeViewItem rootItem =>
            this.m_TreeView.data.root;

        public float rowHeight
        {
            get => 
                this.m_GUI.k_LineHeight;
            set
            {
                this.m_GUI.k_LineHeight = Mathf.Max(value, 16f);
            }
        }

        public string searchString
        {
            get => 
                this.m_TreeView.searchString;
            set
            {
                this.m_TreeView.searchString = value;
            }
        }

        public TreeViewState state =>
            this.m_TreeView.state;

        public int treeViewControlID =>
            this.m_TreeViewKeyControlID;

        public Rect treeViewRect
        {
            get => 
                this.m_TreeView.GetTotalRect();
            set
            {
                this.m_TreeView.SetTotalRect(value);
            }
        }

        [CompilerGenerated]
        private sealed class <GetRowsFromIDs>c__AnonStorey0
        {
            internal IList<int> ids;

            internal bool <>m__0(TreeViewItem item) => 
                this.ids.Contains(item.id);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CanStartDragArgs
        {
            public TreeViewItem draggedItem;
            public IList<int> draggedItemIDs;
        }

        public static class DefaultGUI
        {
            public static void BoldLabel(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.boldLabel.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            public static void BoldLabelRightAligned(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.boldLabelRightAligned.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            public static void Label(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.label.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            public static void LabelRightAligned(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.labelRightAligned.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            public static float columnMargin =>
                5f;

            public static float contentLeftMargin =>
                ((float) TreeView.DefaultStyles.label.margin.left);
        }

        public static class DefaultStyles
        {
            public static GUIStyle boldLabel;
            public static GUIStyle boldLabelRightAligned;
            public static GUIStyle label = new GUIStyle("PR Label");
            public static GUIStyle labelRightAligned;

            static DefaultStyles()
            {
                Texture2D background = label.hover.background;
                label.padding.left = 0;
                label.padding.right = 0;
                label.onNormal.background = background;
                label.onActive.background = background;
                label.onFocused.background = background;
                label.alignment = TextAnchor.MiddleLeft;
                labelRightAligned = new GUIStyle(label);
                labelRightAligned.alignment = TextAnchor.MiddleRight;
                boldLabel = new GUIStyle(label);
                boldLabel.font = EditorStyles.boldLabel.font;
                boldLabel.fontStyle = EditorStyles.boldLabel.fontStyle;
                boldLabelRightAligned = new GUIStyle(boldLabel);
                boldLabelRightAligned.alignment = TextAnchor.MiddleRight;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DragAndDropArgs
        {
            public TreeViewItem parentItem;
            public int insertAtIndex;
            public bool performDrop;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ItemGUIEventArgs
        {
            public TreeViewItem item;
            public Rect rowRect;
            public Rect cellRect;
            public int row;
            public int column;
            public bool selected;
            public bool focused;
            public bool isRenaming;
            public bool isDropTarget;
        }

        private class OverriddenMethods
        {
            public readonly bool hasDrawItemBackground;
            public readonly bool hasGetRenameRect;
            public readonly bool hasHandleDragAndDrop;
            public readonly bool hasItemGUI;

            public OverriddenMethods(TreeView treeView)
            {
                Type type = treeView.GetType();
                this.hasItemGUI = IsOverridden(type, "OnItemGUI");
                this.hasDrawItemBackground = IsOverridden(type, "OnDrawItemBackground");
                this.hasHandleDragAndDrop = IsOverridden(type, "HandleDragAndDrop");
                this.hasGetRenameRect = IsOverridden(type, "GetRenameRect");
                this.ValidateOverriddenMethods(treeView);
            }

            private static bool IsOverridden(Type type, string methodName)
            {
                MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    return !(method.GetBaseDefinition().DeclaringType == method.DeclaringType);
                }
                Debug.LogError("IsOverridden: method name not found: " + methodName + " (check spelling against method declaration)");
                return false;
            }

            private void ValidateOverriddenMethods(TreeView treeView)
            {
                Type type = treeView.GetType();
                bool flag = IsOverridden(type, "CanRename");
                bool flag2 = IsOverridden(type, "RenameEnded");
                if (flag2 != flag)
                {
                    if (flag)
                    {
                        Debug.LogError(type.Name + ": If you are overriding CanRename you should also override RenameEnded (to handle the renaming).");
                    }
                    if (flag2)
                    {
                        Debug.LogError(type.Name + ": If you are overriding RenameEnded you should also override CanRename (to allow renaming).");
                    }
                }
                bool flag3 = IsOverridden(type, "CanStartDrag");
                bool flag4 = IsOverridden(type, "SetupDragAndDrop");
                if (flag3 != flag4)
                {
                    if (flag3)
                    {
                        Debug.LogError(type.Name + ": If you are overriding CanStartDrag you should also override SetupDragAndDrop (to setup the drag).");
                    }
                    if (flag4)
                    {
                        Debug.LogError(type.Name + ": If you are overriding SetupDragAndDrop you should also override CanStartDrag (to allow dragging).");
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RenameEndedArgs
        {
            public bool acceptedRename;
            public int itemID;
            public string originalName;
            public string newName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SetupDragAndDropArgs
        {
            public IList<int> draggedItemIDs;
        }

        private class TreeViewControlDataSource : LazyTreeViewDataSource
        {
            private readonly TreeView m_Owner;

            public TreeViewControlDataSource(TreeViewController treeView, TreeView owner) : base(treeView)
            {
                this.m_Owner = owner;
                base.showRootItem = false;
            }

            public override bool CanBeMultiSelected(TreeViewItem item) => 
                this.m_Owner.CanMultiSelect(item);

            public override bool CanBeParent(TreeViewItem item) => 
                this.m_Owner.CanBeParent(item);

            public override void FetchData()
            {
                this.m_Owner.BuildRootAndRows(out this.m_RootItem, out this.m_Rows);
                if (base.m_RootItem == null)
                {
                    throw new NullReferenceException("BuildRootAndRows should set a valid root item.");
                }
                if (base.m_Rows == null)
                {
                    throw new NullReferenceException("BuildRootAndRows should set valid list of rows.");
                }
                if (base.m_RootItem.depth != -1)
                {
                    base.m_RootItem.depth = -1;
                }
                if (base.m_RootItem.id != 0)
                {
                    base.m_RootItem.id = 0;
                }
                base.m_NeedRefreshRows = false;
            }

            protected override HashSet<int> GetParentsAbove(int id) => 
                new HashSet<int>(this.m_Owner.GetAncestors(id));

            protected override HashSet<int> GetParentsBelow(int id) => 
                new HashSet<int>(this.m_Owner.GetDescendantsThatHaveChildren(id));

            public override bool IsExpandable(TreeViewItem item) => 
                this.m_Owner.CanChangeExpandedState(item);

            public override bool IsRenamingItemAllowed(TreeViewItem item) => 
                this.m_Owner.CanRename(item);
        }

        private class TreeViewControlDragging : TreeViewDragging
        {
            private TreeView m_Owner;

            public TreeViewControlDragging(TreeViewController treeView, TreeView owner) : base(treeView)
            {
                this.m_Owner = owner;
            }

            public override bool CanStartDrag(TreeViewItem targetItem, List<int> draggedItemIDs, Vector2 mouseDownPosition)
            {
                TreeView.CanStartDragArgs args = new TreeView.CanStartDragArgs {
                    draggedItem = targetItem,
                    draggedItemIDs = draggedItemIDs
                };
                return this.m_Owner.CanStartDrag(args);
            }

            public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPosition)
            {
                if (this.m_Owner.m_OverriddenMethods.hasHandleDragAndDrop)
                {
                    TreeView.DragAndDropArgs args = new TreeView.DragAndDropArgs {
                        insertAtIndex = -1,
                        parentItem = parentItem,
                        performDrop = perform
                    };
                    if ((parentItem != null) && (targetItem != null))
                    {
                        args.insertAtIndex = TreeViewDragging.GetInsertionIndex(parentItem, targetItem, dropPosition);
                    }
                    return this.m_Owner.HandleDragAndDrop(args);
                }
                return DragAndDropVisualMode.None;
            }

            public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
            {
                TreeView.SetupDragAndDropArgs args = new TreeView.SetupDragAndDropArgs {
                    draggedItemIDs = draggedItemIDs
                };
                this.m_Owner.SetupDragAndDrop(args);
            }
        }

        private class TreeViewControlGUI : TreeViewGUI
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private int <columnIndexForTreeFoldouts>k__BackingField;
            private readonly TreeView m_Owner;
            private List<Rect> m_RowRects;

            public TreeViewControlGUI(TreeViewController treeView, TreeView owner) : base(treeView)
            {
                this.m_Owner = owner;
            }

            public override void BeginRowGUI()
            {
                base.BeginRowGUI();
                if ((this.m_Owner.isDragging && (this.m_Owner.multiColumnHeader != null)) && (this.columnIndexForTreeFoldouts > 0))
                {
                    int visibleColumnIndex = this.m_Owner.multiColumnHeader.GetVisibleColumnIndex(this.columnIndexForTreeFoldouts);
                    base.extraInsertionMarkerIndent = this.m_Owner.multiColumnHeader.GetColumnRect(visibleColumnIndex).x;
                }
                this.m_Owner.BeginRowGUI();
            }

            private void CalculateRowRects(IList<float> rowHeights)
            {
                if (this.m_RowRects == null)
                {
                    this.m_RowRects = new List<Rect>(rowHeights.Count);
                }
                if (this.m_RowRects.Capacity < rowHeights.Count)
                {
                    this.m_RowRects.Capacity = rowHeights.Count;
                }
                this.m_RowRects.Clear();
                float y = base.k_TopRowMargin;
                for (int i = 0; i < rowHeights.Count; i++)
                {
                    this.m_RowRects.Add(new Rect(0f, y, 1f, rowHeights[i]));
                    y += rowHeights[i];
                }
            }

            internal void DefaultItemGUI(TreeView.ItemGUIEventArgs args)
            {
                string displayName = args.item.displayName;
                if (this.IsRenaming(args.item.id))
                {
                    displayName = "";
                }
                base.OnContentGUI(args.rowRect, args.row, args.item, displayName, args.selected, args.focused, false, false);
            }

            public Rect DefaultRenameRect(Rect rowRect, int row, TreeViewItem item) => 
                base.GetRenameRect(rowRect, row, item);

            protected override Rect DoFoldout(Rect rowRect, TreeViewItem item, int row)
            {
                if (this.m_Owner.multiColumnHeader != null)
                {
                    return this.DoMultiColumnFoldout(rowRect, item, row);
                }
                return base.DoFoldout(rowRect, item, row);
            }

            private Rect DoMultiColumnFoldout(Rect rowRect, TreeViewItem item, int row)
            {
                Rect cellRectForTreeFoldouts = this.m_Owner.GetCellRectForTreeFoldouts(rowRect);
                if (this.GetContentIndent(item) > cellRectForTreeFoldouts.width)
                {
                    GUIClip.Push(cellRectForTreeFoldouts, Vector2.zero, Vector2.zero, false);
                    float num = 0f;
                    cellRectForTreeFoldouts.y = num;
                    cellRectForTreeFoldouts.x = num;
                    Rect rect2 = base.DoFoldout(cellRectForTreeFoldouts, item, row);
                    GUIClip.Pop();
                    return rect2;
                }
                return base.DoFoldout(cellRectForTreeFoldouts, item, row);
            }

            protected override void DrawItemBackground(Rect rect, int row, TreeViewItem item, bool selected, bool focused)
            {
                if (this.m_Owner.m_OverriddenMethods.hasDrawItemBackground)
                {
                    TreeView.ItemGUIEventArgs args = new TreeView.ItemGUIEventArgs {
                        rowRect = rect,
                        row = row,
                        item = item,
                        selected = selected,
                        focused = focused,
                        isRenaming = this.IsRenaming(item.id),
                        isDropTarget = this.IsDropTarget(item)
                    };
                    this.m_Owner.OnDrawItemBackground(args);
                }
            }

            public override void EndRowGUI()
            {
                base.EndRowGUI();
                this.m_Owner.EndRowGUI();
            }

            public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
            {
                if (!this.hasCustomRowRects)
                {
                    base.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
                }
                else
                {
                    int rowCount = base.m_TreeView.data.rowCount;
                    if (rowCount != this.m_RowRects.Count)
                    {
                        Debug.LogError($"Mismatch in state: rows vs cached rects. Did you remember to update row heigths when BuildRootAndRows was called. Number of rows: {rowCount}, number of custom row heights: {this.m_RowRects.Count}");
                    }
                    float y = base.m_TreeView.state.scrollPos.y;
                    float height = base.m_TreeView.GetTotalRect().height;
                    int num4 = -1;
                    int num5 = -1;
                    for (int i = 0; i < this.m_RowRects.Count; i++)
                    {
                        Rect rect2 = this.m_RowRects[i];
                        if (rect2.y > y)
                        {
                            Rect rect3 = this.m_RowRects[i];
                            if (rect3.y < (y + height))
                            {
                                goto Label_0112;
                            }
                        }
                        Rect rect4 = this.m_RowRects[i];
                        System.Boolean ReflectorVariable0 = true;
                        goto Label_0113;
                    Label_0112:
                        ReflectorVariable0 = false;
                    Label_0113:
                        if (ReflectorVariable0 ? ((rect4.yMax > y) && (this.m_RowRects[i].yMax < (y + height))) : true)
                        {
                            if (num4 == -1)
                            {
                                num4 = i;
                            }
                            num5 = i;
                        }
                    }
                    if ((num4 != -1) && (num5 != -1))
                    {
                        firstRowVisible = num4;
                        lastRowVisible = num5;
                    }
                    else
                    {
                        firstRowVisible = 0;
                        lastRowVisible = rowCount - 1;
                    }
                }
            }

            public override Rect GetRectForFraming(int row) => 
                this.GetRowRect(row, 1f);

            public override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
            {
                if (this.m_Owner.m_OverriddenMethods.hasGetRenameRect)
                {
                    return this.m_Owner.GetRenameRect(rowRect, row, item);
                }
                return base.GetRenameRect(rowRect, row, item);
            }

            public override Rect GetRowRect(int row, float rowWidth)
            {
                if (!this.hasCustomRowRects)
                {
                    return base.GetRowRect(row, rowWidth);
                }
                if (this.m_RowRects.Count == 0)
                {
                    Debug.LogError("Mismatch in state: rows vs cached rects. No cached row rects but requested row rect " + row);
                    return new Rect();
                }
                Rect rect3 = this.m_RowRects[row];
                rect3.width = rowWidth;
                return rect3;
            }

            public override Vector2 GetTotalSize()
            {
                Vector2 vector = !this.hasCustomRowRects ? base.GetTotalSize() : new Vector2(1f, this.customRowsTotalHeight);
                if (this.m_Owner.multiColumnHeader != null)
                {
                    vector.x = Mathf.Floor(this.m_Owner.multiColumnHeader.state.widthOfAllVisibleColumns);
                }
                return vector;
            }

            private bool IsDropTarget(TreeViewItem item) => 
                ((base.m_TreeView.dragging.GetDropTargetControlID() == item.id) && base.m_TreeView.data.CanBeParent(item));

            protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
            {
                if (!isPinging)
                {
                    if (this.m_Owner.m_OverriddenMethods.hasItemGUI)
                    {
                        TreeView.ItemGUIEventArgs args = new TreeView.ItemGUIEventArgs {
                            rowRect = rect,
                            row = row,
                            item = item,
                            selected = selected,
                            focused = focused,
                            isRenaming = this.IsRenaming(item.id),
                            isDropTarget = this.IsDropTarget(item)
                        };
                        if (this.m_Owner.multiColumnHeader != null)
                        {
                            int[] visibleColumns = this.m_Owner.multiColumnHeader.state.visibleColumns;
                            MultiColumnHeaderState.Column[] columns = this.m_Owner.multiColumnHeader.state.columns;
                            Rect rowRect = args.rowRect;
                            for (int i = 0; i < visibleColumns.Length; i++)
                            {
                                int index = visibleColumns[i];
                                MultiColumnHeaderState.Column column = columns[index];
                                rowRect.width = column.width;
                                args.cellRect = rowRect;
                                args.column = index;
                                this.m_Owner.OnItemGUI(args);
                                rowRect.x += column.width;
                            }
                        }
                        else
                        {
                            this.m_Owner.OnItemGUI(args);
                        }
                    }
                    else
                    {
                        base.OnContentGUI(rect, row, item, label, selected, focused, useBoldFont, false);
                    }
                }
            }

            protected override void RenameEnded()
            {
                RenameOverlay renameOverlay = base.m_TreeView.state.renameOverlay;
                TreeView.RenameEndedArgs args = new TreeView.RenameEndedArgs {
                    acceptedRename = renameOverlay.userAcceptedRename,
                    itemID = renameOverlay.userData,
                    originalName = renameOverlay.originalName,
                    newName = renameOverlay.name
                };
                this.m_Owner.RenameEnded(args);
            }

            public void SetRowHeights(IList<float> rowHeights)
            {
                if (rowHeights == null)
                {
                    this.m_RowRects = null;
                }
                else
                {
                    this.CalculateRowRects(rowHeights);
                }
            }

            public int columnIndexForTreeFoldouts { get; set; }

            private float customRowsTotalHeight
            {
                get
                {
                    Rect rect = this.m_RowRects[this.m_RowRects.Count - 1];
                    return (rect.yMax + base.k_BottomRowMargin);
                }
            }

            public float foldoutWidth =>
                TreeViewGUI.s_Styles.foldoutWidth;

            private bool hasCustomRowRects =>
                ((this.m_RowRects != null) && (this.m_RowRects.Count > 0));
        }
    }
}

