namespace UnityEngine.TestTools.TestRunner
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEngine.TestTools.Logging;
    using UnityEngine.TestTools.Utils;

    internal class UnhandledLogMessageException : ResultStateException
    {
        public UnityEngine.TestTools.Logging.LogEvent LogEvent;
        private string m_CustomStackTrace;

        public UnhandledLogMessageException(UnityEngine.TestTools.Logging.LogEvent log) : base(BuildMessage(log))
        {
            this.LogEvent = log;
            this.m_CustomStackTrace = StackTraceFilter.Filter(log.StackTrace);
        }

        private static string BuildMessage(UnityEngine.TestTools.Logging.LogEvent log) => 
            $"Unhandled log message: {log}";

        public override NUnit.Framework.Interfaces.ResultState ResultState =>
            NUnit.Framework.Interfaces.ResultState.Failure;

        public override string StackTrace =>
            this.m_CustomStackTrace;
    }
}

