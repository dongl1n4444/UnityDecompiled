namespace UnityEditor
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkManager), true), CanEditMultipleObjects]
    public class NetworkManagerEditor : Editor
    {
        [CompilerGenerated]
        private static ReorderableList.HeaderCallbackDelegate <>f__mg$cache0;
        [CompilerGenerated]
        private static ReorderableList.HeaderCallbackDelegate <>f__mg$cache1;
        private GUIContent m_AllowFragmentationLabel;
        private SerializedProperty m_AllowFragmentationProperty;
        private SerializedProperty m_AutoCreatePlayerProperty;
        private ReorderableList m_ChannelList;
        private SerializedProperty m_ChannelListProperty;
        private GUIContent m_ConnectTimeoutLabel;
        private SerializedProperty m_CustomConfigProperty;
        private GUIContent m_DisconnectTimeoutLabel;
        protected GUIContent m_DontDestroyOnLoadLabel;
        protected SerializedProperty m_DontDestroyOnLoadProperty;
        protected bool m_Initialized;
        private GUIContent m_LatencyLabel;
        protected SerializedProperty m_LogLevelProperty;
        private GUIContent m_MatchHostLabel;
        private SerializedProperty m_MatchHostProperty;
        private GUIContent m_MatchNameLabel;
        private SerializedProperty m_MatchNameProperty;
        private GUIContent m_MatchPortLabel;
        private SerializedProperty m_MatchPortProperty;
        private GUIContent m_MatchSizeLabel;
        private SerializedProperty m_MatchSizeProperty;
        private GUIContent m_MaxBufferedPacketsLabel;
        private SerializedProperty m_MaxBufferedPacketsProperty;
        private GUIContent m_MaxConnectionsLabel;
        private SerializedProperty m_MaxDelayProperty;
        private GUIContent m_MinUpdateTimeoutLabel;
        private SerializedProperty m_NetworkAddressProperty;
        protected NetworkManager m_NetworkManager;
        private SerializedProperty m_NetworkPortProperty;
        private GUIContent m_OfflineSceneLabel;
        private GUIContent m_OnlineSceneLabel;
        private GUIContent m_PacketLossPercentageLabel;
        private SerializedProperty m_PacketLossPercentageProperty;
        private GUIContent m_PingTimeoutLabel;
        private SerializedProperty m_PlayerPrefabProperty;
        private SerializedProperty m_PlayerSpawnMethodProperty;
        private GUIContent m_ReactorMaximumReceivedMessagesLabel;
        private GUIContent m_ReactorMaximumSentMessagesLabel;
        private GUIContent m_ReactorModelLabel;
        protected GUIContent m_RunInBackgroundLabel;
        protected SerializedProperty m_RunInBackgroundProperty;
        protected GUIContent m_ScriptCRCCheckLabel;
        protected SerializedProperty m_ScriptCRCCheckProperty;
        private SerializedProperty m_ServerBindAddressProperty;
        private SerializedProperty m_ServerBindToIPProperty;
        private GUIContent m_ShowNetworkLabel;
        private GUIContent m_ShowSpawnLabel;
        private SerializedProperty m_SimulatedLatencyProperty;
        private ReorderableList m_SpawnList;
        private SerializedProperty m_SpawnListProperty;
        private GUIContent m_ThreadAwakeTimeoutLabel;
        private GUIContent m_UseSimulatorLabel;
        private SerializedProperty m_UseSimulatorProperty;
        private GUIContent m_UseWebSocketsLabel;
        private SerializedProperty m_UseWebSocketsProperty;

        internal void AddButton(Rect rect, ReorderableList list)
        {
            this.m_SpawnListProperty.arraySize++;
            list.index = this.m_SpawnListProperty.arraySize - 1;
            SerializedProperty arrayElementAtIndex = this.m_SpawnListProperty.GetArrayElementAtIndex(this.m_SpawnListProperty.arraySize - 1);
            if (arrayElementAtIndex.objectReferenceValue != null)
            {
                arrayElementAtIndex.objectReferenceValue = null;
            }
            this.m_SpawnList.index = this.m_SpawnList.count - 1;
        }

        internal void Changed(ReorderableList list)
        {
            EditorUtility.SetDirty(base.target);
        }

        internal void ChannelAddButton(Rect rect, ReorderableList list)
        {
            this.m_ChannelListProperty.arraySize++;
            this.m_ChannelListProperty.GetArrayElementAtIndex(this.m_ChannelListProperty.arraySize - 1).enumValueIndex = 5;
            list.index = this.m_ChannelListProperty.arraySize - 1;
        }

        internal void ChannelChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(base.target);
        }

        internal void ChannelDrawChild(Rect r, int index, bool isActive, bool isFocused)
        {
            QosType enumValueIndex = (QosType) this.m_ChannelListProperty.GetArrayElementAtIndex(index).enumValueIndex;
            QosType type2 = (QosType) EditorGUI.EnumPopup(r, "Channel #" + index, enumValueIndex);
            if (type2 != enumValueIndex)
            {
                this.m_ChannelListProperty.GetArrayElementAtIndex(index).enumValueIndex = (int) type2;
            }
        }

        private static void ChannelDrawHeader(Rect headerRect)
        {
            GUI.Label(headerRect, "Qos Channels:");
        }

        internal void ChannelRemoveButton(ReorderableList list)
        {
            if (this.m_NetworkManager.channels.Count == 1)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Cannot remove channel. There must be at least one QoS channel.");
                }
            }
            else
            {
                this.m_ChannelListProperty.DeleteArrayElementAtIndex(this.m_ChannelList.index);
                if (list.index >= (this.m_ChannelListProperty.arraySize - 1))
                {
                    list.index = this.m_ChannelListProperty.arraySize - 1;
                }
            }
        }

        internal void DrawChild(Rect r, int index, bool isActive, bool isFocused)
        {
            GUIContent content;
            SerializedProperty arrayElementAtIndex = this.m_SpawnListProperty.GetArrayElementAtIndex(index);
            GameObject objectReferenceValue = (GameObject) arrayElementAtIndex.objectReferenceValue;
            if (objectReferenceValue == null)
            {
                content = new GUIContent("Empty", "Drag a prefab with a NetworkIdentity here");
            }
            else
            {
                NetworkIdentity component = objectReferenceValue.GetComponent<NetworkIdentity>();
                if (component != null)
                {
                    content = new GUIContent(objectReferenceValue.name, "AssetId: [" + component.assetId + "]");
                }
                else
                {
                    content = new GUIContent(objectReferenceValue.name, "No Network Identity");
                }
            }
            GameObject obj3 = (GameObject) EditorGUI.ObjectField(r, content, objectReferenceValue, typeof(GameObject), false);
            if (obj3 != objectReferenceValue)
            {
                if ((obj3 != null) && (obj3.GetComponent<NetworkIdentity>() == null))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Prefab " + obj3 + " cannot be added as spawnable as it doesn't have a NetworkIdentity.");
                    }
                }
                else
                {
                    arrayElementAtIndex.objectReferenceValue = obj3;
                }
            }
        }

        private static void DrawHeader(Rect headerRect)
        {
            GUI.Label(headerRect, "Registered Spawnable Prefabs:");
        }

        protected SceneAsset GetSceneObject(string sceneObjectName)
        {
            if (!string.IsNullOrEmpty(sceneObjectName))
            {
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (scene.path.IndexOf(sceneObjectName) != -1)
                    {
                        return (AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset);
                    }
                }
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used with networking. Add this scene to the 'Scenes in the Build' in build settings.");
                }
            }
            return null;
        }

        protected void Init()
        {
            if (!this.m_Initialized)
            {
                this.m_Initialized = true;
                this.m_NetworkManager = base.target as NetworkManager;
                this.m_ShowNetworkLabel = new GUIContent("Network Info", "Network host names and ports");
                this.m_ShowSpawnLabel = new GUIContent("Spawn Info", "Registered spawnable objects");
                this.m_OfflineSceneLabel = new GUIContent("Offline Scene", "The scene loaded when the network goes offline (disconnected from server)");
                this.m_OnlineSceneLabel = new GUIContent("Online Scene", "The scene loaded when the network comes online (connected to server)");
                this.m_DontDestroyOnLoadLabel = new GUIContent("Dont Destroy On Load", "Persist the network manager across scene changes.");
                this.m_RunInBackgroundLabel = new GUIContent("Run in Background", "This ensures that the application runs when it does not have focus. This is required when testing multiple instances on a single machine, but not recommended for shipping on mobile platforms.");
                this.m_ScriptCRCCheckLabel = new GUIContent("Script CRC Check", "Enables a CRC check between server and client that ensures the NetworkBehaviour scripts match. This may not be appropriate in some cases, such a when the client and server are different Unity projects.");
                this.m_MaxConnectionsLabel = new GUIContent("Max Connections", "Maximum number of network connections");
                this.m_MinUpdateTimeoutLabel = new GUIContent("Min Update Timeout", "Minimum time network thread waits for events");
                this.m_ConnectTimeoutLabel = new GUIContent("Connect Timeout", "Time to wait for timeout on connecting");
                this.m_DisconnectTimeoutLabel = new GUIContent("Disconnect Timeout", "Time to wait for detecting disconnect");
                this.m_PingTimeoutLabel = new GUIContent("Ping Timeout", "Time to wait for ping messages");
                this.m_ThreadAwakeTimeoutLabel = new GUIContent("Thread Awake Timeout", "The minimum time period when system will check if there are any messages for send (or receive).");
                this.m_ReactorModelLabel = new GUIContent("Reactor Model", "Defines reactor model for the network library");
                this.m_ReactorMaximumReceivedMessagesLabel = new GUIContent("Reactor Max Recv Messages", "Defines maximum amount of messages in the receive queue");
                this.m_ReactorMaximumSentMessagesLabel = new GUIContent("Reactor Max Sent Messages", "Defines maximum message count in sent queue");
                this.m_MaxBufferedPacketsLabel = new GUIContent("Max Buffered Packets", "The maximum number of packets that can be buffered by a NetworkConnection for each channel. This corresponds to the 'ChannelOption.MaxPendingBuffers' channel option.");
                this.m_AllowFragmentationLabel = new GUIContent("Packet Fragmentation", "This allow NetworkConnection instances to fragment packets that are larger than the maxPacketSize, up to a maximum of 64K. This can cause delays in sending large packets, but is usually preferable to send failures.");
                this.m_UseWebSocketsLabel = new GUIContent("Use WebSockets", "This makes the server listen for connections using WebSockets. This allows WebGL clients to connect to the server.");
                this.m_UseSimulatorLabel = new GUIContent("Use Network Simulator", "This simulates network latency and packet loss on clients. Useful for testing under internet-like conditions");
                this.m_LatencyLabel = new GUIContent("Simulated Average Latency", "The amount of delay in milliseconds to add to network packets");
                this.m_PacketLossPercentageLabel = new GUIContent("Simulated Packet Loss", "The percentage of packets that should be dropped");
                this.m_MatchHostLabel = new GUIContent("MatchMaker Host URI", "The URI for the MatchMaker.");
                this.m_MatchPortLabel = new GUIContent("MatchMaker Port", "The port for the MatchMaker.");
                this.m_MatchNameLabel = new GUIContent("Match Name", "The name that will be used when creating a match in MatchMaker.");
                this.m_MatchSizeLabel = new GUIContent("Maximum Match Size", "The maximum size for the match. This value is compared to the maximum size specified in the service configuration at multiplayer.unity3d.com and the lower of the two is enforced. It must be greater than 1. This is typically used to override the match size for various game modes.");
                this.m_DontDestroyOnLoadProperty = base.serializedObject.FindProperty("m_DontDestroyOnLoad");
                this.m_RunInBackgroundProperty = base.serializedObject.FindProperty("m_RunInBackground");
                this.m_ScriptCRCCheckProperty = base.serializedObject.FindProperty("m_ScriptCRCCheck");
                this.m_LogLevelProperty = base.serializedObject.FindProperty("m_LogLevel");
                this.m_NetworkAddressProperty = base.serializedObject.FindProperty("m_NetworkAddress");
                this.m_NetworkPortProperty = base.serializedObject.FindProperty("m_NetworkPort");
                this.m_ServerBindToIPProperty = base.serializedObject.FindProperty("m_ServerBindToIP");
                this.m_ServerBindAddressProperty = base.serializedObject.FindProperty("m_ServerBindAddress");
                this.m_MaxDelayProperty = base.serializedObject.FindProperty("m_MaxDelay");
                this.m_MaxBufferedPacketsProperty = base.serializedObject.FindProperty("m_MaxBufferedPackets");
                this.m_AllowFragmentationProperty = base.serializedObject.FindProperty("m_AllowFragmentation");
                this.m_MatchHostProperty = base.serializedObject.FindProperty("m_MatchHost");
                this.m_MatchPortProperty = base.serializedObject.FindProperty("m_MatchPort");
                this.m_MatchNameProperty = base.serializedObject.FindProperty("matchName");
                this.m_MatchSizeProperty = base.serializedObject.FindProperty("matchSize");
                this.m_PlayerPrefabProperty = base.serializedObject.FindProperty("m_PlayerPrefab");
                this.m_AutoCreatePlayerProperty = base.serializedObject.FindProperty("m_AutoCreatePlayer");
                this.m_PlayerSpawnMethodProperty = base.serializedObject.FindProperty("m_PlayerSpawnMethod");
                this.m_SpawnListProperty = base.serializedObject.FindProperty("m_SpawnPrefabs");
                this.m_SpawnList = new ReorderableList(base.serializedObject, this.m_SpawnListProperty);
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new ReorderableList.HeaderCallbackDelegate(NetworkManagerEditor.DrawHeader);
                }
                this.m_SpawnList.drawHeaderCallback = <>f__mg$cache0;
                this.m_SpawnList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawChild);
                this.m_SpawnList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.Changed);
                this.m_SpawnList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.AddButton);
                this.m_SpawnList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
                this.m_SpawnList.onChangedCallback = new ReorderableList.ChangedCallbackDelegate(this.Changed);
                this.m_SpawnList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.Changed);
                this.m_SpawnList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.Changed);
                this.m_SpawnList.elementHeight = 16f;
                this.m_CustomConfigProperty = base.serializedObject.FindProperty("m_CustomConfig");
                this.m_ChannelListProperty = base.serializedObject.FindProperty("m_Channels");
                this.m_ChannelList = new ReorderableList(base.serializedObject, this.m_ChannelListProperty);
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new ReorderableList.HeaderCallbackDelegate(NetworkManagerEditor.ChannelDrawHeader);
                }
                this.m_ChannelList.drawHeaderCallback = <>f__mg$cache1;
                this.m_ChannelList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.ChannelDrawChild);
                this.m_ChannelList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.ChannelChanged);
                this.m_ChannelList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.ChannelAddButton);
                this.m_ChannelList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.ChannelRemoveButton);
                this.m_ChannelList.onChangedCallback = new ReorderableList.ChangedCallbackDelegate(this.ChannelChanged);
                this.m_ChannelList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.ChannelChanged);
                this.m_ChannelList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.ChannelChanged);
                this.m_UseWebSocketsProperty = base.serializedObject.FindProperty("m_UseWebSockets");
                this.m_UseSimulatorProperty = base.serializedObject.FindProperty("m_UseSimulator");
                this.m_SimulatedLatencyProperty = base.serializedObject.FindProperty("m_SimulatedLatency");
                this.m_PacketLossPercentageProperty = base.serializedObject.FindProperty("m_PacketLossPercentage");
            }
        }

        public override void OnInspectorGUI()
        {
            if ((this.m_DontDestroyOnLoadProperty == null) || (this.m_DontDestroyOnLoadLabel == null))
            {
                this.m_Initialized = false;
            }
            this.Init();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_DontDestroyOnLoadProperty, this.m_DontDestroyOnLoadLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_RunInBackgroundProperty, this.m_RunInBackgroundLabel, new GUILayoutOption[0]);
            if (EditorGUILayout.PropertyField(this.m_LogLevelProperty, new GUILayoutOption[0]))
            {
                LogFilter.currentLogLevel = (int) this.m_NetworkManager.logLevel;
            }
            this.ShowScenes();
            this.ShowNetworkInfo();
            this.ShowSpawnInfo();
            this.ShowConfigInfo();
            this.ShowSimulatorInfo();
            base.serializedObject.ApplyModifiedProperties();
            this.ShowDerivedProperties(typeof(NetworkManager), null);
        }

        internal void RemoveButton(ReorderableList list)
        {
            this.m_SpawnListProperty.DeleteArrayElementAtIndex(this.m_SpawnList.index);
            if (list.index >= this.m_SpawnListProperty.arraySize)
            {
                list.index = this.m_SpawnListProperty.arraySize - 1;
            }
        }

        protected void ShowConfigInfo()
        {
            bool customConfig = this.m_NetworkManager.customConfig;
            EditorGUILayout.PropertyField(this.m_CustomConfigProperty, new GUIContent("Advanced Configuration"), new GUILayoutOption[0]);
            if ((this.m_CustomConfigProperty.boolValue && !customConfig) && (this.m_NetworkManager.channels.Count == 0))
            {
                this.m_NetworkManager.channels.Add(QosType.ReliableSequenced);
                this.m_NetworkManager.channels.Add(QosType.Unreliable);
                this.m_NetworkManager.customConfig = true;
                this.m_CustomConfigProperty.serializedObject.Update();
                this.m_ChannelList.serializedProperty.serializedObject.Update();
            }
            if (this.m_NetworkManager.customConfig)
            {
                EditorGUI.indentLevel++;
                SerializedProperty prop = base.serializedObject.FindProperty("m_MaxConnections");
                ShowPropertySuffix(this.m_MaxConnectionsLabel, prop, "connections");
                this.m_ChannelList.DoLayoutList();
                prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, "Timeouts");
                if (prop.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    SerializedProperty property2 = base.serializedObject.FindProperty("m_ConnectionConfig.m_MinUpdateTimeout");
                    SerializedProperty property3 = base.serializedObject.FindProperty("m_ConnectionConfig.m_ConnectTimeout");
                    SerializedProperty property4 = base.serializedObject.FindProperty("m_ConnectionConfig.m_DisconnectTimeout");
                    SerializedProperty property5 = base.serializedObject.FindProperty("m_ConnectionConfig.m_PingTimeout");
                    ShowPropertySuffix(this.m_MinUpdateTimeoutLabel, property2, "millisec");
                    ShowPropertySuffix(this.m_ConnectTimeoutLabel, property3, "millisec");
                    ShowPropertySuffix(this.m_DisconnectTimeoutLabel, property4, "millisec");
                    ShowPropertySuffix(this.m_PingTimeoutLabel, property5, "millisec");
                    EditorGUI.indentLevel--;
                }
                SerializedProperty property6 = base.serializedObject.FindProperty("m_GlobalConfig.m_ThreadAwakeTimeout");
                property6.isExpanded = EditorGUILayout.Foldout(property6.isExpanded, "Global Config");
                if (property6.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    SerializedProperty property = base.serializedObject.FindProperty("m_GlobalConfig.m_ReactorModel");
                    SerializedProperty property8 = base.serializedObject.FindProperty("m_GlobalConfig.m_ReactorMaximumReceivedMessages");
                    SerializedProperty property9 = base.serializedObject.FindProperty("m_GlobalConfig.m_ReactorMaximumSentMessages");
                    ShowPropertySuffix(this.m_ThreadAwakeTimeoutLabel, property6, "millisec");
                    EditorGUILayout.PropertyField(property, this.m_ReactorModelLabel, new GUILayoutOption[0]);
                    ShowPropertySuffix(this.m_ReactorMaximumReceivedMessagesLabel, property8, "messages");
                    ShowPropertySuffix(this.m_ReactorMaximumSentMessagesLabel, property9, "messages");
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        protected void ShowDerivedProperties(System.Type baseType, System.Type superType)
        {
            bool flag = true;
            SerializedProperty iterator = base.serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                System.Reflection.FieldInfo field = baseType.GetField(iterator.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo property = baseType.GetProperty(iterator.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if ((field == null) && (superType != null))
                {
                    field = superType.GetField(iterator.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                }
                if ((property == null) && (superType != null))
                {
                    property = superType.GetProperty(iterator.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                }
                if ((field == null) && (property == null))
                {
                    if (flag)
                    {
                        flag = false;
                        EditorGUI.BeginChangeCheck();
                        base.serializedObject.Update();
                        EditorGUILayout.Separator();
                    }
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                    enterChildren = false;
                }
            }
            if (!flag)
            {
                base.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndChangeCheck();
            }
        }

        protected void ShowNetworkInfo()
        {
            this.m_NetworkAddressProperty.isExpanded = EditorGUILayout.Foldout(this.m_NetworkAddressProperty.isExpanded, this.m_ShowNetworkLabel);
            if (this.m_NetworkAddressProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
                if (EditorGUILayout.PropertyField(this.m_UseWebSocketsProperty, this.m_UseWebSocketsLabel, new GUILayoutOption[0]))
                {
                    NetworkServer.useWebSockets = this.m_NetworkManager.useWebSockets;
                }
                EditorGUILayout.PropertyField(this.m_NetworkAddressProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_NetworkPortProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ServerBindToIPProperty, new GUILayoutOption[0]);
                if (this.m_NetworkManager.serverBindToIP)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(this.m_ServerBindAddressProperty, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(this.m_ScriptCRCCheckProperty, this.m_ScriptCRCCheckLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MaxDelayProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MaxBufferedPacketsProperty, this.m_MaxBufferedPacketsLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_AllowFragmentationProperty, this.m_AllowFragmentationLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MatchHostProperty, this.m_MatchHostLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MatchPortProperty, this.m_MatchPortLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MatchNameProperty, this.m_MatchNameLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MatchSizeProperty, this.m_MatchSizeLabel, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
        }

        private static void ShowPropertySuffix(GUIContent content, SerializedProperty prop, string suffix)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(prop, content, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(64f) };
            GUILayout.Label(suffix, EditorStyles.miniLabel, options);
            EditorGUILayout.EndHorizontal();
        }

        protected void ShowScenes()
        {
            SceneAsset sceneObject = this.GetSceneObject(this.m_NetworkManager.offlineScene);
            UnityEngine.Object obj2 = EditorGUILayout.ObjectField(this.m_OfflineSceneLabel, sceneObject, typeof(SceneAsset), false, new GUILayoutOption[0]);
            if (obj2 == null)
            {
                base.serializedObject.FindProperty("m_OfflineScene").stringValue = "";
                EditorUtility.SetDirty(base.target);
            }
            else if (obj2.name != this.m_NetworkManager.offlineScene)
            {
                if (this.GetSceneObject(obj2.name) == null)
                {
                    Debug.LogWarning("The scene " + obj2.name + " cannot be used. To use this scene add it to the build settings for the project");
                }
                else
                {
                    base.serializedObject.FindProperty("m_OfflineScene").stringValue = obj2.name;
                    EditorUtility.SetDirty(base.target);
                }
            }
            SceneAsset asset3 = this.GetSceneObject(this.m_NetworkManager.onlineScene);
            UnityEngine.Object obj3 = EditorGUILayout.ObjectField(this.m_OnlineSceneLabel, asset3, typeof(SceneAsset), false, new GUILayoutOption[0]);
            if (obj3 == null)
            {
                base.serializedObject.FindProperty("m_OnlineScene").stringValue = "";
                EditorUtility.SetDirty(base.target);
            }
            else if (obj3.name != this.m_NetworkManager.onlineScene)
            {
                if (this.GetSceneObject(obj3.name) == null)
                {
                    Debug.LogWarning("The scene " + obj3.name + " cannot be used. To use this scene add it to the build settings for the project");
                }
                else
                {
                    base.serializedObject.FindProperty("m_OnlineScene").stringValue = obj3.name;
                    EditorUtility.SetDirty(base.target);
                }
            }
        }

        protected void ShowSimulatorInfo()
        {
            EditorGUILayout.PropertyField(this.m_UseSimulatorProperty, this.m_UseSimulatorLabel, new GUILayoutOption[0]);
            if (this.m_UseSimulatorProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                if (Application.isPlaying && (this.m_NetworkManager.client != null))
                {
                    EditorGUILayout.LabelField(this.m_LatencyLabel, new GUIContent(this.m_NetworkManager.simulatedLatency + " milliseconds"), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField(this.m_PacketLossPercentageLabel, new GUIContent(this.m_NetworkManager.packetLossPercentage + "%"), new GUILayoutOption[0]);
                }
                else
                {
                    int simulatedLatency = this.m_NetworkManager.simulatedLatency;
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    int num2 = EditorGUILayout.IntSlider(this.m_LatencyLabel, simulatedLatency, 1, 400, new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(64f) };
                    GUILayout.Label("millsec", EditorStyles.miniLabel, options);
                    EditorGUILayout.EndHorizontal();
                    if (num2 != simulatedLatency)
                    {
                        this.m_SimulatedLatencyProperty.intValue = num2;
                    }
                    float packetLossPercentage = this.m_NetworkManager.packetLossPercentage;
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    float num4 = EditorGUILayout.Slider(this.m_PacketLossPercentageLabel, packetLossPercentage, 0f, 20f, new GUILayoutOption[0]);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(64f) };
                    GUILayout.Label("%", EditorStyles.miniLabel, optionArray2);
                    EditorGUILayout.EndHorizontal();
                    if (num4 != packetLossPercentage)
                    {
                        this.m_PacketLossPercentageProperty.floatValue = num4;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        protected void ShowSpawnInfo()
        {
            this.m_PlayerPrefabProperty.isExpanded = EditorGUILayout.Foldout(this.m_PlayerPrefabProperty.isExpanded, this.m_ShowSpawnLabel);
            if (this.m_PlayerPrefabProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
                if (!typeof(NetworkLobbyManager).IsAssignableFrom(this.m_NetworkManager.GetType()))
                {
                    EditorGUILayout.PropertyField(this.m_PlayerPrefabProperty, new GUILayoutOption[0]);
                }
                EditorGUILayout.PropertyField(this.m_AutoCreatePlayerProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_PlayerSpawnMethodProperty, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                this.m_SpawnList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                {
                    base.serializedObject.ApplyModifiedProperties();
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}

