namespace UnityEngine.Advertisements
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Collection of options that can be passed to Advertisements.Show to modify advertisement behaviour.</para>
    /// </summary>
    public class ShowOptions
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <gamerSid>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<ShowResult> <resultCallback>k__BackingField;

        /// <summary>
        /// <para>Add a string to specify an identifier for a specific user in the game.</para>
        /// </summary>
        public string gamerSid { get; set; }

        /// <summary>
        /// <para>Callback to recieve the result of the advertisement.</para>
        /// </summary>
        public Action<ShowResult> resultCallback { get; set; }
    }
}

