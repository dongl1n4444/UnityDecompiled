namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Defines global paramters for network library.</para>
    /// </summary>
    [Serializable]
    public class GlobalConfig
    {
        private const uint g_MaxNetSimulatorTimeout = 0x2ee0;
        private const uint g_MaxTimerTimeout = 0x2ee0;
        [SerializeField]
        private ushort m_MaxHosts = 0x10;
        [SerializeField]
        private uint m_MaxNetSimulatorTimeout = 0x2ee0;
        [SerializeField]
        private ushort m_MaxPacketSize = 0x7d0;
        [SerializeField]
        private uint m_MaxTimerTimeout = 0x2ee0;
        [SerializeField]
        private uint m_MinNetSimulatorTimeout = 1;
        [SerializeField]
        private uint m_MinTimerTimeout = 1;
        [SerializeField]
        private ushort m_ReactorMaximumReceivedMessages = 0x400;
        [SerializeField]
        private ushort m_ReactorMaximumSentMessages = 0x400;
        [SerializeField]
        private UnityEngine.Networking.ReactorModel m_ReactorModel = UnityEngine.Networking.ReactorModel.SelectReactor;
        [SerializeField]
        private uint m_ThreadAwakeTimeout = 1;
        [SerializeField]
        private byte m_ThreadPoolSize = 1;

        /// <summary>
        /// <para>Defines how many hosts you can use. Default Value = 16.</para>
        /// </summary>
        public ushort MaxHosts
        {
            get => 
                this.m_MaxHosts;
            set
            {
                this.m_MaxHosts = value;
            }
        }

        /// <summary>
        /// <para>Deprecated. Defines maximum delay for network simulator. See Also: MaxTimerTimeout.</para>
        /// </summary>
        public uint MaxNetSimulatorTimeout
        {
            get => 
                this.m_MaxNetSimulatorTimeout;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("MaxNetSimulatorTimeout should be > 0");
                }
                if (value > 0x2ee0)
                {
                    uint num = 0x2ee0;
                    throw new ArgumentOutOfRangeException("MaxNetSimulatorTimeout should be <=" + num.ToString());
                }
                this.m_MaxNetSimulatorTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines maximum possible packet size in bytes for all network connections.</para>
        /// </summary>
        public ushort MaxPacketSize
        {
            get => 
                this.m_MaxPacketSize;
            set
            {
                this.m_MaxPacketSize = value;
            }
        }

        /// <summary>
        /// <para>Defines the maximum timeout in milliseconds for any configuration. The default value is 12 seconds (12000ms).</para>
        /// </summary>
        public uint MaxTimerTimeout
        {
            get => 
                this.m_MaxTimerTimeout;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("MaxTimerTimeout should be > 0");
                }
                if (value > 0x2ee0)
                {
                    uint num = 0x2ee0;
                    throw new ArgumentOutOfRangeException("MaxTimerTimeout should be <=" + num.ToString());
                }
                this.m_MaxTimerTimeout = value;
            }
        }

        /// <summary>
        /// <para>Deprecated. Defines the minimal timeout for network simulator. You cannot set up any delay less than this value. See Also: MinTimerTimeout.</para>
        /// </summary>
        public uint MinNetSimulatorTimeout
        {
            get => 
                this.m_MinNetSimulatorTimeout;
            set
            {
                if (value > this.MaxNetSimulatorTimeout)
                {
                    throw new ArgumentOutOfRangeException("MinNetSimulatorTimeout should be < MaxTimerTimeout");
                }
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("MinNetSimulatorTimeout should be > 0");
                }
                this.m_MinNetSimulatorTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines the minimum timeout in milliseconds recognised by the system. The default value is 1 ms.</para>
        /// </summary>
        public uint MinTimerTimeout
        {
            get => 
                this.m_MinTimerTimeout;
            set
            {
                if (value > this.MaxTimerTimeout)
                {
                    throw new ArgumentOutOfRangeException("MinTimerTimeout should be < MaxTimerTimeout");
                }
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("MinTimerTimeout should be > 0");
                }
                this.m_MinTimerTimeout = value;
            }
        }

        /// <summary>
        /// <para>This property determines the initial size of the queue that holds messages received by Unity Multiplayer before they are processed.</para>
        /// </summary>
        public ushort ReactorMaximumReceivedMessages
        {
            get => 
                this.m_ReactorMaximumReceivedMessages;
            set
            {
                this.m_ReactorMaximumReceivedMessages = value;
            }
        }

        /// <summary>
        /// <para>Defines the initial size of the send queue. Messages are placed in this queue ready to be sent in packets to their destination.</para>
        /// </summary>
        public ushort ReactorMaximumSentMessages
        {
            get => 
                this.m_ReactorMaximumSentMessages;
            set
            {
                this.m_ReactorMaximumSentMessages = value;
            }
        }

        /// <summary>
        /// <para>Defines reactor model for the network library.</para>
        /// </summary>
        public UnityEngine.Networking.ReactorModel ReactorModel
        {
            get => 
                this.m_ReactorModel;
            set
            {
                this.m_ReactorModel = value;
            }
        }

        /// <summary>
        /// <para>Defines (1) for select reactor, minimum time period, when system will check if there are any messages for send (2) for fixrate reactor, minimum interval of time, when system will check for sending and receiving messages.</para>
        /// </summary>
        public uint ThreadAwakeTimeout
        {
            get => 
                this.m_ThreadAwakeTimeout;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("Minimal thread awake timeout should be > 0");
                }
                this.m_ThreadAwakeTimeout = value;
            }
        }

        /// <summary>
        /// <para>Defines how many worker threads are available to handle incoming and outgoing messages.</para>
        /// </summary>
        public byte ThreadPoolSize
        {
            get => 
                this.m_ThreadPoolSize;
            set
            {
                this.m_ThreadPoolSize = value;
            }
        }
    }
}

