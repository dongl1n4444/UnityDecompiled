namespace UnityEngine.Advertisements
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class ErrorEventArgs : EventArgs
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long <error>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <message>k__BackingField;

        public ErrorEventArgs(long error, string message)
        {
            this.error = error;
            this.message = message;
        }

        public long error { get; private set; }

        public string message { get; private set; }
    }
}

