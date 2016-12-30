namespace UnityEngine.TestTools.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.TestTools.Logging;

    internal class CoroutineRunner
    {
        private const int k_DefaultTimeout = 0x7530;
        private MonoBehaviour m_Controller;
        private LogScope m_LogCollector;
        private bool m_Running;
        private bool m_TestFailed;

        public CoroutineRunner(MonoBehaviour playmodeTestsController, LogScope logCollector)
        {
            this.m_Controller = playmodeTestsController;
            this.m_LogCollector = logCollector;
        }

        internal void CheckLog()
        {
            this.m_TestFailed = this.m_LogCollector.AnyFailingLogs();
        }

        [DebuggerHidden]
        private IEnumerator ExMethod(IEnumerator e, int timeout) => 
            new <ExMethod>c__Iterator1 { 
                e = e,
                timeout = timeout,
                $this = this
            };

        [DebuggerHidden]
        public IEnumerator HandleEnumerableTest(IEnumerator testEnumerator, int timeout) => 
            new <HandleEnumerableTest>c__Iterator0 { 
                timeout = timeout,
                testEnumerator = testEnumerator,
                $this = this
            };

        [DebuggerHidden]
        protected virtual IEnumerator StartTimer(IEnumerator coroutineToBeKilled, int timeout, Action onTimeout) => 
            new <StartTimer>c__Iterator2 { 
                timeout = timeout,
                coroutineToBeKilled = coroutineToBeKilled,
                onTimeout = onTimeout,
                $this = this
            };

        [CompilerGenerated]
        private sealed class <ExMethod>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal CoroutineRunner $this;
            internal Coroutine <timer>__1;
            internal IEnumerator e;
            internal int timeout;

            internal void <>m__0()
            {
                this.$this.m_TestFailed = true;
                this.$this.m_Running = false;
            }

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
                        this.<timer>__1 = this.$this.m_Controller.StartCoroutine(this.$this.StartTimer(this.e, this.timeout, new Action(this.<>m__0)));
                        this.$current = this.$this.m_Controller.StartCoroutine(this.e);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        this.$this.m_Controller.StopCoroutine(this.<timer>__1);
                        this.$this.m_Running = false;
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

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <HandleEnumerableTest>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal CoroutineRunner $this;
            internal IEnumerator testEnumerator;
            internal int timeout;

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
                        if (this.timeout == 0)
                        {
                            this.timeout = 0x7530;
                        }
                        break;

                    case 1:
                        if (this.$this.m_Running)
                        {
                            break;
                        }
                        this.$PC = -1;
                        goto Label_00EB;

                    default:
                        goto Label_00EB;
                }
                if (!this.$this.m_Running)
                {
                    this.$this.m_Running = true;
                    this.$this.m_Controller.StartCoroutine(this.$this.ExMethod(this.testEnumerator, this.timeout));
                }
                this.$this.CheckLog();
                if (this.$this.m_TestFailed)
                {
                    this.$this.m_Controller.StopCoroutine(this.testEnumerator);
                }
                else
                {
                    this.$current = null;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    return true;
                }
            Label_00EB:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <StartTimer>c__Iterator2 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal CoroutineRunner $this;
            internal IEnumerator coroutineToBeKilled;
            internal Action onTimeout;
            internal int timeout;

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
                        this.$current = new WaitForSeconds(((float) this.timeout) / 1000f);
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
                            this.onTimeout();
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

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

