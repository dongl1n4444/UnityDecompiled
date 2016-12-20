namespace UnityEditor.EditorTests
{
    using System;

    /// <summary>
    /// <para>Result of an editor test run.</para>
    /// </summary>
    public enum TestResultState : byte
    {
        /// <summary>
        /// <para>The test was cancelled.</para>
        /// </summary>
        Cancelled = 7,
        /// <summary>
        /// <para>The test finished with an error.</para>
        /// </summary>
        Error = 6,
        /// <summary>
        /// <para>The test finished with a failure.</para>
        /// </summary>
        Failure = 5,
        /// <summary>
        /// <para>The test was ignored.</para>
        /// </summary>
        Ignored = 3,
        /// <summary>
        /// <para>The test result is inconclusive.</para>
        /// </summary>
        Inconclusive = 0,
        /// <summary>
        /// <para>The test is not runnable.</para>
        /// </summary>
        NotRunnable = 1,
        /// <summary>
        /// <para>The test was skipped.</para>
        /// </summary>
        Skipped = 2,
        /// <summary>
        /// <para>The test succeeded.</para>
        /// </summary>
        Success = 4
    }
}

