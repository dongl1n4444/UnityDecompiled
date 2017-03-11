namespace UnityEngine.TestTools.TestRunner.Callbacks
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class ResultsRenderer : MonoBehaviour, TestRunnerListener
    {
        private TestResultRenderer m_ResultRenderer = new TestResultRenderer();

        public void OnGUI()
        {
            this.m_ResultRenderer.Draw();
        }

        public void RunFinished(ITestResult testResults)
        {
            this.m_ResultRenderer.ShowResults();
        }

        public void RunStarted(ITest testsToRun)
        {
        }

        public void TestFinished(ITestResult result)
        {
            this.m_ResultRenderer.AddResults(new TestRunnerResult(result));
        }

        public void TestStarted(ITest test)
        {
        }
    }
}

