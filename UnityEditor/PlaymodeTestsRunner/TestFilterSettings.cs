namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    internal class TestFilterSettings
    {
        public string[] availableCategories = null;
        public int filterByCategory;
        public string filterByName;
        private GUIContent m_FailedBtn;
        private GUIContent m_IgnoredBtn;
        private GUIContent m_NotRunBtn;
        private readonly string m_PrefsKey;
        private GUIContent m_SucceededBtn;
        public bool showFailed;
        public bool showIgnored;
        public bool showNotRun;
        public bool showSucceeded;

        public TestFilterSettings(string prefsKey)
        {
            this.m_PrefsKey = prefsKey;
            this.Load();
            this.UpdateCounters(Enumerable.Empty<TestResult>());
        }

        public RenderingOptions BuildRenderingOptions() => 
            new RenderingOptions { 
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
            return Enumerable.Where<string>(this.availableCategories, (Func<string, int, bool>) ((c, i) => ((this.filterByCategory & (((int) 1) << i)) != 0))).ToArray<string>();
        }

        public void Load()
        {
            this.showSucceeded = EditorPrefs.GetBool(this.m_PrefsKey + ".ShowSucceeded", true);
            this.showFailed = EditorPrefs.GetBool(this.m_PrefsKey + ".ShowFailed", true);
            this.showIgnored = EditorPrefs.GetBool(this.m_PrefsKey + ".ShowIgnored", true);
            this.showNotRun = EditorPrefs.GetBool(this.m_PrefsKey + ".ShowNotRun", true);
            this.filterByName = EditorPrefs.GetString(this.m_PrefsKey + ".FilterByName", string.Empty);
            this.filterByCategory = EditorPrefs.GetInt(this.m_PrefsKey + ".FilterByCategory", 0);
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f), GUILayout.MaxWidth(250f), GUILayout.ExpandWidth(true) };
            this.filterByName = GUILayout.TextField(this.filterByName, "ToolbarSeachTextField", options);
            if (GUILayout.Button(GUIContent.none, !string.IsNullOrEmpty(this.filterByName) ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty", new GUILayoutOption[0]))
            {
                this.filterByName = string.Empty;
            }
            if ((this.availableCategories != null) && (this.availableCategories.Length > 0))
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(90f) };
                this.filterByCategory = EditorGUILayout.MaskField(this.filterByCategory, this.availableCategories, EditorStyles.toolbarDropDown, optionArray2);
            }
            this.showSucceeded = GUILayout.Toggle(this.showSucceeded, this.m_SucceededBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.showFailed = GUILayout.Toggle(this.showFailed, this.m_FailedBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.showIgnored = GUILayout.Toggle(this.showIgnored, this.m_IgnoredBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.showNotRun = GUILayout.Toggle(this.showNotRun, this.m_NotRunBtn, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.Save();
            }
        }

        public void Save()
        {
            EditorPrefs.SetBool(this.m_PrefsKey + ".ShowSucceeded", this.showSucceeded);
            EditorPrefs.SetBool(this.m_PrefsKey + ".ShowFailed", this.showFailed);
            EditorPrefs.SetBool(this.m_PrefsKey + ".ShowIgnored", this.showIgnored);
            EditorPrefs.SetBool(this.m_PrefsKey + ".ShowNotRun", this.showNotRun);
            EditorPrefs.SetString(this.m_PrefsKey + ".FilterByName", this.filterByName);
            EditorPrefs.SetInt(this.m_PrefsKey + ".FilterByCategory", this.filterByCategory);
        }

        public void UpdateCounters(IEnumerable<TestResult> results)
        {
            ResultSummarizer summarizer = new ResultSummarizer(results);
            this.m_SucceededBtn = new GUIContent(summarizer.Passed.ToString(), Icons.s_SuccessImg, "Show tests that succeeded");
            int num2 = (summarizer.errors + summarizer.failures) + summarizer.inconclusive;
            this.m_FailedBtn = new GUIContent(num2.ToString(), Icons.s_FailImg, "Show tests that failed");
            int num3 = summarizer.ignored + summarizer.notRunnable;
            this.m_IgnoredBtn = new GUIContent(num3.ToString(), Icons.s_IgnoreImg, "Show tests that are ignored");
            int num4 = (summarizer.testsNotRun - summarizer.ignored) - summarizer.notRunnable;
            this.m_NotRunBtn = new GUIContent(num4.ToString(), Icons.s_UnknownImg, "Show tests that didn't run");
        }
    }
}

