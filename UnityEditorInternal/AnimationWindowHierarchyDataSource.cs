namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AnimationWindowHierarchyDataSource : TreeViewDataSource
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <showAll>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private AnimationWindowState <state>k__BackingField;

        public AnimationWindowHierarchyDataSource(TreeViewController treeView, AnimationWindowState animationWindowState) : base(treeView)
        {
            this.state = animationWindowState;
        }

        private AnimationWindowHierarchyClipNode AddClipNodeToHierarchy(AnimationWindowSelectionItem selectedItem, AnimationWindowCurve[] curves, AnimationWindowHierarchyNode parentNode) => 
            new AnimationWindowHierarchyClipNode(parentNode, selectedItem.id, selectedItem.animationClip.name) { curves = curves };

        private AnimationWindowHierarchyPropertyGroupNode AddPropertyGroupToHierarchy(AnimationWindowSelectionItem selectedItem, AnimationWindowCurve[] curves, AnimationWindowHierarchyNode parentNode)
        {
            List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
            AnimationWindowHierarchyPropertyGroupNode node = new AnimationWindowHierarchyPropertyGroupNode(curves[0].type, selectedItem.id, AnimationWindowUtility.GetPropertyGroupName(curves[0].propertyName), curves[0].path, parentNode) {
                icon = this.GetIcon(selectedItem, curves[0].binding),
                indent = curves[0].depth,
                curves = curves
            };
            foreach (AnimationWindowCurve curve in curves)
            {
                AnimationWindowHierarchyPropertyNode item = this.AddPropertyToHierarchy(selectedItem, curve, node);
                item.displayName = AnimationWindowUtility.GetPropertyDisplayName(item.propertyName);
                list.Add(item);
            }
            TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), node);
            return node;
        }

        private AnimationWindowHierarchyPropertyNode AddPropertyToHierarchy(AnimationWindowSelectionItem selectedItem, AnimationWindowCurve curve, AnimationWindowHierarchyNode parentNode)
        {
            AnimationWindowHierarchyPropertyNode node = new AnimationWindowHierarchyPropertyNode(curve.type, selectedItem.id, curve.propertyName, curve.path, parentNode, curve.binding, curve.isPPtrCurve);
            if (parentNode.icon != null)
            {
                node.icon = parentNode.icon;
            }
            else
            {
                node.icon = this.GetIcon(selectedItem, curve.binding);
            }
            node.indent = curve.depth;
            node.curves = new AnimationWindowCurve[] { curve };
            return node;
        }

        public List<AnimationWindowHierarchyNode> CreateTreeFromCurves()
        {
            List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
            List<AnimationWindowCurve> list2 = new List<AnimationWindowCurve>();
            foreach (AnimationWindowSelectionItem item in this.state.selection.ToArray())
            {
                AnimationWindowCurve[] curves = item.curves.ToArray();
                AnimationWindowHierarchyNode rootItem = (AnimationWindowHierarchyNode) base.m_RootItem;
                if (this.state.selection.count > 1)
                {
                    AnimationWindowHierarchyNode node2 = this.AddClipNodeToHierarchy(item, curves, rootItem);
                    list.Add(node2);
                    rootItem = node2;
                }
                for (int i = 0; i < curves.Length; i++)
                {
                    AnimationWindowCurve curve = curves[i];
                    AnimationWindowCurve curve2 = (i >= (curves.Length - 1)) ? null : curves[i + 1];
                    list2.Add(curve);
                    bool flag = (curve2 != null) && (AnimationWindowUtility.GetPropertyGroupName(curve2.propertyName) == AnimationWindowUtility.GetPropertyGroupName(curve.propertyName));
                    bool flag2 = ((curve2 != null) && curve.path.Equals(curve2.path)) && (curve.type == curve2.type);
                    if (((i == (curves.Length - 1)) || !flag) || !flag2)
                    {
                        if (list2.Count > 1)
                        {
                            list.Add(this.AddPropertyGroupToHierarchy(item, list2.ToArray(), rootItem));
                        }
                        else
                        {
                            list.Add(this.AddPropertyToHierarchy(item, list2[0], rootItem));
                        }
                        list2.Clear();
                    }
                }
            }
            return list;
        }

        public override void FetchData()
        {
            base.m_RootItem = this.GetEmptyRootNode();
            this.SetupRootNodeSettings();
            base.m_NeedRefreshRows = true;
            if (this.state.selection.disabled)
            {
                base.root.children = null;
            }
            else
            {
                List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
                if (this.state.allCurves.Count > 0)
                {
                    AnimationWindowHierarchyMasterNode item = new AnimationWindowHierarchyMasterNode {
                        curves = this.state.allCurves.ToArray()
                    };
                    list.Add(item);
                }
                list.AddRange(this.CreateTreeFromCurves());
                list.Add(new AnimationWindowHierarchyAddButtonNode());
                TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), base.root);
            }
        }

        private AnimationWindowHierarchyNode GetEmptyRootNode() => 
            new AnimationWindowHierarchyNode(0, -1, null, null, "", "", "root");

        public Texture2D GetIcon(AnimationWindowSelectionItem selectedItem, EditorCurveBinding curveBinding)
        {
            if (selectedItem.rootGameObject != null)
            {
                UnityEngine.Object animatedObject = AnimationUtility.GetAnimatedObject(selectedItem.rootGameObject, curveBinding);
                if (animatedObject != null)
                {
                    return AssetPreview.GetMiniThumbnail(animatedObject);
                }
            }
            return AssetPreview.GetMiniTypeThumbnail(curveBinding.type);
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            if (((item is AnimationWindowHierarchyAddButtonNode) || (item is AnimationWindowHierarchyMasterNode)) || (item is AnimationWindowHierarchyClipNode))
            {
                return false;
            }
            if ((item as AnimationWindowHierarchyNode).path.Length == 0)
            {
                return false;
            }
            return true;
        }

        private void SetupRootNodeSettings()
        {
            base.showRootItem = false;
            base.rootIsCollapsable = false;
            this.SetExpanded(base.m_RootItem, true);
        }

        public void UpdateData()
        {
            base.m_TreeView.ReloadData();
        }

        public bool showAll { get; set; }

        private AnimationWindowState state { get; set; }
    }
}

