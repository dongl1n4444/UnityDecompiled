namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ResultsRenderer : MonoBehaviour, TestRunnerListener
    {
        private TestResultRenderer resultRenderer = new TestResultRenderer();
        private List<TestResult> results = new List<TestResult>();

        public void OnGUI()
        {
            this.resultRenderer.Draw();
        }

        public void RunFinished(List<TestResult> testResults)
        {
            this.resultRenderer.ShowResults();
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
        }

        public void TestFinished(TestResult test)
        {
            Debug.Log("Test run finished");
            this.resultRenderer.AddResults(test);
            this.results.Add(test);
        }

        public void TestStarted(string testName)
        {
        }
    }
}

