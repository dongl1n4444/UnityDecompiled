namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Internal;
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class TestGroupTreeViewItem : PlaymodeTestTreeViewItem
    {
        public TestGroupTreeViewItem(Test test, int depth, TreeViewItem parent) : base(test, depth, parent)
        {
        }

        protected override void ResultUpdated(TestRunnerResult result)
        {
            if (result.resultType == TestRunnerResult.ResultType.Success)
            {
                this.icon = Icons.s_SuccessImg;
            }
            else if (result.resultType == TestRunnerResult.ResultType.Failed)
            {
                this.icon = Icons.s_FailImg;
            }
            else
            {
                this.icon = Icons.s_UnknownImg;
            }
        }
    }
}

