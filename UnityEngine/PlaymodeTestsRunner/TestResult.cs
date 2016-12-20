namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

    [Serializable]
    internal class TestResult
    {
        public float duration;
        public string fullName;
        public string id;
        protected Action<TestResult> m_OnResultUpdate;
        public string messages;
        public string name;
        public ResultType resultType;
        public string stacktrace;

        public TestResult(string id)
        {
            this.resultType = ResultType.NotRun;
            this.id = id;
        }

        internal TestResult(TestListElement test)
        {
            this.resultType = ResultType.NotRun;
            this.id = test.id;
            this.fullName = test.fullName;
            this.name = test.name;
        }

        public void SetResultChangedCallback(Action<TestResult> resultUpdated)
        {
            this.m_OnResultUpdate = resultUpdated;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.name, this.fullName);
        }

        public void Update(TestResult result)
        {
            this.resultType = result.resultType;
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

