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
        [SerializeField]
        private ushort m_MaxPacketSize = 0x7d0;
        [SerializeField]
        private ushort m_ReactorMaximumReceivedMessages = 0x400;
        [SerializeField]
        private ushort m_ReactorMaximumSentMessages = 0x400;
        [SerializeField]
        private UnityEngine.Networking.ReactorModel m_ReactorModel = UnityEngine.Networking.ReactorModel.SelectReactor;
        [SerializeField]
        private uint m_ThreadAwakeTimeout = 1;

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
        /// <para>Defines maximum amount of messages in the receive queue.</para>
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
        /// <para>Defines maximum message count in sent queue.</para>
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
    }
}

