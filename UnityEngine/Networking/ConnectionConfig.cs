﻿namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>This class defines parameters of connection between two peers, this definition includes various timeouts and sizes as well as channel configuration.</para>
    /// </summary>
    [Serializable]
    public class ConnectionConfig
    {
        private const int g_MinPacketSize = 0x80;
        [SerializeField]
        private uint m_AckDelay;
        [SerializeField]
        private ConnectionAcksType m_AcksType;
        [SerializeField]
        private uint m_AllCostTimeout;
        [SerializeField]
        private float m_BandwidthPeakFactor;
        [SerializeField]
        internal List<ChannelQOS> m_Channels;
        [SerializeField]
        private uint m_ConnectTimeout;
        [SerializeField]
        private uint m_DisconnectTimeout;
        [SerializeField]
        private ushort m_FragmentSize;
        [SerializeField]
        private uint m_InitialBandwidth;
        [SerializeField]
        private ushort m_MaxCombinedReliableMessageCount;
        [SerializeField]
        private ushort m_MaxCombinedReliableMessageSize;
        [SerializeField]
        private byte m_MaxConnectionAttempt;
        [SerializeField]
        private ushort m_MaxSentMessageQueueSize;
        [SerializeField]
        private uint m_MinUpdateTimeout;
        [SerializeField]
        private byte m_NetworkDropThreshold;
        [SerializeField]
        private byte m_OverflowDropThreshold;
        [SerializeField]
        private ushort m_PacketSize;
        [SerializeField]
        private uint m_PingTimeout;
        [SerializeField]
        private uint m_ReducedPingTimeout;
        [SerializeField]
        private uint m_ResendTimeout;
        [SerializeField]
        private uint m_SendDelay;
        [SerializeField]
        private string m_SSLCAFilePath;
        [SerializeField]
        private string m_SSLCertFilePath;
        [SerializeField]
        private string m_SSLPrivateKeyFilePath;
        [SerializeField]
        private uint m_UdpSocketReceiveBufferMaxSize;
        [SerializeField]
        private bool m_UsePlatformSpecificProtocols;
        [SerializeField]
        private ushort m_WebSocketReceiveBufferMaxSize;

        /// <summary>
        /// <para>Will create default connection config or will copy them from another.</para>
        /// </summary>
        /// <param name="config">Connection config.</param>
        public ConnectionConfig()
        {
            this.m_Channels = new List<ChannelQOS>();
            this.m_PacketSize = 0x5a0;
            this.m_FragmentSize = 500;
            this.m_ResendTimeout = 0x4b0;
            this.m_DisconnectTimeout = 0x7d0;
            this.m_ConnectTimeout = 0x7d0;
            this.m_MinUpdateTimeout = 10;
            this.m_PingTimeout = 500;
            this.m_ReducedPingTimeout = 100;
            this.m_AllCostTimeout = 20;
            this.m_NetworkDropThreshold = 5;
            this.m_OverflowDropThreshold = 5;
            this.m_MaxConnectionAttempt = 10;
            this.m_AckDelay = 0x21;
            this.m_SendDelay = 10;
            this.m_MaxCombinedReliableMessageSize = 100;
            this.m_MaxCombinedReliableMessageCount = 10;
            this.m_MaxSentMessageQueueSize = 0x200;
            this.m_AcksType = ConnectionAcksType.Acks32;
            this.m_UsePlatformSpecificProtocols = false;
            this.m_InitialBandwidth = 0;
            this.m_BandwidthPeakFactor = 2f;
            this.m_WebSocketReceiveBufferMaxSize = 0;
            this.m_UdpSocketReceiveBufferMaxSize = 0;
            this.m_SSLCertFilePath = null;
            this.m_SSLPrivateKeyFilePath = null;
            this.m_SSLCAFilePath = null;
        }

        /// <summary>
        /// <para>Will create default connection config or will copy them from another.</para>
        /// </summary>
        /// <param name="config">Connection config.</param>
        public ConnectionConfig(ConnectionConfig config)
        {
            this.m_Channels = new List<ChannelQOS>();
            if (config == null)
            {
                throw new NullReferenceException("config is not defined");
            }
            this.m_PacketSize = config.m_PacketSize;
            this.m_FragmentSize = config.m_FragmentSize;
            this.m_ResendTimeout = config.m_ResendTimeout;
            this.m_DisconnectTimeout = config.m_DisconnectTimeout;
            this.m_ConnectTimeout = config.m_ConnectTimeout;
            this.m_MinUpdateTimeout = config.m_MinUpdateTimeout;
            this.m_PingTimeout = config.m_PingTimeout;
            this.m_ReducedPingTimeout = config.m_ReducedPingTimeout;
            this.m_AllCostTimeout = config.m_AllCostTimeout;
            this.m_NetworkDropThreshold = config.m_NetworkDropThreshold;
            this.m_OverflowDropThreshold = config.m_OverflowDropThreshold;
            this.m_MaxConnectionAttempt = config.m_MaxConnectionAttempt;
            this.m_AckDelay = config.m_AckDelay;
            this.m_SendDelay = config.m_SendDelay;
            this.m_MaxCombinedReliableMessageSize = config.MaxCombinedReliableMessageSize;
            this.m_MaxCombinedReliableMessageCount = config.m_MaxCombinedReliableMessageCount;
            this.m_MaxSentMessageQueueSize = config.m_MaxSentMessageQueueSize;
            this.m_AcksType = config.m_AcksType;
            this.m_UsePlatformSpecificProtocols = config.m_UsePlatformSpecificProtocols;
            this.m_InitialBandwidth = config.m_InitialBandwidth;
            if (this.m_InitialBandwidth == 0)
            {
                this.m_InitialBandwidth = (uint) ((this.m_PacketSize * 0x3e8) / this.m_MinUpdateTimeout);
            }
            this.m_BandwidthPeakFactor = config.m_BandwidthPeakFactor;
            this.m_WebSocketReceiveBufferMaxSize = config.m_WebSocketReceiveBufferMaxSize;
            this.m_UdpSocketReceiveBufferMaxSize = config.m_UdpSocketReceiveBufferMaxSize;
            this.m_SSLCertFilePath = config.m_SSLCertFilePath;
            this.m_SSLPrivateKeyFilePath = config.m_SSLPrivateKeyFilePath;
            this.m_SSLCAFilePath = config.m_SSLCAFilePath;
            foreach (ChannelQOS lqos in config.m_Channels)
            {
                this.m_Channels.Add(new ChannelQOS(lqos));
            }
        }

        /// <summary>
        /// <para>Adds a new channel to the configuration and returns the unique id of that channel.
        /// 
        /// Channels are logical delimiters of traffic between peers. Every time you send data to a peer, you should use two ids: connection id and channel id. Channels are not only logically separate traffic but could each be configured with a different quality of service (QOS). In the example below, a configuration is created containing two channels with Unreliable and Reliable QOS types. This configuration is then used for sending data.</para>
        /// </summary>
        /// <param name="value">Add new channel to configuration.</param>
        /// <returns>
        /// <para>Channel id, user can use this id to send message via this channel.</para>
        /// </returns>
        public byte AddChannel(QosType value)
        {
            if (this.m_Channels.Count > 0xff)
            {
                throw new ArgumentOutOfRangeException("Channels Count should be less than 256");
            }
            if (!Enum.IsDefined(typeof(QosType), value))
            {
                throw new ArgumentOutOfRangeException("requested qos type doesn't exist: " + ((int) value));
            }
            ChannelQOS item = new ChannelQOS(value);
            this.m_Channels.Add(item);
            return (byte) (this.m_Channels.Count - 1);
        }

        /// <summary>
        /// <para>Return the QoS set for the given channel or throw an out of range exception.</para>
        /// </summary>
        /// <param name="idx">Index in array.</param>
        /// <returns>
        /// <para>Channel QoS.</para>
        /// </returns>
        public QosType GetChannel(byte idx)
        {
            if (idx >= this.m_Channels.Count)
            {
                throw new ArgumentOutOfRangeException("requested index greater than maximum channels count");
            }
            return this.m_Channels[idx].QOS;
        }

        /// <summary>
        /// <para>Validate parameters of connection config. Will throw exceptions if parameters are incorrect.</para>
        /// </summary>
        /// <param name="config"></param>
        public static void Validate(ConnectionConfig config)
        {
            if (config.m_PacketSize < 0x80)
            {
                int num = 0x80;
                throw new ArgumentOutOfRangeException("PacketSize should be > " + num.ToString());
            }
            if (config.m_FragmentSize >= (config.m_PacketSize - 0x80))
            {
                int num2 = 0x80;
                throw new ArgumentOutOfRangeException("FragmentSize should be < PacketSize - " + num2.ToString());
            }
            if (config.m_Channels.Count > 0xff)
            {
                throw new ArgumentOutOfRangeException("Channels number should be less than 256");
            }
        }

        /// <summary>
        /// <para>Defines the duration in milliseconds that the receiver waits for before it sends an acknowledgement back without waiting for any data payload. Default value = 33.
        /// 
        /// Network clients that send data to a server may do so using many different quality of service (QOS) modes, some of which (reliable modes) expect the server to send back acknowledgement of receipt of data sent.
        /// 
        /// Servers must periodically acknowledge data packets received over channels with reliable QOS modes by sending packets containing acknowledgement data (also known as "acks") back to the client. If the server were to send an acknowledgement immediately after receiving each packet from the client there would be significant overhead (the acknowledgement is a 32 or 64 bit integer, which is very small compared to the whole size of the packet which also contains the IP and the UDP header). AckDelay allows the server some time to accumulate a list of received reliable data packets to acknowledge, and decreases traffic overhead by combining many acknowledgements into a single packet.</para>
        /// </summary>
        public uint AckDelay
        {
            get => 
                this.m_AckDelay;
            set
            {
                this.m_AckDelay = value;
            }
        }

        /// <summary>
        /// <para>Determines the size of the buffer used to store reliable messages that are waiting for acknowledgement. It can be set to Ack32, Ack64, Ack96, or Ack128. Depends of this setting buffer can hold 32, 64, 96, or 128 messages. Default value = Ack32.
        /// 
        /// Messages sent on reliable quality of service channels are stored in a special buffer while they wait for acknowledgement from the peer. This buffer can be either 32, 64, 96 or 128 positions long. It is recommended to begin with this value set to Ack32, which defines a buffer up to 32 messages in size. If you receive NoResources errors often when you send reliable messages, change this value to the next possible size.</para>
        /// </summary>
        public ConnectionAcksType AcksType
        {
            get => 
                this.m_AcksType;
            set
            {
                this.m_AcksType = value;
            }
        }

        /// <summary>
        /// <para>Defines the timeout in milliseconds after which messages sent via the AllCost channel will be re-sent without waiting for acknowledgement. Default value = 20 ms.
        /// 
        /// AllCost delivery quality of service (QOS) is a special QOS for delivering game-critical information, such as when the game starts, or when bullets are shot.
        /// 
        /// Due to packets dropping, sometimes reliable messages cannot be delivered and need to be re-sent. Reliable messages will re-sent after RTT+Delta time, (RTT is round trip time) where RTT is a dynamic value and can reach couple of hundred milliseconds. For the AllCost delivery channel this timeout can be user-defined to force game critical information to be re-sent.</para>
        /// </summary>
        public uint AllCostTimeout
        {
            get => 
                this.m_AllCostTimeout;
            set
            {
                this.m_AllCostTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines, when multiplied internally by InitialBandwidth, the maximum bandwidth that can be used under burst conditions.</para>
        /// </summary>
        public float BandwidthPeakFactor
        {
            get => 
                this.m_BandwidthPeakFactor;
            set
            {
                this.m_BandwidthPeakFactor = value;
            }
        }

        /// <summary>
        /// <para>(Read Only) The number of channels in the current configuration.</para>
        /// </summary>
        public int ChannelCount =>
            this.m_Channels.Count;

        /// <summary>
        /// <para>The list of channels belonging to the current configuration.
        /// 
        /// Note: any ConnectionConfig passed as a parameter to a function in Unity Multiplayer is deep copied (that is, an entirely new copy is made, with no references to the original).</para>
        /// </summary>
        public List<ChannelQOS> Channels =>
            this.m_Channels;

        /// <summary>
        /// <para>Timeout in ms which library will wait before it will send another connection request.</para>
        /// </summary>
        public uint ConnectTimeout
        {
            get => 
                this.m_ConnectTimeout;
            set
            {
                this.m_ConnectTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines the timeout in milliseconds before a connection is considered to have been disconnected. Default value = 2000.
        /// 
        /// Unity Multiplayer defines conditions under which a connection is considered as disconnected. Disconnection can happen for the following reasons:
        /// 
        /// (1) A disconnection request was received.
        /// 
        /// (2) The connection has not received any traffic at all for a time longer than DisconnectTimeout (Note that live connections receive regular keep-alive packets, so in this case "no traffic" means not only no user traffic but also absence of any keep-alive traffic as well).
        /// 
        /// (3) Flow control determines that the time between sending packets is longer than DisconnectTimeout. Keep-alive packets are regularly delivered from peers and contain statistical information. This information includes values of packet loss due to network and peer overflow conditions. Setting NetworkDropThreshold and OverflowDropThreshold defines thresholds for flow control which can decrease packet frequency. When the time before sending the next packet is longer than DisconnectTimeout, the connection will be considered as disconnected and a disconnect event is received.</para>
        /// </summary>
        public uint DisconnectTimeout
        {
            get => 
                this.m_DisconnectTimeout;
            set
            {
                this.m_DisconnectTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines the fragment size for fragmented messages (for QOS: ReliableFragmented and UnreliableFragmented). Default value = 500.
        /// 
        /// Under fragmented quality of service modes, the original message is split into fragments (up to 64) of up to FragmentSize bytes each. The fragment size depends on the frequency and size of reliable messages sent. Each reliable message potentially could be re-sent, so you need to choose a fragment size less than the remaining free space in a UDP packet after retransmitted reliable messages are added to the packet. For example, if Networking.ConnectionConfig.PacketSize is 1440 bytes, and a reliable message's average size is 200 bytes, it would be wise to set this parameter to 900 – 1000 bytes.</para>
        /// </summary>
        public ushort FragmentSize
        {
            get => 
                this.m_FragmentSize;
            set
            {
                this.m_FragmentSize = value;
            }
        }

        /// <summary>
        /// <para>Gets or sets the bandwidth in bytes per second that can be used by Unity Multiplayer. No traffic over this limit is allowed. Unity Multiplayer may internally reduce the bandwidth it uses due to flow control. The default value is 1500MB/sec (1,536,000 bytes per second). The default value is intentionally a large number to allow all traffic to pass without delay.</para>
        /// </summary>
        public uint InitialBandwidth
        {
            get => 
                this.m_InitialBandwidth;
            set
            {
                this.m_InitialBandwidth = value;
            }
        }

        [Obsolete("IsAcksLong is deprecated. Use AcksType = ConnectionAcksType.Acks64", false)]
        public bool IsAcksLong
        {
            get => 
                (this.m_AcksType != ConnectionAcksType.Acks32);
            set
            {
                if (value && (this.m_AcksType == ConnectionAcksType.Acks32))
                {
                    this.m_AcksType = ConnectionAcksType.Acks64;
                }
                else if (!value)
                {
                    this.m_AcksType = ConnectionAcksType.Acks32;
                }
            }
        }

        /// <summary>
        /// <para>Defines the maximum number of small reliable messages that can be included in one combined message. Default value = 10.
        /// 
        /// Since each message sent to a server contains IP information and a UDP header, duplicating this information for every message sent can be inefficient in the case where there are many small messages being sent frequently. Many small reliable messages can be combined into one longer reliable message, saving space in the waiting buffer. Unity Multiplayer will automatically combine up to MaxCombinedReliableMessageCount small messages into one message. To qualify as a small message, the data payload of the message should not be greater than MaxCombinedReliableMessageSize.</para>
        /// </summary>
        public ushort MaxCombinedReliableMessageCount
        {
            get => 
                this.m_MaxCombinedReliableMessageCount;
            set
            {
                this.m_MaxCombinedReliableMessageCount = value;
            }
        }

        /// <summary>
        /// <para>Defines the maximum size in bytes of a reliable message which is considered small enough to include in a combined message. Default value = 100.
        /// 
        /// Since each message sent to a server contains IP information and a UDP header, duplicating this information for every message sent can be inefficient in the case where there are many small messages being sent frequently. Many small reliable messages can be combined into one longer reliable message, saving space in the waiting buffer. Unity Multiplayer will automatically combine up to MaxCombinedReliableMessageCount small messages into one message. To qualify as a small message, the data payload of the message should not be greater than MaxCombinedReliableMessageSize.</para>
        /// </summary>
        public ushort MaxCombinedReliableMessageSize
        {
            get => 
                this.m_MaxCombinedReliableMessageSize;
            set
            {
                this.m_MaxCombinedReliableMessageSize = value;
            }
        }

        /// <summary>
        /// <para>Defines the maximum number of times Unity Multiplayer will attempt to send a connection request without receiving a response before it reports that it cannot establish a connection. Default value = 10.</para>
        /// </summary>
        public byte MaxConnectionAttempt
        {
            get => 
                this.m_MaxConnectionAttempt;
            set
            {
                this.m_MaxConnectionAttempt = value;
            }
        }

        /// <summary>
        /// <para>Defines maximum number of messages that can be held in the queue for sending. Default value = 128.
        /// 
        /// This buffer serves to smooth spikes in traffic and decreases network jitter. If the queue is full, a NoResources error will result from any calls to Send(). Setting this value greater than around 300 is likely to cause significant delaying of message delivering and can make game unplayable.</para>
        /// </summary>
        public ushort MaxSentMessageQueueSize
        {
            get => 
                this.m_MaxSentMessageQueueSize;
            set
            {
                this.m_MaxSentMessageQueueSize = value;
            }
        }

        /// <summary>
        /// <para>Defines minimum time in milliseconds between sending packets. This duration may be automatically increased if required by flow control. Default value = 10.
        /// 
        /// When Send() is called, Unity Multiplayer won’t send the message immediately. Instead, once every SendTimeout milliseconds each connection is checked to see if it has something to send. While initial and minimal send timeouts can be set, these may be increased internally due to network conditions or buffer overflows.</para>
        /// </summary>
        public uint MinUpdateTimeout
        {
            get => 
                this.m_MinUpdateTimeout;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("Minimal update timeout should be > 0");
                }
                this.m_MinUpdateTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines the percentage (from 0 to 100) of packets that need to be dropped due to network conditions before the SendUpdate timeout is automatically increased (and send rate is automatically decreased). Default value = 5.
        /// 
        /// To avoid receiver overflow, Unity Multiplayer supports flow control. Each ping packet sent between connected peers contains two values:
        /// 
        /// (1) Packets lost due to network conditions.
        /// 
        /// (2) Packets lost because the receiver does not have free space in its incoming buffers.
        /// 
        /// Like OverflowDropThreshold, both values are reported in percent. Use NetworkDropThreshold and OverflowDropThreshold to set thresholds for these values. If a value reported in the ping packet exceeds the corresponding threshold, Unity Multiplayer increases the sending timeout for packets up to a maximum value of DisconnectTimeout.
        /// 
        /// Note: wireless networks usually exhibit 5% or greater packet loss. For wireless networks it is advisable to use a NetworkDropThreshold of 40-50%.</para>
        /// </summary>
        public byte NetworkDropThreshold
        {
            get => 
                this.m_NetworkDropThreshold;
            set
            {
                this.m_NetworkDropThreshold = value;
            }
        }

        /// <summary>
        /// <para>Defines the percentage (from 0 to 100) of packets that need to be dropped due to lack of space in internal buffers before the SendUpdate timeout is automatically increased (and send rate is automatically decreased). Default value = 5.
        /// 
        /// To avoid receiver overflow, Unity Multiplayer supports flow control. Each ping packet sent between connected peers contains two values:
        /// 
        /// (1) Packets lost due to network conditions.
        /// 
        /// (2) Packets lost because the receiver does not have free space in its incoming buffers.
        /// 
        /// Like NetworkDropThreshold, both values are reported in percent. Use NetworkDropThreshold and OverflowDropThreshold to set thresholds for these values. If a value reported in the ping packet exceeds the corresponding threshold, Unity Multiplayer increases the sending timeout for packets up to a maximum value of DisconnectTimeout.
        /// 
        /// Note: wireless networks usually exhibit 5% or greater packet loss. For wireless networks it is advisable to use a NetworkDropThreshold of 40-50%.</para>
        /// </summary>
        public byte OverflowDropThreshold
        {
            get => 
                this.m_OverflowDropThreshold;
            set
            {
                this.m_OverflowDropThreshold = value;
            }
        }

        /// <summary>
        /// <para>Defines maximum packet size (in bytes) (including payload and all header). Packet can contain multiple messages inside. Default value = 1500.
        /// 
        /// Note that this default value is suitable for local testing only. Usually you should change this value; a recommended setting for PC or mobile is 1470. For games consoles this value should probably be less than ~1100. Wrong size definition can cause packet dropping.</para>
        /// </summary>
        public ushort PacketSize
        {
            get => 
                this.m_PacketSize;
            set
            {
                this.m_PacketSize = value;
            }
        }

        /// <summary>
        /// <para>Defines the duration in milliseconds between keep-alive packets, also known as pings. Default value = 500.
        /// 
        /// The ping frequency should be long enough to accumulate good statistics and short enough to compare with DisconnectTimeout. A good guideline is to have more than 3 pings per disconnect timeout, and more than 5 messages per ping. For example, with a DisconnectTimeout of 2000ms, a PingTimeout of 500ms works well.</para>
        /// </summary>
        public uint PingTimeout
        {
            get => 
                this.m_PingTimeout;
            set
            {
                this.m_PingTimeout = value;
            }
        }

        public uint ReducedPingTimeout
        {
            get => 
                this.m_ReducedPingTimeout;
            set
            {
                this.m_ReducedPingTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines the maximum wait time in milliseconds before the "not acknowledged" message is re-sent. Default value = 1200.
        /// 
        /// It does not make a lot of sense to wait for acknowledgement forever. This parameter sets an upper time limit at which point reliable messages are re-sent.</para>
        /// </summary>
        public uint ResendTimeout
        {
            get => 
                this.m_ResendTimeout;
            set
            {
                this.m_ResendTimeout = value;
            }
        }

        /// <summary>
        /// <para>Gets or sets the delay in milliseconds after a call to Send() before packets are sent. During this time, new messages may be combined in queued packets. Default value: 10ms.</para>
        /// </summary>
        public uint SendDelay
        {
            get => 
                this.m_SendDelay;
            set
            {
                this.m_SendDelay = value;
            }
        }

        /// <summary>
        /// <para>Defines the path to the file containing the certification authority (CA) certificate for WebSocket via SSL communication.</para>
        /// </summary>
        public string SSLCAFilePath
        {
            get => 
                this.m_SSLCAFilePath;
            set
            {
                this.m_SSLCAFilePath = value;
            }
        }

        /// <summary>
        /// <para>Defines path to SSL certificate file, for WebSocket via SSL communication.</para>
        /// </summary>
        public string SSLCertFilePath
        {
            get => 
                this.m_SSLCertFilePath;
            set
            {
                this.m_SSLCertFilePath = value;
            }
        }

        /// <summary>
        /// <para>Defines the path to the file containing the private key for WebSocket via SSL communication.</para>
        /// </summary>
        public string SSLPrivateKeyFilePath
        {
            get => 
                this.m_SSLPrivateKeyFilePath;
            set
            {
                this.m_SSLPrivateKeyFilePath = value;
            }
        }

        /// <summary>
        /// <para>Defines the size in bytes of the receiving buffer for UDP sockets. It is useful to set this parameter equal to the maximum size of a fragmented message. Default value is OS specific (usually 8kb).</para>
        /// </summary>
        public uint UdpSocketReceiveBufferMaxSize
        {
            get => 
                this.m_UdpSocketReceiveBufferMaxSize;
            set
            {
                this.m_UdpSocketReceiveBufferMaxSize = value;
            }
        }

        /// <summary>
        /// <para>When starting a server use protocols that make use of platform specific optimisations where appropriate rather than cross-platform protocols. (Sony consoles only).</para>
        /// </summary>
        public bool UsePlatformSpecificProtocols
        {
            get => 
                this.m_UsePlatformSpecificProtocols;
            set
            {
                if ((value && (Application.platform != RuntimePlatform.PS4)) && (Application.platform != RuntimePlatform.PSP2))
                {
                    throw new ArgumentOutOfRangeException("Platform specific protocols are not supported on this platform");
                }
                this.m_UsePlatformSpecificProtocols = value;
            }
        }

        /// <summary>
        /// <para>WebSocket only. Defines the buffer size in bytes for received frames on a WebSocket host. If this value is 0 (the default), a 4 kilobyte buffer is used. Any other value results in a buffer of that size, in bytes.
        /// 
        /// WebSocket message fragments are called "frames". A WebSocket host has a buffer to store incoming message frames. Therefore this buffer should be set to the largest legal frame size supported. If an incoming frame exceeds the buffer size, no error is reported. However, the buffer will invoke the user callback in order to create space for the overflow.</para>
        /// </summary>
        public ushort WebSocketReceiveBufferMaxSize
        {
            get => 
                this.m_WebSocketReceiveBufferMaxSize;
            set
            {
                this.m_WebSocketReceiveBufferMaxSize = value;
            }
        }
    }
}

