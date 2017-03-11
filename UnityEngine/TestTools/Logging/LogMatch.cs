namespace UnityEngine.TestTools.Logging
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    internal class LogMatch
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEngine.LogType? <LogType>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Message>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Regex <MessageRegex>k__BackingField;

        public bool Matches(LogEvent log)
        {
            if (this.MessageRegex != null)
            {
                return this.MessageRegex.IsMatch(log.Message);
            }
            return this.Message.Equals(log.Message);
        }

        public override string ToString()
        {
            if (this.MessageRegex != null)
            {
                return $"[{this.LogType}] Regex: {this.MessageRegex}";
            }
            return $"[{this.LogType}] {this.Message}";
        }

        public UnityEngine.LogType? LogType { get; set; }

        public string Message { get; set; }

        public Regex MessageRegex { get; set; }
    }
}

