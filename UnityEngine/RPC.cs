namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Attribute for setting up RPC functions.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true), RequiredByNativeCode, Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system.")]
    public sealed class RPC : Attribute
    {
    }
}

