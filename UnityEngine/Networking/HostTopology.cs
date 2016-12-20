namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Class defines network topology for host (socket opened by Networking.NetworkTransport.AddHost function). This topology defines: (1) how many connection with default config will be supported and (2) what will be special connections (connections with config different from default).</para>
    /// </summary>
    [Serializable]
    public class HostTopology
    {
        [SerializeField]
        private ConnectionConfig m_DefConfig;
        [SerializeField]
        private int m_MaxDefConnections;
        [SerializeField]
        private float m_MessagePoolSizeGrowthFactor;
        [SerializeField]
        private ushort m_ReceivedMessagePoolSize;
        [SerializeField]
        private ushort m_SentMessagePoolSize;
        [SerializeField]
        private List<ConnectionConfig> m_SpecialConnections;

        private HostTopology()
        {
            this.m_DefConfig = null;
            this.m_MaxDefConnections = 0;
            this.m_SpecialConnections = new List<ConnectionConfig>();
            this.m_ReceivedMessagePoolSize = 0x80;
            this.m_SentMessagePoolSize = 0x80;
            this.m_MessagePoolSizeGrowthFactor = 0.75f;
        }

        /// <summary>
        /// <para>Create topology.</para>
        /// </summary>
        /// <param name="defaultConfig">Default config.</param>
        /// <param name="maxDefaultConnections">Maximum default connections.</param>
        public HostTopology(ConnectionConfig defaultConfig, int maxDefaultConnections)
        {
            this.m_DefConfig = null;
            this.m_MaxDefConnections = 0;
            this.m_SpecialConnections = new List<ConnectionConfig>();
            this.m_ReceivedMessagePoolSize = 0x80;
            this.m_SentMessagePoolSize = 0x80;
            this.m_MessagePoolSizeGrowthFactor = 0.75f;
            if (defaultConfig == null)
            {
                throw new NullReferenceException("config is not defined");
            }
            if (maxDefaultConnections <= 0)
            {
                throw new ArgumentOutOfRangeException("maxDefaultConnections", "Number of connections should be > 0");
            }
            if (maxDefaultConnections >= 0xffff)
            {
                throw new ArgumentOutOfRangeException("maxDefaultConnections", "Number of connections should be < 65535");
            }
            ConnectionConfig.Validate(defaultConfig);
            this.m_DefConfig = new ConnectionConfig(defaultConfig);
            this.m_MaxDefConnections = maxDefaultConnections;
        }

        /// <summary>
        /// <para>Add special connection to topology (for example if you need to keep connection to standalone chat server you will need to use this function). Returned id should be use as one of parameters (with ip and port) to establish connection to this server.</para>
        /// </summary>
        /// <param name="config">Connection config for special connection.</param>
        /// <returns>
        /// <para>Id of this connection. You should use this id when you call Networking.NetworkTransport.Connect.</para>
        /// </returns>
        public int AddSpecialConnectionConfig(ConnectionConfig config)
        {
            this.m_SpecialConnections.Add(new ConnectionConfig(config));
            return this.m_SpecialConnections.Count;
        }

        /// <summary>
        /// <para>Return reference to special connection config. Parameters of this config can be changed.</para>
        /// </summary>
        /// <param name="i">Config id.</param>
        /// <returns>
        /// <para>Connection config.</para>
        /// </returns>
        public ConnectionConfig GetSpecialConnectionConfig(int i)
        {
            if ((i > this.m_SpecialConnections.Count) || (i == 0))
            {
                throw new ArgumentException("special configuration index is out of valid range");
            }
            return this.m_SpecialConnections[i - 1];
        }

        /// <summary>
        /// <para>Defines config for default connections in the topology.</para>
        /// </summary>
        public ConnectionConfig DefaultConfig
        {
            get
            {
                return this.m_DefConfig;
            }
        }

        /// <summary>
        /// <para>Defines how many connection with default config be permitted.</para>
        /// </summary>
        public int MaxDefaultConnections
        {
            get
            {
                return this.m_MaxDefConnections;
            }
        }

        /// <summary>
        /// <para>Library keep and reuse internal pools of messages. By default they have size 128. If this value is not enough pools will be automatically increased. This value defines how they will increase. Default value is 0.75, so if original pool size was 128, the new pool size will be 128 * 1.75 = 224.</para>
        /// </summary>
        public float MessagePoolSizeGrowthFactor
        {
            get
            {
                return this.m_MessagePoolSizeGrowthFactor;
            }
            set
            {
                if ((value <= 0.5) || (value > 1.0))
                {
                    throw new ArgumentException("pool growth factor should be varied between 0.5 and 1.0");
                }
                this.m_MessagePoolSizeGrowthFactor = value;
            }
        }

        /// <summary>
        /// <para>What is the size of received messages pool (default 128 bytes).</para>
        /// </summary>
        public ushort ReceivedMessagePoolSize
        {
            get
            {
                return this.m_ReceivedMessagePoolSize;
            }
            set
            {
                this.m_ReceivedMessagePoolSize = value;
            }
        }

        /// <summary>
        /// <para>Defines size of sent message pool (default value 128).</para>
        /// </summary>
        public ushort SentMessagePoolSize
        {
            get
            {
                return this.m_SentMessagePoolSize;
            }
            set
            {
                this.m_SentMessagePoolSize = value;
            }
        }

        /// <summary>
        /// <para>List of special connection configs.</para>
        /// </summary>
        public List<ConnectionConfig> SpecialConnectionConfigs
        {
            get
            {
                return this.m_SpecialConnections;
            }
        }

        /// <summary>
        /// <para>Returns count of special connection added to topology.</para>
        /// </summary>
        public int SpecialConnectionConfigsCount
        {
            get
            {
                return this.m_SpecialConnections.Count;
            }
        }
    }
}

