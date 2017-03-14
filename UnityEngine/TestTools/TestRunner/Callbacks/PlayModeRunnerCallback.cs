namespace UnityEngine.TestTools.TestRunner.Callbacks
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;

    internal class PlayModeRunnerCallback : MonoBehaviour, TestRunnerListener
    {
        private TestResultRenderer m_ResultRenderer;

        private void LogRecieved(string message, string stacktrace, LogType type)
        {
            if (TestContext.Out != null)
            {
                TestContext.Out.WriteLine(message);
            }
        }

        public void OnGUI()
        {
            if (this.m_ResultRenderer != null)
            {
                this.m_ResultRenderer.Draw();
            }
        }

        public void RunFinished(ITestResult testResults)
        {
            Application.logMessageReceivedThreaded -= new Application.LogCallback(this.LogRecieved);
            if (Camera.main == null)
            {
                base.gameObject.AddComponent<Camera>();
            }
            this.m_ResultRenderer = new TestResultRenderer(testResults);
            this.m_ResultRenderer.ShowResults();
        }

        public void RunStarted(ITest testsToRun)
        {
            Application.logMessageReceivedThreaded += new Application.LogCallback(this.LogRecieved);
        }

        public void TestFinished(ITestResult result)
        {
        }

        public void TestStarted(ITest test)
        {
        }
    }
}

