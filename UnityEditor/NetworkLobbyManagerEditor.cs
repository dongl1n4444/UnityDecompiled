namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkLobbyManager), true), CanEditMultipleObjects]
    internal class NetworkLobbyManagerEditor : NetworkManagerEditor
    {
        private SerializedProperty m_GamePlayerPrefabProperty;
        private SerializedProperty m_LobbyPlayerPrefabProperty;
        private GUIContent m_LobbySceneLabel;
        private GUIContent m_MaxPlayersLabel;
        private GUIContent m_MaxPlayersPerConnectionLabel;
        private SerializedProperty m_MaxPlayersPerConnectionProperty;
        private SerializedProperty m_MaxPlayersProperty;
        private GUIContent m_MinPlayersLabel;
        private SerializedProperty m_MinPlayersProperty;
        private GUIContent m_PlaySceneLabel;
        private SerializedProperty m_ShowLobbyGUIProperty;
        private bool ShowSlots;

        private void InitLobby()
        {
            if (!base.m_Initialized)
            {
                this.m_LobbySceneLabel = new GUIContent("Lobby Scene", "The scene loaded for the lobby");
                this.m_PlaySceneLabel = new GUIContent("Play Scene", "The scene loaded to play the game");
                this.m_MaxPlayersLabel = new GUIContent("Max Players", "The maximum number of players allowed in the lobby.");
                this.m_MaxPlayersPerConnectionLabel = new GUIContent("Max Players Per Connection", "The maximum number of players that each connection/client can have in the lobby. Defaults to 1.");
                this.m_MinPlayersLabel = new GUIContent("Minimum Players", "The minimum number of players required to be ready for the game to start. If this is zero then the game can start with any number of players.");
                this.m_ShowLobbyGUIProperty = base.serializedObject.FindProperty("m_ShowLobbyGUI");
                this.m_MaxPlayersProperty = base.serializedObject.FindProperty("m_MaxPlayers");
                this.m_MaxPlayersPerConnectionProperty = base.serializedObject.FindProperty("m_MaxPlayersPerConnection");
                this.m_MinPlayersProperty = base.serializedObject.FindProperty("m_MinPlayers");
                this.m_LobbyPlayerPrefabProperty = base.serializedObject.FindProperty("m_LobbyPlayerPrefab");
                this.m_GamePlayerPrefabProperty = base.serializedObject.FindProperty("m_GamePlayerPrefab");
                NetworkLobbyManager target = base.target as NetworkLobbyManager;
                if (target == null)
                {
                    return;
                }
                if ((target.lobbyScene != "") && (base.GetSceneObject(target.lobbyScene) == null))
                {
                    Debug.LogWarning("LobbyScene '" + target.lobbyScene + "' not found. You must repopulate the LobbyScene slot of the NetworkLobbyManager");
                    target.lobbyScene = "";
                }
                if ((target.playScene != "") && (base.GetSceneObject(target.playScene) == null))
                {
                    Debug.LogWarning("PlayScene '" + target.playScene + "' not found. You must repopulate the PlayScene slot of the NetworkLobbyManager");
                    target.playScene = "";
                }
            }
            base.Init();
        }

        public override void OnInspectorGUI()
        {
            if ((base.m_DontDestroyOnLoadProperty == null) || (base.m_DontDestroyOnLoadLabel == null))
            {
                base.m_Initialized = false;
            }
            this.InitLobby();
            NetworkLobbyManager target = base.target as NetworkLobbyManager;
            if (target != null)
            {
                base.serializedObject.Update();
                EditorGUILayout.PropertyField(base.m_DontDestroyOnLoadProperty, base.m_DontDestroyOnLoadLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(base.m_RunInBackgroundProperty, base.m_RunInBackgroundLabel, new GUILayoutOption[0]);
                if (EditorGUILayout.PropertyField(base.m_LogLevelProperty, new GUILayoutOption[0]))
                {
                    LogFilter.currentLogLevel = (int) base.m_NetworkManager.logLevel;
                }
                this.ShowLobbyScenes();
                EditorGUILayout.PropertyField(this.m_ShowLobbyGUIProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MaxPlayersProperty, this.m_MaxPlayersLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MaxPlayersPerConnectionProperty, this.m_MaxPlayersPerConnectionLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MinPlayersProperty, this.m_MinPlayersLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LobbyPlayerPrefabProperty, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                UnityEngine.Object obj2 = EditorGUILayout.ObjectField("Game Player Prefab", target.gamePlayerPrefab, typeof(NetworkIdentity), false, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (obj2 == null)
                    {
                        this.m_GamePlayerPrefabProperty.objectReferenceValue = null;
                    }
                    else
                    {
                        NetworkIdentity identity = obj2 as NetworkIdentity;
                        if ((identity != null) && (identity.gameObject != target.gamePlayerPrefab))
                        {
                            this.m_GamePlayerPrefabProperty.objectReferenceValue = identity.gameObject;
                        }
                    }
                }
                EditorGUILayout.Separator();
                base.ShowNetworkInfo();
                base.ShowSpawnInfo();
                base.ShowConfigInfo();
                base.ShowSimulatorInfo();
                base.serializedObject.ApplyModifiedProperties();
                base.ShowDerivedProperties(typeof(NetworkLobbyManager), typeof(NetworkManager));
                if (Application.isPlaying)
                {
                    EditorGUILayout.Separator();
                    this.ShowLobbySlots();
                }
            }
        }

        private void SetLobbyScene(NetworkLobbyManager lobby, string sceneName)
        {
            base.serializedObject.FindProperty("m_LobbyScene").stringValue = sceneName;
            base.serializedObject.FindProperty("m_OfflineScene").stringValue = sceneName;
            EditorUtility.SetDirty(lobby);
        }

        private void SetPlayScene(NetworkLobbyManager lobby, string sceneName)
        {
            base.serializedObject.FindProperty("m_PlayScene").stringValue = sceneName;
            base.serializedObject.FindProperty("m_OnlineScene").stringValue = "";
            EditorUtility.SetDirty(lobby);
        }

        protected void ShowLobbyScenes()
        {
            NetworkLobbyManager target = base.target as NetworkLobbyManager;
            if (target != null)
            {
                SceneAsset sceneObject = base.GetSceneObject(target.lobbyScene);
                EditorGUI.BeginChangeCheck();
                UnityEngine.Object obj2 = EditorGUILayout.ObjectField(this.m_LobbySceneLabel, sceneObject, typeof(SceneAsset), false, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (obj2 == null)
                    {
                        this.SetLobbyScene(target, "");
                    }
                    else if (obj2.name != target.offlineScene)
                    {
                        if (base.GetSceneObject(obj2.name) == null)
                        {
                            Debug.LogWarning("The scene " + obj2.name + " cannot be used. To use this scene add it to the build settings for the project");
                        }
                        else
                        {
                            this.SetLobbyScene(target, obj2.name);
                        }
                    }
                }
                SceneAsset asset3 = base.GetSceneObject(target.playScene);
                EditorGUI.BeginChangeCheck();
                UnityEngine.Object obj3 = EditorGUILayout.ObjectField(this.m_PlaySceneLabel, asset3, typeof(SceneAsset), false, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (obj3 == null)
                    {
                        this.SetPlayScene(target, "");
                    }
                    else if (obj3.name != base.m_NetworkManager.onlineScene)
                    {
                        if (base.GetSceneObject(obj3.name) == null)
                        {
                            Debug.LogWarning("The scene " + obj3.name + " cannot be used. To use this scene add it to the build settings for the project");
                        }
                        else
                        {
                            this.SetPlayScene(target, obj3.name);
                        }
                    }
                }
            }
        }

        protected void ShowLobbySlots()
        {
            NetworkLobbyManager target = base.target as NetworkLobbyManager;
            if (target != null)
            {
                this.ShowSlots = EditorGUILayout.Foldout(this.ShowSlots, "LobbySlots");
                if (this.ShowSlots)
                {
                    EditorGUI.indentLevel++;
                    foreach (NetworkLobbyPlayer player in target.lobbySlots)
                    {
                        if (player != null)
                        {
                            EditorGUILayout.ObjectField("Slot " + player.slot, player.gameObject, typeof(UnityEngine.Object), true, new GUILayoutOption[0]);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}

