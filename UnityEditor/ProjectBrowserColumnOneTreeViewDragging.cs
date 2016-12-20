namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class ProjectBrowserColumnOneTreeViewDragging : AssetsTreeViewDragging
    {
        public ProjectBrowserColumnOneTreeViewDragging(TreeViewController treeView) : base(treeView)
        {
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            if (targetItem == null)
            {
                return DragAndDropVisualMode.None;
            }
            object genericData = DragAndDrop.GetGenericData(ProjectWindowUtil.k_DraggingFavoriteGenericData);
            if (genericData != null)
            {
                int instanceID = (int) genericData;
                if ((targetItem is SearchFilterTreeItem) && (parentItem is SearchFilterTreeItem))
                {
                    bool flag = SavedSearchFilters.CanMoveSavedFilter(instanceID, parentItem.id, targetItem.id, dropPos == TreeViewDragging.DropPosition.Below);
                    if (flag && perform)
                    {
                        SavedSearchFilters.MoveSavedFilter(instanceID, parentItem.id, targetItem.id, dropPos == TreeViewDragging.DropPosition.Below);
                        int[] selectedIDs = new int[] { instanceID };
                        base.m_TreeView.SetSelection(selectedIDs, false);
                        base.m_TreeView.NotifyListenersThatSelectionChanged();
                    }
                    return (!flag ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy);
                }
                return DragAndDropVisualMode.None;
            }
            if ((targetItem is SearchFilterTreeItem) && (parentItem is SearchFilterTreeItem))
            {
                string str = DragAndDrop.GetGenericData(ProjectWindowUtil.k_IsFolderGenericData) as string;
                if (str != "isFolder")
                {
                    return DragAndDropVisualMode.None;
                }
                if (perform)
                {
                    Object[] objectReferences = DragAndDrop.objectReferences;
                    if (objectReferences.Length > 0)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(objectReferences[0].GetInstanceID());
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            string name = new DirectoryInfo(assetPath).Name;
                            SearchFilter filter = new SearchFilter();
                            filter.folders = new string[] { assetPath };
                            bool addAsChild = targetItem == parentItem;
                            float listAreaGridSize = ProjectBrowserColumnOneTreeViewGUI.GetListAreaGridSize();
                            int num3 = SavedSearchFilters.AddSavedFilterAfterInstanceID(name, filter, listAreaGridSize, targetItem.id, addAsChild);
                            int[] numArray2 = new int[] { num3 };
                            base.m_TreeView.SetSelection(numArray2, false);
                            base.m_TreeView.NotifyListenersThatSelectionChanged();
                        }
                        else
                        {
                            Debug.Log("Could not get asset path from id " + objectReferences[0].GetInstanceID());
                        }
                    }
                }
                return DragAndDropVisualMode.Copy;
            }
            return base.DoDrag(parentItem, targetItem, perform, dropPos);
        }

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            if (!SavedSearchFilters.IsSavedFilter(draggedItem.id) || (draggedItem.id != SavedSearchFilters.GetRootInstanceID()))
            {
                ProjectWindowUtil.StartDrag(draggedItem.id, draggedItemIDs);
            }
        }
    }
}

