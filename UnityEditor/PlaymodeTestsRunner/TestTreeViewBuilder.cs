namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

    internal class TestTreeViewBuilder
    {
        private List<TestResult> m_OldTestResultList;
        private TestListElement m_TestListRoot;
        public List<TestResult> results = new List<TestResult>();

        public TestTreeViewBuilder(TestListElement tests, List<TestResult> oldTestResultResults)
        {
            this.m_OldTestResultList = oldTestResultResults;
            this.m_TestListRoot = tests;
        }

        public TreeViewItem BuildTreeView(TestFilterSettings settings, bool sceneBased, string sceneName)
        {
            TreeViewItem rootItem = new TreeViewItem(0x7fffffff, 0, null, "Invisible Root Item");
            this.ParseTestTree(0, rootItem, this.m_TestListRoot);
            return rootItem;
        }

        private void ParseTestTree(int depth, TreeViewItem rootItem, TestListElement testElement)
        {
            <ParseTestTree>c__AnonStorey0 storey = new <ParseTestTree>c__AnonStorey0 {
                testElement = testElement
            };
            if (storey.testElement is TestListItem)
            {
                TestResult result = Enumerable.FirstOrDefault<TestResult>(this.m_OldTestResultList, new Func<TestResult, bool>(storey.<>m__0));
                if (result == null)
                {
                    result = new TestResult(storey.testElement);
                }
                this.results.Add(result);
                TestLineTreeViewItem child = new TestLineTreeViewItem(storey.testElement.testExecutor.m_Type, storey.testElement.testExecutor.m_MethodInfo, result, depth, rootItem);
                rootItem.AddChild(child);
                child.SetResult(result);
            }
            else
            {
                TestResult item = Enumerable.FirstOrDefault<TestResult>(this.m_OldTestResultList, new Func<TestResult, bool>(storey.<>m__1));
                if (item == null)
                {
                    item = new TestResult(storey.testElement);
                }
                this.results.Add(item);
                string name = storey.testElement.name;
                TestGroupTreeViewItem item2 = new TestGroupTreeViewItem(item, name, name, depth, rootItem);
                rootItem.AddChild(item2);
                depth++;
                foreach (TestListElement element in storey.testElement.GetChildren())
                {
                    this.ParseTestTree(depth, item2, element);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ParseTestTree>c__AnonStorey0
        {
            internal TestListElement testElement;

            internal bool <>m__0(TestResult a) => 
                (a.id == this.testElement.id);

            internal bool <>m__1(TestResult a) => 
                (a.id == this.testElement.id);
        }
    }
}

