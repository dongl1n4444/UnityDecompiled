namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PackageExportTreeView
    {
        private PackageExport m_PackageExport;
        private List<PackageExportTreeViewItem> m_Selection = new List<PackageExportTreeViewItem>();
        private TreeViewController m_TreeView;
        private static readonly bool s_UseFoldouts = true;

        public PackageExportTreeView(PackageExport packageExport, TreeViewState treeViewState, Rect startRect)
        {
            this.m_PackageExport = packageExport;
            this.m_TreeView = new TreeViewController(this.m_PackageExport, treeViewState);
            PackageExportTreeViewDataSource data = new PackageExportTreeViewDataSource(this.m_TreeView, this);
            PackageExportTreeViewGUI gui = new PackageExportTreeViewGUI(this.m_TreeView, this);
            this.m_TreeView.Init(startRect, data, gui, null);
            this.m_TreeView.ReloadData();
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
            gui.itemWasToggled = (Action<PackageExportTreeViewItem>) Delegate.Combine(gui.itemWasToggled, new Action<PackageExportTreeViewItem>(this.ItemWasToggled));
            this.ComputeEnabledStateForFolders();
        }

        private void ComputeEnabledStateForFolders()
        {
            PackageExportTreeViewItem root = this.m_TreeView.data.root as PackageExportTreeViewItem;
            HashSet<PackageExportTreeViewItem> done = new HashSet<PackageExportTreeViewItem> {
                root
            };
            this.RecursiveComputeEnabledStateForFolders(root, done);
        }

        private void EnableChildrenRecursive(TreeViewItem parentItem, EnabledState enabled)
        {
            if (parentItem.hasChildren)
            {
                foreach (TreeViewItem item in parentItem.children)
                {
                    PackageExportTreeViewItem item2 = item as PackageExportTreeViewItem;
                    item2.enabledState = enabled;
                    this.EnableChildrenRecursive(item2, enabled);
                }
            }
        }

        private EnabledState GetFolderChildrenEnabledState(PackageExportTreeViewItem folder)
        {
            if (!folder.isFolder)
            {
                Debug.LogError("Should be a folder item!");
            }
            if (!folder.hasChildren)
            {
                return EnabledState.None;
            }
            EnabledState notSet = EnabledState.NotSet;
            PackageExportTreeViewItem item = folder.children[0] as PackageExportTreeViewItem;
            EnabledState enabledState = item.enabledState;
            for (int i = 1; i < folder.children.Count; i++)
            {
                PackageExportTreeViewItem item2 = folder.children[i] as PackageExportTreeViewItem;
                if (enabledState != item2.enabledState)
                {
                    notSet = EnabledState.Mixed;
                    break;
                }
            }
            if (notSet == EnabledState.NotSet)
            {
                notSet = (enabledState != EnabledState.All) ? EnabledState.None : EnabledState.All;
            }
            return notSet;
        }

        private void ItemWasToggled(PackageExportTreeViewItem pitem)
        {
            if (this.m_Selection.Count <= 1)
            {
                this.EnableChildrenRecursive(pitem, pitem.enabledState);
            }
            else
            {
                foreach (PackageExportTreeViewItem item in this.m_Selection)
                {
                    item.enabledState = pitem.enabledState;
                }
            }
            this.ComputeEnabledStateForFolders();
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TreeView.OnGUI(rect, controlID);
            if ((((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Space)) && ((this.m_Selection != null) && (this.m_Selection.Count > 0))) && (GUIUtility.keyboardControl == controlID))
            {
                EnabledState state = (this.m_Selection[0].enabledState == EnabledState.All) ? EnabledState.None : EnabledState.All;
                this.m_Selection[0].enabledState = state;
                this.ItemWasToggled(this.m_Selection[0]);
                Event.current.Use();
            }
        }

        private void RecursiveComputeEnabledStateForFolders(PackageExportTreeViewItem pitem, HashSet<PackageExportTreeViewItem> done)
        {
            if (pitem.isFolder)
            {
                if (pitem.hasChildren)
                {
                    foreach (TreeViewItem item in pitem.children)
                    {
                        this.RecursiveComputeEnabledStateForFolders(item as PackageExportTreeViewItem, done);
                    }
                }
                if (!done.Contains(pitem))
                {
                    EnabledState folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
                    pitem.enabledState = folderChildrenEnabledState;
                    if (folderChildrenEnabledState == EnabledState.Mixed)
                    {
                        done.Add(pitem);
                        for (PackageExportTreeViewItem item2 = pitem.parent as PackageExportTreeViewItem; item2 != null; item2 = item2.parent as PackageExportTreeViewItem)
                        {
                            if (!done.Contains(item2))
                            {
                                item2.enabledState = EnabledState.Mixed;
                                done.Add(item2);
                            }
                        }
                    }
                }
            }
        }

        private void SelectionChanged(int[] selectedIDs)
        {
            this.m_Selection = new List<PackageExportTreeViewItem>();
            IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
            foreach (TreeViewItem item in rows)
            {
                if (selectedIDs.Contains<int>(item.id))
                {
                    PackageExportTreeViewItem item2 = item as PackageExportTreeViewItem;
                    if (item2 != null)
                    {
                        this.m_Selection.Add(item2);
                    }
                }
            }
        }

        public void SetAllEnabled(EnabledState enabled)
        {
            this.EnableChildrenRecursive(this.m_TreeView.data.root, enabled);
            this.ComputeEnabledStateForFolders();
        }

        public ExportPackageItem[] items =>
            this.m_PackageExport.items;

        public enum EnabledState
        {
            All = 1,
            Mixed = 2,
            None = 0,
            NotSet = -1
        }

        private class PackageExportTreeViewDataSource : TreeViewDataSource
        {
            private PackageExportTreeView m_PackageExportView;

            public PackageExportTreeViewDataSource(TreeViewController treeView, PackageExportTreeView view) : base(treeView)
            {
                this.m_PackageExportView = view;
                base.rootIsCollapsable = false;
                base.showRootItem = false;
            }

            private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem> treeViewFolders, bool initExpandedState)
            {
                if (folderPath == "")
                {
                    return base.m_RootItem;
                }
                TreeViewItem item2 = TreeViewUtility.FindItem(folderPath.GetHashCode(), base.m_RootItem);
                if (item2 != null)
                {
                    return item2;
                }
                char[] separator = new char[] { '/' };
                string[] strArray = folderPath.Split(separator);
                string key = "";
                TreeViewItem rootItem = base.m_RootItem;
                int depth = -1;
                for (int i = 0; i < strArray.Length; i++)
                {
                    string displayName = strArray[i];
                    if (key != "")
                    {
                        key = key + '/';
                    }
                    key = key + displayName;
                    if ((i != 0) || (key != "Assets"))
                    {
                        PackageExportTreeView.PackageExportTreeViewItem item4;
                        depth++;
                        int hashCode = key.GetHashCode();
                        if (treeViewFolders.TryGetValue(key, out item4))
                        {
                            rootItem = item4;
                        }
                        else
                        {
                            PackageExportTreeView.PackageExportTreeViewItem child = new PackageExportTreeView.PackageExportTreeViewItem(null, hashCode, depth, rootItem, displayName);
                            rootItem.AddChild(child);
                            rootItem = child;
                            if (initExpandedState)
                            {
                                base.m_TreeView.state.expandedIDs.Add(hashCode);
                            }
                            treeViewFolders[key] = child;
                        }
                    }
                }
                return rootItem;
            }

            public override void FetchData()
            {
                int depth = -1;
                base.m_RootItem = new PackageExportTreeView.PackageExportTreeViewItem(null, "Assets".GetHashCode(), depth, null, "InvisibleAssetsFolder");
                bool initExpandedState = true;
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Add(base.m_RootItem.id);
                }
                ExportPackageItem[] items = this.m_PackageExportView.items;
                Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem> treeViewFolders = new Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem>();
                for (int i = 0; i < items.Length; i++)
                {
                    ExportPackageItem itemIn = items[i];
                    if (!PackageImport.HasInvalidCharInFilePath(itemIn.assetPath))
                    {
                        string fileName = Path.GetFileName(itemIn.assetPath);
                        string directoryName = Path.GetDirectoryName(itemIn.assetPath);
                        TreeViewItem parent = this.EnsureFolderPath(directoryName, treeViewFolders, initExpandedState);
                        if (parent != null)
                        {
                            int hashCode = itemIn.assetPath.GetHashCode();
                            PackageExportTreeView.PackageExportTreeViewItem child = new PackageExportTreeView.PackageExportTreeViewItem(itemIn, hashCode, parent.depth + 1, parent, fileName);
                            parent.AddChild(child);
                            if (initExpandedState)
                            {
                                base.m_TreeView.state.expandedIDs.Add(hashCode);
                            }
                            if (itemIn.isFolder)
                            {
                                treeViewFolders[itemIn.assetPath] = child;
                            }
                        }
                    }
                }
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Sort();
                }
            }

            public override bool IsExpandable(TreeViewItem item)
            {
                if (!PackageExportTreeView.s_UseFoldouts)
                {
                    return false;
                }
                return base.IsExpandable(item);
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item) => 
                false;
        }

        private class PackageExportTreeViewGUI : TreeViewGUI
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private int <showPreviewForID>k__BackingField;
            public Action<PackageExportTreeView.PackageExportTreeViewItem> itemWasToggled;
            protected float k_FoldoutWidth;
            private PackageExportTreeView m_PackageExportView;

            public PackageExportTreeViewGUI(TreeViewController treeView, PackageExportTreeView view) : base(treeView)
            {
                this.k_FoldoutWidth = 12f;
                this.m_PackageExportView = view;
                base.k_BaseIndent = 4f;
                if (!PackageExportTreeView.s_UseFoldouts)
                {
                    this.k_FoldoutWidth = 0f;
                }
            }

            private void DoIconAndText(PackageExportTreeView.PackageExportTreeViewItem item, Rect contentRect, bool selected, bool focused)
            {
                EditorGUIUtility.SetIconSize(new Vector2(base.k_IconWidth, base.k_IconWidth));
                GUIStyle lineStyle = TreeViewGUI.Styles.lineStyle;
                lineStyle.padding.left = 0;
                if (Event.current.type == EventType.Repaint)
                {
                    lineStyle.Draw(contentRect, GUIContent.Temp(item.displayName, this.GetIconForItem(item)), false, false, selected, focused);
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }

            private void DoToggle(PackageExportTreeView.PackageExportTreeViewItem pitem, Rect toggleRect)
            {
                EditorGUI.BeginChangeCheck();
                Toggle(this.m_PackageExportView.items, pitem, toggleRect);
                if (EditorGUI.EndChangeCheck())
                {
                    if ((base.m_TreeView.GetSelection().Length <= 1) || !base.m_TreeView.GetSelection().Contains<int>(pitem.id))
                    {
                        int[] selectedIDs = new int[] { pitem.id };
                        base.m_TreeView.SetSelection(selectedIDs, false);
                        base.m_TreeView.NotifyListenersThatSelectionChanged();
                    }
                    if (this.itemWasToggled != null)
                    {
                        this.itemWasToggled(pitem);
                    }
                    Event.current.Use();
                }
            }

            protected override Texture GetIconForItem(TreeViewItem tItem)
            {
                PackageExportTreeView.PackageExportTreeViewItem item = tItem as PackageExportTreeView.PackageExportTreeViewItem;
                ExportPackageItem item2 = item.item;
                if ((item2 == null) || item2.isFolder)
                {
                    return Constants.folderIcon;
                }
                Texture cachedIcon = AssetDatabase.GetCachedIcon(item2.assetPath);
                if (cachedIcon != null)
                {
                    return cachedIcon;
                }
                return InternalEditorUtility.GetIconForFile(item2.assetPath);
            }

            public override void OnRowGUI(Rect rowRect, TreeViewItem tvItem, int row, bool selected, bool focused)
            {
                base.k_IndentWidth = 18f;
                this.k_FoldoutWidth = 18f;
                PackageExportTreeView.PackageExportTreeViewItem pitem = tvItem as PackageExportTreeView.PackageExportTreeViewItem;
                bool flag = Event.current.type == EventType.Repaint;
                if (selected && flag)
                {
                    TreeViewGUI.Styles.selectionStyle.Draw(rowRect, false, false, true, focused);
                }
                if (base.m_TreeView.data.IsExpandable(tvItem))
                {
                    this.DoFoldout(rowRect, tvItem, row);
                }
                Rect toggleRect = new Rect((base.k_BaseIndent + (tvItem.depth * base.indentWidth)) + this.k_FoldoutWidth, rowRect.y, 18f, rowRect.height);
                this.DoToggle(pitem, toggleRect);
                using (new EditorGUI.DisabledScope(pitem.item == null))
                {
                    Rect contentRect = new Rect(toggleRect.xMax, rowRect.y, rowRect.width, rowRect.height);
                    this.DoIconAndText(pitem, contentRect, selected, focused);
                }
            }

            protected override void RenameEnded()
            {
            }

            private static void Toggle(ExportPackageItem[] items, PackageExportTreeView.PackageExportTreeViewItem pitem, Rect toggleRect)
            {
                bool flag = pitem.enabledState > PackageExportTreeView.EnabledState.None;
                GUIStyle toggle = EditorStyles.toggle;
                if (pitem.isFolder && (pitem.enabledState == PackageExportTreeView.EnabledState.Mixed))
                {
                    toggle = EditorStyles.toggleMixed;
                }
                bool flag3 = GUI.Toggle(toggleRect, flag, GUIContent.none, toggle);
                if (flag3 != flag)
                {
                    pitem.enabledState = !flag3 ? PackageExportTreeView.EnabledState.None : PackageExportTreeView.EnabledState.All;
                }
            }

            public int showPreviewForID { get; set; }

            internal static class Constants
            {
                public static Texture2D folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            }
        }

        private class PackageExportTreeViewItem : TreeViewItem
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private ExportPackageItem <item>k__BackingField;
            private PackageExportTreeView.EnabledState m_EnabledState;

            public PackageExportTreeViewItem(ExportPackageItem itemIn, int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
                this.m_EnabledState = PackageExportTreeView.EnabledState.NotSet;
                this.item = itemIn;
            }

            public PackageExportTreeView.EnabledState enabledState
            {
                get => 
                    ((this.item == null) ? this.m_EnabledState : ((PackageExportTreeView.EnabledState) this.item.enabledStatus));
                set
                {
                    if (this.item != null)
                    {
                        this.item.enabledStatus = (int) value;
                    }
                    else
                    {
                        this.m_EnabledState = value;
                    }
                }
            }

            public bool isFolder =>
                ((this.item == null) || this.item.isFolder);

            public ExportPackageItem item { get; set; }
        }
    }
}

