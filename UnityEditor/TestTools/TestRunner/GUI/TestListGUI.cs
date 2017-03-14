namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.TestTools.TestRunner;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal abstract class TestListGUI
    {
        [SerializeField]
        private string m_ResultStacktrace;
        [SerializeField]
        private string m_ResultText;
        private Vector2 m_TestInfoScroll;
        private Vector2 m_TestListScroll;
        [SerializeField]
        internal TreeViewState m_TestListState;
        private TreeViewController m_TestListTree;
        [SerializeField]
        protected TestRunnerWindow m_Window;
        [SerializeField]
        public List<TestRunnerResult> newResultList = new List<TestRunnerResult>();
        private static GUIContent s_GUIOpenErrorLine = new GUIContent("Open error line");
        private static GUIContent s_GUIOpenTest = new GUIContent("Open source code");
        private static GUIContent s_GUIRun = new GUIContent("Run");
        private static GUIContent s_GUIRunAllTests = new GUIContent("Run All", "Run all tests");
        private static GUIContent s_GUIRunSelectedTests = new GUIContent("Run Selected", "Run selected test(s)");

        protected TestListGUI()
        {
        }

        private TestTreeViewItem GetSelectedTest()
        {
            foreach (int num in this.m_TestListState.selectedIDs)
            {
                TreeViewItem item = this.m_TestListTree.FindItem(num);
                if (item is TestTreeViewItem)
                {
                    return (item as TestTreeViewItem);
                }
            }
            return null;
        }

        private string[] GetSelectedTests()
        {
            List<string> list = new List<string>();
            foreach (int num in this.m_TestListState.selectedIDs)
            {
                TreeViewItem item = this.m_TestListTree.FindItem(num);
                if (item is TestTreeViewItem)
                {
                    string fullName = (item as TestTreeViewItem).FullName;
                    list.Add(fullName);
                }
            }
            return list.ToArray();
        }

        public abstract ITest GetTestListNUnit();
        public void Init(TestRunnerWindow window)
        {
            if (this.m_Window == null)
            {
                this.m_Window = window;
            }
            if (this.m_TestListTree == null)
            {
                if (this.m_TestListState == null)
                {
                    this.m_TestListState = new TreeViewState();
                }
                if (this.m_TestListTree == null)
                {
                    this.m_TestListTree = new TreeViewController(this.m_Window, this.m_TestListState);
                }
                this.m_TestListTree.deselectOnUnhandledMouseDown = false;
                this.m_TestListTree.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TestListTree.selectionChangedCallback, new Action<int[]>(this.TestSelectionCallback));
                this.m_TestListTree.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TestListTree.itemDoubleClickedCallback, new Action<int>(this.TestDoubleClickCallback));
                this.m_TestListTree.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_TestListTree.contextClickItemCallback, new Action<int>(this.TestContextClick));
                TestListTreeViewDataSource data = new TestListTreeViewDataSource(this.m_TestListTree, this);
                if (!this.newResultList.Any<TestRunnerResult>())
                {
                    data.ExpandTreeOnCreation();
                }
                this.m_TestListTree.Init(new Rect(), data, new TestListTreeViewGUI(this.m_TestListTree), null);
            }
        }

        public virtual void PrintHeadPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            if (GUILayout.Button(s_GUIRunAllTests, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.RunTests(TestRunnerFilter.empty);
                GUIUtility.ExitGUI();
            }
            using (new EditorGUI.DisabledScope(!this.m_TestListTree.HasSelection()))
            {
                if (GUILayout.Button(s_GUIRunSelectedTests, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    List<string> list = new List<string>();
                    foreach (int num in this.m_TestListTree.GetSelection())
                    {
                        TreeViewItem item = this.m_TestListTree.FindItem(num);
                        if (item is TestTreeViewItem)
                        {
                            list.Add((item as TestTreeViewItem).FullName);
                        }
                    }
                    this.RunTests(new TestRunnerFilter(list.ToArray()));
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public void Reload()
        {
            this.m_TestListTree.ReloadData();
        }

        public void RenderDetails()
        {
            this.m_TestInfoScroll = EditorGUILayout.BeginScrollView(this.m_TestInfoScroll, new GUILayoutOption[0]);
            Vector2 vector = TestRunnerWindow.Styles.info.CalcSize(new GUIContent(this.m_ResultText));
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinWidth(vector.x), GUILayout.MinHeight(vector.y) };
            EditorGUILayout.SelectableLabel(this.m_ResultText, TestRunnerWindow.Styles.info, options);
            EditorGUILayout.EndScrollView();
        }

        public virtual void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
        }

        public virtual void RenderTestList()
        {
            if ((this.m_TestListTree.data.rowCount == 0) || !this.m_TestListTree.data.GetItem(0).hasChildren)
            {
                this.RenderNoTestsInfo();
            }
            else
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2000f) };
                this.m_TestListScroll = EditorGUILayout.BeginScrollView(this.m_TestListScroll, options);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
                Rect controlRect = EditorGUILayout.GetControlRect(optionArray2);
                int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
                this.m_TestListTree.OnGUI(controlRect, controlID);
                EditorGUILayout.EndScrollView();
            }
        }

        public void Repaint()
        {
            this.m_TestListTree.Repaint();
            if (this.m_TestListTree.data.rowCount == 0)
            {
                this.m_TestListTree.SetSelection(new int[0], false);
            }
            this.TestSelectionCallback(this.m_TestListState.selectedIDs.ToArray());
        }

        protected virtual void RunTests(TestRunnerFilter filter)
        {
            throw new NotImplementedException();
        }

        protected virtual void TestContextClick(int id)
        {
            <TestContextClick>c__AnonStorey2 storey = new <TestContextClick>c__AnonStorey2 {
                $this = this
            };
            if (id != 0)
            {
                GenericMenu menu = new GenericMenu();
                storey.testsToRun = this.GetSelectedTests();
                bool flag = this.m_TestListState.selectedIDs.Count > 1;
                if (!flag)
                {
                    <TestContextClick>c__AnonStorey1 storey2 = new <TestContextClick>c__AnonStorey1 {
                        <>f__ref$2 = storey,
                        testNode = this.GetSelectedTest()
                    };
                    if (!storey2.testNode.IsGroupNode)
                    {
                        if (!string.IsNullOrEmpty(this.m_ResultStacktrace))
                        {
                            menu.AddItem(s_GUIOpenErrorLine, false, new GenericMenu.MenuFunction2(storey2.<>m__0), "");
                        }
                        menu.AddItem(s_GUIOpenTest, false, new GenericMenu.MenuFunction2(storey2.<>m__1), "");
                        menu.AddSeparator("");
                    }
                }
                menu.AddItem(!flag ? s_GUIRun : s_GUIRunSelectedTests, false, new GenericMenu.MenuFunction2(storey.<>m__0), "");
                menu.ShowAsContext();
            }
        }

        protected virtual void TestDoubleClickCallback(int id)
        {
            TreeViewItem item = this.m_TestListTree.FindItem(id);
            if (item is TestTreeViewItem)
            {
                TestTreeViewItem item2 = item as TestTreeViewItem;
                TestRunnerFilter filter = new TestRunnerFilter(item2.FullName);
                this.RunTests(filter);
                GUIUtility.ExitGUI();
            }
        }

        internal void TestSelectionCallback(int[] selected)
        {
            if (selected.Length == 1)
            {
                TreeViewItem item = this.m_TestListTree.FindItem(selected[0]);
                if (item is TestTreeViewItem)
                {
                    TestTreeViewItem item2 = item as TestTreeViewItem;
                    this.m_ResultText = item2.GetResultText();
                    this.m_ResultStacktrace = item2.result.stacktrace;
                }
            }
            else if (selected.Length == 0)
            {
                this.m_ResultText = "";
            }
        }

        public void UpdateResult(TestRunnerResult result)
        {
            <UpdateResult>c__AnonStorey0 storey = new <UpdateResult>c__AnonStorey0 {
                result = result
            };
            TestRunnerResult result2 = Enumerable.FirstOrDefault<TestRunnerResult>(this.newResultList, new Func<TestRunnerResult, bool>(storey.<>m__0));
            if (result2 != null)
            {
                result2.Update(storey.result);
            }
        }

        [CompilerGenerated]
        private sealed class <TestContextClick>c__AnonStorey1
        {
            internal TestListGUI.<TestContextClick>c__AnonStorey2 <>f__ref$2;
            internal TestTreeViewItem testNode;

            internal void <>m__0(object data)
            {
                if (!GuiHelper.OpenInEditor(this.<>f__ref$2.$this.m_ResultStacktrace))
                {
                    GuiHelper.OpenInEditor(this.testNode.type, this.testNode.method);
                }
            }

            internal void <>m__1(object data)
            {
                GuiHelper.OpenInEditor(this.testNode.type, this.testNode.method);
            }
        }

        [CompilerGenerated]
        private sealed class <TestContextClick>c__AnonStorey2
        {
            internal TestListGUI $this;
            internal string[] testsToRun;

            internal void <>m__0(object data)
            {
                TestRunnerFilter filter = new TestRunnerFilter {
                    names = this.testsToRun
                };
                this.$this.RunTests(filter);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateResult>c__AnonStorey0
        {
            internal TestRunnerResult result;

            internal bool <>m__0(TestRunnerResult x) => 
                (x.id == this.result.id);
        }
    }
}

