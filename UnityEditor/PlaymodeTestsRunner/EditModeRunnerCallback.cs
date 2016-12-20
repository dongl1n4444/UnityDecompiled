namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.PlaymodeTestsRunner.TestLauncher;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class EditModeRunnerCallback : ScriptableObject, TestRunnerListener
    {
        private EditModeLauncherContextSettings m_Settings;

        public void RunFinished(List<TestResult> testResults)
        {
            this.m_Settings.Dispose();
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
            this.m_Settings = new EditModeLauncherContextSettings();
        }

        public void TestFinished(TestResult test)
        {
        }

        public void TestStarted(string testName)
        {
        }
    }
}

