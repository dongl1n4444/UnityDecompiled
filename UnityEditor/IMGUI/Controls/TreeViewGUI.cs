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
        private float <extraSpaceBeforeIconAndLabel>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <iconLeftPadding>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<TreeViewItem, Rect> <iconOverlayGUI>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <iconRightPadding>k__BackingField;
        public float customFoldoutYOffset;
        public float extraInsertionMarkerIndent;
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
            this.customFoldoutYOffset = 0f;
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
            this.customFoldoutYOffset = 0f;
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
                this.m_Ping.m_PingStyle = Styles.ping;
                GUIContent content = GUIContent.Temp(storey.item.displayName);
                Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
                this.m_Ping.m_ContentRect = new Rect(this.GetContentIndent(storey.item) + this.extraSpaceBeforeIconAndLabel, topPixelOfRow, ((this.k_IconWidth + this.k_SpaceBetweenIconAndText) + vector.x) + this.iconTotalPadding, vector.y);
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
            Rect foldoutRect = new Rect(rect.x + foldoutIndent, this.GetFoldoutYPosition(rect.y), Styles.foldoutWidth, EditorGUIUtility.singleLineHeight);
            this.FoldoutButton(foldoutRect, item, row, Styles.foldout);
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
                    Styles.selectionStyle.Draw(rect, false, false, true, focused);
                }
                if (flag)
                {
                    Styles.lineStyle.Draw(rect, GUIContent.none, true, true, false, false);
                }
                if ((this.m_TreeView.dragging != null) && (this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID))
                {
                    float y = (!this.m_TreeView.dragging.drawRowMarkerAbove ? rect.yMax : rect.y) - (Styles.insertion.fixedHeight * 0.5f);
                    this.m_DraggingInsertionMarkerRect = new Rect((((rect.x + foldoutIndent) + this.extraInsertionMarkerIndent) + Styles.foldoutWidth) + Styles.lineStyle.margin.left, y, rect.width - foldoutIndent, rect.height);
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
                Styles.insertion.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
            this.HandlePing();
        }

        protected virtual void FoldoutButton(Rect foldoutRect, TreeViewItem item, int row, GUIStyle foldoutStyle)
        {
            TreeViewItemExpansionAnimator expansionAnimator = this.m_TreeView.expansionAnimator;
            EditorGUI.BeginChangeCheck();
            bool flag2 = !expansionAnimator.IsAnimating(item.id) ? this.m_TreeView.data.IsExpanded(item) : expansionAnimator.isExpanding;
            bool expand = GUI.Toggle(foldoutRect, flag2, GUIContent.none, foldoutStyle);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_TreeView.UserInputChangedExpandedState(item, row, expand);
            }
        }

        public virtual float GetContentIndent(TreeViewItem item) => 
            ((this.GetFoldoutIndent(item) + Styles.foldoutWidth) + Styles.lineStyle.margin.left);

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

        private float GetFoldoutYPosition(float rectY) => 
            (rectY + this.customFoldoutYOffset);

        protected virtual Texture GetIconForItem(TreeViewItem item) => 
            item.icon;

        protected float GetMaxWidth(IList<TreeViewItem> rows)
        {
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
                Styles.lineStyle.CalcMinMaxWidth(GUIContent.Temp(item.displayName), out num3, out num4);
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
            float num = this.GetContentIndent(item) + this.extraSpaceBeforeIconAndLabel;
            if (this.GetIconForItem(item) != null)
            {
                num += (this.k_SpaceBetweenIconAndText + this.k_IconWidth) + this.iconTotalPadding;
            }
            return new Rect(rowRect.x + num, rowRect.y, rowRect.width - num, EditorGUIUtility.singleLineHeight);
        }

        public virtual Rect GetRowRect(int row, float rowWidth) => 
            new Rect(0f, this.GetTopPixelOfRow(row), rowWidth, this.k_LineHeight);

        private float GetTopPixelOfRow(int row) => 
            ((row * this.k_LineHeight) + this.topRowMargin);

        public virtual Vector2 GetTotalSize()
        {
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

        protected virtual bool IsRenaming(int id) => 
            ((this.GetRenameOverlay().IsRenaming() && (this.GetRenameOverlay().userData == id)) && !this.GetRenameOverlay().isWaitingForDelay);

        protected virtual void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (Event.current.rawType == EventType.Repaint)
            {
                if (!isPinging)
                {
                    float num = this.GetContentIndent(item) + this.extraSpaceBeforeIconAndLabel;
                    rect.xMin += num;
                }
                GUIStyle style = !useBoldFont ? Styles.lineStyle : Styles.lineBoldStyle;
                Rect position = rect;
                position.width = this.k_IconWidth;
                position.x += this.iconLeftPadding;
                Texture iconForItem = this.GetIconForItem(item);
                if (iconForItem != null)
                {
                    GUI.DrawTexture(position, iconForItem, ScaleMode.ScaleToFit);
                }
                if (this.iconOverlayGUI != null)
                {
                    Rect rect3 = rect;
                    rect3.width = this.k_IconWidth + this.iconTotalPadding;
                    this.iconOverlayGUI(item, rect3);
                }
                style.padding.left = 0;
                if (iconForItem != null)
                {
                    rect.xMin += (this.k_IconWidth + this.iconTotalPadding) + this.k_SpaceBetweenIconAndText;
                }
                style.Draw(rect, label, false, false, selected, focused);
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

        public float extraSpaceBeforeIconAndLabel { get; set; }

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

        internal static class Styles
        {
            public static GUIContent content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));
            public static GUIStyle foldout = "IN Foldout";
            public static GUIStyle insertion = new GUIStyle("PR Insertion");
            public static GUIStyle lineBoldStyle;
            public static GUIStyle lineStyle = new GUIStyle("PR Label");
            public static GUIStyle ping = new GUIStyle("PR Ping");
            public static GUIStyle selectionStyle = new GUIStyle("PR Label");
            public static GUIStyle toolbarButton = "ToolbarButton";

            static Styles()
            {
                Texture2D background = lineStyle.hover.background;
                lineStyle.onNormal.background = background;
                lineStyle.onActive.background = background;
                lineStyle.onFocused.background = background;
                lineStyle.alignment = TextAnchor.UpperLeft;
                lineStyle.padding.top = 2;
                lineStyle.fixedHeight = 0f;
                lineBoldStyle = new GUIStyle(lineStyle);
                lineBoldStyle.font = EditorStyles.boldLabel.font;
                lineBoldStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
                ping.padding.left = 0x10;
                ping.padding.right = 0x10;
                ping.fixedHeight = 16f;
                selectionStyle.fixedHeight = 0f;
                selectionStyle.border = new RectOffset();
                insertion.overflow = new RectOffset(4, 0, 0, 0);
            }

            public static float foldoutWidth =>
                foldout.fixedWidth;
        }
    }
}

