namespace UnityEngine.TestTools.TestRunner.Callbacks
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class ResultsRenderer : MonoBehaviour, TestRunnerListener
    {
        private TestResultRenderer resultRenderer = new TestResultRenderer();
        private List<TestRunnerResult> results = new List<TestRunnerResult>();

        public void OnGUI()
        {
            this.resultRenderer.Draw();
        }

        public void RunFinished(ITestResult testResults)
        {
            this.resultRenderer.ShowResults();
        }

        public void RunStarted(ITest testsToRun)
        {
        }

        public void TestFinished(ITestResult result)
        {
            this.resultRenderer.AddResults(TestRunnerResult.FromNUnitResult(result));
            this.results.Add(TestRunnerResult.FromNUnitResult(result));
        }

        public void TestStarted(ITest test)
        {
        }
    }
}

