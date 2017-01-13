namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal abstract class TreeViewGUI : ITreeViewGUI
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <iconLeftPadding>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Action<TreeViewItem, Rect> <iconOverlayGUI>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <iconRightPadding>k__BackingField;
        public float extraInsertionMarkerIndent;
        public float foldoutYOffset;
        public float k_BaseIndent;
        public float k_BottomRowMargin;
        public float k_HalfDropBetweenHeight;
        public float k_IconWidth;
        public float k_IndentWidth;
        public float k_LineHeight;
        public float k_SpaceBetweenIconAndText;
        public float k_TopRowMargin;
        private bool m_AnimateScrollBarOnExpandCollapse;
        protected Rect m_DraggingInsertionMarkerRect;
        protected PingData m_Ping;
        protected TreeViewController m_TreeView;
        protected bool m_UseHorizontalScroll;
        protected static Styles s_Styles;

        public TreeViewGUI(TreeViewController treeView)
        {
            this.m_Ping = new PingData();
            this.m_AnimateScrollBarOnExpandCollapse = true;
            this.k_LineHeight = 16f;
            this.k_BaseIndent = 2f;
            this.k_IndentWidth = 14f;
            this.k_IconWidth = 16f;
            this.k_SpaceBetweenIconAndText = 2f;
            this.k_TopRowMargin = 0f;
            this.k_BottomRowMargin = 0f;
            this.k_HalfDropBetweenHeight = 4f;
            this.foldoutYOffset = 0f;
            this.extraInsertionMarkerIndent = 0f;
            this.m_TreeView = treeView;
        }

        public TreeViewGUI(TreeViewController treeView, bool useHorizontalScroll)
        {
            this.m_Ping = new PingData();
            this.m_AnimateScrollBarOnExpandCollapse = true;
            this.k_LineHeight = 16f;
            this.k_BaseIndent = 2f;
            this.k_IndentWidth = 14f;
            this.k_IconWidth = 16f;
            this.k_SpaceBetweenIconAndText = 2f;
            this.k_TopRowMargin = 0f;
            this.k_BottomRowMargin = 0f;
            this.k_HalfDropBetweenHeight = 4f;
            this.foldoutYOffset = 0f;
            this.extraInsertionMarkerIndent = 0f;
            this.m_TreeView = treeView;
            this.m_UseHorizontalScroll = useHorizontalScroll;
        }

        public virtual void BeginPingItem(TreeViewItem item, float topPixelOfRow, float availableWidth)
        {
            <BeginPingItem>c__AnonStorey1 storey = new <BeginPingItem>c__AnonStorey1 {
                item = item,
                $this = this
            };
            if ((storey.item != null) && (topPixelOfRow >= 0f))
            {
                <BeginPingItem>c__AnonStorey0 storey2 = new <BeginPingItem>c__AnonStorey0 {
                    <>f__ref$1 = storey
                };
                this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
                this.m_Ping.m_PingStyle = s_Styles.ping;
                GUIContent content = GUIContent.Temp(storey.item.displayName);
                Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
                this.m_Ping.m_ContentRect = new Rect(this.GetContentIndent(storey.item), topPixelOfRow, ((this.k_IconWidth + this.k_SpaceBetweenIconAndText) + vector.x) + this.iconTotalPadding, vector.y);
                this.m_Ping.m_AvailableWidth = availableWidth;
                storey2.row = this.m_TreeView.data.GetRow(storey.item.id);
                storey2.useBoldFont = storey.item.displayName.Equals("Assets");
                this.m_Ping.m_ContentDraw = new Action<Rect>(storey2.<>m__0);
                this.m_TreeView.Repaint();
            }
        }

        public virtual bool BeginRename(TreeViewItem item, float delay) => 
            this.GetRenameOverlay().BeginRename(item.displayName, item.id, delay);

        public virtual void BeginRowGUI()
        {
            this.InitStyles();
            this.m_DraggingInsertionMarkerRect.x = -1f;
            this.SyncFakeItem();
            if (Event.current.type != EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
        }

        protected virtual void ClearRenameAndNewItemState()
        {
            this.m_TreeView.data.RemoveFakeItem();
            this.GetRenameOverlay().Clear();
        }

        protected virtual Rect DoFoldout(Rect rect, TreeViewItem item, int row)
        {
            float foldoutIndent = this.GetFoldoutIndent(item);
            Rect foldoutRect = new Rect(rect.x + foldoutIndent, rect.y + this.foldoutYOffset, s_Styles.foldoutWidth, rect.height);
            this.FoldoutButton(foldoutRect, item, row, s_Styles.foldout);
            return foldoutRect;
        }

        protected virtual void DoItemGUI(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
        {
            EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
            float foldoutIndent = this.GetFoldoutIndent(item);
            int itemControlID = TreeViewController.GetItemControlID(item);
            bool flag = false;
            if (this.m_TreeView.dragging != null)
            {
                flag = (this.m_TreeView.dragging.GetDropTargetControlID() == itemControlID) && this.m_TreeView.data.CanBeParent(item);
            }
            bool flag2 = this.IsRenaming(item.id);
            bool flag3 = this.m_TreeView.data.IsExpandable(item);
            if (flag2 && (Event.current.type == EventType.Repaint))
            {
                this.GetRenameOverlay().editFieldRect = this.GetRenameRect(rect, row, item);
            }
            string displayName = item.displayName;
            if (flag2)
            {
                selected = false;
                displayName = "";
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.DrawItemBackground(rect, row, item, selected, focused);
                if (selected)
                {
                    s_Styles.selectionStyle.Draw(rect, false, false, true, focused);
                }
                if (flag)
                {
                    s_Styles.lineStyle.Draw(rect, GUIContent.none, true, true, false, false);
                }
                if ((this.m_TreeView.dragging != null) && (this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID))
                {
                    float y = (!this.m_TreeView.dragging.drawRowMarkerAbove ? rect.yMax : rect.y) - (s_Styles.insertion.fixedHeight * 0.5f);
                    this.m_DraggingInsertionMarkerRect = new Rect((((rect.x + foldoutIndent) + this.extraInsertionMarkerIndent) + s_Styles.foldoutWidth) + s_Styles.lineStyle.margin.left, y, rect.width - foldoutIndent, rect.height);
                }
            }
            this.OnContentGUI(rect, row, item, displayName, selected, focused, useBoldFont, false);
            if (flag3)
            {
                this.DoFoldout(rect, item, row);
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        public virtual void DoRenameOverlay()
        {
            if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().OnGUI())
            {
                this.EndRename();
            }
        }

        protected virtual void DrawItemBackground(Rect rect, int row, TreeViewItem item, bool selected, bool focused)
        {
        }

        public virtual void EndPingItem()
        {
            this.m_Ping.m_TimeStart = -1f;
        }

        public virtual void EndRename()
        {
            if (this.GetRenameOverlay().HasKeyboardFocus())
            {
                this.m_TreeView.GrabKeyboardFocus();
            }
            this.RenameEnded();
            this.ClearRenameAndNewItemState();
        }

        public virtual void EndRowGUI()
        {
            if ((this.m_DraggingInsertionMarkerRect.x >= 0f) && (Event.current.type == EventType.Repaint))
            {
                s_Styles.insertion.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
            this.HandlePing();
        }

        protected virtual void FoldoutButton(Rect foldoutRect, TreeViewItem item, int row, GUIStyle foldoutStyle)
        {
            bool flag;
            TreeViewItemExpansionAnimator expansionAnimator = this.m_TreeView.expansionAnimator;
            EditorGUI.BeginChangeCheck();
            if (expansionAnimator.IsAnimating(item.id))
            {
                float num2;
                Matrix4x4 matrix = GUI.matrix;
                float num = Mathf.Min((float) 1f, (float) (expansionAnimator.expandedValueNormalized * 2f));
                if (!expansionAnimator.isExpanding)
                {
                    num2 = num * 90f;
                }
                else
                {
                    num2 = (1f - num) * -90f;
                }
                GUIUtility.RotateAroundPivot(num2, foldoutRect.center);
                bool isExpanding = expansionAnimator.isExpanding;
                flag = GUI.Toggle(foldoutRect, isExpanding, GUIContent.none, foldoutStyle);
                GUI.matrix = matrix;
            }
            else
            {
                flag = GUI.Toggle(foldoutRect, this.m_TreeView.data.IsExpanded(item), GUIContent.none, foldoutStyle);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.m_TreeView.UserInputChangedExpandedState(item, row, flag);
            }
        }

        public virtual float GetContentIndent(TreeViewItem item) => 
            (this.GetFoldoutIndent(item) + s_Styles.foldoutWidth);

        public virtual void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
        {
            if (this.m_TreeView.data.rowCount == 0)
            {
                firstRowVisible = lastRowVisible = -1;
            }
            else
            {
                float y = this.m_TreeView.state.scrollPos.y;
                float height = this.m_TreeView.GetTotalRect().height;
                firstRowVisible = (int) Mathf.Floor((y - this.topRowMargin) / this.k_LineHeight);
                lastRowVisible = firstRowVisible + ((int) Mathf.Ceil(height / this.k_LineHeight));
                firstRowVisible = Mathf.Max(firstRowVisible, 0);
                lastRowVisible = Mathf.Min(lastRowVisible, this.m_TreeView.data.rowCount - 1);
                if ((firstRowVisible >= this.m_TreeView.data.rowCount) && (firstRowVisible > 0))
                {
                    this.m_TreeView.state.scrollPos.y = 0f;
                    this.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
                }
            }
        }

        public virtual float GetFoldoutIndent(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return this.k_BaseIndent;
            }
            return (this.k_BaseIndent + (item.depth * this.indentWidth));
        }

        protected virtual Texture GetIconForItem(TreeViewItem item) => 
            item.icon;

        protected float GetMaxWidth(IList<TreeViewItem> rows)
        {
            this.InitStyles();
            float num = 1f;
            foreach (TreeViewItem item in rows)
            {
                float num3;
                float num4;
                float num2 = 0f;
                num2 += this.GetContentIndent(item);
                if (item.icon != null)
                {
                    num2 += this.k_IconWidth;
                }
                s_Styles.lineStyle.CalcMinMaxWidth(GUIContent.Temp(item.displayName), out num3, out num4);
                num2 += num4;
                num2 += this.k_BaseIndent;
                if (num2 > num)
                {
                    num = num2;
                }
            }
            return num;
        }

        public virtual int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView) => 
            ((int) Mathf.Floor(heightOfTreeView / this.k_LineHeight));

        public virtual Rect GetRectForFraming(int row) => 
            this.GetRowRect(row, 1f);

        protected RenameOverlay GetRenameOverlay() => 
            this.m_TreeView.state.renameOverlay;

        public virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            float num = this.GetContentIndent(item) + s_Styles.lineStyle.margin.left;
            if (this.GetIconForItem(item) != null)
            {
                num += (this.k_SpaceBetweenIconAndText + this.k_IconWidth) + this.iconTotalPadding;
            }
            return new Rect(rowRect.x + num, rowRect.y, rowRect.width - num, 16f);
        }

        public virtual Rect GetRowRect(int row, float rowWidth) => 
            new Rect(0f, this.GetTopPixelOfRow(row), rowWidth, this.k_LineHeight);

        private float GetTopPixelOfRow(int row) => 
            ((row * this.k_LineHeight) + this.topRowMargin);

        public virtual Vector2 GetTotalSize()
        {
            this.InitStyles();
            float x = 1f;
            if (this.m_UseHorizontalScroll)
            {
                IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
                x = this.GetMaxWidth(rows);
            }
            float y = ((this.m_TreeView.data.rowCount * this.k_LineHeight) + this.topRowMargin) + this.bottomRowMargin;
            if (this.m_AnimateScrollBarOnExpandCollapse && this.m_TreeView.expansionAnimator.isAnimating)
            {
                y -= this.m_TreeView.expansionAnimator.deltaHeight;
            }
            return new Vector2(x, y);
        }

        private void HandlePing()
        {
            this.m_Ping.HandlePing();
            if (this.m_Ping.isPinging)
            {
                this.m_TreeView.Repaint();
            }
        }

        protected virtual void InitStyles()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        protected virtual bool IsRenaming(int id) => 
            ((this.GetRenameOverlay().IsRenaming() && (this.GetRenameOverlay().userData == id)) && !this.GetRenameOverlay().isWaitingForDelay);

        protected virtual void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (Event.current.rawType == EventType.Repaint)
            {
                if (!isPinging)
                {
                    float contentIndent = this.GetContentIndent(item);
                    rect.x += contentIndent;
                    rect.width -= contentIndent;
                }
                GUIStyle style = !useBoldFont ? s_Styles.lineStyle : s_Styles.lineBoldStyle;
                rect.xMin += style.margin.left;
                Rect position = rect;
                position.width = this.k_IconWidth;
                position.x += this.iconLeftPadding;
                Texture iconForItem = this.GetIconForItem(item);
                if (iconForItem != null)
                {
                    GUI.DrawTexture(position, iconForItem, ScaleMode.ScaleToFit);
                }
                style.padding.left = (iconForItem != null) ? ((int) ((this.k_IconWidth + this.iconTotalPadding) + this.k_SpaceBetweenIconAndText)) : 0;
                style.Draw(rect, label, false, false, selected, focused);
                if (this.iconOverlayGUI != null)
                {
                    Rect rect3 = rect;
                    rect3.width = this.k_IconWidth + this.iconTotalPadding;
                    this.iconOverlayGUI(item, rect3);
                }
            }
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
        {
            this.DoItemGUI(rowRect, row, item, selected, focused, false);
        }

        protected virtual void RenameEnded()
        {
        }

        protected virtual void SyncFakeItem()
        {
        }

        public virtual float bottomRowMargin =>
            this.k_BottomRowMargin;

        public float halfDropBetweenHeight =>
            this.k_HalfDropBetweenHeight;

        public float iconLeftPadding { get; set; }

        public Action<TreeViewItem, Rect> iconOverlayGUI { get; set; }

        public float iconRightPadding { get; set; }

        public float iconTotalPadding =>
            (this.iconLeftPadding + this.iconRightPadding);

        public float indentWidth =>
            (this.k_IndentWidth + this.iconTotalPadding);

        public virtual float topRowMargin =>
            this.k_TopRowMargin;

        [CompilerGenerated]
        private sealed class <BeginPingItem>c__AnonStorey0
        {
            internal TreeViewGUI.<BeginPingItem>c__AnonStorey1 <>f__ref$1;
            internal int row;
            internal bool useBoldFont;

            internal void <>m__0(Rect r)
            {
                this.<>f__ref$1.$this.OnContentGUI(r, this.row, this.<>f__ref$1.item, this.<>f__ref$1.item.displayName, false, false, this.useBoldFont, true);
            }
        }

        [CompilerGenerated]
        private sealed class <BeginPingItem>c__AnonStorey1
        {
            internal TreeViewGUI $this;
            internal TreeViewItem item;
        }

        internal class Styles
        {
            public GUIContent content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));
            public GUIStyle foldout = "IN Foldout";
            public GUIStyle insertion = "PR Insertion";
            public GUIStyle lineBoldStyle;
            public GUIStyle lineStyle = new GUIStyle("PR Label");
            public GUIStyle ping = new GUIStyle("PR Ping");
            public GUIStyle selectionStyle = new GUIStyle("PR Label");
            public GUIStyle toolbarButton = "ToolbarButton";

            public Styles()
            {
                Texture2D background = this.lineStyle.hover.background;
                this.lineStyle.onNormal.background = background;
                this.lineStyle.onActive.background = background;
                this.lineStyle.onFocused.background = background;
                this.lineStyle.alignment = TextAnchor.MiddleLeft;
                this.lineStyle.fixedHeight = 0f;
                this.lineBoldStyle = new GUIStyle(this.lineStyle);
                this.lineBoldStyle.font = EditorStyles.boldLabel.font;
                this.lineBoldStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
                this.ping.padding.left = 0x10;
                this.ping.padding.right = 0x10;
                this.ping.fixedHeight = 16f;
                this.selectionStyle.fixedHeight = 0f;
                this.selectionStyle.border = new RectOffset();
                this.insertion.overflow = new RectOffset(4, 0, 0, 0);
            }

            public float foldoutWidth =>
                TreeViewGUI.s_Styles.foldout.fixedWidth;
        }
    }
}

