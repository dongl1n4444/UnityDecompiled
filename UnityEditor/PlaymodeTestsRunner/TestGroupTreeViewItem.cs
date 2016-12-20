namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.PlaymodeTestsRunner;

    internal class TestGroupTreeViewItem : PlaymodeTestTreeViewItem
    {
        public TestGroupTreeViewItem(TestResult result, string name, string fullName, int depth, TreeViewItem parent) : base(name, fullName, depth, parent)
        {
        }

        public override bool IsVisible(FilteringOptions options)
        {
            return base.IsVisible(options);
        }

        protected override void ResultUpdated(TestResult result)
        {
            base.SetResult(result);
        }
    }
}

