namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>The various test results the connection tester may return with.</para>
    /// </summary>
    public enum ConnectionTesterStatus
    {
        /// <summary>
        /// <para>Some unknown error occurred.</para>
        /// </summary>
        Error = -2,
        /// <summary>
        /// <para>Port-restricted NAT type, can do NAT punchthrough to everyone except symmetric.</para>
        /// </summary>
        LimitedNATPunchthroughPortRestricted = 5,
        /// <summary>
        /// <para>Symmetric NAT type, cannot do NAT punchthrough to other symmetric types nor port restricted type.</para>
        /// </summary>
        LimitedNATPunchthroughSymmetric = 6,
        /// <summary>
        /// <para>Address-restricted cone type, NAT punchthrough fully supported.</para>
        /// </summary>
        NATpunchthroughAddressRestrictedCone = 8,
        /// <summary>
        /// <para>Full cone type, NAT punchthrough fully supported.</para>
        /// </summary>
        NATpunchthroughFullCone = 7,
        [Obsolete("No longer returned, use newer connection tester enums instead.")]
        PrivateIPHasNATPunchThrough = 1,
        [Obsolete("No longer returned, use newer connection tester enums instead.")]
        PrivateIPNoNATPunchthrough = 0,
        /// <summary>
        /// <para>Public IP address detected and game listen port is accessible to the internet.</para>
        /// </summary>
        PublicIPIsConnectable = 2,
        /// <summary>
        /// <para>Public IP address detected but server is not initialized and no port is listening.</para>
        /// </summary>
        PublicIPNoServerStarted = 4,
        /// <summary>
        /// <para>Public IP address detected but the port is not connectable from the internet.</para>
        /// </summary>
        PublicIPPortBlocked = 3,
        /// <summary>
        /// <para>Test result undetermined, still in progress.</para>
        /// </summary>
        Undetermined = -1
    }
}

