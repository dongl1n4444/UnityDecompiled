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

    internal class PackageImportTreeView
    {
        private PackageImport m_PackageImport;
        private List<PackageImportTreeViewItem> m_Selection = new List<PackageImportTreeViewItem>();
        private TreeViewController m_TreeView;
        private static readonly bool s_UseFoldouts = true;

        public PackageImportTreeView(PackageImport packageImport, TreeViewState treeViewState, Rect startRect)
        {
            this.m_PackageImport = packageImport;
            this.m_TreeView = new TreeViewController(this.m_PackageImport, treeViewState);
            PackageImportTreeViewDataSource data = new PackageImportTreeViewDataSource(this.m_TreeView, this);
            PackageImportTreeViewGUI gui = new PackageImportTreeViewGUI(this.m_TreeView, this);
            this.m_TreeView.Init(startRect, data, gui, null);
            this.m_TreeView.ReloadData();
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
            gui.itemWasToggled = (Action<PackageImportTreeViewItem>) Delegate.Combine(gui.itemWasToggled, new Action<PackageImportTreeViewItem>(this.ItemWasToggled));
            this.ComputeEnabledStateForFolders();
        }

        private void ComputeEnabledStateForFolders()
        {
            PackageImportTreeViewItem root = this.m_TreeView.data.root as PackageImportTreeViewItem;
            HashSet<PackageImportTreeViewItem> done = new HashSet<PackageImportTreeViewItem> {
                root
            };
            this.RecursiveComputeEnabledStateForFolders(root, done);
        }

        private void EnableChildrenRecursive(TreeViewItem parentItem, EnabledState state)
        {
            if (parentItem.hasChildren)
            {
                foreach (TreeViewItem item in parentItem.children)
                {
                    PackageImportTreeViewItem item2 = item as PackageImportTreeViewItem;
                    item2.enableState = state;
                    this.EnableChildrenRecursive(item2, state);
                }
            }
        }

        private EnabledState GetFolderChildrenEnabledState(PackageImportTreeViewItem folder)
        {
            if ((folder.item != null) && !folder.item.isFolder)
            {
                Debug.LogError("Should be a folder item!");
            }
            if (!folder.hasChildren)
            {
                return EnabledState.None;
            }
            EnabledState notSet = EnabledState.NotSet;
            int num = 0;
            while (num < folder.children.Count)
            {
                PackageImportTreeViewItem pitem = folder.children[num] as PackageImportTreeViewItem;
                if (this.ItemShouldBeConsideredForEnabledCheck(pitem))
                {
                    notSet = pitem.enableState;
                    break;
                }
                num++;
            }
            num++;
            while (num < folder.children.Count)
            {
                PackageImportTreeViewItem item2 = folder.children[num] as PackageImportTreeViewItem;
                if (this.ItemShouldBeConsideredForEnabledCheck(item2) && (notSet != item2.enableState))
                {
                    notSet = EnabledState.Mixed;
                    break;
                }
                num++;
            }
            if (notSet == EnabledState.NotSet)
            {
                return EnabledState.None;
            }
            return notSet;
        }

        private bool ItemShouldBeConsideredForEnabledCheck(PackageImportTreeViewItem pitem)
        {
            if (pitem == null)
            {
                return false;
            }
            if (pitem.item != null)
            {
                ImportPackageItem item = pitem.item;
                if (item.projectAsset || ((!item.isFolder && !item.assetChanged) && !this.doReInstall))
                {
                    return false;
                }
            }
            return true;
        }

        private void ItemWasToggled(PackageImportTreeViewItem pitem)
        {
            if (this.m_Selection.Count <= 1)
            {
                this.EnableChildrenRecursive(pitem, pitem.enableState);
            }
            else
            {
                foreach (PackageImportTreeViewItem item in this.m_Selection)
                {
                    item.enableState = pitem.enableState;
                }
            }
            this.ComputeEnabledStateForFolders();
        }

        public void OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                PopupWindowWithoutFocus.Hide();
            }
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TreeView.OnGUI(rect, controlID);
            if ((((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Space)) && ((this.m_Selection != null) && (this.m_Selection.Count > 0))) && (GUIUtility.keyboardControl == controlID))
            {
                PackageImportTreeViewItem item = this.m_Selection[0];
                if (item != null)
                {
                    EnabledState state = (item.enableState != EnabledState.None) ? EnabledState.None : EnabledState.All;
                    item.enableState = state;
                    this.ItemWasToggled(this.m_Selection[0]);
                }
                Event.current.Use();
            }
        }

        private void RecursiveComputeEnabledStateForFolders(PackageImportTreeViewItem pitem, HashSet<PackageImportTreeViewItem> done)
        {
            if ((pitem.item == null) || pitem.item.isFolder)
            {
                if (pitem.hasChildren)
                {
                    foreach (TreeViewItem item in pitem.children)
                    {
                        this.RecursiveComputeEnabledStateForFolders(item as PackageImportTreeViewItem, done);
                    }
                }
                if (!done.Contains(pitem))
                {
                    EnabledState folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
                    pitem.enableState = folderChildrenEnabledState;
                    if (folderChildrenEnabledState == EnabledState.Mixed)
                    {
                        done.Add(pitem);
                        for (PackageImportTreeViewItem item2 = pitem.parent as PackageImportTreeViewItem; item2 != null; item2 = item2.parent as PackageImportTreeViewItem)
                        {
                            if (!done.Contains(item2))
                            {
                                item2.enableState = EnabledState.Mixed;
                                done.Add(item2);
                            }
                        }
                    }
                }
            }
        }

        private void SelectionChanged(int[] selectedIDs)
        {
            this.m_Selection = new List<PackageImportTreeViewItem>();
            IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
            foreach (TreeViewItem item in rows)
            {
                if (selectedIDs.Contains<int>(item.id))
                {
                    PackageImportTreeViewItem item2 = item as PackageImportTreeViewItem;
                    if (item2 != null)
                    {
                        this.m_Selection.Add(item2);
                    }
                }
            }
            ImportPackageItem item3 = this.m_Selection[0].item;
            if (((this.m_Selection.Count == 1) && (item3 != null)) && !string.IsNullOrEmpty(item3.previewPath))
            {
                PackageImportTreeViewGUI gui = this.m_TreeView.gui as PackageImportTreeViewGUI;
                gui.showPreviewForID = this.m_Selection[0].id;
            }
            else
            {
                PopupWindowWithoutFocus.Hide();
            }
        }

        public void SetAllEnabled(EnabledState state)
        {
            this.EnableChildrenRecursive(this.m_TreeView.data.root, state);
            this.ComputeEnabledStateForFolders();
        }

        public bool canReInstall =>
            this.m_PackageImport.canReInstall;

        public bool doReInstall =>
            this.m_PackageImport.doReInstall;

        public ImportPackageItem[] packageItems =>
            this.m_PackageImport.packageItems;

        public enum EnabledState
        {
            All = 1,
            Mixed = 2,
            None = 0,
            NotSet = -1
        }

        private class PackageImportTreeViewDataSource : TreeViewDataSource
        {
            private PackageImportTreeView m_PackageImportView;

            public PackageImportTreeViewDataSource(TreeViewController treeView, PackageImportTreeView view) : base(treeView)
            {
                this.m_PackageImportView = view;
                base.rootIsCollapsable = false;
                base.showRootItem = false;
            }

            private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders, bool initExpandedState)
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
                        PackageImportTreeView.PackageImportTreeViewItem item4;
                        depth++;
                        int hashCode = key.GetHashCode();
                        if (treeViewFolders.TryGetValue(key, out item4))
                        {
                            rootItem = item4;
                        }
                        else
                        {
                            PackageImportTreeView.PackageImportTreeViewItem child = new PackageImportTreeView.PackageImportTreeViewItem(null, hashCode, depth, rootItem, displayName);
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
                base.m_RootItem = new PackageImportTreeView.PackageImportTreeViewItem(null, "Assets".GetHashCode(), depth, null, "InvisibleAssetsFolder");
                bool initExpandedState = true;
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Add(base.m_RootItem.id);
                }
                ImportPackageItem[] packageItems = this.m_PackageImportView.packageItems;
                Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders = new Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem>();
                for (int i = 0; i < packageItems.Length; i++)
                {
                    ImportPackageItem itemIn = packageItems[i];
                    if (!PackageImport.HasInvalidCharInFilePath(itemIn.destinationAssetPath))
                    {
                        string fileName = Path.GetFileName(itemIn.destinationAssetPath);
                        string directoryName = Path.GetDirectoryName(itemIn.destinationAssetPath);
                        TreeViewItem parent = this.EnsureFolderPath(directoryName, treeViewFolders, initExpandedState);
                        if (parent != null)
                        {
                            int hashCode = itemIn.destinationAssetPath.GetHashCode();
                            PackageImportTreeView.PackageImportTreeViewItem child = new PackageImportTreeView.PackageImportTreeViewItem(itemIn, hashCode, parent.depth + 1, parent, fileName);
                            parent.AddChild(child);
                            if (initExpandedState)
                            {
                                base.m_TreeView.state.expandedIDs.Add(hashCode);
                            }
                            if (itemIn.isFolder)
                            {
                                treeViewFolders[itemIn.destinationAssetPath] = child;
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
                if (!PackageImportTreeView.s_UseFoldouts)
                {
                    return false;
                }
                return base.IsExpandable(item);
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item) => 
                false;
        }

        private class PackageImportTreeViewGUI : TreeViewGUI
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private int <showPreviewForID>k__BackingField;
            public Action<PackageImportTreeView.PackageImportTreeViewItem> itemWasToggled;
            protected float k_FoldoutWidth;
            private PackageImportTreeView m_PackageImportView;

            public PackageImportTreeViewGUI(TreeViewController treeView, PackageImportTreeView view) : base(treeView)
            {
                this.k_FoldoutWidth = 12f;
                this.m_PackageImportView = view;
                base.k_BaseIndent = 4f;
                if (!PackageImportTreeView.s_UseFoldouts)
                {
                    this.k_FoldoutWidth = 0f;
                }
            }

            private void DoIconAndText(TreeViewItem item, Rect contentRect, bool selected, bool focused)
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

            private void DoPreviewPopup(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect rowRect)
            {
                ImportPackageItem item = pitem.item;
                if (item != null)
                {
                    if (((Event.current.type == EventType.MouseDown) && rowRect.Contains(Event.current.mousePosition)) && !PopupWindowWithoutFocus.IsVisible())
                    {
                        this.showPreviewForID = pitem.id;
                    }
                    if ((pitem.id == this.showPreviewForID) && (Event.current.type != EventType.Layout))
                    {
                        this.showPreviewForID = 0;
                        if (!string.IsNullOrEmpty(item.previewPath))
                        {
                            Texture2D preview = PackageImport.GetPreview(item.previewPath);
                            Rect activatorRect = rowRect;
                            activatorRect.width = EditorGUIUtility.currentViewWidth;
                            PopupWindowWithoutFocus.Show(activatorRect, new PackageImportTreeView.PreviewPopup(preview), new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Right });
                        }
                    }
                }
            }

            private void DoToggle(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
            {
                EditorGUI.BeginChangeCheck();
                Toggle(this.m_PackageImportView.packageItems, pitem, toggleRect);
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

            protected override Texture GetIconForItem(TreeViewItem tvItem)
            {
                PackageImportTreeView.PackageImportTreeViewItem item = tvItem as PackageImportTreeView.PackageImportTreeViewItem;
                ImportPackageItem item2 = item.item;
                if ((item2 == null) || item2.isFolder)
                {
                    return Constants.folderIcon;
                }
                Texture cachedIcon = AssetDatabase.GetCachedIcon(item2.destinationAssetPath);
                if (cachedIcon != null)
                {
                    return cachedIcon;
                }
                return InternalEditorUtility.GetIconForFile(item2.destinationAssetPath);
            }

            public override void OnRowGUI(Rect rowRect, TreeViewItem tvItem, int row, bool selected, bool focused)
            {
                base.k_IndentWidth = 18f;
                this.k_FoldoutWidth = 18f;
                PackageImportTreeView.PackageImportTreeViewItem pitem = tvItem as PackageImportTreeView.PackageImportTreeViewItem;
                ImportPackageItem item = pitem.item;
                bool flag = Event.current.type == EventType.Repaint;
                if (selected && flag)
                {
                    TreeViewGUI.Styles.selectionStyle.Draw(rowRect, false, false, true, focused);
                }
                bool flag2 = item != null;
                bool flag3 = (item == null) || item.isFolder;
                bool flag4 = (item != null) && item.assetChanged;
                bool flag5 = (item != null) && item.pathConflict;
                bool flag6 = (item == null) || item.exists;
                bool flag7 = (item != null) && item.projectAsset;
                bool doReInstall = this.m_PackageImportView.doReInstall;
                if (base.m_TreeView.data.IsExpandable(tvItem))
                {
                    this.DoFoldout(rowRect, tvItem, row);
                }
                Rect toggleRect = new Rect((base.k_BaseIndent + (tvItem.depth * base.indentWidth)) + this.k_FoldoutWidth, rowRect.y, 18f, rowRect.height);
                if ((flag3 && !flag7) || ((flag2 && !flag7) && (flag4 || doReInstall)))
                {
                    this.DoToggle(pitem, toggleRect);
                }
                using (new EditorGUI.DisabledScope(!flag2 || flag7))
                {
                    Rect contentRect = new Rect(toggleRect.xMax, rowRect.y, rowRect.width, rowRect.height);
                    this.DoIconAndText(tvItem, contentRect, selected, focused);
                    this.DoPreviewPopup(pitem, rowRect);
                    if ((flag && flag2) && flag5)
                    {
                        Rect position = new Rect(rowRect.xMax - 58f, rowRect.y, rowRect.height, rowRect.height);
                        EditorGUIUtility.SetIconSize(new Vector2(rowRect.height, rowRect.height));
                        GUI.Label(position, Constants.badgeWarn);
                        EditorGUIUtility.SetIconSize(Vector2.zero);
                    }
                    if ((flag && flag2) && (!flag6 && !flag5))
                    {
                        Texture image = Constants.badgeNew.image;
                        Rect rect4 = new Rect((rowRect.xMax - image.width) - 6f, rowRect.y + ((rowRect.height - image.height) / 2f), (float) image.width, (float) image.height);
                        GUI.Label(rect4, Constants.badgeNew, Constants.paddinglessStyle);
                    }
                    if ((flag && doReInstall) && flag7)
                    {
                        Texture texture2 = Constants.badgeDelete.image;
                        Rect rect5 = new Rect((rowRect.xMax - texture2.width) - 6f, rowRect.y + ((rowRect.height - texture2.height) / 2f), (float) texture2.width, (float) texture2.height);
                        GUI.Label(rect5, Constants.badgeDelete, Constants.paddinglessStyle);
                    }
                    if (((flag && flag2) && (flag6 || flag5)) && flag4)
                    {
                        Texture texture3 = Constants.badgeChange.image;
                        Rect rect6 = new Rect((rowRect.xMax - texture3.width) - 6f, rowRect.y, rowRect.height, rowRect.height);
                        GUI.Label(rect6, Constants.badgeChange, Constants.paddinglessStyle);
                    }
                }
            }

            protected override void RenameEnded()
            {
            }

            private static void Toggle(ImportPackageItem[] items, PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
            {
                bool flag = pitem.enableState > PackageImportTreeView.EnabledState.None;
                bool flag2 = (pitem.item == null) || pitem.item.isFolder;
                GUIStyle toggle = EditorStyles.toggle;
                if (flag2 && (pitem.enableState == PackageImportTreeView.EnabledState.Mixed))
                {
                    toggle = EditorStyles.toggleMixed;
                }
                bool flag4 = GUI.Toggle(toggleRect, flag, GUIContent.none, toggle);
                if (flag4 != flag)
                {
                    pitem.enableState = !flag4 ? PackageImportTreeView.EnabledState.None : PackageImportTreeView.EnabledState.All;
                }
            }

            public int showPreviewForID { get; set; }

            internal static class Constants
            {
                public static GUIContent badgeChange = EditorGUIUtility.IconContent("playLoopOff", "|This file is new or has changed.");
                public static GUIContent badgeDelete = EditorGUIUtility.IconContent("AS Badge Delete", "|These files will be deleted!");
                public static GUIContent badgeNew = EditorGUIUtility.IconContent("AS Badge New", "|This is a new Asset");
                public static GUIContent badgeWarn = EditorGUIUtility.IconContent("console.warnicon", "|Warning: File exists in project, but with different GUID. Will override existing asset which may be undesired.");
                public static Texture2D folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
                public static GUIStyle paddinglessStyle = new GUIStyle();

                static Constants()
                {
                    paddinglessStyle.padding = new RectOffset(0, 0, 0, 0);
                }
            }
        }

        private class PackageImportTreeViewItem : TreeViewItem
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private ImportPackageItem <item>k__BackingField;
            private PackageImportTreeView.EnabledState m_EnableState;

            public PackageImportTreeViewItem(ImportPackageItem itemIn, int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
                this.item = itemIn;
                if (this.item == null)
                {
                    this.m_EnableState = PackageImportTreeView.EnabledState.All;
                }
                else
                {
                    this.m_EnableState = (PackageImportTreeView.EnabledState) this.item.enabledStatus;
                }
            }

            public PackageImportTreeView.EnabledState enableState
            {
                get => 
                    this.m_EnableState;
                set
                {
                    if ((this.item == null) || !this.item.projectAsset)
                    {
                        this.m_EnableState = value;
                        if (this.item != null)
                        {
                            this.item.enabledStatus = (int) value;
                        }
                    }
                }
            }

            public ImportPackageItem item { get; set; }
        }

        private class PreviewPopup : PopupWindowContent
        {
            private readonly Vector2 kPreviewSize = new Vector2(128f, 128f);
            private readonly Texture2D m_Preview;

            public PreviewPopup(Texture2D preview)
            {
                this.m_Preview = preview;
            }

            public override Vector2 GetWindowSize() => 
                this.kPreviewSize;

            public override void OnGUI(Rect rect)
            {
                PackageImport.DrawTexture(rect, this.m_Preview, false);
            }
        }
    }
}

