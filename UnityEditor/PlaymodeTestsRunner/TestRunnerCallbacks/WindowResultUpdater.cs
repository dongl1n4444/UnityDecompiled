namespace UnityEditor.PlaymodeTestsRunner.TestRunnerCallbacks
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.PlaymodeTestsRunner;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class WindowResultUpdater : ScriptableObject, TestRunnerListener
    {
        public void RunFinished(List<TestResult> testResults)
        {
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
        }

        public void TestFinished(TestResult test)
        {
            if (PlaymodeTestsRunnerWindow.s_Instance != null)
            {
                PlaymodeTestsRunnerWindow.s_Instance.m_SelectedTestTypes.UpdateResult(test);
                PlaymodeTestsRunnerWindow.s_Instance.m_SelectedTestTypes.Repaint();
                PlaymodeTestsRunnerWindow.s_Instance.Repaint();
            }
        }

        public void TestStarted(string testName)
        {
        }
    }
}

