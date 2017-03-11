namespace UnityEngine.Networking.Match
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal abstract class Request
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <accessTokenString>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AppID <appId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <domain>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <projectId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SourceID <sourceId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <version>k__BackingField;
        public static readonly int currentVersion = 3;

        protected Request()
        {
        }

        public virtual bool IsValid() => 
            (this.sourceId != SourceID.Invalid);

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.sourceId.ToString("X"), this.projectId, string.IsNullOrEmpty(this.accessTokenString), this.domain };
            return UnityString.Format("[{0}]-SourceID:0x{1},projectId:{2},accessTokenString.IsEmpty:{3},domain:{4}", args);
        }

        public string accessTokenString { get; set; }

        public AppID appId { get; set; }

        public int domain { get; set; }

        public string projectId { get; set; }

        public SourceID sourceId { get; set; }

        public int version { get; set; }
    }
}

