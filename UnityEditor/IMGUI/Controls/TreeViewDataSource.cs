namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal abstract class TreeViewDataSource : ITreeViewDataSource
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <alwaysAddFirstItemToSearchResult>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <rootIsCollapsable>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <showRootItem>k__BackingField;
        protected TreeViewItem m_FakeItem;
        protected bool m_NeedRefreshRows = true;
        protected TreeViewItem m_RootItem;
        protected IList<TreeViewItem> m_Rows;
        protected readonly TreeViewController m_TreeView;
        public Action onVisibleRowsChanged;

        public TreeViewDataSource(TreeViewController treeView)
        {
            this.m_TreeView = treeView;
            this.showRootItem = true;
            this.rootIsCollapsable = false;
            this.m_RootItem = null;
            this.onVisibleRowsChanged = null;
        }

        public virtual bool CanBeMultiSelected(TreeViewItem item) => 
            true;

        public virtual bool CanBeParent(TreeViewItem item) => 
            true;

        protected virtual List<TreeViewItem> ExpandedRows(TreeViewItem root)
        {
            List<TreeViewItem> items = new List<TreeViewItem>();
            this.GetVisibleItemsRecursive(this.m_RootItem, items);
            return items;
        }

        public abstract void FetchData();
        public virtual TreeViewItem FindItem(int id) => 
            TreeViewUtility.FindItem(id, this.m_RootItem);

        public virtual int[] GetExpandedIDs() => 
            this.expandedIDs.ToArray();

        public virtual TreeViewItem GetItem(int row) => 
            this.GetRows()[row];

        public virtual int GetRow(int id)
        {
            IList<TreeViewItem> rows = this.GetRows();
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual IList<TreeViewItem> GetRows()
        {
            this.InitIfNeeded();
            return this.m_Rows;
        }

        protected void GetVisibleItemsRecursive(TreeViewItem item, IList<TreeViewItem> items)
        {
            if ((item != this.m_RootItem) || this.showRootItem)
            {
                items.Add(item);
            }
            if (item.hasChildren && this.IsExpanded(item))
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    this.GetVisibleItemsRecursive(item2, items);
                }
            }
        }

        public virtual bool HasFakeItem() => 
            (this.m_FakeItem != null);

        public virtual void InitIfNeeded()
        {
            if ((this.m_Rows == null) || this.m_NeedRefreshRows)
            {
                if (this.m_RootItem != null)
                {
                    if (this.m_TreeView.isSearching)
                    {
                        this.m_Rows = this.Search(this.m_RootItem, this.m_TreeView.searchString.ToLower());
                    }
                    else
                    {
                        this.m_Rows = this.ExpandedRows(this.m_RootItem);
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("TreeView root item is null. Ensure that your TreeViewDataSource sets up at least a root item.");
                    this.m_Rows = new List<TreeViewItem>();
                }
                this.m_NeedRefreshRows = false;
                if (this.onVisibleRowsChanged != null)
                {
                    this.onVisibleRowsChanged();
                }
                this.m_TreeView.Repaint();
            }
        }

        public virtual void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
        {
            UnityEngine.Debug.LogError("InsertFakeItem missing implementation");
        }

        public virtual bool IsExpandable(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return false;
            }
            return item.hasChildren;
        }

        public virtual bool IsExpanded(int id) => 
            (this.expandedIDs.BinarySearch(id) >= 0);

        public virtual bool IsExpanded(TreeViewItem item) => 
            this.IsExpanded(item.id);

        public virtual bool IsRenamingItemAllowed(TreeViewItem item) => 
            true;

        public virtual bool IsRevealed(int id) => 
            (TreeViewController.GetIndexOfID(this.GetRows(), id) >= 0);

        public virtual void OnExpandedStateChanged()
        {
            if (this.m_TreeView.expandedStateChanged != null)
            {
                this.m_TreeView.expandedStateChanged();
            }
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnSearchChanged()
        {
            this.m_NeedRefreshRows = true;
        }

        public virtual void ReloadData()
        {
            this.m_FakeItem = null;
            this.FetchData();
        }

        public virtual void RemoveFakeItem()
        {
            if (this.HasFakeItem())
            {
                IList<TreeViewItem> rows = this.GetRows();
                int indexOfID = TreeViewController.GetIndexOfID(rows, this.m_FakeItem.id);
                if (indexOfID != -1)
                {
                    rows.RemoveAt(indexOfID);
                }
                this.m_FakeItem = null;
            }
        }

        public virtual void RevealItem(int id)
        {
            if (!this.IsRevealed(id))
            {
                TreeViewItem item = this.FindItem(id);
                if (item != null)
                {
                    for (TreeViewItem item2 = item.parent; item2 != null; item2 = item2.parent)
                    {
                        this.SetExpanded(item2, true);
                    }
                }
            }
        }

        protected virtual List<TreeViewItem> Search(TreeViewItem root, string search)
        {
            List<TreeViewItem> searchResult = new List<TreeViewItem>();
            if (this.showRootItem)
            {
                this.SearchRecursive(root, search, searchResult);
                searchResult.Sort(new TreeViewItemAlphaNumericSort());
                return searchResult;
            }
            int num = !this.alwaysAddFirstItemToSearchResult ? 0 : 1;
            if (root.hasChildren)
            {
                for (int i = num; i < root.children.Count; i++)
                {
                    this.SearchRecursive(root.children[i], search, searchResult);
                }
                searchResult.Sort(new TreeViewItemAlphaNumericSort());
                if (this.alwaysAddFirstItemToSearchResult)
                {
                    searchResult.Insert(0, root.children[0]);
                }
            }
            return searchResult;
        }

        protected void SearchRecursive(TreeViewItem item, string search, IList<TreeViewItem> searchResult)
        {
            if (item.displayName.ToLower().Contains(search))
            {
                searchResult.Add(item);
            }
            if (item.children != null)
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    this.SearchRecursive(item2, search, searchResult);
                }
            }
        }

        public virtual bool SetExpanded(int id, bool expand)
        {
            bool flag = this.IsExpanded(id);
            if (expand != flag)
            {
                if (expand)
                {
                    this.expandedIDs.Add(id);
                    this.expandedIDs.Sort();
                }
                else
                {
                    this.expandedIDs.Remove(id);
                }
                this.m_NeedRefreshRows = true;
                this.OnExpandedStateChanged();
                return true;
            }
            return false;
        }

        public virtual void SetExpanded(TreeViewItem item, bool expand)
        {
            this.SetExpanded(item.id, expand);
        }

        public virtual void SetExpandedIDs(int[] ids)
        {
            this.expandedIDs = new List<int>(ids);
            this.expandedIDs.Sort();
            this.m_NeedRefreshRows = true;
            this.OnExpandedStateChanged();
        }

        public virtual void SetExpandedWithChildren(int id, bool expand)
        {
            this.SetExpandedWithChildren(this.FindItem(id), expand);
        }

        public virtual void SetExpandedWithChildren(TreeViewItem fromItem, bool expand)
        {
            if (fromItem == null)
            {
                UnityEngine.Debug.LogError("item is null");
            }
            else
            {
                HashSet<int> parentsBelowItem = TreeViewUtility.GetParentsBelowItem(fromItem);
                HashSet<int> source = new HashSet<int>(this.expandedIDs);
                if (expand)
                {
                    source.UnionWith(parentsBelowItem);
                }
                else
                {
                    source.ExceptWith(parentsBelowItem);
                }
                this.SetExpandedIDs(source.ToArray<int>());
            }
        }

        public bool alwaysAddFirstItemToSearchResult { get; set; }

        protected List<int> expandedIDs
        {
            get => 
                this.m_TreeView.state.expandedIDs;
            set
            {
                this.m_TreeView.state.expandedIDs = value;
            }
        }

        public bool isInitialized =>
            ((this.m_RootItem != null) && (this.m_Rows != null));

        public TreeViewItem root =>
            this.m_RootItem;

        public bool rootIsCollapsable { get; set; }

        public virtual int rowCount =>
            this.GetRows().Count;

        public bool showRootItem { get; set; }
    }
}

