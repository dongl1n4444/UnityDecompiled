namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.PlaymodeTestsRunner.GUI;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.SceneManagement;

    internal class TestListTreeViewDataSource : TreeViewDataSource
    {
        private bool m_ExpandTreeOnCreation;
        private TestListGUI m_TestListGUI;

        public TestListTreeViewDataSource(TreeViewController testListTree, TestListGUI testListGUI) : base(testListTree)
        {
            base.showRootItem = false;
            base.rootIsCollapsable = false;
            this.m_TestListGUI = testListGUI;
        }

        public void ExpandTreeOnCreation()
        {
            this.m_ExpandTreeOnCreation = true;
        }

        public override void FetchData()
        {
            string name = SceneManager.GetActiveScene().name;
            if (name.StartsWith("InitTestScene"))
            {
                name = PlaymodeTestsController.GetController().settings.originalScene;
            }
            TestTreeViewBuilder builder = new TestTreeViewBuilder(this.m_TestListGUI.GetTestList(), this.m_TestListGUI.newResultList);
            base.m_RootItem = builder.BuildTreeView(null, false, name);
            this.SetExpanded(base.m_RootItem, true);
            if (this.m_ExpandTreeOnCreation)
            {
                this.SetExpandedWithChildren(base.m_RootItem, true);
            }
            this.m_TestListGUI.newResultList = new List<TestResult>(builder.results);
            base.m_NeedRefreshRows = true;
        }

        public override bool IsExpandable(TreeViewItem item)
        {
            return ((item is TestGroupTreeViewItem) || base.IsExpandable(item));
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            return false;
        }
    }
}

