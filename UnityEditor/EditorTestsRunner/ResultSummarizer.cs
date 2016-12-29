namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.EditorTests;

    internal class ResultSummarizer
    {
        private TimeSpan m_Duration;
        private int m_ErrorCount;
        private int m_FailureCount;
        private int m_IgnoreCount;
        private int m_InconclusiveCount;
        private int m_NotRunnable;
        private int m_ResultCount;
        private int m_SkipCount;
        private int m_SuccessCount;
        private int m_TestsRun;

        public ResultSummarizer(IEnumerable<ITestResult> results)
        {
            foreach (ITestResult result in results)
            {
                this.Summarize(result);
            }
        }

        public void Summarize(ITestResult result)
        {
            this.m_Duration += TimeSpan.FromSeconds(result.duration);
            this.m_ResultCount++;
            if (!result.executed)
            {
                if (result.isIgnored)
                {
                    this.m_IgnoreCount++;
                }
                else
                {
                    this.m_SkipCount++;
                }
            }
            else
            {
                switch (result.resultState)
                {
                    case TestResultState.Inconclusive:
                        this.m_InconclusiveCount++;
                        this.m_TestsRun++;
                        return;

                    case TestResultState.NotRunnable:
                        this.m_NotRunnable++;
                        return;

                    case TestResultState.Ignored:
                        this.m_IgnoreCount++;
                        return;

                    case TestResultState.Success:
                        this.m_SuccessCount++;
                        this.m_TestsRun++;
                        return;

                    case TestResultState.Failure:
                        this.m_FailureCount++;
                        this.m_TestsRun++;
                        return;

                    case TestResultState.Error:
                    case TestResultState.Cancelled:
                        this.m_ErrorCount++;
                        this.m_TestsRun++;
                        return;
                }
                this.m_SkipCount++;
            }
        }

        public double duration =>
            this.m_Duration.TotalSeconds;

        public int errors =>
            this.m_ErrorCount;

        public int failures =>
            this.m_FailureCount;

        public int ignored =>
            this.m_IgnoreCount;

        public int inconclusive =>
            this.m_InconclusiveCount;

        public int notRunnable =>
            this.m_NotRunnable;

        public int passed =>
            this.m_SuccessCount;

        public int resultCount =>
            this.m_ResultCount;

        public int skipped =>
            this.m_SkipCount;

        public bool success =>
            (this.m_FailureCount == 0);

        public int testsNotRun =>
            ((this.m_SkipCount + this.m_IgnoreCount) + this.m_NotRunnable);

        public int testsRun =>
            this.m_TestsRun;
    }
}

