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

    /// <summary>
    /// <para>The TreeView is an IMGUI control that lets you create tree views, list views and multi-column tables for Editor tools.</para>
    /// </summary>
    public abstract class TreeView
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <showAlternatingRowBackgrounds>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <showBorder>k__BackingField;
        private TreeViewControlDataSource m_DataSource;
        private List<TreeViewItem> m_DefaultRows;
        private TreeViewControlDragging m_Dragging;
        private TreeViewControlGUI m_GUI;
        private MultiColumnHeader m_MultiColumnHeader;
        private OverriddenMethods m_OverriddenMethods;
        private TreeViewController m_TreeView;
        private int m_TreeViewKeyControlID;
        private bool m_WarnedUser;

        /// <summary>
        /// <para>The TreeView is always constructed with a state object and optionally a multi-column header object if a header is needed.</para>
        /// </summary>
        /// <param name="state">TreeView state (expanded items, selection etc.)</param>
        /// <param name="multiColumnHeader">Multi-column header for the TreeView.</param>
        public TreeView(TreeViewState state)
        {
            this.Init(state);
        }

        /// <summary>
        /// <para>The TreeView is always constructed with a state object and optionally a multi-column header object if a header is needed.</para>
        /// </summary>
        /// <param name="state">TreeView state (expanded items, selection etc.)</param>
        /// <param name="multiColumnHeader">Multi-column header for the TreeView.</param>
        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader)
        {
            this.m_MultiColumnHeader = multiColumnHeader;
            this.Init(state);
        }

        protected void AddExpandedRows(TreeViewItem root, IList<TreeViewItem> rows)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "root is null");
            }
            if (rows == null)
            {
                throw new ArgumentNullException("rows", "rows is null");
            }
            if (root.hasChildren)
            {
                foreach (TreeViewItem item in root.children)
                {
                    this.GetExpandedRowsRecursive(item, rows);
                }
            }
        }

        /// <summary>
        /// <para>This is called after all rows have their RowGUI called.</para>
        /// </summary>
        protected virtual void AfterRowsGUI()
        {
        }

        /// <summary>
        /// <para>This is called before any rows have their RowGUI called.</para>
        /// </summary>
        protected virtual void BeforeRowsGUI()
        {
            if (this.showAlternatingRowBackgrounds)
            {
                this.m_GUI.DrawAlternatingRowBackgrounds();
            }
        }

        /// <summary>
        /// <para>Shows the rename overlay for a TreeViewItem.</para>
        /// </summary>
        /// <param name="item">Item to rename.</param>
        /// <param name="delay">Delay in seconds until the rename overlay shows.</param>
        /// <returns>
        /// <para>Returns true if renaming was started. Returns false if renaming was already active.</para>
        /// </returns>
        public bool BeginRename(TreeViewItem item) => 
            this.BeginRename(item, 0f);

        /// <summary>
        /// <para>Shows the rename overlay for a TreeViewItem.</para>
        /// </summary>
        /// <param name="item">Item to rename.</param>
        /// <param name="delay">Delay in seconds until the rename overlay shows.</param>
        /// <returns>
        /// <para>Returns true if renaming was started. Returns false if renaming was already active.</para>
        /// </returns>
        public bool BeginRename(TreeViewItem item, float delay) => 
            this.m_GUI.BeginRename(item, delay);

        /// <summary>
        /// <para>Abstract method that is required to be implemented. By default this method should create the full tree of TreeViewItems and return the root.</para>
        /// </summary>
        /// <returns>
        /// <para>The root of the tree. This item can later be accessed by 'rootItem'.</para>
        /// </returns>
        protected abstract TreeViewItem BuildRoot();
        /// <summary>
        /// <para>Override this method to take control of how the rows are generated.</para>
        /// </summary>
        /// <param name="root">Root item that was created in the BuildRoot method.</param>
        /// <returns>
        /// <para>The rows list shown in the TreeView. Can later be accessed using GetRows().</para>
        /// </returns>
        protected virtual IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (this.m_DefaultRows == null)
            {
                this.m_DefaultRows = new List<TreeViewItem>(100);
            }
            this.m_DefaultRows.Clear();
            if (this.hasSearch)
            {
                this.m_DataSource.SearchFullTree(this.searchString, this.m_DefaultRows);
            }
            else
            {
                this.AddExpandedRows(root, this.m_DefaultRows);
            }
            return this.m_DefaultRows;
        }

        /// <summary>
        /// <para>Override this method to control which items are allowed to be parents.</para>
        /// </summary>
        /// <param name="item">Can this item be a parent?</param>
        protected virtual bool CanBeParent(TreeViewItem item) => 
            true;

        /// <summary>
        /// <para>Override this method to control whether an item can be expanded or collapsed by key or mouse.</para>
        /// </summary>
        /// <param name="item">Can this item be expanded/collapsed.</param>
        protected virtual bool CanChangeExpandedState(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return false;
            }
            return item.hasChildren;
        }

        /// <summary>
        /// <para>Override this method to control whether the item can be part of a multiselection.</para>
        /// </summary>
        /// <param name="item">Can this item be part of a multiselection.</param>
        protected virtual bool CanMultiSelect(TreeViewItem item) => 
            true;

        /// <summary>
        /// <para>Override this method to control whether the item can be renamed using a keyboard shortcut or when clicking an already selected item.</para>
        /// </summary>
        /// <param name="item">Can this item be renamed?</param>
        protected virtual bool CanRename(TreeViewItem item) => 
            false;

        protected virtual bool CanStartDrag(CanStartDragArgs args) => 
            false;

        protected void CenterRectUsingSingleLineHeight(ref Rect rect)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            if (rect.height > singleLineHeight)
            {
                rect.y += (rect.height - singleLineHeight) * 0.5f;
                rect.height = singleLineHeight;
            }
        }

        /// <summary>
        /// <para>Collapse all expanded items in the TreeView.</para>
        /// </summary>
        public void CollapseAll()
        {
            this.SetExpanded(new int[0]);
        }

        /// <summary>
        /// <para>This function is called automatically and handles the ExecuteCommand events for “SelectAll” and “FrameSelection”. Override this function to extend or avoid Command events.</para>
        /// </summary>
        protected virtual void CommandEventHandling()
        {
            Event current = Event.current;
            if ((current.type == EventType.ExecuteCommand) || (current.type == EventType.ValidateCommand))
            {
                bool flag = current.type == EventType.ExecuteCommand;
                if (this.HasFocus() && (current.commandName == "SelectAll"))
                {
                    if (flag)
                    {
                        this.SelectAllRows();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                if (current.commandName == "FrameSelected")
                {
                    if (flag)
                    {
                        if (this.hasSearch)
                        {
                            this.searchString = string.Empty;
                        }
                        if (this.HasSelection())
                        {
                            this.FrameItem(this.GetSelection()[0]);
                        }
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
            }
        }

        /// <summary>
        /// <para>Override this method to handle context clicks outside any items (but still in the TreeView rect).</para>
        /// </summary>
        protected virtual void ContextClicked()
        {
        }

        /// <summary>
        /// <para>Override this method to handle a context click on an item with ID TreeViewItem.id.</para>
        /// </summary>
        /// <param name="id">TreeViewItem id.</param>
        protected virtual void ContextClickedItem(int id)
        {
        }

        /// <summary>
        /// <para>Creates a dummy TreeViewItem list. Useful when overriding BuildRows to prevent building a full tree of items.</para>
        /// </summary>
        protected static List<TreeViewItem> CreateChildListForCollapsedParent() => 
            LazyTreeViewDataSource.CreateChildListForCollapsedParent();

        /// <summary>
        /// <para>Override this function to extend or change the search behavior.</para>
        /// </summary>
        /// <param name="item">Item used for matching against the search string.</param>
        /// <param name="search">The search string of the TreeView.</param>
        /// <returns>
        /// <para>True if item matches search string, otherwise false.</para>
        /// </returns>
        protected virtual bool DoesItemMatchSearch(TreeViewItem item, string search) => 
            (item.displayName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);

        /// <summary>
        /// <para>Override this method to handle double click events on an item.</para>
        /// </summary>
        /// <param name="id">ID of TreeViewItem that was double clicked.</param>
        protected virtual void DoubleClickedItem(int id)
        {
        }

        /// <summary>
        /// <para>Ends renaming if the rename overlay is shown. If called while the rename overlay is not being shown, this method does nothing.</para>
        /// </summary>
        public void EndRename()
        {
            this.m_GUI.EndRename();
        }

        /// <summary>
        /// <para>Expand all collapsed items in the TreeView.</para>
        /// </summary>
        public void ExpandAll()
        {
            this.SetExpandedRecursive(this.rootItem.id, true);
        }

        /// <summary>
        /// <para>Override to get notified when items are expanded or collapsed. This is a general notification that the expanded state has changed.</para>
        /// </summary>
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
                throw new ArgumentException($"Could not find item with id: {id}. FindItem assumes complete tree is built. Most likely the item is not allocated because it is hidden under a collapsed item. Check if GetAncestors are overriden for the tree view.");
            }
            return item;
        }

        /// <summary>
        /// <para>Finds a TreeViewItem by an ID.</para>
        /// </summary>
        /// <param name="id">Find the TreeViewItem with this ID.</param>
        /// <param name="searchFromThisItem">Sets the search to start from an item. Use 'rootItem' to search the entire tree.</param>
        /// <returns>
        /// <para>This search method returns the TreeViewItem found and returns null if not found.</para>
        /// </returns>
        protected TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem) => 
            TreeViewUtility.FindItem(id, searchFromThisItem);

        protected IList<TreeViewItem> FindRows(IList<int> ids)
        {
            <FindRows>c__AnonStorey0 storey = new <FindRows>c__AnonStorey0 {
                ids = ids
            };
            return Enumerable.Where<TreeViewItem>(this.GetRows(), new Func<TreeViewItem, bool>(storey.<>m__0)).ToList<TreeViewItem>();
        }

        /// <summary>
        /// <para>This will reveal the item with ID id (by expanding the ancestors of that item) and will make sure it is visible in the ScrollView.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        public void FrameItem(int id)
        {
            bool animated = false;
            this.m_TreeView.Frame(id, true, false, animated);
        }

        /// <summary>
        /// <para>This method is e.g. used for revealing items that are currently under a collapsed item.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        /// <returns>
        /// <para>List of all the ancestors of a given item with ID id.</para>
        /// </returns>
        protected virtual IList<int> GetAncestors(int id) => 
            TreeViewUtility.GetParentsAboveItem(this.FindItem(id)).ToList<int>();

        /// <summary>
        /// <para>Utility for multi column setups. This method will clip the input rowRect against the column rect defined by columnIndexForTreeFoldouts to get the cell rect where the the foldout arrows appear.</para>
        /// </summary>
        /// <param name="rowRect">Rect for a row.</param>
        /// <returns>
        /// <para>Cell rect in a multi column setup.</para>
        /// </returns>
        protected Rect GetCellRectForTreeFoldouts(Rect rowRect)
        {
            if (this.multiColumnHeader == null)
            {
                throw new InvalidOperationException("GetCellRect can only be called when 'multiColumnHeader' has been set");
            }
            int columnIndexForTreeFoldouts = this.columnIndexForTreeFoldouts;
            int visibleColumnIndex = this.multiColumnHeader.GetVisibleColumnIndex(columnIndexForTreeFoldouts);
            return this.multiColumnHeader.GetCellRect(visibleColumnIndex, rowRect);
        }

        /// <summary>
        /// <para>Returns the horizontal content offset for an item. This is where the content should begin (after the foldout arrow).</para>
        /// </summary>
        /// <param name="item">Item used to determine the indent.</param>
        /// <returns>
        /// <para>Indent.</para>
        /// </returns>
        protected float GetContentIndent(TreeViewItem item) => 
            this.m_GUI.GetContentIndent(item);

        /// <summary>
        /// <para>Override to control individual row heights.</para>
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="item">Item for given row.</param>
        /// <returns>
        /// <para>Height of row.</para>
        /// </returns>
        protected virtual float GetCustomRowHeight(int row, TreeViewItem item) => 
            this.rowHeight;

        /// <summary>
        /// <para>Returns all descendants for the item with ID id that have children.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        /// <returns>
        /// <para>Descendants that have children.</para>
        /// </returns>
        protected virtual IList<int> GetDescendantsThatHaveChildren(int id) => 
            TreeViewUtility.GetParentsBelowItem(this.FindItem(id)).ToList<int>();

        /// <summary>
        /// <para>Returns a list of TreeViewItem IDs that are currently expanded in the TreeView.</para>
        /// </summary>
        /// <returns>
        /// <para>TreeViewItem IDs.</para>
        /// </returns>
        public IList<int> GetExpanded() => 
            this.m_DataSource.GetExpandedIDs();

        private void GetExpandedRowsRecursive(TreeViewItem item, IList<TreeViewItem> expandedRows)
        {
            if (item == null)
            {
                UnityEngine.Debug.LogError("Found a TreeViewItem that is null. Invalid use of AddExpandedRows(): This method is only valid to call if you have built the full tree of TreeViewItems.");
            }
            expandedRows.Add(item);
            if (item.hasChildren && this.IsExpanded(item.id))
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    this.GetExpandedRowsRecursive(item2, expandedRows);
                }
            }
        }

        protected void GetFirstAndLastVisibleRows(out int firstRowVisible, out int lastRowVisible)
        {
            this.m_GUI.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
        }

        /// <summary>
        /// <para>Returns the horizontal foldout offset for an item. This is where the foldout arrow is rendered.</para>
        /// </summary>
        /// <param name="item">Item used to determine the indent.</param>
        /// <returns>
        /// <para>Indent for the foldout arrow.</para>
        /// </returns>
        protected float GetFoldoutIndent(TreeViewItem item) => 
            this.m_GUI.GetFoldoutIndent(item);

        /// <summary>
        /// <para>Override this method if custom GUI handling are used in RowGUI. This method for controls where the rename overlay appears.</para>
        /// </summary>
        /// <param name="rowRect">Row rect for the item currently being renamed.</param>
        /// <param name="row">Row index for the item currently being renamed.</param>
        /// <param name="item">TreeViewItem that are currently being renamed.</param>
        /// <returns>
        /// <para>The rect where the rename overlay should appear.</para>
        /// </returns>
        protected virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item) => 
            this.m_GUI.DefaultRenameRect(rowRect, row, item);

        /// <summary>
        /// <para>Get the rect for a row.</para>
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>
        /// <para>Row rect.</para>
        /// </returns>
        protected Rect GetRowRect(int row) => 
            this.m_TreeView.gui.GetRowRect(row, GUIClip.visibleRect.width);

        /// <summary>
        /// <para>This is the list of TreeViewItems that have been built in BuildRows.</para>
        /// </summary>
        /// <returns>
        /// <para>Rows.</para>
        /// </returns>
        public virtual IList<TreeViewItem> GetRows()
        {
            if (!this.isInitialized)
            {
                return null;
            }
            return this.m_TreeView.data.GetRows();
        }

        /// <summary>
        /// <para>Returns the list of TreeViewItem IDs that are currently selected.</para>
        /// </summary>
        public IList<int> GetSelection() => 
            this.m_TreeView.GetSelection();

        protected virtual DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) => 
            DragAndDropVisualMode.None;

        /// <summary>
        /// <para>Returns true if the TreeView and its EditorWindow have keyboard focus.</para>
        /// </summary>
        public bool HasFocus() => 
            this.m_TreeView.HasFocus();

        /// <summary>
        /// <para>Returns true if the TreeView has a selection.</para>
        /// </summary>
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
            this.m_TreeView.contextClickOutsideItemsCallback = (System.Action) Delegate.Combine(this.m_TreeView.contextClickOutsideItemsCallback, new System.Action(this.ContextClicked));
            this.m_TreeView.expandedStateChanged = (System.Action) Delegate.Combine(this.m_TreeView.expandedStateChanged, new System.Action(this.ExpandedStateChanged));
            this.m_TreeView.keyboardInputCallback = (System.Action) Delegate.Combine(this.m_TreeView.keyboardInputCallback, new System.Action(this.KeyEvent));
            this.m_TreeViewKeyControlID = GUIUtility.GetPermanentControlID();
        }

        protected static bool IsChildListForACollapsedParent(IList<TreeViewItem> childList) => 
            LazyTreeViewDataSource.IsChildListForACollapsedParent(childList);

        /// <summary>
        /// <para>Returns true if the TreeViewItem with ID id is currently expanded.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        public bool IsExpanded(int id) => 
            this.m_DataSource.IsExpanded(id);

        /// <summary>
        /// <para>Returns true if the TreeViewItem with ID id is currently selected.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        public bool IsSelected(int id) => 
            this.m_TreeView.IsSelected(id);

        /// <summary>
        /// <para>Override this method to handle events when the TreeView has keyboard focus.</para>
        /// </summary>
        protected virtual void KeyEvent()
        {
        }

        /// <summary>
        /// <para>This is the main GUI method of the TreeView, where the TreeViewItems are processed and drawn.</para>
        /// </summary>
        /// <param name="rect">Rect where the TreeView is rendered.</param>
        public virtual void OnGUI(Rect rect)
        {
            if (this.ValidTreeView())
            {
                this.m_TreeView.OnEvent();
                if (this.showBorder)
                {
                    rect = this.m_GUI.DoBorder(rect);
                }
                if (this.m_MultiColumnHeader != null)
                {
                    this.TreeViewWithMultiColumnHeader(rect);
                }
                else
                {
                    this.m_TreeView.OnGUI(rect, this.m_TreeViewKeyControlID);
                }
                this.CommandEventHandling();
            }
        }

        /// <summary>
        /// <para>Refreshes the cache of custom row rects based on the heights returned by GetCustomRowHeight.</para>
        /// </summary>
        protected virtual void RefreshCustomRowHeights()
        {
            if (!this.m_OverriddenMethods.hasGetCustomRowHeight)
            {
                throw new InvalidOperationException("Only call RefreshCustomRowHeights if you have overridden GetCustomRowHeight to customize the height of each row.");
            }
            this.m_GUI.RefreshRowRects(this.GetRows());
        }

        /// <summary>
        /// <para>Call this to force the TreeView to reload its data. This in turn causes BuildRoot and BuildRows to be called.</para>
        /// </summary>
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

        /// <summary>
        /// <para>Request a repaint of the window that the TreeView is rendered in.</para>
        /// </summary>
        public void Repaint()
        {
            this.m_TreeView.Repaint();
        }

        protected virtual void RowGUI(RowGUIArgs args)
        {
            this.m_GUI.DefaultRowGUI(args);
        }

        /// <summary>
        /// <para>Override the method to get notified of search string changes.</para>
        /// </summary>
        /// <param name="newSearch"></param>
        protected virtual void SearchChanged(string newSearch)
        {
        }

        /// <summary>
        /// <para>Selects all rows in the TreeView.</para>
        /// </summary>
        public void SelectAllRows()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = treeViewItem => treeViewItem.id;
            }
            List<int> selectedIDs = Enumerable.Select<TreeViewItem, int>(from treeViewItem in this.GetRows()
                where this.CanMultiSelect(treeViewItem)
                select treeViewItem, <>f__am$cache0).ToList<int>();
            this.SetSelection(selectedIDs, TreeViewSelectionOptions.FireSelectionChanged);
        }

        protected virtual void SelectionChanged(IList<int> selectedIds)
        {
        }

        /// <summary>
        /// <para>Use this method in RowGUI to peform the logic of a mouse click.</para>
        /// </summary>
        /// <param name="item">TreeViewItem clicked.</param>
        /// <param name="keepMultiSelection">If true then keeps the multiselection when clicking on a item already part of the selection. If false then clears the selection before selecting the item clicked. For left button clicks this is usually false. For context clicks it is usually true so a context opereration can operate on the multiselection.</param>
        protected void SelectionClick(TreeViewItem item, bool keepMultiSelection)
        {
            this.m_TreeView.SelectionClick(item, keepMultiSelection);
        }

        public void SetExpanded(IList<int> ids)
        {
            this.m_DataSource.SetExpandedIDs(ids.ToArray<int>());
        }

        /// <summary>
        /// <para>Set a single TreeViewItem to be expanded or collapsed.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        /// <param name="expanded">True expands item. False collapses item.</param>
        /// <returns>
        /// <para>True if item changed expanded state, false if item already had the expanded state.</para>
        /// </returns>
        public bool SetExpanded(int id, bool expanded) => 
            this.m_DataSource.SetExpanded(id, expanded);

        /// <summary>
        /// <para>Expand or collapse all items under item with id.</para>
        /// </summary>
        /// <param name="id">TreeViewItem ID.</param>
        /// <param name="expanded">Expanded state: true expands, false collapses.</param>
        public void SetExpandedRecursive(int id, bool expanded)
        {
            this.m_DataSource.SetExpandedWithChildren(id, expanded);
        }

        /// <summary>
        /// <para>Calling this function changes the keyboard focus to the TreeView.</para>
        /// </summary>
        public void SetFocus()
        {
            GUIUtility.keyboardControl = this.m_TreeViewKeyControlID;
            EditorGUIUtility.editingTextField = false;
        }

        /// <summary>
        /// <para>Calling this function changes the keyboard focus to the TreeView and ensures an item is selected. Use this function to enable key navigation of the TreeView.</para>
        /// </summary>
        public void SetFocusAndEnsureSelectedItem()
        {
            this.SetFocus();
            if (this.GetRows().Count > 0)
            {
                if (this.m_TreeView.IsLastClickedPartOfRows())
                {
                    this.FrameItem(this.state.lastClickedID);
                }
                else
                {
                    int[] selectedIDs = new int[] { this.GetRows()[0].id };
                    this.SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
                }
            }
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

        /// <summary>
        /// <para>Utility method using the depth of the input TreeViewItem to set the correct depths for all its descendant TreeViewItems.</para>
        /// </summary>
        /// <param name="root">TreeViewItem from which the descendentans should have their depth updated.</param>
        protected static void SetupDepthsFromParentsAndChildren(TreeViewItem root)
        {
            TreeViewUtility.SetDepthValuesForItems(root);
        }

        protected virtual void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
        }

        protected static void SetupParentsAndChildrenFromDepths(TreeViewItem root, IList<TreeViewItem> rows)
        {
            TreeViewUtility.SetChildParentReferences(rows, root);
        }

        protected IList<int> SortItemIDsInRowOrder(IList<int> ids) => 
            this.m_TreeView.SortIDsInVisiblityOrder(ids);

        private void TreeViewWithMultiColumnHeader(Rect rect)
        {
            Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_MultiColumnHeader.height);
            Rect rect3 = new Rect(rect.x, rect2.yMax, rect.width, rect.height - rect2.height);
            float xScroll = Mathf.Max(this.m_TreeView.state.scrollPos.x, 0f);
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect2.Contains(current.mousePosition))
            {
                GUIUtility.keyboardControl = this.m_TreeViewKeyControlID;
            }
            this.m_MultiColumnHeader.OnGUI(rect2, xScroll);
            this.m_TreeView.OnGUI(rect3, this.m_TreeViewKeyControlID);
        }

        private bool ValidTreeView()
        {
            if (this.isInitialized)
            {
                return true;
            }
            if (!this.m_WarnedUser)
            {
                UnityEngine.Debug.LogError("TreeView has not been properly intialized yet. Ensure to call Reload() before using the tree view.");
                this.m_WarnedUser = true;
            }
            return false;
        }

        /// <summary>
        /// <para>Indent used for all rows before the tree foldout arrows and content.</para>
        /// </summary>
        protected float baseIndent
        {
            get => 
                this.m_GUI.k_BaseIndent;
            set
            {
                this.m_GUI.k_BaseIndent = value;
            }
        }

        /// <summary>
        /// <para>When using a MultiColumnHeader this value adjusts the cell rects provided for all columns except the tree foldout column.</para>
        /// </summary>
        protected float cellMargin
        {
            get => 
                this.m_GUI.cellMargin;
            set
            {
                this.m_GUI.cellMargin = value;
            }
        }

        /// <summary>
        /// <para>When using a MultiColumnHeader this value should be set to the column index in which the foldout arrows should appear.</para>
        /// </summary>
        protected int columnIndexForTreeFoldouts
        {
            get => 
                this.m_GUI.columnIndexForTreeFoldouts;
            set
            {
                if (this.multiColumnHeader == null)
                {
                    throw new InvalidOperationException("Setting columnIndexForTreeFoldouts can only be set when using TreeView with a MultiColumnHeader");
                }
                if ((value < 0) || (value >= this.multiColumnHeader.state.columns.Length))
                {
                    throw new ArgumentOutOfRangeException("value", $"Invalid index for columnIndexForTreeFoldouts: {value}. Number of available columns: {this.multiColumnHeader.state.columns.Length}");
                }
                this.m_GUI.columnIndexForTreeFoldouts = value;
            }
        }

        /// <summary>
        /// <para>Custom vertical offset of the foldout arrow.</para>
        /// </summary>
        protected float customFoldoutYOffset
        {
            get => 
                this.m_GUI.customFoldoutYOffset;
            set
            {
                this.m_GUI.customFoldoutYOffset = value;
            }
        }

        /// <summary>
        /// <para>Value that returns how far the foldouts are indented for each increasing depth value.</para>
        /// </summary>
        protected float depthIndentWidth =>
            this.m_GUI.k_IndentWidth;

        /// <summary>
        /// <para>Value to control the spacing before the default icon and label. Can be used e.g for placing a toggle button to the left of the content.</para>
        /// </summary>
        protected float extraSpaceBeforeIconAndLabel
        {
            get => 
                this.m_GUI.extraSpaceBeforeIconAndLabel;
            set
            {
                this.m_GUI.extraSpaceBeforeIconAndLabel = value;
            }
        }

        /// <summary>
        /// <para>Width of the built-in foldout arrow.</para>
        /// </summary>
        protected float foldoutWidth =>
            this.m_GUI.foldoutWidth;

        /// <summary>
        /// <para>The current search state of the TreeView.</para>
        /// </summary>
        public bool hasSearch =>
            !string.IsNullOrEmpty(this.searchString);

        /// <summary>
        /// <para>True if the user is currently dragging one or more items in the TreeView, and false otherwise.</para>
        /// </summary>
        protected bool isDragging =>
            this.m_TreeView.isDragging;

        /// <summary>
        /// <para>The TreeView is initialized by calling Reload(). Therefore returns false until Reload() is called the first time.</para>
        /// </summary>
        protected bool isInitialized =>
            this.m_DataSource.isInitialized;

        /// <summary>
        /// <para>Get the MultiColumnHeader of the TreeView. Can be null if the TreeView was created without a MultiColumnHeader.</para>
        /// </summary>
        public MultiColumnHeader multiColumnHeader
        {
            get => 
                this.m_MultiColumnHeader;
            set
            {
                this.m_MultiColumnHeader = value;
            }
        }

        /// <summary>
        /// <para>The hidden root item of the TreeView (it is never rendered).</para>
        /// </summary>
        protected TreeViewItem rootItem =>
            this.m_TreeView.data.root;

        /// <summary>
        /// <para>The fixed height used for each row in the TreeView if GetCustomRowHeight have not been overridden.</para>
        /// </summary>
        protected float rowHeight
        {
            get => 
                this.m_GUI.k_LineHeight;
            set
            {
                this.m_GUI.k_LineHeight = Mathf.Max(value, EditorGUIUtility.singleLineHeight);
            }
        }

        /// <summary>
        /// <para>Current search string of the TreeView.</para>
        /// </summary>
        public string searchString
        {
            get => 
                this.m_TreeView.searchString;
            set
            {
                this.m_TreeView.searchString = value;
            }
        }

        /// <summary>
        /// <para>Enable this to show alternating row background colors.</para>
        /// </summary>
        protected bool showAlternatingRowBackgrounds { get; set; }

        /// <summary>
        /// <para>Enable this to show a border around the TreeView.</para>
        /// </summary>
        protected bool showBorder { get; set; }

        /// <summary>
        /// <para>Returns true if the horizontal scroll bar is showing, otherwise false.</para>
        /// </summary>
        protected bool showingHorizontalScrollBar =>
            this.m_TreeView.showingHorizontalScrollBar;

        /// <summary>
        /// <para>Returns true if the vertical scroll bar is showing, otherwise false.</para>
        /// </summary>
        protected bool showingVerticalScrollBar =>
            this.m_TreeView.showingVerticalScrollBar;

        /// <summary>
        /// <para>The state of the TreeView (expanded state, selection, scroll etc.)</para>
        /// </summary>
        public TreeViewState state =>
            this.m_TreeView.state;

        /// <summary>
        /// <para>Returns the sum of the TreeView row heights, the MultiColumnHeader height (if used) and the border (if used).</para>
        /// </summary>
        public float totalHeight =>
            (this.m_GUI.totalHeight + (!this.showBorder ? 0f : (this.m_GUI.borderWidth * 2f)));

        /// <summary>
        /// <para>The controlID used by the TreeView to obtain keyboard control focus.</para>
        /// </summary>
        public int treeViewControlID
        {
            get => 
                this.m_TreeViewKeyControlID;
            set
            {
                this.m_TreeViewKeyControlID = value;
            }
        }

        /// <summary>
        /// <para>The Rect the TreeView is being rendered to.</para>
        /// </summary>
        protected Rect treeViewRect
        {
            get => 
                this.m_TreeView.GetTotalRect();
            set
            {
                this.m_TreeView.SetTotalRect(value);
            }
        }

        [CompilerGenerated]
        private sealed class <FindRows>c__AnonStorey0
        {
            internal IList<int> ids;

            internal bool <>m__0(TreeViewItem item) => 
                this.ids.Contains(item.id);
        }

        /// <summary>
        /// <para>Method arguments for the CanStartDrag virtual method.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct CanStartDragArgs
        {
            /// <summary>
            /// <para>Item about to be dragged.</para>
            /// </summary>
            public TreeViewItem draggedItem;
            /// <summary>
            /// <para>The multi-selection about to be dragged.</para>
            /// </summary>
            public IList<int> draggedItemIDs;
        }

        /// <summary>
        /// <para>Default GUI methods and properties for the TreeView class.</para>
        /// </summary>
        public static class DefaultGUI
        {
            /// <summary>
            /// <para>Draws a bold label that have correct text color when selected and/or focused.</para>
            /// </summary>
            /// <param name="rect">Rect to render the text in.</param>
            /// <param name="label">Label to render.</param>
            /// <param name="selected">Selected state used for determining text color.</param>
            /// <param name="focused">Focused state used for determining text color.</param>
            public static void BoldLabel(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.boldLabel.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            /// <summary>
            /// <para>Draws a bold right aligned label that have correct text color when selected and/or focused.</para>
            /// </summary>
            /// <param name="rect">Rect to render the text in.</param>
            /// <param name="label">Label to render.</param>
            /// <param name="selected">Selected state used for determining text color.</param>
            /// <param name="focused">Focused state used for determining text color.</param>
            public static void BoldLabelRightAligned(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.boldLabelRightAligned.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            /// <summary>
            /// <para>Draws a foldout label that have correct text color when selected and/or focused.</para>
            /// </summary>
            /// <param name="rect">Rect to render the text in.</param>
            /// <param name="label">Label to render.</param>
            /// <param name="selected">Selected state used for determining text color.</param>
            /// <param name="focused">Focused state used for determining text color.</param>
            public static void FoldoutLabel(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.foldoutLabel.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            /// <summary>
            /// <para>Draws a label that have correct text color when selected and/or focused.</para>
            /// </summary>
            /// <param name="rect">Rect to render the text in.</param>
            /// <param name="label">Label to render.</param>
            /// <param name="selected">Selected state used for determining text color.</param>
            /// <param name="focused">Focused state used for determining text color.</param>
            public static void Label(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.label.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            /// <summary>
            /// <para>Draws a right aligned label that have correct text color when selected and/or focused.</para>
            /// </summary>
            /// <param name="rect">Rect to render the text in.</param>
            /// <param name="label">Label to render.</param>
            /// <param name="selected">Selected state used for determining text color.</param>
            /// <param name="focused">Focused state used for determining text color.</param>
            public static void LabelRightAligned(Rect rect, string label, bool selected, bool focused)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    TreeView.DefaultStyles.labelRightAligned.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
                }
            }

            internal static float contentLeftMargin =>
                ((float) TreeView.DefaultStyles.foldoutLabel.margin.left);
        }

        /// <summary>
        /// <para>Default styles used by the TreeView class.</para>
        /// </summary>
        public static class DefaultStyles
        {
            /// <summary>
            /// <para>Background style used for alternating row background colors when enabling TreeView.showAlternatingRowBackgrounds.</para>
            /// </summary>
            public static GUIStyle backgroundEven = "OL EntryBackEven";
            /// <summary>
            /// <para>Background style used for alternating row background colors when enabling TreeView.showAlternatingRowBackgrounds.</para>
            /// </summary>
            public static GUIStyle backgroundOdd = "OL EntryBackOdd";
            /// <summary>
            /// <para>Bold label with alternative text color when selected and/or focused.</para>
            /// </summary>
            public static GUIStyle boldLabel;
            /// <summary>
            /// <para>Right aligned bold label with alternative text color when selected and/or focused.</para>
            /// </summary>
            public static GUIStyle boldLabelRightAligned;
            /// <summary>
            /// <para>The label that is used for foldout label with alternative text color when selected and/or focused.</para>
            /// </summary>
            public static GUIStyle foldoutLabel = new GUIStyle(TreeViewGUI.Styles.lineStyle);
            /// <summary>
            /// <para>Left aligned label with alternative text color when selected and/or focused.</para>
            /// </summary>
            public static GUIStyle label;
            /// <summary>
            /// <para>Right aligend label with alternative text color when selected and/or focused.</para>
            /// </summary>
            public static GUIStyle labelRightAligned;

            static DefaultStyles()
            {
                foldoutLabel.padding.left = 0;
                label = new GUIStyle(foldoutLabel);
                label.padding.left = 2;
                label.padding.right = 2;
                labelRightAligned = new GUIStyle(label);
                labelRightAligned.alignment = TextAnchor.UpperRight;
                boldLabel = new GUIStyle(label);
                boldLabel.font = EditorStyles.boldLabel.font;
                boldLabel.fontStyle = EditorStyles.boldLabel.fontStyle;
                boldLabelRightAligned = new GUIStyle(boldLabel);
                boldLabelRightAligned.alignment = TextAnchor.UpperRight;
            }
        }

        /// <summary>
        /// <para>Method arguments for the HandleDragAndDrop virtual method.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct DragAndDropArgs
        {
            /// <summary>
            /// <para>When dragging items the current drag can have the following 3 positions relative to the items: Upon an item, Between two items or Outside items.</para>
            /// </summary>
            public TreeView.DragAndDropPosition dragAndDropPosition;
            /// <summary>
            /// <para>The parent item is set if the drag is either upon this item or between two of its children.</para>
            /// </summary>
            public TreeViewItem parentItem;
            /// <summary>
            /// <para>This index refers to the index in the children list of the parentItem where the current drag is positioned.</para>
            /// </summary>
            public int insertAtIndex;
            /// <summary>
            /// <para>This value is false as long as the mouse button is down, when the mouse button is released it is true.</para>
            /// </summary>
            public bool performDrop;
        }

        /// <summary>
        /// <para>Enum describing the possible positions a drag can have relative to the items: upon a item, between two items or outside items.</para>
        /// </summary>
        protected enum DragAndDropPosition
        {
            UponItem,
            BetweenItems,
            OutsideItems
        }

        private class OverriddenMethods
        {
            public readonly bool hasBuildRows;
            public readonly bool hasGetCustomRowHeight;
            public readonly bool hasGetRenameRect;
            public readonly bool hasHandleDragAndDrop;
            public readonly bool hasRowGUI;

            public OverriddenMethods(TreeView treeView)
            {
                System.Type type = treeView.GetType();
                this.hasRowGUI = IsOverridden(type, "RowGUI");
                this.hasHandleDragAndDrop = IsOverridden(type, "HandleDragAndDrop");
                this.hasGetRenameRect = IsOverridden(type, "GetRenameRect");
                this.hasBuildRows = IsOverridden(type, "BuildRows");
                this.hasGetCustomRowHeight = IsOverridden(type, "GetCustomRowHeight");
                this.ValidateOverriddenMethods(treeView);
            }

            private static bool IsOverridden(System.Type type, string methodName)
            {
                MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    return !(method.GetBaseDefinition().DeclaringType == method.DeclaringType);
                }
                UnityEngine.Debug.LogError("IsOverridden: method name not found: " + methodName + " (check spelling against method declaration)");
                return false;
            }

            private void ValidateOverriddenMethods(TreeView treeView)
            {
                System.Type type = treeView.GetType();
                bool flag = IsOverridden(type, "CanRename");
                bool flag2 = IsOverridden(type, "RenameEnded");
                if (flag2 != flag)
                {
                    if (flag)
                    {
                        UnityEngine.Debug.LogError(type.Name + ": If you are overriding CanRename you should also override RenameEnded (to handle the renaming).");
                    }
                    if (flag2)
                    {
                        UnityEngine.Debug.LogError(type.Name + ": If you are overriding RenameEnded you should also override CanRename (to allow renaming).");
                    }
                }
                bool flag3 = IsOverridden(type, "CanStartDrag");
                bool flag4 = IsOverridden(type, "SetupDragAndDrop");
                if (flag3 != flag4)
                {
                    if (flag3)
                    {
                        UnityEngine.Debug.LogError(type.Name + ": If you are overriding CanStartDrag you should also override SetupDragAndDrop (to setup the drag).");
                    }
                    if (flag4)
                    {
                        UnityEngine.Debug.LogError(type.Name + ": If you are overriding SetupDragAndDrop you should also override CanStartDrag (to allow dragging).");
                    }
                }
            }
        }

        /// <summary>
        /// <para>Method arguments for the virtual method RenameEnded.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct RenameEndedArgs
        {
            /// <summary>
            /// <para>Is true if the rename is accepted.</para>
            /// </summary>
            public bool acceptedRename;
            /// <summary>
            /// <para>Item with ID that are being renamed.</para>
            /// </summary>
            public int itemID;
            /// <summary>
            /// <para>The original name when starting the rename.</para>
            /// </summary>
            public string originalName;
            /// <summary>
            /// <para>Name entered in the rename overlay.</para>
            /// </summary>
            public string newName;
        }

        /// <summary>
        /// <para>Method arguments for the virtual method RowGUI.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct RowGUIArgs
        {
            /// <summary>
            /// <para>Item for the current row being handled in TreeView.RowGUI.</para>
            /// </summary>
            public TreeViewItem item;
            /// <summary>
            /// <para>Label used for text rendering of the item displayName. Note this is an empty string when isRenaming == true.</para>
            /// </summary>
            public string label;
            /// <summary>
            /// <para>Row rect for the current row being handled.</para>
            /// </summary>
            public Rect rowRect;
            /// <summary>
            /// <para>Row index into the list of current rows.</para>
            /// </summary>
            public int row;
            /// <summary>
            /// <para>This value is true when the current row's item is part of the current selection.</para>
            /// </summary>
            public bool selected;
            /// <summary>
            /// <para>This value is true only when the TreeView has keyboard focus and the TreeView's window has focus.</para>
            /// </summary>
            public bool focused;
            /// <summary>
            /// <para>This value is true when the ::item is currently being renamed.</para>
            /// </summary>
            public bool isRenaming;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MultiColumnInfo <columnInfo>k__BackingField;
            internal MultiColumnInfo columnInfo { get; set; }
            /// <summary>
            /// <para>If using a MultiColumnHeader for the TreeView use this method to get the number of visible columns currently being shown in the MultiColumnHeader.</para>
            /// </summary>
            public int GetNumVisibleColumns()
            {
                if (!this.HasMultiColumnInfo())
                {
                    throw new NotSupportedException("Only call this method if you are using a MultiColumnHeader with the TreeView.");
                }
                return this.columnInfo.multiColumnHeaderState.visibleColumns.Length;
            }

            /// <summary>
            /// <para>If using a MultiColumnHeader for the TreeView this method can be used to convert an index from the visible columns list to a index into the actual columns in the MultiColumnHeaderState.</para>
            /// </summary>
            /// <param name="visibleColumnIndex">This index is the index into the current visible columns.</param>
            /// <returns>
            /// <para>Column index into the columns array in MultiColumnHeaderState.</para>
            /// </returns>
            public int GetColumn(int visibleColumnIndex)
            {
                if (!this.HasMultiColumnInfo())
                {
                    throw new NotSupportedException("Only call this method if you are using a MultiColumnHeader with the TreeView.");
                }
                return this.columnInfo.multiColumnHeaderState.visibleColumns[visibleColumnIndex];
            }

            /// <summary>
            /// <para>If using a MultiColumnHeader for the TreeView this method can be used to get the cell rects of a row using the visible columns of the MultiColumnHeader.</para>
            /// </summary>
            /// <param name="visibleColumnIndex">Index into the list of visible columns of the multi column header.</param>
            /// <returns>
            /// <para>Cell rect defined by the intersection between the row rect and the rect of the visible column.</para>
            /// </returns>
            public Rect GetCellRect(int visibleColumnIndex)
            {
                if (!this.HasMultiColumnInfo())
                {
                    throw new NotSupportedException("Only call this method if you are using a MultiColumnHeader with the TreeView.");
                }
                return this.columnInfo.cellRects[visibleColumnIndex];
            }

            internal bool HasMultiColumnInfo() => 
                (this.columnInfo.multiColumnHeaderState != null);
            [StructLayout(LayoutKind.Sequential)]
            internal struct MultiColumnInfo
            {
                public MultiColumnHeaderState multiColumnHeaderState;
                public Rect[] cellRects;
                internal MultiColumnInfo(MultiColumnHeaderState multiColumnHeaderState, Rect[] cellRects)
                {
                    this.multiColumnHeaderState = multiColumnHeaderState;
                    this.cellRects = cellRects;
                }
            }
        }

        /// <summary>
        /// <para>Method arguments to the virtual method SetupDragAndDrop.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct SetupDragAndDropArgs
        {
            /// <summary>
            /// <para>TreeViewItem IDs being dragged.</para>
            /// </summary>
            public IList<int> draggedItemIDs;
        }

        internal class TreeViewControlDataSource : LazyTreeViewDataSource
        {
            [CompilerGenerated]
            private static Comparison<TreeViewItem> <>f__am$cache0;
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
                base.m_NeedRefreshRows = false;
                if (base.m_RootItem == null)
                {
                    base.m_RootItem = this.m_Owner.BuildRoot();
                    this.ValidateRootItem();
                }
                base.m_Rows = this.m_Owner.BuildRows(base.m_RootItem);
                if (base.m_Rows == null)
                {
                    throw new NullReferenceException("RefreshRows should set valid list of rows.");
                }
                if (this.m_Owner.m_OverriddenMethods.hasGetCustomRowHeight)
                {
                    this.m_Owner.m_GUI.RefreshRowRects(base.m_Rows);
                }
            }

            protected override HashSet<int> GetParentsAbove(int id) => 
                new HashSet<int>(this.m_Owner.GetAncestors(id));

            protected override HashSet<int> GetParentsBelow(int id) => 
                new HashSet<int>(this.m_Owner.GetDescendantsThatHaveChildren(id));

            public override bool IsExpandable(TreeViewItem item) => 
                this.m_Owner.CanChangeExpandedState(item);

            public override bool IsRenamingItemAllowed(TreeViewItem item) => 
                this.m_Owner.CanRename(item);

            public override void ReloadData()
            {
                base.m_RootItem = null;
                base.ReloadData();
            }

            public void SearchFullTree(string search, List<TreeViewItem> result)
            {
                if (string.IsNullOrEmpty(search))
                {
                    throw new ArgumentException("Invalid search: cannot be null or empty", "search");
                }
                if (result == null)
                {
                    throw new ArgumentException("Invalid list: cannot be null", "result");
                }
                Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
                stack.Push(base.m_RootItem);
                while (stack.Count > 0)
                {
                    TreeViewItem item = stack.Pop();
                    if (item.children != null)
                    {
                        foreach (TreeViewItem item2 in item.children)
                        {
                            if (item2 != null)
                            {
                                if (this.m_Owner.DoesItemMatchSearch(item2, search))
                                {
                                    result.Add(item2);
                                }
                                stack.Push(item2);
                            }
                        }
                    }
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName);
                }
                result.Sort(<>f__am$cache0);
            }

            private void ValidateRootItem()
            {
                if (base.m_RootItem == null)
                {
                    throw new NullReferenceException("BuildRoot should set a valid root item.");
                }
                if (base.m_RootItem.depth != -1)
                {
                    UnityEngine.Debug.LogError("BuildRoot should ensure the root item has a depth == -1. The visible items start at depth == 0.");
                    base.m_RootItem.depth = -1;
                }
                if ((base.m_RootItem.children == null) && !this.m_Owner.m_OverriddenMethods.hasBuildRows)
                {
                    throw new InvalidOperationException("TreeView: 'rootItem.children == null'. Did you forget to add children? If you intend to only create the list of rows (not the full tree) then you need to override: BuildRows, GetAncestors and GetDescendantsThatHaveChildren.");
                }
            }
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
                        dragAndDropPosition = this.GetDragAndDropPosition(parentItem, targetItem),
                        insertAtIndex = TreeViewDragging.GetInsertionIndex(parentItem, targetItem, dropPosition),
                        parentItem = parentItem,
                        performDrop = perform
                    };
                    return this.m_Owner.HandleDragAndDrop(args);
                }
                return DragAndDropVisualMode.None;
            }

            private TreeView.DragAndDropPosition GetDragAndDropPosition(TreeViewItem parentItem, TreeViewItem targetItem)
            {
                if (parentItem == null)
                {
                    return TreeView.DragAndDropPosition.OutsideItems;
                }
                if (parentItem == targetItem)
                {
                    return TreeView.DragAndDropPosition.UponItem;
                }
                return TreeView.DragAndDropPosition.BetweenItems;
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
            private float <cellMargin>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private int <columnIndexForTreeFoldouts>k__BackingField;
            public float borderWidth;
            private const float k_BackgroundWidth = 100000f;
            private Rect[] m_CellRects;
            private readonly TreeView m_Owner;
            private List<Rect> m_RowRects;

            public TreeViewControlGUI(TreeViewController treeView, TreeView owner) : base(treeView)
            {
                this.borderWidth = 1f;
                this.m_Owner = owner;
                this.cellMargin = MultiColumnHeader.DefaultGUI.columnContentMargin;
            }

            public override void BeginRowGUI()
            {
                base.BeginRowGUI();
                if ((this.m_Owner.isDragging && (this.m_Owner.multiColumnHeader != null)) && (this.columnIndexForTreeFoldouts > 0))
                {
                    int visibleColumnIndex = this.m_Owner.multiColumnHeader.GetVisibleColumnIndex(this.columnIndexForTreeFoldouts);
                    base.extraInsertionMarkerIndent = this.m_Owner.multiColumnHeader.GetColumnRect(visibleColumnIndex).x;
                }
                this.m_Owner.BeforeRowsGUI();
            }

            public Rect DefaultRenameRect(Rect rowRect, int row, TreeViewItem item) => 
                base.GetRenameRect(rowRect, row, item);

            internal void DefaultRowGUI(TreeView.RowGUIArgs args)
            {
                base.OnContentGUI(args.rowRect, args.row, args.item, args.label, args.selected, args.focused, false, false);
            }

            public Rect DoBorder(Rect rect)
            {
                EditorGUI.DrawOutline(rect, this.borderWidth, EditorGUI.kSplitLineSkinnedColor.color);
                return new Rect(rect.x + this.borderWidth, rect.y + this.borderWidth, rect.width - (2f * this.borderWidth), rect.height - (2f * this.borderWidth));
            }

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
                if (!this.m_Owner.multiColumnHeader.IsColumnVisible(this.columnIndexForTreeFoldouts))
                {
                    return new Rect();
                }
                Rect cellRectForTreeFoldouts = this.m_Owner.GetCellRectForTreeFoldouts(rowRect);
                if (this.GetContentIndent(item) > cellRectForTreeFoldouts.width)
                {
                    GUIClip.Push(cellRectForTreeFoldouts, Vector2.zero, Vector2.zero, false);
                    float num = 0f;
                    cellRectForTreeFoldouts.y = num;
                    cellRectForTreeFoldouts.x = num;
                    Rect rect4 = base.DoFoldout(cellRectForTreeFoldouts, item, row);
                    GUIClip.Pop();
                    return rect4;
                }
                return base.DoFoldout(cellRectForTreeFoldouts, item, row);
            }

            public void DrawAlternatingRowBackgrounds()
            {
                if (Event.current.rawType == EventType.Repaint)
                {
                    float height = this.m_Owner.treeViewRect.height + this.m_Owner.state.scrollPos.y;
                    TreeView.DefaultStyles.backgroundOdd.Draw(new Rect(0f, 0f, 100000f, height), false, false, false, false);
                    int firstRowVisible = 0;
                    int count = this.m_Owner.GetRows().Count;
                    if (count > 0)
                    {
                        int num4;
                        this.GetFirstAndLastRowVisible(out firstRowVisible, out num4);
                        if ((firstRowVisible < 0) || (firstRowVisible >= count))
                        {
                            return;
                        }
                    }
                    Rect position = new Rect(0f, 0f, 0f, this.m_Owner.rowHeight);
                    for (int i = firstRowVisible; position.yMax < height; i++)
                    {
                        if ((i % 2) != 1)
                        {
                            if (i < count)
                            {
                                position = this.m_Owner.GetRowRect(i);
                            }
                            else if (i > 0)
                            {
                                position.y += position.height * 2f;
                            }
                            position.width = 100000f;
                            TreeView.DefaultStyles.backgroundEven.Draw(position, false, false, false, false);
                        }
                    }
                }
            }

            protected override void DrawItemBackground(Rect rect, int row, TreeViewItem item, bool selected, bool focused)
            {
                if (this.m_Owner.showAlternatingRowBackgrounds && base.m_TreeView.animatingExpansion)
                {
                    rect.width = 100000f;
                    (((row % 2) != 0) ? TreeView.DefaultStyles.backgroundOdd : TreeView.DefaultStyles.backgroundEven).Draw(rect, false, false, false, false);
                }
            }

            public override void EndRowGUI()
            {
                base.EndRowGUI();
                this.m_Owner.AfterRowsGUI();
            }

            public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
            {
                if (!this.useCustomRowRects)
                {
                    base.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
                }
                else if (base.m_TreeView.data.rowCount == 0)
                {
                    firstRowVisible = lastRowVisible = -1;
                }
                else
                {
                    int rowCount = base.m_TreeView.data.rowCount;
                    if (rowCount != this.m_RowRects.Count)
                    {
                        this.m_RowRects = null;
                        throw new InvalidOperationException($"Number of rows does not match number of row rects. Did you remember to update the row rects when BuildRootAndRows was called? Number of rows: {rowCount}, number of custom row rects: {this.m_RowRects.Count}. Falling back to fixed row height.");
                    }
                    float y = base.m_TreeView.state.scrollPos.y;
                    float height = base.m_TreeView.GetTotalRect().height;
                    int num5 = -1;
                    int num6 = -1;
                    for (int i = 0; i < this.m_RowRects.Count; i++)
                    {
                        Rect rect2 = this.m_RowRects[i];
                        if (rect2.y > y)
                        {
                            Rect rect3 = this.m_RowRects[i];
                            if (rect3.y < (y + height))
                            {
                                goto Label_013D;
                            }
                        }
                        Rect rect4 = this.m_RowRects[i];
                        System.Boolean ReflectorVariable0 = true;
                        goto Label_013E;
                    Label_013D:
                        ReflectorVariable0 = false;
                    Label_013E:
                        if (ReflectorVariable0 ? ((rect4.yMax > y) && (this.m_RowRects[i].yMax < (y + height))) : true)
                        {
                            if (num5 == -1)
                            {
                                num5 = i;
                            }
                            num6 = i;
                        }
                    }
                    if ((num5 != -1) && (num6 != -1))
                    {
                        firstRowVisible = num5;
                        lastRowVisible = num6;
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
                if (!this.useCustomRowRects)
                {
                    return base.GetRowRect(row, rowWidth);
                }
                if ((row < 0) || (row >= this.m_RowRects.Count))
                {
                    throw new ArgumentOutOfRangeException("row", $"Input row index: {row} is invalid. Number of rows rects: {this.m_RowRects.Count}. (Number of rows: {this.m_Owner.m_DataSource.rowCount})");
                }
                Rect rect2 = this.m_RowRects[row];
                rect2.width = rowWidth;
                return rect2;
            }

            public override Vector2 GetTotalSize()
            {
                Vector2 vector = !this.useCustomRowRects ? base.GetTotalSize() : new Vector2(1f, this.customRowsTotalHeight);
                if (this.m_Owner.multiColumnHeader != null)
                {
                    vector.x = Mathf.Floor(this.m_Owner.multiColumnHeader.state.widthOfAllVisibleColumns);
                }
                return vector;
            }

            protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
            {
                if (!isPinging)
                {
                    GUIUtility.GetControlID(TreeViewController.GetItemControlID(item), FocusType.Passive);
                    if (this.m_Owner.m_OverriddenMethods.hasRowGUI)
                    {
                        TreeView.RowGUIArgs args = new TreeView.RowGUIArgs {
                            rowRect = rect,
                            row = row,
                            item = item,
                            label = label,
                            selected = selected,
                            focused = focused,
                            isRenaming = this.IsRenaming(item.id)
                        };
                        if (this.m_Owner.multiColumnHeader != null)
                        {
                            int[] visibleColumns = this.m_Owner.multiColumnHeader.state.visibleColumns;
                            if ((this.m_CellRects == null) || (this.m_CellRects.Length != visibleColumns.Length))
                            {
                                this.m_CellRects = new Rect[visibleColumns.Length];
                            }
                            MultiColumnHeaderState.Column[] columns = this.m_Owner.multiColumnHeader.state.columns;
                            Rect rowRect = args.rowRect;
                            for (int i = 0; i < visibleColumns.Length; i++)
                            {
                                MultiColumnHeaderState.Column column = columns[visibleColumns[i]];
                                rowRect.width = column.width;
                                this.m_CellRects[i] = rowRect;
                                if (this.columnIndexForTreeFoldouts != visibleColumns[i])
                                {
                                    this.m_CellRects[i].x += this.cellMargin;
                                    this.m_CellRects[i].width -= 2f * this.cellMargin;
                                }
                                rowRect.x += column.width;
                            }
                            args.columnInfo = new TreeView.RowGUIArgs.MultiColumnInfo(this.m_Owner.multiColumnHeader.state, this.m_CellRects);
                        }
                        this.m_Owner.RowGUI(args);
                    }
                    else
                    {
                        base.OnContentGUI(rect, row, item, label, selected, focused, useBoldFont, false);
                    }
                }
            }

            public void RefreshRowRects(IList<TreeViewItem> rows)
            {
                if (this.m_RowRects == null)
                {
                    this.m_RowRects = new List<Rect>(rows.Count);
                }
                if (this.m_RowRects.Capacity < rows.Count)
                {
                    this.m_RowRects.Capacity = rows.Count;
                }
                this.m_RowRects.Clear();
                float y = base.k_TopRowMargin;
                for (int i = 0; i < rows.Count; i++)
                {
                    float customRowHeight = this.m_Owner.GetCustomRowHeight(i, rows[i]);
                    this.m_RowRects.Add(new Rect(0f, y, 1f, customRowHeight));
                    y += customRowHeight;
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

            public float cellMargin { get; set; }

            public int columnIndexForTreeFoldouts { get; set; }

            private float customRowsTotalHeight =>
                ((((this.m_RowRects.Count <= 0) ? 0f : this.m_RowRects[this.m_RowRects.Count - 1].yMax) + base.k_BottomRowMargin) - (!base.m_TreeView.expansionAnimator.isAnimating ? 0f : base.m_TreeView.expansionAnimator.deltaHeight));

            public float foldoutWidth =>
                TreeViewGUI.Styles.foldoutWidth;

            public float totalHeight =>
                ((!this.useCustomRowRects ? base.GetTotalSize().y : this.customRowsTotalHeight) + ((this.m_Owner.multiColumnHeader == null) ? 0f : this.m_Owner.multiColumnHeader.height));

            private bool useCustomRowRects =>
                (this.m_RowRects != null);
        }
    }
}

