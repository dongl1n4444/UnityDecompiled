namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// <para>This is a specialized NetworkManager that includes a networked lobby.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkLobbyManager")]
    public class NetworkLobbyManager : NetworkManager
    {
        /// <summary>
        /// <para>These slots track players that enter the lobby.</para>
        /// </summary>
        public NetworkLobbyPlayer[] lobbySlots;
        [SerializeField]
        private GameObject m_GamePlayerPrefab;
        [SerializeField]
        private NetworkLobbyPlayer m_LobbyPlayerPrefab;
        [SerializeField]
        private string m_LobbyScene = "";
        [SerializeField]
        private int m_MaxPlayers = 4;
        [SerializeField]
        private int m_MaxPlayersPerConnection = 1;
        [SerializeField]
        private int m_MinPlayers;
        private List<PendingPlayer> m_PendingPlayers = new List<PendingPlayer>();
        [SerializeField]
        private string m_PlayScene = "";
        [SerializeField]
        private bool m_ShowLobbyGUI = true;
        private static LobbyReadyToBeginMessage s_LobbyReadyToBeginMessage = new LobbyReadyToBeginMessage();
        private static LobbyReadyToBeginMessage s_ReadyToBeginMessage = new LobbyReadyToBeginMessage();
        private static IntegerMessage s_SceneLoadedMessage = new IntegerMessage();

        private void CallOnClientEnterLobby()
        {
            this.OnLobbyClientEnter();
            for (int i = 0; i < this.lobbySlots.Length; i++)
            {
                NetworkLobbyPlayer player = this.lobbySlots[i];
                if (player != null)
                {
                    player.readyToBegin = false;
                    player.OnClientEnterLobby();
                }
            }
        }

        private void CallOnClientExitLobby()
        {
            this.OnLobbyClientExit();
            for (int i = 0; i < this.lobbySlots.Length; i++)
            {
                NetworkLobbyPlayer player = this.lobbySlots[i];
                if (player != null)
                {
                    player.OnClientExitLobby();
                }
            }
        }

        private static int CheckConnectionIsReadyToBegin(NetworkConnection conn)
        {
            int num = 0;
            for (int i = 0; i < conn.playerControllers.Count; i++)
            {
                PlayerController controller = conn.playerControllers[i];
                if (controller.IsValid && controller.gameObject.GetComponent<NetworkLobbyPlayer>().readyToBegin)
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// <para>CheckReadyToBegin checks all of the players in the lobby to see if their readyToBegin flag is set.</para>
        /// </summary>
        public void CheckReadyToBegin()
        {
            if (SceneManager.GetSceneAt(0).name == this.m_LobbyScene)
            {
                int num = 0;
                int num2 = 0;
                for (int i = 0; i < NetworkServer.connections.Count; i++)
                {
                    NetworkConnection conn = NetworkServer.connections[i];
                    if (conn != null)
                    {
                        num2++;
                        num += CheckConnectionIsReadyToBegin(conn);
                    }
                }
                if (((this.m_MinPlayers <= 0) || (num >= this.m_MinPlayers)) && (num >= num2))
                {
                    this.m_PendingPlayers.Clear();
                    this.OnLobbyServerPlayersReady();
                }
            }
        }

        private byte FindSlot()
        {
            for (byte i = 0; i < this.maxPlayers; i = (byte) (i + 1))
            {
                if (this.lobbySlots[i] == null)
                {
                    return i;
                }
            }
            return 0xff;
        }

        private void OnClientAddPlayerFailedMessage(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager Add Player failed.");
            }
            this.OnLobbyClientAddPlayerFailed();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            this.OnLobbyClientConnect(conn);
            this.CallOnClientEnterLobby();
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            this.OnLobbyClientDisconnect(conn);
            base.OnClientDisconnect(conn);
        }

        private void OnClientReadyToBegin(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<LobbyReadyToBeginMessage>(s_LobbyReadyToBeginMessage);
            if (s_LobbyReadyToBeginMessage.slotId >= this.lobbySlots.Count<NetworkLobbyPlayer>())
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager OnClientReadyToBegin invalid lobby slot " + s_LobbyReadyToBeginMessage.slotId);
                }
            }
            else
            {
                NetworkLobbyPlayer player = this.lobbySlots[s_LobbyReadyToBeginMessage.slotId];
                if ((player == null) || (player.gameObject == null))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkLobbyManager OnClientReadyToBegin no player at lobby slot " + s_LobbyReadyToBeginMessage.slotId);
                    }
                }
                else
                {
                    player.readyToBegin = s_LobbyReadyToBeginMessage.readyState;
                    player.OnClientReady(s_LobbyReadyToBeginMessage.readyState);
                }
            }
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == this.m_LobbyScene)
            {
                if (base.client.isConnected)
                {
                    this.CallOnClientEnterLobby();
                }
            }
            else
            {
                this.CallOnClientExitLobby();
            }
            base.OnClientSceneChanged(conn);
            this.OnLobbyClientSceneChanged(conn);
        }

        private void OnGUI()
        {
            if (this.showLobbyGUI && (SceneManager.GetSceneAt(0).name == this.m_LobbyScene))
            {
                Rect position = new Rect(90f, 180f, 500f, 150f);
                GUI.Box(position, "Players:");
                if (NetworkClient.active)
                {
                    Rect rect2 = new Rect(100f, 300f, 120f, 20f);
                    if (GUI.Button(rect2, "Add Player"))
                    {
                        this.TryToAddPlayer();
                    }
                }
            }
        }

        /// <summary>
        /// <para>Called on the client when adding a player to the lobby fails.</para>
        /// </summary>
        public virtual void OnLobbyClientAddPlayerFailed()
        {
        }

        /// <summary>
        /// <para>This is called on the client when it connects to server.</para>
        /// </summary>
        /// <param name="conn">The connection that connected.</param>
        public virtual void OnLobbyClientConnect(NetworkConnection conn)
        {
        }

        /// <summary>
        /// <para>This is called on the client when disconnected from a server.</para>
        /// </summary>
        /// <param name="conn">The connection that disconnected.</param>
        public virtual void OnLobbyClientDisconnect(NetworkConnection conn)
        {
        }

        /// <summary>
        /// <para>This is a hook to allow custom behaviour when the game client enters the lobby.</para>
        /// </summary>
        public virtual void OnLobbyClientEnter()
        {
        }

        /// <summary>
        /// <para>This is a hook to allow custom behaviour when the game client exits the lobby.</para>
        /// </summary>
        public virtual void OnLobbyClientExit()
        {
        }

        /// <summary>
        /// <para>This is called on the client when the client is finished loading a new networked scene.</para>
        /// </summary>
        /// <param name="conn"></param>
        public virtual void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
        }

        /// <summary>
        /// <para>This is called on the server when a new client connects to the server.</para>
        /// </summary>
        /// <param name="conn">The new connection.</param>
        public virtual void OnLobbyServerConnect(NetworkConnection conn)
        {
        }

        /// <summary>
        /// <para>This allows customization of the creation of the GamePlayer object on the server.</para>
        /// </summary>
        /// <param name="conn">The connection the player object is for.</param>
        /// <param name="playerControllerId">The controllerId of the player on the connnection.</param>
        /// <returns>
        /// <para>A new GamePlayer object.</para>
        /// </returns>
        public virtual GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId) => 
            null;

        /// <summary>
        /// <para>This allows customization of the creation of the lobby-player object on the server.</para>
        /// </summary>
        /// <param name="conn">The connection the player object is for.</param>
        /// <param name="playerControllerId">The controllerId of the player.</param>
        /// <returns>
        /// <para>The new lobby-player object.</para>
        /// </returns>
        public virtual GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId) => 
            null;

        /// <summary>
        /// <para>This is called on the server when a client disconnects.</para>
        /// </summary>
        /// <param name="conn">The connection that disconnected.</param>
        public virtual void OnLobbyServerDisconnect(NetworkConnection conn)
        {
        }

        /// <summary>
        /// <para>This is called on the server when a player is removed.</para>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="playerControllerId"></param>
        public virtual void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
        }

        /// <summary>
        /// <para>This is called on the server when all the players in the lobby are ready.</para>
        /// </summary>
        public virtual void OnLobbyServerPlayersReady()
        {
            this.ServerChangeScene(this.m_PlayScene);
        }

        /// <summary>
        /// <para>This is called on the server when a networked scene finishes loading.</para>
        /// </summary>
        /// <param name="sceneName">Name of the new scene.</param>
        public virtual void OnLobbyServerSceneChanged(string sceneName)
        {
        }

        /// <summary>
        /// <para>This is called on the server when it is told that a client has finished switching from the lobby scene to a game player scene.</para>
        /// </summary>
        /// <param name="lobbyPlayer">The lobby player object.</param>
        /// <param name="gamePlayer">The game player object.</param>
        /// <returns>
        /// <para>False to not allow this player to replace the lobby player.</para>
        /// </returns>
        public virtual bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer) => 
            true;

        /// <summary>
        /// <para>This is called on the client when a client is started.</para>
        /// </summary>
        /// <param name="lobbyClient"></param>
        public virtual void OnLobbyStartClient(NetworkClient lobbyClient)
        {
        }

        /// <summary>
        /// <para>This is called on the host when a host is started.</para>
        /// </summary>
        public virtual void OnLobbyStartHost()
        {
        }

        /// <summary>
        /// <para>This is called on the server when the server is started - including when a host is started.</para>
        /// </summary>
        public virtual void OnLobbyStartServer()
        {
        }

        /// <summary>
        /// <para>This is called on the client when the client stops.</para>
        /// </summary>
        public virtual void OnLobbyStopClient()
        {
        }

        /// <summary>
        /// <para>This is called on the host when the host is stopped.</para>
        /// </summary>
        public virtual void OnLobbyStopHost()
        {
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            if (SceneManager.GetSceneAt(0).name == this.m_LobbyScene)
            {
                int num = 0;
                for (int i = 0; i < conn.playerControllers.Count; i++)
                {
                    if (conn.playerControllers[i].IsValid)
                    {
                        num++;
                    }
                }
                if (num >= this.maxPlayersPerConnection)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("NetworkLobbyManager no more players for this connection.");
                    }
                    EmptyMessage msg = new EmptyMessage();
                    conn.Send(0x2d, msg);
                }
                else
                {
                    byte index = this.FindSlot();
                    if (index == 0xff)
                    {
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning("NetworkLobbyManager no space for more players");
                        }
                        EmptyMessage message2 = new EmptyMessage();
                        conn.Send(0x2d, message2);
                    }
                    else
                    {
                        GameObject obj2 = this.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
                        if (obj2 == null)
                        {
                            obj2 = UnityEngine.Object.Instantiate<GameObject>(this.lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
                        }
                        NetworkLobbyPlayer component = obj2.GetComponent<NetworkLobbyPlayer>();
                        component.slot = index;
                        this.lobbySlots[index] = component;
                        NetworkServer.AddPlayerForConnection(conn, obj2, playerControllerId);
                    }
                }
            }
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (base.numPlayers > this.maxPlayers)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("NetworkLobbyManager can't accept new connection [" + conn + "], too many players connected.");
                }
                conn.Disconnect();
            }
            else if (SceneManager.GetSceneAt(0).name != this.m_LobbyScene)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("NetworkLobbyManager can't accept new connection [" + conn + "], not in lobby and game already in progress.");
                }
                conn.Disconnect();
            }
            else
            {
                base.OnServerConnect(conn);
                for (int i = 0; i < this.lobbySlots.Length; i++)
                {
                    if (this.lobbySlots[i] != null)
                    {
                        this.lobbySlots[i].SetDirtyBit(1);
                    }
                }
                this.OnLobbyServerConnect(conn);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            for (int i = 0; i < this.lobbySlots.Length; i++)
            {
                NetworkLobbyPlayer player = this.lobbySlots[i];
                if ((player != null) && (player.connectionToClient == conn))
                {
                    this.lobbySlots[i] = null;
                    NetworkServer.Destroy(player.gameObject);
                }
            }
            this.OnLobbyServerDisconnect(conn);
        }

        private void OnServerReadyToBeginMessage(NetworkMessage netMsg)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager OnServerReadyToBeginMessage");
            }
            netMsg.ReadMessage<LobbyReadyToBeginMessage>(s_ReadyToBeginMessage);
            if (!netMsg.conn.GetPlayerController(s_ReadyToBeginMessage.slotId, out controller))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager OnServerReadyToBeginMessage invalid playerControllerId " + s_ReadyToBeginMessage.slotId);
                }
            }
            else
            {
                NetworkLobbyPlayer component = controller.gameObject.GetComponent<NetworkLobbyPlayer>();
                component.readyToBegin = s_ReadyToBeginMessage.readyState;
                LobbyReadyToBeginMessage msg = new LobbyReadyToBeginMessage {
                    slotId = component.slot,
                    readyState = s_ReadyToBeginMessage.readyState
                };
                NetworkServer.SendToReady(null, 0x2b, msg);
                this.CheckReadyToBegin();
            }
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            short playerControllerId = player.playerControllerId;
            byte slot = player.gameObject.GetComponent<NetworkLobbyPlayer>().slot;
            this.lobbySlots[slot] = null;
            base.OnServerRemovePlayer(conn, player);
            for (int i = 0; i < this.lobbySlots.Length; i++)
            {
                NetworkLobbyPlayer player2 = this.lobbySlots[i];
                if (player2 != null)
                {
                    player2.GetComponent<NetworkLobbyPlayer>().readyToBegin = false;
                    s_LobbyReadyToBeginMessage.slotId = player2.slot;
                    s_LobbyReadyToBeginMessage.readyState = false;
                    NetworkServer.SendToReady(null, 0x2b, s_LobbyReadyToBeginMessage);
                }
            }
            this.OnLobbyServerPlayerRemoved(conn, playerControllerId);
        }

        private void OnServerReturnToLobbyMessage(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager OnServerReturnToLobbyMessage");
            }
            this.ServerReturnToLobby();
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName != this.m_LobbyScene)
            {
                for (int i = 0; i < this.m_PendingPlayers.Count; i++)
                {
                    PendingPlayer player = this.m_PendingPlayers[i];
                    this.SceneLoadedForPlayer(player.conn, player.lobbyPlayer);
                }
                this.m_PendingPlayers.Clear();
            }
            this.OnLobbyServerSceneChanged(sceneName);
        }

        private void OnServerSceneLoadedMessage(NetworkMessage netMsg)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager OnSceneLoadedMessage");
            }
            netMsg.ReadMessage<IntegerMessage>(s_SceneLoadedMessage);
            if (!netMsg.conn.GetPlayerController((short) s_SceneLoadedMessage.value, out controller))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager OnServerSceneLoadedMessage invalid playerControllerId " + s_SceneLoadedMessage.value);
                }
            }
            else
            {
                this.SceneLoadedForPlayer(netMsg.conn, controller.gameObject);
            }
        }

        public override void OnStartClient(NetworkClient lobbyClient)
        {
            if (this.lobbySlots.Length == 0)
            {
                this.lobbySlots = new NetworkLobbyPlayer[this.maxPlayers];
            }
            if ((this.m_LobbyPlayerPrefab == null) || (this.m_LobbyPlayerPrefab.gameObject == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager no LobbyPlayer prefab is registered. Please add a LobbyPlayer prefab.");
                }
            }
            else
            {
                ClientScene.RegisterPrefab(this.m_LobbyPlayerPrefab.gameObject);
            }
            if (this.m_GamePlayerPrefab == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager no GamePlayer prefab is registered. Please add a GamePlayer prefab.");
                }
            }
            else
            {
                ClientScene.RegisterPrefab(this.m_GamePlayerPrefab);
            }
            lobbyClient.RegisterHandler(0x2b, new NetworkMessageDelegate(this.OnClientReadyToBegin));
            lobbyClient.RegisterHandler(0x2d, new NetworkMessageDelegate(this.OnClientAddPlayerFailedMessage));
            this.OnLobbyStartClient(lobbyClient);
        }

        public override void OnStartHost()
        {
            this.OnLobbyStartHost();
        }

        public override void OnStartServer()
        {
            if (string.IsNullOrEmpty(this.m_LobbyScene))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager LobbyScene is empty. Set the LobbyScene in the inspector for the NetworkLobbyMangaer");
                }
            }
            else if (string.IsNullOrEmpty(this.m_PlayScene))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager PlayScene is empty. Set the PlayScene in the inspector for the NetworkLobbyMangaer");
                }
            }
            else
            {
                if (this.lobbySlots.Length == 0)
                {
                    this.lobbySlots = new NetworkLobbyPlayer[this.maxPlayers];
                }
                NetworkServer.RegisterHandler(0x2b, new NetworkMessageDelegate(this.OnServerReadyToBeginMessage));
                NetworkServer.RegisterHandler(0x2c, new NetworkMessageDelegate(this.OnServerSceneLoadedMessage));
                NetworkServer.RegisterHandler(0x2e, new NetworkMessageDelegate(this.OnServerReturnToLobbyMessage));
                this.OnLobbyStartServer();
            }
        }

        public override void OnStopClient()
        {
            this.OnLobbyStopClient();
            this.CallOnClientExitLobby();
        }

        public override void OnStopHost()
        {
            this.OnLobbyStopHost();
        }

        private void OnValidate()
        {
            if (this.m_MaxPlayers <= 0)
            {
                this.m_MaxPlayers = 1;
            }
            if (this.m_MaxPlayersPerConnection <= 0)
            {
                this.m_MaxPlayersPerConnection = 1;
            }
            if (this.m_MaxPlayersPerConnection > this.maxPlayers)
            {
                this.m_MaxPlayersPerConnection = this.maxPlayers;
            }
            if (this.m_MinPlayers < 0)
            {
                this.m_MinPlayers = 0;
            }
            if (this.m_MinPlayers > this.m_MaxPlayers)
            {
                this.m_MinPlayers = this.m_MaxPlayers;
            }
            if ((this.m_LobbyPlayerPrefab != null) && (this.m_LobbyPlayerPrefab.GetComponent<NetworkIdentity>() == null))
            {
                this.m_LobbyPlayerPrefab = null;
                Debug.LogWarning("LobbyPlayer prefab must have a NetworkIdentity component.");
            }
            if ((this.m_GamePlayerPrefab != null) && (this.m_GamePlayerPrefab.GetComponent<NetworkIdentity>() == null))
            {
                this.m_GamePlayerPrefab = null;
                Debug.LogWarning("GamePlayer prefab must have a NetworkIdentity component.");
            }
        }

        private void SceneLoadedForPlayer(NetworkConnection conn, GameObject lobbyPlayerGameObject)
        {
            if (lobbyPlayerGameObject.GetComponent<NetworkLobbyPlayer>() != null)
            {
                string name = SceneManager.GetSceneAt(0).name;
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "NetworkLobby SceneLoadedForPlayer scene:", name, " ", conn }));
                }
                if (name == this.m_LobbyScene)
                {
                    PendingPlayer player2;
                    player2.conn = conn;
                    player2.lobbyPlayer = lobbyPlayerGameObject;
                    this.m_PendingPlayers.Add(player2);
                }
                else
                {
                    short playerControllerId = lobbyPlayerGameObject.GetComponent<NetworkIdentity>().playerControllerId;
                    GameObject gamePlayer = this.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
                    if (gamePlayer == null)
                    {
                        Transform startPosition = base.GetStartPosition();
                        if (startPosition != null)
                        {
                            gamePlayer = UnityEngine.Object.Instantiate<GameObject>(this.gamePlayerPrefab, startPosition.position, startPosition.rotation);
                        }
                        else
                        {
                            gamePlayer = UnityEngine.Object.Instantiate<GameObject>(this.gamePlayerPrefab, Vector3.zero, Quaternion.identity);
                        }
                    }
                    if (this.OnLobbyServerSceneLoadedForPlayer(lobbyPlayerGameObject, gamePlayer))
                    {
                        NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, playerControllerId);
                    }
                }
            }
        }

        /// <summary>
        /// <para>Sends a message to the server to make the game return to the lobby scene.</para>
        /// </summary>
        /// <returns>
        /// <para>True if message was sent.</para>
        /// </returns>
        public bool SendReturnToLobby()
        {
            if ((base.client == null) || !base.client.isConnected)
            {
                return false;
            }
            EmptyMessage msg = new EmptyMessage();
            base.client.Send(0x2e, msg);
            return true;
        }

        public override void ServerChangeScene(string sceneName)
        {
            if (sceneName == this.m_LobbyScene)
            {
                for (int i = 0; i < this.lobbySlots.Length; i++)
                {
                    NetworkLobbyPlayer player = this.lobbySlots[i];
                    if (player != null)
                    {
                        PlayerController controller;
                        NetworkIdentity component = player.GetComponent<NetworkIdentity>();
                        if (component.connectionToClient.GetPlayerController(component.playerControllerId, out controller))
                        {
                            NetworkServer.Destroy(controller.gameObject);
                        }
                        if (NetworkServer.active)
                        {
                            player.GetComponent<NetworkLobbyPlayer>().readyToBegin = false;
                            NetworkServer.ReplacePlayerForConnection(component.connectionToClient, player.gameObject, component.playerControllerId);
                        }
                    }
                }
            }
            base.ServerChangeScene(sceneName);
        }

        /// <summary>
        /// <para>Calling this causes the server to switch back to the lobby scene.</para>
        /// </summary>
        public void ServerReturnToLobby()
        {
            if (!NetworkServer.active)
            {
                Debug.Log("ServerReturnToLobby called on client");
            }
            else
            {
                this.ServerChangeScene(this.m_LobbyScene);
            }
        }

        /// <summary>
        /// <para>This is used on clients to attempt to add a player to the game.</para>
        /// </summary>
        public void TryToAddPlayer()
        {
            if (!NetworkClient.active)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkLobbyManager NetworkClient not active!");
                }
            }
            else
            {
                short playerControllerId = -1;
                List<PlayerController> playerControllers = NetworkClient.allClients[0].connection.playerControllers;
                if (playerControllers.Count < this.maxPlayers)
                {
                    playerControllerId = (short) playerControllers.Count;
                }
                else
                {
                    for (short i = 0; i < this.maxPlayers; i = (short) (i + 1))
                    {
                        if (!playerControllers[i].IsValid)
                        {
                            playerControllerId = i;
                            break;
                        }
                    }
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "NetworkLobbyManager TryToAddPlayer controllerId ", playerControllerId, " ready:", ClientScene.ready }));
                }
                if (playerControllerId == -1)
                {
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("NetworkLobbyManager No Space!");
                    }
                }
                else if (ClientScene.ready)
                {
                    ClientScene.AddPlayer(playerControllerId);
                }
                else
                {
                    ClientScene.AddPlayer(NetworkClient.allClients[0].connection, playerControllerId);
                }
            }
        }

        /// <summary>
        /// <para>This is the prefab of the player to be created in the PlayScene.</para>
        /// </summary>
        public GameObject gamePlayerPrefab
        {
            get => 
                this.m_GamePlayerPrefab;
            set
            {
                this.m_GamePlayerPrefab = value;
            }
        }

        /// <summary>
        /// <para>This is the prefab of the player to be created in the LobbyScene.</para>
        /// </summary>
        public NetworkLobbyPlayer lobbyPlayerPrefab
        {
            get => 
                this.m_LobbyPlayerPrefab;
            set
            {
                this.m_LobbyPlayerPrefab = value;
            }
        }

        /// <summary>
        /// <para>The scene to use for the lobby. This is similar to the offlineScene of the NetworkManager.</para>
        /// </summary>
        public string lobbyScene
        {
            get => 
                this.m_LobbyScene;
            set
            {
                this.m_LobbyScene = value;
                base.offlineScene = value;
            }
        }

        /// <summary>
        /// <para>The maximum number of players allowed in the game.</para>
        /// </summary>
        public int maxPlayers
        {
            get => 
                this.m_MaxPlayers;
            set
            {
                this.m_MaxPlayers = value;
            }
        }

        /// <summary>
        /// <para>The maximum number of players per connection.</para>
        /// </summary>
        public int maxPlayersPerConnection
        {
            get => 
                this.m_MaxPlayersPerConnection;
            set
            {
                this.m_MaxPlayersPerConnection = value;
            }
        }

        /// <summary>
        /// <para>The minimum number of players required to be ready for the game to start.</para>
        /// </summary>
        public int minPlayers
        {
            get => 
                this.m_MinPlayers;
            set
            {
                this.m_MinPlayers = value;
            }
        }

        /// <summary>
        /// <para>The scene to use for the playing the game from the lobby. This is similar to the onlineScene of the NetworkManager.</para>
        /// </summary>
        public string playScene
        {
            get => 
                this.m_PlayScene;
            set
            {
                this.m_PlayScene = value;
            }
        }

        /// <summary>
        /// <para>This flag enables display of the default lobby UI.</para>
        /// </summary>
        public bool showLobbyGUI
        {
            get => 
                this.m_ShowLobbyGUI;
            set
            {
                this.m_ShowLobbyGUI = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PendingPlayer
        {
            public NetworkConnection conn;
            public GameObject lobbyPlayer;
        }
    }
}

