namespace UnityEditor.EditorTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.EditorTestsRunner;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    /// <summary>
    /// <para>This class can invoke editor tests runs.</para>
    /// </summary>
    public static class Batch
    {
        private const string k_DefaultResultFileName = "EditorTestResults.xml";
        private const int returnCodeRunError = 3;
        private const int returnCodeTestsFailed = 2;
        private const int returnCodeTestsOk = 0;

        private static ITestRunnerCallback GetVerboseTestLogger(string verboseLog)
        {
            if (verboseLog != null)
            {
                if (verboseLog == "empty")
                {
                    return null;
                }
                if ((verboseLog == "teamcity") || (verboseLog == "tc"))
                {
                    return new TeamCityTestLogger();
                }
            }
            return new DefaultVerboseTestLogger();
        }

        private static void RunEditorTests(string resultFilePath, string verboseLog, string[] nameFilter, string[] categoryFilter)
        {
            object[] args = new object[] { string.Join(", ", nameFilter), string.Join(", ", categoryFilter) };
            Debug.LogFormat("Running editor tests...\nName filter: {0}\nCategory filter: {1}", args);
            TestRunnerFilter filter = new TestRunnerFilter {
                names = nameFilter,
                categories = categoryFilter
            };
            if (string.IsNullOrEmpty(resultFilePath))
            {
                resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EditorTestResults.xml");
            }
            else if (Directory.Exists(resultFilePath))
            {
                resultFilePath = Path.Combine(resultFilePath, "EditorTestResults.xml");
            }
            if (!Directory.Exists(Path.GetDirectoryName(resultFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(resultFilePath));
            }
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            NUnitTestEngine engine = new NUnitTestEngine();
            new TestListBuilder().GetTests(engine.testSuite, null);
            engine.RunTests(filter, new TestRunnerEventListener(resultFilePath, GetVerboseTestLogger(verboseLog)));
        }

        /// <summary>
        /// <para>Execute editor tests run.</para>
        /// </summary>
        /// <param name="testRunnerCallback">Test runner callback.</param>
        /// <param name="nameFilter">Test names to run.</param>
        /// <param name="categoryFilter">Categories to run.</param>
        public static void RunTests(ITestRunnerCallback testRunnerCallback)
        {
            RunTests(testRunnerCallback, null, null);
        }

        /// <summary>
        /// <para>Execute editor tests run.</para>
        /// </summary>
        /// <param name="testRunnerCallback">Test runner callback.</param>
        /// <param name="nameFilter">Test names to run.</param>
        /// <param name="categoryFilter">Categories to run.</param>
        public static void RunTests(ITestRunnerCallback testRunnerCallback, string[] nameFilter, string[] categoryFilter)
        {
            TestRunnerFilter filter = new TestRunnerFilter {
                names = nameFilter,
                categories = categoryFilter
            };
            NUnitTestEngine engine = new NUnitTestEngine();
            if (engine.testSuite == null)
            {
                Debug.Log("No tests were found. Compilation error?");
                EditorApplication.Exit(3);
            }
            new TestListBuilder().GetTests(engine.testSuite, null);
            engine.RunTests(filter, testRunnerCallback);
        }

        /// <summary>
        /// <para>Run tests in the editor tests runner window.</para>
        /// </summary>
        /// <param name="tests">Names of the tests to run.</param>
        public static void RunTestsInRunnerWindow()
        {
            RunTestsInRunnerWindow(null);
        }

        /// <summary>
        /// <para>Run tests in the editor tests runner window.</para>
        /// </summary>
        /// <param name="tests">Names of the tests to run.</param>
        public static void RunTestsInRunnerWindow(string[] tests)
        {
            (EditorWindow.GetWindow(typeof(EditorTestsRunnerWindow)) as EditorTestsRunnerWindow).RunTests(tests);
        }

        private class TestRunnerEventListener : ITestRunnerCallback
        {
            [CompilerGenerated]
            private static Func<EditorTestResult, bool> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<EditorTestResult, bool> <>f__am$cache1;
            private readonly string m_ResultFilePath;
            private readonly List<EditorTestResult> m_Results;
            private ITestRunnerCallback m_VerboseTestLogger;

            public TestRunnerEventListener(string resultFilePath, ITestRunnerCallback verboseTestLogger)
            {
                this.m_ResultFilePath = resultFilePath;
                this.m_Results = new List<EditorTestResult>();
                this.m_VerboseTestLogger = verboseTestLogger;
            }

            public void RunFinished()
            {
                if (this.m_VerboseTestLogger != null)
                {
                    this.m_VerboseTestLogger.RunFinished();
                }
                string dataPath = Application.dataPath;
                if (!string.IsNullOrEmpty(this.m_ResultFilePath))
                {
                    dataPath = this.m_ResultFilePath;
                }
                string fileName = Path.GetFileName(dataPath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    dataPath = dataPath.Substring(0, dataPath.Length - fileName.Length);
                }
                else
                {
                    fileName = "EditorTestResults.xml";
                }
                char[] separator = new char[] { '/' };
                string[] strArray = Application.dataPath.Split(separator);
                string suiteName = strArray[strArray.Length - 2];
                new XmlResultWriter(suiteName, Application.platform.ToString(), this.m_Results.ToArray()).WriteToFile(dataPath, fileName);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = result => result.executed;
                }
                IEnumerable<EditorTestResult> source = Enumerable.Where<EditorTestResult>(this.m_Results, <>f__am$cache0);
                if (!source.Any<EditorTestResult>())
                {
                    EditorApplication.Exit(3);
                }
                else
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = result => !result.isSuccess;
                    }
                    EditorApplication.Exit(!Enumerable.Where<EditorTestResult>(source, <>f__am$cache1).Any<EditorTestResult>() ? 0 : 2);
                }
            }

            public void RunFinishedException(Exception exception)
            {
                if (this.m_VerboseTestLogger != null)
                {
                    this.m_VerboseTestLogger.RunFinishedException(exception);
                }
                EditorApplication.Exit(3);
                throw exception;
            }

            public void RunStarted(string suiteName, int testCount)
            {
                if (this.m_VerboseTestLogger != null)
                {
                    this.m_VerboseTestLogger.RunStarted(suiteName, testCount);
                }
            }

            public void TestFinished(ITestResult test)
            {
                if (this.m_VerboseTestLogger != null)
                {
                    this.m_VerboseTestLogger.TestFinished(test);
                }
                this.m_Results.Add(test as EditorTestResult);
            }

            public void TestStarted(string fullName)
            {
                if (this.m_VerboseTestLogger != null)
                {
                    this.m_VerboseTestLogger.TestStarted(fullName);
                }
            }
        }
    }
}

