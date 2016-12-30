namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AudioProfilerClipView
    {
        private int delayedPingObject;
        private AudioProfilerClipViewBackend m_Backend;
        private AudioProfilerClipViewColumnHeader m_ColumnHeader;
        private EditorWindow m_EditorWindow;
        private GUIStyle m_HeaderStyle;
        private TreeViewController m_TreeView;
        private AudioProfilerClipTreeViewState m_TreeViewState;

        public AudioProfilerClipView(EditorWindow editorWindow, AudioProfilerClipTreeViewState state)
        {
            this.m_EditorWindow = editorWindow;
            this.m_TreeViewState = state;
        }

        public int GetNumItemsInData() => 
            this.m_Backend.items.Count;

        public void Init(Rect rect, AudioProfilerClipViewBackend backend)
        {
            if (this.m_HeaderStyle == null)
            {
                this.m_HeaderStyle = new GUIStyle("OL title");
            }
            this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
            if (this.m_TreeView == null)
            {
                this.m_Backend = backend;
                if ((this.m_TreeViewState.columnWidths == null) || (this.m_TreeViewState.columnWidths.Length == 0))
                {
                    int num = AudioProfilerClipInfoHelper.GetLastColumnIndex() + 1;
                    this.m_TreeViewState.columnWidths = new float[num];
                    for (int i = 0; i < num; i++)
                    {
                        this.m_TreeViewState.columnWidths[i] = (i != 0) ? ((i != 2) ? ((float) 80) : ((float) 110)) : ((float) 300);
                    }
                }
                this.m_TreeView = new TreeViewController(this.m_EditorWindow, this.m_TreeViewState);
                ITreeViewGUI gui = new AudioProfilerClipViewGUI(this.m_TreeView);
                ITreeViewDataSource data = new AudioProfilerDataSource(this.m_TreeView, this.m_Backend);
                this.m_TreeView.Init(rect, data, gui, null);
                this.m_ColumnHeader = new AudioProfilerClipViewColumnHeader(this.m_TreeViewState, this.m_Backend);
                this.m_ColumnHeader.columnWidths = this.m_TreeViewState.columnWidths;
                this.m_ColumnHeader.minColumnWidth = 30f;
                this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
            }
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            Rect position = new Rect(rect.x, rect.y, rect.width, this.m_HeaderStyle.fixedHeight);
            GUI.Label(position, "", this.m_HeaderStyle);
            this.m_ColumnHeader.OnGUI(position, true, this.m_HeaderStyle);
            rect.y += position.height;
            rect.height -= position.height;
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(rect, controlID);
        }

        public void OnTreeSelectionChanged(int[] selection)
        {
            if (selection.Length == 1)
            {
                AudioProfilerClipTreeViewItem item2 = this.m_TreeView.FindItem(selection[0]) as AudioProfilerClipTreeViewItem;
                if (item2 != null)
                {
                    EditorGUIUtility.PingObject(item2.info.info.assetInstanceId);
                }
            }
        }

        private void PingObjectDelayed()
        {
            EditorGUIUtility.PingObject(this.delayedPingObject);
        }

        internal class AudioProfilerClipTreeViewItem : TreeViewItem
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private AudioProfilerClipInfoWrapper <info>k__BackingField;

            public AudioProfilerClipTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, AudioProfilerClipInfoWrapper info) : base(id, depth, parent, displayName)
            {
                this.info = info;
            }

            public AudioProfilerClipInfoWrapper info { get; set; }
        }

        internal class AudioProfilerClipViewColumnHeader
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private float[] <columnWidths>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private float <dragWidth>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private float <minColumnWidth>k__BackingField;
            private string[] headers = new string[] { "Asset", "Load State", "Internal Load State", "Age", "Disposed", "Num Voices" };
            private AudioProfilerClipViewBackend m_Backend;
            private AudioProfilerClipTreeViewState m_TreeViewState;

            public AudioProfilerClipViewColumnHeader(AudioProfilerClipTreeViewState state, AudioProfilerClipViewBackend backend)
            {
                this.m_TreeViewState = state;
                this.m_Backend = backend;
                this.minColumnWidth = 10f;
                this.dragWidth = 6f;
            }

            public void OnGUI(Rect rect, bool allowSorting, GUIStyle headerStyle)
            {
                GUI.BeginClip(rect);
                float x = -this.m_TreeViewState.scrollPos.x;
                int lastColumnIndex = AudioProfilerClipInfoHelper.GetLastColumnIndex();
                for (int i = 0; i <= lastColumnIndex; i++)
                {
                    Rect position = new Rect(x, 0f, this.columnWidths[i], rect.height - 1f);
                    x += this.columnWidths[i];
                    Rect rect3 = new Rect(x - (this.dragWidth / 2f), 0f, 3f, rect.height);
                    float num4 = EditorGUI.MouseDeltaReader(rect3, true).x;
                    if (num4 != 0f)
                    {
                        this.columnWidths[i] += num4;
                        this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
                    }
                    string text = this.headers[i];
                    if (allowSorting && (i == this.m_TreeViewState.selectedColumn))
                    {
                        text = text + (!this.m_TreeViewState.sortByDescendingOrder ? " ▲" : " ▼");
                    }
                    GUI.Box(position, text, headerStyle);
                    if ((allowSorting && (Event.current.type == EventType.MouseDown)) && position.Contains(Event.current.mousePosition))
                    {
                        this.m_TreeViewState.SetSelectedColumn(i);
                        this.m_Backend.UpdateSorting();
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorGUIUtility.AddCursorRect(rect3, MouseCursor.SplitResizeLeftRight);
                    }
                }
                GUI.EndClip();
            }

            public float[] columnWidths { get; set; }

            public float dragWidth { get; set; }

            public float minColumnWidth { get; set; }
        }

        internal class AudioProfilerClipViewGUI : TreeViewGUI
        {
            public AudioProfilerClipViewGUI(TreeViewController treeView) : base(treeView)
            {
                base.k_IconWidth = 0f;
            }

            protected override Texture GetIconForItem(TreeViewItem item) => 
                null;

            public override Vector2 GetTotalSize()
            {
                Vector2 totalSize = base.GetTotalSize();
                totalSize.x = 0f;
                foreach (float num in this.columnWidths)
                {
                    totalSize.x += num;
                }
                return totalSize;
            }

            protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle style = !useBoldFont ? TreeViewGUI.Styles.lineStyle : TreeViewGUI.Styles.lineBoldStyle;
                    style.alignment = TextAnchor.MiddleLeft;
                    style.padding.left = 0;
                    int num = 2;
                    base.OnContentGUI(new Rect(rect.x, rect.y, this.columnWidths[0] - num, rect.height), row, item, label, selected, focused, useBoldFont, isPinging);
                    rect.x += this.columnWidths[0] + num;
                    AudioProfilerClipView.AudioProfilerClipTreeViewItem item2 = item as AudioProfilerClipView.AudioProfilerClipTreeViewItem;
                    for (int i = 1; i < this.columnWidths.Length; i++)
                    {
                        rect.width = this.columnWidths[i] - (2 * num);
                        style.alignment = TextAnchor.MiddleRight;
                        style.Draw(rect, AudioProfilerClipInfoHelper.GetColumnString(item2.info, (AudioProfilerClipInfoHelper.ColumnIndices) i), false, false, selected, focused);
                        Handles.color = Color.black;
                        Handles.DrawLine(new Vector3((rect.x - num) + 1f, rect.y, 0f), new Vector3((rect.x - num) + 1f, rect.y + rect.height, 0f));
                        rect.x += this.columnWidths[i];
                    }
                    style.alignment = TextAnchor.MiddleLeft;
                }
            }

            protected override void RenameEnded()
            {
            }

            protected override void SyncFakeItem()
            {
            }

            private float[] columnWidths =>
                ((AudioProfilerClipTreeViewState) base.m_TreeView.state).columnWidths;
        }

        internal class AudioProfilerDataSource : TreeViewDataSource
        {
            private AudioProfilerClipViewBackend m_Backend;

            public AudioProfilerDataSource(TreeViewController treeView, AudioProfilerClipViewBackend backend) : base(treeView)
            {
                this.m_Backend = backend;
                this.m_Backend.OnUpdate = new AudioProfilerClipViewBackend.DataUpdateDelegate(this.FetchData);
                base.showRootItem = false;
                base.rootIsCollapsable = false;
                this.FetchData();
            }

            public override bool CanBeParent(TreeViewItem item) => 
                item.hasChildren;

            public override void FetchData()
            {
                AudioProfilerClipView.AudioProfilerClipTreeViewItem parentNode = new AudioProfilerClipView.AudioProfilerClipTreeViewItem(1, 0, null, "ROOT", new AudioProfilerClipInfoWrapper(new AudioProfilerClipInfo(), "ROOT"));
                this.FillTreeItems(parentNode, 1, 0, this.m_Backend.items);
                base.m_RootItem = parentNode;
                this.SetExpandedWithChildren(base.m_RootItem, true);
                base.m_NeedRefreshRows = true;
            }

            private void FillTreeItems(AudioProfilerClipView.AudioProfilerClipTreeViewItem parentNode, int depth, int parentId, List<AudioProfilerClipInfoWrapper> items)
            {
                parentNode.children = new List<TreeViewItem>(items.Count);
                int num = 1;
                foreach (AudioProfilerClipInfoWrapper wrapper in items)
                {
                    AudioProfilerClipView.AudioProfilerClipTreeViewItem item = new AudioProfilerClipView.AudioProfilerClipTreeViewItem(++num, 1, parentNode, wrapper.assetName, wrapper);
                    parentNode.children.Add(item);
                }
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item) => 
                false;
        }
    }
}

