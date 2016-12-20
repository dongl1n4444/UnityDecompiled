namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>Represent the connection state of the version control provider.</para>
    /// </summary>
    [Flags]
    public enum OnlineState
    {
        Updating,
        Online,
        Offline
    }
}

