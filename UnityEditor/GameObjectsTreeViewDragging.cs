namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class GameObjectsTreeViewDragging : TreeViewDragging
    {
        [CompilerGenerated]
        private static Func<Scene, int> <>f__am$cache0;
        private const string kSceneHeaderDragString = "SceneHeaderList";

        public GameObjectsTreeViewDragging(TreeViewController treeView) : base(treeView)
        {
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            DragAndDropVisualMode mode = this.DoDragScenes(parentItem as GameObjectTreeViewItem, targetItem as GameObjectTreeViewItem, perform, dropPos);
            if (mode != DragAndDropVisualMode.None)
            {
                return mode;
            }
            InternalEditorUtility.HierarchyDropMode kHierarchyDragNormal = InternalEditorUtility.HierarchyDropMode.kHierarchyDragNormal;
            bool flag = !string.IsNullOrEmpty(((GameObjectTreeViewDataSource) base.m_TreeView.data).searchString);
            if (flag)
            {
                kHierarchyDragNormal |= InternalEditorUtility.HierarchyDropMode.kHierarchySearchActive;
            }
            if ((parentItem == null) || (targetItem == null))
            {
                kHierarchyDragNormal |= InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon;
                return InternalEditorUtility.HierarchyWindowDrag(null, perform, kHierarchyDragNormal);
            }
            HierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
            if (!property.Find(targetItem.id, null))
            {
                property = null;
            }
            bool flag2 = dropPos == TreeViewDragging.DropPosition.Upon;
            if (flag && !flag2)
            {
                return DragAndDropVisualMode.None;
            }
            kHierarchyDragNormal |= !flag2 ? InternalEditorUtility.HierarchyDropMode.kHierarchyDropBetween : InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon;
            if ((((parentItem != null) && (targetItem != parentItem)) && (dropPos == TreeViewDragging.DropPosition.Above)) && (parentItem.children[0] == targetItem))
            {
                kHierarchyDragNormal |= InternalEditorUtility.HierarchyDropMode.kHierarchyDropAfterParent;
            }
            return InternalEditorUtility.HierarchyWindowDrag(property, perform, kHierarchyDragNormal);
        }

        private DragAndDropVisualMode DoDragScenes(GameObjectTreeViewItem parentItem, GameObjectTreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            List<Scene> genericData = DragAndDrop.GetGenericData("SceneHeaderList") as List<Scene>;
            bool flag = genericData != null;
            bool flag2 = false;
            if (!flag && (DragAndDrop.objectReferences.Length > 0))
            {
                int num = 0;
                foreach (Object obj2 in DragAndDrop.objectReferences)
                {
                    if (obj2 is SceneAsset)
                    {
                        num++;
                    }
                }
                flag2 = num == DragAndDrop.objectReferences.Length;
            }
            if (!flag && !flag2)
            {
                return DragAndDropVisualMode.None;
            }
            if (perform)
            {
                List<Scene> list2 = null;
                if (flag2)
                {
                    List<Scene> source = new List<Scene>();
                    foreach (Object obj3 in DragAndDrop.objectReferences)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(obj3);
                        Scene sceneByPath = SceneManager.GetSceneByPath(assetPath);
                        if (SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(sceneByPath))
                        {
                            base.m_TreeView.Frame(sceneByPath.handle, true, true);
                        }
                        else
                        {
                            if (Event.current.alt)
                            {
                                sceneByPath = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.AdditiveWithoutLoading);
                            }
                            else
                            {
                                sceneByPath = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
                            }
                            if (SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(sceneByPath))
                            {
                                source.Add(sceneByPath);
                            }
                        }
                    }
                    if (targetItem != null)
                    {
                        list2 = source;
                    }
                    if (source.Count > 0)
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = x => x.handle;
                        }
                        Selection.instanceIDs = Enumerable.Select<Scene, int>(source, <>f__am$cache0).ToArray<int>();
                        base.m_TreeView.Frame(source.Last<Scene>().handle, true, false);
                    }
                }
                else
                {
                    list2 = genericData;
                }
                if (list2 != null)
                {
                    if (targetItem != null)
                    {
                        Scene scene = targetItem.scene;
                        if (SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(scene))
                        {
                            if (!targetItem.isSceneHeader || (dropPos == TreeViewDragging.DropPosition.Upon))
                            {
                                dropPos = TreeViewDragging.DropPosition.Below;
                            }
                            if (dropPos == TreeViewDragging.DropPosition.Above)
                            {
                                for (int i = 0; i < list2.Count; i++)
                                {
                                    EditorSceneManager.MoveSceneBefore(list2[i], scene);
                                }
                            }
                            else if (dropPos == TreeViewDragging.DropPosition.Below)
                            {
                                for (int j = list2.Count - 1; j >= 0; j--)
                                {
                                    EditorSceneManager.MoveSceneAfter(list2[j], scene);
                                }
                            }
                        }
                    }
                    else
                    {
                        Scene sceneAt = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        for (int k = list2.Count - 1; k >= 0; k--)
                        {
                            EditorSceneManager.MoveSceneAfter(list2[k], sceneAt);
                        }
                    }
                }
            }
            return DragAndDropVisualMode.Move;
        }

        public override void DragCleanup(bool revertExpanded)
        {
            DragAndDrop.SetGenericData("SceneHeaderList", null);
            base.DragCleanup(revertExpanded);
        }

        private List<Scene> GetDraggedScenes(List<int> draggedItemIDs)
        {
            List<Scene> list = new List<Scene>();
            foreach (int num in draggedItemIDs)
            {
                Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(num);
                if (!SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(sceneByHandle))
                {
                    return null;
                }
                list.Add(sceneByHandle);
            }
            return list;
        }

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            string dragAndDropTitle;
            DragAndDrop.PrepareStartDrag();
            draggedItemIDs = base.m_TreeView.SortIDsInVisiblityOrder(draggedItemIDs);
            if (!draggedItemIDs.Contains(draggedItem.id))
            {
                List<int> list = new List<int> {
                    draggedItem.id
                };
                draggedItemIDs = list;
            }
            Object[] dragAndDropObjects = ProjectWindowUtil.GetDragAndDropObjects(draggedItem.id, draggedItemIDs);
            DragAndDrop.objectReferences = dragAndDropObjects;
            List<Scene> draggedScenes = this.GetDraggedScenes(draggedItemIDs);
            if (draggedScenes != null)
            {
                DragAndDrop.SetGenericData("SceneHeaderList", draggedScenes);
                List<string> list3 = new List<string>();
                foreach (Scene scene in draggedScenes)
                {
                    if (scene.path.Length > 0)
                    {
                        list3.Add(scene.path);
                    }
                }
                DragAndDrop.paths = list3.ToArray();
            }
            else
            {
                DragAndDrop.paths = new string[0];
            }
            if (draggedItemIDs.Count > 1)
            {
                dragAndDropTitle = "<Multiple>";
            }
            else if (dragAndDropObjects.Length == 1)
            {
                dragAndDropTitle = ObjectNames.GetDragAndDropTitle(dragAndDropObjects[0]);
            }
            else if ((draggedScenes != null) && (draggedScenes.Count == 1))
            {
                Scene scene2 = draggedScenes[0];
                dragAndDropTitle = scene2.path;
            }
            else
            {
                dragAndDropTitle = "Unhandled dragged item";
                Debug.LogError("Unhandled dragged item");
            }
            DragAndDrop.StartDrag(dragAndDropTitle);
            if (base.m_TreeView.data is GameObjectTreeViewDataSource)
            {
                ((GameObjectTreeViewDataSource) base.m_TreeView.data).SetupChildParentReferencesIfNeeded();
            }
        }
    }
}

