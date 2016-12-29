namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal abstract class TreeViewDragging : ITreeViewDragging
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <drawRowMarkerAbove>k__BackingField;
        private const double k_DropExpandTimeout = 0.7;
        protected DropData m_DropData = new DropData();
        protected TreeViewController m_TreeView;

        public TreeViewDragging(TreeViewController treeView)
        {
            this.m_TreeView = treeView;
        }

        public virtual bool CanStartDrag(TreeViewItem targetItem, List<int> draggedItemIDs, Vector2 mouseDownPosition) => 
            true;

        public abstract DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, DropPosition dropPosition);
        public virtual void DragCleanup(bool revertExpanded)
        {
            if (this.m_DropData != null)
            {
                if ((this.m_DropData.expandedArrayBeforeDrag != null) && revertExpanded)
                {
                    this.RestoreExpanded(new List<int>(this.m_DropData.expandedArrayBeforeDrag));
                }
                this.m_DropData = new DropData();
            }
        }

        public virtual bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem)
        {
            DropPosition below;
            bool perform = Event.current.type == EventType.DragPerform;
            if (targetItem == null)
            {
                if (this.m_DropData != null)
                {
                    this.m_DropData.dropTargetControlID = 0;
                    this.m_DropData.rowMarkerControlID = 0;
                }
                DragAndDrop.visualMode = this.DoDrag(null, null, perform, DropPosition.Below);
                if ((DragAndDrop.visualMode != DragAndDropVisualMode.None) && perform)
                {
                    this.FinalizeDragPerformed(true);
                }
                return false;
            }
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag3 = this.m_TreeView.data.CanBeParent(targetItem);
            Rect rect = targetItemRect;
            float betweenHalfHeight = !flag3 ? (targetItemRect.height * 0.5f) : this.m_TreeView.gui.halfDropBetweenHeight;
            if (firstItem)
            {
                rect.yMin -= betweenHalfHeight;
            }
            rect.yMax += betweenHalfHeight;
            if (!rect.Contains(mousePosition))
            {
                return false;
            }
            if (mousePosition.y >= (targetItemRect.yMax - betweenHalfHeight))
            {
                below = DropPosition.Below;
            }
            else if (firstItem && (mousePosition.y <= (targetItemRect.yMin + betweenHalfHeight)))
            {
                below = DropPosition.Above;
            }
            else
            {
                below = !flag3 ? DropPosition.Above : DropPosition.Upon;
            }
            TreeViewItem parentItem = null;
            switch (below)
            {
                case DropPosition.Upon:
                    parentItem = targetItem;
                    break;

                case DropPosition.Below:
                    if (!this.m_TreeView.data.IsExpanded(targetItem) || !targetItem.hasChildren)
                    {
                        parentItem = targetItem.parent;
                        break;
                    }
                    parentItem = targetItem;
                    targetItem = targetItem.children[0];
                    below = DropPosition.Above;
                    break;

                case DropPosition.Above:
                    parentItem = targetItem.parent;
                    break;
            }
            DragAndDropVisualMode none = DragAndDropVisualMode.None;
            if (perform)
            {
                if (below == DropPosition.Upon)
                {
                    none = this.DoDrag(targetItem, targetItem, true, below);
                }
                if ((none == DragAndDropVisualMode.None) && (parentItem != null))
                {
                    none = this.DoDrag(parentItem, targetItem, true, below);
                }
                if (none != DragAndDropVisualMode.None)
                {
                    this.FinalizeDragPerformed(false);
                }
                else
                {
                    this.DragCleanup(true);
                    this.m_TreeView.NotifyListenersThatDragEnded(null, false);
                }
            }
            else
            {
                if (this.m_DropData == null)
                {
                    this.m_DropData = new DropData();
                }
                this.m_DropData.dropTargetControlID = 0;
                this.m_DropData.rowMarkerControlID = 0;
                int itemControlID = TreeViewController.GetItemControlID(targetItem);
                this.HandleAutoExpansion(itemControlID, targetItem, targetItemRect, betweenHalfHeight, mousePosition);
                if (below == DropPosition.Upon)
                {
                    none = this.DoDrag(targetItem, targetItem, false, below);
                }
                if (none != DragAndDropVisualMode.None)
                {
                    this.m_DropData.dropTargetControlID = itemControlID;
                    DragAndDrop.visualMode = none;
                }
                else if ((targetItem != null) && (parentItem != null))
                {
                    none = this.DoDrag(parentItem, targetItem, false, below);
                    if (none != DragAndDropVisualMode.None)
                    {
                        this.drawRowMarkerAbove = below == DropPosition.Above;
                        this.m_DropData.rowMarkerControlID = itemControlID;
                        DragAndDrop.visualMode = none;
                    }
                }
            }
            Event.current.Use();
            return true;
        }

        private void FinalizeDragPerformed(bool revertExpanded)
        {
            this.DragCleanup(revertExpanded);
            DragAndDrop.AcceptDrag();
            List<Object> list = new List<Object>(DragAndDrop.objectReferences);
            bool draggedItemsFromOwnTreeView = true;
            if (((list.Count > 0) && (list[0] != null)) && (TreeViewUtility.FindItemInList<TreeViewItem>(list[0].GetInstanceID(), this.m_TreeView.data.GetRows()) == null))
            {
                draggedItemsFromOwnTreeView = false;
            }
            int[] draggedIDs = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    draggedIDs[i] = list[i].GetInstanceID();
                }
            }
            this.m_TreeView.NotifyListenersThatDragEnded(draggedIDs, draggedItemsFromOwnTreeView);
        }

        public List<int> GetCurrentExpanded()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TreeViewItem, int>(null, (IntPtr) <GetCurrentExpanded>m__1);
            }
            return Enumerable.Select<TreeViewItem, int>(Enumerable.Where<TreeViewItem>(this.m_TreeView.data.GetRows(), new Func<TreeViewItem, bool>(this, (IntPtr) this.<GetCurrentExpanded>m__0)), <>f__am$cache0).ToList<int>();
        }

        public int GetDropTargetControlID() => 
            this.m_DropData.dropTargetControlID;

        internal static int GetInsertionIndex(TreeViewItem parentItem, TreeViewItem targetItem, DropPosition dropPosition)
        {
            if (parentItem != null)
            {
                if (parentItem == targetItem)
                {
                    return -1;
                }
                int index = parentItem.children.IndexOf(targetItem);
                if (index >= 0)
                {
                    int num2;
                    if (dropPosition == DropPosition.Below)
                    {
                        num2 = index + 1;
                    }
                    else
                    {
                        num2 = index;
                    }
                    return num2;
                }
                Debug.LogError("Did not find targetItem,; should be a child of parentItem");
            }
            return -1;
        }

        public int GetRowMarkerControlID() => 
            this.m_DropData.rowMarkerControlID;

        protected virtual void HandleAutoExpansion(int itemControlID, TreeViewItem targetItem, Rect targetItemRect, float betweenHalfHeight, Vector2 currentMousePos)
        {
            float contentIndent = this.m_TreeView.gui.GetContentIndent(targetItem);
            bool flag = new Rect(targetItemRect.x + contentIndent, targetItemRect.y + betweenHalfHeight, targetItemRect.width - contentIndent, targetItemRect.height - (betweenHalfHeight * 2f)).Contains(currentMousePos);
            if (((itemControlID != this.m_DropData.lastControlID) || !flag) || (this.m_DropData.expandItemBeginPosition != currentMousePos))
            {
                this.m_DropData.lastControlID = itemControlID;
                this.m_DropData.expandItemBeginTimer = Time.realtimeSinceStartup;
                this.m_DropData.expandItemBeginPosition = currentMousePos;
            }
            bool flag2 = (Time.realtimeSinceStartup - this.m_DropData.expandItemBeginTimer) > 0.7;
            bool flag3 = flag && flag2;
            if (((targetItem != null) && flag3) && (targetItem.hasChildren && !this.m_TreeView.data.IsExpanded(targetItem)))
            {
                if (this.m_DropData.expandedArrayBeforeDrag == null)
                {
                    this.m_DropData.expandedArrayBeforeDrag = this.GetCurrentExpanded().ToArray();
                }
                this.m_TreeView.data.SetExpanded(targetItem, true);
                this.m_DropData.expandItemBeginTimer = Time.realtimeSinceStartup;
                this.m_DropData.lastControlID = 0;
            }
        }

        public virtual void OnInitialize()
        {
        }

        public void RestoreExpanded(List<int> ids)
        {
            IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
            foreach (TreeViewItem item in rows)
            {
                this.m_TreeView.data.SetExpanded(item, ids.Contains(item.id));
            }
        }

        public abstract void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs);

        public bool drawRowMarkerAbove { get; set; }

        protected class DropData
        {
            public int dropTargetControlID = -1;
            public int[] expandedArrayBeforeDrag;
            public Vector2 expandItemBeginPosition;
            public double expandItemBeginTimer;
            public int lastControlID = -1;
            public int rowMarkerControlID = -1;
        }

        public enum DropPosition
        {
            Upon,
            Below,
            Above
        }
    }
}

