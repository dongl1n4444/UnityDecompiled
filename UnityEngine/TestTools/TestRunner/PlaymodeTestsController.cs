namespace UnityEngine.TestTools.TestRunner
{
    using NUnit.Framework.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.Logging;
    using UnityEngine.TestTools.NUnitExtensions;
    using UnityEngine.TestTools.Utils;

    [Serializable]
    internal class PlaymodeTestsController : MonoBehaviour
    {
        internal const string kPlaymodeTestControllerName = "Code-based tests runner";
        [SerializeField]
        internal RunFinishedEvent runFinishedEvent = new RunFinishedEvent();
        [SerializeField]
        internal RunStartedEvent runStartedEvent = new RunStartedEvent();
        [SerializeField]
        public PlaymodeTestsControllerSettings settings = new PlaymodeTestsControllerSettings();
        [SerializeField]
        internal TestFinishedEvent testFinishedEvent = new TestFinishedEvent();
        [SerializeField]
        internal TestStartedEvent testStartedEvent = new TestStartedEvent();

        internal static PlaymodeTestsController GetController() => 
            GameObject.Find("Code-based tests runner").GetComponent<PlaymodeTestsController>();

        internal static bool IsControllerOnScene() => 
            (GameObject.Find("Code-based tests runner") != null);

        [DebuggerHidden]
        public IEnumerator Run() => 
            new <Run>c__Iterator1 { $this = this };

        [DebuggerHidden]
        public IEnumerator Start() => 
            new <Start>c__Iterator0 { $this = this };

        [CompilerGenerated]
        private sealed class <Run>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal PlaymodeTestsController $this;
            internal CoroutineRunner <cr>__3;
            internal LogScope <logCollector>__2;
            internal UnityTestAssemblyRunner <runner>__1;
            internal IEnumerator <testEnum>__3;
            internal TestListUtil <testListUtil>__1;

            private void <>__Finally0()
            {
                if (this.<logCollector>__2 != null)
                {
                    this.<logCollector>__2.Dispose();
                }
            }

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 3:
                    case 4:
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
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if (!this.$this.settings.sceneBased)
                        {
                            break;
                        }
                        SceneManager.LoadScene(1, LoadSceneMode.Additive);
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_02F1;

                    case 1:
                        break;

                    case 2:
                        goto Label_02BD;

                    case 3:
                    case 4:
                        goto Label_019A;

                    default:
                        goto Label_02EF;
                }
                this.<testListUtil>__1 = new TestListUtil();
                this.<runner>__1 = new UnityTestAssemblyRunner(this.<testListUtil>__1.GetNUnitTestBuilder(TestPlatform.PlayMode));
                Reflect.MethodCallWrapper = new Func<Func<object>, object>(ActionDelegator.instance.Delegate);
                this.<runner>__1.Load(this.<testListUtil>__1.GetUserAssemblies(false).ToArray<Assembly>(), this.<testListUtil>__1.GetNUnitTestBuilderSettings(TestPlatform.PlayMode));
                this.$this.runStartedEvent.Invoke(this.<runner>__1.LoadedTest);
                this.<runner>__1.RunAsync(new TestListenerWrapper(this.$this.testStartedEvent, this.$this.testFinishedEvent), this.$this.settings.filter.BuildNUnitFilter());
            Label_02BD:
                while (!this.<runner>__1.IsTestComplete)
                {
                    if (ActionDelegator.instance.HasAction())
                    {
                        using (LogScope scope = new LogScope())
                        {
                            ActionDelegator.instance.Execute(scope);
                        }
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_02F1;
                    }
                    if (!TestDelegator.instance.HasTest())
                    {
                        goto Label_02BC;
                    }
                    this.<logCollector>__2 = new LogScope();
                    num = 0xfffffffd;
                Label_019A:
                    try
                    {
                        switch (num)
                        {
                            case 3:
                            {
                                if (!this.<logCollector>__2.AnyFailingLogs())
                                {
                                    break;
                                }
                                LogEvent log = this.<logCollector>__2.FailingLogs.First<LogEvent>();
                                TestDelegator.instance.RegisterResultException(new UnhandledLogMessageException(log));
                                goto Label_028D;
                            }
                            case 4:
                                goto Label_02BC;

                            default:
                                this.<testEnum>__3 = TestDelegator.instance.GetTestEnumerator();
                                this.<cr>__3 = new CoroutineRunner(this.$this, this.<logCollector>__2);
                                this.$current = this.<cr>__3.HandleEnumerableTest(this.<testEnum>__3, TestDelegator.instance.GetCurrentTestContext().TestCaseTimeout);
                                if (!this.$disposing)
                                {
                                    this.$PC = 3;
                                }
                                flag = true;
                                goto Label_02F1;
                        }
                        if (this.<logCollector>__2.ExpectedLogs.Any<LogMatch>())
                        {
                            TestDelegator.instance.RegisterResultException(new UnexpectedLogMessageException(LogScope.Current.ExpectedLogs.Peek()));
                        }
                        else
                        {
                            TestDelegator.instance.RegisterResult(null);
                        }
                    Label_028D:
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        flag = true;
                        goto Label_02F1;
                    }
                    finally
                    {
                        if (!flag)
                        {
                        }
                        this.<>__Finally0();
                    }
                Label_02BC:;
                }
                this.$this.runFinishedEvent.Invoke(this.<runner>__1.Result);
                this.$PC = -1;
            Label_02EF:
                return false;
            Label_02F1:
                return true;
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
        private sealed class <Start>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal PlaymodeTestsController $this;

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
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_007C;

                    case 1:
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_007C;

                    case 2:
                        this.$this.StartCoroutine(this.$this.Run());
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_007C:
                return true;
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

