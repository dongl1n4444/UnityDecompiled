namespace UnityEditor.TestRunner.GUI
{
    using System;
    using UnityEditor;
    using UnityEditor.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class PlayerResultWindowUpdater : ScriptableSingleton<PlayerResultWindowUpdater>
    {
        public void Error()
        {
            foreach (TestRunnerResult result in TestRunnerWindow.s_Instance.m_SelectedTestTypes.newResultList)
            {
                result.resultStatus = TestRunnerResult.ResultStatus.Failed;
                TestRunnerWindow.s_Instance.m_SelectedTestTypes.UpdateResult(result);
            }
            this.UpdateWindow();
        }

        public void ResetTestState()
        {
            if (TestRunnerWindow.s_Instance != null)
            {
                foreach (TestRunnerResult result in TestRunnerWindow.s_Instance.m_SelectedTestTypes.newResultList)
                {
                    result.resultStatus = TestRunnerResult.ResultStatus.NotRun;
                    TestRunnerWindow.s_Instance.m_SelectedTestTypes.UpdateResult(result);
                }
                this.UpdateWindow();
            }
        }

        public void RunStarted()
        {
        }

        public void TestDone(TestRunnerResult testRunnerResult)
        {
            if (TestRunnerWindow.s_Instance != null)
            {
                TestRunnerWindow.s_Instance.m_SelectedTestTypes.UpdateResult(testRunnerResult);
                this.UpdateWindow();
            }
        }

        private void UpdateWindow()
        {
            TestRunnerWindow.s_Instance.m_SelectedTestTypes.Repaint();
            TestRunnerWindow.s_Instance.Repaint();
        }
    }
}

