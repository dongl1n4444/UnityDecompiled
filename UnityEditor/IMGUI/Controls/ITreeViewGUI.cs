namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal interface ITreeViewGUI
    {
        void BeginPingItem(TreeViewItem item, float topPixelOfRow, float availableWidth);
        bool BeginRename(TreeViewItem item, float delay);
        void BeginRowGUI();
        void EndPingItem();
        void EndRename();
        void EndRowGUI();
        float GetContentIndent(TreeViewItem item);
        void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible);
        int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView);
        Rect GetRectForFraming(int row);
        Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item);
        Rect GetRowRect(int row, float rowWidth);
        Vector2 GetTotalSize();
        void OnInitialize();
        void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused);

        float bottomRowMargin { get; }

        float halfDropBetweenHeight { get; }

        float topRowMargin { get; }
    }
}

