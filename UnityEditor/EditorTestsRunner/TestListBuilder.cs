namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class TestListBuilder
    {
        private HashSet<string> m_CategoryList;
        private List<EditorTestResult> m_NewResultList;
        private List<EditorTestResult> m_OldResultList;

        public TestListBuilder() : this(null)
        {
        }

        public TestListBuilder(List<EditorTestResult> oldResultsList)
        {
            this.m_NewResultList = new List<EditorTestResult>();
            this.m_CategoryList = new HashSet<string>();
            this.m_OldResultList = oldResultsList;
        }

        private EditorTestResult FindExistingResult(string fullTestName, string id)
        {
            <FindExistingResult>c__AnonStorey0 storey = new <FindExistingResult>c__AnonStorey0 {
                fullTestName = fullTestName,
                id = id
            };
            if (this.m_OldResultList == null)
            {
                return null;
            }
            IEnumerable<EditorTestResult> source = Enumerable.Where<EditorTestResult>(this.m_OldResultList, new Func<EditorTestResult, bool>(storey.<>m__0));
            if (source.Count<EditorTestResult>() == 0)
            {
                return null;
            }
            if (source.Count<EditorTestResult>() > 1)
            {
                source = Enumerable.Where<EditorTestResult>(this.m_OldResultList, new Func<EditorTestResult, bool>(storey.<>m__1));
                if (source.Count<EditorTestResult>() == 0)
                {
                    return null;
                }
            }
            return source.First<EditorTestResult>();
        }

        public TreeViewItem GetTests(TestSuite testSuite, TestFilterSettings renderingFilter)
        {
            if (testSuite == null)
            {
                Debug.Log("No tests were found. Was there a compilation error?");
                return null;
            }
            TreeViewItem parent = new TreeViewItem(0x7fffffff, 0, null, "Invisible Root Item");
            FilteringOptions renderingOptions = null;
            if (renderingFilter != null)
            {
                renderingOptions = renderingFilter.BuildFilteringOptions();
            }
            this.ParseTestList(parent, null, 0, testSuite, renderingOptions);
            return parent;
        }

        private void ParseTestList(TreeViewItem parent, EditorTestResultGroup parentResult, int depth, Test test, FilteringOptions renderingOptions)
        {
            IEnumerator enumerator = test.get_Categories().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = (string) enumerator.Current;
                    this.m_CategoryList.Add(current);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            if (test is TestMethod)
            {
                EditorTestResult result = this.FindExistingResult(test.get_TestName().get_FullName(), test.get_TestName().get_TestID().ToString());
                if (result == null)
                {
                    result = new EditorTestResult();
                }
                result.SetTestInfo(new EditorTestInfo(test as TestMethod));
                result.SetParent(parentResult);
                result.SetTestMethod(test as TestMethod);
                this.m_NewResultList.Add(result);
                if ((renderingOptions == null) || result.IsVisible(renderingOptions))
                {
                    TestLineTreeViewItem child = new TestLineTreeViewItem(test as TestMethod, depth, parent);
                    child.SetResult(result);
                    parent.AddChild(child);
                }
            }
            else
            {
                EditorTestResultGroup group = new EditorTestResultGroup(parentResult);
                TestGroupTreeViewItem item2 = parent as TestGroupTreeViewItem;
                if (test is NamespaceSuite)
                {
                    item2 = new TestGroupTreeViewItem(group, test as TestSuite, depth, parent);
                    parent.AddChild(item2);
                }
                else if (test is TestAssembly)
                {
                    depth--;
                }
                else if ((test is ParameterizedMethodSuite) || (test is ParameterizedFixtureSuite))
                {
                    item2 = new TestGroupTreeViewItem(group, test as TestSuite, depth, parent);
                    parent.AddChild(item2);
                }
                else if (test is NUnitTestFixture)
                {
                    if (test.get_Tests().Count > 0)
                    {
                        item2 = new TestGroupTreeViewItem(group, test as TestSuite, depth, parent);
                        parent.AddChild(item2);
                    }
                }
                else if (test is ProjectRootSuite)
                {
                    item2 = new TestGroupTreeViewItem(group, test as TestSuite, depth, parent);
                    parent.AddChild(item2);
                }
                depth++;
                IEnumerator enumerator2 = test.get_Tests().GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        Test test2 = (Test) enumerator2.Current;
                        this.ParseTestList(item2, group, depth, test2, renderingOptions);
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    if (disposable2 != null)
                    {
                        disposable2.Dispose();
                    }
                }
                if (((item2.children == null) || (item2.children.Count == 0)) && (parent.children != null))
                {
                    parent.children.Remove(item2);
                }
            }
        }

        public string[] categories =>
            this.m_CategoryList.ToArray<string>();

        public List<EditorTestResult> newResultList =>
            this.m_NewResultList;

        [CompilerGenerated]
        private sealed class <FindExistingResult>c__AnonStorey0
        {
            internal string fullTestName;
            internal string id;

            internal bool <>m__0(EditorTestResult t) => 
                (t.fullName == this.fullTestName);

            internal bool <>m__1(EditorTestResult t) => 
                (t.id == this.id);
        }
    }
}

