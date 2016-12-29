namespace UnityEditor.PlaymodeTestsRunner.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.PlaymodeTestsRunner;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

    internal abstract class TestListGUI
    {
        [SerializeField]
        private string m_ResultText;
        private Vector2 m_TestInfoScroll;
        private Vector2 m_TestListScroll;
        [SerializeField]
        internal TreeViewState m_TestListState;
        private TreeViewController m_TestListTree;
        [SerializeField]
        protected PlaymodeTestsRunnerWindow m_Window;
        [SerializeField]
        public List<TestResult> newResultList = new List<TestResult>();
        private static GUIContent s_GUIRunAllTests = new GUIContent("Run All", "Run all tests");
        private static GUIContent s_GUIRunSelectedTests = new GUIContent("Run Selected", "Run selected test(s)");

        protected TestListGUI()
        {
        }

        public abstract TestListElement GetTestList();
        public void Init(PlaymodeTestsRunnerWindow window)
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
                if (!this.newResultList.Any<TestResult>())
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
            }
            using (new EditorGUI.DisabledScope(!this.m_TestListTree.HasSelection()))
            {
                if (GUILayout.Button(s_GUIRunSelectedTests, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    List<string> list = new List<string>();
                    foreach (int num in this.m_TestListTree.GetSelection())
                    {
                        TreeViewItem item = this.m_TestListTree.FindItem(num);
                        if (item is TestLineTreeViewItem)
                        {
                            list.Add((item as TestLineTreeViewItem).fullName);
                        }
                        else if (item is TestGroupTreeViewItem)
                        {
                            list.Add((item as TestGroupTreeViewItem).fullName);
                        }
                    }
                    this.RunTests(new TestRunnerFilter(list.ToArray()));
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
            Vector2 vector = PlaymodeTestsRunnerWindow.Styles.info.CalcSize(new GUIContent(this.m_ResultText));
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinWidth(vector.x), GUILayout.MinHeight(vector.y) };
            EditorGUILayout.SelectableLabel(this.m_ResultText, PlaymodeTestsRunnerWindow.Styles.info, options);
            EditorGUILayout.EndScrollView();
        }

        public virtual void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
        }

        public void RenderTestList()
        {
            if (this.m_TestListTree.data.rowCount == 0)
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
        }

        protected virtual void TestDoubleClickCallback(int id)
        {
            TreeViewItem item = this.m_TestListTree.FindItem(id);
            if (item is TestLineTreeViewItem)
            {
                TestLineTreeViewItem item2 = item as TestLineTreeViewItem;
                Event current = Event.current;
                if (current.control || current.command)
                {
                    GuiHelper.OpenInEditor(item2.type, item2.method);
                    return;
                }
                TestRunnerFilter filter = new TestRunnerFilter(item2.fullName);
                this.RunTests(filter);
            }
            GUIUtility.ExitGUI();
        }

        internal void TestSelectionCallback(int[] selected)
        {
            if (selected.Length == 1)
            {
                TreeViewItem item = this.m_TestListTree.FindItem(selected[0]);
                if (item is TestLineTreeViewItem)
                {
                    this.m_ResultText = (item as TestLineTreeViewItem).GetResultText();
                }
                else if (item is TestGroupTreeViewItem)
                {
                }
            }
            else if (selected.Length == 0)
            {
                this.m_ResultText = "";
            }
        }

        public void UpdateResult(TestResult result)
        {
            <UpdateResult>c__AnonStorey0 storey = new <UpdateResult>c__AnonStorey0 {
                result = result
            };
            TestResult result2 = Enumerable.FirstOrDefault<TestResult>(this.newResultList, new Func<TestResult, bool>(storey, (IntPtr) this.<>m__0));
            if (result2 != null)
            {
                result2.Update(storey.result);
            }
            else
            {
                Debug.LogWarning("result null for " + storey.result.name);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateResult>c__AnonStorey0
        {
            internal TestResult result;

            internal bool <>m__0(TestResult r) => 
                (r.id == this.result.id);
        }
    }
}

