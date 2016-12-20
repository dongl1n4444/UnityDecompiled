namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Reflection;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.PlaymodeTestsRunner;

    internal class TestLineTreeViewItem : PlaymodeTestTreeViewItem
    {
        public MethodInfo method;
        public Type type;

        public TestLineTreeViewItem(Type type, MethodInfo method, TestResult result, int depth, TreeViewItem parent) : base(CreateName(type, method), CreateFullName(type, method), depth, parent)
        {
            base.result = result;
            this.type = type;
            this.method = method;
        }

        private static string CreateFullName(Type type, MethodInfo method)
        {
            string fullName = type.FullName;
            if (method != null)
            {
                fullName = fullName + "+" + method.Name;
            }
            return fullName;
        }

        private static string CreateName(Type type, MethodInfo method)
        {
            return ((method == null) ? type.FullName : method.Name);
        }

        protected override void ResultUpdated(TestResult result)
        {
            if (result.resultType == TestResult.ResultType.Success)
            {
                this.icon = Icons.s_SuccessImg;
            }
            else if (result.resultType == TestResult.ResultType.Failed)
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

