﻿namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal sealed class ConnectionConfigInternal : IDisposable
    {
        internal IntPtr m_Ptr;

        private ConnectionConfigInternal()
        {
        }

        public ConnectionConfigInternal(ConnectionConfig config)
        {
            if (config == null)
            {
                throw new NullReferenceException("config is not defined");
            }
            this.InitWrapper();
            this.InitPacketSize(config.PacketSize);
            this.InitFragmentSize(config.FragmentSize);
            this.InitResendTimeout(config.ResendTimeout);
            this.InitDisconnectTimeout(config.DisconnectTimeout);
            this.InitConnectTimeout(config.ConnectTimeout);
            this.InitMinUpdateTimeout(config.MinUpdateTimeout);
            this.InitPingTimeout(config.PingTimeout);
            this.InitReducedPingTimeout(config.ReducedPingTimeout);
            this.InitAllCostTimeout(config.AllCostTimeout);
            this.InitNetworkDropThreshold(config.NetworkDropThreshold);
            this.InitOverflowDropThreshold(config.OverflowDropThreshold);
            this.InitMaxConnectionAttempt(config.MaxConnectionAttempt);
            this.InitAckDelay(config.AckDelay);
            this.InitSendDelay(config.SendDelay);
            this.InitMaxCombinedReliableMessageSize(config.MaxCombinedReliableMessageSize);
            this.InitMaxCombinedReliableMessageCount(config.MaxCombinedReliableMessageCount);
            this.InitMaxSentMessageQueueSize(config.MaxSentMessageQueueSize);
            this.InitAcksType((int) config.AcksType);
            this.InitUsePlatformSpecificProtocols(config.UsePlatformSpecificProtocols);
            this.InitInitialBandwidth(config.InitialBandwidth);
            this.InitBandwidthPeakFactor(config.BandwidthPeakFactor);
            this.InitWebSocketReceiveBufferMaxSize(config.WebSocketReceiveBufferMaxSize);
            this.InitUdpSocketReceiveBufferMaxSize(config.UdpSocketReceiveBufferMaxSize);
            if (config.SSLCertFilePath != null)
            {
                int num = this.InitSSLCertFilePath(config.SSLCertFilePath);
                if (num != 0)
                {
                    throw new ArgumentOutOfRangeException("SSLCertFilePath cannot be > than " + num.ToString());
                }
            }
            if (config.SSLPrivateKeyFilePath != null)
            {
                int num2 = this.InitSSLPrivateKeyFilePath(config.SSLPrivateKeyFilePath);
                if (num2 != 0)
                {
                    throw new ArgumentOutOfRangeException("SSLPrivateKeyFilePath cannot be > than " + num2.ToString());
                }
            }
            if (config.SSLCAFilePath != null)
            {
                int num3 = this.InitSSLCAFilePath(config.SSLCAFilePath);
                if (num3 != 0)
                {
                    throw new ArgumentOutOfRangeException("SSLCAFilePath cannot be > than " + num3.ToString());
                }
            }
            for (byte i = 0; i < config.ChannelCount; i = (byte) (i + 1))
            {
                this.AddChannel(config.GetChannel(i));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern byte AddChannel(QosType value);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern void Dispose();
        ~ConnectionConfigInternal()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern QosType GetChannel(int i);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitAckDelay(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitAcksType(int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitAllCostTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitBandwidthPeakFactor(float value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitConnectTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitDisconnectTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitFragmentSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitInitialBandwidth(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxCombinedReliableMessageCount(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxCombinedReliableMessageSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxConnectionAttempt(byte value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxSentMessageQueueSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMinUpdateTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitNetworkDropThreshold(byte value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitOverflowDropThreshold(byte value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitPacketSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitPingTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitReducedPingTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitResendTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitSendDelay(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int InitSSLCAFilePath(string value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int InitSSLCertFilePath(string value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int InitSSLPrivateKeyFilePath(string value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitUdpSocketReceiveBufferMaxSize(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitUsePlatformSpecificProtocols(bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitWebSocketReceiveBufferMaxSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitWrapper();

        public int ChannelSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

