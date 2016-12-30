﻿namespace UnityEditor.TreeViewExamples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;

    internal class LazyTestDataSource : LazyTreeViewDataSource
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <itemCounter>k__BackingField;
        private BackendData m_Backend;

        public LazyTestDataSource(TreeViewController treeView, BackendData data) : base(treeView)
        {
            this.m_Backend = data;
            this.FetchData();
        }

        private void AddVisibleChildrenRecursive(BackendData.Foo source, TreeViewItem dest)
        {
            if (this.IsExpanded(source.id))
            {
                if ((source.children != null) && (source.children.Count > 0))
                {
                    dest.children = new List<TreeViewItem>(source.children.Count);
                    for (int i = 0; i < source.children.Count; i++)
                    {
                        BackendData.Foo foo = source.children[i];
                        dest.children.Add(new FooTreeViewItem(foo.id, dest.depth + 1, dest, foo.name, foo));
                        this.itemCounter++;
                        this.AddVisibleChildrenRecursive(foo, dest.children[i]);
                    }
                }
            }
            else if (source.hasChildren)
            {
                dest.children = LazyTreeViewDataSource.CreateChildListForCollapsedParent();
            }
        }

        public override bool CanBeParent(TreeViewItem item) => 
            item.hasChildren;

        public override void FetchData()
        {
            this.itemCounter = 1;
            base.m_RootItem = new FooTreeViewItem(this.m_Backend.root.id, 0, null, this.m_Backend.root.name, this.m_Backend.root);
            this.AddVisibleChildrenRecursive(this.m_Backend.root, base.m_RootItem);
            base.m_Rows = new List<TreeViewItem>();
            base.GetVisibleItemsRecursive(base.m_RootItem, base.m_Rows);
            base.m_NeedRefreshRows = false;
        }

        protected override HashSet<int> GetParentsAbove(int id)
        {
            HashSet<int> set = new HashSet<int>();
            for (BackendData.Foo foo = BackendData.FindItemRecursive(this.m_Backend.root, id); foo != null; foo = foo.parent)
            {
                if (foo.parent != null)
                {
                    set.Add(foo.parent.id);
                }
            }
            return set;
        }

        protected override HashSet<int> GetParentsBelow(int id) => 
            this.m_Backend.GetParentsBelow(id);

        public int itemCounter { get; private set; }
    }
}

