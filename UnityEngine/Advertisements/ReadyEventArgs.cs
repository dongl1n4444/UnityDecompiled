namespace UnityEngine.Advertisements
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class ReadyEventArgs : EventArgs
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <placementId>k__BackingField;

        public ReadyEventArgs(string placementId)
        {
            this.placementId = placementId;
        }

        public string placementId { get; private set; }
    }
}

