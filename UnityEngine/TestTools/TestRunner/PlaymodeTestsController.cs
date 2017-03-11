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

    [Serializable, AddComponentMenu("")]
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
            internal CoroutineRunner <cr>__2;
            internal bool <enableUnityTest>__0;
            internal LogScope <logCollector>__1;
            internal UnityTestAssemblyRunner <runner>__0;
            internal IEnumerator <testEnum>__2;
            internal TestAssemblyProvider <testListUtil>__0;

            private void <>__Finally0()
            {
                if (this.<logCollector>__1 != null)
                {
                    this.<logCollector>__1.Dispose();
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
                        goto Label_0337;

                    case 1:
                        break;

                    case 2:
                        goto Label_02F8;

                    case 3:
                    case 4:
                        goto Label_01AA;

                    default:
                        goto Label_0335;
                }
                this.<testListUtil>__0 = new TestAssemblyProvider();
                this.<runner>__0 = new UnityTestAssemblyRunner(UnityTestAssemblyBuilder.GetNUnitTestBuilder(TestPlatform.PlayMode));
                this.<enableUnityTest>__0 = UnityTestAttribute.IsSupportedOnPlatform();
                if (this.<enableUnityTest>__0)
                {
                    Reflect.MethodCallWrapper = new Func<Func<object>, object>(ActionDelegator.instance.Delegate);
                }
                this.<runner>__0.Load(this.<testListUtil>__0.GetUserAssemblies(false).ToArray<Assembly>(), UnityTestAssemblyBuilder.GetNUnitTestBuilderSettings(TestPlatform.PlayMode));
                this.$this.runStartedEvent.Invoke(this.<runner>__0.LoadedTest);
                this.<runner>__0.RunAsync(new TestListenerWrapper(this.$this.testStartedEvent, this.$this.testFinishedEvent, this.<enableUnityTest>__0), this.$this.settings.filter.BuildNUnitFilter());
            Label_02F8:
                while (this.<enableUnityTest>__0 && !this.<runner>__0.IsTestComplete)
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
                        goto Label_0337;
                    }
                    if (!TestDelegator.instance.HasTest())
                    {
                        goto Label_02F7;
                    }
                    this.<logCollector>__1 = new LogScope();
                    num = 0xfffffffd;
                Label_01AA:
                    try
                    {
                        switch (num)
                        {
                            case 3:
                                break;

                            case 4:
                                goto Label_02F7;

                            default:
                                this.<testEnum>__2 = null;
                                try
                                {
                                    this.<testEnum>__2 = TestDelegator.instance.GetTestEnumerator();
                                }
                                catch (Exception exception)
                                {
                                    UnityEngine.Debug.LogException(exception);
                                }
                                this.<cr>__2 = new CoroutineRunner(this.$this, this.<logCollector>__1);
                                if (this.<testEnum>__2 != null)
                                {
                                    this.$current = this.<cr>__2.HandleEnumerableTest(this.<testEnum>__2, TestDelegator.instance.GetCurrentTestContext().TestCaseTimeout);
                                    if (!this.$disposing)
                                    {
                                        this.$PC = 3;
                                    }
                                    flag = true;
                                    goto Label_0337;
                                }
                                break;
                        }
                        if (this.<logCollector>__1.AnyFailingLogs())
                        {
                            LogEvent log = this.<logCollector>__1.FailingLogs.First<LogEvent>();
                            TestDelegator.instance.RegisterResultException(new UnhandledLogMessageException(log));
                        }
                        else if (this.<logCollector>__1.ExpectedLogs.Any<LogMatch>())
                        {
                            TestDelegator.instance.RegisterResultException(new UnexpectedLogMessageException(LogScope.Current.ExpectedLogs.Peek()));
                        }
                        else
                        {
                            TestDelegator.instance.RegisterResult(null);
                        }
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        flag = true;
                        goto Label_0337;
                    }
                    finally
                    {
                        if (!flag)
                        {
                        }
                        this.<>__Finally0();
                    }
                Label_02F7:;
                }
                this.$this.runFinishedEvent.Invoke(this.<runner>__0.Result);
                this.$PC = -1;
            Label_0335:
                return false;
            Label_0337:
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

