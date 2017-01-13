namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using NUnit.Core.Filters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEditor.EditorTests;
    using UnityEngine;

    internal class NUnitTestEngine
    {
        [CompilerGenerated]
        private static Func<Assembly, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<AssemblyName, bool> <>f__am$cache1;
        private TestSuite m_TestSuite;
        private static readonly string[] s_WhitelistedAssemblies = new string[] { "Assembly-CSharp-Editor", "Assembly-CSharp-Editor-firstpass", "Assembly-UnityScript-Editor", "Assembly-UnityScript-Editor-firstpass" };

        private void ExecuteTestSuite(TestSuite suite, ITestRunnerCallback testRunnerEventListener, TestRunnerFilter filter)
        {
            EventListener listener;
            if (testRunnerEventListener == null)
            {
                listener = new NullListener();
            }
            else
            {
                listener = new TestRunnerEventListener(testRunnerEventListener);
            }
            TestExecutionContext.get_CurrentContext().set_Out(new EventListenerTextWriter(listener, 0));
            TestExecutionContext.get_CurrentContext().set_Error(new EventListenerTextWriter(listener, 1));
            suite.Run(listener, this.GetFilter(filter));
        }

        private static Assembly[] GetAssembliesWithTests()
        {
            List<Assembly> list = new List<Assembly>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                <GetAssembliesWithTests>c__AnonStorey0 storey = new <GetAssembliesWithTests>c__AnonStorey0 {
                    assembly = assemblies[i]
                };
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = a => a.Name != "nunit.framework";
                }
                if (!Enumerable.All<AssemblyName>(storey.assembly.GetReferencedAssemblies(), <>f__am$cache1) && (storey.assembly.Location.Replace('\\', '/').StartsWith(Application.dataPath) || Enumerable.Any<string>(s_WhitelistedAssemblies, new Func<string, bool>(storey.<>m__0))))
                {
                    list.Add(storey.assembly);
                }
            }
            return list.ToArray();
        }

        private ITestFilter GetFilter(TestRunnerFilter filter)
        {
            AndFilter filter2 = new AndFilter();
            if ((filter.names != null) && (filter.names.Length > 0))
            {
                filter2.Add(new SimpleNameFilter(filter.names));
            }
            if ((filter.categories != null) && (filter.categories.Length > 0))
            {
                filter2.Add(new CategoryFilter(filter.categories));
            }
            if ((filter.testnames != null) && (filter.testnames.Length > 0))
            {
                NameFilter filter3 = new NameFilter();
                foreach (TestName name in filter.testnames)
                {
                    filter3.Add(name);
                }
                filter2.Add(filter3);
            }
            return filter2;
        }

        private TestSuite PrepareTestSuite(List<string> assemblyList)
        {
            CoreExtensions.get_Host().InitializeService();
            TestPackage package = new TestPackage(PlayerSettings.productName, assemblyList);
            TestSuiteBuilder builder = new TestSuiteBuilder();
            TestExecutionContext.get_CurrentContext().set_TestPackage(package);
            return builder.Build(package);
        }

        public void RunTests(ITestRunnerCallback testRunnerEventListener)
        {
            this.RunTests(TestRunnerFilter.empty, testRunnerEventListener);
        }

        public void RunTests(TestRunnerFilter filter, ITestRunnerCallback testRunnerEventListener)
        {
            try
            {
                if (testRunnerEventListener != null)
                {
                    testRunnerEventListener.RunStarted(this.m_TestSuite.get_TestName().get_FullName(), this.m_TestSuite.get_TestCount());
                }
                this.ExecuteTestSuite(this.m_TestSuite, testRunnerEventListener, filter);
                if (testRunnerEventListener != null)
                {
                    testRunnerEventListener.RunFinished();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (testRunnerEventListener != null)
                {
                    testRunnerEventListener.RunFinishedException(exception);
                }
            }
        }

        public TestSuite testSuite
        {
            get
            {
                if (this.m_TestSuite == null)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = a => a.Location;
                    }
                    List<string> assemblyList = Enumerable.Select<Assembly, string>(GetAssembliesWithTests(), <>f__am$cache0).ToList<string>();
                    TestSuite suite = this.PrepareTestSuite(assemblyList);
                    this.m_TestSuite = suite;
                }
                return this.m_TestSuite;
            }
        }

        [CompilerGenerated]
        private sealed class <GetAssembliesWithTests>c__AnonStorey0
        {
            internal Assembly assembly;

            internal bool <>m__0(string a) => 
                this.assembly.GetName().Name.StartsWith(a);
        }

        public class TestRunnerEventListener : EventListener
        {
            private string assertFromTheLogMessage;
            private string assertFromTheLogStacktrace;
            private string currentDir = Environment.CurrentDirectory;
            private StringBuilder m_TestLog;
            private readonly ITestRunnerCallback m_TestRunnerEventListener;

            public TestRunnerEventListener(ITestRunnerCallback testRunnerEventListener)
            {
                this.m_TestRunnerEventListener = testRunnerEventListener;
            }

            private void LogCallback(string message, string stacktrace, LogType type)
            {
                if (type == LogType.Assert)
                {
                    this.assertFromTheLogMessage = message;
                    this.assertFromTheLogStacktrace = stacktrace;
                }
            }

            public void RunFinished(TestResult result)
            {
                EditorApplication.UnlockReloadAssemblies();
                Application.logMessageReceived -= new UnityEngine.Application.LogCallback(this.LogCallback);
                this.m_TestRunnerEventListener.RunFinished();
            }

            public void RunFinished(Exception exception)
            {
                EditorApplication.UnlockReloadAssemblies();
                this.m_TestRunnerEventListener.RunFinishedException(exception);
            }

            public void RunStarted(string name, int testCount)
            {
                EditorApplication.LockReloadAssemblies();
                this.m_TestRunnerEventListener.RunStarted(name, testCount);
            }

            public void SuiteFinished(TestResult result)
            {
            }

            public void SuiteStarted(TestName testName)
            {
                Environment.CurrentDirectory = this.currentDir;
                Application.logMessageReceived += new UnityEngine.Application.LogCallback(this.LogCallback);
            }

            public void TestFinished(TestResult result)
            {
                EditorTestResult testResult = new EditorTestResult {
                    test = new EditorTestInfo(result.get_Test()),
                    executed = result.get_Executed(),
                    resultState = (TestResultState) result.get_ResultState(),
                    message = result.get_Message(),
                    logs = this.m_TestLog.ToString(),
                    stackTrace = result.get_StackTrace(),
                    duration = result.get_Time(),
                    isIgnored = (result.get_ResultState() == 3) || (result.get_Test().get_RunState() == 4)
                };
                if (this.assertFromTheLogMessage != null)
                {
                    testResult.resultState = TestResultState.Failure;
                    testResult.message = testResult.message + this.assertFromTheLogMessage;
                    testResult.stackTrace = testResult.stackTrace + this.assertFromTheLogStacktrace;
                }
                testResult.SetTestMethod(result.get_Test() as TestMethod);
                this.m_TestRunnerEventListener.TestFinished(testResult);
                this.m_TestLog = null;
            }

            public void TestOutput(NUnit.Core.TestOutput testOutput)
            {
                if (this.m_TestLog != null)
                {
                    this.m_TestLog.AppendLine(testOutput.get_Text());
                }
            }

            public void TestStarted(TestName testName)
            {
                this.assertFromTheLogMessage = null;
                this.m_TestLog = new StringBuilder();
                this.m_TestRunnerEventListener.TestStarted(testName.get_FullName());
            }

            public void UnhandledException(Exception exception)
            {
            }
        }
    }
}

