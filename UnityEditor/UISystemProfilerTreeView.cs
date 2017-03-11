namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    internal class UISystemProfilerTreeView : TreeView
    {
        private RootTreeViewItem m_AllCanvasesItem;
        private readonly CanvasBatchComparer m_Comparer;
        public ProfilerProperty property;

        public UISystemProfilerTreeView(State state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            this.m_Comparer = new CanvasBatchComparer();
            base.showBorder = false;
            base.showAlternatingRowBackgrounds = true;
        }

        protected override TreeViewItem BuildRoot() => 
            new TreeViewItem(0, -1);

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            this.profilerState.lastFrame = this.profilerState.profilerWindow.GetActiveVisibleFrameIndex();
            List<TreeViewItem> rows = new List<TreeViewItem>();
            if ((this.property != null) && this.property.frameDataReady)
            {
                this.m_AllCanvasesItem = new RootTreeViewItem();
                base.SetExpanded(this.m_AllCanvasesItem.id, true);
                root.AddChild(this.m_AllCanvasesItem);
                UISystemProfilerInfo[] uISystemProfilerInfo = this.property.GetUISystemProfilerInfo();
                int[] uISystemBatchInstanceIDs = this.property.GetUISystemBatchInstanceIDs();
                if (uISystemProfilerInfo != null)
                {
                    Dictionary<int, TreeViewItem> dictionary = new Dictionary<int, TreeViewItem>();
                    int num = 0;
                    foreach (UISystemProfilerInfo info in uISystemProfilerInfo)
                    {
                        TreeViewItem allCanvasesItem;
                        string uISystemProfilerNameByOffset;
                        BaseTreeViewItem item2;
                        if (!dictionary.TryGetValue(info.parentId, out allCanvasesItem))
                        {
                            allCanvasesItem = this.m_AllCanvasesItem;
                            this.m_AllCanvasesItem.totalBatchCount += info.totalBatchCount;
                            this.m_AllCanvasesItem.totalVertexCount += info.totalVertexCount;
                            this.m_AllCanvasesItem.gameObjectCount += info.instanceIDsCount;
                        }
                        if (info.isBatch)
                        {
                            uISystemProfilerNameByOffset = "Batch " + num++;
                            item2 = new BatchTreeViewItem(info, allCanvasesItem.depth + 1, uISystemProfilerNameByOffset, uISystemBatchInstanceIDs);
                        }
                        else
                        {
                            uISystemProfilerNameByOffset = this.property.GetUISystemProfilerNameByOffset(info.objectNameOffset);
                            item2 = new CanvasTreeViewItem(info, allCanvasesItem.depth + 1, uISystemProfilerNameByOffset);
                            num = 0;
                            dictionary[info.objectInstanceId] = item2;
                        }
                        if (!base.IsExpanded(allCanvasesItem.id))
                        {
                            if (!allCanvasesItem.hasChildren)
                            {
                                allCanvasesItem.children = TreeView.CreateChildListForCollapsedParent();
                            }
                        }
                        else
                        {
                            allCanvasesItem.AddChild(item2);
                        }
                    }
                    this.m_Comparer.Col = Column.Element;
                    if (base.multiColumnHeader.sortedColumnIndex != -1)
                    {
                        this.m_Comparer.Col = (Column) base.multiColumnHeader.sortedColumnIndex;
                    }
                    this.m_Comparer.isAscending = base.multiColumnHeader.GetColumn((int) this.m_Comparer.Col).sortedAscending;
                    this.SetupRows(this.m_AllCanvasesItem, rows);
                }
            }
            return rows;
        }

        protected override void ContextClickedItem(int id)
        {
            <ContextClickedItem>c__AnonStorey0 storey = new <ContextClickedItem>c__AnonStorey0 {
                id = id,
                $this = this
            };
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Find matching objects in scene"), false, new GenericMenu.MenuFunction(storey.<>m__0));
            menu.ShowAsContext();
        }

        protected override void DoubleClickedItem(int id)
        {
            List<int> selection = new List<int> {
                id
            };
            HighlightRowsMatchingObjects(this.GetRowsFromIDs(selection));
        }

        private static string FormatBatchBreakingReason(UISystemProfilerInfo info)
        {
            BatchBreakingReason batchBreakingReason = info.batchBreakingReason;
            switch (batchBreakingReason)
            {
                case BatchBreakingReason.NoBreaking:
                    return "NoBreaking";

                case BatchBreakingReason.NotCoplanarWithCanvas:
                    return "Not Coplanar With Canvas";

                case BatchBreakingReason.CanvasInjectionIndex:
                    return "Canvas Injection Index";

                case BatchBreakingReason.DifferentMaterialInstance:
                    return "Different Material Instance";

                case BatchBreakingReason.DifferentRectClipping:
                    return "Different Rect Clipping";
            }
            if (batchBreakingReason != BatchBreakingReason.DifferentTexture)
            {
                if (batchBreakingReason != BatchBreakingReason.DifferentA8TextureUsage)
                {
                    if (batchBreakingReason != BatchBreakingReason.DifferentClipRect)
                    {
                        if (batchBreakingReason != BatchBreakingReason.Unknown)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        return "Unknown";
                    }
                    return "Different Clip Rect";
                }
            }
            else
            {
                return "Different Texture";
            }
            return "Different A8 Texture Usage";
        }

        private string GetItemcontent(TreeView.RowGUIArgs args, int column)
        {
            if ((this.m_AllCanvasesItem == null) || (args.item.id != this.m_AllCanvasesItem.id))
            {
                BatchTreeViewItem item = args.item as BatchTreeViewItem;
                if (item == null)
                {
                    CanvasTreeViewItem item2 = args.item as CanvasTreeViewItem;
                    if (item2 != null)
                    {
                        UISystemProfilerInfo info2 = item2.info;
                        switch (((Column) column))
                        {
                            case Column.Element:
                            case Column.VertexCount:
                            case Column.BatchBreakingReason:
                            case Column.InstanceIds:
                                return null;

                            case Column.BatchCount:
                                return info2.batchCount.ToString();

                            case Column.TotalBatchCount:
                                return info2.totalBatchCount.ToString();

                            case Column.TotalVertexCount:
                                return info2.totalVertexCount.ToString();

                            case Column.GameObjectCount:
                                return info2.instanceIDsCount.ToString();

                            case Column.Rerender:
                                return (info2.renderDataIndex + " : " + info2.renderDataCount);
                        }
                        return "Missing";
                    }
                    return null;
                }
                UISystemProfilerInfo info = item.info;
                switch (((Column) column))
                {
                    case Column.Element:
                    case Column.BatchCount:
                    case Column.TotalBatchCount:
                        goto Label_0234;

                    case Column.VertexCount:
                        return info.vertexCount.ToString();

                    case Column.TotalVertexCount:
                        return info.totalVertexCount.ToString();

                    case Column.BatchBreakingReason:
                        if (info.batchBreakingReason == BatchBreakingReason.NoBreaking)
                        {
                            goto Label_0234;
                        }
                        return FormatBatchBreakingReason(info);

                    case Column.GameObjectCount:
                        return info.instanceIDsCount.ToString();

                    case Column.InstanceIds:
                    {
                        if (item.instanceIDs.Length > 5)
                        {
                            return $"{item.instanceIDs.Length} objects";
                        }
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < item.instanceIDs.Length; i++)
                        {
                            if (i != 0)
                            {
                                builder.Append(", ");
                            }
                            int instanceID = item.instanceIDs[i];
                            UnityEngine.Object obj2 = EditorUtility.InstanceIDToObject(instanceID);
                            if (obj2 == null)
                            {
                                builder.Append(instanceID);
                            }
                            else
                            {
                                builder.Append(obj2.name);
                            }
                        }
                        return builder.ToString();
                    }
                    case Column.Rerender:
                        return info.renderDataIndex.ToString();
                }
                return "Missing";
            }
            switch (((Column) column))
            {
                case Column.TotalBatchCount:
                    return this.m_AllCanvasesItem.totalBatchCount.ToString();

                case Column.TotalVertexCount:
                    return this.m_AllCanvasesItem.totalVertexCount.ToString();

                case Column.GameObjectCount:
                    return this.m_AllCanvasesItem.gameObjectCount.ToString();

                default:
                    return null;
            }
        Label_0234:
            return null;
        }

        internal IList<TreeViewItem> GetRowsFromIDs(IList<int> selection) => 
            base.FindRows(selection);

        private static void HighlightRowsMatchingObjects(IList<TreeViewItem> rows)
        {
            List<int> list = new List<int>();
            foreach (TreeViewItem item in rows)
            {
                BatchTreeViewItem item2 = item as BatchTreeViewItem;
                if (item2 != null)
                {
                    list.AddRange(item2.instanceIDs);
                }
                else
                {
                    CanvasTreeViewItem item3 = item as CanvasTreeViewItem;
                    if (item3 != null)
                    {
                        Canvas canvas = EditorUtility.InstanceIDToObject(item3.info.objectInstanceId) as Canvas;
                        if ((canvas != null) && (canvas.gameObject != null))
                        {
                            list.Add(canvas.gameObject.GetInstanceID());
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                Selection.instanceIDs = list.ToArray();
            }
        }

        protected override void RowGUI(TreeView.RowGUIArgs args)
        {
            if (Event.current.type == EventType.Repaint)
            {
                int visibleColumnIndex = 0;
                int numVisibleColumns = args.GetNumVisibleColumns();
                while (visibleColumnIndex < numVisibleColumns)
                {
                    int column = args.GetColumn(visibleColumnIndex);
                    Rect cellRect = args.GetCellRect(visibleColumnIndex);
                    if (column == 0)
                    {
                        GUIStyle label = TreeView.DefaultStyles.label;
                        cellRect.xMin += label.margin.left + base.GetContentIndent(args.item);
                        int num4 = 0x10;
                        int num5 = 2;
                        Rect position = cellRect;
                        position.width = num4;
                        Texture icon = args.item.icon;
                        if (icon != null)
                        {
                            GUI.DrawTexture(position, icon, ScaleMode.ScaleToFit);
                        }
                        label.padding.left = (icon != null) ? (num4 + num5) : 0;
                        label.Draw(cellRect, args.item.displayName, false, false, args.selected, args.focused);
                    }
                    else
                    {
                        string itemcontent = this.GetItemcontent(args, column);
                        if (itemcontent != null)
                        {
                            TreeView.DefaultGUI.LabelRightAligned(cellRect, itemcontent, args.selected, args.focused);
                        }
                        else
                        {
                            GUI.enabled = false;
                            TreeView.DefaultGUI.LabelRightAligned(cellRect, "-", false, false);
                            GUI.enabled = true;
                        }
                    }
                    visibleColumnIndex++;
                }
            }
        }

        private void SetupRows(TreeViewItem item, IList<TreeViewItem> rows)
        {
            rows.Add(item);
            if (item.hasChildren && !TreeView.IsChildListForACollapsedParent(item.children))
            {
                if ((this.m_Comparer.Col != Column.Element) || this.m_Comparer.isAscending)
                {
                    item.children.Sort(this.m_Comparer);
                }
                foreach (TreeViewItem item2 in item.children)
                {
                    this.SetupRows(item2, rows);
                }
            }
        }

        public State profilerState =>
            ((State) base.state);

        [CompilerGenerated]
        private sealed class <ContextClickedItem>c__AnonStorey0
        {
            internal UISystemProfilerTreeView $this;
            internal int id;

            internal void <>m__0()
            {
                this.$this.DoubleClickedItem(this.id);
            }
        }

        internal class BaseTreeViewItem : TreeViewItem
        {
            public UISystemProfilerInfo info;
            public int renderDataIndex;
            protected static readonly Texture2D s_CanvasIcon = EditorGUIUtility.LoadIcon("RectTool On");

            internal BaseTreeViewItem(UISystemProfilerInfo info, int depth, string displayName) : base(info.objectInstanceId, depth, displayName)
            {
                this.info = info;
            }
        }

        internal sealed class BatchTreeViewItem : UISystemProfilerTreeView.BaseTreeViewItem
        {
            public int[] instanceIDs;

            public BatchTreeViewItem(UISystemProfilerInfo info, int depth, string displayName, int[] allBatchesInstanceIDs) : base(info, depth, displayName)
            {
                this.icon = null;
                this.instanceIDs = new int[info.instanceIDsCount];
                Array.Copy(allBatchesInstanceIDs, info.instanceIDsIndex, this.instanceIDs, 0, info.instanceIDsCount);
                base.renderDataIndex = info.renderDataIndex;
            }
        }

        internal class CanvasBatchComparer : IComparer<TreeViewItem>
        {
            internal UISystemProfilerTreeView.Column Col;
            internal bool isAscending;

            public int Compare(TreeViewItem x, TreeViewItem y)
            {
                int num = !this.isAscending ? -1 : 1;
                UISystemProfilerTreeView.BaseTreeViewItem item = (UISystemProfilerTreeView.BaseTreeViewItem) x;
                UISystemProfilerTreeView.BaseTreeViewItem item2 = (UISystemProfilerTreeView.BaseTreeViewItem) y;
                if (item.info.isBatch != item2.info.isBatch)
                {
                    return (!item.info.isBatch ? -1 : 1);
                }
                switch (this.Col)
                {
                    case UISystemProfilerTreeView.Column.Element:
                        if (!item.info.isBatch)
                        {
                            return (item.displayName.CompareTo(item2.displayName) * num);
                        }
                        return -1;

                    case UISystemProfilerTreeView.Column.BatchCount:
                        if (!item.info.isBatch)
                        {
                            return (item.info.batchCount.CompareTo(item2.info.batchCount) * num);
                        }
                        return -1;

                    case UISystemProfilerTreeView.Column.TotalBatchCount:
                        if (!item.info.isBatch)
                        {
                            return (item.info.totalBatchCount.CompareTo(item2.info.totalBatchCount) * num);
                        }
                        return -1;

                    case UISystemProfilerTreeView.Column.VertexCount:
                        if (!item.info.isBatch)
                        {
                            return string.CompareOrdinal(item.displayName, item2.displayName);
                        }
                        return (item.info.vertexCount.CompareTo(item2.info.vertexCount) * num);

                    case UISystemProfilerTreeView.Column.TotalVertexCount:
                        return (item.info.totalVertexCount.CompareTo(item2.info.totalVertexCount) * num);

                    case UISystemProfilerTreeView.Column.GameObjectCount:
                        return (item.info.instanceIDsCount.CompareTo(item2.info.instanceIDsCount) * num);
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        internal sealed class CanvasTreeViewItem : UISystemProfilerTreeView.BaseTreeViewItem
        {
            public CanvasTreeViewItem(UISystemProfilerInfo info, int depth, string displayName) : base(info, depth, displayName)
            {
                this.icon = UISystemProfilerTreeView.BaseTreeViewItem.s_CanvasIcon;
            }
        }

        internal enum Column
        {
            Element,
            BatchCount,
            TotalBatchCount,
            VertexCount,
            TotalVertexCount,
            BatchBreakingReason,
            GameObjectCount,
            InstanceIds,
            Rerender
        }

        internal class RootTreeViewItem : TreeViewItem
        {
            public int gameObjectCount;
            public int totalBatchCount;
            public int totalVertexCount;

            public RootTreeViewItem() : base(1, 0, null, "All Canvases")
            {
            }
        }

        internal class State : TreeViewState
        {
            public int lastFrame;
            public ProfilerWindow profilerWindow;
        }
    }
}

