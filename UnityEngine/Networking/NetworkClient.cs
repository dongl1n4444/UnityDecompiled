namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;

    /// <summary>
    /// <para>This is a network client class used by the networking system. It contains a NetworkConnection that is used to connection to a network server.</para>
    /// </summary>
    public class NetworkClient
    {
        [CompilerGenerated]
        private static AsyncCallback <>f__mg$cache0;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache1;
        private const int k_MaxEventsPerFrame = 500;
        protected ConnectState m_AsyncConnect;
        private int m_ClientConnectionId;
        private int m_ClientId;
        protected NetworkConnection m_Connection;
        private HostTopology m_HostTopology;
        private NetworkMessageHandlers m_MessageHandlers;
        private byte[] m_MsgBuffer;
        private NetworkReader m_MsgReader;
        private System.Type m_NetworkConnectionClass;
        private float m_PacketLoss;
        private EndPoint m_RemoteEndPoint;
        private string m_RequestedServerHost;
        private string m_ServerIp;
        private int m_ServerPort;
        private int m_SimulatedLatency;
        private int m_StatResetTime;
        private bool m_UseSimulator;
        private static List<NetworkClient> s_Clients = new List<NetworkClient>();
        private static CRCMessage s_CRCMessage = new CRCMessage();
        private static bool s_IsActive;

        /// <summary>
        /// <para>Creates a new NetworkClient instance.</para>
        /// </summary>
        public NetworkClient()
        {
            this.m_NetworkConnectionClass = typeof(NetworkConnection);
            this.m_ServerIp = "";
            this.m_ClientId = -1;
            this.m_ClientConnectionId = -1;
            this.m_MessageHandlers = new NetworkMessageHandlers();
            this.m_AsyncConnect = ConnectState.None;
            this.m_RequestedServerHost = "";
            if (LogFilter.logDev)
            {
                Debug.Log("Client created version " + UnityEngine.Networking.Version.Current);
            }
            this.m_MsgBuffer = new byte[0xffff];
            this.m_MsgReader = new NetworkReader(this.m_MsgBuffer);
            AddClient(this);
        }

        public NetworkClient(NetworkConnection conn)
        {
            this.m_NetworkConnectionClass = typeof(NetworkConnection);
            this.m_ServerIp = "";
            this.m_ClientId = -1;
            this.m_ClientConnectionId = -1;
            this.m_MessageHandlers = new NetworkMessageHandlers();
            this.m_AsyncConnect = ConnectState.None;
            this.m_RequestedServerHost = "";
            if (LogFilter.logDev)
            {
                Debug.Log("Client created version " + UnityEngine.Networking.Version.Current);
            }
            this.m_MsgBuffer = new byte[0xffff];
            this.m_MsgReader = new NetworkReader(this.m_MsgBuffer);
            AddClient(this);
            SetActive(true);
            this.m_Connection = conn;
            this.m_AsyncConnect = ConnectState.Connected;
            conn.SetHandlers(this.m_MessageHandlers);
            this.RegisterSystemHandlers(false);
        }

        internal static void AddClient(NetworkClient client)
        {
            s_Clients.Add(client);
        }

        /// <summary>
        /// <para>This configures the transport layer settings for a client.</para>
        /// </summary>
        /// <param name="config">Transport layer configuration object.</param>
        /// <param name="maxConnections">The maximum number of connections to allow.</param>
        /// <param name="topology">Transport layer topology object.</param>
        /// <returns>
        /// <para>True if the configuration was successful.</para>
        /// </returns>
        public bool Configure(HostTopology topology)
        {
            this.m_HostTopology = topology;
            return true;
        }

        /// <summary>
        /// <para>This configures the transport layer settings for a client.</para>
        /// </summary>
        /// <param name="config">Transport layer configuration object.</param>
        /// <param name="maxConnections">The maximum number of connections to allow.</param>
        /// <param name="topology">Transport layer topology object.</param>
        /// <returns>
        /// <para>True if the configuration was successful.</para>
        /// </returns>
        public bool Configure(ConnectionConfig config, int maxConnections)
        {
            HostTopology topology = new HostTopology(config, maxConnections);
            return this.Configure(topology);
        }

        public void Connect(EndPoint secureTunnelEndPoint)
        {
            bool usePlatformSpecificProtocols = NetworkTransport.DoesEndPointUsePlatformProtocols(secureTunnelEndPoint);
            this.PrepareForConnect(usePlatformSpecificProtocols);
            if (LogFilter.logDebug)
            {
                Debug.Log("Client Connect to remoteSockAddr");
            }
            if (secureTunnelEndPoint == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Connect failed: null endpoint passed in");
                }
                this.m_AsyncConnect = ConnectState.Failed;
            }
            else if ((secureTunnelEndPoint.AddressFamily != AddressFamily.InterNetwork) && (secureTunnelEndPoint.AddressFamily != AddressFamily.InterNetworkV6))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Connect failed: Endpoint AddressFamily must be either InterNetwork or InterNetworkV6");
                }
                this.m_AsyncConnect = ConnectState.Failed;
            }
            else
            {
                string fullName = secureTunnelEndPoint.GetType().FullName;
                if (fullName == "System.Net.IPEndPoint")
                {
                    IPEndPoint point = (IPEndPoint) secureTunnelEndPoint;
                    this.Connect(point.Address.ToString(), point.Port);
                }
                else if (((fullName != "UnityEngine.XboxOne.XboxOneEndPoint") && (fullName != "UnityEngine.PS4.SceEndPoint")) && (fullName != "UnityEngine.PSVita.SceEndPoint"))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Connect failed: invalid Endpoint (not IPEndPoint or XboxOneEndPoint or SceEndPoint)");
                    }
                    this.m_AsyncConnect = ConnectState.Failed;
                }
                else
                {
                    byte error = 0;
                    this.m_RemoteEndPoint = secureTunnelEndPoint;
                    this.m_AsyncConnect = ConnectState.Connecting;
                    try
                    {
                        this.m_ClientConnectionId = NetworkTransport.ConnectEndPoint(this.m_ClientId, this.m_RemoteEndPoint, 0, out error);
                    }
                    catch (Exception exception)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError("Connect failed: Exception when trying to connect to EndPoint: " + exception);
                        }
                        this.m_AsyncConnect = ConnectState.Failed;
                        return;
                    }
                    if (this.m_ClientConnectionId == 0)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError("Connect failed: Unable to connect to EndPoint (" + error + ")");
                        }
                        this.m_AsyncConnect = ConnectState.Failed;
                    }
                    else
                    {
                        this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
                        this.m_Connection.SetHandlers(this.m_MessageHandlers);
                        this.m_Connection.Initialize(this.m_ServerIp, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
                    }
                }
            }
        }

        public void Connect(MatchInfo matchInfo)
        {
            this.PrepareForConnect();
            this.ConnectWithRelay(matchInfo);
        }

        /// <summary>
        /// <para>Connect client to a NetworkServer instance.</para>
        /// </summary>
        /// <param name="serverIp">Target IP address or hostname.</param>
        /// <param name="serverPort">Target port number.</param>
        public void Connect(string serverIp, int serverPort)
        {
            this.PrepareForConnect();
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "Client Connect: ", serverIp, ":", serverPort }));
            }
            string hostNameOrAddress = serverIp;
            this.m_ServerPort = serverPort;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                this.m_ServerIp = hostNameOrAddress;
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else if (serverIp.Equals("127.0.0.1") || serverIp.Equals("localhost"))
            {
                this.m_ServerIp = "127.0.0.1";
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else if ((serverIp.IndexOf(":") != -1) && IsValidIpV6(serverIp))
            {
                this.m_ServerIp = serverIp;
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("Async DNS START:" + hostNameOrAddress);
                }
                this.m_RequestedServerHost = hostNameOrAddress;
                this.m_AsyncConnect = ConnectState.Resolving;
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new AsyncCallback(NetworkClient.GetHostAddressesCallback);
                }
                Dns.BeginGetHostAddresses(hostNameOrAddress, <>f__mg$cache0, this);
            }
        }

        private void ConnectWithRelay(MatchInfo info)
        {
            byte num;
            this.m_AsyncConnect = ConnectState.Connecting;
            this.Update();
            this.m_ClientConnectionId = NetworkTransport.ConnectToNetworkPeer(this.m_ClientId, info.address, info.port, 0, 0, info.networkId, Utility.GetSourceID(), info.nodeId, out num);
            this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
            this.m_Connection.SetHandlers(this.m_MessageHandlers);
            this.m_Connection.Initialize(info.address, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
            if (num != 0)
            {
                Debug.LogError("ConnectToNetworkPeer Error: " + num);
            }
        }

        /// <summary>
        /// <para>Connect client to a NetworkServer instance with simulated latency and packet loss.</para>
        /// </summary>
        /// <param name="serverIp">Target IP address or hostname.</param>
        /// <param name="serverPort">Target port number.</param>
        /// <param name="latency">Simulated latency in milliseconds.</param>
        /// <param name="packetLoss">Simulated packet loss percentage.</param>
        public void ConnectWithSimulator(string serverIp, int serverPort, int latency, float packetLoss)
        {
            this.m_UseSimulator = true;
            this.m_SimulatedLatency = latency;
            this.m_PacketLoss = packetLoss;
            this.Connect(serverIp, serverPort);
        }

        internal void ContinueConnect()
        {
            byte num;
            if (this.m_UseSimulator)
            {
                int outMinDelay = this.m_SimulatedLatency / 3;
                if (outMinDelay < 1)
                {
                    outMinDelay = 1;
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Connect Using Simulator ", this.m_SimulatedLatency / 3, "/", this.m_SimulatedLatency }));
                }
                ConnectionSimulatorConfig conf = new ConnectionSimulatorConfig(outMinDelay, this.m_SimulatedLatency, outMinDelay, this.m_SimulatedLatency, this.m_PacketLoss);
                this.m_ClientConnectionId = NetworkTransport.ConnectWithSimulator(this.m_ClientId, this.m_ServerIp, this.m_ServerPort, 0, out num, conf);
            }
            else
            {
                this.m_ClientConnectionId = NetworkTransport.Connect(this.m_ClientId, this.m_ServerIp, this.m_ServerPort, 0, out num);
            }
            this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
            this.m_Connection.SetHandlers(this.m_MessageHandlers);
            this.m_Connection.Initialize(this.m_ServerIp, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
        }

        /// <summary>
        /// <para>Disconnect from server.</para>
        /// </summary>
        public virtual void Disconnect()
        {
            this.m_AsyncConnect = ConnectState.Disconnected;
            ClientScene.HandleClientDisconnect(this.m_Connection);
            if (this.m_Connection != null)
            {
                this.m_Connection.Disconnect();
                this.m_Connection.Dispose();
                this.m_Connection = null;
                NetworkTransport.RemoveHost(this.m_ClientId);
                this.m_ClientId = -1;
            }
        }

        private void GenerateConnectError(int error)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Client Error Connect Error: " + error);
            }
            this.GenerateError(error);
        }

        private void GenerateDataError(int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Client Data Error: " + error2);
            }
            this.GenerateError(error);
        }

        private void GenerateDisconnectError(int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Client Disconnect Error: " + error2);
            }
            this.GenerateError(error);
        }

        private void GenerateError(int error)
        {
            NetworkMessageDelegate handler = this.m_MessageHandlers.GetHandler(0x22);
            if (handler == null)
            {
                handler = this.m_MessageHandlers.GetHandler(0x22);
            }
            if (handler != null)
            {
                ErrorMessage message = new ErrorMessage {
                    errorCode = error
                };
                byte[] buffer = new byte[200];
                NetworkWriter writer = new NetworkWriter(buffer);
                message.Serialize(writer);
                NetworkReader reader = new NetworkReader(buffer);
                NetworkMessage netMsg = new NetworkMessage {
                    msgType = 0x22,
                    reader = reader,
                    conn = this.m_Connection,
                    channelId = 0
                };
                handler(netMsg);
            }
        }

        /// <summary>
        /// <para>Retrieves statistics about the network packets sent on this connection.</para>
        /// </summary>
        /// <returns>
        /// <para>Dictionary of packet statistics for the client's connection.</para>
        /// </returns>
        public Dictionary<short, NetworkConnection.PacketStat> GetConnectionStats() => 
            this.m_Connection?.packetStats;

        internal static void GetHostAddressesCallback(IAsyncResult ar)
        {
            try
            {
                IPAddress[] addressArray = Dns.EndGetHostAddresses(ar);
                NetworkClient asyncState = (NetworkClient) ar.AsyncState;
                if (addressArray.Length == 0)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("DNS lookup failed for:" + asyncState.m_RequestedServerHost);
                    }
                    asyncState.m_AsyncConnect = ConnectState.Failed;
                }
                else
                {
                    asyncState.m_ServerIp = addressArray[0].ToString();
                    asyncState.m_AsyncConnect = ConnectState.Resolved;
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Async DNS Result:" + asyncState.m_ServerIp + " for " + asyncState.m_RequestedServerHost + ": " + asyncState.m_ServerIp);
                    }
                }
            }
            catch (SocketException exception)
            {
                NetworkClient client2 = (NetworkClient) ar.AsyncState;
                if (LogFilter.logError)
                {
                    Debug.LogError("DNS resolution failed: " + exception.GetErrorCode());
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log("Exception:" + exception);
                }
                client2.m_AsyncConnect = ConnectState.Failed;
            }
        }

        /// <summary>
        /// <para>Gets the Return Trip Time for this connection.</para>
        /// </summary>
        /// <returns>
        /// <para>Return trip time in milliseconds.</para>
        /// </returns>
        public int GetRTT()
        {
            byte num2;
            if (this.m_ClientId == -1)
            {
                return 0;
            }
            return NetworkTransport.GetCurrentRtt(this.m_ClientId, this.m_ClientConnectionId, out num2);
        }

        public void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
            if (this.m_Connection != null)
            {
                this.m_Connection.GetStatsIn(out numMsgs, out numBytes);
            }
        }

        public void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
            if (this.m_Connection != null)
            {
                this.m_Connection.GetStatsOut(out numMsgs, out numBufferedMsgs, out numBytes, out lastBufferedPerSecond);
            }
        }

        /// <summary>
        /// <para>Retrieves statistics about the network packets sent on all connections.</para>
        /// </summary>
        /// <returns>
        /// <para>Dictionary of stats.</para>
        /// </returns>
        public static Dictionary<short, NetworkConnection.PacketStat> GetTotalConnectionStats()
        {
            Dictionary<short, NetworkConnection.PacketStat> dictionary = new Dictionary<short, NetworkConnection.PacketStat>();
            for (int i = 0; i < s_Clients.Count; i++)
            {
                Dictionary<short, NetworkConnection.PacketStat> connectionStats = s_Clients[i].GetConnectionStats();
                foreach (short num2 in connectionStats.Keys)
                {
                    if (dictionary.ContainsKey(num2))
                    {
                        NetworkConnection.PacketStat stat = dictionary[num2];
                        stat.count += connectionStats[num2].count;
                        stat.bytes += connectionStats[num2].bytes;
                        dictionary[num2] = stat;
                    }
                    else
                    {
                        dictionary[num2] = new NetworkConnection.PacketStat(connectionStats[num2]);
                    }
                }
            }
            return dictionary;
        }

        private static bool IsValidIpV6(string address)
        {
            for (int i = 0; i < address.Length; i++)
            {
                char ch = address[i];
                if ((((ch != ':') && ((ch < '0') || (ch > '9'))) && ((ch < 'a') || (ch > 'f'))) && ((ch < 'A') || (ch > 'F')))
                {
                    return false;
                }
            }
            return true;
        }

        private void OnCRC(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<CRCMessage>(s_CRCMessage);
            NetworkCRC.Validate(s_CRCMessage.scripts, this.numChannels);
        }

        private void PrepareForConnect()
        {
            this.PrepareForConnect(false);
        }

        private void PrepareForConnect(bool usePlatformSpecificProtocols)
        {
            SetActive(true);
            this.RegisterSystemHandlers(false);
            if (this.m_HostTopology == null)
            {
                ConnectionConfig defaultConfig = new ConnectionConfig();
                defaultConfig.AddChannel(QosType.ReliableSequenced);
                defaultConfig.AddChannel(QosType.Unreliable);
                defaultConfig.UsePlatformSpecificProtocols = usePlatformSpecificProtocols;
                this.m_HostTopology = new HostTopology(defaultConfig, 8);
            }
            if (this.m_UseSimulator)
            {
                int minTimeout = (this.m_SimulatedLatency / 3) - 1;
                if (minTimeout < 1)
                {
                    minTimeout = 1;
                }
                int maxTimeout = this.m_SimulatedLatency * 3;
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "AddHost Using Simulator ", minTimeout, "/", maxTimeout }));
                }
                this.m_ClientId = NetworkTransport.AddHostWithSimulator(this.m_HostTopology, minTimeout, maxTimeout, 0);
            }
            else
            {
                this.m_ClientId = NetworkTransport.AddHost(this.m_HostTopology, 0);
            }
        }

        public bool ReconnectToNewHost(EndPoint secureTunnelEndPoint)
        {
            if (!active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect - NetworkClient must be active");
                }
                return false;
            }
            if (this.m_Connection == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect - no old connection exists");
                }
                return false;
            }
            if (LogFilter.logInfo)
            {
                Debug.Log("NetworkClient Reconnect to remoteSockAddr");
            }
            ClientScene.HandleClientDisconnect(this.m_Connection);
            ClientScene.ClearLocalPlayers();
            this.m_Connection.Disconnect();
            this.m_Connection = null;
            this.m_ClientId = NetworkTransport.AddHost(this.m_HostTopology, 0);
            if (secureTunnelEndPoint == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect failed: null endpoint passed in");
                }
                this.m_AsyncConnect = ConnectState.Failed;
                return false;
            }
            if ((secureTunnelEndPoint.AddressFamily != AddressFamily.InterNetwork) && (secureTunnelEndPoint.AddressFamily != AddressFamily.InterNetworkV6))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect failed: Endpoint AddressFamily must be either InterNetwork or InterNetworkV6");
                }
                this.m_AsyncConnect = ConnectState.Failed;
                return false;
            }
            string fullName = secureTunnelEndPoint.GetType().FullName;
            if (fullName == "System.Net.IPEndPoint")
            {
                IPEndPoint point = (IPEndPoint) secureTunnelEndPoint;
                this.Connect(point.Address.ToString(), point.Port);
                return (this.m_AsyncConnect != ConnectState.Failed);
            }
            if ((fullName != "UnityEngine.XboxOne.XboxOneEndPoint") && (fullName != "UnityEngine.PS4.SceEndPoint"))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect failed: invalid Endpoint (not IPEndPoint or XboxOneEndPoint or SceEndPoint)");
                }
                this.m_AsyncConnect = ConnectState.Failed;
                return false;
            }
            byte error = 0;
            this.m_RemoteEndPoint = secureTunnelEndPoint;
            this.m_AsyncConnect = ConnectState.Connecting;
            try
            {
                this.m_ClientConnectionId = NetworkTransport.ConnectEndPoint(this.m_ClientId, this.m_RemoteEndPoint, 0, out error);
            }
            catch (Exception exception)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect failed: Exception when trying to connect to EndPoint: " + exception);
                }
                this.m_AsyncConnect = ConnectState.Failed;
                return false;
            }
            if (this.m_ClientConnectionId == 0)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect failed: Unable to connect to EndPoint (" + error + ")");
                }
                this.m_AsyncConnect = ConnectState.Failed;
                return false;
            }
            this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
            this.m_Connection.SetHandlers(this.m_MessageHandlers);
            this.m_Connection.Initialize(this.m_ServerIp, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
            return true;
        }

        /// <summary>
        /// <para>This is used by a client that has lost the connection to the old host, to reconnect to the new host of a game.</para>
        /// </summary>
        /// <param name="serverIp">The IP address of the new host.</param>
        /// <param name="serverPort">The port of the new host.</param>
        /// <returns>
        /// <para>True if able to reconnect.</para>
        /// </returns>
        public bool ReconnectToNewHost(string serverIp, int serverPort)
        {
            if (!active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect - NetworkClient must be active");
                }
                return false;
            }
            if (this.m_Connection == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Reconnect - no old connection exists");
                }
                return false;
            }
            if (LogFilter.logInfo)
            {
                Debug.Log(string.Concat(new object[] { "NetworkClient Reconnect ", serverIp, ":", serverPort }));
            }
            ClientScene.HandleClientDisconnect(this.m_Connection);
            ClientScene.ClearLocalPlayers();
            this.m_Connection.Disconnect();
            this.m_Connection = null;
            this.m_ClientId = NetworkTransport.AddHost(this.m_HostTopology, 0);
            string hostNameOrAddress = serverIp;
            this.m_ServerPort = serverPort;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                this.m_ServerIp = hostNameOrAddress;
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else if (serverIp.Equals("127.0.0.1") || serverIp.Equals("localhost"))
            {
                this.m_ServerIp = "127.0.0.1";
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("Async DNS START:" + hostNameOrAddress);
                }
                this.m_AsyncConnect = ConnectState.Resolving;
                Dns.BeginGetHostAddresses(hostNameOrAddress, new AsyncCallback(NetworkClient.GetHostAddressesCallback), this);
            }
            return true;
        }

        /// <summary>
        /// <para>Register a handler for a particular message type.</para>
        /// </summary>
        /// <param name="msgType">Message type number.</param>
        /// <param name="handler">Function handler which will be invoked for when this message type is received.</param>
        public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandler(msgType, handler);
        }

        public void RegisterHandlerSafe(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandlerSafe(msgType, handler);
        }

        internal void RegisterSystemHandlers(bool localClient)
        {
            ClientScene.RegisterSystemHandlers(this, localClient);
            this.RegisterHandlerSafe(14, new NetworkMessageDelegate(this.OnCRC));
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new NetworkMessageDelegate(NetworkConnection.OnFragment);
            }
            this.RegisterHandlerSafe(0x11, <>f__mg$cache1);
        }

        internal static bool RemoveClient(NetworkClient client) => 
            s_Clients.Remove(client);

        /// <summary>
        /// <para>Resets the statistics return by NetworkClient.GetConnectionStats() to zero values.</para>
        /// </summary>
        public void ResetConnectionStats()
        {
            if (this.m_Connection != null)
            {
                this.m_Connection.ResetStats();
            }
        }

        /// <summary>
        /// <para>This sends a network message with a message Id to the server. This message is sent on channel zero, which by default is the reliable channel.</para>
        /// </summary>
        /// <param name="msgType">The id of the message to send.</param>
        /// <param name="msg">A message instance to send.</param>
        /// <returns>
        /// <para>True if message was sent.</para>
        /// </returns>
        public bool Send(short msgType, MessageBase msg)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient Send when not connected to a server");
                    }
                    return false;
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0, msgType.ToString() + ":" + msg.GetType().Name, 1);
                return this.m_Connection.Send(msgType, msg);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient Send with no connection");
            }
            return false;
        }

        /// <summary>
        /// <para>This sends a network message with a message Id to the server on a specific channel.</para>
        /// </summary>
        /// <param name="msgType">The id of the message to send.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="channelId">The channel to send the message on.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public bool SendByChannel(short msgType, MessageBase msg, int channelId)
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0, msgType.ToString() + ":" + msg.GetType().Name, 1);
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient SendByChannel when not connected to a server");
                    }
                    return false;
                }
                return this.m_Connection.SendByChannel(msgType, msg, channelId);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendByChannel with no connection");
            }
            return false;
        }

        /// <summary>
        /// <para>This sends the data in an array of bytes to the server that the client is connected to.</para>
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="numBytes">Number of bytes of data.</param>
        /// <param name="channelId">The QoS channel to send data on.</param>
        /// <returns>
        /// <para>True if successfully sent.</para>
        /// </returns>
        public bool SendBytes(byte[] data, int numBytes, int channelId)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient SendBytes when not connected to a server");
                    }
                    return false;
                }
                return this.m_Connection.SendBytes(data, numBytes, channelId);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendBytes with no connection");
            }
            return false;
        }

        /// <summary>
        /// <para>This sends a network message with a message Id to the server on channel one, which by default is the unreliable channel.</para>
        /// </summary>
        /// <param name="msgType">The message id to send.</param>
        /// <param name="msg">The message to send.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public bool SendUnreliable(short msgType, MessageBase msg)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient SendUnreliable when not connected to a server");
                    }
                    return false;
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0, msgType.ToString() + ":" + msg.GetType().Name, 1);
                return this.m_Connection.SendUnreliable(msgType, msg);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendUnreliable with no connection");
            }
            return false;
        }

        /// <summary>
        /// <para>This sends the contents of the NetworkWriter's buffer to the connected server on the specified channel.</para>
        /// </summary>
        /// <param name="writer">Writer object containing data to send.</param>
        /// <param name="channelId">QoS channel to send data on.</param>
        /// <returns>
        /// <para>True if data successfully sent.</para>
        /// </returns>
        public bool SendWriter(NetworkWriter writer, int channelId)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient SendWriter when not connected to a server");
                    }
                    return false;
                }
                return this.m_Connection.SendWriter(writer, channelId);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendWriter with no connection");
            }
            return false;
        }

        internal static void SetActive(bool state)
        {
            if (!s_IsActive && state)
            {
                NetworkTransport.Init();
            }
            s_IsActive = state;
        }

        internal void SetHandlers(NetworkConnection conn)
        {
            conn.SetHandlers(this.m_MessageHandlers);
        }

        /// <summary>
        /// <para>Set the maximum amount of time that can pass for transmitting the send buffer.</para>
        /// </summary>
        /// <param name="seconds">Delay in seconds.</param>
        public void SetMaxDelay(float seconds)
        {
            if (this.m_Connection == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("SetMaxDelay failed, not connected.");
                }
            }
            else
            {
                this.m_Connection.SetMaxDelay(seconds);
            }
        }

        public void SetNetworkConnectionClass<T>() where T: NetworkConnection
        {
            this.m_NetworkConnectionClass = typeof(T);
        }

        /// <summary>
        /// <para>Shut down a client.</para>
        /// </summary>
        public void Shutdown()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Shutting down client " + this.m_ClientId);
            }
            if (this.m_ClientId != -1)
            {
                NetworkTransport.RemoveHost(this.m_ClientId);
                this.m_ClientId = -1;
            }
            RemoveClient(this);
            if (s_Clients.Count == 0)
            {
                SetActive(false);
            }
        }

        /// <summary>
        /// <para>Shuts down all network clients.</para>
        /// </summary>
        public static void ShutdownAll()
        {
            while (s_Clients.Count != 0)
            {
                s_Clients[0].Shutdown();
            }
            s_Clients = new List<NetworkClient>();
            s_IsActive = false;
            ClientScene.Shutdown();
            NetworkDetailStats.ResetAll();
        }

        /// <summary>
        /// <para>Unregisters a network message handler.</para>
        /// </summary>
        /// <param name="msgType">The message type to unregister.</param>
        public void UnregisterHandler(short msgType)
        {
            this.m_MessageHandlers.UnregisterHandler(msgType);
        }

        internal virtual void Update()
        {
            int num;
            int num2;
            int num3;
            byte num4;
            int num5;
            if (this.m_ClientId == -1)
            {
                return;
            }
            switch (this.m_AsyncConnect)
            {
                case ConnectState.None:
                case ConnectState.Resolving:
                case ConnectState.Disconnected:
                    return;

                case ConnectState.Resolved:
                    this.m_AsyncConnect = ConnectState.Connecting;
                    this.ContinueConnect();
                    return;

                case ConnectState.Failed:
                    this.GenerateConnectError(11);
                    this.m_AsyncConnect = ConnectState.Disconnected;
                    return;

                default:
                    if ((this.m_Connection != null) && (((int) Time.time) != this.m_StatResetTime))
                    {
                        this.m_Connection.ResetStats();
                        this.m_StatResetTime = (int) Time.time;
                    }
                    break;
            }
        Label_00A9:
            num5 = 0;
            NetworkEventType type = NetworkTransport.ReceiveFromHost(this.m_ClientId, out num, out num2, this.m_MsgBuffer, (ushort) this.m_MsgBuffer.Length, out num3, out num4);
            if (this.m_Connection != null)
            {
                this.m_Connection.lastError = (NetworkError) num4;
            }
            if ((type != NetworkEventType.Nothing) && LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "Client event: host=", this.m_ClientId, " event=", type, " error=", num4 }));
            }
            switch (type)
            {
                case NetworkEventType.DataEvent:
                    if (num4 == 0)
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x1d, "msg", 1);
                        this.m_MsgReader.SeekZero();
                        this.m_Connection.TransportReceive(this.m_MsgBuffer, num3, num2);
                        break;
                    }
                    this.GenerateDataError(num4);
                    return;

                case NetworkEventType.ConnectEvent:
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Client connected");
                    }
                    if (num4 != 0)
                    {
                        this.GenerateConnectError(num4);
                        return;
                    }
                    this.m_AsyncConnect = ConnectState.Connected;
                    this.m_Connection.InvokeHandlerNoData(0x20);
                    break;

                case NetworkEventType.DisconnectEvent:
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Client disconnected");
                    }
                    this.m_AsyncConnect = ConnectState.Disconnected;
                    if ((num4 != 0) && (num4 != 6))
                    {
                        this.GenerateDisconnectError(num4);
                    }
                    ClientScene.HandleClientDisconnect(this.m_Connection);
                    if (this.m_Connection != null)
                    {
                        this.m_Connection.InvokeHandlerNoData(0x21);
                    }
                    break;

                case NetworkEventType.Nothing:
                    break;

                default:
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Unknown network message type received: " + type);
                    }
                    break;
            }
            if (++num5 >= 500)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("MaxEventsPerFrame hit (" + 500 + ")");
                }
            }
            else if ((this.m_ClientId != -1) && (type != NetworkEventType.Nothing))
            {
                goto Label_00A9;
            }
            if ((this.m_Connection != null) && (this.m_AsyncConnect == ConnectState.Connected))
            {
                this.m_Connection.FlushChannels();
            }
        }

        internal static void UpdateClients()
        {
            for (int i = 0; i < s_Clients.Count; i++)
            {
                if (s_Clients[i] != null)
                {
                    s_Clients[i].Update();
                }
                else
                {
                    s_Clients.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// <para>True if a network client is currently active.</para>
        /// </summary>
        public static bool active =>
            s_IsActive;

        /// <summary>
        /// <para>A list of all the active network clients in the current process.</para>
        /// </summary>
        public static List<NetworkClient> allClients =>
            s_Clients;

        /// <summary>
        /// <para>The NetworkConnection object this client is using.</para>
        /// </summary>
        public NetworkConnection connection =>
            this.m_Connection;

        /// <summary>
        /// <para>The registered network message handlers.</para>
        /// </summary>
        public Dictionary<short, NetworkMessageDelegate> handlers =>
            this.m_MessageHandlers.GetHandlers();

        internal int hostId =>
            this.m_ClientId;

        /// <summary>
        /// <para>The host topology that this client is using.</para>
        /// </summary>
        public HostTopology hostTopology =>
            this.m_HostTopology;

        /// <summary>
        /// <para>This gives the current connection status of the client.</para>
        /// </summary>
        public bool isConnected =>
            (this.m_AsyncConnect == ConnectState.Connected);

        /// <summary>
        /// <para>The class to use when creating new NetworkConnections.</para>
        /// </summary>
        public System.Type networkConnectionClass =>
            this.m_NetworkConnectionClass;

        /// <summary>
        /// <para>The number of QoS channels currently configured for this client.</para>
        /// </summary>
        public int numChannels =>
            this.m_HostTopology.DefaultConfig.ChannelCount;

        /// <summary>
        /// <para>This is obsolete. This information is now in the NetworkMigrationManager.</para>
        /// </summary>
        [Obsolete("Moved to NetworkMigrationManager.")]
        public PeerInfoMessage[] peers =>
            null;

        /// <summary>
        /// <para>The IP address of the server that this client is connected to.</para>
        /// </summary>
        public string serverIp =>
            this.m_ServerIp;

        /// <summary>
        /// <para>The port of the server that this client is connected to.</para>
        /// </summary>
        public int serverPort =>
            this.m_ServerPort;

        protected enum ConnectState
        {
            None,
            Resolving,
            Resolved,
            Connecting,
            Connected,
            Disconnected,
            Failed
        }
    }
}

