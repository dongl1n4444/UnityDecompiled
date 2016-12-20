namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// <para>This component works in conjunction with the NetworkLobbyManager to make up the multiplayer lobby system.</para>
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Network/NetworkLobbyPlayer")]
    public class NetworkLobbyPlayer : NetworkBehaviour
    {
        private bool m_ReadyToBegin;
        private byte m_Slot;
        /// <summary>
        /// <para>This flag controls whether the default UI is shown for the lobby player.</para>
        /// </summary>
        [SerializeField]
        public bool ShowLobbyGUI = true;

        /// <summary>
        /// <para>This is a hook that is invoked on all player objects when entering the lobby.</para>
        /// </summary>
        public virtual void OnClientEnterLobby()
        {
        }

        /// <summary>
        /// <para>This is a hook that is invoked on all player objects when exiting the lobby.</para>
        /// </summary>
        public virtual void OnClientExitLobby()
        {
        }

        /// <summary>
        /// <para>This is a hook that is invoked on clients when a LobbyPlayer switches between ready or not ready.</para>
        /// </summary>
        /// <param name="readyState">Whether the player is ready or not.</param>
        public virtual void OnClientReady(bool readyState)
        {
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (reader.ReadPackedUInt32() != 0)
            {
                this.m_Slot = reader.ReadByte();
                this.m_ReadyToBegin = reader.ReadBoolean();
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
        }

        private void OnGUI()
        {
            if (this.ShowLobbyGUI)
            {
                NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
                if ((singleton == null) || (singleton.showLobbyGUI && (SceneManager.GetSceneAt(0).name == singleton.lobbyScene)))
                {
                    Rect position = new Rect((float) (100 + (this.m_Slot * 100)), 200f, 90f, 20f);
                    if (base.isLocalPlayer)
                    {
                        string str2;
                        if (this.m_ReadyToBegin)
                        {
                            str2 = "(Ready)";
                        }
                        else
                        {
                            str2 = "(Not Ready)";
                        }
                        GUI.Label(position, str2);
                        if (this.m_ReadyToBegin)
                        {
                            position.y += 25f;
                            if (GUI.Button(position, "STOP"))
                            {
                                this.SendNotReadyToBeginMessage();
                            }
                        }
                        else
                        {
                            position.y += 25f;
                            if (GUI.Button(position, "START"))
                            {
                                this.SendReadyToBeginMessage();
                            }
                            position.y += 25f;
                            if (GUI.Button(position, "Remove"))
                            {
                                ClientScene.RemovePlayer(base.GetComponent<NetworkIdentity>().playerControllerId);
                            }
                        }
                    }
                    else
                    {
                        GUI.Label(position, "Player [" + base.netId + "]");
                        position.y += 25f;
                        GUI.Label(position, "Ready [" + this.m_ReadyToBegin + "]");
                    }
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (((singleton == null) || (scene.name != singleton.lobbyScene)) && base.isLocalPlayer)
            {
                this.SendSceneLoadedMessage();
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            writer.WritePackedUInt32(1);
            writer.Write(this.m_Slot);
            writer.Write(this.m_ReadyToBegin);
            return true;
        }

        public override void OnStartClient()
        {
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                singleton.lobbySlots[this.m_Slot] = this;
                this.m_ReadyToBegin = false;
                this.OnClientEnterLobby();
            }
            else
            {
                Debug.LogError("LobbyPlayer could not find a NetworkLobbyManager. The LobbyPlayer requires a NetworkLobbyManager object to function. Make sure that there is one in the scene.");
            }
        }

        /// <summary>
        /// <para>This removes this player from the lobby.</para>
        /// </summary>
        public void RemovePlayer()
        {
            if (base.isLocalPlayer && !this.m_ReadyToBegin)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkLobbyPlayer RemovePlayer");
                }
                ClientScene.RemovePlayer(base.GetComponent<NetworkIdentity>().playerControllerId);
            }
        }

        /// <summary>
        /// <para>This is used on clients to tell the server that this player is not ready for the game to begin.</para>
        /// </summary>
        public void SendNotReadyToBeginMessage()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyPlayer SendReadyToBeginMessage");
            }
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                LobbyReadyToBeginMessage msg = new LobbyReadyToBeginMessage {
                    slotId = (byte) base.playerControllerId,
                    readyState = false
                };
                singleton.client.Send(0x2b, msg);
            }
        }

        /// <summary>
        /// <para>This is used on clients to tell the server that this player is ready for the game to begin.</para>
        /// </summary>
        public void SendReadyToBeginMessage()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyPlayer SendReadyToBeginMessage");
            }
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                LobbyReadyToBeginMessage msg = new LobbyReadyToBeginMessage {
                    slotId = (byte) base.playerControllerId,
                    readyState = true
                };
                singleton.client.Send(0x2b, msg);
            }
        }

        /// <summary>
        /// <para>This is used on clients to tell the server that the client has switched from the lobby to the GameScene and is ready to play.</para>
        /// </summary>
        public void SendSceneLoadedMessage()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyPlayer SendSceneLoadedMessage");
            }
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                IntegerMessage msg = new IntegerMessage(base.playerControllerId);
                singleton.client.Send(0x2c, msg);
            }
        }

        private void Start()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }

        /// <summary>
        /// <para>This is a flag that control whether this player is ready for the game to begin.</para>
        /// </summary>
        public bool readyToBegin
        {
            get
            {
                return this.m_ReadyToBegin;
            }
            set
            {
                this.m_ReadyToBegin = value;
            }
        }

        /// <summary>
        /// <para>The slot within the lobby that this player inhabits.</para>
        /// </summary>
        public byte slot
        {
            get
            {
                return this.m_Slot;
            }
            set
            {
                this.m_Slot = value;
            }
        }
    }
}

