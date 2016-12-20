namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.PlaymodeTestsRunner;

    internal class ResultSummarizer
    {
        private TimeSpan m_Duration = TimeSpan.FromSeconds(0.0);
        private int m_ErrorCount = -1;
        private int m_FailureCount;
        private int m_IgnoreCount = -1;
        private int m_InconclusiveCount = -1;
        private int m_NotRunnable = -1;
        private int m_ResultCount;
        private int m_SkipCount;
        private int m_SuccessCount;
        private int m_TestsRun;

        public ResultSummarizer(IEnumerable<TestResult> results)
        {
            foreach (TestResult result in results)
            {
                this.Summarize(result);
            }
        }

        public void Summarize(TestResult result)
        {
            this.m_Duration += TimeSpan.FromSeconds((double) result.duration);
            this.m_ResultCount++;
            if (result.resultType == TestResult.ResultType.NotRun)
            {
                this.m_SkipCount++;
            }
            else
            {
                TestResult.ResultType resultType = result.resultType;
                if (resultType == TestResult.ResultType.Success)
                {
                    this.m_SuccessCount++;
                    this.m_TestsRun++;
                }
                else if (resultType == TestResult.ResultType.Failed)
                {
                    this.m_FailureCount++;
                    this.m_TestsRun++;
                }
                else
                {
                    this.m_SkipCount++;
                }
            }
        }

        public double duration
        {
            get
            {
                return this.m_Duration.TotalSeconds;
            }
        }

        public int errors
        {
            get
            {
                return this.m_ErrorCount;
            }
        }

        public int failures
        {
            get
            {
                return this.m_FailureCount;
            }
        }

        public int ignored
        {
            get
            {
                return this.m_IgnoreCount;
            }
        }

        public int inconclusive
        {
            get
            {
                return this.m_InconclusiveCount;
            }
        }

        public int notRunnable
        {
            get
            {
                return this.m_NotRunnable;
            }
        }

        public int Passed
        {
            get
            {
                return this.m_SuccessCount;
            }
        }

        public int ResultCount
        {
            get
            {
                return this.m_ResultCount;
            }
        }

        public int Skipped
        {
            get
            {
                return this.m_SkipCount;
            }
        }

        public bool success
        {
            get
            {
                return (this.m_FailureCount == 0);
            }
        }

        public int testsNotRun
        {
            get
            {
                return ((this.m_SkipCount + this.m_IgnoreCount) + this.m_NotRunnable);
            }
        }

        public int TestsRun
        {
            get
            {
                return this.m_TestsRun;
            }
        }
    }
}

