namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using UnityEditor.EditorTests;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class TestLineTreeViewItem : EditorTestTreeViewItem
    {
        public TestLineTreeViewItem(TestMethod test, int depth, TreeViewItem parent) : base(test, depth, parent)
        {
        }

        protected override void ResultUpdated(EditorTestResult result)
        {
            EditorTestResult result2 = result;
            Texture iconForResult = ((!result2.executed && !result2.isIgnored) && (result2.resultState != TestResultState.NotRunnable)) ? Icons.s_UnknownImg : GuiHelper.GetIconForResult(new TestResultState?(result2.resultState));
            if (base.m_Test.RunState == RunState.Ignored)
            {
                iconForResult = GuiHelper.GetIconForResult(3);
            }
            this.icon = iconForResult as Texture2D;
        }
    }
}

