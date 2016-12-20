namespace UnityEditor.EditorTestsRunner
{
    using System;
    using UnityEditor.EditorTests;
    using UnityEngine;

    internal class DefaultVerboseTestLogger : ITestRunnerCallback
    {
        private int currentTest = 1;
        private int testCount;

        public void RunFinished()
        {
            Debug.Log("Editor Tests Run finished");
        }

        public void RunFinishedException(Exception exception)
        {
        }

        public void RunStarted(string suiteName, int testCount)
        {
            this.testCount = testCount;
            object[] args = new object[] { suiteName, testCount };
            Debug.LogFormat("Editor Tests Run started {0} ({1} tests)", args);
        }

        public void TestFinished(ITestResult testResult)
        {
            object[] args = new object[] { testResult.fullName, testResult.isSuccess };
            Debug.LogFormat("Test Finished: {0} {1}", args);
        }

        public void TestStarted(string fullName)
        {
            object[] args = new object[] { fullName, this.currentTest++, this.testCount };
            Debug.LogFormat("Running test: {0} [{1}/{2}]", args);
        }
    }
}

