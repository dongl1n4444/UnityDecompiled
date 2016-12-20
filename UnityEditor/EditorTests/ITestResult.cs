namespace UnityEditor.EditorTests
{
    using System;

    public interface ITestResult
    {
        /// <summary>
        /// <para>Duration of the test in seconds.</para>
        /// </summary>
        double duration { get; }

        /// <summary>
        /// <para>Whether the test was executed.</para>
        /// </summary>
        bool executed { get; }

        /// <summary>
        /// <para>Full name of the test (includes namespace).</para>
        /// </summary>
        string fullName { get; }

        /// <summary>
        /// <para>Id of the test.</para>
        /// </summary>
        string id { get; }

        /// <summary>
        /// <para>Whether the test should be ignored (and not executed).</para>
        /// </summary>
        bool isIgnored { get; }

        /// <summary>
        /// <para>Whether the test completed successfully.</para>
        /// </summary>
        bool isSuccess { get; }

        /// <summary>
        /// <para>Logs from the test run.</para>
        /// </summary>
        string logs { get; }

        /// <summary>
        /// <para>Message from the test.</para>
        /// </summary>
        string message { get; }

        /// <summary>
        /// <para>Name of the test (without namespace).</para>
        /// </summary>
        string name { get; }

        /// <summary>
        /// <para>The result.</para>
        /// </summary>
        TestResultState resultState { get; }

        /// <summary>
        /// <para>Stacktrace from the test run.</para>
        /// </summary>
        string stackTrace { get; }
    }
}

