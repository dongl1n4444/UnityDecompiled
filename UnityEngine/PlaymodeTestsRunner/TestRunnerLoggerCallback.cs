namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class TestRunnerLoggerCallback : MonoBehaviour, TestRunnerListener
    {
        public void RunFinished(List<TestResult> testResults)
        {
            Debug.Log("Finishing run");
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
            Debug.Log("Starting run");
        }

        public void TestFinished(TestResult test)
        {
            Debug.Log("Finishing test: " + test.name);
        }

        public void TestStarted(string testName)
        {
            Debug.Log("Starting test: " + testName);
        }
    }
}

