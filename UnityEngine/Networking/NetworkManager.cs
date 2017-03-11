namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.Networking.Types;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// <para>The NetworkManager is a convenience class for the HLAPI for managing networking systems.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkManager")]
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// <para>The current NetworkClient being used by the manager.</para>
        /// </summary>
        public NetworkClient client;
        /// <summary>
        /// <para>True if the NetworkServer or NetworkClient isactive.</para>
        /// </summary>
        public bool isNetworkActive;
        [SerializeField]
        private bool m_AllowFragmentation = true;
        [SerializeField]
        private bool m_AutoCreatePlayer = true;
        [SerializeField]
        private List<QosType> m_Channels = new List<QosType>();
        private bool m_ClientLoadedScene;
        [SerializeField]
        private ConnectionConfig m_ConnectionConfig;
        [SerializeField]
        private bool m_CustomConfig;
        [SerializeField]
        private bool m_DontDestroyOnLoad = true;
        private EndPoint m_EndPoint;
        [SerializeField]
        private GlobalConfig m_GlobalConfig;
        [SerializeField]
        private LogFilter.FilterLevel m_LogLevel = LogFilter.FilterLevel.Info;
        [SerializeField]
        private string m_MatchHost = "mm.unet.unity3d.com";
        [SerializeField]
        private int m_MatchPort = 0x1bb;
        [SerializeField]
        private int m_MaxBufferedPackets = 0x10;
        [SerializeField]
        private int m_MaxConnections = 4;
        [SerializeField]
        private float m_MaxDelay = 0.01f;
        private NetworkMigrationManager m_MigrationManager;
        [SerializeField]
        private string m_NetworkAddress = "localhost";
        [SerializeField]
        private int m_NetworkPort = 0x1e61;
        [SerializeField]
        private string m_OfflineScene = "";
        [SerializeField]
        private string m_OnlineScene = "";
        [SerializeField]
        private float m_PacketLossPercentage;
        [SerializeField]
        private GameObject m_PlayerPrefab;
        [SerializeField]
        private PlayerSpawnMethod m_PlayerSpawnMethod;
        [SerializeField]
        private bool m_RunInBackground = true;
        [SerializeField]
        private bool m_ScriptCRCCheck = true;
        [SerializeField]
        private string m_ServerBindAddress = "";
        [SerializeField]
        private bool m_ServerBindToIP;
        [SerializeField]
        private int m_SimulatedLatency = 1;
        [SerializeField]
        private List<GameObject> m_SpawnPrefabs = new List<GameObject>();
        [SerializeField]
        private bool m_UseSimulator;
        [SerializeField]
        private bool m_UseWebSockets;
        /// <summary>
        /// <para>The list of matches that are available to join.</para>
        /// </summary>
        public List<MatchInfoSnapshot> matches;
        /// <summary>
        /// <para>A MatchInfo instance that will be used when StartServer() or StartClient() are called.</para>
        /// </summary>
        public MatchInfo matchInfo;
        /// <summary>
        /// <para>The UMatch MatchMaker object.</para>
        /// </summary>
        public NetworkMatch matchMaker;
        /// <summary>
        /// <para>The name of the current match.</para>
        /// </summary>
        [SerializeField]
        public string matchName = "default";
        /// <summary>
        /// <para>The maximum number of players in the current match.</para>
        /// </summary>
        [SerializeField]
        public uint matchSize = 4;
        /// <summary>
        /// <para>The name of the current network scene.</para>
        /// </summary>
        public static string networkSceneName = "";
        private static AddPlayerMessage s_AddPlayerMessage = new AddPlayerMessage();
        private static string s_Address;
        private static NetworkConnection s_ClientReadyConnection;
        private static bool s_DomainReload;
        private static ErrorMessage s_ErrorMessage = new ErrorMessage();
        private static AsyncOperation s_LoadingSceneAsync;
        private static NetworkManager s_PendingSingleton;
        private static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();
        private static int s_StartPositionIndex;
        private static List<Transform> s_StartPositions = new List<Transform>();
        /// <summary>
        /// <para>The NetworkManager singleton object.</para>
        /// </summary>
        public static NetworkManager singleton;

        public NetworkManager()
        {
            s_PendingSingleton = this;
        }

        private void Awake()
        {
            this.InitializeSingleton();
        }

        private void CleanupNetworkIdentities()
        {
            foreach (NetworkIdentity identity in UnityEngine.Object.FindObjectsOfType<NetworkIdentity>())
            {
                identity.MarkForReset();
            }
        }

        internal void ClientChangeScene(string newSceneName, bool forceReload)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientChangeScene empty scene name");
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ClientChangeScene newSceneName:" + newSceneName + " networkSceneName:" + networkSceneName);
                }
                if (newSceneName == networkSceneName)
                {
                    if (this.m_MigrationManager != null)
                    {
                        this.FinishLoadScene();
                        return;
                    }
                    if (!forceReload)
                    {
                        this.FinishLoadScene();
                        return;
                    }
                }
                s_LoadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
                networkSceneName = newSceneName;
            }
        }

        private NetworkClient ConnectLocalClient()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StartHost port:" + this.m_NetworkPort);
            }
            this.m_NetworkAddress = "localhost";
            this.client = ClientScene.ConnectLocalServer();
            this.RegisterClientMessages(this.client);
            if (this.m_MigrationManager != null)
            {
                this.m_MigrationManager.Initialize(this.client, this.matchInfo);
            }
            return this.client;
        }

        private void FinishLoadScene()
        {
            if (this.client != null)
            {
                if (s_ClientReadyConnection != null)
                {
                    this.m_ClientLoadedScene = true;
                    this.OnClientConnect(s_ClientReadyConnection);
                    s_ClientReadyConnection = null;
                }
            }
            else if (LogFilter.logDev)
            {
                Debug.Log("FinishLoadScene client is null");
            }
            if (NetworkServer.active)
            {
                NetworkServer.SpawnObjects();
                this.OnServerSceneChanged(networkSceneName);
            }
            if (this.IsClientConnected() && (this.client != null))
            {
                this.RegisterClientMessages(this.client);
                this.OnClientSceneChanged(this.client.connection);
            }
        }

        /// <summary>
        /// <para>This finds a spawn position based on NetworkStartPosition objects in the scene.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the transform to spawn a player at, or null.</para>
        /// </returns>
        public Transform GetStartPosition()
        {
            if (s_StartPositions.Count > 0)
            {
                for (int i = s_StartPositions.Count - 1; i >= 0; i--)
                {
                    if (s_StartPositions[i] == null)
                    {
                        s_StartPositions.RemoveAt(i);
                    }
                }
            }
            if ((this.m_PlayerSpawnMethod == PlayerSpawnMethod.Random) && (s_StartPositions.Count > 0))
            {
                int num2 = UnityEngine.Random.Range(0, s_StartPositions.Count);
                return s_StartPositions[num2];
            }
            if ((this.m_PlayerSpawnMethod == PlayerSpawnMethod.RoundRobin) && (s_StartPositions.Count > 0))
            {
                if (s_StartPositionIndex >= s_StartPositions.Count)
                {
                    s_StartPositionIndex = 0;
                }
                Transform transform2 = s_StartPositions[s_StartPositionIndex];
                s_StartPositionIndex++;
                return transform2;
            }
            return null;
        }

        private void InitializeSingleton()
        {
            if ((singleton == null) || (singleton != this))
            {
                int logLevel = (int) this.m_LogLevel;
                if (logLevel != -1)
                {
                    LogFilter.currentLogLevel = logLevel;
                }
                if (this.m_DontDestroyOnLoad)
                {
                    if (singleton != null)
                    {
                        if (LogFilter.logDev)
                        {
                            Debug.Log("Multiple NetworkManagers detected in the scene. Only one NetworkManager can exist at a time. The duplicate NetworkManager will not be used.");
                        }
                        UnityEngine.Object.Destroy(base.gameObject);
                        return;
                    }
                    if (LogFilter.logDev)
                    {
                        Debug.Log("NetworkManager created singleton (DontDestroyOnLoad)");
                    }
                    singleton = this;
                    if (Application.isPlaying)
                    {
                        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
                    }
                }
                else
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("NetworkManager created singleton (ForScene)");
                    }
                    singleton = this;
                }
                if (this.m_NetworkAddress != "")
                {
                    s_Address = this.m_NetworkAddress;
                }
                else if (s_Address != "")
                {
                    this.m_NetworkAddress = s_Address;
                }
            }
        }

        /// <summary>
        /// <para>This checks if the NetworkManager has a client and that it is connected to a server.</para>
        /// </summary>
        /// <returns>
        /// <para>True if the NetworkManagers client is connected to a server.</para>
        /// </returns>
        public bool IsClientConnected() => 
            ((this.client != null) && this.client.isConnected);

        /// <summary>
        /// <para>Called on the client when connected to a server.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public virtual void OnClientConnect(NetworkConnection conn)
        {
            if (!this.clientLoadedScene)
            {
                ClientScene.Ready(conn);
                if (this.m_AutoCreatePlayer)
                {
                    ClientScene.AddPlayer(0);
                }
            }
        }

        internal void OnClientConnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientConnectInternal");
            }
            netMsg.conn.SetMaxDelay(this.m_MaxDelay);
            string name = SceneManager.GetSceneAt(0).name;
            if ((string.IsNullOrEmpty(this.m_OnlineScene) || (this.m_OnlineScene == this.m_OfflineScene)) || (name == this.m_OnlineScene))
            {
                this.m_ClientLoadedScene = false;
                this.OnClientConnect(netMsg.conn);
            }
            else
            {
                s_ClientReadyConnection = netMsg.conn;
            }
        }

        /// <summary>
        /// <para>Called on clients when disconnected from a server.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public virtual void OnClientDisconnect(NetworkConnection conn)
        {
            this.StopClient();
            if ((conn.lastError != NetworkError.Ok) && LogFilter.logError)
            {
                Debug.LogError("ClientDisconnected due to error: " + conn.lastError);
            }
        }

        internal void OnClientDisconnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientDisconnectInternal");
            }
            if ((this.m_MigrationManager == null) || !this.m_MigrationManager.LostHostOnClient(netMsg.conn))
            {
                if (!string.IsNullOrEmpty(this.m_OfflineScene))
                {
                    this.ClientChangeScene(this.m_OfflineScene, false);
                }
                if (((this.matchMaker != null) && (this.matchInfo != null)) && ((this.matchInfo.networkId != NetworkID.Invalid) && (this.matchInfo.nodeId != NodeID.Invalid)))
                {
                    this.matchMaker.DropConnection(this.matchInfo.networkId, this.matchInfo.nodeId, this.matchInfo.domain, new NetworkMatch.BasicResponseDelegate(this.OnDropConnection));
                }
                this.OnClientDisconnect(netMsg.conn);
            }
        }

        /// <summary>
        /// <para>Called on clients when a network error occurs.</para>
        /// </summary>
        /// <param name="conn">Connection to a server.</param>
        /// <param name="errorCode">Error code.</param>
        public virtual void OnClientError(NetworkConnection conn, int errorCode)
        {
        }

        internal void OnClientErrorInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientErrorInternal");
            }
            netMsg.ReadMessage<ErrorMessage>(s_ErrorMessage);
            this.OnClientError(netMsg.conn, s_ErrorMessage.errorCode);
        }

        /// <summary>
        /// <para>Called on clients when a servers tells the client it is no longer ready.</para>
        /// </summary>
        /// <param name="conn">Connection to a server.</param>
        public virtual void OnClientNotReady(NetworkConnection conn)
        {
        }

        internal void OnClientNotReadyMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientNotReadyMessageInternal");
            }
            ClientScene.SetNotReady();
            this.OnClientNotReady(netMsg.conn);
        }

        /// <summary>
        /// <para>Called on clients when a scene has completed loaded, when the scene load was initiated by the server.</para>
        /// </summary>
        /// <param name="conn">The network connection that the scene change message arrived on.</param>
        public virtual void OnClientSceneChanged(NetworkConnection conn)
        {
            ClientScene.Ready(conn);
            if (this.m_AutoCreatePlayer)
            {
                bool flag = ClientScene.localPlayers.Count == 0;
                bool flag2 = false;
                for (int i = 0; i < ClientScene.localPlayers.Count; i++)
                {
                    if (ClientScene.localPlayers[i].gameObject != null)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    flag = true;
                }
                if (flag)
                {
                    ClientScene.AddPlayer(0);
                }
            }
        }

        internal void OnClientSceneInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientSceneInternal");
            }
            string newSceneName = netMsg.reader.ReadString();
            if (this.IsClientConnected() && !NetworkServer.active)
            {
                this.ClientChangeScene(newSceneName, true);
            }
        }

        private void OnDestroy()
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkManager destroyed");
            }
        }

        /// <summary>
        /// <para>Callback that happens when a NetworkMatch.DestroyMatch request has been processed on the server.</para>
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        public virtual void OnDestroyMatch(bool success, string extendedInfo)
        {
            if (LogFilter.logDebug)
            {
                object[] args = new object[] { success, extendedInfo };
                Debug.LogFormat("NetworkManager OnDestroyMatch Success:{0}, ExtendedInfo:{1}", args);
            }
        }

        internal static void OnDomainReload()
        {
            s_DomainReload = true;
        }

        /// <summary>
        /// <para>Callback that happens when a NetworkMatch.DropConnection match request has been processed on the server.</para>
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        public virtual void OnDropConnection(bool success, string extendedInfo)
        {
            if (LogFilter.logDebug)
            {
                object[] args = new object[] { success, extendedInfo };
                Debug.LogFormat("NetworkManager OnDropConnection Success:{0}, ExtendedInfo:{1}", args);
            }
        }

        /// <summary>
        /// <para>Callback that happens when a NetworkMatch.CreateMatch request has been processed on the server.</para>
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        /// <param name="matchInfo">The information about the newly created match.</param>
        public virtual void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (LogFilter.logDebug)
            {
                object[] args = new object[] { success, extendedInfo, matchInfo };
                Debug.LogFormat("NetworkManager OnMatchCreate Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", args);
            }
            if (success)
            {
                this.StartHost(matchInfo);
            }
        }

        /// <summary>
        /// <para>Callback that happens when a NetworkMatch.JoinMatch request has been processed on the server.</para>
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        /// <param name="matchInfo">The info for the newly joined match.</param>
        public virtual void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (LogFilter.logDebug)
            {
                object[] args = new object[] { success, extendedInfo, matchInfo };
                Debug.LogFormat("NetworkManager OnMatchJoined Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", args);
            }
            if (success)
            {
                this.StartClient(matchInfo);
            }
        }

        public virtual void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
        {
            if (LogFilter.logDebug)
            {
                object[] args = new object[] { success, extendedInfo, matchList.Count };
                Debug.LogFormat("NetworkManager OnMatchList Success:{0}, ExtendedInfo:{1}, matchList.Count:{2}", args);
            }
            this.matches = matchList;
        }

        /// <summary>
        /// <para>Called on the server when a client adds a new player with ClientScene.AddPlayer.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        /// <param name="playerControllerId">Id of the new player.</param>
        /// <param name="extraMessageReader">An extra message object passed for the new player.</param>
        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            this.OnServerAddPlayerInternal(conn, playerControllerId);
        }

        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            this.OnServerAddPlayerInternal(conn, playerControllerId);
        }

        private void OnServerAddPlayerInternal(NetworkConnection conn, short playerControllerId)
        {
            if (this.m_PlayerPrefab == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
                }
            }
            else if (this.m_PlayerPrefab.GetComponent<NetworkIdentity>() == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
                }
            }
            else if (((playerControllerId < conn.playerControllers.Count) && conn.playerControllers[playerControllerId].IsValid) && (conn.playerControllers[playerControllerId].gameObject != null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("There is already a player at that playerControllerId for this connections.");
                }
            }
            else
            {
                GameObject obj2;
                Transform startPosition = this.GetStartPosition();
                if (startPosition != null)
                {
                    obj2 = UnityEngine.Object.Instantiate<GameObject>(this.m_PlayerPrefab, startPosition.position, startPosition.rotation);
                }
                else
                {
                    obj2 = UnityEngine.Object.Instantiate<GameObject>(this.m_PlayerPrefab, Vector3.zero, Quaternion.identity);
                }
                NetworkServer.AddPlayerForConnection(conn, obj2, playerControllerId);
            }
        }

        internal void OnServerAddPlayerMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerAddPlayerMessageInternal");
            }
            netMsg.ReadMessage<AddPlayerMessage>(s_AddPlayerMessage);
            if (s_AddPlayerMessage.msgSize != 0)
            {
                NetworkReader extraMessageReader = new NetworkReader(s_AddPlayerMessage.msgData);
                this.OnServerAddPlayer(netMsg.conn, s_AddPlayerMessage.playerControllerId, extraMessageReader);
            }
            else
            {
                this.OnServerAddPlayer(netMsg.conn, s_AddPlayerMessage.playerControllerId);
            }
            if (this.m_MigrationManager != null)
            {
                this.m_MigrationManager.SendPeerInfo();
            }
        }

        /// <summary>
        /// <para>Called on the server when a new client connects.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public virtual void OnServerConnect(NetworkConnection conn)
        {
        }

        internal void OnServerConnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerConnectInternal");
            }
            netMsg.conn.SetMaxDelay(this.m_MaxDelay);
            if (this.m_MaxBufferedPackets != 0x200)
            {
                for (int i = 0; i < NetworkServer.numChannels; i++)
                {
                    netMsg.conn.SetChannelOption(i, ChannelOption.MaxPendingBuffers, this.m_MaxBufferedPackets);
                }
            }
            if (!this.m_AllowFragmentation)
            {
                for (int j = 0; j < NetworkServer.numChannels; j++)
                {
                    netMsg.conn.SetChannelOption(j, ChannelOption.AllowFragmentation, 0);
                }
            }
            if ((networkSceneName != "") && (networkSceneName != this.m_OfflineScene))
            {
                StringMessage msg = new StringMessage(networkSceneName);
                netMsg.conn.Send(0x27, msg);
            }
            if (this.m_MigrationManager != null)
            {
                this.m_MigrationManager.SendPeerInfo();
            }
            this.OnServerConnect(netMsg.conn);
        }

        /// <summary>
        /// <para>Called on the server when a client disconnects.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public virtual void OnServerDisconnect(NetworkConnection conn)
        {
            NetworkServer.DestroyPlayersForConnection(conn);
            if ((conn.lastError != NetworkError.Ok) && LogFilter.logError)
            {
                Debug.LogError("ServerDisconnected due to error: " + conn.lastError);
            }
        }

        internal void OnServerDisconnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerDisconnectInternal");
            }
            if (this.m_MigrationManager != null)
            {
                this.m_MigrationManager.SendPeerInfo();
            }
            this.OnServerDisconnect(netMsg.conn);
        }

        /// <summary>
        /// <para>Called on the server when a network error occurs for a client connection.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        /// <param name="errorCode">Error code.</param>
        public virtual void OnServerError(NetworkConnection conn, int errorCode)
        {
        }

        internal void OnServerErrorInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerErrorInternal");
            }
            netMsg.ReadMessage<ErrorMessage>(s_ErrorMessage);
            this.OnServerError(netMsg.conn, s_ErrorMessage.errorCode);
        }

        /// <summary>
        /// <para>Called on the server when a client is ready.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public virtual void OnServerReady(NetworkConnection conn)
        {
            if ((conn.playerControllers.Count == 0) && LogFilter.logDebug)
            {
                Debug.Log("Ready with no player object");
            }
            NetworkServer.SetClientReady(conn);
        }

        internal void OnServerReadyMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerReadyMessageInternal");
            }
            this.OnServerReady(netMsg.conn);
        }

        /// <summary>
        /// <para>Called on the server when a client removes a player.</para>
        /// </summary>
        /// <param name="conn">The connection to remove the player from.</param>
        /// <param name="player">The player controller to remove.</param>
        public virtual void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            if (player.gameObject != null)
            {
                NetworkServer.Destroy(player.gameObject);
            }
        }

        internal void OnServerRemovePlayerMessageInternal(NetworkMessage netMsg)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerRemovePlayerMessageInternal");
            }
            netMsg.ReadMessage<RemovePlayerMessage>(s_RemovePlayerMessage);
            netMsg.conn.GetPlayerController(s_RemovePlayerMessage.playerControllerId, out controller);
            this.OnServerRemovePlayer(netMsg.conn, controller);
            netMsg.conn.RemovePlayerController(s_RemovePlayerMessage.playerControllerId);
            if (this.m_MigrationManager != null)
            {
                this.m_MigrationManager.SendPeerInfo();
            }
        }

        /// <summary>
        /// <para>Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().</para>
        /// </summary>
        /// <param name="sceneName">The name of the new scene.</param>
        public virtual void OnServerSceneChanged(string sceneName)
        {
        }

        /// <summary>
        /// <para>Callback that happens when a NetworkMatch.SetMatchAttributes has been processed on the server.</para>
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        public virtual void OnSetMatchAttributes(bool success, string extendedInfo)
        {
            if (LogFilter.logDebug)
            {
                object[] args = new object[] { success, extendedInfo };
                Debug.LogFormat("NetworkManager OnSetMatchAttributes Success:{0}, ExtendedInfo:{1}", args);
            }
        }

        /// <summary>
        /// <para>This is a hook that is invoked when the client is started.</para>
        /// </summary>
        /// <param name="client">The NetworkClient object that was started.</param>
        public virtual void OnStartClient(NetworkClient client)
        {
        }

        /// <summary>
        /// <para>This hook is invoked when a host is started.</para>
        /// </summary>
        public virtual void OnStartHost()
        {
        }

        /// <summary>
        /// <para>This hook is invoked when a server is started - including when a host is started.</para>
        /// </summary>
        public virtual void OnStartServer()
        {
        }

        /// <summary>
        /// <para>This hook is called when a client is stopped.</para>
        /// </summary>
        public virtual void OnStopClient()
        {
        }

        /// <summary>
        /// <para>This hook is called when a host is stopped.</para>
        /// </summary>
        public virtual void OnStopHost()
        {
        }

        /// <summary>
        /// <para>This hook is called when a server is stopped - including when a host is stopped.</para>
        /// </summary>
        public virtual void OnStopServer()
        {
        }

        private void OnValidate()
        {
            if (this.m_SimulatedLatency < 1)
            {
                this.m_SimulatedLatency = 1;
            }
            if (this.m_SimulatedLatency > 500)
            {
                this.m_SimulatedLatency = 500;
            }
            if (this.m_PacketLossPercentage < 0f)
            {
                this.m_PacketLossPercentage = 0f;
            }
            if (this.m_PacketLossPercentage > 99f)
            {
                this.m_PacketLossPercentage = 99f;
            }
            if (this.m_MaxConnections <= 0)
            {
                this.m_MaxConnections = 1;
            }
            if (this.m_MaxConnections > 0x7d00)
            {
                this.m_MaxConnections = 0x7d00;
            }
            if (this.m_MaxBufferedPackets <= 0)
            {
                this.m_MaxBufferedPackets = 0;
            }
            if (this.m_MaxBufferedPackets > 0x200)
            {
                this.m_MaxBufferedPackets = 0x200;
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkManager - MaxBufferedPackets cannot be more than " + 0x200);
                }
            }
            if ((this.m_PlayerPrefab != null) && (this.m_PlayerPrefab.GetComponent<NetworkIdentity>() == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkManager - playerPrefab must have a NetworkIdentity.");
                }
                this.m_PlayerPrefab = null;
            }
            if (this.m_ConnectionConfig.MinUpdateTimeout <= 0)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkManager MinUpdateTimeout cannot be zero or less. The value will be reset to 1 millisecond");
                }
                this.m_ConnectionConfig.MinUpdateTimeout = 1;
            }
            if ((this.m_GlobalConfig != null) && (this.m_GlobalConfig.ThreadAwakeTimeout <= 0))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkManager ThreadAwakeTimeout cannot be zero or less. The value will be reset to 1 millisecond");
                }
                this.m_GlobalConfig.ThreadAwakeTimeout = 1;
            }
        }

        internal void RegisterClientMessages(NetworkClient client)
        {
            client.RegisterHandler(0x20, new NetworkMessageDelegate(this.OnClientConnectInternal));
            client.RegisterHandler(0x21, new NetworkMessageDelegate(this.OnClientDisconnectInternal));
            client.RegisterHandler(0x24, new NetworkMessageDelegate(this.OnClientNotReadyMessageInternal));
            client.RegisterHandler(0x22, new NetworkMessageDelegate(this.OnClientErrorInternal));
            client.RegisterHandler(0x27, new NetworkMessageDelegate(this.OnClientSceneInternal));
            if (this.m_PlayerPrefab != null)
            {
                ClientScene.RegisterPrefab(this.m_PlayerPrefab);
            }
            for (int i = 0; i < this.m_SpawnPrefabs.Count; i++)
            {
                GameObject prefab = this.m_SpawnPrefabs[i];
                if (prefab != null)
                {
                    ClientScene.RegisterPrefab(prefab);
                }
            }
        }

        internal void RegisterServerMessages()
        {
            NetworkServer.RegisterHandler(0x20, new NetworkMessageDelegate(this.OnServerConnectInternal));
            NetworkServer.RegisterHandler(0x21, new NetworkMessageDelegate(this.OnServerDisconnectInternal));
            NetworkServer.RegisterHandler(0x23, new NetworkMessageDelegate(this.OnServerReadyMessageInternal));
            NetworkServer.RegisterHandler(0x25, new NetworkMessageDelegate(this.OnServerAddPlayerMessageInternal));
            NetworkServer.RegisterHandler(0x26, new NetworkMessageDelegate(this.OnServerRemovePlayerMessageInternal));
            NetworkServer.RegisterHandler(0x22, new NetworkMessageDelegate(this.OnServerErrorInternal));
        }

        /// <summary>
        /// <para>Registers the transform of a game object as a player spawn location.</para>
        /// </summary>
        /// <param name="start">Transform to register.</param>
        public static void RegisterStartPosition(Transform start)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("RegisterStartPosition:" + start);
            }
            s_StartPositions.Add(start);
        }

        /// <summary>
        /// <para>This causes the server to switch scenes and sets the networkSceneName.</para>
        /// </summary>
        /// <param name="newSceneName">The name of the scene to change to. The server will change scene immediately, and a message will be sent to connected clients to ask them to change scene also.</param>
        public virtual void ServerChangeScene(string newSceneName)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ServerChangeScene empty scene name");
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ServerChangeScene " + newSceneName);
                }
                NetworkServer.SetAllClientsNotReady();
                networkSceneName = newSceneName;
                s_LoadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
                StringMessage msg = new StringMessage(networkSceneName);
                NetworkServer.SendToAll(0x27, msg);
                s_StartPositionIndex = 0;
                s_StartPositions.Clear();
            }
        }

        /// <summary>
        /// <para>This sets the address of the MatchMaker service.</para>
        /// </summary>
        /// <param name="newHost">Hostname of MatchMaker service.</param>
        /// <param name="port">Port of MatchMaker service.</param>
        /// <param name="https">Protocol used by MatchMaker service.</param>
        public void SetMatchHost(string newHost, int port, bool https)
        {
            if (this.matchMaker == null)
            {
                this.matchMaker = base.gameObject.AddComponent<NetworkMatch>();
            }
            if (newHost == "127.0.0.1")
            {
                newHost = "localhost";
            }
            string str = "http://";
            if (https)
            {
                str = "https://";
            }
            if (newHost.StartsWith("http://"))
            {
                newHost = newHost.Replace("http://", "");
            }
            if (newHost.StartsWith("https://"))
            {
                newHost = newHost.Replace("https://", "");
            }
            this.m_MatchHost = newHost;
            this.m_MatchPort = port;
            object[] objArray1 = new object[] { str, this.m_MatchHost, ":", this.m_MatchPort };
            string uriString = string.Concat(objArray1);
            if (LogFilter.logDebug)
            {
                Debug.Log("SetMatchHost:" + uriString);
            }
            this.matchMaker.baseUri = new Uri(uriString);
        }

        /// <summary>
        /// <para>This sets up a NetworkMigrationManager object to work with this NetworkManager.</para>
        /// </summary>
        /// <param name="man">The migration manager object to use with the NetworkManager.</param>
        public void SetupMigrationManager(NetworkMigrationManager man)
        {
            this.m_MigrationManager = man;
        }

        /// <summary>
        /// <para>Shuts down the NetworkManager completely and destroy the singleton.</para>
        /// </summary>
        public static void Shutdown()
        {
            if (singleton != null)
            {
                s_StartPositions.Clear();
                s_StartPositionIndex = 0;
                s_ClientReadyConnection = null;
                singleton.StopHost();
                singleton = null;
            }
        }

        /// <summary>
        /// <para>This starts a network client. It uses the networkAddress and networkPort properties as the address to connect to.</para>
        /// </summary>
        /// <returns>
        /// <para>The client object created.</para>
        /// </returns>
        public NetworkClient StartClient() => 
            this.StartClient(null, null);

        public NetworkClient StartClient(MatchInfo matchInfo) => 
            this.StartClient(matchInfo, null);

        public NetworkClient StartClient(MatchInfo info, ConnectionConfig config)
        {
            this.InitializeSingleton();
            this.matchInfo = info;
            if (this.m_RunInBackground)
            {
                Application.runInBackground = true;
            }
            this.isNetworkActive = true;
            if (this.m_GlobalConfig != null)
            {
                NetworkTransport.Init(this.m_GlobalConfig);
            }
            this.client = new NetworkClient();
            if (config != null)
            {
                if ((config.UsePlatformSpecificProtocols && (Application.platform != RuntimePlatform.PS4)) && (Application.platform != RuntimePlatform.PSP2))
                {
                    throw new ArgumentOutOfRangeException("Platform specific protocols are not supported on this platform");
                }
                this.client.Configure(config, 1);
            }
            else if (this.m_CustomConfig && (this.m_ConnectionConfig != null))
            {
                this.m_ConnectionConfig.Channels.Clear();
                for (int i = 0; i < this.m_Channels.Count; i++)
                {
                    this.m_ConnectionConfig.AddChannel(this.m_Channels[i]);
                }
                if ((this.m_ConnectionConfig.UsePlatformSpecificProtocols && (Application.platform != RuntimePlatform.PS4)) && (Application.platform != RuntimePlatform.PSP2))
                {
                    throw new ArgumentOutOfRangeException("Platform specific protocols are not supported on this platform");
                }
                this.client.Configure(this.m_ConnectionConfig, this.m_MaxConnections);
            }
            this.RegisterClientMessages(this.client);
            if (this.matchInfo != null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkManager StartClient match: " + this.matchInfo);
                }
                this.client.Connect(this.matchInfo);
            }
            else if (this.m_EndPoint != null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkManager StartClient using provided SecureTunnel");
                }
                this.client.Connect(this.m_EndPoint);
            }
            else
            {
                if (string.IsNullOrEmpty(this.m_NetworkAddress))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Must set the Network Address field in the manager");
                    }
                    return null;
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "NetworkManager StartClient address:", this.m_NetworkAddress, " port:", this.m_NetworkPort }));
                }
                if (this.m_UseSimulator)
                {
                    this.client.ConnectWithSimulator(this.m_NetworkAddress, this.m_NetworkPort, this.m_SimulatedLatency, this.m_PacketLossPercentage);
                }
                else
                {
                    this.client.Connect(this.m_NetworkAddress, this.m_NetworkPort);
                }
            }
            if (this.m_MigrationManager != null)
            {
                this.m_MigrationManager.Initialize(this.client, this.matchInfo);
            }
            this.OnStartClient(this.client);
            s_Address = this.m_NetworkAddress;
            return this.client;
        }

        /// <summary>
        /// <para>This starts a network "host" - a server and client in the same application.</para>
        /// </summary>
        /// <returns>
        /// <para>The client object created - this is a "local client".</para>
        /// </returns>
        public virtual NetworkClient StartHost()
        {
            this.OnStartHost();
            if (this.StartServer())
            {
                NetworkClient client = this.ConnectLocalClient();
                this.OnStartClient(client);
                return client;
            }
            return null;
        }

        public virtual NetworkClient StartHost(MatchInfo info)
        {
            this.OnStartHost();
            this.matchInfo = info;
            if (this.StartServer(info))
            {
                NetworkClient client = this.ConnectLocalClient();
                this.OnStartClient(client);
                return client;
            }
            return null;
        }

        public virtual NetworkClient StartHost(ConnectionConfig config, int maxConnections)
        {
            this.OnStartHost();
            if (this.StartServer(config, maxConnections))
            {
                NetworkClient client = this.ConnectLocalClient();
                this.OnServerConnect(client.connection);
                this.OnStartClient(client);
                return client;
            }
            return null;
        }

        /// <summary>
        /// <para>This starts MatchMaker for the NetworkManager.</para>
        /// </summary>
        public void StartMatchMaker()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StartMatchMaker");
            }
            this.SetMatchHost(this.m_MatchHost, this.m_MatchPort, this.m_MatchPort == 0x1bb);
        }

        /// <summary>
        /// <para>This starts a new server.</para>
        /// </summary>
        /// <returns>
        /// <para>True is the server was started.</para>
        /// </returns>
        public bool StartServer() => 
            this.StartServer(null);

        public bool StartServer(MatchInfo info) => 
            this.StartServer(info, null, -1);

        public bool StartServer(ConnectionConfig config, int maxConnections) => 
            this.StartServer(null, config, maxConnections);

        private bool StartServer(MatchInfo info, ConnectionConfig config, int maxConnections)
        {
            this.InitializeSingleton();
            this.OnStartServer();
            if (this.m_RunInBackground)
            {
                Application.runInBackground = true;
            }
            NetworkCRC.scriptCRCCheck = this.scriptCRCCheck;
            NetworkServer.useWebSockets = this.m_UseWebSockets;
            if (this.m_GlobalConfig != null)
            {
                NetworkTransport.Init(this.m_GlobalConfig);
            }
            if ((this.m_CustomConfig && (this.m_ConnectionConfig != null)) && (config == null))
            {
                this.m_ConnectionConfig.Channels.Clear();
                for (int i = 0; i < this.m_Channels.Count; i++)
                {
                    this.m_ConnectionConfig.AddChannel(this.m_Channels[i]);
                }
                NetworkServer.Configure(this.m_ConnectionConfig, this.m_MaxConnections);
            }
            if (config != null)
            {
                NetworkServer.Configure(config, maxConnections);
            }
            if (info != null)
            {
                if (!NetworkServer.Listen(info, this.m_NetworkPort))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("StartServer listen failed.");
                    }
                    return false;
                }
            }
            else if (this.m_ServerBindToIP && !string.IsNullOrEmpty(this.m_ServerBindAddress))
            {
                if (!NetworkServer.Listen(this.m_ServerBindAddress, this.m_NetworkPort))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("StartServer listen on " + this.m_ServerBindAddress + " failed.");
                    }
                    return false;
                }
            }
            else if (!NetworkServer.Listen(this.m_NetworkPort))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("StartServer listen failed.");
                }
                return false;
            }
            this.RegisterServerMessages();
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StartServer port:" + this.m_NetworkPort);
            }
            this.isNetworkActive = true;
            string name = SceneManager.GetSceneAt(0).name;
            if ((!string.IsNullOrEmpty(this.m_OnlineScene) && (this.m_OnlineScene != name)) && (this.m_OnlineScene != this.m_OfflineScene))
            {
                this.ServerChangeScene(this.m_OnlineScene);
            }
            else
            {
                NetworkServer.SpawnObjects();
            }
            return true;
        }

        /// <summary>
        /// <para>Stops the client that the manager is using.</para>
        /// </summary>
        public void StopClient()
        {
            this.OnStopClient();
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StopClient");
            }
            this.isNetworkActive = false;
            if (this.client != null)
            {
                this.client.Disconnect();
                this.client.Shutdown();
                this.client = null;
            }
            this.StopMatchMaker();
            ClientScene.DestroyAllClientObjects();
            if (!string.IsNullOrEmpty(this.m_OfflineScene))
            {
                this.ClientChangeScene(this.m_OfflineScene, false);
            }
            this.CleanupNetworkIdentities();
        }

        /// <summary>
        /// <para>This stops both the client and the server that the manager is using.</para>
        /// </summary>
        public void StopHost()
        {
            bool active = NetworkServer.active;
            this.OnStopHost();
            this.StopServer();
            this.StopClient();
            if ((this.m_MigrationManager != null) && active)
            {
                this.m_MigrationManager.LostHostOnHost();
            }
        }

        /// <summary>
        /// <para>Stops the MatchMaker that the NetworkManager is using.</para>
        /// </summary>
        public void StopMatchMaker()
        {
            if (((this.matchMaker != null) && (this.matchInfo != null)) && ((this.matchInfo.networkId != NetworkID.Invalid) && (this.matchInfo.nodeId != NodeID.Invalid)))
            {
                this.matchMaker.DropConnection(this.matchInfo.networkId, this.matchInfo.nodeId, this.matchInfo.domain, new NetworkMatch.BasicResponseDelegate(this.OnDropConnection));
            }
            if (this.matchMaker != null)
            {
                UnityEngine.Object.Destroy(this.matchMaker);
                this.matchMaker = null;
            }
            this.matchInfo = null;
            this.matches = null;
        }

        /// <summary>
        /// <para>Stops the server that the manager is using.</para>
        /// </summary>
        public void StopServer()
        {
            if (NetworkServer.active)
            {
                this.OnStopServer();
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkManager StopServer");
                }
                this.isNetworkActive = false;
                NetworkServer.Shutdown();
                this.StopMatchMaker();
                if (!string.IsNullOrEmpty(this.m_OfflineScene))
                {
                    this.ServerChangeScene(this.m_OfflineScene);
                }
                this.CleanupNetworkIdentities();
            }
        }

        /// <summary>
        /// <para>Unregisters the transform of a game object as a player spawn location.</para>
        /// </summary>
        /// <param name="start"></param>
        public static void UnRegisterStartPosition(Transform start)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("UnRegisterStartPosition:" + start);
            }
            s_StartPositions.Remove(start);
        }

        internal static void UpdateScene()
        {
            if (((singleton == null) && (s_PendingSingleton != null)) && s_DomainReload)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("NetworkManager detected a script reload in the editor. This has caused the network to be shut down.");
                }
                s_DomainReload = false;
                s_PendingSingleton.InitializeSingleton();
                NetworkIdentity[] identityArray = UnityEngine.Object.FindObjectsOfType<NetworkIdentity>();
                foreach (NetworkIdentity identity in identityArray)
                {
                    UnityEngine.Object.Destroy(identity.gameObject);
                }
                singleton.StopHost();
                NetworkTransport.Shutdown();
            }
            if (((singleton != null) && (s_LoadingSceneAsync != null)) && s_LoadingSceneAsync.isDone)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ClientChangeScene done readyCon:" + s_ClientReadyConnection);
                }
                singleton.FinishLoadScene();
                s_LoadingSceneAsync.allowSceneActivation = true;
                s_LoadingSceneAsync = null;
            }
        }

        /// <summary>
        /// <para>This allows the NetworkManager to use a client object created externally to the NetworkManager instead of using StartClient().</para>
        /// </summary>
        /// <param name="externalClient">The NetworkClient object to use.</param>
        public void UseExternalClient(NetworkClient externalClient)
        {
            if (this.m_RunInBackground)
            {
                Application.runInBackground = true;
            }
            if (externalClient != null)
            {
                this.client = externalClient;
                this.isNetworkActive = true;
                this.RegisterClientMessages(this.client);
                this.OnStartClient(this.client);
            }
            else
            {
                this.OnStopClient();
                ClientScene.DestroyAllClientObjects();
                ClientScene.HandleClientDisconnect(this.client.connection);
                this.client = null;
                if (!string.IsNullOrEmpty(this.m_OfflineScene))
                {
                    this.ClientChangeScene(this.m_OfflineScene, false);
                }
            }
            s_Address = this.m_NetworkAddress;
        }

        /// <summary>
        /// <para>A flag to control whether or not player objects are automatically created on connect, and on scene change.</para>
        /// </summary>
        public bool autoCreatePlayer
        {
            get => 
                this.m_AutoCreatePlayer;
            set
            {
                this.m_AutoCreatePlayer = value;
            }
        }

        /// <summary>
        /// <para>The Quality-of-Service channels to use for the network transport layer.</para>
        /// </summary>
        public List<QosType> channels =>
            this.m_Channels;

        /// <summary>
        /// <para>This is true if the client loaded a new scene when connecting to the server.</para>
        /// </summary>
        public bool clientLoadedScene
        {
            get => 
                this.m_ClientLoadedScene;
            set
            {
                this.m_ClientLoadedScene = value;
            }
        }

        /// <summary>
        /// <para>The custom network configuration to use.</para>
        /// </summary>
        public ConnectionConfig connectionConfig
        {
            get
            {
                if (this.m_ConnectionConfig == null)
                {
                    this.m_ConnectionConfig = new ConnectionConfig();
                }
                return this.m_ConnectionConfig;
            }
        }

        /// <summary>
        /// <para>Flag to enable custom network configuration.</para>
        /// </summary>
        public bool customConfig
        {
            get => 
                this.m_CustomConfig;
            set
            {
                this.m_CustomConfig = value;
            }
        }

        /// <summary>
        /// <para>A flag to control whether the NetworkManager object is destroyed when the scene changes.</para>
        /// </summary>
        public bool dontDestroyOnLoad
        {
            get => 
                this.m_DontDestroyOnLoad;
            set
            {
                this.m_DontDestroyOnLoad = value;
            }
        }

        /// <summary>
        /// <para>The transport layer global configuration to be used.</para>
        /// </summary>
        public GlobalConfig globalConfig
        {
            get
            {
                if (this.m_GlobalConfig == null)
                {
                    this.m_GlobalConfig = new GlobalConfig();
                }
                return this.m_GlobalConfig;
            }
        }

        /// <summary>
        /// <para>The log level specifically to user for network log messages.</para>
        /// </summary>
        public LogFilter.FilterLevel logLevel
        {
            get => 
                this.m_LogLevel;
            set
            {
                this.m_LogLevel = value;
                LogFilter.currentLogLevel = (int) value;
            }
        }

        /// <summary>
        /// <para>The hostname of the matchmaking server.</para>
        /// </summary>
        public string matchHost
        {
            get => 
                this.m_MatchHost;
            set
            {
                this.m_MatchHost = value;
            }
        }

        /// <summary>
        /// <para>The port of the matchmaking service.</para>
        /// </summary>
        public int matchPort
        {
            get => 
                this.m_MatchPort;
            set
            {
                this.m_MatchPort = value;
            }
        }

        /// <summary>
        /// <para>The maximum number of concurrent network connections to support.</para>
        /// </summary>
        public int maxConnections
        {
            get => 
                this.m_MaxConnections;
            set
            {
                this.m_MaxConnections = value;
            }
        }

        /// <summary>
        /// <para>The maximum delay before sending packets on connections.</para>
        /// </summary>
        public float maxDelay
        {
            get => 
                this.m_MaxDelay;
            set
            {
                this.m_MaxDelay = value;
            }
        }

        /// <summary>
        /// <para>The migration manager being used with the NetworkManager.</para>
        /// </summary>
        public NetworkMigrationManager migrationManager =>
            this.m_MigrationManager;

        /// <summary>
        /// <para>The network address currently in use.</para>
        /// </summary>
        public string networkAddress
        {
            get => 
                this.m_NetworkAddress;
            set
            {
                this.m_NetworkAddress = value;
            }
        }

        /// <summary>
        /// <para>The network port currently in use.</para>
        /// </summary>
        public int networkPort
        {
            get => 
                this.m_NetworkPort;
            set
            {
                this.m_NetworkPort = value;
            }
        }

        /// <summary>
        /// <para>NumPlayers is the number of active player objects across all connections on the server.</para>
        /// </summary>
        public int numPlayers
        {
            get
            {
                int num = 0;
                for (int i = 0; i < NetworkServer.connections.Count; i++)
                {
                    NetworkConnection connection = NetworkServer.connections[i];
                    if (connection != null)
                    {
                        for (int j = 0; j < connection.playerControllers.Count; j++)
                        {
                            if (connection.playerControllers[j].IsValid)
                            {
                                num++;
                            }
                        }
                    }
                }
                return num;
            }
        }

        /// <summary>
        /// <para>The scene to switch to when offline.</para>
        /// </summary>
        public string offlineScene
        {
            get => 
                this.m_OfflineScene;
            set
            {
                this.m_OfflineScene = value;
            }
        }

        /// <summary>
        /// <para>The scene to switch to when online.</para>
        /// </summary>
        public string onlineScene
        {
            get => 
                this.m_OnlineScene;
            set
            {
                this.m_OnlineScene = value;
            }
        }

        /// <summary>
        /// <para>The percentage of incoming and outgoing packets to be dropped for clients.</para>
        /// </summary>
        public float packetLossPercentage
        {
            get => 
                this.m_PacketLossPercentage;
            set
            {
                this.m_PacketLossPercentage = value;
            }
        }

        /// <summary>
        /// <para>The default prefab to be used to create player objects on the server.</para>
        /// </summary>
        public GameObject playerPrefab
        {
            get => 
                this.m_PlayerPrefab;
            set
            {
                this.m_PlayerPrefab = value;
            }
        }

        /// <summary>
        /// <para>The current method of spawning players used by the NetworkManager.</para>
        /// </summary>
        public PlayerSpawnMethod playerSpawnMethod
        {
            get => 
                this.m_PlayerSpawnMethod;
            set
            {
                this.m_PlayerSpawnMethod = value;
            }
        }

        /// <summary>
        /// <para>Controls whether the program runs when it is in the background.</para>
        /// </summary>
        public bool runInBackground
        {
            get => 
                this.m_RunInBackground;
            set
            {
                this.m_RunInBackground = value;
            }
        }

        /// <summary>
        /// <para>Flag for using the script CRC check between server and clients.</para>
        /// </summary>
        public bool scriptCRCCheck
        {
            get => 
                this.m_ScriptCRCCheck;
            set
            {
                this.m_ScriptCRCCheck = value;
            }
        }

        /// <summary>
        /// <para>Allows you to specify an EndPoint object instead of setting networkAddress and networkPort (required for some platforms such as Xbox One).</para>
        /// </summary>
        public EndPoint secureTunnelEndpoint
        {
            get => 
                this.m_EndPoint;
            set
            {
                this.m_EndPoint = value;
            }
        }

        /// <summary>
        /// <para>A flag to control sending the network information about every peer to all members of a match.</para>
        /// </summary>
        [Obsolete("moved to NetworkMigrationManager")]
        public bool sendPeerInfo
        {
            get => 
                false;
            set
            {
            }
        }

        /// <summary>
        /// <para>The IP address to bind the server to.</para>
        /// </summary>
        public string serverBindAddress
        {
            get => 
                this.m_ServerBindAddress;
            set
            {
                this.m_ServerBindAddress = value;
            }
        }

        /// <summary>
        /// <para>Flag to tell the server whether to bind to a specific IP address.</para>
        /// </summary>
        public bool serverBindToIP
        {
            get => 
                this.m_ServerBindToIP;
            set
            {
                this.m_ServerBindToIP = value;
            }
        }

        /// <summary>
        /// <para>The delay in milliseconds to be added to incoming and outgoing packets for clients.</para>
        /// </summary>
        public int simulatedLatency
        {
            get => 
                this.m_SimulatedLatency;
            set
            {
                this.m_SimulatedLatency = value;
            }
        }

        /// <summary>
        /// <para>List of prefabs that will be registered with the spawning system.</para>
        /// </summary>
        public List<GameObject> spawnPrefabs =>
            this.m_SpawnPrefabs;

        /// <summary>
        /// <para>The list of currently registered player start positions for the current scene.</para>
        /// </summary>
        public List<Transform> startPositions =>
            s_StartPositions;

        /// <summary>
        /// <para>Flag that control whether clients started by this NetworkManager will use simulated latency and packet loss.</para>
        /// </summary>
        public bool useSimulator
        {
            get => 
                this.m_UseSimulator;
            set
            {
                this.m_UseSimulator = value;
            }
        }

        /// <summary>
        /// <para>This makes the NetworkServer listen for WebSockets connections instead of normal transport layer connections.</para>
        /// </summary>
        public bool useWebSockets
        {
            get => 
                this.m_UseWebSockets;
            set
            {
                this.m_UseWebSockets = value;
            }
        }
    }
}

