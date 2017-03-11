namespace UnityEngine.Networking.Match
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal class SetMatchAttributesRequest : Request
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <isListed>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private NetworkID <networkId>k__BackingField;

        public override bool IsValid() => 
            (base.IsValid() && (this.networkId != NetworkID.Invalid));

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.networkId.ToString("X"), this.isListed };
            return UnityString.Format("[{0}]-networkId:{1},isListed:{2}", args);
        }

        public bool isListed { get; set; }

        public NetworkID networkId { get; set; }
    }
}

