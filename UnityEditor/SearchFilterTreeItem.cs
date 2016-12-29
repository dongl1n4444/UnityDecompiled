namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;

    internal class SearchFilterTreeItem : TreeViewItem
    {
        private bool m_IsFolder;

        public SearchFilterTreeItem(int id, int depth, TreeViewItem parent, string displayName, bool isFolder) : base(id, depth, parent, displayName)
        {
            this.m_IsFolder = isFolder;
        }

        public bool isFolder =>
            this.m_IsFolder;
    }
}

