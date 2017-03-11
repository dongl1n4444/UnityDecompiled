﻿namespace UnityEngine.Networking
{
    using System;
    using System.ComponentModel;
    using UnityEngine;
    using UnityEngine.Networking.Match;

    /// <summary>
    /// <para>An extension for the NetworkManager that displays a default HUD for controlling the network state of the game.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkManagerHUD"), RequireComponent(typeof(NetworkManager)), EditorBrowsable(EditorBrowsableState.Never)]
    public class NetworkManagerHUD : MonoBehaviour
    {
        private bool m_ShowServer;
        /// <summary>
        /// <para>The NetworkManager associated with this HUD.</para>
        /// </summary>
        public NetworkManager manager;
        /// <summary>
        /// <para>The horizontal offset in pixels to draw the HUD runtime GUI at.</para>
        /// </summary>
        [SerializeField]
        public int offsetX;
        /// <summary>
        /// <para>The vertical offset in pixels to draw the HUD runtime GUI at.</para>
        /// </summary>
        [SerializeField]
        public int offsetY;
        /// <summary>
        /// <para>Whether to show the default control HUD at runtime.</para>
        /// </summary>
        [SerializeField]
        public bool showGUI = true;

        private void Awake()
        {
            this.manager = base.GetComponent<NetworkManager>();
        }

        private void OnGUI()
        {
            if (this.showGUI)
            {
                int num = 10 + this.offsetX;
                int num2 = 40 + this.offsetY;
                bool flag = ((this.manager.client == null) || (this.manager.client.connection == null)) || (this.manager.client.connection.connectionId == -1);
                if ((!this.manager.IsClientConnected() && !NetworkServer.active) && (this.manager.matchMaker == null))
                {
                    if (flag)
                    {
                        if (Application.platform != RuntimePlatform.WebGLPlayer)
                        {
                            if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "LAN Host(H)"))
                            {
                                this.manager.StartHost();
                            }
                            num2 += 0x18;
                        }
                        if (GUI.Button(new Rect((float) num, (float) num2, 105f, 20f), "LAN Client(C)"))
                        {
                            this.manager.StartClient();
                        }
                        this.manager.networkAddress = GUI.TextField(new Rect((float) (num + 100), (float) num2, 95f, 20f), this.manager.networkAddress);
                        num2 += 0x18;
                        if (Application.platform == RuntimePlatform.WebGLPlayer)
                        {
                            GUI.Box(new Rect((float) num, (float) num2, 200f, 25f), "(  WebGL cannot be server  )");
                            num2 += 0x18;
                        }
                        else
                        {
                            if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "LAN Server Only(S)"))
                            {
                                this.manager.StartServer();
                            }
                            num2 += 0x18;
                        }
                    }
                    else
                    {
                        GUI.Label(new Rect((float) num, (float) num2, 200f, 20f), string.Concat(new object[] { "Connecting to ", this.manager.networkAddress, ":", this.manager.networkPort, ".." }));
                        num2 += 0x18;
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Cancel Connection Attempt"))
                        {
                            this.manager.StopClient();
                        }
                    }
                }
                else
                {
                    if (NetworkServer.active)
                    {
                        string text = "Server: port=" + this.manager.networkPort;
                        if (this.manager.useWebSockets)
                        {
                            text = text + " (Using WebSockets)";
                        }
                        GUI.Label(new Rect((float) num, (float) num2, 300f, 20f), text);
                        num2 += 0x18;
                    }
                    if (this.manager.IsClientConnected())
                    {
                        GUI.Label(new Rect((float) num, (float) num2, 300f, 20f), string.Concat(new object[] { "Client: address=", this.manager.networkAddress, " port=", this.manager.networkPort }));
                        num2 += 0x18;
                    }
                }
                if (this.manager.IsClientConnected() && !ClientScene.ready)
                {
                    if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Client Ready"))
                    {
                        ClientScene.Ready(this.manager.client.connection);
                        if (ClientScene.localPlayers.Count == 0)
                        {
                            ClientScene.AddPlayer(0);
                        }
                    }
                    num2 += 0x18;
                }
                if (NetworkServer.active || this.manager.IsClientConnected())
                {
                    if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Stop (X)"))
                    {
                        this.manager.StopHost();
                    }
                    num2 += 0x18;
                }
                if ((!NetworkServer.active && !this.manager.IsClientConnected()) && flag)
                {
                    num2 += 10;
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        GUI.Box(new Rect((float) (num - 5), (float) num2, 220f, 25f), "(WebGL cannot use Match Maker)");
                    }
                    else if (this.manager.matchMaker == null)
                    {
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Enable Match Maker (M)"))
                        {
                            this.manager.StartMatchMaker();
                        }
                        num2 += 0x18;
                    }
                    else
                    {
                        if (this.manager.matchInfo == null)
                        {
                            if (this.manager.matches == null)
                            {
                                if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Create Internet Match"))
                                {
                                    this.manager.matchMaker.CreateMatch(this.manager.matchName, this.manager.matchSize, true, "", "", "", 0, 0, new NetworkMatch.DataResponseDelegate<MatchInfo>(this.manager.OnMatchCreate));
                                }
                                num2 += 0x18;
                                GUI.Label(new Rect((float) num, (float) num2, 100f, 20f), "Room Name:");
                                this.manager.matchName = GUI.TextField(new Rect((float) (num + 100), (float) num2, 100f, 20f), this.manager.matchName);
                                num2 += 0x18;
                                num2 += 10;
                                if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Find Internet Match"))
                                {
                                    this.manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, new NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>(this.manager.OnMatchList));
                                }
                                num2 += 0x18;
                            }
                            else
                            {
                                for (int i = 0; i < this.manager.matches.Count; i++)
                                {
                                    MatchInfoSnapshot snapshot = this.manager.matches[i];
                                    if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Join Match:" + snapshot.name))
                                    {
                                        this.manager.matchName = snapshot.name;
                                        this.manager.matchMaker.JoinMatch(snapshot.networkId, "", "", "", 0, 0, new NetworkMatch.DataResponseDelegate<MatchInfo>(this.manager.OnMatchJoined));
                                    }
                                    num2 += 0x18;
                                }
                                if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Back to Match Menu"))
                                {
                                    this.manager.matches = null;
                                }
                                num2 += 0x18;
                            }
                        }
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Change MM server"))
                        {
                            this.m_ShowServer = !this.m_ShowServer;
                        }
                        if (this.m_ShowServer)
                        {
                            num2 += 0x18;
                            if (GUI.Button(new Rect((float) num, (float) num2, 100f, 20f), "Local"))
                            {
                                this.manager.SetMatchHost("localhost", 0x539, false);
                                this.m_ShowServer = false;
                            }
                            num2 += 0x18;
                            if (GUI.Button(new Rect((float) num, (float) num2, 100f, 20f), "Internet"))
                            {
                                this.manager.SetMatchHost("mm.unet.unity3d.com", 0x1bb, true);
                                this.m_ShowServer = false;
                            }
                            num2 += 0x18;
                            if (GUI.Button(new Rect((float) num, (float) num2, 100f, 20f), "Staging"))
                            {
                                this.manager.SetMatchHost("staging-mm.unet.unity3d.com", 0x1bb, true);
                                this.m_ShowServer = false;
                            }
                        }
                        num2 += 0x18;
                        GUI.Label(new Rect((float) num, (float) num2, 300f, 20f), "MM Uri: " + this.manager.matchMaker.baseUri);
                        num2 += 0x18;
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Disable Match Maker"))
                        {
                            this.manager.StopMatchMaker();
                        }
                        num2 += 0x18;
                    }
                }
            }
        }

        private void Update()
        {
            if (this.showGUI)
            {
                if ((!this.manager.IsClientConnected() && !NetworkServer.active) && (this.manager.matchMaker == null))
                {
                    if (Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        if (Input.GetKeyDown(KeyCode.S))
                        {
                            this.manager.StartServer();
                        }
                        if (Input.GetKeyDown(KeyCode.H))
                        {
                            this.manager.StartHost();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        this.manager.StartClient();
                    }
                }
                if (NetworkServer.active)
                {
                    if (this.manager.IsClientConnected())
                    {
                        if (Input.GetKeyDown(KeyCode.X))
                        {
                            this.manager.StopHost();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.X))
                    {
                        this.manager.StopServer();
                    }
                }
            }
        }
    }
}

