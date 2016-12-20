namespace UnityEditor.EditorTests
{
    using System;

    public interface ITestRunnerCallback
    {
        /// <summary>
        /// <para>The run was finished.</para>
        /// </summary>
        void RunFinished();
        /// <summary>
        /// <para>The run was interrupted by an exception.</para>
        /// </summary>
        /// <param name="exception">The exception that was raised.</param>
        void RunFinishedException(Exception exception);
        /// <summary>
        /// <para>The run has started.</para>
        /// </summary>
        /// <param name="suiteName">The name of the suite that is being run.</param>
        /// <param name="testCount">The number of tests that will be run.</param>
        void RunStarted(string suiteName, int testCount);
        /// <summary>
        /// <para>A test has been finished.</para>
        /// </summary>
        /// <param name="testResult">The result of the test.</param>
        void TestFinished(ITestResult testResult);
        /// <summary>
        /// <para>A test has been started.</para>
        /// </summary>
        /// <param name="testName">The name of the test.</param>
        void TestStarted(string testName);
    }
}

