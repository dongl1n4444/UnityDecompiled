namespace UnityEngine.TestTools.Logging
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class LogEvent
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsHandled>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEngine.LogType <LogType>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Message>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <StackTrace>k__BackingField;

        public override string ToString() => 
            $"[{this.LogType}] {this.Message}";

        public bool IsHandled { get; set; }

        public UnityEngine.LogType LogType { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}

