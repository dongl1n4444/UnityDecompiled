namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Attribute for setting up RPC functions.</para>
    /// </summary>
    [Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system."), AttributeUsage(AttributeTargets.Method, AllowMultiple=true), RequiredByNativeCode]
    public sealed class RPC : Attribute
    {
    }
}

