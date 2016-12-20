namespace UnityEditor.PlaymodeTestsRunner.TestRunnerCallbacks
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class AfterRunFinishedCallback : ScriptableObject, TestRunnerListener
    {
        public Action afterRun;

        public AfterRunFinishedCallback(Action afterRun)
        {
            this.afterRun = afterRun;
        }

        public void RunFinished(List<TestResult> testResults)
        {
            this.afterRun.Invoke();
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

