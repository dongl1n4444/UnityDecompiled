namespace UnityEditor.HolographicEmulation
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class PerceptionRemotingPlugin
    {
        internal static HolographicStreamerConnectionFailureReason CheckForDisconnect() => 
            CheckForDisconnect_Internal();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern HolographicStreamerConnectionFailureReason CheckForDisconnect_Internal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Connect(string clientName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Disconnect();
        internal static HolographicStreamerConnectionState GetConnectionState() => 
            GetConnectionState_Internal();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern HolographicStreamerConnectionState GetConnectionState_Internal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetEnableAudio(bool enable);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetEnableVideo(bool enable);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetVideoEncodingParameters(int maxBitRate);
    }
}

