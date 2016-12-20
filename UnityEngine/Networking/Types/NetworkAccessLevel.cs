namespace UnityEngine.Networking.Types
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// <para>Describes the access levels granted to this client.</para>
    /// </summary>
    [DefaultValue(0L)]
    public enum NetworkAccessLevel : ulong
    {
        /// <summary>
        /// <para>Administration access level, generally describing clearence to perform game altering actions against anyone inside a particular match.</para>
        /// </summary>
        Admin = 4L,
        /// <summary>
        /// <para>Invalid access level, signifying no access level has been granted/specified.</para>
        /// </summary>
        Invalid = 0L,
        /// <summary>
        /// <para>Access level Owner, generally granting access for operations key to the peer host server performing it's work.</para>
        /// </summary>
        Owner = 2L,
        /// <summary>
        /// <para>User access level. This means you can do operations which affect yourself only, like disconnect yourself from the match.</para>
        /// </summary>
        User = 1L
    }
}

