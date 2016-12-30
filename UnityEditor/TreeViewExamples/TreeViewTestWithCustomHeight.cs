namespace UnityEditor.TreeViewExamples
{
    using System;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class TreeViewTestWithCustomHeight
    {
        private BackendData m_BackendData;
        private TreeViewController m_TreeView;

        public TreeViewTestWithCustomHeight(EditorWindow editorWindow, BackendData backendData, Rect rect)
        {
            this.m_BackendData = backendData;
            TreeViewState treeViewState = new TreeViewState();
            this.m_TreeView = new TreeViewController(editorWindow, treeViewState);
            TestGUICustomItemHeights gui = new TestGUICustomItemHeights(this.m_TreeView);
            TestDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
            TestDataSource data = new TestDataSource(this.m_TreeView, this.m_BackendData);
            data.onVisibleRowsChanged = (Action) Delegate.Combine(data.onVisibleRowsChanged, new Action(gui.CalculateRowRects));
            this.m_TreeView.Init(rect, data, gui, dragging);
            data.SetExpanded(data.root, true);
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            this.m_TreeView.OnGUI(rect, controlID);
        }
    }
}

