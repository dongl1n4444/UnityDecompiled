namespace UnityEngine.TestTools
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.TestTools.Logging;
    using UnityEngine.TestTools.TestRunner;

    /// <summary>
    /// <para>AssertLog allows you to expect Unity log messages that would normally cause the test to fail.</para>
    /// </summary>
    public static class AssertLog
    {
        [CompilerGenerated]
        private static Func<LogEvent, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<LogEvent, bool> <>f__am$cache1;

        /// <summary>
        /// <para>Expect a log message of a specfic type. If an error, assertion or exception log is expected, the test will not fail. The test will fail if a log message is expected but does not appear.</para>
        /// </summary>
        /// <param name="type">Log type.</param>
        /// <param name="message">Log message to expect.</param>
        public static void Expect(LogType type, string message)
        {
            LogMatch item = new LogMatch {
                LogType = new LogType?(type),
                Message = message
            };
            LogScope.Current.ExpectedLogs.Enqueue(item);
        }

        /// <summary>
        /// <para>Expect a log message of a specfic type. If an error, assertion or exception log is expected, the test will not fail. The test will fail if a log message is expected but does not appear.</para>
        /// </summary>
        /// <param name="type">Log type.</param>
        /// <param name="message">Log message to expect.</param>
        public static void Expect(LogType type, Regex message)
        {
            LogMatch item = new LogMatch {
                LogType = new LogType?(type),
                MessageRegex = message
            };
            LogScope.Current.ExpectedLogs.Enqueue(item);
        }

        /// <summary>
        /// <para>Triggers an assert if any logs have been received that were not expected. Returns without asserting if all logs received so far have been registered as expected.</para>
        /// </summary>
        public static void NoUnexpectedReceived()
        {
            LogScope.Current.ProcessExpectedLogs();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => x.IsHandled;
            }
            if (!Enumerable.All<LogEvent>(LogScope.Current.AllLogs, <>f__am$cache0))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = x => !x.IsHandled;
                }
                throw new UnhandledLogMessageException(Enumerable.First<LogEvent>(LogScope.Current.AllLogs, <>f__am$cache1));
            }
        }
    }
}

