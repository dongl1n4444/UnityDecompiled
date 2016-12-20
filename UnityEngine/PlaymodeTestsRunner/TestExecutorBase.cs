namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    internal class TestExecutorBase
    {
        public string IdName;
        private bool m_AssertionHappened;
        protected PlaymodeTestsController m_Controller;
        protected StringBuilder m_CurrentTestMessageBuilder = new StringBuilder();
        protected StringBuilder m_CurrentTestStacktraceBuilder = new StringBuilder();
        protected float m_Duration;
        private bool m_ExceptionHappened;
        protected bool m_MBTestFailed;
        protected bool m_MBTestFinished;
        protected internal MethodInfo m_MethodInfo;
        internal bool m_TestFailed;
        protected internal System.Type m_Type;
        public string name;

        public TestExecutorBase(string name)
        {
            this.name = name;
            this.IdName = name;
        }

        internal void CheckLog()
        {
            if (this.m_ExceptionHappened || this.m_AssertionHappened)
            {
                this.m_TestFailed = true;
            }
            if (this.m_MBTestFailed)
            {
                this.m_TestFailed = true;
            }
        }

        internal virtual IEnumerator Execute(PlaymodeTestsController controller)
        {
            throw new NotImplementedException();
        }

        internal void FailPlaymodeTest()
        {
            this.m_MBTestFinished = true;
            this.m_MBTestFailed = true;
        }

        public virtual TestResult GetResult()
        {
            throw new NotImplementedException();
        }

        public bool IsIncluded(TestRunnerFilter filter)
        {
            return ((filter == null) || filter.Matches(this.name));
        }

        protected void KillGOTest(GameObject go)
        {
            if (go != null)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
            this.FailPlaymodeTest();
        }

        public void OnLogCallback(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    this.m_ExceptionHappened = true;
                    this.m_CurrentTestMessageBuilder.AppendLine(logString);
                    this.m_CurrentTestStacktraceBuilder.AppendLine(stackTrace);
                    break;

                case LogType.Assert:
                    this.m_AssertionHappened = true;
                    this.m_CurrentTestMessageBuilder.AppendLine(logString);
                    this.m_CurrentTestStacktraceBuilder.AppendLine(stackTrace);
                    break;

                case LogType.Log:
                    this.m_CurrentTestMessageBuilder.AppendLine(logString);
                    break;

                case LogType.Exception:
                    this.m_ExceptionHappened = true;
                    this.m_CurrentTestMessageBuilder.AppendLine(logString);
                    this.m_CurrentTestStacktraceBuilder.AppendLine(stackTrace);
                    break;
            }
        }

        public virtual void OnPlaymodeFailCall()
        {
            this.m_MBTestFinished = true;
            this.m_MBTestFailed = true;
        }

        public virtual void OnPlaymodePassCall()
        {
            this.m_MBTestFinished = true;
            this.m_MBTestFailed = false;
        }

        [DebuggerHidden]
        protected virtual IEnumerator StartTimer(IEnumerator coroutineToBeKilled, float timeout, Action onTimeout)
        {
            return new <StartTimer>c__Iterator0 { 
                timeout = timeout,
                coroutineToBeKilled = coroutineToBeKilled,
                onTimeout = onTimeout,
                $this = this
            };
        }

        [CompilerGenerated]
        private sealed class <StartTimer>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal TestExecutorBase $this;
            internal IEnumerator coroutineToBeKilled;
            internal Action onTimeout;
            internal float timeout;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.timeout);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        if (this.coroutineToBeKilled != null)
                        {
                            this.$this.m_Controller.StopCoroutine(this.coroutineToBeKilled);
                        }
                        if (this.onTimeout != null)
                        {
                            this.onTimeout.Invoke();
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

