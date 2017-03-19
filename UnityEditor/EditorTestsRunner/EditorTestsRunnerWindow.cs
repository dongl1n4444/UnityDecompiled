namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditor.EditorTests;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    internal class EditorTestsRunnerWindow : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<EditorTestResult, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<EditorTestResult, TestName> <>f__am$cache1;
        [SerializeField]
        internal TestFilterSettings m_FilterSettings = new TestFilterSettings();
        private readonly GUIContent m_GUIDontPromptForSaving;
        private readonly GUIContent m_GUIHorizontalLayout;
        private readonly string m_GUINoTestsWereFound;
        private readonly GUIContent m_GUIOpenInEditor;
        private readonly GUIContent m_GUIRerunFailedTestsIcon;
        private readonly GUIContent m_GUIRun;
        private readonly GUIContent m_GUIRunAllTestsIcon;
        private readonly GUIContent m_GUIRunOnRecompile;
        private readonly GUIContent m_GUIRunSelected;
        private readonly GUIContent m_GUIRunSelectedTestsIcon;
        private readonly GUIContent m_GUIVerticalLayout;
        [SerializeField]
        internal List<EditorTestResult> m_ResultList = new List<EditorTestResult>();
        private string m_ResultText = "";
        private EditorTestsRunnerSettings m_Settings;
        private SplitterState m_Spl;
        private Vector2 m_TestInfoScroll;
        private Vector2 m_TestListScroll;
        [SerializeField]
        private TreeViewState m_TestListState;
        internal TreeViewController m_TestListTree;
        private TestListTreeViewDataSource m_TestListTreeViewDataSource;
        private TestRunnerFilter nextRunFilter;
        private static EditorTestsRunnerWindow s_Instance;
        internal static readonly NUnitTestEngine s_TestEngine = new NUnitTestEngine();

        public EditorTestsRunnerWindow()
        {
            float[] relativeSizes = new float[] { 75f, 25f };
            int[] minSizes = new int[] { 0x20, 0x20 };
            this.m_Spl = new SplitterState(relativeSizes, minSizes, null);
            this.m_GUIRunSelectedTestsIcon = new GUIContent("Run Selected", "Run selected tests");
            this.m_GUIRunAllTestsIcon = new GUIContent("Run All", "Run all tests");
            this.m_GUIRerunFailedTestsIcon = new GUIContent("Rerun Failed", "Rerun failed tests");
            this.m_GUIRunOnRecompile = new GUIContent("Run on recompilation", "Run all tests after successful recompilation");
            this.m_GUIDontPromptForSaving = new GUIContent("Don't prompt for saving scene", "Don't prompt for saving the scene when there are unsaved changes.");
            this.m_GUIVerticalLayout = new GUIContent("Vertical layout", "Show test details below the test list");
            this.m_GUIHorizontalLayout = new GUIContent("Horizontal layout", "Show test details next to the test list");
            this.m_GUIRunSelected = new GUIContent("Run Selected");
            this.m_GUIRun = new GUIContent("Run");
            this.m_GUIOpenInEditor = new GUIContent("Open in editor");
            this.m_GUINoTestsWereFound = "No tests were found.\nMake sure you placed your scripts under Editor folder.";
            this.nextRunFilter = null;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(this.m_GUIRunOnRecompile, this.m_Settings.runOnRecompilation, new GenericMenu.MenuFunction(this.m_Settings.ToggleRunOnRecompilation));
            menu.AddItem(this.m_GUIDontPromptForSaving, this.m_Settings.dontPromptForSaving, new GenericMenu.MenuFunction(this.m_Settings.ToggleDontPromptForSaving));
            menu.AddSeparator(string.Empty);
            menu.AddItem(this.m_GUIVerticalLayout, this.m_Settings.horizontalSplit, new GenericMenu.MenuFunction(this.m_Settings.ToggleHorizontalSplit));
            menu.AddItem(this.m_GUIHorizontalLayout, !this.m_Settings.horizontalSplit, new GenericMenu.MenuFunction(this.m_Settings.ToggleHorizontalSplit));
        }

        private List<TestName> GetSelectedTests()
        {
            List<TestName> list = new List<TestName>();
            foreach (int num in this.m_TestListState.selectedIDs)
            {
                TreeViewItem item = this.m_TestListTree.FindItem(num);
                if (item is EditorTestTreeViewItem)
                {
                    TestName testName = (item as EditorTestTreeViewItem).testName;
                    list.Add(testName);
                }
            }
            return list;
        }

        public void OnDestroy()
        {
            s_Instance = null;
        }

        [DidReloadScripts]
        public static void OnDidReloadScripts()
        {
            if (s_Instance != null)
            {
                if (s_Instance.m_Settings.runOnRecompilation)
                {
                    s_Instance.RunTests();
                    s_Instance.Repaint();
                }
                s_Instance.Repaint();
            }
        }

        public void OnEnable()
        {
            bool flag = false;
            base.titleContent = new GUIContent("Editor Tests");
            s_Instance = this;
            this.m_Settings = new EditorTestsRunnerSettings("UnityTest.EditorTestsRunnerWindow");
            this.m_FilterSettings.UpdateCounters(Enumerable.Empty<ITestResult>());
            if ((s_TestEngine.testSuite != null) && (s_TestEngine.testSuite.get_TestCount() != 0))
            {
                if (this.m_TestListTree == null)
                {
                    if (this.m_TestListState == null)
                    {
                        this.m_TestListState = new TreeViewState();
                    }
                    if (this.m_TestListTree == null)
                    {
                        this.m_TestListTree = new TreeViewController(this, this.m_TestListState);
                    }
                    this.m_TestListTree.deselectOnUnhandledMouseDown = false;
                    this.m_TestListTree.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TestListTree.selectionChangedCallback, new Action<int[]>(this.TestSelectionCallback));
                    this.m_TestListTree.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TestListTree.itemDoubleClickedCallback, new Action<int>(this.TestDoubleClickCallback));
                    this.m_TestListTree.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_TestListTree.contextClickItemCallback, new Action<int>(this.TestContextClick));
                    this.m_TestListTreeViewDataSource = new TestListTreeViewDataSource(this.m_TestListTree, this);
                    if ((this.m_ResultList == null) || (this.m_ResultList.Count<EditorTestResult>() == 0))
                    {
                        this.m_TestListTreeViewDataSource.ExpandTreeOnCreation();
                    }
                    this.m_TestListTree.Init(new Rect(), this.m_TestListTreeViewDataSource, new TestListTreeViewGUI(this.m_TestListTree), null);
                    flag = true;
                }
                this.m_TestListTree.ReloadData();
                this.m_ResultList = this.m_TestListTreeViewDataSource.resultState;
                if (flag)
                {
                    foreach (EditorTestResult result in this.m_TestListTreeViewDataSource.resultState)
                    {
                        result.outdated = true;
                    }
                }
                if (this.m_TestListTree.HasSelection())
                {
                    this.m_TestListTree.selectionChangedCallback(this.m_TestListTree.GetSelection());
                }
            }
        }

        public void OnGUI()
        {
            if (this.m_TestListTree == null)
            {
                EditorGUILayout.HelpBox(this.m_GUINoTestsWereFound, MessageType.Info);
                if (GUILayout.Button("Create editor test", new GUILayoutOption[0]))
                {
                    EditorApplication.ExecuteMenuItem("Assets/Create/Testing/Editor Test C# Script");
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
                if (GUILayout.Button(this.m_GUIRunAllTestsIcon, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    this.RunTests();
                    GUIUtility.ExitGUI();
                }
                using (new EditorGUI.DisabledScope(this.m_TestListState.selectedIDs.Count == 0))
                {
                    if (GUILayout.Button(this.m_GUIRunSelectedTestsIcon, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        this.RunTests(this.GetSelectedTests().ToArray());
                        GUIUtility.ExitGUI();
                    }
                }
                if (GUILayout.Button(this.m_GUIRerunFailedTestsIcon, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = result => result.isFailure || result.isError;
                    }
                    IEnumerable<EditorTestResult> source = Enumerable.Where<EditorTestResult>(this.m_ResultList, <>f__am$cache0);
                    if (source.Any<EditorTestResult>())
                    {
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = l => l.test.testMethod.get_TestName();
                        }
                        this.RunTests(Enumerable.Select<EditorTestResult, TestName>(source, <>f__am$cache1).ToArray<TestName>());
                        GUIUtility.ExitGUI();
                    }
                    else
                    {
                        Debug.Log("No failed tests found.");
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                this.m_FilterSettings.OnGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_TestListTree.searchString = this.m_FilterSettings.filterByName;
                    this.m_TestListTree.ReloadData();
                }
                EditorGUILayout.EndHorizontal();
                if (this.m_Settings.horizontalSplit)
                {
                    SplitterGUILayout.BeginVerticalSplit(this.m_Spl, new GUILayoutOption[0]);
                }
                else
                {
                    SplitterGUILayout.BeginHorizontalSplit(this.m_Spl, new GUILayoutOption[0]);
                }
                this.RenderTestList();
                this.RenderTestInfo();
                if (this.m_Settings.horizontalSplit)
                {
                    SplitterGUILayout.EndVerticalSplit();
                }
                else
                {
                    SplitterGUILayout.EndHorizontalSplit();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private Scene? OpenNewScene()
        {
            string operation = "Current scene is not saved. Do you want to save it?\n\n(You can disable this prompt in the Editor Tests Runner's options)";
            if (string.IsNullOrEmpty(SceneManager.GetActiveScene().path) && (this.m_Settings.dontPromptForSaving || !EditorSceneManager.EnsureUntitledSceneHasBeenSaved(operation)))
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                return null;
            }
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            return new Scene?(scene);
        }

        private static void PerformUndo(int undoGroup)
        {
            EditorUtility.DisplayProgressBar("Undo", "Reverting changes to the scene", 0f);
            DateTime now = DateTime.Now;
            Undo.RevertAllDownToGroup(undoGroup);
            TimeSpan span = (TimeSpan) (DateTime.Now - now);
            if (span.TotalSeconds > 1.0)
            {
                TimeSpan span2 = (TimeSpan) (DateTime.Now - now);
                Debug.LogWarning("Undo after editor test run took " + span2.Seconds + " seconds.");
            }
            EditorUtility.ClearProgressBar();
        }

        private static int RegisterUndo() => 
            Undo.GetCurrentGroup();

        private void RenderTestInfo()
        {
            this.m_TestInfoScroll = EditorGUILayout.BeginScrollView(this.m_TestInfoScroll, new GUILayoutOption[0]);
            Vector2 vector = Styles.info.CalcSize(new GUIContent(this.m_ResultText));
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinWidth(vector.x), GUILayout.MinHeight(vector.y) };
            EditorGUILayout.SelectableLabel(this.m_ResultText, Styles.info, options);
            EditorGUILayout.EndScrollView();
        }

        private void RenderTestList()
        {
            EditorGUILayout.BeginVertical(Styles.testList, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2000f) };
            this.m_TestListScroll = EditorGUILayout.BeginScrollView(this.m_TestListScroll, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
            Rect controlRect = EditorGUILayout.GetControlRect(optionArray2);
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TestListTree.OnGUI(controlRect, controlID);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        internal void RunTests()
        {
            TestRunnerFilter filter = new TestRunnerFilter();
            string[] selectedCategories = this.m_FilterSettings.GetSelectedCategories();
            if ((selectedCategories != null) && (selectedCategories.Length > 0))
            {
                filter.categories = selectedCategories;
            }
            this.RunTests(filter);
        }

        internal void RunTests(TestName[] tests)
        {
            TestRunnerFilter filter = new TestRunnerFilter {
                testnames = tests
            };
            string[] selectedCategories = this.m_FilterSettings.GetSelectedCategories();
            if ((selectedCategories != null) && (selectedCategories.Length > 0))
            {
                filter.categories = selectedCategories;
            }
            this.RunTests(filter);
        }

        internal void RunTests(string[] tests)
        {
            TestRunnerFilter filter = new TestRunnerFilter {
                names = tests
            };
            string[] selectedCategories = this.m_FilterSettings.GetSelectedCategories();
            if ((selectedCategories != null) && (selectedCategories.Length > 0))
            {
                filter.categories = selectedCategories;
            }
            this.RunTests(filter);
        }

        private void RunTests(TestRunnerFilter filter)
        {
            this.nextRunFilter = filter;
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.RunTestsOnUpdate));
        }

        private void RunTestsOnUpdate()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.RunTestsOnUpdate));
            TestRunnerFilter nextRunFilter = this.nextRunFilter;
            this.nextRunFilter = null;
            EditorApplication.LockReloadAssemblies();
            int undoGroup = -1;
            undoGroup = RegisterUndo();
            Scene? nullable = null;
            nullable = this.OpenNewScene();
            this.StartTestRun(nextRunFilter, new TestRunnerEventListener(new Action<ITestResult>(this.UpdateTestInfo)));
            if (nullable.HasValue)
            {
                EditorSceneManager.CloseScene(nullable.Value, true);
            }
            PerformUndo(undoGroup);
            EditorApplication.UnlockReloadAssemblies();
            this.m_TestListTreeViewDataSource.ReloadData();
            this.m_TestListTree.Repaint();
            base.Repaint();
        }

        [UnityEditor.MenuItem("Window/Editor Tests Runner", false, 0x7df)]
        public static void ShowEditorTestsRunnerWindow()
        {
            EditorWindow.GetWindow(typeof(EditorTestsRunnerWindow)).Show();
        }

        public void StartTestRun(TestRunnerFilter filter, ITestRunnerCallback eventListener)
        {
            TestRunnerCallbackList testRunnerEventListener = new TestRunnerCallbackList();
            if (eventListener != null)
            {
                testRunnerEventListener.Add(eventListener);
            }
            s_TestEngine.RunTests(filter, testRunnerEventListener);
        }

        private void TestContextClick(int selected)
        {
            <TestContextClick>c__AnonStorey1 storey = new <TestContextClick>c__AnonStorey1 {
                $this = this
            };
            if (selected != 0)
            {
                GenericMenu menu = new GenericMenu();
                storey.namesToRun = this.GetSelectedTests();
                bool flag = this.m_TestListState.selectedIDs.Count > 1;
                if (!flag)
                {
                    <TestContextClick>c__AnonStorey0 storey2 = new <TestContextClick>c__AnonStorey0 {
                        node = this.m_TestListTree.FindItem(selected)
                    };
                    if (storey2.node is TestLineTreeViewItem)
                    {
                        menu.AddItem(this.m_GUIOpenInEditor, false, new GenericMenu.MenuFunction2(storey2.<>m__0), "");
                        menu.AddSeparator("");
                    }
                }
                menu.AddItem(!flag ? this.m_GUIRun : this.m_GUIRunSelected, false, new GenericMenu.MenuFunction2(storey.<>m__0), "");
                menu.ShowAsContext();
            }
        }

        private void TestDoubleClickCallback(int nodeId)
        {
            TreeViewItem item = this.m_TestListTree.FindItem(nodeId);
            UnityEngine.Event current = UnityEngine.Event.current;
            if (current.control || current.command)
            {
                if (item is TestLineTreeViewItem)
                {
                    GuiHelper.OpenInEditor((item as TestLineTreeViewItem).result, true);
                }
            }
            else
            {
                if (item is EditorTestTreeViewItem)
                {
                    TestName[] tests = new TestName[] { (item as EditorTestTreeViewItem).testName };
                    this.RunTests(tests);
                }
                GUIUtility.ExitGUI();
            }
        }

        private void TestSelectionCallback(int[] selected)
        {
            if (selected.Length == 1)
            {
                TreeViewItem item = this.m_TestListTree.FindItem(selected[0]);
                if (item is TestLineTreeViewItem)
                {
                    TestLineTreeViewItem item2 = item as TestLineTreeViewItem;
                    this.m_ResultText = item2.result.GetResultText();
                }
                else if (item is TestGroupTreeViewItem)
                {
                    TestGroupTreeViewItem item3 = item as TestGroupTreeViewItem;
                    this.m_ResultText = $"{item3.GroupName} ({item3.GroupFullName})
{item3.GroupDescription}";
                }
            }
            else if (selected.Length == 0)
            {
                this.m_ResultText = "";
            }
        }

        private void UpdateTestInfo(ITestResult newResult)
        {
            <UpdateTestInfo>c__AnonStorey2 storey = new <UpdateTestInfo>c__AnonStorey2 {
                newResult = newResult
            };
            EditorTestResult result = this.m_ResultList.Find(new Predicate<EditorTestResult>(storey.<>m__0));
            if (result == null)
            {
                result = this.m_ResultList.Find(new Predicate<EditorTestResult>(storey.<>m__1));
            }
            result.Update(storey.newResult, false);
            this.m_TestListTree.selectionChangedCallback(this.m_TestListTree.GetSelection());
            this.m_FilterSettings.UpdateCounters(this.m_ResultList.Cast<ITestResult>());
        }

        [CompilerGenerated]
        private sealed class <TestContextClick>c__AnonStorey0
        {
            internal TreeViewItem node;

            internal void <>m__0(object data)
            {
                GuiHelper.OpenInEditor((this.node as TestLineTreeViewItem).result, false);
            }
        }

        [CompilerGenerated]
        private sealed class <TestContextClick>c__AnonStorey1
        {
            internal EditorTestsRunnerWindow $this;
            internal List<TestName> namesToRun;

            internal void <>m__0(object data)
            {
                this.$this.RunTests(this.namesToRun.ToArray());
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateTestInfo>c__AnonStorey2
        {
            internal ITestResult newResult;

            internal bool <>m__0(EditorTestResult r) => 
                ((r.id == this.newResult.id) && (r.fullName == this.newResult.fullName));

            internal bool <>m__1(EditorTestResult r) => 
                (r.fullName == this.newResult.fullName);
        }

        private static class Styles
        {
            public static GUIStyle info = new GUIStyle(EditorStyles.wordWrappedLabel);
            public static GUIStyle testList;

            static Styles()
            {
                info.wordWrap = false;
                info.stretchHeight = true;
                info.margin.right = 15;
                testList = new GUIStyle("CN Box");
                testList.margin.top = 0;
                testList.padding.left = 3;
            }
        }

        public class TestRunnerEventListener : ITestRunnerCallback
        {
            private int m_TestCount;
            private readonly Action<ITestResult> m_UpdateCallback;

            public TestRunnerEventListener(Action<ITestResult> updateCallback)
            {
                this.m_UpdateCallback = updateCallback;
            }

            public void RunFinished()
            {
                EditorUtility.ClearProgressBar();
            }

            public void RunFinishedException(Exception exception)
            {
                this.RunFinished();
            }

            public void RunStarted(string suiteName, int testCount)
            {
                this.m_TestCount = testCount;
            }

            public void TestFinished(ITestResult result)
            {
                this.m_UpdateCallback(result);
            }

            public void TestStarted(string fullName)
            {
                if (this.m_TestCount > 100)
                {
                    EditorUtility.DisplayProgressBar("Editor Tests Runner", "Running editor tests...", 1f);
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Editor Tests Runner", fullName, 1f);
                }
            }
        }
    }
}

