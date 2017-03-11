namespace UnityEngine.TestTools.TestRunner
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEngine.TestTools.Logging;

    internal class UnexpectedLogMessageException : ResultStateException
    {
        public LogMatch LogEvent;

        public UnexpectedLogMessageException(LogMatch log) : base(BuildMessage(log))
        {
            this.LogEvent = log;
        }

        private static string BuildMessage(LogMatch log) => 
            $"Expected log did not appear: {log}";

        public override NUnit.Framework.Interfaces.ResultState ResultState =>
            NUnit.Framework.Interfaces.ResultState.Failure;

        public override string StackTrace =>
            null;
    }
}

