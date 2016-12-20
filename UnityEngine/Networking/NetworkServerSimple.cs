namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    /// <summary>
    /// <para>The NetworkServerSimple is a basic server class without the "game" related functionality that the NetworkServer class has.</para>
    /// </summary>
    public class NetworkServerSimple
    {
        private List<NetworkConnection> m_Connections = new List<NetworkConnection>();
        private ReadOnlyCollection<NetworkConnection> m_ConnectionsReadOnly;
        private HostTopology m_HostTopology;
        private bool m_Initialized = false;
        private int m_ListenPort;
        private NetworkMessageHandlers m_MessageHandlers = new NetworkMessageHandlers();
        private byte[] m_MsgBuffer = null;
        private NetworkReader m_MsgReader = null;
        private System.Type m_NetworkConnectionClass = typeof(NetworkConnection);
        private int m_RelaySlotId = -1;
        private int m_ServerHostId = -1;
        private bool m_UseWebSockets;

        public NetworkServerSimple()
        {
            this.m_ConnectionsReadOnly = new ReadOnlyCollection<NetworkConnection>(this.m_Connections);
        }

        /// <summary>
        /// <para>Clears the message handlers that are registered.</para>
        /// </summary>
        public void ClearHandlers()
        {
            this.m_MessageHandlers.ClearMessageHandlers();
        }

        /// <summary>
        /// <para>This configures the network transport layer of the server.</para>
        /// </summary>
        /// <param name="config">The transport layer configuration to use.</param>
        /// <param name="maxConnections">Maximum number of network connections to allow.</param>
        /// <param name="topology">The transport layer host topology to use.</param>
        /// <returns>
        /// <para>True if configured.</para>
        /// </returns>
        public bool Configure(HostTopology topology)
        {
            this.m_HostTopology = topology;
            return true;
        }

        /// <summary>
        /// <para>This configures the network transport layer of the server.</para>
        /// </summary>
        /// <param name="config">The transport layer configuration to use.</param>
        /// <param name="maxConnections">Maximum number of network connections to allow.</param>
        /// <param name="topology">The transport layer host topology to use.</param>
        /// <returns>
        /// <para>True if configured.</para>
        /// </returns>
        public bool Configure(ConnectionConfig config, int maxConnections)
        {
            HostTopology topology = new HostTopology(config, maxConnections);
            return this.Configure(topology);
        }

        /// <summary>
        /// <para>This disconnects the connection of the corresponding connection id.</para>
        /// </summary>
        /// <param name="connectionId">The id of the connection to disconnect.</param>
        public void Disconnect(int connectionId)
        {
            NetworkConnection connection = this.FindConnection(connectionId);
            if (connection != null)
            {
                connection.Disconnect();
                this.m_Connections[connectionId] = null;
            }
        }

        /// <summary>
        /// <para>This disconnects all of the active connections.</para>
        /// </summary>
        public void DisconnectAllConnections()
        {
            for (int i = 0; i < this.m_Connections.Count; i++)
            {
                NetworkConnection connection = this.m_Connections[i];
                if (connection != null)
                {
                    connection.Disconnect();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <para>This looks up the network connection object for the specified connection Id.</para>
        /// </summary>
        /// <param name="connectionId">The connection id to look up.</param>
        /// <returns>
        /// <para>A NetworkConnection objects, or null if no connection found.</para>
        /// </returns>
        public NetworkConnection FindConnection(int connectionId)
        {
            if ((connectionId < 0) || (connectionId >= this.m_Connections.Count))
            {
                return null;
            }
            return this.m_Connections[connectionId];
        }

        private void HandleConnect(int connectionId, byte error)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkServerSimple accepted client:" + connectionId);
            }
            if (error != 0)
            {
                this.OnConnectError(connectionId, error);
            }
            else
            {
                string str;
                int num;
                NetworkID kid;
                NodeID eid;
                byte num2;
                NetworkTransport.GetConnectionInfo(this.m_ServerHostId, connectionId, out str, out num, out kid, out eid, out num2);
                NetworkConnection conn = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
                conn.SetHandlers(this.m_MessageHandlers);
                conn.Initialize(str, this.m_ServerHostId, connectionId, this.m_HostTopology);
                conn.lastError = (NetworkError) num2;
                while (this.m_Connections.Count <= connectionId)
                {
                    this.m_Connections.Add(null);
                }
                this.m_Connections[connectionId] = conn;
                this.OnConnected(conn);
            }
        }

        private void HandleData(int connectionId, int channelId, int receivedSize, byte error)
        {
            NetworkConnection conn = this.FindConnection(connectionId);
            if (conn == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("HandleData Unknown connectionId:" + connectionId);
                }
            }
            else
            {
                conn.lastError = (NetworkError) error;
                if (error != 0)
                {
                    this.OnDataError(conn, error);
                }
                else
                {
                    this.m_MsgReader.SeekZero();
                    this.OnData(conn, receivedSize, channelId);
                }
            }
        }

        private void HandleDisconnect(int connectionId, byte error)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkServerSimple disconnect client:" + connectionId);
            }
            NetworkConnection conn = this.FindConnection(connectionId);
            if (conn != null)
            {
                conn.lastError = (NetworkError) error;
                if ((error != 0) && (error != 6))
                {
                    this.m_Connections[connectionId] = null;
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Server client disconnect error, connectionId: ", connectionId, " error: ", (NetworkError) error }));
                    }
                    this.OnDisconnectError(conn, error);
                }
                else
                {
                    conn.Disconnect();
                    this.m_Connections[connectionId] = null;
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Server lost client:" + connectionId);
                    }
                    this.OnDisconnected(conn);
                }
            }
        }

        /// <summary>
        /// <para>Initialization function that is invoked when the server starts listening. This can be overridden to perform custom initialization such as setting the NetworkConnectionClass.</para>
        /// </summary>
        public virtual void Initialize()
        {
            if (!this.m_Initialized)
            {
                this.m_Initialized = true;
                NetworkTransport.Init();
                this.m_MsgBuffer = new byte[0xffff];
                this.m_MsgReader = new NetworkReader(this.m_MsgBuffer);
                if (this.m_HostTopology == null)
                {
                    ConnectionConfig defaultConfig = new ConnectionConfig();
                    defaultConfig.AddChannel(QosType.ReliableSequenced);
                    defaultConfig.AddChannel(QosType.Unreliable);
                    this.m_HostTopology = new HostTopology(defaultConfig, 8);
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkServerSimple initialize.");
                }
            }
        }

        /// <summary>
        /// <para>This starts the server listening for connections on the specified port.</para>
        /// </summary>
        /// <param name="serverListenPort">The port to listen on.</param>
        /// <param name="topology">The transport layer host toplogy to configure with.</param>
        /// <returns>
        /// <para>True if able to listen.</para>
        /// </returns>
        public bool Listen(int serverListenPort)
        {
            return this.Listen(serverListenPort, this.m_HostTopology);
        }

        /// <summary>
        /// <para>This starts the server listening for connections on the specified port.</para>
        /// </summary>
        /// <param name="serverListenPort">The port to listen on.</param>
        /// <param name="topology">The transport layer host toplogy to configure with.</param>
        /// <returns>
        /// <para>True if able to listen.</para>
        /// </returns>
        public bool Listen(int serverListenPort, HostTopology topology)
        {
            this.m_HostTopology = topology;
            this.Initialize();
            this.m_ListenPort = serverListenPort;
            if (this.m_UseWebSockets)
            {
                this.m_ServerHostId = NetworkTransport.AddWebsocketHost(this.m_HostTopology, serverListenPort);
            }
            else
            {
                this.m_ServerHostId = NetworkTransport.AddHost(this.m_HostTopology, serverListenPort);
            }
            if (this.m_ServerHostId == -1)
            {
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkServerSimple listen " + this.m_ListenPort);
            }
            return true;
        }

        public bool Listen(string ipAddress, int serverListenPort)
        {
            this.Initialize();
            this.m_ListenPort = serverListenPort;
            if (this.m_UseWebSockets)
            {
                this.m_ServerHostId = NetworkTransport.AddWebsocketHost(this.m_HostTopology, serverListenPort, ipAddress);
            }
            else
            {
                this.m_ServerHostId = NetworkTransport.AddHost(this.m_HostTopology, serverListenPort, ipAddress);
            }
            if (this.m_ServerHostId == -1)
            {
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "NetworkServerSimple listen: ", ipAddress, ":", this.m_ListenPort }));
            }
            return true;
        }

        /// <summary>
        /// <para>Starts a server using a Relay server. This is the manual way of using the Relay server, as the regular NetworkServer.Connect() will automatically use the Relay server if a match exists.</para>
        /// </summary>
        /// <param name="relayIp">Relay server IP Address.</param>
        /// <param name="relayPort">Relay server port.</param>
        /// <param name="netGuid">GUID of the network to create.</param>
        /// <param name="sourceId">This server's sourceId.</param>
        /// <param name="nodeId">The node to join the network with.</param>
        public void ListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId)
        {
            byte num;
            this.Initialize();
            this.m_ServerHostId = NetworkTransport.AddHost(this.m_HostTopology, this.listenPort);
            if (LogFilter.logDebug)
            {
                Debug.Log("Server Host Slot Id: " + this.m_ServerHostId);
            }
            this.Update();
            NetworkTransport.ConnectAsNetworkHost(this.m_ServerHostId, relayIp, relayPort, netGuid, sourceId, nodeId, out num);
            this.m_RelaySlotId = 0;
            if (LogFilter.logDebug)
            {
                Debug.Log("Relay Slot Id: " + this.m_RelaySlotId);
            }
        }

        /// <summary>
        /// <para>This virtual function can be overridden to perform custom functionality for new network connections.</para>
        /// </summary>
        /// <param name="conn">The new connection object.</param>
        public virtual void OnConnected(NetworkConnection conn)
        {
            conn.InvokeHandlerNoData(0x20);
        }

        /// <summary>
        /// <para>A virtual function that is invoked when there is a connection error.</para>
        /// </summary>
        /// <param name="connectionId">The id of the connection with the error.</param>
        /// <param name="error">The error code.</param>
        public virtual void OnConnectError(int connectionId, byte error)
        {
            Debug.LogError("OnConnectError error:" + error);
        }

        /// <summary>
        /// <para>This virtual function can be overridden to perform custom functionality when data is received for a connection.</para>
        /// </summary>
        /// <param name="conn">The connection that data was received on.</param>
        /// <param name="channelId">The channel that data was received on.</param>
        /// <param name="receivedSize">The amount of data received.</param>
        public virtual void OnData(NetworkConnection conn, int receivedSize, int channelId)
        {
            conn.TransportRecieve(this.m_MsgBuffer, receivedSize, channelId);
        }

        /// <summary>
        /// <para>A virtual function that is called when a data error occurs on a connection.</para>
        /// </summary>
        /// <param name="conn">The connection object that the error occured on.</param>
        /// <param name="error">The error code.</param>
        public virtual void OnDataError(NetworkConnection conn, byte error)
        {
            Debug.LogError("OnDataError error:" + error);
        }

        /// <summary>
        /// <para>This virtual function can be overridden to perform custom functionality for disconnected network connections.</para>
        /// </summary>
        /// <param name="conn"></param>
        public virtual void OnDisconnected(NetworkConnection conn)
        {
            conn.InvokeHandlerNoData(0x21);
        }

        /// <summary>
        /// <para>A virtual function that is called when a disconnect error happens.</para>
        /// </summary>
        /// <param name="conn">The connection object that the error occured on.</param>
        /// <param name="error">The error code.</param>
        public virtual void OnDisconnectError(NetworkConnection conn, byte error)
        {
            Debug.LogError("OnDisconnectError error:" + error);
        }

        /// <summary>
        /// <para>This registers a handler function for a message Id.</para>
        /// </summary>
        /// <param name="msgType">Message Id to register handler for.</param>
        /// <param name="handler">Handler function.</param>
        public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandler(msgType, handler);
        }

        internal void RegisterHandlerSafe(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandlerSafe(msgType, handler);
        }

        /// <summary>
        /// <para>This removes a connection object from the server's list of connections.</para>
        /// </summary>
        /// <param name="connectionId">The id of the connection to remove.</param>
        /// <returns>
        /// <para>True if removed.</para>
        /// </returns>
        public bool RemoveConnectionAtIndex(int connectionId)
        {
            if ((connectionId < 0) || (connectionId >= this.m_Connections.Count))
            {
                return false;
            }
            this.m_Connections[connectionId] = null;
            return true;
        }

        /// <summary>
        /// <para>This sends the data in an array of bytes to the connected client.</para>
        /// </summary>
        /// <param name="connectionId">The id of the connection to send on.</param>
        /// <param name="bytes">The data to send.</param>
        /// <param name="numBytes">The size of the data to send.</param>
        /// <param name="channelId">The channel to send the data on.</param>
        public void SendBytesTo(int connectionId, byte[] bytes, int numBytes, int channelId)
        {
            NetworkConnection connection = this.FindConnection(connectionId);
            if (connection != null)
            {
                connection.SendBytes(bytes, numBytes, channelId);
            }
        }

        /// <summary>
        /// <para>This sends the contents of a NetworkWriter object to the connected client.</para>
        /// </summary>
        /// <param name="connectionId">The id of the connection to send on.</param>
        /// <param name="writer">The writer object to send.</param>
        /// <param name="channelId">The channel to send the data on.</param>
        public void SendWriterTo(int connectionId, NetworkWriter writer, int channelId)
        {
            NetworkConnection connection = this.FindConnection(connectionId);
            if (connection != null)
            {
                connection.SendWriter(writer, channelId);
            }
        }

        /// <summary>
        /// <para>This adds a connection created by external code to the server's list of connections, at the connection's connectionId index.</para>
        /// </summary>
        /// <param name="conn">A new connection object.</param>
        /// <returns>
        /// <para>True if added.</para>
        /// </returns>
        public bool SetConnectionAtIndex(NetworkConnection conn)
        {
            while (this.m_Connections.Count <= conn.connectionId)
            {
                this.m_Connections.Add(null);
            }
            if (this.m_Connections[conn.connectionId] != null)
            {
                return false;
            }
            this.m_Connections[conn.connectionId] = conn;
            conn.SetHandlers(this.m_MessageHandlers);
            return true;
        }

        public void SetNetworkConnectionClass<T>() where T: NetworkConnection
        {
            this.m_NetworkConnectionClass = typeof(T);
        }

        /// <summary>
        /// <para>This stops a server from listening.</para>
        /// </summary>
        public void Stop()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkServerSimple stop ");
            }
            NetworkTransport.RemoveHost(this.m_ServerHostId);
            this.m_ServerHostId = -1;
        }

        /// <summary>
        /// <para>This unregisters a registered message handler function.</para>
        /// </summary>
        /// <param name="msgType">The message id to unregister.</param>
        public void UnregisterHandler(short msgType)
        {
            this.m_MessageHandlers.UnregisterHandler(msgType);
        }

        /// <summary>
        /// <para>This function pumps the server causing incoming network data to be processed, and pending outgoing data to be sent.</para>
        /// </summary>
        public void Update()
        {
            if (this.m_ServerHostId != -1)
            {
                byte num4;
                NetworkEventType dataEvent = NetworkEventType.DataEvent;
                if (this.m_RelaySlotId != -1)
                {
                    dataEvent = NetworkTransport.ReceiveRelayEventFromHost(this.m_ServerHostId, out num4);
                    if ((dataEvent != NetworkEventType.Nothing) && LogFilter.logDebug)
                    {
                        Debug.Log("NetGroup event:" + dataEvent);
                    }
                    if ((dataEvent == NetworkEventType.ConnectEvent) && LogFilter.logDebug)
                    {
                        Debug.Log("NetGroup server connected");
                    }
                    if ((dataEvent == NetworkEventType.DisconnectEvent) && LogFilter.logDebug)
                    {
                        Debug.Log("NetGroup server disconnected");
                    }
                }
                do
                {
                    int num;
                    int num2;
                    int num3;
                    dataEvent = NetworkTransport.ReceiveFromHost(this.m_ServerHostId, out num, out num2, this.m_MsgBuffer, this.m_MsgBuffer.Length, out num3, out num4);
                    if ((dataEvent != NetworkEventType.Nothing) && LogFilter.logDev)
                    {
                        Debug.Log(string.Concat(new object[] { "Server event: host=", this.m_ServerHostId, " event=", dataEvent, " error=", num4 }));
                    }
                    switch (dataEvent)
                    {
                        case NetworkEventType.DataEvent:
                            this.HandleData(num, num2, num3, num4);
                            break;

                        case NetworkEventType.ConnectEvent:
                            this.HandleConnect(num, num4);
                            break;

                        case NetworkEventType.DisconnectEvent:
                            this.HandleDisconnect(num, num4);
                            break;

                        case NetworkEventType.Nothing:
                            break;

                        default:
                            if (LogFilter.logError)
                            {
                                Debug.LogError("Unknown network message type received: " + dataEvent);
                            }
                            break;
                    }
                }
                while (dataEvent != NetworkEventType.Nothing);
                this.UpdateConnections();
            }
        }

        /// <summary>
        /// <para>This function causes pending outgoing data on connections to be sent, but unlike Update() it works when the server is not listening.</para>
        /// </summary>
        public void UpdateConnections()
        {
            for (int i = 0; i < this.m_Connections.Count; i++)
            {
                NetworkConnection connection = this.m_Connections[i];
                if (connection != null)
                {
                    connection.FlushChannels();
                }
            }
        }

        /// <summary>
        /// <para>A read-only list of the current connections being managed.</para>
        /// </summary>
        public ReadOnlyCollection<NetworkConnection> connections
        {
            get
            {
                return this.m_ConnectionsReadOnly;
            }
        }

        /// <summary>
        /// <para>The message handler functions that are registered.</para>
        /// </summary>
        public Dictionary<short, NetworkMessageDelegate> handlers
        {
            get
            {
                return this.m_MessageHandlers.GetHandlers();
            }
        }

        /// <summary>
        /// <para>The transport layer host-topology that the server is configured with.</para>
        /// </summary>
        public HostTopology hostTopology
        {
            get
            {
                return this.m_HostTopology;
            }
        }

        /// <summary>
        /// <para>The network port that the server is listening on.</para>
        /// </summary>
        public int listenPort
        {
            get
            {
                return this.m_ListenPort;
            }
            set
            {
                this.m_ListenPort = value;
            }
        }

        /// <summary>
        /// <para>The internal buffer that the server reads data from the network into. This will contain the most recent data read from the network when OnData() is called.</para>
        /// </summary>
        public byte[] messageBuffer
        {
            get
            {
                return this.m_MsgBuffer;
            }
        }

        /// <summary>
        /// <para>A NetworkReader object that is bound to the server's messageBuffer.</para>
        /// </summary>
        public NetworkReader messageReader
        {
            get
            {
                return this.m_MsgReader;
            }
        }

        /// <summary>
        /// <para>The type of class to be created for new network connections from clients.</para>
        /// </summary>
        public System.Type networkConnectionClass
        {
            get
            {
                return this.m_NetworkConnectionClass;
            }
        }

        /// <summary>
        /// <para>The transport layer hostId of the server.</para>
        /// </summary>
        public int serverHostId
        {
            get
            {
                return this.m_ServerHostId;
            }
            set
            {
                this.m_ServerHostId = value;
            }
        }

        /// <summary>
        /// <para>This causes the server to listen for WebSocket connections instead of regular transport layer connections.</para>
        /// </summary>
        public bool useWebSockets
        {
            get
            {
                return this.m_UseWebSockets;
            }
            set
            {
                this.m_UseWebSockets = value;
            }
        }
    }
}

