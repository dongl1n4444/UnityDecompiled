namespace UnityEngine.Networking
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
        private uint m_AllCostTimeout;
        [SerializeField]
        internal List<ChannelQOS> m_Channels;
        [SerializeField]
        private uint m_ConnectTimeout;
        [SerializeField]
        private uint m_DisconnectTimeout;
        [SerializeField]
        private ushort m_FragmentSize;
        [SerializeField]
        private bool m_IsAcksLong;
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
            this.m_PacketSize = 0x5dc;
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
            this.m_MaxCombinedReliableMessageSize = 100;
            this.m_MaxCombinedReliableMessageCount = 10;
            this.m_MaxSentMessageQueueSize = 0x80;
            this.m_IsAcksLong = false;
            this.m_UsePlatformSpecificProtocols = false;
            this.m_WebSocketReceiveBufferMaxSize = 0;
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
            this.m_MaxCombinedReliableMessageSize = config.MaxCombinedReliableMessageSize;
            this.m_MaxCombinedReliableMessageCount = config.m_MaxCombinedReliableMessageCount;
            this.m_MaxSentMessageQueueSize = config.m_MaxSentMessageQueueSize;
            this.m_IsAcksLong = config.m_IsAcksLong;
            this.m_UsePlatformSpecificProtocols = config.m_UsePlatformSpecificProtocols;
            this.m_WebSocketReceiveBufferMaxSize = config.m_WebSocketReceiveBufferMaxSize;
            foreach (ChannelQOS lqos in config.m_Channels)
            {
                this.m_Channels.Add(new ChannelQOS(lqos));
            }
        }

        /// <summary>
        /// <para></para>
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
        /// <para>How long in ms receiver will wait before it will force send acknowledgements back without waiting any payload.</para>
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
        /// <para>Defines timeout in ms after that message with AllCost deliver qos will force resend without acknowledgement waiting.</para>
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
        /// <para>Return amount of channels for current configuration.</para>
        /// </summary>
        public int ChannelCount =>
            this.m_Channels.Count;

        /// <summary>
        /// <para>Allow access to channels list.</para>
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
        /// <para>How long (in ms) library will wait before it will consider connection as disconnected.</para>
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
        /// <para>What should be maximum fragment size (in Bytes) for fragmented messages.</para>
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
        /// <para>If it is true, connection will use 64 bit mask to acknowledge received reliable messages.</para>
        /// </summary>
        public bool IsAcksLong
        {
            get => 
                this.m_IsAcksLong;
            set
            {
                this.m_IsAcksLong = value;
            }
        }

        /// <summary>
        /// <para>Maximum amount of small reliable messages which will combine in one "array of messages". Useful if you are going to send a lot of small reliable messages.</para>
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
        /// <para>Maximum size of reliable message which library will consider as small and will try to combine in one "array of messages" message.</para>
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
        /// <para>How many attempt library will get before it will consider the connection as disconnected.</para>
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
        /// <para>Defines maximum messages which will wait for sending before user will receive error on Send() call.</para>
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
        /// <para>Minimal send update timeout (in ms) for connection. this timeout could be increased by library if flow control will required.</para>
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
        /// <para>How many (in %) packet need to be dropped due network condition before library will throttle send rate.</para>
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
        /// <para>How many (in %) packet need to be dropped due lack of internal bufferes before library will throttle send rate.</para>
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
        /// <para>What is a maximum packet size (in Bytes) (including payload and all header). Packet can contain multiple messages inside.</para>
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
        /// <para>Timeout in ms between control protocol messages.</para>
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

        /// <summary>
        /// <para>Timeout in ms for control messages which library will use before it will accumulate statistics.</para>
        /// </summary>
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
        /// <para>Minimum timeout (in ms) which library will wait before it will resend reliable message.</para>
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
        /// <para>Defines received buffer size for web socket host; you should set this to the size of the biggest legal frame that you support.  If the frame size is exceeded, there is no error, but the buffer will spill to the user callback when 	full. In case zero 4k buffer will be used. Default value is zero.</para>
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

