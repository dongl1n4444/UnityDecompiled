namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.NetworkSystem;

    [CustomEditor(typeof(NetworkMigrationManager), true)]
    public class NetworkMigrationManagerEditor : Editor
    {
        private GUIContent m_HostMigrationLabel;
        private SerializedProperty m_HostMigrationProperty;
        private bool m_Initialized;
        private NetworkMigrationManager m_Manager;
        private SerializedProperty m_OffsetXProperty;
        private SerializedProperty m_OffsetYProperty;
        private SerializedProperty m_ShowGUIProperty;
        private bool m_ShowPeers;
        private bool m_ShowPlayers;

        private void DrawControls()
        {
            if (this.m_Manager != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_HostMigrationProperty, this.m_HostMigrationLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ShowGUIProperty, new GUILayoutOption[0]);
                if (this.m_Manager.showGUI)
                {
                    EditorGUILayout.PropertyField(this.m_OffsetXProperty, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_OffsetYProperty, new GUILayoutOption[0]);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    base.serializedObject.ApplyModifiedProperties();
                }
                if (Application.isPlaying)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Disconnected From Host", this.m_Manager.disconnectedFromHost.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Waiting to become New Host", this.m_Manager.waitingToBecomeNewHost.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Waitingto Reconnect to New Host", this.m_Manager.waitingReconnectToNewHost.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Your ConnectionId", this.m_Manager.oldServerConnectionId.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("New Host Address", this.m_Manager.newHostAddress, new GUILayoutOption[0]);
                    if (this.m_Manager.peers != null)
                    {
                        this.m_ShowPeers = EditorGUILayout.Foldout(this.m_ShowPeers, "Peers");
                        if (this.m_ShowPeers)
                        {
                            EditorGUI.indentLevel++;
                            foreach (PeerInfoMessage message in this.m_Manager.peers)
                            {
                                EditorGUILayout.LabelField("Peer: ", message.ToString(), new GUILayoutOption[0]);
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    if (this.m_Manager.pendingPlayers != null)
                    {
                        this.m_ShowPlayers = EditorGUILayout.Foldout(this.m_ShowPlayers, "Pending Players");
                        if (this.m_ShowPlayers)
                        {
                            EditorGUI.indentLevel++;
                            foreach (int num3 in this.m_Manager.pendingPlayers.Keys)
                            {
                                EditorGUILayout.LabelField("Connection: ", num3.ToString(), new GUILayoutOption[0]);
                                EditorGUI.indentLevel++;
                                NetworkMigrationManager.ConnectionPendingPlayers players = this.m_Manager.pendingPlayers[num3];
                                List<NetworkMigrationManager.PendingPlayerInfo> list = players.players;
                                foreach (NetworkMigrationManager.PendingPlayerInfo info in list)
                                {
                                    EditorGUILayout.ObjectField(string.Concat(new object[] { "Player netId:", info.netId, " contId:", info.playerControllerId }), info.obj, typeof(GameObject), false, new GUILayoutOption[0]);
                                }
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
        }

        private void Init()
        {
            if (!this.m_Initialized || (this.m_HostMigrationProperty == null))
            {
                this.m_Initialized = true;
                this.m_Manager = base.target as NetworkMigrationManager;
                this.m_HostMigrationProperty = base.serializedObject.FindProperty("m_HostMigration");
                this.m_ShowGUIProperty = base.serializedObject.FindProperty("m_ShowGUI");
                this.m_OffsetXProperty = base.serializedObject.FindProperty("m_OffsetX");
                this.m_OffsetYProperty = base.serializedObject.FindProperty("m_OffsetY");
                this.m_HostMigrationLabel = new GUIContent("Use Host Migration", "s.");
            }
        }

        public override void OnInspectorGUI()
        {
            this.Init();
            base.serializedObject.Update();
            this.DrawControls();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

