namespace UnityEngine.TestTools.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class LogScope : IDisposable
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<LogEvent> <AllLogs>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Queue<LogMatch> <ExpectedLogs>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<LogEvent> <FailingLogs>k__BackingField;
        private bool m_Disposed = false;
        private bool m_NeedToProcessLogs;
        private static LogScope s_CurrentScope;

        public LogScope()
        {
            this.AllLogs = new List<LogEvent>();
            this.FailingLogs = new List<LogEvent>();
            this.ExpectedLogs = new Queue<LogMatch>();
            this.Activate();
        }

        private void Activate()
        {
            s_CurrentScope = this;
            Application.logMessageReceivedThreaded -= new Application.LogCallback(this.AddLog);
            Application.logMessageReceivedThreaded += new Application.LogCallback(this.AddLog);
        }

        public void AddLog(string message, string stacktrace, LogType type)
        {
            this.m_NeedToProcessLogs = true;
            LogEvent item = new LogEvent {
                LogType = type,
                Message = message,
                StackTrace = stacktrace
            };
            this.AllLogs.Add(item);
            if (IsFailingLog(type))
            {
                this.FailingLogs.Add(item);
            }
        }

        internal bool AnyFailingLogs()
        {
            this.ProcessExpectedLogs();
            return this.FailingLogs.Any<LogEvent>();
        }

        private void Deactivate()
        {
            Application.logMessageReceivedThreaded -= new Application.LogCallback(this.AddLog);
            s_CurrentScope = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_Disposed)
            {
                this.m_Disposed = true;
                if (disposing)
                {
                    this.Deactivate();
                }
            }
        }

        private static bool IsFailingLog(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    return true;
            }
            return false;
        }

        internal void ProcessExpectedLogs()
        {
            if (this.m_NeedToProcessLogs && this.ExpectedLogs.Any<LogMatch>())
            {
                LogMatch match = null;
                foreach (LogEvent event2 in this.AllLogs)
                {
                    if (!this.ExpectedLogs.Any<LogMatch>())
                    {
                        break;
                    }
                    if ((match == null) && this.ExpectedLogs.Any<LogMatch>())
                    {
                        match = this.ExpectedLogs.Dequeue();
                    }
                    if (match.Matches(event2))
                    {
                        event2.IsHandled = true;
                        if (Enumerable.Any<LogEvent>(this.FailingLogs, new Func<LogEvent, bool>(match.Matches)))
                        {
                            LogEvent item = Enumerable.First<LogEvent>(this.FailingLogs, new Func<LogEvent, bool>(match.Matches));
                            this.FailingLogs.Remove(item);
                        }
                        match = null;
                    }
                }
                this.m_NeedToProcessLogs = false;
            }
        }

        public List<LogEvent> AllLogs { get; private set; }

        internal static LogScope Current
        {
            get
            {
                if (s_CurrentScope == null)
                {
                    throw new InvalidOperationException("No log scope is available");
                }
                return s_CurrentScope;
            }
            private set
            {
                if (s_CurrentScope != null)
                {
                    throw new InvalidOperationException("Log scope is already present");
                }
                s_CurrentScope = value;
            }
        }

        public Queue<LogMatch> ExpectedLogs { get; private set; }

        public List<LogEvent> FailingLogs { get; private set; }
    }
}

