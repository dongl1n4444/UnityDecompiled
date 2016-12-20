namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkDiscovery), true), CanEditMultipleObjects]
    public class NetworkDiscoveryEditor : Editor
    {
        private GUIContent m_BroadcastDataLabel;
        private SerializedProperty m_BroadcastDataProperty;
        private GUIContent m_BroadcastIntervalLabel;
        private SerializedProperty m_BroadcastIntervalProperty;
        private GUIContent m_BroadcastKeyLabel;
        private SerializedProperty m_BroadcastKeyProperty;
        private GUIContent m_BroadcastPortLabel;
        private SerializedProperty m_BroadcastPortProperty;
        private GUIContent m_BroadcastSubVersionLabel;
        private SerializedProperty m_BroadcastSubVersionProperty;
        private GUIContent m_BroadcastVersionLabel;
        private SerializedProperty m_BroadcastVersionProperty;
        private NetworkDiscovery m_Discovery;
        private bool m_Initialized;
        private SerializedProperty m_OffsetXProperty;
        private SerializedProperty m_OffsetYProperty;
        private SerializedProperty m_ShowGUIProperty;
        private GUIContent m_UseNetworkManagerLabel;
        private SerializedProperty m_UseNetworkManagerProperty;

        private void DrawControls()
        {
            if (this.m_Discovery != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_BroadcastPortProperty, this.m_BroadcastPortLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_BroadcastKeyProperty, this.m_BroadcastKeyLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_BroadcastVersionProperty, this.m_BroadcastVersionLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_BroadcastSubVersionProperty, this.m_BroadcastSubVersionLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_BroadcastIntervalProperty, this.m_BroadcastIntervalLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_UseNetworkManagerProperty, this.m_UseNetworkManagerLabel, new GUILayoutOption[0]);
                if (this.m_Discovery.useNetworkManager)
                {
                    EditorGUILayout.LabelField(this.m_BroadcastDataLabel, new GUIContent(this.m_BroadcastDataProperty.stringValue), new GUILayoutOption[0]);
                }
                else
                {
                    EditorGUILayout.PropertyField(this.m_BroadcastDataProperty, this.m_BroadcastDataLabel, new GUILayoutOption[0]);
                }
                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(this.m_ShowGUIProperty, new GUILayoutOption[0]);
                if (this.m_Discovery.showGUI)
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
                    EditorGUILayout.LabelField("hostId", this.m_Discovery.hostId.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("running", this.m_Discovery.running.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("isServer", this.m_Discovery.isServer.ToString(), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("isClient", this.m_Discovery.isClient.ToString(), new GUILayoutOption[0]);
                }
            }
        }

        private void Init()
        {
            if (!this.m_Initialized || (this.m_BroadcastPortProperty == null))
            {
                this.m_Initialized = true;
                this.m_Discovery = base.target as NetworkDiscovery;
                this.m_BroadcastPortProperty = base.serializedObject.FindProperty("m_BroadcastPort");
                this.m_BroadcastKeyProperty = base.serializedObject.FindProperty("m_BroadcastKey");
                this.m_BroadcastVersionProperty = base.serializedObject.FindProperty("m_BroadcastVersion");
                this.m_BroadcastSubVersionProperty = base.serializedObject.FindProperty("m_BroadcastSubVersion");
                this.m_BroadcastIntervalProperty = base.serializedObject.FindProperty("m_BroadcastInterval");
                this.m_UseNetworkManagerProperty = base.serializedObject.FindProperty("m_UseNetworkManager");
                this.m_BroadcastDataProperty = base.serializedObject.FindProperty("m_BroadcastData");
                this.m_ShowGUIProperty = base.serializedObject.FindProperty("m_ShowGUI");
                this.m_OffsetXProperty = base.serializedObject.FindProperty("m_OffsetX");
                this.m_OffsetYProperty = base.serializedObject.FindProperty("m_OffsetY");
                this.m_BroadcastPortLabel = new GUIContent("Broadcast Port", "The network port to broadcast to, and listen on.");
                this.m_BroadcastKeyLabel = new GUIContent("Broadcast Key", "The key to broadcast. This key typically identifies the application.");
                this.m_BroadcastVersionLabel = new GUIContent("Broadcast Version", "The version of the application to broadcast. This is used to match versions of the same application.");
                this.m_BroadcastSubVersionLabel = new GUIContent("Broadcast SubVersion", "The sub-version of the application to broadcast.");
                this.m_BroadcastIntervalLabel = new GUIContent("Broadcast Interval", "How often in milliseconds to broadcast when running as a server.");
                this.m_UseNetworkManagerLabel = new GUIContent("Use NetworkManager", "Broadcast information from the NetworkManager, and auto-join matching games using the NetworkManager.");
                this.m_BroadcastDataLabel = new GUIContent("Broadcast Data", "The data to broadcast when not using the NetworkManager");
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

