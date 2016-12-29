namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class LazyTreeViewDataSource : TreeViewDataSource
    {
        public LazyTreeViewDataSource(TreeViewController treeView) : base(treeView)
        {
        }

        public static List<TreeViewItem> CreateChildListForCollapsedParent() => 
            new List<TreeViewItem> { null };

        public override TreeViewItem FindItem(int itemID)
        {
            this.RevealItem(itemID);
            return base.FindItem(itemID);
        }

        protected abstract HashSet<int> GetParentsAbove(int id);
        protected abstract HashSet<int> GetParentsBelow(int id);
        public override IList<TreeViewItem> GetRows()
        {
            this.InitIfNeeded();
            return base.m_Rows;
        }

        public override void InitIfNeeded()
        {
            if ((base.m_Rows == null) || base.m_NeedRefreshRows)
            {
                this.FetchData();
                base.m_NeedRefreshRows = false;
                if (base.onVisibleRowsChanged != null)
                {
                    base.onVisibleRowsChanged.Invoke();
                }
                base.m_TreeView.Repaint();
            }
        }

        public static bool IsChildListForACollapsedParent(IList<TreeViewItem> childList) => 
            (((childList != null) && (childList.Count == 1)) && (childList[0] == null));

        public override void RevealItem(int itemID)
        {
            HashSet<int> source = new HashSet<int>(base.expandedIDs);
            int count = source.Count;
            HashSet<int> parentsAbove = this.GetParentsAbove(itemID);
            source.UnionWith(parentsAbove);
            if (count != source.Count)
            {
                this.SetExpandedIDs(source.ToArray<int>());
                if (base.m_NeedRefreshRows)
                {
                    this.FetchData();
                }
            }
        }

        public override void SetExpandedWithChildren(int id, bool expand)
        {
            HashSet<int> source = new HashSet<int>(base.expandedIDs);
            HashSet<int> parentsBelow = this.GetParentsBelow(id);
            if (expand)
            {
                source.UnionWith(parentsBelow);
            }
            else
            {
                source.ExceptWith(parentsBelow);
            }
            this.SetExpandedIDs(source.ToArray<int>());
        }

        public override void SetExpandedWithChildren(TreeViewItem item, bool expand)
        {
            this.SetExpandedWithChildren(item.id, expand);
        }
    }
}

