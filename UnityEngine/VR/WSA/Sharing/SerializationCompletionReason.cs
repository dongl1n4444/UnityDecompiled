namespace UnityEngine.VR.WSA.Sharing
{
    using System;

    /// <summary>
    /// <para>This enum represents the result of a WorldAnchorTransferBatch operation.</para>
    /// </summary>
    public enum SerializationCompletionReason
    {
        Succeeded,
        NotSupported,
        AccessDenied,
        UnknownError
    }
}

