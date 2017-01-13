namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class TestFilterSettings : ISerializationCallbackReceiver
    {
        [SerializeField]
        public string[] availableCategories = new string[0];
        [SerializeField]
        public string filterByName = "";
        private GUIContent m_FailedBtn;
        private GUIContent m_IgnoredBtn;
        private GUIContent m_NotRunBtn;
        private GUIContent m_SucceededBtn;
        private bool rebuildCategories;
        private int selectedCategory;
        [SerializeField]
        private List<string> selectedStringsList = new List<string>();
        [SerializeField]
        public bool showFailed = true;
        [SerializeField]
        public bool showIgnored = true;
        [SerializeField]
        public bool showNotRun = true;
        [SerializeField]
        public bool showSucceeded = true;

        internal FilteringOptions BuildFilteringOptions() => 
            new FilteringOptions { 
                showSucceeded = this.showSucceeded,
                showFailed = this.showFailed,
                showIgnored = this.showIgnored,
                showNotRunned = this.showNotRun,
                nameFilter = this.filterByName,
                categories = this.GetSelectedCategories()
            };

        public string[] GetSelectedCategories()
        {
            if (this.availableCategories == null)
            {
                return new string[0];
            }
            return Enumerable.Where<string>(this.availableCategories, (Func<string, int, bool>) ((c, i) => ((this.selectedCategory & (((int) 1) << i)) != 0))).ToArray<string>();
        }

        public void OnGUI()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f), GUILayout.MaxWidth(250f), GUILayout.ExpandWidth(true) };
            this.filterByName = GUILayout.TextField(this.filterByName, "ToolbarSeachTextField", options);
            if (GUILayout.Button(GUIContent.none, !string.IsNullOrEmpty(this.filterByName) ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty", new GUILayoutOption[0]))
            {
                this.filterByName = string.Empty;
            }
            if ((this.availableCategories != null) && (this.availableCategories.Length > 0))
            {
                if (this.rebuildCategories)
                {
                    this.selectedCategory = this.RebuildCategories();
                }
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(90f) };
                this.selectedCategory = EditorGUILayout.MaskField(this.selectedCategory, this.availableCategories, EditorStyles.toolbarDropDown, optionArray2);
            }
            this.showSucceeded = GUILayout.Toggle(this.showSucceeded, this.m_SucceededBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.showFailed = GUILayout.Toggle(this.showFailed, this.m_FailedBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.showIgnored = GUILayout.Toggle(this.showIgnored, this.m_IgnoredBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.showNotRun = GUILayout.Toggle(this.showNotRun, this.m_NotRunBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
        }

        private int RebuildCategories()
        {
            this.rebuildCategories = false;
            int num = 0;
            for (int i = 0; i < this.availableCategories.Length; i++)
            {
                string item = this.availableCategories[i];
                if (this.selectedStringsList.Contains(item))
                {
                    num |= ((int) 1) << i;
                }
            }
            return num;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.rebuildCategories = true;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.selectedStringsList.Clear();
            for (int i = 0; i < this.availableCategories.Length; i++)
            {
                if (this.availableCategories.Length == i)
                {
                    break;
                }
                if ((this.selectedCategory & (((int) 1) << i)) != 0)
                {
                    this.selectedStringsList.Add(this.availableCategories[i]);
                }
            }
        }

        public void UpdateCounters(IEnumerable<ITestResult> results)
        {
            ResultSummarizer summarizer = new ResultSummarizer(results);
            this.m_SucceededBtn = new GUIContent(summarizer.passed.ToString(), Icons.s_SuccessImg, "Show tests that succeeded");
            int num2 = (summarizer.errors + summarizer.failures) + summarizer.inconclusive;
            this.m_FailedBtn = new GUIContent(num2.ToString(), Icons.s_FailImg, "Show tests that failed");
            int num3 = summarizer.ignored + summarizer.notRunnable;
            this.m_IgnoredBtn = new GUIContent(num3.ToString(), Icons.s_IgnoreImg, "Show tests that are ignored");
            int num4 = (summarizer.testsNotRun - summarizer.ignored) - summarizer.notRunnable;
            this.m_NotRunBtn = new GUIContent(num4.ToString(), Icons.s_UnknownImg, "Show tests that didn't run");
        }
    }
}

