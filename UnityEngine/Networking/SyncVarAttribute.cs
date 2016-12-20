namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>[SyncVar] is an attribute that can be put on member variables of NetworkBehaviour classes. These variables will have their values sychronized from the server to clients in the game that are in the ready state.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SyncVarAttribute : Attribute
    {
        /// <summary>
        /// <para>The hook attribute can be used to specify a function to be called when the sync var changes value on the client.</para>
        /// </summary>
        public string hook;
    }
}

