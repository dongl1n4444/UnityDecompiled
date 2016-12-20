namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.PlaymodeTests;

    internal class MethodExecutor : TestExecutorBase
    {
        private object m_Instance;
        private bool m_IsFrameFinished;
        private bool m_Running;

        public MethodExecutor(System.Type type, MethodInfo methodInfo) : base(type.FullName + "+" + methodInfo.Name)
        {
            base.m_Type = type;
            base.m_MethodInfo = methodInfo;
        }

        [DebuggerHidden]
        internal override IEnumerator Execute(PlaymodeTestsController controller)
        {
            return new <Execute>c__Iterator0 { 
                controller = controller,
                $this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ExMethod(IEnumerator e, float timeout)
        {
            return new <ExMethod>c__Iterator2 { 
                e = e,
                timeout = timeout,
                $this = this
            };
        }

        private object GetInstanceOfTestObject(MethodInfo m)
        {
            if (m.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                return base.m_Controller.gameObject.AddComponent(m.DeclaringType);
            }
            return Activator.CreateInstance(m.DeclaringType);
        }

        public override TestResult GetResult()
        {
            base.CheckLog();
            return new TestResult(base.IdName) { 
                resultType = !base.m_TestFailed ? TestResult.ResultType.Success : TestResult.ResultType.Failed,
                messages = base.m_CurrentTestMessageBuilder.ToString(),
                stacktrace = base.m_CurrentTestStacktraceBuilder.ToString(),
                duration = base.m_Duration
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleEnumerableTest(object instance, MethodInfo m, System.Type type)
        {
            return new <HandleEnumerableTest>c__Iterator1 { 
                type = type,
                m = m,
                instance = instance,
                $this = this
            };
        }

        private void HandleVoidTest(object instance, MethodInfo m)
        {
            try
            {
                m.Invoke(instance, null);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            base.CheckLog();
        }

        [CompilerGenerated]
        private sealed class <Execute>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal MethodExecutor $this;
            internal DateTime <start>__0;
            internal PlaymodeTestsController controller;

            private void <>__Finally0()
            {
                if (!this.$this.m_IsFrameFinished)
                {
                    Debug.Log("Test didn't finish OK. Exception?");
                }
                this.$this.m_IsFrameFinished = false;
            }

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<>__Finally0();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                TimeSpan span;
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$this.m_Controller = this.controller;
                        this.$this.m_Instance = this.$this.GetInstanceOfTestObject(this.$this.m_MethodInfo);
                        this.<start>__0 = DateTime.Now;
                        if (this.$this.m_MethodInfo.ReturnType != typeof(void))
                        {
                            if (this.$this.m_MethodInfo.ReturnType != typeof(IEnumerator))
                            {
                                Debug.Log(string.Concat(new object[] { "Return type ", this.$this.m_MethodInfo.ReturnType, " of ", this.$this.m_MethodInfo.Name, " in ", this.$this.m_Type.FullName, " is not supported." }));
                                goto Label_01C1;
                            }
                            num = 0xfffffffd;
                            break;
                        }
                        this.$this.HandleVoidTest(this.$this.m_Instance, this.$this.m_MethodInfo);
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_01EE;

                    case 1:
                        goto Label_01C1;

                    case 2:
                        break;

                    default:
                        goto Label_01EC;
                }
                try
                {
                    switch (num)
                    {
                        case 2:
                            goto Label_01C1;
                    }
                    this.$current = this.$this.HandleEnumerableTest(this.$this.m_Instance, this.$this.m_MethodInfo, this.$this.m_Type);
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    flag = true;
                    goto Label_01EE;
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.<>__Finally0();
                }
            Label_01C1:
                span = (TimeSpan) (DateTime.Now - this.<start>__0);
                this.$this.m_Duration = (float) span.TotalSeconds;
                this.$PC = -1;
            Label_01EC:
                return false;
            Label_01EE:
                return true;
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

        [CompilerGenerated]
        private sealed class <ExMethod>c__Iterator2 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal MethodExecutor $this;
            internal Coroutine <timer>__0;
            internal IEnumerator e;
            internal float timeout;

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
                        this.$this.m_Running = true;
                        this.<timer>__0 = this.$this.m_Controller.StartCoroutine(this.$this.StartTimer(this.e, this.timeout, new Action(this, (IntPtr) this.<>m__0)));
                        this.$current = this.$this.m_Controller.StartCoroutine(this.e);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_00E1;

                    case 1:
                        this.$this.m_Controller.StopCoroutine(this.<timer>__0);
                        this.$this.m_Running = false;
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_00E1;

                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00E1:
                return true;
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

        [CompilerGenerated]
        private sealed class <HandleEnumerableTest>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal MethodExecutor $this;
            internal float <suiteTimeout>__0;
            internal object instance;
            internal MethodInfo m;
            internal System.Type type;

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
                        this.<suiteTimeout>__0 = TestAttribute.GetAttribute(this.type).GetTimeout();
                        break;

                    case 1:
                        if (this.$this.m_Running)
                        {
                            break;
                        }
                        goto Label_0112;

                    default:
                        goto Label_0119;
                }
                this.$this.m_IsFrameFinished = false;
                if (!this.$this.m_Running)
                {
                    this.$this.m_Running = true;
                    float timeout = TestAttribute.GetAttribute(this.m).GetTimeout();
                    if (timeout == 5f)
                    {
                        timeout = this.<suiteTimeout>__0;
                    }
                    this.$this.m_Controller.StartCoroutine(this.$this.ExMethod(this.m.Invoke(this.instance, null) as IEnumerator, timeout));
                }
                this.$this.CheckLog();
                this.$this.m_IsFrameFinished = true;
                if (!this.$this.m_TestFailed)
                {
                    this.$current = null;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    return true;
                }
            Label_0112:
                this.$PC = -1;
            Label_0119:
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

