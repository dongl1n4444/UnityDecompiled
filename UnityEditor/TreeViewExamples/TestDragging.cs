namespace UnityEditor.TreeViewExamples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    internal class TestDragging : TreeViewDragging
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TreeViewItem, BackendData.Foo> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TreeViewItem, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache3;
        private const string k_GenericDragID = "FooDragging";
        private BackendData m_BackendData;

        public TestDragging(TreeViewController treeView, BackendData data) : base(treeView)
        {
            this.m_BackendData = data;
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            FooDragData genericData = DragAndDrop.GetGenericData("FooDragging") as FooDragData;
            FooTreeViewItem item = parentItem as FooTreeViewItem;
            if ((item != null) && (genericData != null))
            {
                bool flag = this.ValidDrag(parentItem, genericData.m_DraggedItems);
                if (perform && flag)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<TreeViewItem, bool>(null, (IntPtr) <DoDrag>m__0);
                    }
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = new Func<TreeViewItem, BackendData.Foo>(null, (IntPtr) <DoDrag>m__1);
                    }
                    List<BackendData.Foo> draggedItems = Enumerable.Select<TreeViewItem, BackendData.Foo>(Enumerable.Where<TreeViewItem>(genericData.m_DraggedItems, <>f__am$cache0), <>f__am$cache1).ToList<BackendData.Foo>();
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = new Func<TreeViewItem, bool>(null, (IntPtr) <DoDrag>m__2);
                    }
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = new Func<TreeViewItem, int>(null, (IntPtr) <DoDrag>m__3);
                    }
                    int[] selectedIDs = Enumerable.Select<TreeViewItem, int>(Enumerable.Where<TreeViewItem>(genericData.m_DraggedItems, <>f__am$cache2), <>f__am$cache3).ToArray<int>();
                    int insertionIndex = TreeViewDragging.GetInsertionIndex(parentItem, targetItem, dropPos);
                    this.m_BackendData.ReparentSelection(item.foo, insertionIndex, draggedItems);
                    base.m_TreeView.ReloadData();
                    base.m_TreeView.SetSelection(selectedIDs, true);
                }
                return (!flag ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
            }
            return DragAndDropVisualMode.None;
        }

        private List<TreeViewItem> GetItemsFromIDs(IEnumerable<int> draggedItemIDs) => 
            TreeViewUtility.FindItemsInList(draggedItemIDs, base.m_TreeView.data.GetRows());

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("FooDragging", new FooDragData(this.GetItemsFromIDs(draggedItemIDs)));
            DragAndDrop.objectReferences = new Object[0];
            DragAndDrop.StartDrag(draggedItemIDs.Count + " Foo" + ((draggedItemIDs.Count <= 1) ? "" : "s"));
        }

        private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
        {
            for (TreeViewItem item = parent; item != null; item = item.parent)
            {
                if (draggedItems.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        private class FooDragData
        {
            public List<TreeViewItem> m_DraggedItems;

            public FooDragData(List<TreeViewItem> draggedItems)
            {
                this.m_DraggedItems = draggedItems;
            }
        }
    }
}

