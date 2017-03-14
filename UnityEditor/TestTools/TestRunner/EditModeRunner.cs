namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.TestTools.Logging;
    using UnityEngine.TestTools.NUnitExtensions;
    using UnityEngine.TestTools.TestRunner;

    internal class EditModeRunner : IDisposable
    {
        private List<ScriptableObject> m_CallbackObjects = new List<ScriptableObject>();
        private IEnumerator m_CurrentTest;
        private LogScope m_LogCollector;
        private RunFinishedEvent m_RunFinishedEvent = new RunFinishedEvent();
        private UnityTestAssemblyRunner m_Runner;
        private RunStartedEvent m_RunStartedEvent = new RunStartedEvent();
        private TestFinishedEvent m_TestFinishedEvent = new TestFinishedEvent();
        private TestStartedEvent m_TestStartedEvent = new TestStartedEvent();
        public static bool RunningTests;

        public EditModeRunner(UnityTestAssemblyRunner runner)
        {
            this.m_Runner = runner;
        }

        public T AddEventHandler<T>() where T: ScriptableObject, TestRunnerListener
        {
            T item = ScriptableObject.CreateInstance<T>();
            item.hideFlags |= HideFlags.DontSave;
            this.m_CallbackObjects.Add(item);
            T local1 = item;
            this.m_TestStartedEvent.AddListener(new UnityAction<ITest>(local1.TestStarted));
            T local3 = item;
            this.m_TestFinishedEvent.AddListener(new UnityAction<ITestResult>(local3.TestFinished));
            T local4 = item;
            this.m_RunStartedEvent.AddListener(new UnityAction<ITest>(local4.RunStarted));
            T local5 = item;
            this.m_RunFinishedEvent.AddListener(new UnityAction<ITestResult>(local5.RunFinished));
            return item;
        }

        public void Dispose()
        {
            Reflect.MethodCallWrapper = null;
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.TestConsumer));
            if (this.m_CallbackObjects != null)
            {
                foreach (ScriptableObject obj2 in this.m_CallbackObjects)
                {
                    UnityEngine.Object.DestroyImmediate(obj2);
                }
                this.m_CallbackObjects.Clear();
            }
            RunningTests = false;
        }

        private void InvokeDelegator()
        {
            TestDelegator instance = TestDelegator.instance;
            if (instance.HasTest())
            {
                bool flag = false;
                try
                {
                    if (this.m_CurrentTest == null)
                    {
                        this.m_LogCollector = new LogScope();
                        this.m_CurrentTest = instance.GetTestEnumerator();
                        if (this.m_CurrentTest == null)
                        {
                            flag = true;
                        }
                    }
                    if (this.m_CurrentTest != null)
                    {
                        flag = !this.m_CurrentTest.MoveNext();
                    }
                    if (this.m_LogCollector.AnyFailingLogs())
                    {
                        throw new UnhandledLogMessageException(this.m_LogCollector.FailingLogs.First<LogEvent>());
                    }
                    if (flag && this.m_LogCollector.ExpectedLogs.Any<LogMatch>())
                    {
                        throw new UnexpectedLogMessageException(LogScope.Current.ExpectedLogs.Peek());
                    }
                }
                catch (TargetInvocationException exception)
                {
                    this.OnTestFinished(exception.InnerException);
                    return;
                }
                catch (Exception exception2)
                {
                    this.OnTestFinished(exception2);
                    return;
                }
                if (flag)
                {
                    this.OnTestFinished(null);
                }
                else if (this.m_CurrentTest.Current != null)
                {
                    Debug.LogError("EditMode test can only yield null");
                }
            }
        }

        private void OnTestFinished(Exception exception)
        {
            this.m_CurrentTest = null;
            if (exception == null)
            {
                TestDelegator.instance.RegisterResult(null);
            }
            else
            {
                TestDelegator.instance.RegisterResultException(exception);
            }
            this.m_LogCollector.Dispose();
            this.m_LogCollector = null;
        }

        public void Run(ITestFilter filter)
        {
            this.m_RunStartedEvent.Invoke(this.m_Runner.LoadedTest);
            Reflect.MethodCallWrapper = new Func<Func<object>, object>(ActionDelegator.instance.Delegate);
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.TestConsumer));
            if (RunningTests)
            {
                throw new Exception("Tests are currently running");
            }
            RunningTests = true;
            this.m_Runner.RunAsync(new TestListenerWrapper(this.m_TestStartedEvent, this.m_TestFinishedEvent, true), filter);
        }

        private void TestConsumer()
        {
            if (this.m_Runner.IsTestComplete)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.TestConsumer));
                this.m_RunFinishedEvent.Invoke(this.m_Runner.Result);
                this.Dispose();
            }
            if (ActionDelegator.instance.HasAction())
            {
                using (LogScope scope = new LogScope())
                {
                    ActionDelegator.instance.Execute(scope);
                }
            }
            else
            {
                this.InvokeDelegator();
            }
        }
    }
}

