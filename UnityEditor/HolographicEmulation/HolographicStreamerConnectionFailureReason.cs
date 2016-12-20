namespace UnityEditor.HolographicEmulation
{
    using System;

    internal enum HolographicStreamerConnectionFailureReason
    {
        None,
        Unknown,
        Unreachable,
        HandshakeFailed,
        ProtocolVersionMismatch,
        ConnectionLost
    }
}

