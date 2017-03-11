namespace UnityEngine.TestTools.TestRunner.Callbacks
{
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class TestResultRenderer
    {
        [CompilerGenerated]
        private static Func<ITestResult, string> <>f__am$cache0;
        private const int k_MaxStringLength = 0x3a98;
        private readonly List<ITestResult> m_FailedTestCollection = new List<ITestResult>();
        private Vector2 m_ScrollPosition;
        private bool m_ShowResults;

        public TestResultRenderer(ITestResult testResults)
        {
            this.GetFailedTests(testResults);
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
                    string text = "";
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = result => string.Concat(new object[] { result.Name, " ", result.ResultState, "\n", result.Message });
                    }
                    text = text + "<b><size=18>Code-based tests</size></b>\n" + string.Join("\n", Enumerable.Select<ITestResult, string>(this.m_FailedTestCollection, <>f__am$cache0).ToArray<string>());
                    if (text.Length > 0x3a98)
                    {
                        text = text.Substring(0, 0x3a98);
                    }
                    GUILayout.TextArea(text, Styles.FailedMessagesStyle, new GUILayoutOption[0]);
                    GUILayout.EndScrollView();
                }
                if (GUILayout.Button("Close", new GUILayoutOption[0]))
                {
                    Application.Quit();
                }
            }
        }

        private void GetFailedTests(ITestResult testResults)
        {
            if (testResults is TestCaseResult)
            {
                if (testResults.ResultState.Status == TestStatus.Failed)
                {
                    this.m_FailedTestCollection.Add(testResults);
                }
            }
            else if (testResults.HasChildren)
            {
                foreach (ITestResult result in testResults.Children)
                {
                    this.GetFailedTests(result);
                }
            }
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

