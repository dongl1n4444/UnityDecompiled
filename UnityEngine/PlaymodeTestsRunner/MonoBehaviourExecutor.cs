namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.PlaymodeTests;

    internal class MonoBehaviourExecutor : TestExecutorBase
    {
        public MonoBehaviourExecutor(System.Type type) : base(type.FullName)
        {
            base.m_Type = type;
        }

        [DebuggerHidden]
        internal override IEnumerator Execute(PlaymodeTestsController controller) => 
            new <Execute>c__Iterator0 { 
                controller = controller,
                $this = this
            };

        public override TestResult GetResult() => 
            new TestResult(base.name) { 
                resultType = !base.m_TestFailed ? TestResult.ResultType.Success : TestResult.ResultType.Failed,
                messages = base.m_CurrentTestMessageBuilder.ToString(),
                stacktrace = base.m_CurrentTestStacktraceBuilder.ToString(),
                duration = base.m_Duration
            };

        protected bool IsMBTestFinished()
        {
            base.CheckLog();
            return base.m_MBTestFinished;
        }

        [CompilerGenerated]
        private sealed class <Execute>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            private <Execute>c__AnonStorey1 $locvar0;
            internal int $PC;
            internal MonoBehaviourExecutor $this;
            internal PlayModeTestAttribute <attribute>__0;
            internal DateTime <start>__1;
            internal Coroutine <timer>__3;
            internal PlaymodeTestsController controller;

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
                        this.$locvar0 = new <Execute>c__AnonStorey1();
                        this.$locvar0.<>f__ref$0 = this;
                        this.$this.m_Controller = this.controller;
                        this.<attribute>__0 = TestAttribute.GetAttribute(this.$this.m_Type);
                        this.<start>__1 = DateTime.Now;
                        this.$locvar0.go = new GameObject("Test: " + this.$this.m_Type.Name);
                        this.$locvar0.go.AddComponent(this.$this.m_Type);
                        this.<timer>__3 = this.controller.StartCoroutine(this.$this.StartTimer(null, this.<attribute>__0.GetTimeout(), new Action(this.$locvar0.<>m__0)));
                        this.$current = new WaitUntil(new Func<bool>(this.$this.IsMBTestFinished));
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                    {
                        TimeSpan span = (TimeSpan) (DateTime.Now - this.<start>__1);
                        this.$this.m_Duration = (float) span.TotalSeconds;
                        UnityEngine.Object.DestroyImmediate(this.$locvar0.go);
                        this.controller.StopCoroutine(this.<timer>__3);
                        this.$PC = -1;
                        break;
                    }
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

            private sealed class <Execute>c__AnonStorey1
            {
                internal MonoBehaviourExecutor.<Execute>c__Iterator0 <>f__ref$0;
                internal GameObject go;

                internal void <>m__0()
                {
                    this.<>f__ref$0.$this.KillGOTest(this.go);
                }
            }
        }
    }
}

