namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using System.Linq;
    using System.Text;
    using UnityEditor.EditorTests;
    using UnityEngine;

    [Serializable]
    internal class EditorTestResult : EditorTestResultBase, ITestResult
    {
        private const int k_MaxLineLenght = 0x3a98;
        [SerializeField]
        private double m_Duration;
        [SerializeField]
        private bool m_Executed;
        [SerializeField]
        private bool m_IsIgnored;
        [SerializeField]
        private string m_Logs;
        [SerializeField]
        private string m_Message;
        [SerializeField]
        private bool m_Outdated;
        [SerializeField]
        private TestResultState m_ResultState;
        [SerializeField]
        private string m_StackTrace;
        [SerializeField]
        private EditorTestInfo m_Test;

        public string GetResultText()
        {
            StringBuilder builder = new StringBuilder(this.name);
            if (this.executed)
            {
                builder.AppendFormat(" [{0}s]", this.duration.ToString("##0.###"));
            }
            builder.AppendLine();
            if ((this.m_Test != null) && !string.IsNullOrEmpty(this.m_Test.testMethod.Description))
            {
                builder.AppendFormat("({0})\n", this.m_Test.testMethod.Description.Trim());
            }
            if (!string.IsNullOrEmpty(this.message))
            {
                builder.AppendFormat("---\n{0}\n", this.message.Trim());
            }
            if (!string.IsNullOrEmpty(this.logs))
            {
                builder.AppendFormat("---\n{0}\n", this.logs.Trim());
            }
            if (!this.isSuccess && !string.IsNullOrEmpty(this.stackTrace))
            {
                string str = StackTraceFilter.Filter(this.stackTrace).Trim();
                builder.AppendFormat("---\n{0}\n", str);
            }
            if (builder.Length > 0x3a98)
            {
                builder.Length = 0x3a98;
                builder.AppendFormat("...\n\n---MESSAGE TRUNCATED AT {0} CHARACTERS---", 0x3a98);
            }
            return builder.ToString().Trim();
        }

        public bool IsVisible(FilteringOptions options)
        {
            if (((options.categories != null) && (options.categories.Length > 0)) && !Enumerable.Any<string>(options.categories, (Func<string, bool>) (c => this.test.categories.Contains<string>(c))))
            {
                return false;
            }
            if (!options.showIgnored && ((this.m_Test.testMethod.RunState == RunState.Ignored) || (this.executed && (this.test.testMethod.RunState == RunState.Skipped))))
            {
                return false;
            }
            if ((!options.showFailed && this.executed) && ((this.isFailure || this.isError) || this.isInconclusive))
            {
                return false;
            }
            if ((!options.showNotRunned && !this.executed) && !this.isIgnored)
            {
                return false;
            }
            if (!options.showSucceeded && this.isSuccess)
            {
                return false;
            }
            return true;
        }

        public void SetTestInfo(EditorTestInfo editorTestInfo)
        {
            this.m_Test = editorTestInfo;
        }

        public void SetTestMethod(TestMethod testMethod)
        {
            this.m_Test.testMethod = testMethod;
        }

        public void Update(ITestResult source, bool outdated)
        {
            this.m_ResultState = source.resultState;
            this.m_Duration = source.duration;
            this.m_Message = source.message;
            this.m_Logs = source.logs;
            this.m_StackTrace = source.stackTrace;
            this.m_Executed = source.executed;
            this.m_IsIgnored = source.isIgnored || ((this.test != null) && this.test.isIgnored);
            this.m_Outdated = outdated;
            if (base.m_ParentGroup != null)
            {
                base.m_ParentGroup.InvalidateGroup(new TestResultState?(this.m_ResultState));
            }
            if (base.m_OnResultUpdate != null)
            {
                base.m_OnResultUpdate(this);
            }
        }

        public double duration
        {
            get => 
                this.m_Duration;
            set
            {
                this.m_Duration = value;
            }
        }

        public bool executed
        {
            get => 
                this.m_Executed;
            set
            {
                this.m_Executed = value;
            }
        }

        public string fullName =>
            this.test.fullName;

        public string id =>
            this.test.id;

        public bool isError =>
            (this.resultState == TestResultState.Error);

        public bool isFailure =>
            (this.resultState == TestResultState.Failure);

        public bool isIgnored
        {
            get => 
                this.m_IsIgnored;
            set
            {
                this.m_IsIgnored = value;
            }
        }

        public bool isInconclusive =>
            (this.resultState == TestResultState.Inconclusive);

        public bool isSuccess =>
            (this.resultState == TestResultState.Success);

        public string logs
        {
            get => 
                this.m_Logs;
            set
            {
                this.m_Logs = value;
            }
        }

        public string message
        {
            get => 
                this.m_Message;
            set
            {
                this.m_Message = value;
            }
        }

        public string name =>
            this.test.methodName;

        public bool outdated
        {
            get => 
                this.m_Outdated;
            set
            {
                this.m_Outdated = value;
            }
        }

        public TestResultState resultState
        {
            get => 
                this.m_ResultState;
            set
            {
                this.m_ResultState = value;
            }
        }

        public string stackTrace
        {
            get => 
                this.m_StackTrace;
            set
            {
                this.m_StackTrace = value;
            }
        }

        public EditorTestInfo test
        {
            get => 
                this.m_Test;
            set
            {
                this.m_Test = value;
            }
        }
    }
}

