namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.Logging;
    using UnityEngine.TestTools.NUnitExtensions;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;
    using UnityEngine.TestTools.Utils;

    internal class EditModeRunner : IDisposable
    {
        private List<ScriptableObject> m_CallbackObjects = new List<ScriptableObject>();
        private IEnumerator m_CurrentTest;
        private TestRunnerFilter m_Filter;
        private LogScope m_LogCollector;
        private RunFinishedEvent m_RunFinishedEvent = new RunFinishedEvent();
        private RunStartedEvent m_RunStartedEvent = new RunStartedEvent();
        private TestFinishedEvent m_TestFinishedEvent = new TestFinishedEvent();
        private TestStartedEvent m_TestStartedEvent = new TestStartedEvent();
        private UnityTestAssemblyRunner runner;

        public EditModeRunner(TestRunnerFilter filter)
        {
            this.m_Filter = filter;
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
            if (this.m_CallbackObjects != null)
            {
                foreach (ScriptableObject obj2 in this.m_CallbackObjects)
                {
                    UnityEngine.Object.DestroyImmediate(obj2);
                }
                this.m_CallbackObjects.Clear();
            }
        }

        private void InvokeDelegator()
        {
            TestDelegator instance = TestDelegator.instance;
            if (instance.HasTest())
            {
                if (this.m_CurrentTest == null)
                {
                    this.m_LogCollector = new LogScope();
                    this.m_CurrentTest = instance.GetTestEnumerator();
                }
                bool flag = false;
                try
                {
                    flag = !this.m_CurrentTest.MoveNext();
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
                    Debug.LogWarning("EditMode test can only yield null");
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

        public void Run()
        {
            TestListUtil util = new TestListUtil();
            this.runner = new UnityTestAssemblyRunner(util.GetNUnitTestBuilder(TestPlatform.EditMode));
            Reflect.MethodCallWrapper = new Func<Func<object>, object>(ActionDelegator.instance.Delegate);
            this.runner.Load(util.GetUserAssemblies(true).ToArray<Assembly>(), util.GetNUnitTestBuilderSettings(TestPlatform.EditMode));
            this.m_RunStartedEvent.Invoke(this.runner.LoadedTest);
            this.runner.RunAsync(new TestListenerWrapper(this.m_TestStartedEvent, this.m_TestFinishedEvent), this.m_Filter.BuildNUnitFilter());
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.TestConsumer));
        }

        private void TestConsumer()
        {
            if (this.runner.IsTestComplete)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.TestConsumer));
                this.m_RunFinishedEvent.Invoke(this.runner.Result);
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

