namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;

    [CustomEditor(typeof(NetworkManagerHUD), true), CanEditMultipleObjects]
    public class NetworkManagerHUDEditor : Editor
    {
        private bool m_Initialized;
        private NetworkManager m_Manager;
        private NetworkManagerHUD m_ManagerHud;
        protected GUIContent m_OffsetXLabel;
        private SerializedProperty m_OffsetXProperty;
        protected GUIContent m_OffsetYLabel;
        private SerializedProperty m_OffsetYProperty;
        private bool m_ShowClient;
        protected GUIContent m_ShowClientLabel;
        private bool m_ShowClientObjects;
        protected GUIContent m_ShowClientObjectsLabel;
        private bool m_ShowControls;
        protected GUIContent m_ShowControlsLabel;
        private List<bool> m_ShowDetailForConnections;
        private SerializedProperty m_ShowGUIProperty;
        private bool m_ShowMatchMaker;
        protected GUIContent m_ShowMatchMakerLabel;
        protected GUIContent m_ShowNetworkLabel;
        private List<bool> m_ShowOwnedForConnections;
        private List<bool> m_ShowPlayersForConnections;
        protected GUIContent m_ShowRuntimeGuiLabel;
        private bool m_ShowServer;
        private bool m_ShowServerConnections;
        protected GUIContent m_ShowServerConnectionsLabel;
        protected GUIContent m_ShowServerLabel;
        private bool m_ShowServerObjects;
        protected GUIContent m_ShowServerObjectsLabel;
        private List<bool> m_ShowVisibleForConnections;

        private static Rect GetButtonRect()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            float num = controlRect.width / 6f;
            return new Rect(controlRect.xMin + num, controlRect.yMin, controlRect.width - (num * 2f), controlRect.height);
        }

        private static UnityEngine.Object GetSceneObject(string sceneObjectName)
        {
            if (!string.IsNullOrEmpty(sceneObjectName))
            {
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (scene.path.IndexOf(sceneObjectName) != -1)
                    {
                        return AssetDatabase.LoadAssetAtPath(scene.path, typeof(UnityEngine.Object));
                    }
                }
            }
            return null;
        }

        private void Init()
        {
            if (!this.m_Initialized || (this.m_ShowGUIProperty == null))
            {
                this.m_Initialized = true;
                this.m_ManagerHud = base.target as NetworkManagerHUD;
                if (this.m_ManagerHud != null)
                {
                    this.m_Manager = this.m_ManagerHud.manager;
                }
                this.m_ShowGUIProperty = base.serializedObject.FindProperty("showGUI");
                this.m_OffsetXProperty = base.serializedObject.FindProperty("offsetX");
                this.m_OffsetYProperty = base.serializedObject.FindProperty("offsetY");
                this.m_ShowServerLabel = new GUIContent("Server Info", "Details of internal server state");
                this.m_ShowServerConnectionsLabel = new GUIContent("Server Connections", "List of local and remote network connections to the server");
                this.m_ShowServerObjectsLabel = new GUIContent("Server Objects", "Networked objects spawned by the server");
                this.m_ShowClientLabel = new GUIContent("Client Info", "Details of internal client state");
                this.m_ShowClientObjectsLabel = new GUIContent("Client Objects", "Networked objects created on the client");
                this.m_ShowMatchMakerLabel = new GUIContent("MatchMaker Info", "Details about the matchmaker state");
                this.m_ShowControlsLabel = new GUIContent("Runtime Controls", "Buttons for controlling network state at runtime");
                this.m_ShowRuntimeGuiLabel = new GUIContent("Show Runtime GUI", "Show the default network control GUI when the game is running");
                this.m_OffsetXLabel = new GUIContent("GUI Horizontal Offset", "Horizontal offset of runtime GUI");
                this.m_OffsetYLabel = new GUIContent("GUI Vertical Offset", "Vertical offset of runtime GUI");
            }
        }

        public override void OnInspectorGUI()
        {
            this.Init();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_ShowGUIProperty, this.m_ShowRuntimeGuiLabel, new GUILayoutOption[0]);
            if (this.m_ManagerHud.showGUI)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_OffsetXProperty, this.m_OffsetXLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_OffsetYProperty, this.m_OffsetYLabel, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
            base.serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying)
            {
                this.ShowControls();
                this.ShowServerInfo();
                this.ShowClientInfo();
                this.ShowMatchMakerInfo();
            }
        }

        private void ShowClientInfo()
        {
            if (NetworkClient.active)
            {
                this.m_ShowClient = EditorGUILayout.Foldout(this.m_ShowClient, this.m_ShowClientLabel);
                if (this.m_ShowClient)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                    int num = 0;
                    foreach (NetworkClient client in NetworkClient.allClients)
                    {
                        if (client.connection == null)
                        {
                            EditorGUILayout.TextField("client " + num + ": ", client.GetType().Name + " Conn: null", new GUILayoutOption[0]);
                        }
                        else
                        {
                            EditorGUILayout.TextField("client " + num + ":", client.GetType().Name + " Conn: " + client.connection, new GUILayoutOption[0]);
                            EditorGUI.indentLevel++;
                            foreach (PlayerController controller in client.connection.playerControllers)
                            {
                                EditorGUILayout.LabelField("Player", controller.ToString(), new GUILayoutOption[0]);
                            }
                            EditorGUI.indentLevel--;
                        }
                        num++;
                    }
                    this.ShowClientObjects();
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void ShowClientObjects()
        {
            this.m_ShowClientObjects = EditorGUILayout.Foldout(this.m_ShowClientObjects, this.m_ShowClientObjectsLabel);
            if (this.m_ShowClientObjects)
            {
                EditorGUI.indentLevel++;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in ClientScene.objects)
                {
                    string label = "NetId:" + pair.Key;
                    GameObject gameObject = null;
                    if (pair.Value != null)
                    {
                        NetworkIdentity component = pair.Value.GetComponent<NetworkIdentity>();
                        label = label + " SceneId:" + component.sceneId;
                        gameObject = pair.Value.gameObject;
                    }
                    EditorGUILayout.ObjectField(label, gameObject, typeof(GameObject), true, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void ShowControls()
        {
            this.m_ShowControls = EditorGUILayout.Foldout(this.m_ShowControls, this.m_ShowControlsLabel);
            if (this.m_ShowControls)
            {
                if (!string.IsNullOrEmpty(NetworkManager.networkSceneName))
                {
                    EditorGUILayout.ObjectField("Current Scene:", GetSceneObject(NetworkManager.networkSceneName), typeof(UnityEngine.Object), true, new GUILayoutOption[0]);
                }
                EditorGUILayout.Separator();
                if ((!NetworkClient.active && !NetworkServer.active) && (this.m_Manager.matchMaker == null))
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    if (GUILayout.Toggle(false, "LAN Host", EditorStyles.miniButton, new GUILayoutOption[0]))
                    {
                        this.m_Manager.StartHost();
                    }
                    if (GUILayout.Toggle(false, "LAN Server", EditorStyles.miniButton, new GUILayoutOption[0]))
                    {
                        this.m_Manager.StartServer();
                    }
                    if (GUILayout.Toggle(false, "LAN Client", EditorStyles.miniButton, new GUILayoutOption[0]))
                    {
                        this.m_Manager.StartClient();
                    }
                    if (GUILayout.Toggle(false, "Start Matchmaker", EditorStyles.miniButton, new GUILayoutOption[0]))
                    {
                        this.m_Manager.StartMatchMaker();
                        this.m_ShowMatchMaker = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if ((NetworkClient.active && !ClientScene.ready) && GUI.Button(GetButtonRect(), "Client Ready"))
                {
                    ClientScene.Ready(this.m_Manager.client.connection);
                    if (ClientScene.localPlayers.Count == 0)
                    {
                        ClientScene.AddPlayer(0);
                    }
                }
                if ((NetworkServer.active || NetworkClient.active) && GUI.Button(GetButtonRect(), "Stop"))
                {
                    this.m_Manager.StopServer();
                    this.m_Manager.StopClient();
                }
                if (!NetworkServer.active && !NetworkClient.active)
                {
                    EditorGUILayout.Separator();
                    if ((this.m_Manager.matchMaker != null) && (this.m_Manager.matchInfo == null))
                    {
                        if (this.m_Manager.matches == null)
                        {
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            if (GUILayout.Toggle(false, "Create Internet Match", EditorStyles.miniButton, new GUILayoutOption[0]))
                            {
                                this.m_Manager.matchMaker.CreateMatch(this.m_Manager.matchName, this.m_Manager.matchSize, true, "", "", "", 0, 0, new NetworkMatch.DataResponseDelegate<MatchInfo>(this.m_Manager.OnMatchCreate));
                            }
                            if (GUILayout.Toggle(false, "Find Internet Match", EditorStyles.miniButton, new GUILayoutOption[0]))
                            {
                                this.m_Manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, new NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>(this.m_Manager.OnMatchList));
                            }
                            if (GUILayout.Toggle(false, "Stop MatchMaker", EditorStyles.miniButton, new GUILayoutOption[0]))
                            {
                                this.m_Manager.StopMatchMaker();
                            }
                            EditorGUILayout.EndHorizontal();
                            this.m_Manager.matchName = EditorGUILayout.TextField("Room Name:", this.m_Manager.matchName, new GUILayoutOption[0]);
                            this.m_Manager.matchSize = (uint) EditorGUILayout.IntField("Room Size:", (int) this.m_Manager.matchSize, new GUILayoutOption[0]);
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            if (GUILayout.Toggle(false, "Use Local Relay", EditorStyles.miniButton, new GUILayoutOption[0]))
                            {
                                this.m_Manager.SetMatchHost("localhost", 0x539, false);
                            }
                            if (GUILayout.Toggle(false, "Use Internet Relay", EditorStyles.miniButton, new GUILayoutOption[0]))
                            {
                                this.m_Manager.SetMatchHost("mm.unet.unity3d.com", 0x1bb, true);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Separator();
                        }
                        else
                        {
                            foreach (MatchInfoSnapshot snapshot in this.m_Manager.matches)
                            {
                                if (GUI.Button(GetButtonRect(), "Join Match:" + snapshot.name))
                                {
                                    this.m_Manager.matchName = snapshot.name;
                                    this.m_Manager.matchSize = (uint) snapshot.currentSize;
                                    this.m_Manager.matchMaker.JoinMatch(snapshot.networkId, "", "", "", 0, 0, new NetworkMatch.DataResponseDelegate<MatchInfo>(this.m_Manager.OnMatchJoined));
                                }
                            }
                            if (GUI.Button(GetButtonRect(), "Stop MatchMaker"))
                            {
                                this.m_Manager.StopMatchMaker();
                            }
                        }
                    }
                }
                EditorGUILayout.Separator();
            }
        }

        private void ShowMatchMakerInfo()
        {
            if (this.m_Manager.matchMaker != null)
            {
                this.m_ShowMatchMaker = EditorGUILayout.Foldout(this.m_ShowMatchMaker, this.m_ShowMatchMakerLabel);
                if (this.m_ShowMatchMaker)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Match Information", (this.m_Manager.matchInfo != null) ? this.m_Manager.matchInfo.ToString() : "None", new GUILayoutOption[0]);
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void ShowServerConnections()
        {
            this.m_ShowServerConnections = EditorGUILayout.Foldout(this.m_ShowServerConnections, this.m_ShowServerConnectionsLabel);
            if (this.m_ShowServerConnections)
            {
                EditorGUI.indentLevel++;
                if (this.m_ShowDetailForConnections == null)
                {
                    this.m_ShowDetailForConnections = new List<bool>();
                    this.m_ShowPlayersForConnections = new List<bool>();
                    this.m_ShowVisibleForConnections = new List<bool>();
                    this.m_ShowOwnedForConnections = new List<bool>();
                }
                while (this.m_ShowDetailForConnections.Count < NetworkServer.connections.Count)
                {
                    this.m_ShowDetailForConnections.Add(false);
                    this.m_ShowPlayersForConnections.Add(false);
                    this.m_ShowVisibleForConnections.Add(false);
                    this.m_ShowOwnedForConnections.Add(false);
                }
                int num = 0;
                foreach (NetworkConnection connection in NetworkServer.connections)
                {
                    if (connection == null)
                    {
                        num++;
                    }
                    else
                    {
                        this.m_ShowDetailForConnections[num] = EditorGUILayout.Foldout(this.m_ShowDetailForConnections[num], string.Concat(new object[] { "Conn: ", connection.connectionId, " (", connection.address, ")" }));
                        if (this.m_ShowDetailForConnections[num])
                        {
                            EditorGUI.indentLevel++;
                            this.m_ShowPlayersForConnections[num] = EditorGUILayout.Foldout(this.m_ShowPlayersForConnections[num], "Players");
                            if (this.m_ShowPlayersForConnections[num])
                            {
                                EditorGUI.indentLevel++;
                                foreach (PlayerController controller in connection.playerControllers)
                                {
                                    EditorGUILayout.ObjectField("Player: " + controller.playerControllerId, controller.gameObject, typeof(GameObject), true, new GUILayoutOption[0]);
                                }
                                EditorGUI.indentLevel--;
                            }
                            this.m_ShowVisibleForConnections[num] = EditorGUILayout.Foldout(this.m_ShowVisibleForConnections[num], "Visible Objects");
                            if (this.m_ShowVisibleForConnections[num])
                            {
                                EditorGUI.indentLevel++;
                                foreach (NetworkIdentity identity in connection.visList)
                                {
                                    EditorGUILayout.ObjectField("NetId: " + identity.netId, identity, typeof(NetworkIdentity), true, new GUILayoutOption[0]);
                                }
                                EditorGUI.indentLevel--;
                            }
                            if (connection.clientOwnedObjects != null)
                            {
                                this.m_ShowOwnedForConnections[num] = EditorGUILayout.Foldout(this.m_ShowOwnedForConnections[num], "Owned Objects");
                                if (this.m_ShowOwnedForConnections[num])
                                {
                                    EditorGUI.indentLevel++;
                                    foreach (NetworkInstanceId id in connection.clientOwnedObjects)
                                    {
                                        GameObject obj2 = NetworkServer.FindLocalObject(id);
                                        EditorGUILayout.ObjectField("Owned: " + id, obj2, typeof(NetworkIdentity), true, new GUILayoutOption[0]);
                                    }
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                        num++;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void ShowServerInfo()
        {
            if (NetworkServer.active)
            {
                this.m_ShowServer = EditorGUILayout.Foldout(this.m_ShowServer, this.m_ShowServerLabel);
                if (this.m_ShowServer)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                    this.ShowServerConnections();
                    this.ShowServerObjects();
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void ShowServerObjects()
        {
            this.m_ShowServerObjects = EditorGUILayout.Foldout(this.m_ShowServerObjects, this.m_ShowServerObjectsLabel);
            if (this.m_ShowServerObjects)
            {
                EditorGUI.indentLevel++;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
                {
                    string label = "NetId:" + pair.Key;
                    GameObject gameObject = null;
                    if (pair.Value != null)
                    {
                        NetworkIdentity component = pair.Value.GetComponent<NetworkIdentity>();
                        label = label + " SceneId:" + component.sceneId;
                        gameObject = pair.Value.gameObject;
                    }
                    EditorGUILayout.ObjectField(label, gameObject, typeof(GameObject), true, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}

