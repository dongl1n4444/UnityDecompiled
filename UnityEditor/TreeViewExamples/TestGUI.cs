namespace UnityEditor.TreeViewExamples
{
    using System;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    internal class TestGUI : TreeViewGUI
    {
        private Texture2D m_FolderIcon;
        private Texture2D m_Icon;
        private GUIStyle m_LabelStyle;
        private GUIStyle m_LabelStyleRightAlign;

        public TestGUI(TreeViewController treeView) : base(treeView)
        {
            this.m_FolderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            this.m_Icon = EditorGUIUtility.FindTexture("boo Script Icon");
        }

        protected override Texture GetIconForItem(TreeViewItem item) => 
            (!item.hasChildren ? this.m_Icon : this.m_FolderIcon);

        protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (Event.current.rawType == EventType.Repaint)
            {
                if (this.m_LabelStyle == null)
                {
                    this.m_LabelStyle = new GUIStyle(TreeViewGUI.Styles.lineStyle);
                    int num = 6;
                    this.m_LabelStyle.padding.right = num;
                    this.m_LabelStyle.padding.left = num;
                    this.m_LabelStyleRightAlign = new GUIStyle(TreeViewGUI.Styles.lineStyle);
                    num = 6;
                    this.m_LabelStyleRightAlign.padding.left = num;
                    this.m_LabelStyleRightAlign.padding.right = num;
                    this.m_LabelStyleRightAlign.alignment = TextAnchor.MiddleRight;
                }
                if (selected)
                {
                    TreeViewGUI.Styles.selectionStyle.Draw(rect, false, false, true, focused);
                }
                if ((isPinging || (this.columnWidths == null)) || (this.columnWidths.Length == 0))
                {
                    base.OnContentGUI(rect, row, item, label, selected, focused, useBoldFont, isPinging);
                }
                else
                {
                    Rect rect2 = rect;
                    for (int i = 0; i < this.columnWidths.Length; i++)
                    {
                        rect2.width = this.columnWidths[i];
                        if (i == 0)
                        {
                            base.OnContentGUI(rect2, row, item, label, selected, focused, useBoldFont, isPinging);
                        }
                        else
                        {
                            GUI.Label(rect2, "Zksdf SDFS DFASDF ", ((i % 2) != 0) ? this.m_LabelStyleRightAlign : this.m_LabelStyle);
                        }
                        rect2.x += rect2.width;
                    }
                }
            }
        }

        protected override void RenameEnded()
        {
        }

        protected override void SyncFakeItem()
        {
        }

        private float[] columnWidths =>
            ((TreeViewStateWithColumns) base.m_TreeView.state).columnWidths;
    }
}

