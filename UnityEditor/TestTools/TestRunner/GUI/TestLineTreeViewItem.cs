namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Internal;
    using System;
    using System.Reflection;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class TestLineTreeViewItem : PlaymodeTestTreeViewItem
    {
        public MethodInfo method;
        public Type type;

        public TestLineTreeViewItem(Test test, int depth, TreeViewItem parent) : base(test, depth, parent)
        {
            this.type = test.TypeInfo.Type;
            this.method = test.Method.MethodInfo;
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

