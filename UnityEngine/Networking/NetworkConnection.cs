namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>A High level network connection. This is used for connections from client-to-server and for connection from server-to-client.</para>
    /// </summary>
    public class NetworkConnection : IDisposable
    {
        /// <summary>
        /// <para>The IP address associated with the connection.</para>
        /// </summary>
        public string address;
        /// <summary>
        /// <para>Unique identifier for this connection that is assigned by the transport layer.</para>
        /// </summary>
        public int connectionId = -1;
        private NetworkError error;
        /// <summary>
        /// <para>Transport level host ID for this connection.</para>
        /// </summary>
        public int hostId = -1;
        /// <summary>
        /// <para>Flag that tells if the connection has been marked as "ready" by a client calling ClientScene.Ready().</para>
        /// </summary>
        public bool isReady;
        private const int k_MaxMessageLogSize = 150;
        /// <summary>
        /// <para>The last time that a message was received on this connection.</para>
        /// </summary>
        public float lastMessageTime;
        /// <summary>
        /// <para>Setting this to true will log the contents of network message to the console.</para>
        /// </summary>
        public bool logNetworkMessages = false;
        private ChannelBuffer[] m_Channels;
        private HashSet<NetworkInstanceId> m_ClientOwnedObjects;
        private bool m_Disposed;
        private NetworkMessageHandlers m_MessageHandlers;
        private Dictionary<short, NetworkMessageDelegate> m_MessageHandlersDict;
        private NetworkMessage m_MessageInfo = new NetworkMessage();
        private NetworkMessage m_NetMsg = new NetworkMessage();
        private Dictionary<short, PacketStat> m_PacketStats = new Dictionary<short, PacketStat>();
        private List<PlayerController> m_PlayerControllers = new List<PlayerController>();
        private HashSet<NetworkIdentity> m_VisList = new HashSet<NetworkIdentity>();
        private NetworkWriter m_Writer = new NetworkWriter();
        private static int s_MaxPacketStats = 0xff;

        internal void AddOwnedObject(NetworkIdentity obj)
        {
            if (this.m_ClientOwnedObjects == null)
            {
                this.m_ClientOwnedObjects = new HashSet<NetworkInstanceId>();
            }
            this.m_ClientOwnedObjects.Add(obj.netId);
        }

        internal void AddToVisList(NetworkIdentity uv)
        {
            this.m_VisList.Add(uv);
            NetworkServer.ShowForConnection(uv, this);
        }

        private bool CheckChannel(int channelId)
        {
            if (this.m_Channels == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Channels not initialized sending on id '" + channelId);
                }
                return false;
            }
            if ((channelId < 0) || (channelId >= this.m_Channels.Length))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "Invalid channel when sending buffered data, '", channelId, "'. Current channel count is ", this.m_Channels.Length }));
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// <para>This function checks if there is a message handler registered for the message ID.</para>
        /// </summary>
        /// <param name="msgType">The message ID of the handler to look for.</param>
        /// <returns>
        /// <para>True if a handler function was found.</para>
        /// </returns>
        public bool CheckHandler(short msgType) => 
            this.m_MessageHandlersDict.ContainsKey(msgType);

        /// <summary>
        /// <para>Disconnects this connection.</para>
        /// </summary>
        public void Disconnect()
        {
            this.address = "";
            this.isReady = false;
            ClientScene.HandleClientDisconnect(this);
            if (this.hostId != -1)
            {
                byte num;
                NetworkTransport.Disconnect(this.hostId, this.connectionId, out num);
                this.RemoveObservers();
            }
        }

        /// <summary>
        /// <para>Disposes of this connection, releasing channel buffers that it holds.</para>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_Disposed && (this.m_Channels != null))
            {
                for (int i = 0; i < this.m_Channels.Length; i++)
                {
                    this.m_Channels[i].Dispose();
                }
            }
            this.m_Channels = null;
            if (this.m_ClientOwnedObjects != null)
            {
                foreach (NetworkInstanceId id in this.m_ClientOwnedObjects)
                {
                    GameObject obj2 = NetworkServer.FindLocalObject(id);
                    if (obj2 != null)
                    {
                        obj2.GetComponent<NetworkIdentity>().ClearClientOwner();
                    }
                }
            }
            this.m_ClientOwnedObjects = null;
            this.m_Disposed = true;
        }

        ~NetworkConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// <para>This causes the channels of the network connection to flush their data to the transport layer.</para>
        /// </summary>
        public void FlushChannels()
        {
            if (this.m_Channels != null)
            {
                for (int i = 0; i < this.m_Channels.Length; i++)
                {
                    this.m_Channels[i].CheckInternalBuffer();
                }
            }
        }

        internal bool GetPlayerController(short playerControllerId, out PlayerController playerController)
        {
            playerController = null;
            if (this.playerControllers.Count > 0)
            {
                for (int i = 0; i < this.playerControllers.Count; i++)
                {
                    if (this.playerControllers[i].IsValid && (this.playerControllers[i].playerControllerId == playerControllerId))
                    {
                        playerController = this.playerControllers[i];
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public virtual void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
            for (int i = 0; i < this.m_Channels.Length; i++)
            {
                ChannelBuffer buffer = this.m_Channels[i];
                numMsgs += buffer.numMsgsIn;
                numBytes += buffer.numBytesIn;
            }
        }

        public virtual void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
            for (int i = 0; i < this.m_Channels.Length; i++)
            {
                ChannelBuffer buffer = this.m_Channels[i];
                numMsgs += buffer.numMsgsOut;
                numBufferedMsgs += buffer.numBufferedMsgsOut;
                numBytes += buffer.numBytesOut;
                lastBufferedPerSecond += buffer.lastBufferedPerSecond;
            }
        }

        /// <summary>
        /// <para>This makes the connection process the data contained in the buffer, and call handler functions.</para>
        /// </summary>
        /// <param name="buffer">Data to process.</param>
        /// <param name="receivedSize">Size of the data to process.</param>
        /// <param name="channelId">Channel the data was recieved on.</param>
        protected void HandleBytes(byte[] buffer, int receivedSize, int channelId)
        {
            NetworkReader reader = new NetworkReader(buffer);
            this.HandleReader(reader, receivedSize, channelId);
        }

        internal void HandleFragment(NetworkReader reader, int channelId)
        {
            if ((channelId >= 0) && (channelId < this.m_Channels.Length))
            {
                ChannelBuffer buffer = this.m_Channels[channelId];
                if (buffer.HandleFragment(reader))
                {
                    NetworkReader reader2 = new NetworkReader(buffer.fragmentBuffer.AsArraySegment().Array);
                    reader2.ReadInt16();
                    short msgType = reader2.ReadInt16();
                    this.InvokeHandler(msgType, reader2, channelId);
                }
            }
        }

        /// <summary>
        /// <para>This makes the connection process the data contained in the stream, and call handler functions.</para>
        /// </summary>
        /// <param name="reader">Stream that contains data.</param>
        /// <param name="receivedSize">Size of the data.</param>
        /// <param name="channelId">Channel the data was received on.</param>
        protected void HandleReader(NetworkReader reader, int receivedSize, int channelId)
        {
            while (reader.Position < receivedSize)
            {
                ushort count = reader.ReadUInt16();
                short key = reader.ReadInt16();
                byte[] buffer = reader.ReadBytes(count);
                NetworkReader reader2 = new NetworkReader(buffer);
                if (this.logNetworkMessages)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        builder.AppendFormat("{0:X2}", buffer[i]);
                        if (i > 150)
                        {
                            break;
                        }
                    }
                    Debug.Log(string.Concat(new object[] { "ConnectionRecv con:", this.connectionId, " bytes:", count, " msgId:", key, " ", builder }));
                }
                NetworkMessageDelegate delegate2 = null;
                if (this.m_MessageHandlersDict.ContainsKey(key))
                {
                    delegate2 = this.m_MessageHandlersDict[key];
                }
                if (delegate2 != null)
                {
                    this.m_NetMsg.msgType = key;
                    this.m_NetMsg.reader = reader2;
                    this.m_NetMsg.conn = this;
                    this.m_NetMsg.channelId = channelId;
                    delegate2(this.m_NetMsg);
                    this.lastMessageTime = Time.time;
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x1c, "msg", 1);
                    if (key > 0x2f)
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0, key.ToString() + ":" + key.GetType().Name, 1);
                    }
                    if (this.m_PacketStats.ContainsKey(key))
                    {
                        PacketStat stat = this.m_PacketStats[key];
                        stat.count++;
                        stat.bytes += count;
                    }
                    else
                    {
                        PacketStat stat2 = new PacketStat {
                            msgType = key
                        };
                        stat2.count++;
                        stat2.bytes += count;
                        this.m_PacketStats[key] = stat2;
                    }
                }
                else
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Unknown message ID ", key, " connId:", this.connectionId }));
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// <para>This inializes the internal data structures of a NetworkConnection object, including channel buffers.</para>
        /// </summary>
        /// <param name="hostTopology">The topology to be used.</param>
        /// <param name="networkAddress">The host or IP connected to.</param>
        /// <param name="networkHostId">The transport hostId for the connection.</param>
        /// <param name="networkConnectionId">The transport connectionId for the connection.</param>
        public virtual void Initialize(string networkAddress, int networkHostId, int networkConnectionId, HostTopology hostTopology)
        {
            this.m_Writer = new NetworkWriter();
            this.address = networkAddress;
            this.hostId = networkHostId;
            this.connectionId = networkConnectionId;
            int channelCount = hostTopology.DefaultConfig.ChannelCount;
            int packetSize = hostTopology.DefaultConfig.PacketSize;
            if ((hostTopology.DefaultConfig.UsePlatformSpecificProtocols && (Application.platform != RuntimePlatform.PS4)) && (Application.platform != RuntimePlatform.PSP2))
            {
                throw new ArgumentOutOfRangeException("Platform specific protocols are not supported on this platform");
            }
            this.m_Channels = new ChannelBuffer[channelCount];
            for (int i = 0; i < channelCount; i++)
            {
                ChannelQOS lqos = hostTopology.DefaultConfig.Channels[i];
                int bufferSize = packetSize;
                if ((lqos.QOS == QosType.ReliableFragmented) || (lqos.QOS == QosType.UnreliableFragmented))
                {
                    bufferSize = hostTopology.DefaultConfig.FragmentSize * 0x80;
                }
                this.m_Channels[i] = new ChannelBuffer(this, bufferSize, (byte) i, IsReliableQoS(lqos.QOS), IsSequencedQoS(lqos.QOS));
            }
        }

        /// <summary>
        /// <para>This function invokes the registered handler function for a message.</para>
        /// </summary>
        /// <param name="msgType">The message type of the handler to use.</param>
        /// <param name="reader">The stream to read the contents of the message from.</param>
        /// <param name="channelId">The channel that the message arrived on.</param>
        /// <param name="netMsg">The message object to process.</param>
        /// <returns>
        /// <para>True if a handler function was found and invoked.</para>
        /// </returns>
        public bool InvokeHandler(NetworkMessage netMsg)
        {
            if (this.m_MessageHandlersDict.ContainsKey(netMsg.msgType))
            {
                NetworkMessageDelegate delegate2 = this.m_MessageHandlersDict[netMsg.msgType];
                delegate2(netMsg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>This function invokes the registered handler function for a message.</para>
        /// </summary>
        /// <param name="msgType">The message type of the handler to use.</param>
        /// <param name="reader">The stream to read the contents of the message from.</param>
        /// <param name="channelId">The channel that the message arrived on.</param>
        /// <param name="netMsg">The message object to process.</param>
        /// <returns>
        /// <para>True if a handler function was found and invoked.</para>
        /// </returns>
        public bool InvokeHandler(short msgType, NetworkReader reader, int channelId)
        {
            if (this.m_MessageHandlersDict.ContainsKey(msgType))
            {
                this.m_MessageInfo.msgType = msgType;
                this.m_MessageInfo.conn = this;
                this.m_MessageInfo.reader = reader;
                this.m_MessageInfo.channelId = channelId;
                NetworkMessageDelegate delegate2 = this.m_MessageHandlersDict[msgType];
                if (delegate2 == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkConnection InvokeHandler no handler for " + msgType);
                    }
                    return false;
                }
                delegate2(this.m_MessageInfo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>This function invokes the registered handler function for a message, without any message data.</para>
        /// </summary>
        /// <param name="msgType">The message ID of the handler to invoke.</param>
        /// <returns>
        /// <para>True if a handler function was found and invoked.</para>
        /// </returns>
        public bool InvokeHandlerNoData(short msgType) => 
            this.InvokeHandler(msgType, null, 0);

        private static bool IsReliableQoS(QosType qos) => 
            ((((qos == QosType.Reliable) || (qos == QosType.ReliableFragmented)) || (qos == QosType.ReliableSequenced)) || (qos == QosType.ReliableStateUpdate));

        private static bool IsSequencedQoS(QosType qos) => 
            ((qos == QosType.ReliableSequenced) || (qos == QosType.UnreliableSequenced));

        private void LogSend(byte[] bytes)
        {
            NetworkReader reader = new NetworkReader(bytes);
            ushort num = reader.ReadUInt16();
            ushort num2 = reader.ReadUInt16();
            StringBuilder builder = new StringBuilder();
            for (int i = 4; i < (4 + num); i++)
            {
                builder.AppendFormat("{0:X2}", bytes[i]);
                if (i > 150)
                {
                    break;
                }
            }
            Debug.Log(string.Concat(new object[] { "ConnectionSend con:", this.connectionId, " bytes:", num, " msgId:", num2, " ", builder }));
        }

        internal static void OnFragment(NetworkMessage netMsg)
        {
            netMsg.conn.HandleFragment(netMsg.reader, netMsg.channelId);
        }

        /// <summary>
        /// <para>This registers a handler function for a message Id.</para>
        /// </summary>
        /// <param name="msgType">The message ID to register.</param>
        /// <param name="handler">The handler function to register.</param>
        public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandler(msgType, handler);
        }

        internal void RemoveFromVisList(NetworkIdentity uv, bool isDestroyed)
        {
            this.m_VisList.Remove(uv);
            if (!isDestroyed)
            {
                NetworkServer.HideForConnection(uv, this);
            }
        }

        internal void RemoveObservers()
        {
            foreach (NetworkIdentity identity in this.m_VisList)
            {
                identity.RemoveObserverInternal(this);
            }
            this.m_VisList.Clear();
        }

        internal void RemoveOwnedObject(NetworkIdentity obj)
        {
            if (this.m_ClientOwnedObjects != null)
            {
                this.m_ClientOwnedObjects.Remove(obj.netId);
            }
        }

        internal void RemovePlayerController(short playerControllerId)
        {
            for (int i = this.m_PlayerControllers.Count; i >= 0; i--)
            {
                if ((playerControllerId == i) && (playerControllerId == this.m_PlayerControllers[i].playerControllerId))
                {
                    this.m_PlayerControllers[i] = new PlayerController();
                    return;
                }
            }
            if (LogFilter.logError)
            {
                Debug.LogError("RemovePlayer player at playerControllerId " + playerControllerId + " not found");
            }
        }

        /// <summary>
        /// <para>Resets the statistics that are returned from NetworkClient.GetConnectionStats().</para>
        /// </summary>
        public void ResetStats()
        {
            for (short i = 0; i < s_MaxPacketStats; i = (short) (i + 1))
            {
                if (this.m_PacketStats.ContainsKey(i))
                {
                    PacketStat stat = this.m_PacketStats[i];
                    stat.count = 0;
                    stat.bytes = 0;
                    NetworkTransport.SetPacketStat(0, i, 0, 0);
                    NetworkTransport.SetPacketStat(1, i, 0, 0);
                }
            }
        }

        /// <summary>
        /// <para>This sends a network message with a message ID on the connection. This message is sent on channel zero, which by default is the reliable channel.</para>
        /// </summary>
        /// <param name="msgType">The ID of the message to send.</param>
        /// <param name="msg">The message to send.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public virtual bool Send(short msgType, MessageBase msg) => 
            this.SendByChannel(msgType, msg, 0);

        /// <summary>
        /// <para>This sends a network message on the connection using a specific transport layer channel.</para>
        /// </summary>
        /// <param name="msgType">The message ID to send.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="channelId">The transport layer channel to send on.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public virtual bool SendByChannel(short msgType, MessageBase msg, int channelId)
        {
            this.m_Writer.StartMessage(msgType);
            msg.Serialize(this.m_Writer);
            this.m_Writer.FinishMessage();
            return this.SendWriter(this.m_Writer, channelId);
        }

        /// <summary>
        /// <para>This sends an array of bytes on the connection.</para>
        /// </summary>
        /// <param name="bytes">The array of data to be sent.</param>
        /// <param name="numBytes">The number of bytes in the array to be sent.</param>
        /// <param name="channelId">The transport channel to send on.</param>
        /// <returns>
        /// <para>Success if data was sent.</para>
        /// </returns>
        public virtual bool SendBytes(byte[] bytes, int numBytes, int channelId)
        {
            if (this.logNetworkMessages)
            {
                this.LogSend(bytes);
            }
            return (this.CheckChannel(channelId) && this.m_Channels[channelId].SendBytes(bytes, numBytes));
        }

        /// <summary>
        /// <para>This sends a network message with a message ID on the connection. This message is sent on channel one, which by default is the unreliable channel.</para>
        /// </summary>
        /// <param name="msgType">The message ID to send.</param>
        /// <param name="msg">The message to send.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public virtual bool SendUnreliable(short msgType, MessageBase msg) => 
            this.SendByChannel(msgType, msg, 1);

        /// <summary>
        /// <para>This sends the contents of a NetworkWriter object on the connection.</para>
        /// </summary>
        /// <param name="writer">A writer object containing data to send.</param>
        /// <param name="channelId">The transport channel to send on.</param>
        /// <returns>
        /// <para>True if the data was sent.</para>
        /// </returns>
        public virtual bool SendWriter(NetworkWriter writer, int channelId)
        {
            if (this.logNetworkMessages)
            {
                this.LogSend(writer.ToArray());
            }
            return (this.CheckChannel(channelId) && this.m_Channels[channelId].SendWriter(writer));
        }

        /// <summary>
        /// <para>This sets an option on the network channel.</para>
        /// </summary>
        /// <param name="channelId">The channel the option will be set on.</param>
        /// <param name="option">The option to set.</param>
        /// <param name="value">The value for the option.</param>
        /// <returns>
        /// <para>True if the option was set.</para>
        /// </returns>
        public bool SetChannelOption(int channelId, ChannelOption option, int value)
        {
            if (this.m_Channels == null)
            {
                return false;
            }
            if ((channelId < 0) || (channelId >= this.m_Channels.Length))
            {
                return false;
            }
            return this.m_Channels[channelId].SetOption(option, value);
        }

        internal void SetHandlers(NetworkMessageHandlers handlers)
        {
            this.m_MessageHandlers = handlers;
            this.m_MessageHandlersDict = handlers.GetHandlers();
        }

        /// <summary>
        /// <para>The maximum time in seconds that messages are buffered before being sent.</para>
        /// </summary>
        /// <param name="seconds">Time in seconds.</param>
        public void SetMaxDelay(float seconds)
        {
            if (this.m_Channels != null)
            {
                for (int i = 0; i < this.m_Channels.Length; i++)
                {
                    this.m_Channels[i].maxDelay = seconds;
                }
            }
        }

        internal void SetPlayerController(PlayerController player)
        {
            while (player.playerControllerId >= this.m_PlayerControllers.Count)
            {
                this.m_PlayerControllers.Add(new PlayerController());
            }
            this.m_PlayerControllers[player.playerControllerId] = player;
        }

        /// <summary>
        /// <para>Returns a string representation of the NetworkConnection object state.</para>
        /// </summary>
        public override string ToString() => 
            $"hostId: {this.hostId} connectionId: {this.connectionId} isReady: {this.isReady} channel count: {((this.m_Channels == null) ? 0 : this.m_Channels.Length)}";

        /// <summary>
        /// <para>This virtual function allows custom network connection classes to process data from the network before it is passed to the application.</para>
        /// </summary>
        /// <param name="bytes">The data recieved.</param>
        /// <param name="numBytes">The size of the data recieved.</param>
        /// <param name="channelId">The channel that the data was received on.</param>
        public virtual void TransportReceive(byte[] bytes, int numBytes, int channelId)
        {
            this.HandleBytes(bytes, numBytes, channelId);
        }

        [Obsolete("TransportRecieve has been deprecated. Use TransportReceive instead (UnityUpgradable) -> TransportReceive(*)", false)]
        public virtual void TransportRecieve(byte[] bytes, int numBytes, int channelId)
        {
            this.TransportReceive(bytes, numBytes, channelId);
        }

        public virtual bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error) => 
            NetworkTransport.Send(this.hostId, this.connectionId, channelId, bytes, numBytes, out error);

        /// <summary>
        /// <para>This removes the handler registered for a message Id.</para>
        /// </summary>
        /// <param name="msgType">The message ID to unregister.</param>
        public void UnregisterHandler(short msgType)
        {
            this.m_MessageHandlers.UnregisterHandler(msgType);
        }

        /// <summary>
        /// <para>A list of the NetworkIdentity objects owned by this connection.</para>
        /// </summary>
        public HashSet<NetworkInstanceId> clientOwnedObjects =>
            this.m_ClientOwnedObjects;

        /// <summary>
        /// <para>True if the connection is connected to a remote end-point.</para>
        /// </summary>
        public bool isConnected =>
            (this.hostId != -1);

        /// <summary>
        /// <para>The last error associated with this connection.</para>
        /// </summary>
        public NetworkError lastError
        {
            get => 
                this.error;
            internal set
            {
                this.error = value;
            }
        }

        internal Dictionary<short, PacketStat> packetStats =>
            this.m_PacketStats;

        /// <summary>
        /// <para>The list of players for this connection.</para>
        /// </summary>
        public List<PlayerController> playerControllers =>
            this.m_PlayerControllers;

        internal HashSet<NetworkIdentity> visList =>
            this.m_VisList;

        /// <summary>
        /// <para>Structure used to track the number and size of packets of each packets type.</para>
        /// </summary>
        public class PacketStat
        {
            /// <summary>
            /// <para>Total bytes of all messages of this type.</para>
            /// </summary>
            public int bytes;
            /// <summary>
            /// <para>The total number of messages of this type.</para>
            /// </summary>
            public int count;
            /// <summary>
            /// <para>The message type these stats are for.</para>
            /// </summary>
            public short msgType;

            public PacketStat()
            {
                this.msgType = 0;
                this.count = 0;
                this.bytes = 0;
            }

            public PacketStat(NetworkConnection.PacketStat s)
            {
                this.msgType = s.msgType;
                this.count = s.count;
                this.bytes = s.bytes;
            }

            public override string ToString()
            {
                object[] objArray1 = new object[] { MsgType.MsgTypeToString(this.msgType), ": count=", this.count, " bytes=", this.bytes };
                return string.Concat(objArray1);
            }
        }
    }
}

