namespace UnityEngine.TestTools.TestRunner
{
    using System;
    using UnityEngine.TestTools.Logging;

    internal class UnhandledLogMessageException : Exception
    {
        public UnityEngine.TestTools.Logging.LogEvent LogEvent;

        public UnhandledLogMessageException(UnityEngine.TestTools.Logging.LogEvent log) : base(BuildMessage(log))
        {
            this.LogEvent = log;
        }

        private static string BuildMessage(UnityEngine.TestTools.Logging.LogEvent log) => 
            $"Unhandled log message: [{log.LogType}] {log.Message}
{log.StackTrace}
--
{Environment.StackTrace}";
    }
}

