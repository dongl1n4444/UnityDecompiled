namespace UnityEngine.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class TestRunnerResult
    {
        public string description;
        public float duration;
        public string fullName;
        public string id;
        public bool ignoredOrSkipped;
        protected Action<TestRunnerResult> m_OnResultUpdate;
        public string messages;
        public string name;
        [NonSerialized]
        public bool notOutdated;
        public bool notRunnable;
        public ResultStatus resultStatus;
        public string stacktrace;

        internal TestRunnerResult(ITest test)
        {
            this.resultStatus = ResultStatus.NotRun;
            this.id = GetId(test);
            this.fullName = test.FullName;
            this.name = test.Name;
            this.description = (string) test.Properties.Get("Description");
            this.ignoredOrSkipped = (test.RunState == RunState.Ignored) || (test.RunState == RunState.Skipped);
            this.notRunnable = test.RunState == RunState.NotRunnable;
            if (this.ignoredOrSkipped && test.Properties.ContainsKey("_SKIPREASON"))
            {
                this.messages = (string) test.Properties.Get("_SKIPREASON");
            }
            if (this.notRunnable)
            {
                this.resultStatus = ResultStatus.Failed;
                if (test.Properties.ContainsKey("_SKIPREASON"))
                {
                    this.messages = (string) test.Properties.Get("_SKIPREASON");
                }
            }
        }

        internal TestRunnerResult(ITestResult testResult) : this(testResult.Test)
        {
            this.notOutdated = true;
            this.messages = testResult.Message;
            this.stacktrace = testResult.StackTrace;
            this.duration = (float) testResult.Duration;
            this.resultStatus = ParseNUnitResultStatus(testResult.ResultState.Status);
        }

        private static string GetFullName(ITest test) => 
            ((test.Parent == null) ? ("[" + test.Name + "]") : $"{GetFullName(test.Parent)}[{test.Name}]");

        public static string GetId(ITest test)
        {
            <GetId>c__AnonStorey0 storey = new <GetId>c__AnonStorey0 {
                id = GetFullName(test)
            };
            if ((test.Parent is ParameterizedMethodSuite) && Enumerable.All<ITest>(test.Parent.Tests, new Func<ITest, bool>(storey.<>m__0)))
            {
                int index = test.Parent.Tests.IndexOf(test);
                storey.id = storey.id + index;
            }
            return storey.id;
        }

        private static ResultStatus ParseNUnitResultStatus(TestStatus status)
        {
            switch (status)
            {
                case TestStatus.Inconclusive:
                    return ResultStatus.Inconclusive;

                case TestStatus.Skipped:
                    return ResultStatus.Skipped;

                case TestStatus.Passed:
                    return ResultStatus.Passed;

                case TestStatus.Failed:
                    return ResultStatus.Failed;
            }
            return ResultStatus.NotRun;
        }

        public void SetResultChangedCallback(Action<TestRunnerResult> resultUpdated)
        {
            this.m_OnResultUpdate = resultUpdated;
        }

        public override string ToString() => 
            $"{this.name} ({this.fullName})";

        public void Update(TestRunnerResult result)
        {
            this.resultStatus = result.resultStatus;
            this.duration = result.duration;
            this.messages = result.messages;
            this.stacktrace = result.stacktrace;
            this.ignoredOrSkipped = result.ignoredOrSkipped;
            this.notRunnable = result.notRunnable;
            this.description = result.description;
            this.notOutdated = result.notOutdated;
            if (this.m_OnResultUpdate != null)
            {
                this.m_OnResultUpdate(this);
            }
        }

        [CompilerGenerated]
        private sealed class <GetId>c__AnonStorey0
        {
            internal string id;

            internal bool <>m__0(ITest t) => 
                (TestRunnerResult.GetFullName(t) == this.id);
        }

        [Serializable]
        internal enum ResultStatus
        {
            NotRun,
            Passed,
            Failed,
            Inconclusive,
            Skipped
        }
    }
}

