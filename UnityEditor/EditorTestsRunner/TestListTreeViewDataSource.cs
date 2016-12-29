namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.IMGUI.Controls;

    internal class TestListTreeViewDataSource : TreeViewDataSource
    {
        private EditorTestsRunnerWindow m_EditorTestsRunnerWindow;
        private bool m_ExpandTreeOnCreation;
        private List<EditorTestResult> m_NewResults;

        public TestListTreeViewDataSource(TreeViewController testListTree, EditorTestsRunnerWindow view) : base(testListTree)
        {
            base.showRootItem = false;
            base.rootIsCollapsable = false;
            this.m_EditorTestsRunnerWindow = view;
        }

        public void ExpandTreeOnCreation()
        {
            this.m_ExpandTreeOnCreation = true;
        }

        public override void FetchData()
        {
            TestListBuilder builder = new TestListBuilder(this.m_EditorTestsRunnerWindow.m_ResultList);
            base.m_RootItem = builder.GetTests(EditorTestsRunnerWindow.s_TestEngine.testSuite, this.m_EditorTestsRunnerWindow.m_FilterSettings);
            this.SetExpanded(base.m_RootItem, true);
            if (this.m_ExpandTreeOnCreation)
            {
                this.m_ExpandTreeOnCreation = false;
                this.SetExpandedWithChildren(base.m_RootItem, true);
            }
            this.m_EditorTestsRunnerWindow.m_FilterSettings.availableCategories = builder.categories;
            Array.Sort<string>(this.m_EditorTestsRunnerWindow.m_FilterSettings.availableCategories);
            this.m_NewResults = builder.newResultList;
            this.m_EditorTestsRunnerWindow.m_ResultList = new List<EditorTestResult>(this.m_NewResults);
            this.m_EditorTestsRunnerWindow.m_FilterSettings.UpdateCounters(this.m_EditorTestsRunnerWindow.m_ResultList.Cast<ITestResult>());
            this.m_EditorTestsRunnerWindow.Repaint();
            base.m_NeedRefreshRows = true;
        }

        public override bool IsExpandable(TreeViewItem item) => 
            ((item is TestGroupTreeViewItem) || base.IsExpandable(item));

        public override bool IsRenamingItemAllowed(TreeViewItem item) => 
            false;

        public List<EditorTestResult> resultState =>
            this.m_NewResults;
    }
}

