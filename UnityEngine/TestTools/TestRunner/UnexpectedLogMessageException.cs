namespace UnityEngine.TestTools.TestRunner
{
    using System;
    using UnityEngine.TestTools.Logging;

    internal class UnexpectedLogMessageException : Exception
    {
        public LogMatch LogEvent;

        public UnexpectedLogMessageException(LogMatch log) : base(BuildMessage(log))
        {
            this.LogEvent = log;
        }

        private static string BuildMessage(LogMatch log) => 
            string.Format("Expected log did not appear: [{0}] {1}\nEnv st: ", log.LogType, log.Message, Environment.StackTrace);
    }
}

