namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class TestRunnerCallback : ScriptableObject, TestRunnerListener
    {
        public void RunFinished(List<TestResult> testResults)
        {
            EditorApplication.isPlaying = false;
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
        }

        public void TestFinished(TestResult test)
        {
        }

        public void TestStarted(string testName)
        {
        }
    }
}

