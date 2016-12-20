namespace UnityEditor.PlaymodeTestsRunner.TestLauncher
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.PlaymodeTestsRunner;

    internal class EditModeLauncher : UnityEditor.PlaymodeTestsRunner.TestLauncher.TestLauncher
    {
        [CompilerGenerated]
        private static Func<TestListElement, TestExecutorBase> <>f__am$cache0;
        private TestExecutorBase m_CurrentTest;
        private IEnumerator m_CurrentTestEnumerator;
        private TestRunnerFilter m_Filter;
        private RunFinishedEvent m_RunFinishedEvent = new RunFinishedEvent();
        private RunStartedEvent m_RunStartedEvent = new RunStartedEvent();
        private TestFinishedEvent m_TestFinishedEvent = new TestFinishedEvent();
        private TestStartedEvent m_TestStartedEvent = new TestStartedEvent();
        private List<TestExecutorBase> m_TestsToRun;
        private bool started;

        public EditModeLauncher(TestRunnerFilter filter)
        {
            this.m_Filter = filter;
        }

        public T AddEventHandler<T>() where T: ScriptableObject, TestRunnerListener
        {
            T local = ScriptableObject.CreateInstance<T>();
            T local1 = local;
            this.m_TestStartedEvent.AddListener(new UnityAction<string>(local1.TestStarted));
            T local3 = local;
            this.m_TestFinishedEvent.AddListener(new UnityAction<TestResult>(local3.TestFinished));
            T local4 = local;
            this.m_RunStartedEvent.AddListener(new UnityAction<string, List<string>>(local4.RunStarted));
            T local5 = local;
            this.m_RunFinishedEvent.AddListener(new UnityAction<List<TestResult>>(local5.RunFinished));
            return local;
        }

        public override void Run()
        {
            TestListUtil util = new TestListUtil(true);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TestListElement, TestExecutorBase>(null, (IntPtr) <Run>m__0);
            }
            this.m_TestsToRun = Enumerable.ToList<TestExecutorBase>(Enumerable.Select<TestListElement, TestExecutorBase>(util.GetEditmodeTests(this.m_Filter).GetFlattenedHierarchy(), <>f__am$cache0));
            if (this.m_Filter != null)
            {
                this.m_TestsToRun = Enumerable.ToList<TestExecutorBase>(Enumerable.Where<TestExecutorBase>(this.m_TestsToRun, new Func<TestExecutorBase, bool>(this, (IntPtr) this.<Run>m__1)));
            }
            this.AddEventHandler<EditModeRunnerCallback>();
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateCallback));
        }

        public void UpdateCallback()
        {
            if (!this.started)
            {
                this.started = !this.started;
                this.m_RunStartedEvent.Invoke(null, null);
            }
            else if ((this.m_CurrentTestEnumerator == null) && ((this.m_TestsToRun == null) || !Enumerable.Any<TestExecutorBase>(this.m_TestsToRun)))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateCallback));
                this.m_RunFinishedEvent.Invoke(null);
            }
            else
            {
                if (this.m_CurrentTest == null)
                {
                    this.m_CurrentTest = Enumerable.First<TestExecutorBase>(this.m_TestsToRun);
                    Application.logMessageReceived += new Application.LogCallback(this.m_CurrentTest.OnLogCallback);
                    this.m_CurrentTestEnumerator = this.m_CurrentTest.Execute(null);
                    this.m_TestsToRun.Remove(this.m_CurrentTest);
                    this.m_TestStartedEvent.Invoke(this.m_CurrentTest.name);
                }
                if (!this.m_CurrentTestEnumerator.MoveNext())
                {
                    this.m_TestFinishedEvent.Invoke(this.m_CurrentTest.GetResult());
                    Application.logMessageReceived -= new Application.LogCallback(this.m_CurrentTest.OnLogCallback);
                    this.m_CurrentTest = null;
                    this.m_CurrentTestEnumerator = null;
                }
                else if (this.m_CurrentTestEnumerator.Current != null)
                {
                    Debug.Log("EditMode test can only yield null");
                }
            }
        }
    }
}

