namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class TreeViewUtility
    {
        public static void BuildRowsUsingExpandedState(IList<TreeViewItem> rows, TreeViewItem root, TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException("treeView", "treeView is null");
            }
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
                    GetExpandedRowsRecursive(treeView, item, rows);
                }
            }
        }

        internal static void DebugPrintToEditorLogRecursive(TreeViewItem item)
        {
            if (item != null)
            {
                Console.WriteLine(new string(' ', item.depth * 3) + item.displayName);
                if (item.hasChildren)
                {
                    foreach (TreeViewItem item2 in item.children)
                    {
                        DebugPrintToEditorLogRecursive(item2);
                    }
                }
            }
        }

        internal static TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem)
        {
            return FindItemRecursive(id, searchFromThisItem);
        }

        internal static TreeViewItem FindItemInList<T>(int id, IList<T> treeViewItems) where T: TreeViewItem
        {
            <FindItemInList>c__AnonStorey1<T> storey = new <FindItemInList>c__AnonStorey1<T> {
                id = id
            };
            return Enumerable.FirstOrDefault<T>(treeViewItems, new Func<T, bool>(storey, (IntPtr) this.<>m__0));
        }

        private static TreeViewItem FindItemRecursive(int id, TreeViewItem item)
        {
            if (item != null)
            {
                if (item.id == id)
                {
                    return item;
                }
                if (!item.hasChildren)
                {
                    return null;
                }
                foreach (TreeViewItem item3 in item.children)
                {
                    TreeViewItem item4 = FindItemRecursive(id, item3);
                    if (item4 != null)
                    {
                        return item4;
                    }
                }
            }
            return null;
        }

        internal static List<TreeViewItem> FindItemsInList(IEnumerable<int> itemIDs, IList<TreeViewItem> treeViewItems)
        {
            <FindItemsInList>c__AnonStorey0 storey = new <FindItemsInList>c__AnonStorey0 {
                itemIDs = itemIDs
            };
            return Enumerable.ToList<TreeViewItem>(Enumerable.Where<TreeViewItem>(treeViewItems, new Func<TreeViewItem, bool>(storey, (IntPtr) this.<>m__0)));
        }

        private static void GetExpandedRowsRecursive(TreeView treeView, TreeViewItem item, IList<TreeViewItem> expandedRows)
        {
            expandedRows.Add(item);
            if (item.hasChildren && treeView.IsExpanded(item.id))
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    GetExpandedRowsRecursive(treeView, item2, expandedRows);
                }
            }
        }

        internal static HashSet<int> GetParentsAboveItem(TreeViewItem fromItem)
        {
            if (fromItem == null)
            {
                throw new ArgumentNullException("fromItem");
            }
            HashSet<int> set = new HashSet<int>();
            for (TreeViewItem item = fromItem.parent; item != null; item = item.parent)
            {
                set.Add(item.id);
            }
            return set;
        }

        internal static HashSet<int> GetParentsBelowItem(TreeViewItem fromItem)
        {
            if (fromItem == null)
            {
                throw new ArgumentNullException("fromItem");
            }
            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            stack.Push(fromItem);
            HashSet<int> set = new HashSet<int>();
            while (stack.Count > 0)
            {
                TreeViewItem item = stack.Pop();
                if (item.hasChildren)
                {
                    set.Add(item.id);
                    if (LazyTreeViewDataSource.IsChildListForACollapsedParent(item.children))
                    {
                        throw new InvalidOperationException("Invalid tree for finding descendants: Ensure a complete tree when using this utillity method.");
                    }
                    foreach (TreeViewItem item2 in item.children)
                    {
                        stack.Push(item2);
                    }
                }
            }
            return set;
        }

        internal static void SetChildParentReferences(IList<TreeViewItem> visibleItems, TreeViewItem root)
        {
            for (int i = 0; i < visibleItems.Count; i++)
            {
                visibleItems[i].parent = null;
            }
            int capacity = 0;
            for (int j = 0; j < visibleItems.Count; j++)
            {
                SetChildParentReferences(j, visibleItems);
                if (visibleItems[j].parent == null)
                {
                    capacity++;
                }
            }
            if (capacity > 0)
            {
                List<TreeViewItem> list = new List<TreeViewItem>(capacity);
                for (int k = 0; k < visibleItems.Count; k++)
                {
                    if (visibleItems[k].parent == null)
                    {
                        list.Add(visibleItems[k]);
                        visibleItems[k].parent = root;
                    }
                }
                root.children = list;
            }
        }

        private static void SetChildParentReferences(int parentIndex, IList<TreeViewItem> visibleItems)
        {
            TreeViewItem item = visibleItems[parentIndex];
            if (((item.children == null) || (item.children.Count <= 0)) || (item.children[0] == null))
            {
                int depth = item.depth;
                int capacity = 0;
                for (int i = parentIndex + 1; i < visibleItems.Count; i++)
                {
                    if (visibleItems[i].depth == (depth + 1))
                    {
                        capacity++;
                    }
                    if (visibleItems[i].depth <= depth)
                    {
                        break;
                    }
                }
                List<TreeViewItem> newChildList = null;
                if (capacity != 0)
                {
                    newChildList = new List<TreeViewItem>(capacity);
                    capacity = 0;
                    for (int j = parentIndex + 1; j < visibleItems.Count; j++)
                    {
                        if (visibleItems[j].depth == (depth + 1))
                        {
                            visibleItems[j].parent = item;
                            newChildList.Add(visibleItems[j]);
                            capacity++;
                        }
                        if (visibleItems[j].depth <= depth)
                        {
                            break;
                        }
                    }
                }
                SetChildren(item, newChildList);
            }
        }

        private static void SetChildren(TreeViewItem item, List<TreeViewItem> newChildList)
        {
            if (!LazyTreeViewDataSource.IsChildListForACollapsedParent(item.children) || (newChildList != null))
            {
                item.children = newChildList;
            }
        }

        public static void SetParentAndChildrenForItems(IList<TreeViewItem> rows, TreeViewItem root)
        {
            SetChildParentReferences(rows, root);
        }

        [CompilerGenerated]
        private sealed class <FindItemInList>c__AnonStorey1<T> where T: TreeViewItem
        {
            internal int id;

            internal bool <>m__0(T t)
            {
                return (t.id == this.id);
            }
        }

        [CompilerGenerated]
        private sealed class <FindItemsInList>c__AnonStorey0
        {
            internal IEnumerable<int> itemIDs;

            internal bool <>m__0(TreeViewItem x)
            {
                return Enumerable.Contains<int>(this.itemIDs, x.id);
            }
        }
    }
}

