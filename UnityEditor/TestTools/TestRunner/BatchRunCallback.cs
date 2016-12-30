namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;

    internal class BatchRunCallback : ScriptableObject, TestRunnerListener
    {
        private bool m_AnyTestsExecuted;
        private bool m_RunFailed;

        public void RunFinished(ITestResult testResults)
        {
            if (!this.m_AnyTestsExecuted)
            {
                EditorApplication.Exit(3);
            }
            else
            {
                EditorApplication.Exit(!this.m_RunFailed ? 0 : 2);
            }
        }

        public void RunStarted(ITest testsToRun)
        {
        }

        public void TestFinished(ITestResult result)
        {
            if (!result.Test.IsSuite && ((result.ResultState.Status == TestStatus.Failed) || (result.ResultState.Status == TestStatus.Inconclusive)))
            {
                this.m_RunFailed = true;
            }
        }

        public void TestStarted(ITest test)
        {
            if (!test.IsSuite)
            {
                this.m_AnyTestsExecuted = true;
            }
        }
    }
}

