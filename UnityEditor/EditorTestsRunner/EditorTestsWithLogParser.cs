namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Base class for editor tests which have the ability to assert log messages.</para>
    /// </summary>
    [NUnit.Framework.TestFixture]
    public class EditorTestsWithLogParser
    {
        private Queue<MessageMatcher> expectedLogList;
        private Queue<string> logList;

        /// <summary>
        /// <para>Expect a log message that matches the parameter.</para>
        /// </summary>
        /// <param name="message">The expected log message.</param>
        public void ExpectLogLine(string message)
        {
            SimpleMessageMatcher matcher = new SimpleMessageMatcher {
                pattern = message
            };
            this.ExpectLogLine(matcher);
        }

        private void ExpectLogLine(MessageMatcher matcher)
        {
            this.expectedLogList.Enqueue(matcher);
        }

        /// <summary>
        /// <para>Expect a log message that matches the regular expression pattern.</para>
        /// </summary>
        /// <param name="pattern">The expected regex pattern.</param>
        public void ExpectLogLineRegex(string pattern)
        {
            RegexpMessageMatcher matcher = new RegexpMessageMatcher {
                pattern = pattern
            };
            this.ExpectLogLine(matcher);
        }

        [NUnit.Framework.TestFixtureSetUp]
        protected virtual void FixtureSetup()
        {
            this.expectedLogList = new Queue<MessageMatcher>();
            this.logList = new Queue<string>();
            Application.logMessageReceived += new Application.LogCallback(this.LogMessageReceived);
        }

        [NUnit.Framework.TestFixtureTearDown]
        protected virtual void FixtureTeardown()
        {
            Application.logMessageReceived -= new Application.LogCallback(this.LogMessageReceived);
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            this.logList.Enqueue(condition);
        }

        [NUnit.Framework.TearDown]
        protected virtual void TestCleanup()
        {
            while (this.expectedLogList.Count > 0)
            {
                MessageMatcher matcher = this.expectedLogList.Dequeue();
                if (this.logList.Count == 0)
                {
                    NUnit.Framework.Assert.Fail("Expected log message never appeared: " + matcher.pattern);
                }
                string message = this.logList.Dequeue();
                matcher.Assert(message);
            }
        }

        [NUnit.Framework.SetUp]
        protected virtual void TestSetup()
        {
            this.expectedLogList.Clear();
            this.logList.Clear();
        }
    }
}

