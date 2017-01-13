namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.Networking.Types;

    /// <summary>
    /// <para>The NetworkServer uses a NetworkServerSimple for basic network functionality and adds more game-like functionality.</para>
    /// </summary>
    public sealed class NetworkServer
    {
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache0;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache1;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache2;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache3;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache4;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache5;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache6;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache7;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache8;
        private const int k_RemoveListInterval = 100;
        private static bool m_DontListen;
        private HashSet<int> m_ExternalConnections;
        private bool m_LocalClientActive;
        private ULocalConnectionToClient m_LocalConnection = null;
        private List<NetworkConnection> m_LocalConnectionsFakeList = new List<NetworkConnection>();
        private float m_MaxDelay = 0.1f;
        private NetworkScene m_NetworkScene;
        private HashSet<NetworkInstanceId> m_RemoveList;
        private int m_RemoveListCount;
        private ServerSimpleWrapper m_SimpleServerSimple;
        internal static ushort maxPacketSize;
        private static bool s_Active;
        private static volatile NetworkServer s_Instance;
        private static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();
        private static object s_Sync = new UnityEngine.Object();

        private NetworkServer()
        {
            NetworkTransport.Init();
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer Created version " + UnityEngine.Networking.Version.Current);
            }
            this.m_RemoveList = new HashSet<NetworkInstanceId>();
            this.m_ExternalConnections = new HashSet<int>();
            this.m_NetworkScene = new NetworkScene();
            this.m_SimpleServerSimple = new ServerSimpleWrapper(this);
        }

        internal void ActivateLocalClientScene()
        {
            if (!this.m_LocalClientActive)
            {
                this.m_LocalClientActive = true;
                foreach (NetworkIdentity identity in objects.Values)
                {
                    if (!identity.isClient)
                    {
                        if (LogFilter.logDev)
                        {
                            Debug.Log(string.Concat(new object[] { "ActivateClientScene ", identity.netId, " ", identity.gameObject }));
                        }
                        ClientScene.SetLocalObject(identity.netId, identity.gameObject);
                        identity.OnStartClient();
                    }
                }
            }
        }

        /// <summary>
        /// <para>This accepts a network connection from another external source and adds it to the server.</para>
        /// </summary>
        /// <param name="conn">Network connection to add.</param>
        /// <returns>
        /// <para>True if added.</para>
        /// </returns>
        public static bool AddExternalConnection(NetworkConnection conn) => 
            instance.AddExternalConnectionInternal(conn);

        private bool AddExternalConnectionInternal(NetworkConnection conn)
        {
            if (conn.connectionId < 0)
            {
                return false;
            }
            if ((conn.connectionId < connections.Count) && (connections[conn.connectionId] != null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AddExternalConnection failed, already connection for id:" + conn.connectionId);
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("AddExternalConnection external connection " + conn.connectionId);
            }
            this.m_SimpleServerSimple.SetConnectionAtIndex(conn);
            this.m_ExternalConnections.Add(conn.connectionId);
            conn.InvokeHandlerNoData(0x20);
            return true;
        }

        internal int AddLocalClient(LocalClient localClient)
        {
            if (this.m_LocalConnectionsFakeList.Count != 0)
            {
                Debug.LogError("Local Connection already exists");
                return -1;
            }
            this.m_LocalConnection = new ULocalConnectionToClient(localClient);
            this.m_LocalConnection.connectionId = 0;
            this.m_SimpleServerSimple.SetConnectionAtIndex(this.m_LocalConnection);
            this.m_LocalConnectionsFakeList.Add(this.m_LocalConnection);
            this.m_LocalConnection.InvokeHandlerNoData(0x20);
            return 0;
        }

        /// <summary>
        /// <para>When an AddPlayer message handler has received a request from a player, the server calls this to associate the player object with the connection.</para>
        /// </summary>
        /// <param name="conn">Connection which is adding the player.</param>
        /// <param name="player">Player object spawned for the player.</param>
        /// <param name="playerControllerId">The player controller ID number as specified by client.</param>
        /// <returns>
        /// <para>True if player was added.</para>
        /// </returns>
        public static bool AddPlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId) => 
            instance.InternalAddPlayerForConnection(conn, player, playerControllerId);

        public static bool AddPlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId, NetworkHash128 assetId)
        {
            NetworkIdentity identity;
            if (GetNetworkIdentity(player, out identity))
            {
                identity.SetDynamicAssetId(assetId);
            }
            return instance.InternalAddPlayerForConnection(conn, player, playerControllerId);
        }

        public static NetworkClient BecomeHost(NetworkClient oldClient, int port, MatchInfo matchInfo, int oldConnectionId, PeerInfoMessage[] peers) => 
            instance.BecomeHostInternal(oldClient, port, matchInfo, oldConnectionId, peers);

        internal NetworkClient BecomeHostInternal(NetworkClient oldClient, int port, MatchInfo matchInfo, int oldConnectionId, PeerInfoMessage[] peers)
        {
            if (s_Active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("BecomeHost already a server.");
                }
                return null;
            }
            if (!NetworkClient.active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("BecomeHost NetworkClient not active.");
                }
                return null;
            }
            Configure(hostTopology);
            if (matchInfo == null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("BecomeHost Listen on " + port);
                }
                if (!Listen(port))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("BecomeHost bind failed.");
                    }
                    return null;
                }
            }
            else
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("BecomeHost match:" + matchInfo.networkId);
                }
                ListenRelay(matchInfo.address, matchInfo.port, matchInfo.networkId, Utility.GetSourceID(), matchInfo.nodeId);
            }
            foreach (NetworkIdentity identity in ClientScene.objects.Values)
            {
                if ((identity != null) && (identity.gameObject != null))
                {
                    NetworkIdentity.AddNetworkId(identity.netId.Value);
                    this.m_NetworkScene.SetLocalObject(identity.netId, identity.gameObject, false, false);
                    identity.OnStartServer(true);
                }
            }
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer BecomeHost done. oldConnectionId:" + oldConnectionId);
            }
            this.RegisterMessageHandlers();
            if (!NetworkClient.RemoveClient(oldClient) && LogFilter.logError)
            {
                Debug.LogError("BecomeHost failed to remove client");
            }
            if (LogFilter.logDev)
            {
                Debug.Log("BecomeHost localClient ready");
            }
            NetworkClient client2 = ClientScene.ReconnectLocalServer();
            ClientScene.Ready(client2.connection);
            ClientScene.SetReconnectId(oldConnectionId, peers);
            ClientScene.AddPlayer(ClientScene.readyConnection, 0);
            return client2;
        }

        private void CheckForNullObjects()
        {
            foreach (NetworkInstanceId id in objects.Keys)
            {
                NetworkIdentity identity = objects[id];
                if ((identity == null) || (identity.gameObject == null))
                {
                    this.m_RemoveList.Add(id);
                }
            }
            if (this.m_RemoveList.Count > 0)
            {
                foreach (NetworkInstanceId id2 in this.m_RemoveList)
                {
                    objects.Remove(id2);
                }
                this.m_RemoveList.Clear();
            }
        }

        private static bool CheckPlayerControllerIdForConnection(NetworkConnection conn, short playerControllerId)
        {
            if (playerControllerId < 0)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AddPlayer: playerControllerId of " + playerControllerId + " is negative");
                }
                return false;
            }
            if (playerControllerId > 0x20)
            {
                if (LogFilter.logError)
                {
                    Debug.Log(string.Concat(new object[] { "AddPlayer: playerControllerId of ", playerControllerId, " is too high. max is ", 0x20 }));
                }
                return false;
            }
            if ((playerControllerId > 0x10) && LogFilter.logWarn)
            {
                Debug.LogWarning("AddPlayer: playerControllerId of " + playerControllerId + " is unusually high");
            }
            return true;
        }

        /// <summary>
        /// <para>Clear all registered callback handlers.</para>
        /// </summary>
        public static void ClearHandlers()
        {
            instance.m_SimpleServerSimple.ClearHandlers();
        }

        /// <summary>
        /// <para>This clears all of the networked objects that the server is aware of. This can be required if a scene change deleted all of the objects without destroying them in the normal manner.</para>
        /// </summary>
        public static void ClearLocalObjects()
        {
            objects.Clear();
        }

        /// <summary>
        /// <para>Clears all registered spawn prefab and spawn handler functions for this server.</para>
        /// </summary>
        public static void ClearSpawners()
        {
            NetworkScene.ClearSpawners();
        }

        /// <summary>
        /// <para>This configures the transport layer settings for the server.</para>
        /// </summary>
        /// <param name="config">Transport layer confuration object.</param>
        /// <param name="maxConnections">The maximum number of client connections to allow.</param>
        /// <param name="topology">Transport layer topology object to use.</param>
        /// <returns>
        /// <para>True if successfully configured.</para>
        /// </returns>
        public static bool Configure(HostTopology topology) => 
            instance.m_SimpleServerSimple.Configure(topology);

        /// <summary>
        /// <para>This configures the transport layer settings for the server.</para>
        /// </summary>
        /// <param name="config">Transport layer confuration object.</param>
        /// <param name="maxConnections">The maximum number of client connections to allow.</param>
        /// <param name="topology">Transport layer topology object to use.</param>
        /// <returns>
        /// <para>True if successfully configured.</para>
        /// </returns>
        public static bool Configure(ConnectionConfig config, int maxConnections) => 
            instance.m_SimpleServerSimple.Configure(config, maxConnections);

        /// <summary>
        /// <para>Destroys this object and corresponding objects on all clients.</para>
        /// </summary>
        /// <param name="obj">Game object to destroy.</param>
        public static void Destroy(GameObject obj)
        {
            DestroyObject(obj);
        }

        private static void DestroyObject(GameObject obj)
        {
            if (obj == null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkServer DestroyObject is null");
                }
            }
            else
            {
                NetworkIdentity identity;
                if (GetNetworkIdentity(obj, out identity))
                {
                    DestroyObject(identity, true);
                }
            }
        }

        private static void DestroyObject(NetworkIdentity uv, bool destroyServerObject)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("DestroyObject instance:" + uv.netId);
            }
            if (objects.ContainsKey(uv.netId))
            {
                objects.Remove(uv.netId);
            }
            if (uv.clientAuthorityOwner != null)
            {
                uv.clientAuthorityOwner.RemoveOwnedObject(uv);
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 1, uv.assetId.ToString(), 1);
            ObjectDestroyMessage msg = new ObjectDestroyMessage {
                netId = uv.netId
            };
            SendToObservers(uv.gameObject, 1, msg);
            uv.ClearObservers();
            if (NetworkClient.active && instance.m_LocalClientActive)
            {
                uv.OnNetworkDestroy();
                ClientScene.SetLocalObject(msg.netId, null);
            }
            if (destroyServerObject)
            {
                UnityEngine.Object.Destroy(uv.gameObject);
            }
            uv.MarkForReset();
        }

        /// <summary>
        /// <para>This destroys all the player objects associated with a NetworkConnections on a server.</para>
        /// </summary>
        /// <param name="conn">The connections object to clean up for.</param>
        public static void DestroyPlayersForConnection(NetworkConnection conn)
        {
            if (conn.playerControllers.Count == 0)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Empty player list given to NetworkServer.Destroy(), nothing to do.");
                }
            }
            else
            {
                if (conn.clientOwnedObjects != null)
                {
                    HashSet<NetworkInstanceId> set = new HashSet<NetworkInstanceId>(conn.clientOwnedObjects);
                    foreach (NetworkInstanceId id in set)
                    {
                        GameObject obj2 = FindLocalObject(id);
                        if (obj2 != null)
                        {
                            DestroyObject(obj2);
                        }
                    }
                }
                for (int i = 0; i < conn.playerControllers.Count; i++)
                {
                    PlayerController controller = conn.playerControllers[i];
                    if (controller.IsValid)
                    {
                        if (controller.unetView != null)
                        {
                            DestroyObject(controller.unetView, true);
                        }
                        controller.gameObject = null;
                    }
                }
                conn.playerControllers.Clear();
            }
        }

        /// <summary>
        /// <para>Disconnect all currently connected clients.</para>
        /// </summary>
        public static void DisconnectAll()
        {
            instance.InternalDisconnectAll();
        }

        /// <summary>
        /// <para>This finds the local NetworkIdentity object with the specified network Id.</para>
        /// </summary>
        /// <param name="netId">The netId of the NetworkIdentity object to find.</param>
        /// <returns>
        /// <para>The game object that matches the netId.</para>
        /// </returns>
        public static GameObject FindLocalObject(NetworkInstanceId netId) => 
            instance.m_NetworkScene.FindLocalObject(netId);

        private static void FinishPlayerForConnection(NetworkConnection conn, NetworkIdentity uv, GameObject playerGameObject)
        {
            if (uv.netId.IsEmpty())
            {
                Spawn(playerGameObject);
            }
            OwnerMessage msg = new OwnerMessage {
                netId = uv.netId,
                playerControllerId = uv.playerControllerId
            };
            conn.Send(4, msg);
        }

        private void GenerateConnectError(int error)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Server Connect Error: " + error);
            }
            this.GenerateError(null, error);
        }

        private void GenerateDataError(NetworkConnection conn, int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Server Data Error: " + error2);
            }
            this.GenerateError(conn, error);
        }

        private void GenerateDisconnectError(NetworkConnection conn, int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError(string.Concat(new object[] { "UNet Server Disconnect Error: ", error2, " conn:[", conn, "]:", conn.connectionId }));
            }
            this.GenerateError(conn, error);
        }

        private void GenerateError(NetworkConnection conn, int error)
        {
            if (handlers.ContainsKey(0x22))
            {
                ErrorMessage message = new ErrorMessage {
                    errorCode = error
                };
                NetworkWriter writer = new NetworkWriter();
                message.Serialize(writer);
                NetworkReader reader = new NetworkReader(writer);
                conn.InvokeHandler(0x22, reader, 0);
            }
        }

        /// <summary>
        /// <para>Gets aggregate packet stats for all connections.</para>
        /// </summary>
        /// <returns>
        /// <para>Dictionary of msg types and packet statistics.</para>
        /// </returns>
        public static Dictionary<short, NetworkConnection.PacketStat> GetConnectionStats()
        {
            Dictionary<short, NetworkConnection.PacketStat> dictionary = new Dictionary<short, NetworkConnection.PacketStat>();
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    foreach (short num2 in connection.packetStats.Keys)
                    {
                        if (dictionary.ContainsKey(num2))
                        {
                            NetworkConnection.PacketStat stat = dictionary[num2];
                            stat.count += connection.packetStats[num2].count;
                            stat.bytes += connection.packetStats[num2].bytes;
                            dictionary[num2] = stat;
                        }
                        else
                        {
                            dictionary[num2] = new NetworkConnection.PacketStat(connection.packetStats[num2]);
                        }
                    }
                }
            }
            return dictionary;
        }

        private static bool GetNetworkIdentity(GameObject go, out NetworkIdentity view)
        {
            view = go.GetComponent<NetworkIdentity>();
            if (view == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("UNET failure. GameObject doesn't have NetworkIdentity.");
                }
                return false;
            }
            return true;
        }

        public static void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    int num2;
                    int num3;
                    connection.GetStatsIn(out num2, out num3);
                    numMsgs += num2;
                    numBytes += num3;
                }
            }
        }

        public static void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    connection.GetStatsOut(out num2, out num3, out num4, out num5);
                    numMsgs += num2;
                    numBufferedMsgs += num3;
                    numBytes += num4;
                    lastBufferedPerSecond += num5;
                }
            }
        }

        internal static void HideForConnection(NetworkIdentity uv, NetworkConnection conn)
        {
            ObjectDestroyMessage msg = new ObjectDestroyMessage {
                netId = uv.netId
            };
            conn.Send(13, msg);
        }

        internal bool InternalAddPlayerForConnection(NetworkConnection conn, GameObject playerGameObject, short playerControllerId)
        {
            NetworkIdentity identity;
            if (!GetNetworkIdentity(playerGameObject, out identity))
            {
                if (LogFilter.logError)
                {
                    Debug.Log("AddPlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to " + playerGameObject);
                }
                return false;
            }
            identity.Reset();
            if (!CheckPlayerControllerIdForConnection(conn, playerControllerId))
            {
                return false;
            }
            PlayerController playerController = null;
            GameObject gameObject = null;
            if (conn.GetPlayerController(playerControllerId, out playerController))
            {
                gameObject = playerController.gameObject;
            }
            if (gameObject != null)
            {
                if (LogFilter.logError)
                {
                    Debug.Log("AddPlayer: player object already exists for playerControllerId of " + playerControllerId);
                }
                return false;
            }
            PlayerController player = new PlayerController(playerGameObject, playerControllerId);
            conn.SetPlayerController(player);
            identity.SetConnectionToClient(conn, player.playerControllerId);
            SetClientReady(conn);
            if (!this.SetupLocalPlayerForConnection(conn, identity, player))
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Adding new playerGameObject object netId: ", playerGameObject.GetComponent<NetworkIdentity>().netId, " asset ID ", playerGameObject.GetComponent<NetworkIdentity>().assetId }));
                }
                FinishPlayerForConnection(conn, identity, playerGameObject);
                if (identity.localPlayerAuthority)
                {
                    identity.SetClientOwner(conn);
                }
            }
            return true;
        }

        internal void InternalDisconnectAll()
        {
            this.m_SimpleServerSimple.DisconnectAllConnections();
            if (this.m_LocalConnection != null)
            {
                this.m_LocalConnection.Disconnect();
                this.m_LocalConnection.Dispose();
                this.m_LocalConnection = null;
            }
            s_Active = false;
            this.m_LocalClientActive = false;
        }

        internal bool InternalListen(string ipAddress, int serverPort)
        {
            if (m_DontListen)
            {
                this.m_SimpleServerSimple.Initialize();
            }
            else if (!this.m_SimpleServerSimple.Listen(ipAddress, serverPort))
            {
                return false;
            }
            maxPacketSize = hostTopology.DefaultConfig.PacketSize;
            s_Active = true;
            this.RegisterMessageHandlers();
            return true;
        }

        private void InternalListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId)
        {
            this.m_SimpleServerSimple.ListenRelay(relayIp, relayPort, netGuid, sourceId, nodeId);
            s_Active = true;
            this.RegisterMessageHandlers();
        }

        internal bool InternalReplacePlayerForConnection(NetworkConnection conn, GameObject playerGameObject, short playerControllerId)
        {
            NetworkIdentity identity;
            PlayerController controller;
            if (!GetNetworkIdentity(playerGameObject, out identity))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReplacePlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to " + playerGameObject);
                }
                return false;
            }
            if (!CheckPlayerControllerIdForConnection(conn, playerControllerId))
            {
                return false;
            }
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer ReplacePlayer");
            }
            if (conn.GetPlayerController(playerControllerId, out controller))
            {
                controller.unetView.SetNotLocalPlayer();
                controller.unetView.ClearClientOwner();
            }
            PlayerController player = new PlayerController(playerGameObject, playerControllerId);
            conn.SetPlayerController(player);
            identity.SetConnectionToClient(conn, player.playerControllerId);
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer ReplacePlayer setup local");
            }
            if (!this.SetupLocalPlayerForConnection(conn, identity, player))
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Replacing playerGameObject object netId: ", playerGameObject.GetComponent<NetworkIdentity>().netId, " asset ID ", playerGameObject.GetComponent<NetworkIdentity>().assetId }));
                }
                FinishPlayerForConnection(conn, identity, playerGameObject);
                if (identity.localPlayerAuthority)
                {
                    identity.SetClientOwner(conn);
                }
            }
            return true;
        }

        internal void InternalSetClientNotReady(NetworkConnection conn)
        {
            if (conn.isReady)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("PlayerNotReady " + conn);
                }
                conn.isReady = false;
                conn.RemoveObservers();
                NotReadyMessage msg = new NotReadyMessage();
                conn.Send(0x24, msg);
            }
        }

        private void InternalSetMaxDelay(float seconds)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    connection.SetMaxDelay(seconds);
                }
            }
            this.m_MaxDelay = seconds;
        }

        internal void InternalUpdate()
        {
            this.m_SimpleServerSimple.Update();
            if (m_DontListen)
            {
                this.m_SimpleServerSimple.UpdateConnections();
            }
            this.UpdateServerObjects();
        }

        internal bool InvokeBytes(ULocalConnectionToServer conn, byte[] buffer, int numBytes, int channelId)
        {
            NetworkReader reader = new NetworkReader(buffer);
            reader.ReadInt16();
            short key = reader.ReadInt16();
            if (handlers.ContainsKey(key) && (this.m_LocalConnection != null))
            {
                this.m_LocalConnection.InvokeHandler(key, reader, channelId);
                return true;
            }
            return false;
        }

        internal bool InvokeHandlerOnServer(ULocalConnectionToServer conn, short msgType, MessageBase msg, int channelId)
        {
            if (handlers.ContainsKey(msgType) && (this.m_LocalConnection != null))
            {
                NetworkWriter writer = new NetworkWriter();
                msg.Serialize(writer);
                NetworkReader reader = new NetworkReader(writer);
                this.m_LocalConnection.InvokeHandler(msgType, reader, channelId);
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError(string.Concat(new object[] { "Local invoke: Failed to find local connection to invoke handler on [connectionId=", conn.connectionId, "] for MsgId:", msgType }));
            }
            return false;
        }

        /// <summary>
        /// <para>Start the server on the given port number. Note that if a match has been created, this will listen using the Relay server instead of a local socket.</para>
        /// </summary>
        /// <param name="ipAddress">The IP address to bind to (optional).</param>
        /// <param name="serverPort">Listen port number.</param>
        /// <returns>
        /// <para>True if listen succeeded.</para>
        /// </returns>
        public static bool Listen(int serverPort) => 
            instance.InternalListen(null, serverPort);

        /// <summary>
        /// <para>Start the server on the given port number. Note that if a match has been created, this will listen using the Relay server instead of a local socket.</para>
        /// </summary>
        /// <param name="ipAddress">The IP address to bind to (optional).</param>
        /// <param name="serverPort">Listen port number.</param>
        /// <returns>
        /// <para>True if listen succeeded.</para>
        /// </returns>
        public static bool Listen(string ipAddress, int serverPort) => 
            instance.InternalListen(ipAddress, serverPort);

        public static bool Listen(MatchInfo matchInfo, int listenPort)
        {
            if (!matchInfo.usingRelay)
            {
                return instance.InternalListen(null, listenPort);
            }
            instance.InternalListenRelay(matchInfo.address, matchInfo.port, matchInfo.networkId, Utility.GetSourceID(), matchInfo.nodeId);
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
        public static void ListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId)
        {
            instance.InternalListenRelay(relayIp, relayPort, netGuid, sourceId, nodeId);
        }

        private static void OnClientReadyMessage(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Default handler for ready message from " + netMsg.conn);
            }
            SetClientReady(netMsg.conn);
        }

        private static void OnCommandMessage(NetworkMessage netMsg)
        {
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            GameObject obj2 = FindLocalObject(netId);
            if (obj2 == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Instance not found when handling Command message [netId=" + netId + "]");
                }
            }
            else
            {
                NetworkIdentity component = obj2.GetComponent<NetworkIdentity>();
                if (component == null)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("NetworkIdentity deleted when handling Command message [netId=" + netId + "]");
                    }
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < netMsg.conn.playerControllers.Count; i++)
                    {
                        PlayerController controller = netMsg.conn.playerControllers[i];
                        if ((controller.gameObject != null) && (controller.gameObject.GetComponent<NetworkIdentity>().netId == component.netId))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag && (component.clientAuthorityOwner != netMsg.conn))
                    {
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning("Command for object without authority [netId=" + netId + "]");
                        }
                    }
                    else
                    {
                        if (LogFilter.logDev)
                        {
                            Debug.Log(string.Concat(new object[] { "OnCommandMessage for netId=", netId, " conn=", netMsg.conn }));
                        }
                        component.HandleCommand(cmdHash, netMsg.reader);
                    }
                }
            }
        }

        private void OnConnected(NetworkConnection conn)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Server accepted client:" + conn.connectionId);
            }
            conn.SetMaxDelay(this.m_MaxDelay);
            conn.InvokeHandlerNoData(0x20);
            SendCrc(conn);
        }

        private void OnData(NetworkConnection conn, int receivedSize, int channelId)
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x1d, "msg", 1);
            conn.TransportRecieve(this.m_SimpleServerSimple.messageBuffer, receivedSize, channelId);
        }

        private void OnDisconnected(NetworkConnection conn)
        {
            conn.InvokeHandlerNoData(0x21);
            for (int i = 0; i < conn.playerControllers.Count; i++)
            {
                if ((conn.playerControllers[i].gameObject != null) && LogFilter.logWarn)
                {
                    Debug.LogWarning("Player not destroyed when connection disconnected.");
                }
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("Server lost client:" + conn.connectionId);
            }
            conn.RemoveObservers();
            conn.Dispose();
        }

        private static void OnRemovePlayerMessage(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<RemovePlayerMessage>(s_RemovePlayerMessage);
            PlayerController playerController = null;
            netMsg.conn.GetPlayerController(s_RemovePlayerMessage.playerControllerId, out playerController);
            if (playerController != null)
            {
                netMsg.conn.RemovePlayerController(s_RemovePlayerMessage.playerControllerId);
                Destroy(playerController.gameObject);
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Received remove player message but could not find the player ID: " + s_RemovePlayerMessage.playerControllerId);
            }
        }

        /// <summary>
        /// <para>Register a handler for a particular message type.</para>
        /// </summary>
        /// <param name="msgType">Message type number.</param>
        /// <param name="handler">Function handler which will be invoked for when this message type is received.</param>
        public static void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            instance.m_SimpleServerSimple.RegisterHandler(msgType, handler);
        }

        internal void RegisterMessageHandlers()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new NetworkMessageDelegate(NetworkServer.OnClientReadyMessage);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(0x23, <>f__mg$cache0);
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new NetworkMessageDelegate(NetworkServer.OnCommandMessage);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(5, <>f__mg$cache1);
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new NetworkMessageDelegate(NetworkTransform.HandleTransform);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(6, <>f__mg$cache2);
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new NetworkMessageDelegate(NetworkTransformChild.HandleChildTransform);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(0x10, <>f__mg$cache3);
            if (<>f__mg$cache4 == null)
            {
                <>f__mg$cache4 = new NetworkMessageDelegate(NetworkServer.OnRemovePlayerMessage);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(0x26, <>f__mg$cache4);
            if (<>f__mg$cache5 == null)
            {
                <>f__mg$cache5 = new NetworkMessageDelegate(NetworkAnimator.OnAnimationServerMessage);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(40, <>f__mg$cache5);
            if (<>f__mg$cache6 == null)
            {
                <>f__mg$cache6 = new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersServerMessage);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(0x29, <>f__mg$cache6);
            if (<>f__mg$cache7 == null)
            {
                <>f__mg$cache7 = new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerServerMessage);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(0x2a, <>f__mg$cache7);
            if (<>f__mg$cache8 == null)
            {
                <>f__mg$cache8 = new NetworkMessageDelegate(NetworkConnection.OnFragment);
            }
            this.m_SimpleServerSimple.RegisterHandlerSafe(0x11, <>f__mg$cache8);
            maxPacketSize = hostTopology.DefaultConfig.PacketSize;
        }

        /// <summary>
        /// <para>This removes an external connection added with AddExternalConnection().</para>
        /// </summary>
        /// <param name="connectionId">The id of the connection to remove.</param>
        public static void RemoveExternalConnection(int connectionId)
        {
            instance.RemoveExternalConnectionInternal(connectionId);
        }

        private bool RemoveExternalConnectionInternal(int connectionId)
        {
            if (!this.m_ExternalConnections.Contains(connectionId))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveExternalConnection failed, no connection for id:" + connectionId);
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("RemoveExternalConnection external connection " + connectionId);
            }
            NetworkConnection connection = this.m_SimpleServerSimple.FindConnection(connectionId);
            if (connection != null)
            {
                connection.RemoveObservers();
            }
            this.m_SimpleServerSimple.RemoveConnectionAtIndex(connectionId);
            return true;
        }

        internal void RemoveLocalClient(NetworkConnection localClientConnection)
        {
            for (int i = 0; i < this.m_LocalConnectionsFakeList.Count; i++)
            {
                if (this.m_LocalConnectionsFakeList[i].connectionId == localClientConnection.connectionId)
                {
                    this.m_LocalConnectionsFakeList.RemoveAt(i);
                    break;
                }
            }
            if (this.m_LocalConnection != null)
            {
                this.m_LocalConnection.Disconnect();
                this.m_LocalConnection.Dispose();
                this.m_LocalConnection = null;
            }
            this.m_LocalClientActive = false;
            this.m_SimpleServerSimple.RemoveConnectionAtIndex(0);
        }

        /// <summary>
        /// <para>This replaces the player object for a connection with a different player object. The old player object is not destroyed.</para>
        /// </summary>
        /// <param name="conn">Connection which is adding the player.</param>
        /// <param name="player">Player object spawned for the player.</param>
        /// <param name="playerControllerId">The player controller ID number as specified by client.</param>
        /// <returns>
        /// <para>True if player was replaced.</para>
        /// </returns>
        public static bool ReplacePlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId) => 
            instance.InternalReplacePlayerForConnection(conn, player, playerControllerId);

        public static bool ReplacePlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId, NetworkHash128 assetId)
        {
            NetworkIdentity identity;
            if (GetNetworkIdentity(player, out identity))
            {
                identity.SetDynamicAssetId(assetId);
            }
            return instance.InternalReplacePlayerForConnection(conn, player, playerControllerId);
        }

        /// <summary>
        /// <para>Reset the NetworkServer singleton.</para>
        /// </summary>
        public static void Reset()
        {
            NetworkDetailStats.ResetAll();
            NetworkTransport.Shutdown();
            NetworkTransport.Init();
            s_Instance = null;
            s_Active = false;
        }

        /// <summary>
        /// <para>Resets the packet stats on all connections.</para>
        /// </summary>
        public static void ResetConnectionStats()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    connection.ResetStats();
                }
            }
        }

        /// <summary>
        /// <para>Sends a network message to all connected clients on a specified transport layer QoS channel.</para>
        /// </summary>
        /// <param name="msgType">The message id.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="channelId">The transport layer channel to use.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public static bool SendByChannelToAll(short msgType, MessageBase msg, int channelId)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendByChannelToAll id:" + msgType);
            }
            bool flag = true;
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    flag &= connection.SendByChannel(msgType, msg, channelId);
                }
            }
            return flag;
        }

        /// <summary>
        /// <para>Sends a network message to all connected clients that are "ready" on a specified transport layer QoS channel.</para>
        /// </summary>
        /// <param name="contextObj">An object to use for context when calculating object visibility. If null, then the message is sent to all ready clients.</param>
        /// <param name="msgType">The message id.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="channelId">The transport layer channel to send on.</param>
        /// <returns>
        /// <para>True if the message was sent.</para>
        /// </returns>
        public static bool SendByChannelToReady(GameObject contextObj, short msgType, MessageBase msg, int channelId)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendByChannelToReady msgType:" + msgType);
            }
            if (contextObj == null)
            {
                for (int j = 0; j < connections.Count; j++)
                {
                    NetworkConnection connection = connections[j];
                    if ((connection != null) && connection.isReady)
                    {
                        connection.SendByChannel(msgType, msg, channelId);
                    }
                }
                return true;
            }
            bool flag2 = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection2 = component.observers[i];
                if (connection2.isReady)
                {
                    flag2 &= connection2.SendByChannel(msgType, msg, channelId);
                }
            }
            return flag2;
        }

        /// <summary>
        /// <para>This sends an array of bytes to a specific player.</para>
        /// </summary>
        /// <param name="player">The player to send the bytes to.</param>
        /// <param name="buffer">Array of bytes to send.</param>
        /// <param name="numBytes">Size of array.</param>
        /// <param name="channelId">Transport layer channel id to send bytes on.</param>
        public static void SendBytesToPlayer(GameObject player, byte[] buffer, int numBytes, int channelId)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    for (int j = 0; j < connection.playerControllers.Count; j++)
                    {
                        if (connection.playerControllers[j].IsValid && (connection.playerControllers[j].gameObject == player))
                        {
                            connection.SendBytes(buffer, numBytes, channelId);
                            break;
                        }
                    }
                }
            }
        }

        public static void SendBytesToReady(GameObject contextObj, byte[] buffer, int numBytes, int channelId)
        {
            if (contextObj == null)
            {
                bool flag = true;
                for (int i = 0; i < connections.Count; i++)
                {
                    NetworkConnection connection = connections[i];
                    if (((connection != null) && connection.isReady) && !connection.SendBytes(buffer, numBytes, channelId))
                    {
                        flag = false;
                    }
                }
                if (!flag && LogFilter.logWarn)
                {
                    Debug.LogWarning("SendBytesToReady failed");
                }
            }
            else
            {
                NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
                try
                {
                    bool flag2 = true;
                    int count = component.observers.Count;
                    for (int j = 0; j < count; j++)
                    {
                        NetworkConnection connection2 = component.observers[j];
                        if (connection2.isReady && !connection2.SendBytes(buffer, numBytes, channelId))
                        {
                            flag2 = false;
                        }
                    }
                    if (!flag2 && LogFilter.logWarn)
                    {
                        Debug.LogWarning("SendBytesToReady failed for " + contextObj);
                    }
                }
                catch (NullReferenceException)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("SendBytesToReady object " + contextObj + " has not been spawned");
                    }
                }
            }
        }

        private static void SendCrc(NetworkConnection targetConnection)
        {
            if ((NetworkCRC.singleton != null) && NetworkCRC.scriptCRCCheck)
            {
                CRCMessage msg = new CRCMessage();
                List<CRCMessageEntry> list = new List<CRCMessageEntry>();
                foreach (string str in NetworkCRC.singleton.scripts.Keys)
                {
                    CRCMessageEntry item = new CRCMessageEntry {
                        name = str,
                        channel = (byte) NetworkCRC.singleton.scripts[str]
                    };
                    list.Add(item);
                }
                msg.scripts = list.ToArray();
                targetConnection.Send(14, msg);
            }
        }

        /// <summary>
        /// <para>This is obsolete. This functionality is now part of the NetworkMigrationManager.</para>
        /// </summary>
        /// <param name="targetConnection">Connection to send peer info to.</param>
        [Obsolete("moved to NetworkMigrationManager")]
        public void SendNetworkInfo(NetworkConnection targetConnection)
        {
        }

        internal void SendSpawnMessage(NetworkIdentity uv, NetworkConnection conn)
        {
            if (!uv.serverOnly)
            {
                if (uv.sceneId.IsEmpty())
                {
                    ObjectSpawnMessage msg = new ObjectSpawnMessage {
                        netId = uv.netId,
                        assetId = uv.assetId,
                        position = uv.transform.position,
                        rotation = uv.transform.rotation
                    };
                    NetworkWriter writer = new NetworkWriter();
                    uv.UNetSerializeAllVars(writer);
                    if (writer.Position > 0)
                    {
                        msg.payload = writer.ToArray();
                    }
                    if (conn != null)
                    {
                        conn.Send(3, msg);
                    }
                    else
                    {
                        SendToReady(uv.gameObject, 3, msg);
                    }
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 3, uv.assetId.ToString(), 1);
                }
                else
                {
                    ObjectSpawnSceneMessage message2 = new ObjectSpawnSceneMessage {
                        netId = uv.netId,
                        sceneId = uv.sceneId,
                        position = uv.transform.position
                    };
                    NetworkWriter writer2 = new NetworkWriter();
                    uv.UNetSerializeAllVars(writer2);
                    if (writer2.Position > 0)
                    {
                        message2.payload = writer2.ToArray();
                    }
                    if (conn != null)
                    {
                        conn.Send(10, message2);
                    }
                    else
                    {
                        SendToReady(uv.gameObject, 3, message2);
                    }
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 10, "sceneId", 1);
                }
            }
        }

        public static bool SendToAll(short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendToAll msgType:" + msgType);
            }
            bool flag = true;
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    flag &= connection.Send(msgType, msg);
                }
            }
            return flag;
        }

        public static void SendToClient(int connectionId, short msgType, MessageBase msg)
        {
            if (connectionId < connections.Count)
            {
                NetworkConnection connection = connections[connectionId];
                if (connection != null)
                {
                    connection.Send(msgType, msg);
                    return;
                }
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Failed to send message to connection ID '" + connectionId + ", not found in connection list");
            }
        }

        public static void SendToClientOfPlayer(GameObject player, short msgType, MessageBase msg)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    for (int j = 0; j < connection.playerControllers.Count; j++)
                    {
                        if (connection.playerControllers[j].IsValid && (connection.playerControllers[j].gameObject == player))
                        {
                            connection.Send(msgType, msg);
                            return;
                        }
                    }
                }
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Failed to send message to player object '" + player.name + ", not found in connection list");
            }
        }

        private static bool SendToObservers(GameObject contextObj, short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendToObservers id:" + msgType);
            }
            bool flag = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            if ((component == null) || (component.observers == null))
            {
                return false;
            }
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection = component.observers[i];
                flag &= connection.Send(msgType, msg);
            }
            return flag;
        }

        public static bool SendToReady(GameObject contextObj, short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendToReady id:" + msgType);
            }
            if (contextObj == null)
            {
                for (int j = 0; j < connections.Count; j++)
                {
                    NetworkConnection connection = connections[j];
                    if ((connection != null) && connection.isReady)
                    {
                        connection.Send(msgType, msg);
                    }
                }
                return true;
            }
            bool flag2 = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            if ((component == null) || (component.observers == null))
            {
                return false;
            }
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection2 = component.observers[i];
                if (connection2.isReady)
                {
                    flag2 &= connection2.Send(msgType, msg);
                }
            }
            return flag2;
        }

        public static bool SendUnreliableToAll(short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendUnreliableToAll msgType:" + msgType);
            }
            bool flag = true;
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    flag &= connection.SendUnreliable(msgType, msg);
                }
            }
            return flag;
        }

        public static bool SendUnreliableToReady(GameObject contextObj, short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendUnreliableToReady id:" + msgType);
            }
            if (contextObj == null)
            {
                for (int j = 0; j < connections.Count; j++)
                {
                    NetworkConnection connection = connections[j];
                    if ((connection != null) && connection.isReady)
                    {
                        connection.SendUnreliable(msgType, msg);
                    }
                }
                return true;
            }
            bool flag2 = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection2 = component.observers[i];
                if (connection2.isReady)
                {
                    flag2 &= connection2.SendUnreliable(msgType, msg);
                }
            }
            return flag2;
        }

        public static void SendWriterToReady(GameObject contextObj, NetworkWriter writer, int channelId)
        {
            if (writer.AsArraySegment().Count > 0x7fff)
            {
                throw new UnityException("NetworkWriter used buffer is too big!");
            }
            SendBytesToReady(contextObj, writer.AsArraySegment().Array, writer.AsArraySegment().Count, channelId);
        }

        /// <summary>
        /// <para>Marks all connected clients as no longer ready.</para>
        /// </summary>
        public static void SetAllClientsNotReady()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection conn = connections[i];
                if (conn != null)
                {
                    SetClientNotReady(conn);
                }
            }
        }

        /// <summary>
        /// <para>Sets the client of the connection to be not-ready.</para>
        /// </summary>
        /// <param name="conn">The connection of the client to make not ready.</param>
        public static void SetClientNotReady(NetworkConnection conn)
        {
            instance.InternalSetClientNotReady(conn);
        }

        /// <summary>
        /// <para>Sets the client to be ready.</para>
        /// </summary>
        /// <param name="conn">The connection of the client to make ready.</param>
        public static void SetClientReady(NetworkConnection conn)
        {
            instance.SetClientReadyInternal(conn);
        }

        internal void SetClientReadyInternal(NetworkConnection conn)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("SetClientReadyInternal for conn:" + conn.connectionId);
            }
            if (conn.isReady)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("SetClientReady conn " + conn.connectionId + " already ready");
                }
            }
            else
            {
                if ((conn.playerControllers.Count == 0) && LogFilter.logDebug)
                {
                    Debug.LogWarning("Ready with no player object");
                }
                conn.isReady = true;
                if (conn is ULocalConnectionToClient)
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("NetworkServer Ready handling ULocalConnectionToClient");
                    }
                    foreach (NetworkIdentity identity in objects.Values)
                    {
                        if ((identity != null) && (identity.gameObject != null))
                        {
                            if (identity.OnCheckObserver(conn))
                            {
                                identity.AddObserver(conn);
                            }
                            if (!identity.isClient)
                            {
                                if (LogFilter.logDev)
                                {
                                    Debug.Log("LocalClient.SetSpawnObject calling OnStartClient");
                                }
                                identity.OnStartClient();
                            }
                        }
                    }
                }
                else
                {
                    if (LogFilter.logDebug)
                    {
                        Debug.Log(string.Concat(new object[] { "Spawning ", objects.Count, " objects for conn ", conn.connectionId }));
                    }
                    ObjectSpawnFinishedMessage msg = new ObjectSpawnFinishedMessage {
                        state = 0
                    };
                    conn.Send(12, msg);
                    foreach (NetworkIdentity identity2 in objects.Values)
                    {
                        if (identity2 == null)
                        {
                            if (LogFilter.logWarn)
                            {
                                Debug.LogWarning("Invalid object found in server local object list (null NetworkIdentity).");
                            }
                        }
                        else if (identity2.gameObject.activeSelf)
                        {
                            if (LogFilter.logDebug)
                            {
                                Debug.Log(string.Concat(new object[] { "Sending spawn message for current server objects name='", identity2.gameObject.name, "' netId=", identity2.netId }));
                            }
                            if (identity2.OnCheckObserver(conn))
                            {
                                identity2.AddObserver(conn);
                            }
                        }
                    }
                    msg.state = 1;
                    conn.Send(12, msg);
                }
            }
        }

        internal void SetLocalObjectOnServer(NetworkInstanceId netId, GameObject obj)
        {
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "SetLocalObjectOnServer ", netId, " ", obj }));
            }
            this.m_NetworkScene.SetLocalObject(netId, obj, false, true);
        }

        public static void SetNetworkConnectionClass<T>() where T: NetworkConnection
        {
            instance.m_SimpleServerSimple.SetNetworkConnectionClass<T>();
        }

        private bool SetupLocalPlayerForConnection(NetworkConnection conn, NetworkIdentity uv, PlayerController newPlayerController)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer SetupLocalPlayerForConnection netID:" + uv.netId);
            }
            ULocalConnectionToClient client = conn as ULocalConnectionToClient;
            if (client != null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkServer AddPlayer handling ULocalConnectionToClient");
                }
                if (uv.netId.IsEmpty())
                {
                    uv.OnStartServer(true);
                }
                uv.RebuildObservers(true);
                this.SendSpawnMessage(uv, null);
                client.localClient.AddLocalPlayer(newPlayerController);
                uv.SetClientOwner(conn);
                uv.ForceAuthority(true);
                uv.SetLocalPlayer(newPlayerController.playerControllerId);
                return true;
            }
            return false;
        }

        internal static void ShowForConnection(NetworkIdentity uv, NetworkConnection conn)
        {
            if (conn.isReady)
            {
                instance.SendSpawnMessage(uv, conn);
            }
        }

        /// <summary>
        /// <para>This shuts down the server and disconnects all clients.</para>
        /// </summary>
        public static void Shutdown()
        {
            if (s_Instance != null)
            {
                s_Instance.InternalDisconnectAll();
                if (!m_DontListen)
                {
                    s_Instance.m_SimpleServerSimple.Stop();
                }
                s_Instance = null;
            }
            m_DontListen = false;
            s_Active = false;
        }

        /// <summary>
        /// <para>Spawn the given game object on all clients which are ready.</para>
        /// </summary>
        /// <param name="obj">Game object with NetworkIdentity to spawn.</param>
        public static void Spawn(GameObject obj)
        {
            instance.SpawnObject(obj);
        }

        public static void Spawn(GameObject obj, NetworkHash128 assetId)
        {
            NetworkIdentity identity;
            if (GetNetworkIdentity(obj, out identity))
            {
                identity.SetDynamicAssetId(assetId);
            }
            instance.SpawnObject(obj);
        }

        internal void SpawnObject(GameObject obj)
        {
            if (!active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("SpawnObject for " + obj + ", NetworkServer is not active. Cannot spawn objects without an active server.");
                }
            }
            else
            {
                NetworkIdentity identity;
                if (!GetNetworkIdentity(obj, out identity))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "SpawnObject ", obj, " has no NetworkIdentity. Please add a NetworkIdentity to ", obj }));
                    }
                }
                else
                {
                    identity.Reset();
                    identity.OnStartServer(false);
                    if (LogFilter.logDebug)
                    {
                        Debug.Log(string.Concat(new object[] { "SpawnObject instance ID ", identity.netId, " asset ID ", identity.assetId }));
                    }
                    identity.RebuildObservers(true);
                }
            }
        }

        /// <summary>
        /// <para>This causes NetworkIdentity objects in a scene to be spawned on a server.</para>
        /// </summary>
        /// <returns>
        /// <para>Success if objects where spawned.</para>
        /// </returns>
        public static bool SpawnObjects()
        {
            if (active)
            {
                NetworkIdentity[] identityArray = UnityEngine.Resources.FindObjectsOfTypeAll<NetworkIdentity>();
                for (int i = 0; i < identityArray.Length; i++)
                {
                    NetworkIdentity identity = identityArray[i];
                    if (((identity.gameObject.hideFlags != HideFlags.NotEditable) && (identity.gameObject.hideFlags != HideFlags.HideAndDontSave)) && !identity.sceneId.IsEmpty())
                    {
                        if (LogFilter.logDebug)
                        {
                            Debug.Log(string.Concat(new object[] { "SpawnObjects sceneId:", identity.sceneId, " name:", identity.gameObject.name }));
                        }
                        identity.gameObject.SetActive(true);
                    }
                }
                for (int j = 0; j < identityArray.Length; j++)
                {
                    NetworkIdentity identity2 = identityArray[j];
                    if ((((identity2.gameObject.hideFlags != HideFlags.NotEditable) && (identity2.gameObject.hideFlags != HideFlags.HideAndDontSave)) && (!identity2.sceneId.IsEmpty() && !identity2.isServer)) && (identity2.gameObject != null))
                    {
                        Spawn(identity2.gameObject);
                        identity2.ForceAuthority(true);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// <para>This spawns an object like NetworkServer.Spawn() but also assigns Client Authority to the specified client.</para>
        /// </summary>
        /// <param name="obj">The object to spawn.</param>
        /// <param name="player">The player object to set Client Authority to.</param>
        /// <param name="assetId">The assetId of the object to spawn. Used for custom spawn handlers.</param>
        /// <param name="conn">The connection to set Client Authority to.</param>
        /// <returns>
        /// <para>True if the object was spawned.</para>
        /// </returns>
        public static bool SpawnWithClientAuthority(GameObject obj, GameObject player)
        {
            NetworkIdentity component = player.GetComponent<NetworkIdentity>();
            if (component == null)
            {
                Debug.LogError("SpawnWithClientAuthority player object has no NetworkIdentity");
                return false;
            }
            if (component.connectionToClient == null)
            {
                Debug.LogError("SpawnWithClientAuthority player object is not a player.");
                return false;
            }
            return SpawnWithClientAuthority(obj, component.connectionToClient);
        }

        /// <summary>
        /// <para>This spawns an object like NetworkServer.Spawn() but also assigns Client Authority to the specified client.</para>
        /// </summary>
        /// <param name="obj">The object to spawn.</param>
        /// <param name="player">The player object to set Client Authority to.</param>
        /// <param name="assetId">The assetId of the object to spawn. Used for custom spawn handlers.</param>
        /// <param name="conn">The connection to set Client Authority to.</param>
        /// <returns>
        /// <para>True if the object was spawned.</para>
        /// </returns>
        public static bool SpawnWithClientAuthority(GameObject obj, NetworkConnection conn)
        {
            Spawn(obj);
            NetworkIdentity component = obj.GetComponent<NetworkIdentity>();
            if ((component == null) || !component.isServer)
            {
                return false;
            }
            return component.AssignClientAuthority(conn);
        }

        /// <summary>
        /// <para>This spawns an object like NetworkServer.Spawn() but also assigns Client Authority to the specified client.</para>
        /// </summary>
        /// <param name="obj">The object to spawn.</param>
        /// <param name="player">The player object to set Client Authority to.</param>
        /// <param name="assetId">The assetId of the object to spawn. Used for custom spawn handlers.</param>
        /// <param name="conn">The connection to set Client Authority to.</param>
        /// <returns>
        /// <para>True if the object was spawned.</para>
        /// </returns>
        public static bool SpawnWithClientAuthority(GameObject obj, NetworkHash128 assetId, NetworkConnection conn)
        {
            Spawn(obj, assetId);
            NetworkIdentity component = obj.GetComponent<NetworkIdentity>();
            if ((component == null) || !component.isServer)
            {
                return false;
            }
            return component.AssignClientAuthority(conn);
        }

        /// <summary>
        /// <para>Unregisters a handler for a particular message type.</para>
        /// </summary>
        /// <param name="msgType">The message type to remove the handler for.</param>
        public static void UnregisterHandler(short msgType)
        {
            instance.m_SimpleServerSimple.UnregisterHandler(msgType);
        }

        /// <summary>
        /// <para>This takes an object that has been spawned and un-spawns it.</para>
        /// </summary>
        /// <param name="obj">The spawned object to be unspawned.</param>
        public static void UnSpawn(GameObject obj)
        {
            UnSpawnObject(obj);
        }

        private static void UnSpawnObject(GameObject obj)
        {
            if (obj == null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkServer UnspawnObject is null");
                }
            }
            else
            {
                NetworkIdentity identity;
                if (GetNetworkIdentity(obj, out identity))
                {
                    UnSpawnObject(identity);
                }
            }
        }

        private static void UnSpawnObject(NetworkIdentity uv)
        {
            DestroyObject(uv, false);
        }

        internal static void Update()
        {
            if (s_Instance != null)
            {
                s_Instance.InternalUpdate();
            }
        }

        private void UpdateServerObjects()
        {
            foreach (NetworkIdentity identity in objects.Values)
            {
                try
                {
                    identity.UNetUpdate();
                }
                catch (NullReferenceException)
                {
                }
                catch (MissingReferenceException)
                {
                }
            }
            if ((this.m_RemoveListCount++ % 100) == 0)
            {
                this.CheckForNullObjects();
            }
        }

        /// <summary>
        /// <para>Checks if the server has been started.</para>
        /// </summary>
        public static bool active =>
            s_Active;

        /// <summary>
        /// <para>A list of all the current connections from clients.</para>
        /// </summary>
        public static ReadOnlyCollection<NetworkConnection> connections =>
            instance.m_SimpleServerSimple.connections;

        /// <summary>
        /// <para>If you enable this, the server will not listen for incoming connections on the regular network port.</para>
        /// </summary>
        public static bool dontListen
        {
            get => 
                m_DontListen;
            set
            {
                m_DontListen = value;
            }
        }

        /// <summary>
        /// <para>Dictionary of the message handlers registered with the server.</para>
        /// </summary>
        public static Dictionary<short, NetworkMessageDelegate> handlers =>
            instance.m_SimpleServerSimple.handlers;

        /// <summary>
        /// <para>The host topology that the server is using.</para>
        /// </summary>
        public static HostTopology hostTopology =>
            instance.m_SimpleServerSimple.hostTopology;

        internal static NetworkServer instance
        {
            get
            {
                if (s_Instance == null)
                {
                    object obj2 = s_Sync;
                    lock (obj2)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new NetworkServer();
                        }
                    }
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// <para>The port that the server is listening on.</para>
        /// </summary>
        public static int listenPort =>
            instance.m_SimpleServerSimple.listenPort;

        /// <summary>
        /// <para>True is a local client is currently active on the server.</para>
        /// </summary>
        public static bool localClientActive =>
            instance.m_LocalClientActive;

        /// <summary>
        /// <para>A list of local connections on the server.</para>
        /// </summary>
        public static List<NetworkConnection> localConnections =>
            instance.m_LocalConnectionsFakeList;

        /// <summary>
        /// <para>The maximum delay before sending packets on connections.</para>
        /// </summary>
        public static float maxDelay
        {
            get => 
                instance.m_MaxDelay;
            set
            {
                instance.InternalSetMaxDelay(value);
            }
        }

        /// <summary>
        /// <para>The class to be used when creating new network connections.</para>
        /// </summary>
        public static System.Type networkConnectionClass =>
            instance.m_SimpleServerSimple.networkConnectionClass;

        /// <summary>
        /// <para>The number of channels the network is configure with.</para>
        /// </summary>
        public static int numChannels =>
            instance.m_SimpleServerSimple.hostTopology.DefaultConfig.ChannelCount;

        /// <summary>
        /// <para>This is a dictionary of networked objects that have been spawned on the server.</para>
        /// </summary>
        public static Dictionary<NetworkInstanceId, NetworkIdentity> objects =>
            instance.m_NetworkScene.localObjects;

        /// <summary>
        /// <para>Setting this true will make the server send peer info to all participants of the network.</para>
        /// </summary>
        [Obsolete("Moved to NetworkMigrationManager")]
        public static bool sendPeerInfo
        {
            get => 
                false;
            set
            {
            }
        }

        /// <summary>
        /// <para>The transport layer hostId used by this server.</para>
        /// </summary>
        public static int serverHostId =>
            instance.m_SimpleServerSimple.serverHostId;

        /// <summary>
        /// <para>This makes the server listen for WebSockets connections instead of normal transport layer connections.</para>
        /// </summary>
        public static bool useWebSockets
        {
            get => 
                instance.m_SimpleServerSimple.useWebSockets;
            set
            {
                instance.m_SimpleServerSimple.useWebSockets = value;
            }
        }

        private class ServerSimpleWrapper : NetworkServerSimple
        {
            private NetworkServer m_Server;

            public ServerSimpleWrapper(NetworkServer server)
            {
                this.m_Server = server;
            }

            public override void OnConnected(NetworkConnection conn)
            {
                this.m_Server.OnConnected(conn);
            }

            public override void OnConnectError(int connectionId, byte error)
            {
                this.m_Server.GenerateConnectError(error);
            }

            public override void OnData(NetworkConnection conn, int receivedSize, int channelId)
            {
                this.m_Server.OnData(conn, receivedSize, channelId);
            }

            public override void OnDataError(NetworkConnection conn, byte error)
            {
                this.m_Server.GenerateDataError(conn, error);
            }

            public override void OnDisconnected(NetworkConnection conn)
            {
                this.m_Server.OnDisconnected(conn);
            }

            public override void OnDisconnectError(NetworkConnection conn, byte error)
            {
                this.m_Server.GenerateDisconnectError(conn, error);
            }
        }
    }
}

