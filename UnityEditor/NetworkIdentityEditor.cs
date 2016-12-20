namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkIdentity), true), CanEditMultipleObjects]
    public class NetworkIdentityEditor : Editor
    {
        private bool m_Initialized;
        private GUIContent m_LocalPlayerAuthorityLabel;
        private SerializedProperty m_LocalPlayerAuthorityProperty;
        private NetworkIdentity m_NetworkIdentity;
        private GUIContent m_ServerOnlyLabel;
        private SerializedProperty m_ServerOnlyProperty;
        private bool m_ShowObservers;
        private GUIContent m_SpawnLabel;

        private void Init()
        {
            if (!this.m_Initialized)
            {
                this.m_Initialized = true;
                this.m_NetworkIdentity = base.target as NetworkIdentity;
                this.m_ServerOnlyProperty = base.serializedObject.FindProperty("m_ServerOnly");
                this.m_LocalPlayerAuthorityProperty = base.serializedObject.FindProperty("m_LocalPlayerAuthority");
                this.m_ServerOnlyLabel = new GUIContent("Server Only", "True if the object should only exist on the server.");
                this.m_LocalPlayerAuthorityLabel = new GUIContent("Local Player Authority", "True if this object will be controlled by a player on a client.");
                this.m_SpawnLabel = new GUIContent("Spawn Object", "This causes an unspawned server object to be spawned on clients");
            }
        }

        public override void OnInspectorGUI()
        {
            if (this.m_ServerOnlyProperty == null)
            {
                this.m_Initialized = false;
            }
            this.Init();
            base.serializedObject.Update();
            if (this.m_ServerOnlyProperty.boolValue)
            {
                EditorGUILayout.PropertyField(this.m_ServerOnlyProperty, this.m_ServerOnlyLabel, new GUILayoutOption[0]);
                EditorGUILayout.LabelField("Local Player Authority cannot be set for server-only objects", new GUILayoutOption[0]);
            }
            else if (this.m_LocalPlayerAuthorityProperty.boolValue)
            {
                EditorGUILayout.LabelField("Server Only cannot be set for Local Player Authority objects", new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LocalPlayerAuthorityProperty, this.m_LocalPlayerAuthorityLabel, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_ServerOnlyProperty, this.m_ServerOnlyLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LocalPlayerAuthorityProperty, this.m_LocalPlayerAuthorityLabel, new GUILayoutOption[0]);
            }
            base.serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying)
            {
                EditorGUILayout.Separator();
                if ((this.m_NetworkIdentity.observers != null) && (this.m_NetworkIdentity.observers.Count > 0))
                {
                    this.m_ShowObservers = EditorGUILayout.Foldout(this.m_ShowObservers, "Observers");
                    if (this.m_ShowObservers)
                    {
                        EditorGUI.indentLevel++;
                        foreach (NetworkConnection connection in this.m_NetworkIdentity.observers)
                        {
                            GameObject gameObject = null;
                            foreach (PlayerController controller in connection.playerControllers)
                            {
                                if (controller != null)
                                {
                                    gameObject = controller.gameObject;
                                    break;
                                }
                            }
                            if (gameObject != null)
                            {
                                EditorGUILayout.ObjectField("Connection " + connection.connectionId, gameObject, typeof(GameObject), false, new GUILayoutOption[0]);
                            }
                            else
                            {
                                EditorGUILayout.TextField("Connection " + connection.connectionId, new GUILayoutOption[0]);
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                if ((PrefabUtility.GetPrefabType(this.m_NetworkIdentity.gameObject) != PrefabType.Prefab) && ((this.m_NetworkIdentity.gameObject.activeSelf && this.m_NetworkIdentity.netId.IsEmpty()) && NetworkServer.active))
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.LabelField(this.m_SpawnLabel, new GUILayoutOption[0]);
                    if (GUILayout.Toggle(false, "Spawn", EditorStyles.miniButtonLeft, new GUILayoutOption[0]))
                    {
                        NetworkServer.Spawn(this.m_NetworkIdentity.gameObject);
                        EditorUtility.SetDirty(base.target);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}

