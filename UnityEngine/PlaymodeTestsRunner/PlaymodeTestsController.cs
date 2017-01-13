namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;
    using UnityEngine.SceneManagement;

    [Serializable]
    internal class PlaymodeTestsController : MonoBehaviour
    {
        internal const string kPlaymodeTestControllerName = "Code-based tests runner";
        private TestExecutorBase m_TestExecutor;
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

        internal void FailPlaymodeTest()
        {
            this.m_TestExecutor.OnPlaymodeFailCall();
        }

        internal static PlaymodeTestsController GetController() => 
            GameObject.Find("Code-based tests runner").GetComponent<PlaymodeTestsController>();

        private void HandleLogCallback(string logString, string stackTrace, LogType type)
        {
            if (this.m_TestExecutor != null)
            {
                this.m_TestExecutor.OnLogCallback(logString, stackTrace, type);
            }
        }

        internal static bool IsControllerOnScene() => 
            (GameObject.Find("Code-based tests runner") != null);

        internal void PassPlaymodeTest()
        {
            this.m_TestExecutor.OnPlaymodePassCall();
        }

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
            internal IEnumerator<TestExecutorBase> $locvar0;
            internal int $PC;
            internal PlaymodeTestsController $this;
            private static Func<TestListElement, TestExecutorBase> <>f__am$cache0;
            internal TestResult <result>__3;
            internal TestExecutorBase <t>__2;
            internal TestListUtil <testListUtil>__0;
            internal IEnumerable<TestExecutorBase> <tests>__1;

            private static TestExecutorBase <>m__0(TestListElement t) => 
                t.testExecutor;

            internal bool <>m__1(TestExecutorBase e) => 
                this.$this.settings.filter.Matches(e.name);

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
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
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
                        Application.logMessageReceived += new Application.LogCallback(this.$this.HandleLogCallback);
                        this.$this.runStartedEvent.Invoke(Application.platform.ToString(), null);
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
                        goto Label_0229;

                    case 1:
                        break;

                    case 2:
                        goto Label_013B;

                    default:
                        goto Label_0227;
                }
                this.<testListUtil>__0 = new TestListUtil(false);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<TestListElement, TestExecutorBase>(PlaymodeTestsController.<Run>c__Iterator1.<>m__0);
                }
                this.<tests>__1 = Enumerable.Select<TestListElement, TestExecutorBase>(this.<testListUtil>__0.GetPlaymodeTests(this.$this.settings.filter).GetFlattenedHierarchy(), <>f__am$cache0);
                if (this.$this.settings.filter != null)
                {
                    this.<tests>__1 = Enumerable.Where<TestExecutorBase>(this.<tests>__1, new Func<TestExecutorBase, bool>(this.<>m__1)).ToList<TestExecutorBase>();
                }
                this.$locvar0 = this.<tests>__1.GetEnumerator();
                num = 0xfffffffd;
            Label_013B:
                try
                {
                    switch (num)
                    {
                        case 2:
                            this.<result>__3 = this.<t>__2.GetResult();
                            this.$this.testFinishedEvent.Invoke(this.<result>__3);
                            break;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<t>__2 = this.$locvar0.Current;
                        this.$this.m_TestExecutor = this.<t>__2;
                        this.$this.testStartedEvent.Invoke(this.<t>__2.name);
                        this.$current = this.<t>__2.Execute(this.$this);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_0229;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$this.runFinishedEvent.Invoke(null);
                this.$PC = -1;
            Label_0227:
                return false;
            Label_0229:
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

