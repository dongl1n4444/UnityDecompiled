namespace UnityEditor.TestTools.TestRunner.GUI
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools.TestRunner;

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
            TestTreeViewBuilder builder = new TestTreeViewBuilder(this.m_TestListGUI.GetTestListNUnit(), this.m_TestListGUI.newResultList);
            base.m_RootItem = builder.BuildTreeView(null, false, name);
            this.SetExpanded(base.m_RootItem, true);
            if (base.m_RootItem.hasChildren && (base.m_RootItem.children.Count == 1))
            {
                this.SetExpanded(base.m_RootItem.children[0], true);
            }
            if (this.m_ExpandTreeOnCreation)
            {
                this.SetExpandedWithChildren(base.m_RootItem, true);
            }
            this.m_TestListGUI.newResultList = new List<TestRunnerResult>(builder.results);
            base.m_NeedRefreshRows = true;
        }

        public override bool IsExpandable(TreeViewItem item)
        {
            if (item is TestTreeViewItem)
            {
                return ((TestTreeViewItem) item).IsGroupNode;
            }
            return base.IsExpandable(item);
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item) => 
            false;
    }
}

