namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class CreateMatchRequest : Request
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <advertise>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <eloScore>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, long> <matchAttributes>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <name>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <password>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <privateAddress>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <publicAddress>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private uint <size>k__BackingField;

        public override bool IsValid() => 
            ((base.IsValid() && (this.size >= 2)) && ((this.matchAttributes != null) ? (this.matchAttributes.Count <= 10) : true));

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.name, this.size, this.publicAddress, this.privateAddress, this.eloScore, this.advertise, !string.IsNullOrEmpty(this.password) ? "YES" : "NO", (this.matchAttributes != null) ? this.matchAttributes.Count : 0 };
            return UnityString.Format("[{0}]-name:{1},size:{2},publicAddress:{3},privateAddress:{4},eloScore:{5},advertise:{6},HasPassword:{7},matchAttributes.Count:{8}", args);
        }

        public bool advertise { get; set; }

        public int eloScore { get; set; }

        public Dictionary<string, long> matchAttributes { get; set; }

        public string name { get; set; }

        public string password { get; set; }

        public string privateAddress { get; set; }

        public string publicAddress { get; set; }

        public uint size { get; set; }
    }
}

