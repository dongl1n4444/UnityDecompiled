namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class TestResultRenderer
    {
        [CompilerGenerated]
        private static Func<TestResult, string> <>f__am$cache0;
        private readonly List<TestResult> m_FailedTestCollection = new List<TestResult>();
        private int m_FailureCount;
        private Vector2 m_ScrollPosition;
        private bool m_ShowResults;
        private readonly List<TestResult> m_TestCollection = new List<TestResult>();

        internal void AddResults(TestResult result)
        {
            this.m_TestCollection.Add(result);
            if (result.resultType != TestResult.ResultType.Success)
            {
                this.m_FailureCount++;
                this.m_FailedTestCollection.Add(result);
            }
        }

        public void Draw()
        {
            if (this.m_ShowResults)
            {
                if (this.m_FailedTestCollection.Count == 0)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(600f) };
                    GUILayout.Label("All test succeeded", Styles.SucceedLabelStyle, options);
                }
                else
                {
                    GUILayout.Label(this.m_FailedTestCollection.Count + " tests failed!", Styles.FailedLabelStyle, new GUILayoutOption[0]);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                    this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, optionArray2);
                    string str = "";
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<TestResult, string>(null, (IntPtr) <Draw>m__0);
                    }
                    GUILayout.TextArea(str + "<b><size=18>Code-based tests</size></b>\n" + string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<TestResult, string>(this.m_FailedTestCollection, <>f__am$cache0))), Styles.FailedMessagesStyle, new GUILayoutOption[0]);
                    GUILayout.EndScrollView();
                }
                if (GUILayout.Button("Close", new GUILayoutOption[0]))
                {
                    Application.Quit();
                }
            }
        }

        public int FailureCount()
        {
            return this.m_FailureCount;
        }

        public void ShowResults()
        {
            this.m_ShowResults = true;
            Cursor.visible = true;
        }

        private static class Styles
        {
            public static readonly GUIStyle FailedLabelStyle;
            public static readonly GUIStyle FailedMessagesStyle;
            public static readonly GUIStyle SucceedLabelStyle = new GUIStyle("label");

            static Styles()
            {
                SucceedLabelStyle.normal.textColor = Color.green;
                SucceedLabelStyle.fontSize = 0x30;
                FailedLabelStyle = new GUIStyle("label");
                FailedLabelStyle.normal.textColor = Color.red;
                FailedLabelStyle.fontSize = 0x20;
                FailedMessagesStyle = new GUIStyle("label");
                FailedMessagesStyle.wordWrap = false;
                FailedMessagesStyle.richText = true;
            }
        }
    }
}

