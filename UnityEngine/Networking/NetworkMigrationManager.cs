namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.Networking.Types;

    /// <summary>
    /// <para>A component that manages the process of a new host taking over a game when the old host is lost. This is referred to as "host migration". The migration manager sends information about each peer in the game to all the clients, and when the host is lost because of a crash or network outage, the clients are able to choose a new host, and continue the game.
    /// 
    /// The old host is able to rejoin the new game on the new host.
    /// 
    /// The state of SyncVars and SyncLists on all objects with NetworkIdentities in the scene is maintained during a host migration. This also applies to custom serialized data for objects.
    /// 
    /// All of the player objects in the game are disabled when the host is lost. Then, when the other clients rejoin the new game on the new host, the corresponding players for those clients are re-enabled on the host, and respawned on the other clients. No player state data is lost during a host migration.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkMigrationManager")]
    public class NetworkMigrationManager : MonoBehaviour
    {
        private NetworkClient m_Client;
        private bool m_DisconnectedFromHost;
        [SerializeField]
        private bool m_HostMigration = true;
        private bool m_HostWasShutdown;
        private MatchInfo m_MatchInfo;
        private string m_NewHostAddress;
        private PeerInfoMessage m_NewHostInfo = new PeerInfoMessage();
        [SerializeField]
        private int m_OffsetX = 10;
        [SerializeField]
        private int m_OffsetY = 300;
        private int m_OldServerConnectionId = -1;
        private PeerListMessage m_PeerListMessage = new PeerListMessage();
        private PeerInfoMessage[] m_Peers;
        private Dictionary<int, ConnectionPendingPlayers> m_PendingPlayers = new Dictionary<int, ConnectionPendingPlayers>();
        [SerializeField]
        private bool m_ShowGUI = true;
        private bool m_WaitingReconnectToNewHost;
        private bool m_WaitingToBecomeNewHost;

        private void AddPendingPlayer(GameObject obj, int connectionId, NetworkInstanceId netId, short playerControllerId)
        {
            if (!this.m_PendingPlayers.ContainsKey(connectionId))
            {
                ConnectionPendingPlayers players = new ConnectionPendingPlayers {
                    players = new List<PendingPlayerInfo>()
                };
                this.m_PendingPlayers[connectionId] = players;
            }
            PendingPlayerInfo item = new PendingPlayerInfo {
                netId = netId,
                playerControllerId = playerControllerId,
                obj = obj
            };
            ConnectionPendingPlayers players2 = this.m_PendingPlayers[connectionId];
            players2.players.Add(item);
        }

        internal void AssignAuthorityCallback(NetworkConnection conn, NetworkIdentity uv, bool authorityState)
        {
            PeerAuthorityMessage msg = new PeerAuthorityMessage {
                connectionId = conn.connectionId,
                netId = uv.netId,
                authorityState = authorityState
            };
            if (LogFilter.logDebug)
            {
                Debug.Log("AssignAuthorityCallback send for netId" + uv.netId);
            }
            for (int i = 0; i < NetworkServer.connections.Count; i++)
            {
                NetworkConnection connection = NetworkServer.connections[i];
                if (connection != null)
                {
                    connection.Send(0x12, msg);
                }
            }
        }

        /// <summary>
        /// <para>This causes a client that has been disconnected from the host to become the new host of the game.</para>
        /// </summary>
        /// <param name="port">The network port to listen on.</param>
        /// <returns>
        /// <para>True if able to become the new host.</para>
        /// </returns>
        public bool BecomeNewHost(int port)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkMigrationManager BecomeNewHost " + this.m_MatchInfo);
            }
            NetworkServer.RegisterHandler(0x2f, new NetworkMessageDelegate(this.OnServerReconnectPlayerMessage));
            NetworkClient externalClient = NetworkServer.BecomeHost(this.m_Client, port, this.m_MatchInfo, this.oldServerConnectionId, this.peers);
            if (externalClient != null)
            {
                if (NetworkManager.singleton != null)
                {
                    NetworkManager.singleton.RegisterServerMessages();
                    NetworkManager.singleton.UseExternalClient(externalClient);
                }
                else
                {
                    Debug.LogWarning("MigrationManager BecomeNewHost - No NetworkManager.");
                }
                externalClient.RegisterHandlerSafe(11, new NetworkMessageDelegate(this.OnPeerInfo));
                this.RemovePendingPlayer(this.m_OldServerConnectionId);
                this.Reset(-1);
                this.SendPeerInfo();
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkServer.BecomeHost failed");
            }
            return false;
        }

        /// <summary>
        /// <para>This causes objects for known players to be disabled.</para>
        /// </summary>
        public void DisablePlayerObjects()
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkMigrationManager DisablePlayerObjects");
            }
            if (this.m_Peers != null)
            {
                for (int i = 0; i < this.m_Peers.Length; i++)
                {
                    PeerInfoMessage message = this.m_Peers[i];
                    if (message.playerIds != null)
                    {
                        for (int j = 0; j < message.playerIds.Length; j++)
                        {
                            PeerInfoPlayer player = message.playerIds[j];
                            if (LogFilter.logDev)
                            {
                                Debug.Log(string.Concat(new object[] { "DisablePlayerObjects disable player for ", message.address, " netId:", player.netId, " control:", player.playerControllerId }));
                            }
                            GameObject obj2 = ClientScene.FindLocalObject(player.netId);
                            if (obj2 != null)
                            {
                                obj2.SetActive(false);
                                this.AddPendingPlayer(obj2, message.connectionId, player.netId, player.playerControllerId);
                            }
                            else if (LogFilter.logWarn)
                            {
                                Debug.LogWarning(string.Concat(new object[] { "DisablePlayerObjects didnt find player Conn:", message.connectionId, " NetId:", player.netId }));
                            }
                        }
                    }
                }
            }
        }

        public virtual bool FindNewHost(out PeerInfoMessage newHostInfo, out bool youAreNewHost)
        {
            if (this.m_Peers == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkMigrationManager FindLowestHost no peers");
                }
                newHostInfo = null;
                youAreNewHost = false;
                return false;
            }
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkMigrationManager FindLowestHost");
            }
            newHostInfo = new PeerInfoMessage();
            newHostInfo.connectionId = 0xc350;
            newHostInfo.address = "";
            newHostInfo.port = 0;
            int connectionId = -1;
            youAreNewHost = false;
            if (this.m_Peers == null)
            {
                return false;
            }
            for (int i = 0; i < this.m_Peers.Length; i++)
            {
                PeerInfoMessage message = this.m_Peers[i];
                if ((message.connectionId != 0) && !message.isHost)
                {
                    if (message.isYou)
                    {
                        connectionId = message.connectionId;
                    }
                    if (message.connectionId < newHostInfo.connectionId)
                    {
                        newHostInfo = message;
                    }
                }
            }
            if (newHostInfo.connectionId == 0xc350)
            {
                return false;
            }
            if (newHostInfo.connectionId == connectionId)
            {
                youAreNewHost = true;
            }
            if (LogFilter.logDev)
            {
                Debug.Log("FindNewHost new host is " + newHostInfo.address);
            }
            return true;
        }

        private GameObject FindPendingPlayer(int connectionId, NetworkInstanceId netId, short playerControllerId)
        {
            ConnectionPendingPlayers players2;
            if (!this.m_PendingPlayers.ContainsKey(connectionId))
            {
                goto Label_0089;
            }
            int num = 0;
        Label_0068:
            players2 = this.m_PendingPlayers[connectionId];
            if (num < players2.players.Count)
            {
                ConnectionPendingPlayers players = this.m_PendingPlayers[connectionId];
                PendingPlayerInfo info = players.players[num];
                if ((info.netId == netId) && (info.playerControllerId == playerControllerId))
                {
                    return info.obj;
                }
                num++;
                goto Label_0068;
            }
        Label_0089:
            return null;
        }

        /// <summary>
        /// <para>Used to initialize the migration manager with client and match information.</para>
        /// </summary>
        /// <param name="newClient">The NetworkClient being used to connect to the host.</param>
        /// <param name="newMatchInfo">Information about the match being used. This may be null if there is no match.</param>
        public void Initialize(NetworkClient newClient, MatchInfo newMatchInfo)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkMigrationManager initialize");
            }
            this.m_Client = newClient;
            this.m_MatchInfo = newMatchInfo;
            newClient.RegisterHandlerSafe(11, new NetworkMessageDelegate(this.OnPeerInfo));
            newClient.RegisterHandlerSafe(0x12, new NetworkMessageDelegate(this.OnPeerClientAuthority));
            NetworkIdentity.clientAuthorityCallback = new NetworkIdentity.ClientAuthorityCallback(this.AssignAuthorityCallback);
        }

        /// <summary>
        /// <para>This should be called on a client when it has lost its connection to the host.</para>
        /// </summary>
        /// <param name="conn">The connection of the client that was connected to the host.</param>
        /// <returns>
        /// <para>True if the client should stay in the on-line scene.</para>
        /// </returns>
        public bool LostHostOnClient(NetworkConnection conn)
        {
            byte num;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkMigrationManager client OnDisconnectedFromHost");
            }
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("LostHostOnClient: Host migration not supported on WebGL");
                }
                return false;
            }
            if (this.m_Client == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkMigrationManager LostHostOnHost client was never initialized.");
                }
                return false;
            }
            if (!this.m_HostMigration)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkMigrationManager LostHostOnHost migration not enabled.");
                }
                return false;
            }
            this.m_DisconnectedFromHost = true;
            this.DisablePlayerObjects();
            NetworkTransport.Disconnect(this.m_Client.hostId, this.m_Client.connection.connectionId, out num);
            if (this.m_OldServerConnectionId != -1)
            {
                SceneChangeOption option;
                this.OnClientDisconnectedFromHost(conn, out option);
                return (option == SceneChangeOption.StayInOnlineScene);
            }
            return false;
        }

        /// <summary>
        /// <para>This should be called on a host when it has has been shutdown.</para>
        /// </summary>
        public void LostHostOnHost()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkMigrationManager LostHostOnHost");
            }
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("LostHostOnHost: Host migration not supported on WebGL");
                }
            }
            else
            {
                this.OnServerHostShutdown();
                if (this.m_Peers == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkMigrationManager LostHostOnHost no peers");
                    }
                }
                else if (this.m_Peers.Length != 1)
                {
                    this.m_HostWasShutdown = true;
                }
            }
        }

        /// <summary>
        /// <para>A virtual function that is called when the authority of a non-player object changes.</para>
        /// </summary>
        /// <param name="go">The game object whose authority has changed.</param>
        /// <param name="connectionId">The id of the connection whose authority changed for this object.</param>
        /// <param name="authorityState">The new authority state for the object.</param>
        protected virtual void OnAuthorityUpdated(GameObject go, int connectionId, bool authorityState)
        {
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "NetworkMigrationManager OnAuthorityUpdated for ", go, " conn:", connectionId, " state:", authorityState }));
            }
        }

        protected virtual void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange)
        {
            sceneChange = SceneChangeOption.StayInOnlineScene;
        }

        private void OnGUI()
        {
            if (this.m_ShowGUI)
            {
                if (this.m_HostWasShutdown)
                {
                    this.OnGUIHost();
                }
                else if (this.m_DisconnectedFromHost && (this.m_OldServerConnectionId != -1))
                {
                    this.OnGUIClient();
                }
            }
        }

        private void OnGUIClient()
        {
            int offsetY = this.m_OffsetY;
            GUI.Label(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 40f), "Lost Connection To Host ID(" + this.m_OldServerConnectionId + ")");
            offsetY += 0x19;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                GUI.Label(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 40f), "Host Migration not supported for WebGL");
            }
            else
            {
                if (this.m_WaitingToBecomeNewHost)
                {
                    GUI.Label(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 40f), "You are the new host");
                    offsetY += 0x19;
                    if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Start As Host"))
                    {
                        if (NetworkManager.singleton != null)
                        {
                            this.BecomeNewHost(NetworkManager.singleton.networkPort);
                        }
                        else
                        {
                            Debug.LogWarning("MigrationManager Client BecomeNewHost - No NetworkManager.");
                        }
                    }
                    offsetY += 0x19;
                }
                else if (this.m_WaitingReconnectToNewHost)
                {
                    GUI.Label(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 40f), "New host is " + this.m_NewHostAddress);
                    offsetY += 0x19;
                    if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Reconnect To New Host"))
                    {
                        this.Reset(this.m_OldServerConnectionId);
                        if (NetworkManager.singleton != null)
                        {
                            NetworkManager.singleton.networkAddress = this.m_NewHostAddress;
                            NetworkManager.singleton.client.ReconnectToNewHost(this.m_NewHostAddress, NetworkManager.singleton.networkPort);
                        }
                        else
                        {
                            Debug.LogWarning("MigrationManager Client reconnect - No NetworkManager.");
                        }
                    }
                    offsetY += 0x19;
                }
                else
                {
                    bool flag;
                    if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Pick New Host") && this.FindNewHost(out this.m_NewHostInfo, out flag))
                    {
                        this.m_NewHostAddress = this.m_NewHostInfo.address;
                        if (flag)
                        {
                            this.m_WaitingToBecomeNewHost = true;
                        }
                        else
                        {
                            this.m_WaitingReconnectToNewHost = true;
                        }
                    }
                    offsetY += 0x19;
                }
                if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Leave Game"))
                {
                    if (NetworkManager.singleton != null)
                    {
                        NetworkManager.singleton.SetupMigrationManager(null);
                        NetworkManager.singleton.StopHost();
                    }
                    else
                    {
                        Debug.LogWarning("MigrationManager Client LeaveGame - No NetworkManager.");
                    }
                    this.Reset(-1);
                }
                offsetY += 0x19;
            }
        }

        private void OnGUIHost()
        {
            int offsetY = this.m_OffsetY;
            GUI.Label(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 40f), "Host Was Shutdown ID(" + this.m_OldServerConnectionId + ")");
            offsetY += 0x19;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                GUI.Label(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 40f), "Host Migration not supported for WebGL");
            }
            else
            {
                if (this.m_WaitingReconnectToNewHost)
                {
                    if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Reconnect as Client"))
                    {
                        this.Reset(0);
                        if (NetworkManager.singleton != null)
                        {
                            NetworkManager.singleton.networkAddress = GUI.TextField(new Rect((float) (this.m_OffsetX + 100), (float) offsetY, 95f, 20f), NetworkManager.singleton.networkAddress);
                            NetworkManager.singleton.StartClient();
                        }
                        else
                        {
                            Debug.LogWarning("MigrationManager Old Host Reconnect - No NetworkManager.");
                        }
                    }
                    offsetY += 0x19;
                }
                else
                {
                    bool flag;
                    if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Pick New Host") && this.FindNewHost(out this.m_NewHostInfo, out flag))
                    {
                        this.m_NewHostAddress = this.m_NewHostInfo.address;
                        if (flag)
                        {
                            Debug.LogWarning("MigrationManager FindNewHost - new host is self?");
                        }
                        else
                        {
                            this.m_WaitingReconnectToNewHost = true;
                        }
                    }
                    offsetY += 0x19;
                }
                if (GUI.Button(new Rect((float) this.m_OffsetX, (float) offsetY, 200f, 20f), "Leave Game"))
                {
                    if (NetworkManager.singleton != null)
                    {
                        NetworkManager.singleton.SetupMigrationManager(null);
                        NetworkManager.singleton.StopHost();
                    }
                    else
                    {
                        Debug.LogWarning("MigrationManager Old Host LeaveGame - No NetworkManager.");
                    }
                    this.Reset(-1);
                }
                offsetY += 0x19;
            }
        }

        private void OnPeerClientAuthority(NetworkMessage netMsg)
        {
            PeerAuthorityMessage message = netMsg.ReadMessage<PeerAuthorityMessage>();
            if (LogFilter.logDebug)
            {
                Debug.Log("OnPeerClientAuthority for netId:" + message.netId);
            }
            if (this.m_Peers != null)
            {
                for (int i = 0; i < this.m_Peers.Length; i++)
                {
                    PeerInfoMessage message2 = this.m_Peers[i];
                    if (message2.connectionId == message.connectionId)
                    {
                        if (message2.playerIds == null)
                        {
                            message2.playerIds = new PeerInfoPlayer[0];
                        }
                        if (message.authorityState)
                        {
                            for (int j = 0; j < message2.playerIds.Length; j++)
                            {
                                if (message2.playerIds[j].netId == message.netId)
                                {
                                    return;
                                }
                            }
                            PeerInfoPlayer player = new PeerInfoPlayer {
                                netId = message.netId,
                                playerControllerId = -1
                            };
                            message2.playerIds = new List<PeerInfoPlayer>(message2.playerIds) { player }.ToArray();
                        }
                        else
                        {
                            for (int k = 0; k < message2.playerIds.Length; k++)
                            {
                                if (message2.playerIds[k].netId == message.netId)
                                {
                                    List<PeerInfoPlayer> list2 = new List<PeerInfoPlayer>(message2.playerIds);
                                    list2.RemoveAt(k);
                                    message2.playerIds = list2.ToArray();
                                    break;
                                }
                            }
                        }
                    }
                }
                GameObject go = ClientScene.FindLocalObject(message.netId);
                this.OnAuthorityUpdated(go, message.connectionId, message.authorityState);
            }
        }

        private void OnPeerInfo(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("OnPeerInfo");
            }
            netMsg.ReadMessage<PeerListMessage>(this.m_PeerListMessage);
            this.m_Peers = this.m_PeerListMessage.peers;
            this.m_OldServerConnectionId = this.m_PeerListMessage.oldServerConnectionId;
            for (int i = 0; i < this.m_Peers.Length; i++)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "peer conn ", this.m_Peers[i].connectionId, " your conn ", this.m_PeerListMessage.oldServerConnectionId }));
                }
                if (this.m_Peers[i].connectionId == this.m_PeerListMessage.oldServerConnectionId)
                {
                    this.m_Peers[i].isYou = true;
                    break;
                }
            }
            this.OnPeersUpdated(this.m_PeerListMessage);
        }

        /// <summary>
        /// <para>A virtual function that is called when the set of peers in the game changes.</para>
        /// </summary>
        /// <param name="peers">The set of peers in the game.</param>
        protected virtual void OnPeersUpdated(PeerListMessage peers)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkMigrationManager NumPeers " + peers.peers.Length);
            }
        }

        /// <summary>
        /// <para>A virtual function that is called when the host is shutdown.</para>
        /// </summary>
        protected virtual void OnServerHostShutdown()
        {
        }

        /// <summary>
        /// <para>A virtual function that is called for non-player objects with client authority on the new host when a client from the old host reconnects to the new host.</para>
        /// </summary>
        /// <param name="newConnection">The connection of the new client.</param>
        /// <param name="oldObject">The object with authority that is being reconnected.</param>
        /// <param name="oldConnectionId">The connectionId of this client on the old host.</param>
        protected virtual void OnServerReconnectObject(NetworkConnection newConnection, GameObject oldObject, int oldConnectionId)
        {
            this.ReconnectObjectForConnection(newConnection, oldObject, oldConnectionId);
        }

        /// <summary>
        /// <para>A virtual function that is called on the new host when a client from the old host reconnects to the new host.</para>
        /// </summary>
        /// <param name="newConnection">The connection of the new client.</param>
        /// <param name="oldPlayer">The player object associated with this client.</param>
        /// <param name="oldConnectionId">The connectionId of this client on the old host.</param>
        /// <param name="playerControllerId">The playerControllerId of the player that is re-joining.</param>
        /// <param name="extraMessageReader">Additional message data (optional).</param>
        protected virtual void OnServerReconnectPlayer(NetworkConnection newConnection, GameObject oldPlayer, int oldConnectionId, short playerControllerId)
        {
            this.ReconnectPlayerForConnection(newConnection, oldPlayer, oldConnectionId, playerControllerId);
        }

        /// <summary>
        /// <para>A virtual function that is called on the new host when a client from the old host reconnects to the new host.</para>
        /// </summary>
        /// <param name="newConnection">The connection of the new client.</param>
        /// <param name="oldPlayer">The player object associated with this client.</param>
        /// <param name="oldConnectionId">The connectionId of this client on the old host.</param>
        /// <param name="playerControllerId">The playerControllerId of the player that is re-joining.</param>
        /// <param name="extraMessageReader">Additional message data (optional).</param>
        protected virtual void OnServerReconnectPlayer(NetworkConnection newConnection, GameObject oldPlayer, int oldConnectionId, short playerControllerId, NetworkReader extraMessageReader)
        {
            this.ReconnectPlayerForConnection(newConnection, oldPlayer, oldConnectionId, playerControllerId);
        }

        private void OnServerReconnectPlayerMessage(NetworkMessage netMsg)
        {
            ReconnectMessage message = netMsg.ReadMessage<ReconnectMessage>();
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "OnReconnectMessage: connId=", message.oldConnectionId, " playerControllerId:", message.playerControllerId, " netId:", message.netId }));
            }
            GameObject oldPlayer = this.FindPendingPlayer(message.oldConnectionId, message.netId, message.playerControllerId);
            if (oldPlayer == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "OnReconnectMessage connId=", message.oldConnectionId, " player null for netId:", message.netId, " msg.playerControllerId:", message.playerControllerId }));
                }
            }
            else if (oldPlayer.activeSelf)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("OnReconnectMessage connId=" + message.oldConnectionId + " player already active?");
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("OnReconnectMessage: player=" + oldPlayer);
                }
                NetworkReader extraMessageReader = null;
                if (message.msgSize != 0)
                {
                    extraMessageReader = new NetworkReader(message.msgData);
                }
                if (message.playerControllerId != -1)
                {
                    if (extraMessageReader == null)
                    {
                        this.OnServerReconnectPlayer(netMsg.conn, oldPlayer, message.oldConnectionId, message.playerControllerId);
                    }
                    else
                    {
                        this.OnServerReconnectPlayer(netMsg.conn, oldPlayer, message.oldConnectionId, message.playerControllerId, extraMessageReader);
                    }
                }
                else
                {
                    this.OnServerReconnectObject(netMsg.conn, oldPlayer, message.oldConnectionId);
                }
            }
        }

        /// <summary>
        /// <para>This re-establishes a non-player object with client authority with a client that is reconnected.  It is similar to NetworkServer.SpawnWithClientAuthority().</para>
        /// </summary>
        /// <param name="newConnection">The connection of the new client.</param>
        /// <param name="oldObject">The object with client authority that is being reconnected.</param>
        /// <param name="oldConnectionId">This client's connectionId on the old host.</param>
        /// <returns>
        /// <para>True if the object was reconnected.</para>
        /// </returns>
        public bool ReconnectObjectForConnection(NetworkConnection newConnection, GameObject oldObject, int oldConnectionId)
        {
            if (!NetworkServer.active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReconnectObjectForConnection must have active server");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ReconnectObjectForConnection: oldConnId=", oldConnectionId, " obj=", oldObject, " conn:", newConnection }));
            }
            if (!this.m_PendingPlayers.ContainsKey(oldConnectionId))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReconnectObjectForConnection oldConnId=" + oldConnectionId + " not found.");
                }
                return false;
            }
            oldObject.SetActive(true);
            oldObject.GetComponent<NetworkIdentity>().SetNetworkInstanceId(new NetworkInstanceId(0));
            if (!NetworkServer.SpawnWithClientAuthority(oldObject, newConnection))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReconnectObjectForConnection oldConnId=" + oldConnectionId + " SpawnWithClientAuthority failed.");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// <para>This re-establishes a player object with a client that is reconnected.  It is similar to NetworkServer.AddPlayerForConnection(). The player game object will become the player object for the new connection.</para>
        /// </summary>
        /// <param name="newConnection">The connection of the new client.</param>
        /// <param name="oldPlayer">The player object.</param>
        /// <param name="oldConnectionId">This client's connectionId on the old host.</param>
        /// <param name="playerControllerId">The playerControllerId of the player that is rejoining.</param>
        /// <returns>
        /// <para>True if able to re-add this player.</para>
        /// </returns>
        public bool ReconnectPlayerForConnection(NetworkConnection newConnection, GameObject oldPlayer, int oldConnectionId, short playerControllerId)
        {
            if (!NetworkServer.active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReconnectPlayerForConnection must have active server");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ReconnectPlayerForConnection: oldConnId=", oldConnectionId, " player=", oldPlayer, " conn:", newConnection }));
            }
            if (!this.m_PendingPlayers.ContainsKey(oldConnectionId))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReconnectPlayerForConnection oldConnId=" + oldConnectionId + " not found.");
                }
                return false;
            }
            oldPlayer.SetActive(true);
            NetworkServer.Spawn(oldPlayer);
            if (!NetworkServer.AddPlayerForConnection(newConnection, oldPlayer, playerControllerId))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReconnectPlayerForConnection oldConnId=" + oldConnectionId + " AddPlayerForConnection failed.");
                }
                return false;
            }
            if (NetworkServer.localClientActive)
            {
                this.SendPeerInfo();
            }
            return true;
        }

        private void RemovePendingPlayer(int connectionId)
        {
            this.m_PendingPlayers.Remove(connectionId);
        }

        /// <summary>
        /// <para>Resets the migration manager, and sets the ClientScene's ReconnectId.</para>
        /// </summary>
        /// <param name="reconnectId">The connectionId for the ClientScene to use when reconnecting.</param>
        public void Reset(int reconnectId)
        {
            this.m_OldServerConnectionId = -1;
            this.m_WaitingToBecomeNewHost = false;
            this.m_WaitingReconnectToNewHost = false;
            this.m_DisconnectedFromHost = false;
            this.m_HostWasShutdown = false;
            ClientScene.SetReconnectId(reconnectId, this.m_Peers);
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.SetupMigrationManager(this);
            }
        }

        /// <summary>
        /// <para>This sends the set of peers in the game to all the peers in the game.</para>
        /// </summary>
        public void SendPeerInfo()
        {
            if (this.m_HostMigration)
            {
                PeerListMessage msg = new PeerListMessage();
                List<PeerInfoMessage> list = new List<PeerInfoMessage>();
                for (int i = 0; i < NetworkServer.connections.Count; i++)
                {
                    NetworkConnection connection = NetworkServer.connections[i];
                    if (connection != null)
                    {
                        string str;
                        int num2;
                        NetworkID kid;
                        NodeID eid;
                        byte num3;
                        PeerInfoMessage item = new PeerInfoMessage();
                        NetworkTransport.GetConnectionInfo(NetworkServer.serverHostId, connection.connectionId, out str, out num2, out kid, out eid, out num3);
                        item.connectionId = connection.connectionId;
                        item.port = num2;
                        if (i == 0)
                        {
                            item.port = NetworkServer.listenPort;
                            item.isHost = true;
                            item.address = "<host>";
                        }
                        else
                        {
                            item.address = str;
                            item.isHost = false;
                        }
                        List<PeerInfoPlayer> list2 = new List<PeerInfoPlayer>();
                        for (int k = 0; k < connection.playerControllers.Count; k++)
                        {
                            PlayerController controller = connection.playerControllers[k];
                            if ((controller != null) && (controller.unetView != null))
                            {
                                PeerInfoPlayer player;
                                player.netId = controller.unetView.netId;
                                player.playerControllerId = controller.unetView.playerControllerId;
                                list2.Add(player);
                            }
                        }
                        if (connection.clientOwnedObjects != null)
                        {
                            foreach (NetworkInstanceId id in connection.clientOwnedObjects)
                            {
                                GameObject obj2 = NetworkServer.FindLocalObject(id);
                                if ((obj2 != null) && (obj2.GetComponent<NetworkIdentity>().playerControllerId == -1))
                                {
                                    PeerInfoPlayer player2;
                                    player2.netId = id;
                                    player2.playerControllerId = -1;
                                    list2.Add(player2);
                                }
                            }
                        }
                        if (list2.Count > 0)
                        {
                            item.playerIds = list2.ToArray();
                        }
                        list.Add(item);
                    }
                }
                msg.peers = list.ToArray();
                for (int j = 0; j < NetworkServer.connections.Count; j++)
                {
                    NetworkConnection connection2 = NetworkServer.connections[j];
                    if (connection2 != null)
                    {
                        msg.oldServerConnectionId = connection2.connectionId;
                        connection2.Send(11, msg);
                    }
                }
            }
        }

        private void Start()
        {
            this.Reset(-1);
        }

        /// <summary>
        /// <para>The client instance that is being used to connect to the host.</para>
        /// </summary>
        public NetworkClient client
        {
            get
            {
                return this.m_Client;
            }
        }

        /// <summary>
        /// <para>True is this is a client that has been disconnected from a host.</para>
        /// </summary>
        public bool disconnectedFromHost
        {
            get
            {
                return this.m_DisconnectedFromHost;
            }
        }

        /// <summary>
        /// <para>Controls whether host migration is active.</para>
        /// </summary>
        public bool hostMigration
        {
            get
            {
                return this.m_HostMigration;
            }
            set
            {
                this.m_HostMigration = value;
            }
        }

        /// <summary>
        /// <para>True if this was the host and the host has been shut down.</para>
        /// </summary>
        public bool hostWasShutdown
        {
            get
            {
                return this.m_HostWasShutdown;
            }
        }

        /// <summary>
        /// <para>Information about the match. This may be null if there is no match.</para>
        /// </summary>
        public MatchInfo matchInfo
        {
            get
            {
                return this.m_MatchInfo;
            }
        }

        /// <summary>
        /// <para>The IP address of the new host to connect to.</para>
        /// </summary>
        public string newHostAddress
        {
            get
            {
                return this.m_NewHostAddress;
            }
            set
            {
                this.m_NewHostAddress = value;
            }
        }

        /// <summary>
        /// <para>The X offset in pixels of the migration manager default GUI.</para>
        /// </summary>
        public int offsetX
        {
            get
            {
                return this.m_OffsetX;
            }
            set
            {
                this.m_OffsetX = value;
            }
        }

        /// <summary>
        /// <para>The Y offset in pixels of the migration manager default GUI.</para>
        /// </summary>
        public int offsetY
        {
            get
            {
                return this.m_OffsetY;
            }
            set
            {
                this.m_OffsetY = value;
            }
        }

        /// <summary>
        /// <para>The connectionId that this client was assign on the old host.</para>
        /// </summary>
        public int oldServerConnectionId
        {
            get
            {
                return this.m_OldServerConnectionId;
            }
        }

        /// <summary>
        /// <para>The set of peers involved in the game. This includes the host and this client.</para>
        /// </summary>
        public PeerInfoMessage[] peers
        {
            get
            {
                return this.m_Peers;
            }
        }

        /// <summary>
        /// <para>The player objects that have been disabled, and are waiting for their corresponding clients to reconnect.</para>
        /// </summary>
        public Dictionary<int, ConnectionPendingPlayers> pendingPlayers
        {
            get
            {
                return this.m_PendingPlayers;
            }
        }

        /// <summary>
        /// <para>Flag to toggle display of the default UI.</para>
        /// </summary>
        public bool showGUI
        {
            get
            {
                return this.m_ShowGUI;
            }
            set
            {
                this.m_ShowGUI = value;
            }
        }

        /// <summary>
        /// <para>True if this is a client that was disconnected from the host and is now waiting to reconnect to the new host.</para>
        /// </summary>
        public bool waitingReconnectToNewHost
        {
            get
            {
                return this.m_WaitingReconnectToNewHost;
            }
            set
            {
                this.m_WaitingReconnectToNewHost = value;
            }
        }

        /// <summary>
        /// <para>True if this is a client that was disconnected from the host, and was chosen as the new host.</para>
        /// </summary>
        public bool waitingToBecomeNewHost
        {
            get
            {
                return this.m_WaitingToBecomeNewHost;
            }
            set
            {
                this.m_WaitingToBecomeNewHost = value;
            }
        }

        /// <summary>
        /// <para>The player objects for connections to the old host.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ConnectionPendingPlayers
        {
            /// <summary>
            /// <para>The list of players for a connection.</para>
            /// </summary>
            public List<NetworkMigrationManager.PendingPlayerInfo> players;
        }

        /// <summary>
        /// <para>Information about a player object from another peer.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PendingPlayerInfo
        {
            /// <summary>
            /// <para>The networkId of the player object.</para>
            /// </summary>
            public NetworkInstanceId netId;
            /// <summary>
            /// <para>The playerControllerId of the player GameObject.</para>
            /// </summary>
            public short playerControllerId;
            /// <summary>
            /// <para>The gameObject for the player.</para>
            /// </summary>
            public GameObject obj;
        }

        /// <summary>
        /// <para>An enumeration of how to handle scene changes when the connection to the host is lost.</para>
        /// </summary>
        public enum SceneChangeOption
        {
            StayInOnlineScene,
            SwitchToOfflineScene
        }
    }
}

