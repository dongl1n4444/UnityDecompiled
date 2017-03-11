namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using UnityEditor.IMGUI.Controls;

    internal abstract class EditorTestTreeViewItem : TreeViewItem
    {
        protected EditorTestResult m_EditorTestResult;
        protected Test m_Test;

        protected EditorTestTreeViewItem(Test test, int depth, TreeViewItem parent) : base(CreateId(test, parent), depth, parent, test.TestName.Name)
        {
            if (this.displayName.Length > 100)
            {
                this.displayName = this.displayName.Substring(0, 100);
            }
            this.displayName = this.displayName.Replace("\n", "");
            this.m_Test = test;
        }

        private static int CreateId(Test test, TreeViewItem parent)
        {
            if ((parent is TestGroupTreeViewItem) && (((parent as TestGroupTreeViewItem).m_Test is ParameterizedMethodSuite) || (test is ParameterizedFixtureSuite)))
            {
                return test.TestName.UniqueName.GetHashCode();
            }
            return test.TestName.FullName.GetHashCode();
        }

        public virtual bool IsVisible(FilteringOptions options) => 
            true;

        protected virtual void ResultUpdated(EditorTestResult result)
        {
        }

        public void SetResult(EditorTestResult result)
        {
            this.m_EditorTestResult = result;
            this.m_EditorTestResult.SetResultChangedCallback(new Action<EditorTestResult>(this.ResultUpdated));
            this.ResultUpdated(this.m_EditorTestResult);
        }

        public EditorTestResult result =>
            this.m_EditorTestResult;

        public TestName testName =>
            this.m_Test.TestName;
    }
}

