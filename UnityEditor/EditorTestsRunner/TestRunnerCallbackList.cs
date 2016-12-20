namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.EditorTests;

    internal class TestRunnerCallbackList : ITestRunnerCallback
    {
        private readonly List<ITestRunnerCallback> m_CallbackList = new List<ITestRunnerCallback>();

        public void Add(ITestRunnerCallback callback)
        {
            this.m_CallbackList.Add(callback);
        }

        public void Remove(ITestRunnerCallback callback)
        {
            this.m_CallbackList.Remove(callback);
        }

        public void RunFinished()
        {
            foreach (ITestRunnerCallback callback in this.m_CallbackList)
            {
                callback.RunFinished();
            }
        }

        public void RunFinishedException(Exception exception)
        {
            foreach (ITestRunnerCallback callback in this.m_CallbackList)
            {
                callback.RunFinishedException(exception);
            }
        }

        public void RunStarted(string suiteName, int testCount)
        {
            foreach (ITestRunnerCallback callback in this.m_CallbackList)
            {
                callback.RunStarted(suiteName, testCount);
            }
        }

        public void TestFinished(ITestResult fullName)
        {
            foreach (ITestRunnerCallback callback in this.m_CallbackList)
            {
                callback.TestFinished(fullName);
            }
        }

        public void TestStarted(string fullName)
        {
            foreach (ITestRunnerCallback callback in this.m_CallbackList)
            {
                callback.TestStarted(fullName);
            }
        }
    }
}

