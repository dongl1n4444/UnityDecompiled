namespace UnityEngine.Advertisements
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class FinishEventArgs : EventArgs
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <placementId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ShowResult <showResult>k__BackingField;

        public FinishEventArgs(string placementId, ShowResult showResult)
        {
            this.placementId = placementId;
            this.showResult = showResult;
        }

        public string placementId { get; private set; }

        public ShowResult showResult { get; private set; }
    }
}

