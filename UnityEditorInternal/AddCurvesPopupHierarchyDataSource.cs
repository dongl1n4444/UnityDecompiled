namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AddCurvesPopupHierarchyDataSource : TreeViewDataSource
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static bool <showEntireHierarchy>k__BackingField;

        public AddCurvesPopupHierarchyDataSource(TreeViewController treeView) : base(treeView)
        {
            base.showRootItem = false;
            base.rootIsCollapsable = false;
        }

        private TreeViewItem AddAnimatableObjectToHierarchy(AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] curveBindings, TreeViewItem parentNode, string path)
        {
            TreeViewItem item = new AddCurvesPopupObjectNode(parentNode, path, GetClassName(selectionItem, curveBindings[0])) {
                icon = GetIcon(selectionItem, curveBindings[0])
            };
            List<TreeViewItem> visibleItems = new List<TreeViewItem>();
            List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
            for (int i = 0; i < curveBindings.Length; i++)
            {
                EditorCurveBinding binding = curveBindings[i];
                list2.Add(binding);
                if ((i == (curveBindings.Length - 1)) || (AnimationWindowUtility.GetPropertyGroupName(curveBindings[i + 1].propertyName) != AnimationWindowUtility.GetPropertyGroupName(binding.propertyName)))
                {
                    TreeViewItem item2 = this.CreateNode(selectionItem, list2.ToArray(), item);
                    if (item2 != null)
                    {
                        visibleItems.Add(item2);
                    }
                    list2.Clear();
                }
            }
            visibleItems.Sort();
            TreeViewUtility.SetChildParentReferences(visibleItems, item);
            return item;
        }

        private TreeViewItem AddGameObjectToHierarchy(GameObject gameObject, AnimationWindowSelectionItem selectionItem, TreeViewItem parent)
        {
            string path = AnimationUtility.CalculateTransformPath(gameObject.transform, selectionItem.rootGameObject.transform);
            TreeViewItem parentNode = new AddCurvesPopupGameObjectNode(gameObject, parent, gameObject.name);
            List<TreeViewItem> visibleItems = new List<TreeViewItem>();
            if (base.m_RootItem == null)
            {
                base.m_RootItem = parentNode;
            }
            EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, selectionItem.rootGameObject);
            List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
            for (int i = 0; i < animatableBindings.Length; i++)
            {
                EditorCurveBinding item = animatableBindings[i];
                list2.Add(item);
                if (item.propertyName == "m_IsActive")
                {
                    if (item.path != "")
                    {
                        TreeViewItem item2 = this.CreateNode(selectionItem, list2.ToArray(), parentNode);
                        if (item2 != null)
                        {
                            visibleItems.Add(item2);
                        }
                        list2.Clear();
                    }
                    else
                    {
                        list2.Clear();
                    }
                }
                else
                {
                    bool flag = i == (animatableBindings.Length - 1);
                    bool flag2 = false;
                    if (!flag)
                    {
                        flag2 = !(animatableBindings[i + 1].type == item.type);
                    }
                    if (AnimationWindowUtility.IsCurveCreated(selectionItem.animationClip, item))
                    {
                        list2.Remove(item);
                    }
                    if ((item.type == typeof(Animator)) && (item.propertyName == "m_Enabled"))
                    {
                        list2.Remove(item);
                    }
                    if ((flag || flag2) && (list2.Count > 0))
                    {
                        visibleItems.Add(this.AddAnimatableObjectToHierarchy(selectionItem, list2.ToArray(), parentNode, path));
                        list2.Clear();
                    }
                }
            }
            if (showEntireHierarchy)
            {
                for (int j = 0; j < gameObject.transform.childCount; j++)
                {
                    Transform child = gameObject.transform.GetChild(j);
                    TreeViewItem item3 = this.AddGameObjectToHierarchy(child.gameObject, selectionItem, parentNode);
                    if (item3 != null)
                    {
                        visibleItems.Add(item3);
                    }
                }
            }
            TreeViewUtility.SetChildParentReferences(visibleItems, parentNode);
            return parentNode;
        }

        private TreeViewItem AddScriptableObjectToHierarchy(ScriptableObject scriptableObject, AnimationWindowSelectionItem selectionItem, TreeViewItem parent)
        {
            <AddScriptableObjectToHierarchy>c__AnonStorey0 storey = new <AddScriptableObjectToHierarchy>c__AnonStorey0 {
                selectionItem = selectionItem
            };
            EditorCurveBinding[] curveBindings = Enumerable.Where<EditorCurveBinding>(AnimationUtility.GetScriptableObjectAnimatableBindings(scriptableObject), new Func<EditorCurveBinding, bool>(storey.<>m__0)).ToArray<EditorCurveBinding>();
            TreeViewItem item = null;
            if (curveBindings.Length > 0)
            {
                item = this.AddAnimatableObjectToHierarchy(storey.selectionItem, curveBindings, parent, "");
            }
            else
            {
                item = new AddCurvesPopupObjectNode(parent, "", scriptableObject.name);
            }
            if (base.m_RootItem == null)
            {
                base.m_RootItem = item;
            }
            return item;
        }

        private TreeViewItem CreateNode(AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] curveBindings, TreeViewItem parentNode)
        {
            AddCurvesPopupPropertyNode node = new AddCurvesPopupPropertyNode(parentNode, selectionItem, curveBindings);
            if (AnimationWindowUtility.IsRectTransformPosition(node.curveBindings[0]))
            {
                node.curveBindings = new EditorCurveBinding[] { node.curveBindings[2] };
            }
            node.icon = parentNode.icon;
            return node;
        }

        public override void FetchData()
        {
            if (AddCurvesPopup.selection != null)
            {
                AnimationWindowSelectionItem[] itemArray = AddCurvesPopup.selection.ToArray();
                if (itemArray.Length > 1)
                {
                    base.m_RootItem = new AddCurvesPopupObjectNode(null, "", "");
                }
                foreach (AnimationWindowSelectionItem item in itemArray)
                {
                    if (item.canAddCurves)
                    {
                        if (item.rootGameObject != null)
                        {
                            this.AddGameObjectToHierarchy(item.rootGameObject, item, base.m_RootItem);
                        }
                        else if (item.scriptableObject != null)
                        {
                            this.AddScriptableObjectToHierarchy(item.scriptableObject, item, base.m_RootItem);
                        }
                    }
                }
                this.SetupRootNodeSettings();
                base.m_NeedRefreshRows = true;
            }
        }

        private static string GetClassName(AnimationWindowSelectionItem selectionItem, EditorCurveBinding binding)
        {
            if (selectionItem.rootGameObject != null)
            {
                UnityEngine.Object animatedObject = AnimationUtility.GetAnimatedObject(selectionItem.rootGameObject, binding);
                if (animatedObject != null)
                {
                    return ObjectNames.GetInspectorTitle(animatedObject);
                }
            }
            return binding.type.Name;
        }

        private static Texture2D GetIcon(AnimationWindowSelectionItem selectionItem, EditorCurveBinding binding)
        {
            if (selectionItem.rootGameObject != null)
            {
                return AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(selectionItem.rootGameObject, binding));
            }
            if (selectionItem.scriptableObject != null)
            {
                return AssetPreview.GetMiniThumbnail(selectionItem.scriptableObject);
            }
            return null;
        }

        private void SetupRootNodeSettings()
        {
            base.showRootItem = false;
            this.SetExpanded(base.root, true);
        }

        public void UpdateData()
        {
            base.m_TreeView.ReloadData();
        }

        public static bool showEntireHierarchy
        {
            [CompilerGenerated]
            get => 
                <showEntireHierarchy>k__BackingField;
            [CompilerGenerated]
            set
            {
                <showEntireHierarchy>k__BackingField = value;
            }
        }

        [CompilerGenerated]
        private sealed class <AddScriptableObjectToHierarchy>c__AnonStorey0
        {
            internal AnimationWindowSelectionItem selectionItem;

            internal bool <>m__0(EditorCurveBinding c) => 
                !AnimationWindowUtility.IsCurveCreated(this.selectionItem.animationClip, c);
        }
    }
}

