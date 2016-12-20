namespace UnityEditor.EditorTestsRunner
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class TestListTreeViewGUI : TreeViewGUI
    {
        public TestListTreeViewGUI(TreeViewController testListTree) : base(testListTree)
        {
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
        {
            bool flag = false;
            Color color = GUI.color;
            if (item is TestLineTreeViewItem)
            {
                TestLineTreeViewItem item2 = item as TestLineTreeViewItem;
                if (item2.result.executed && item2.result.outdated)
                {
                    GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    flag = true;
                }
            }
            base.OnRowGUI(rowRect, item, row, selected, focused);
            if (flag)
            {
                GUI.color = color;
            }
        }
    }
}

