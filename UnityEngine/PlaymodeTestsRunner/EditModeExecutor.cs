namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class EditModeExecutor : TestExecutorBase
    {
        private object m_Instance;

        public EditModeExecutor(System.Type type, MethodInfo methodInfo) : base(type.FullName + "+" + methodInfo.Name)
        {
            base.m_Type = type;
            base.m_MethodInfo = methodInfo;
        }

        internal override IEnumerator Execute(PlaymodeTestsController controller)
        {
            base.m_Controller = controller;
            this.m_Instance = this.GetInstanceOfTestObject(base.m_MethodInfo);
            DateTime now = DateTime.Now;
            if (base.m_MethodInfo.ReturnType == typeof(void))
            {
                return this.HandleVoidTest(this.m_Instance, base.m_MethodInfo);
            }
            if (base.m_MethodInfo.ReturnType == typeof(IEnumerator))
            {
                return this.HandleEnumerableTest(this.m_Instance, base.m_MethodInfo);
            }
            Debug.Log(string.Concat(new object[] { "Return type ", base.m_MethodInfo.ReturnType, " of ", base.m_MethodInfo.Name, " in ", base.m_Type.FullName, " is not supported." }));
            TimeSpan span = (TimeSpan) (DateTime.Now - now);
            base.m_Duration = (float) span.TotalSeconds;
            return null;
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
                name = base.name,
                fullName = base.name,
                resultType = !base.m_TestFailed ? TestResult.ResultType.Success : TestResult.ResultType.Failed,
                messages = base.m_CurrentTestMessageBuilder.ToString(),
                stacktrace = base.m_CurrentTestStacktraceBuilder.ToString(),
                duration = base.m_Duration
            };
        }

        private IEnumerator HandleEnumerableTest(object instance, MethodInfo m) => 
            (m.Invoke(instance, null) as IEnumerator);

        [DebuggerHidden]
        private IEnumerator HandleVoidTest(object instance, MethodInfo m) => 
            new <HandleVoidTest>c__Iterator0 { 
                m = m,
                instance = instance,
                $this = this
            };

        [CompilerGenerated]
        private sealed class <HandleVoidTest>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal EditModeExecutor $this;
            internal object instance;
            internal MethodInfo m;

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
                        try
                        {
                            this.m.Invoke(this.instance, null);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception);
                        }
                        this.$this.CheckLog();
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
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

