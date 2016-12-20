namespace UnityEditor.PlaymodeTestsRunner.TestRunnerCallbacks
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class BatchRunCallback : ScriptableObject, TestRunnerListener
    {
        private bool m_AnyTestsExecuted;
        private bool m_RunFailed;
        private static int s_ReturnCodeRunError = 3;
        private static int s_ReturnCodeTestsFailed = 2;
        private static int s_ReturnCodeTestsOk = 0;

        public void RunFinished(List<TestResult> testResults)
        {
            if (!this.m_AnyTestsExecuted)
            {
                EditorApplication.Exit(s_ReturnCodeRunError);
            }
            else
            {
                EditorApplication.Exit(!this.m_RunFailed ? s_ReturnCodeTestsOk : s_ReturnCodeTestsFailed);
            }
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
        }

        public void TestFinished(TestResult test)
        {
            if (test.resultType != TestResult.ResultType.NotRun)
            {
                this.m_AnyTestsExecuted = true;
            }
            if (test.resultType != TestResult.ResultType.Success)
            {
                this.m_RunFailed = true;
            }
        }

        public void TestStarted(string fullName)
        {
        }
    }
}

