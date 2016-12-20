namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using UnityEditor.IMGUI.Controls;

    internal abstract class EditorTestTreeViewItem : TreeViewItem
    {
        protected EditorTestResult m_EditorTestResult;
        protected Test m_Test;

        protected EditorTestTreeViewItem(Test test, int depth, TreeViewItem parent) : base(CreateId(test, parent), depth, parent, test.get_TestName().get_Name())
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
                return test.get_TestName().get_UniqueName().GetHashCode();
            }
            return test.get_TestName().get_FullName().GetHashCode();
        }

        public virtual bool IsVisible(FilteringOptions options)
        {
            return true;
        }

        protected virtual void ResultUpdated(EditorTestResult result)
        {
        }

        public void SetResult(EditorTestResult result)
        {
            this.m_EditorTestResult = result;
            this.m_EditorTestResult.SetResultChangedCallback(new Action<EditorTestResult>(this.ResultUpdated));
            this.ResultUpdated(this.m_EditorTestResult);
        }

        public EditorTestResult result
        {
            get
            {
                return this.m_EditorTestResult;
            }
        }

        public TestName testName
        {
            get
            {
                return this.m_Test.get_TestName();
            }
        }
    }
}

