namespace UnityEngine.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Interfaces;
    using System;

    [Serializable]
    internal class TestRunnerResult
    {
        public float duration;
        public string fullName;
        public string id;
        protected Action<TestRunnerResult> m_OnResultUpdate;
        public string messages;
        public string name;
        public ResultType resultType;
        public string stacktrace;

        internal TestRunnerResult(ITest test)
        {
            this.resultType = ResultType.NotRun;
            this.id = test.FullName;
            this.fullName = test.FullName;
            this.name = test.Name;
        }

        public TestRunnerResult(string id)
        {
            this.resultType = ResultType.NotRun;
            this.id = id;
        }

        public static TestRunnerResult FromNUnitResult(ITestResult result) => 
            new TestRunnerResult(result.FullName) { 
                name = result.Name,
                fullName = result.FullName,
                messages = result.Message,
                stacktrace = result.StackTrace,
                duration = (float) result.Duration,
                resultType = (result.ResultState != ResultState.Success) ? ResultType.Failed : ResultType.Success
            };

        public void SetResultChangedCallback(Action<TestRunnerResult> resultUpdated)
        {
            this.m_OnResultUpdate = resultUpdated;
        }

        public override string ToString() => 
            $"{this.name} ({this.fullName})";

        public void Update(TestRunnerResult result)
        {
            this.resultType = result.resultType;
            this.duration = result.duration;
            this.messages = result.messages;
            this.stacktrace = result.stacktrace;
            if (this.m_OnResultUpdate != null)
            {
                this.m_OnResultUpdate(this);
            }
        }

        [Serializable]
        public enum ResultType
        {
            Success,
            Failed,
            NotRun
        }
    }
}

