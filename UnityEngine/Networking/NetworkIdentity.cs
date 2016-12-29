namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    /// <summary>
    /// <para>The NetworkIdentity identifies objects across the network, between server and clients. Its primary data is a NetworkInstanceId which is allocated by the server and then set on clients. This is used in network communications to be able to lookup game objects on different machines.</para>
    /// </summary>
    [ExecuteInEditMode, DisallowMultipleComponent, AddComponentMenu("Network/NetworkIdentity")]
    public sealed class NetworkIdentity : MonoBehaviour
    {
        /// <summary>
        /// <para>A callback that can be populated to be notified when the client-authority state of objects changes.</para>
        /// </summary>
        public static ClientAuthorityCallback clientAuthorityCallback;
        [SerializeField]
        private NetworkHash128 m_AssetId;
        private NetworkConnection m_ClientAuthorityOwner;
        private NetworkConnection m_ConnectionToClient;
        private NetworkConnection m_ConnectionToServer;
        private bool m_HasAuthority;
        private bool m_IsClient;
        private bool m_IsLocalPlayer;
        private bool m_IsServer;
        [SerializeField]
        private bool m_LocalPlayerAuthority;
        private NetworkInstanceId m_NetId;
        private NetworkBehaviour[] m_NetworkBehaviours;
        private HashSet<int> m_ObserverConnections;
        private List<NetworkConnection> m_Observers;
        private short m_PlayerId = -1;
        [SerializeField]
        private NetworkSceneId m_SceneId;
        [SerializeField]
        private bool m_ServerOnly;
        private static uint s_NextNetworkId = 1;
        private static NetworkWriter s_UpdateWriter = new NetworkWriter();

        internal static void AddNetworkId(uint id)
        {
            if (id >= s_NextNetworkId)
            {
                s_NextNetworkId = id + 1;
            }
        }

        internal void AddObserver(NetworkConnection conn)
        {
            if (this.m_Observers == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AddObserver for " + base.gameObject + " observer list is null");
                }
            }
            else if (this.m_ObserverConnections.Contains(conn.connectionId))
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Duplicate observer ", conn.address, " added for ", base.gameObject }));
                }
            }
            else
            {
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "Added observer ", conn.address, " added for ", base.gameObject }));
                }
                this.m_Observers.Add(conn);
                this.m_ObserverConnections.Add(conn.connectionId);
                conn.AddToVisList(this);
            }
        }

        private void AssignAssetID(GameObject prefab)
        {
            string assetPath = AssetDatabase.GetAssetPath(prefab);
            this.m_AssetId = NetworkHash128.Parse(AssetDatabase.AssetPathToGUID(assetPath));
        }

        /// <summary>
        /// <para>This assigns control of an object to a client via the client's NetworkConnection.</para>
        /// </summary>
        /// <param name="conn">The connection of the client to assign authority to.</param>
        /// <returns>
        /// <para>True if authority was assigned.</para>
        /// </returns>
        public bool AssignClientAuthority(NetworkConnection conn)
        {
            if (!this.isServer)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority can only be call on the server for spawned objects.");
                }
                return false;
            }
            if (!this.localPlayerAuthority)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority can only be used for NetworkIdentity component with LocalPlayerAuthority set.");
                }
                return false;
            }
            if ((this.m_ClientAuthorityOwner != null) && (conn != this.m_ClientAuthorityOwner))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority for " + base.gameObject + " already has an owner. Use RemoveClientAuthority() first.");
                }
                return false;
            }
            if (conn == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority for " + base.gameObject + " owner cannot be null. Use RemoveClientAuthority() instead.");
                }
                return false;
            }
            this.m_ClientAuthorityOwner = conn;
            this.m_ClientAuthorityOwner.AddOwnedObject(this);
            this.ForceAuthority(false);
            ClientAuthorityMessage msg = new ClientAuthorityMessage {
                netId = this.netId,
                authority = true
            };
            conn.Send(15, msg);
            if (clientAuthorityCallback != null)
            {
                clientAuthorityCallback(conn, this, true);
            }
            return true;
        }

        private void CacheBehaviours()
        {
            if (this.m_NetworkBehaviours == null)
            {
                this.m_NetworkBehaviours = base.GetComponents<NetworkBehaviour>();
            }
        }

        internal void ClearClientOwner()
        {
            this.m_ClientAuthorityOwner = null;
        }

        internal void ClearObservers()
        {
            if (this.m_Observers != null)
            {
                int count = this.m_Observers.Count;
                for (int i = 0; i < count; i++)
                {
                    this.m_Observers[i].RemoveFromVisList(this, true);
                }
                this.m_Observers.Clear();
                this.m_ObserverConnections.Clear();
            }
        }

        internal void ForceAuthority(bool authority)
        {
            if (this.m_HasAuthority != authority)
            {
                this.m_HasAuthority = authority;
                if (authority)
                {
                    this.OnStartAuthority();
                }
                else
                {
                    this.OnStopAuthority();
                }
            }
        }

        /// <summary>
        /// <para>Force the scene ID to a specific value.</para>
        /// </summary>
        /// <param name="sceneId">The new scene ID.</param>
        /// <param name="newSceneId"></param>
        public void ForceSceneId(int newSceneId)
        {
            this.m_SceneId = new NetworkSceneId((uint) newSceneId);
        }

        private bool GetInvokeComponent(int cmdHash, System.Type invokeClass, out NetworkBehaviour invokeComponent)
        {
            NetworkBehaviour behaviour = null;
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour2 = this.m_NetworkBehaviours[i];
                if ((behaviour2.GetType() == invokeClass) || behaviour2.GetType().IsSubclassOf(invokeClass))
                {
                    behaviour = behaviour2;
                    break;
                }
            }
            if (behaviour == null)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "Found no behaviour for incoming [", cmdHashHandlerName, "] on ", base.gameObject, ",  the server and client should have the same NetworkBehaviour instances [netId=", this.netId, "]." }));
                }
                invokeComponent = null;
                return false;
            }
            invokeComponent = behaviour;
            return true;
        }

        internal static NetworkInstanceId GetNextNetworkId()
        {
            uint num = s_NextNetworkId;
            s_NextNetworkId++;
            return new NetworkInstanceId(num);
        }

        internal void HandleClientAuthority(bool authority)
        {
            if (!this.localPlayerAuthority)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("HandleClientAuthority " + base.gameObject + " does not have localPlayerAuthority");
                }
            }
            else
            {
                this.ForceAuthority(authority);
            }
        }

        internal void HandleCommand(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "Command [", cmdHashHandlerName, "] received for deleted object [netId=", this.netId, "]" }));
                }
            }
            else
            {
                NetworkBehaviour.CmdDelegate delegate2;
                System.Type type;
                if (!NetworkBehaviour.GetInvokerForHashCommand(cmdHash, out type, out delegate2))
                {
                    string str2 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Found no receiver for incoming [", str2, "] on ", base.gameObject, ",  the server and client should have the same NetworkBehaviour instances [netId=", this.netId, "]." }));
                    }
                }
                else
                {
                    NetworkBehaviour behaviour;
                    if (!this.GetInvokeComponent(cmdHash, type, out behaviour))
                    {
                        string str3 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning(string.Concat(new object[] { "Command [", str3, "] handler not found [netId=", this.netId, "]" }));
                        }
                    }
                    else
                    {
                        delegate2(behaviour, reader);
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 5, NetworkBehaviour.GetCmdHashCmdName(cmdHash), 1);
                    }
                }
            }
        }

        internal void HandleRPC(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "ClientRpc [", cmdHashHandlerName, "] received for deleted object [netId=", this.netId, "]" }));
                }
            }
            else
            {
                NetworkBehaviour.CmdDelegate delegate2;
                System.Type type;
                if (!NetworkBehaviour.GetInvokerForHashClientRpc(cmdHash, out type, out delegate2))
                {
                    string str2 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Found no receiver for incoming [", str2, "] on ", base.gameObject, ",  the server and client should have the same NetworkBehaviour instances [netId=", this.netId, "]." }));
                    }
                }
                else
                {
                    NetworkBehaviour behaviour;
                    if (!this.GetInvokeComponent(cmdHash, type, out behaviour))
                    {
                        string str3 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning(string.Concat(new object[] { "ClientRpc [", str3, "] handler not found [netId=", this.netId, "]" }));
                        }
                    }
                    else
                    {
                        delegate2(behaviour, reader);
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 2, NetworkBehaviour.GetCmdHashRpcName(cmdHash), 1);
                    }
                }
            }
        }

        internal void HandleSyncEvent(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "SyncEvent [", cmdHashHandlerName, "] received for deleted object [netId=", this.netId, "]" }));
                }
            }
            else
            {
                NetworkBehaviour.CmdDelegate delegate2;
                System.Type type;
                if (!NetworkBehaviour.GetInvokerForHashSyncEvent(cmdHash, out type, out delegate2))
                {
                    string str2 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Found no receiver for incoming [", str2, "] on ", base.gameObject, ",  the server and client should have the same NetworkBehaviour instances [netId=", this.netId, "]." }));
                    }
                }
                else
                {
                    NetworkBehaviour behaviour;
                    if (!this.GetInvokeComponent(cmdHash, type, out behaviour))
                    {
                        string str3 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning(string.Concat(new object[] { "SyncEvent [", str3, "] handler not found [netId=", this.netId, "]" }));
                        }
                    }
                    else
                    {
                        delegate2(behaviour, reader);
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 7, NetworkBehaviour.GetCmdHashEventName(cmdHash), 1);
                    }
                }
            }
        }

        internal void HandleSyncList(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "SyncList [", cmdHashHandlerName, "] received for deleted object [netId=", this.netId, "]" }));
                }
            }
            else
            {
                NetworkBehaviour.CmdDelegate delegate2;
                System.Type type;
                if (!NetworkBehaviour.GetInvokerForHashSyncList(cmdHash, out type, out delegate2))
                {
                    string str2 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Found no receiver for incoming [", str2, "] on ", base.gameObject, ",  the server and client should have the same NetworkBehaviour instances [netId=", this.netId, "]." }));
                    }
                }
                else
                {
                    NetworkBehaviour behaviour;
                    if (!this.GetInvokeComponent(cmdHash, type, out behaviour))
                    {
                        string str3 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning(string.Concat(new object[] { "SyncList [", str3, "] handler not found [netId=", this.netId, "]" }));
                        }
                    }
                    else
                    {
                        delegate2(behaviour, reader);
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 9, NetworkBehaviour.GetCmdHashListName(cmdHash), 1);
                    }
                }
            }
        }

        internal bool OnCheckObserver(NetworkConnection conn)
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    if (!behaviour.OnCheckObserver(conn))
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnCheckObserver:" + exception.Message + " " + exception.StackTrace);
                }
            }
            return true;
        }

        private void OnDestroy()
        {
            if (this.m_IsServer && NetworkServer.active)
            {
                NetworkServer.Destroy(base.gameObject);
            }
        }

        internal void OnNetworkDestroy()
        {
            for (int i = 0; (this.m_NetworkBehaviours != null) && (i < this.m_NetworkBehaviours.Length); i++)
            {
                this.m_NetworkBehaviours[i].OnNetworkDestroy();
            }
            this.m_IsServer = false;
        }

        internal void OnSetLocalVisibility(bool vis)
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.OnSetLocalVisibility(vis);
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnSetLocalVisibility:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnStartAuthority()
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.OnStartAuthority();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnStartAuthority:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnStartClient()
        {
            if (!this.m_IsClient)
            {
                this.m_IsClient = true;
            }
            this.CacheBehaviours();
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "OnStartClient ", base.gameObject, " GUID:", this.netId, " localPlayerAuthority:", this.localPlayerAuthority }));
            }
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.PreStartClient();
                    behaviour.OnStartClient();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnStartClient:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnStartServer(bool allowNonZeroNetId)
        {
            if (!this.m_IsServer)
            {
                this.m_IsServer = true;
                if (this.m_LocalPlayerAuthority)
                {
                    this.m_HasAuthority = false;
                }
                else
                {
                    this.m_HasAuthority = true;
                }
                this.m_Observers = new List<NetworkConnection>();
                this.m_ObserverConnections = new HashSet<int>();
                this.CacheBehaviours();
                if (this.netId.IsEmpty())
                {
                    this.m_NetId = GetNextNetworkId();
                }
                else if (!allowNonZeroNetId)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Object has non-zero netId ", this.netId, " for ", base.gameObject }));
                    }
                    return;
                }
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "OnStartServer ", base.gameObject, " GUID:", this.netId }));
                }
                NetworkServer.instance.SetLocalObjectOnServer(this.netId, base.gameObject);
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    try
                    {
                        behaviour.OnStartServer();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Exception in OnStartServer:" + exception.Message + " " + exception.StackTrace);
                    }
                }
                if (NetworkClient.active && NetworkServer.localClientActive)
                {
                    ClientScene.SetLocalObject(this.netId, base.gameObject);
                    this.OnStartClient();
                }
                if (this.m_HasAuthority)
                {
                    this.OnStartAuthority();
                }
            }
        }

        internal void OnStopAuthority()
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.OnStopAuthority();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnStopAuthority:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnUpdateVars(NetworkReader reader, bool initialState)
        {
            if (initialState && (this.m_NetworkBehaviours == null))
            {
                this.m_NetworkBehaviours = base.GetComponents<NetworkBehaviour>();
            }
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                uint position = reader.Position;
                behaviour.OnDeserialize(reader, initialState);
                if ((reader.Position - position) > 1)
                {
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 8, behaviour.GetType().Name, 1);
                }
            }
        }

        private void OnValidate()
        {
            if (this.m_ServerOnly && this.m_LocalPlayerAuthority)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Disabling Local Player Authority for " + base.gameObject + " because it is server-only.");
                }
                this.m_LocalPlayerAuthority = false;
            }
            this.SetupIDs();
        }

        /// <summary>
        /// <para>This causes the set of players that can see this object to be rebuild. The OnRebuildObservers callback function will be invoked on each NetworkBehaviour.</para>
        /// </summary>
        /// <param name="initialize">True if this is the first time.</param>
        public void RebuildObservers(bool initialize)
        {
            if (this.m_Observers != null)
            {
                bool flag = false;
                bool flag2 = false;
                HashSet<NetworkConnection> observers = new HashSet<NetworkConnection>();
                HashSet<NetworkConnection> set2 = new HashSet<NetworkConnection>(this.m_Observers);
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    flag2 |= behaviour.OnRebuildObservers(observers, initialize);
                }
                if (!flag2)
                {
                    if (initialize)
                    {
                        for (int j = 0; j < NetworkServer.connections.Count; j++)
                        {
                            NetworkConnection conn = NetworkServer.connections[j];
                            if ((conn != null) && conn.isReady)
                            {
                                this.AddObserver(conn);
                            }
                        }
                        for (int k = 0; k < NetworkServer.localConnections.Count; k++)
                        {
                            NetworkConnection connection2 = NetworkServer.localConnections[k];
                            if ((connection2 != null) && connection2.isReady)
                            {
                                this.AddObserver(connection2);
                            }
                        }
                    }
                }
                else
                {
                    foreach (NetworkConnection connection3 in observers)
                    {
                        if (connection3 != null)
                        {
                            if (!connection3.isReady)
                            {
                                if (LogFilter.logWarn)
                                {
                                    Debug.LogWarning(string.Concat(new object[] { "Observer is not ready for ", base.gameObject, " ", connection3 }));
                                }
                            }
                            else if (initialize || !set2.Contains(connection3))
                            {
                                connection3.AddToVisList(this);
                                if (LogFilter.logDebug)
                                {
                                    Debug.Log(string.Concat(new object[] { "New Observer for ", base.gameObject, " ", connection3 }));
                                }
                                flag = true;
                            }
                        }
                    }
                    foreach (NetworkConnection connection4 in set2)
                    {
                        if (!observers.Contains(connection4))
                        {
                            connection4.RemoveFromVisList(this, false);
                            if (LogFilter.logDebug)
                            {
                                Debug.Log(string.Concat(new object[] { "Removed Observer for ", base.gameObject, " ", connection4 }));
                            }
                            flag = true;
                        }
                    }
                    if (initialize)
                    {
                        for (int m = 0; m < NetworkServer.localConnections.Count; m++)
                        {
                            if (!observers.Contains(NetworkServer.localConnections[m]))
                            {
                                this.OnSetLocalVisibility(false);
                            }
                        }
                    }
                    if (flag)
                    {
                        this.m_Observers = new List<NetworkConnection>(observers);
                        this.m_ObserverConnections.Clear();
                        for (int n = 0; n < this.m_Observers.Count; n++)
                        {
                            this.m_ObserverConnections.Add(this.m_Observers[n].connectionId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>Removes ownership for an object for a client by its conneciton.</para>
        /// </summary>
        /// <param name="conn">The connection of the client to remove authority for.</param>
        /// <returns>
        /// <para>True if authority is removed.</para>
        /// </returns>
        public bool RemoveClientAuthority(NetworkConnection conn)
        {
            if (!this.isServer)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority can only be call on the server for spawned objects.");
                }
                return false;
            }
            if (this.connectionToClient != null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority cannot remove authority for a player object");
                }
                return false;
            }
            if (this.m_ClientAuthorityOwner == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority for " + base.gameObject + " has no clientAuthority owner.");
                }
                return false;
            }
            if (this.m_ClientAuthorityOwner != conn)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority for " + base.gameObject + " has different owner.");
                }
                return false;
            }
            this.m_ClientAuthorityOwner.RemoveOwnedObject(this);
            this.m_ClientAuthorityOwner = null;
            this.ForceAuthority(true);
            ClientAuthorityMessage msg = new ClientAuthorityMessage {
                netId = this.netId,
                authority = false
            };
            conn.Send(15, msg);
            if (clientAuthorityCallback != null)
            {
                clientAuthorityCallback(conn, this, false);
            }
            return true;
        }

        internal void RemoveObserver(NetworkConnection conn)
        {
            if (this.m_Observers != null)
            {
                this.m_Observers.Remove(conn);
                this.m_ObserverConnections.Remove(conn.connectionId);
                conn.RemoveFromVisList(this, false);
            }
        }

        internal void RemoveObserverInternal(NetworkConnection conn)
        {
            if (this.m_Observers != null)
            {
                this.m_Observers.Remove(conn);
                this.m_ObserverConnections.Remove(conn.connectionId);
            }
        }

        internal void Reset()
        {
            this.m_IsServer = false;
            this.m_IsClient = false;
            this.m_HasAuthority = false;
            this.m_NetId = NetworkInstanceId.Zero;
            this.m_IsLocalPlayer = false;
            this.m_ConnectionToServer = null;
            this.m_ConnectionToClient = null;
            this.m_PlayerId = -1;
            this.m_NetworkBehaviours = null;
            this.ClearObservers();
            this.m_ClientAuthorityOwner = null;
        }

        internal void SetClientOwner(NetworkConnection conn)
        {
            if ((this.m_ClientAuthorityOwner != null) && LogFilter.logError)
            {
                Debug.LogError("SetClientOwner m_ClientAuthorityOwner already set!");
            }
            this.m_ClientAuthorityOwner = conn;
            this.m_ClientAuthorityOwner.AddOwnedObject(this);
        }

        internal void SetConnectionToClient(NetworkConnection conn, short newPlayerControllerId)
        {
            this.m_PlayerId = newPlayerControllerId;
            this.m_ConnectionToClient = conn;
        }

        internal void SetConnectionToServer(NetworkConnection conn)
        {
            this.m_ConnectionToServer = conn;
        }

        internal void SetDynamicAssetId(NetworkHash128 newAssetId)
        {
            if (!this.m_AssetId.IsValid() || this.m_AssetId.Equals(newAssetId))
            {
                this.m_AssetId = newAssetId;
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("SetDynamicAssetId object already has an assetId <" + this.m_AssetId + ">");
            }
        }

        internal void SetLocalPlayer(short localPlayerControllerId)
        {
            this.m_IsLocalPlayer = true;
            this.m_PlayerId = localPlayerControllerId;
            bool hasAuthority = this.m_HasAuthority;
            if (this.localPlayerAuthority)
            {
                this.m_HasAuthority = true;
            }
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                behaviour.OnStartLocalPlayer();
                if (this.localPlayerAuthority && !hasAuthority)
                {
                    behaviour.OnStartAuthority();
                }
            }
        }

        internal void SetNetworkInstanceId(NetworkInstanceId newNetId)
        {
            this.m_NetId = newNetId;
            if (newNetId.Value == 0)
            {
                this.m_IsServer = false;
            }
        }

        internal void SetNotLocalPlayer()
        {
            this.m_IsLocalPlayer = false;
            if (!NetworkServer.active || !NetworkServer.localClientActive)
            {
                this.m_HasAuthority = false;
            }
        }

        private void SetupIDs()
        {
            if (this.ThisIsAPrefab())
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("This is a prefab: " + base.gameObject.name);
                }
                this.AssignAssetID(base.gameObject);
            }
            else
            {
                GameObject obj2;
                if (this.ThisIsASceneObjectWithPrefabParent(out obj2))
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("This is a scene object with prefab link: " + base.gameObject.name);
                    }
                    this.AssignAssetID(obj2);
                }
                else
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("This is a pure scene object: " + base.gameObject.name);
                    }
                    this.m_AssetId.Reset();
                }
            }
        }

        private bool ThisIsAPrefab() => 
            (PrefabUtility.GetPrefabType(base.gameObject) == PrefabType.Prefab);

        private bool ThisIsASceneObjectWithPrefabParent(out GameObject prefab)
        {
            prefab = null;
            if (PrefabUtility.GetPrefabType(base.gameObject) == PrefabType.None)
            {
                return false;
            }
            prefab = (GameObject) PrefabUtility.GetPrefabParent(base.gameObject);
            if (prefab == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Failed to find prefab parent for scene object [name:" + base.gameObject.name + "]");
                }
                return false;
            }
            return true;
        }

        internal static void UNetDomainReload()
        {
            NetworkManager.OnDomainReload();
        }

        internal void UNetSerializeAllVars(NetworkWriter writer)
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                this.m_NetworkBehaviours[i].OnSerialize(writer, true);
            }
        }

        internal static void UNetStaticUpdate()
        {
            NetworkServer.Update();
            NetworkClient.UpdateClients();
            NetworkManager.UpdateScene();
            NetworkDetailStats.NewProfilerTick(Time.time);
        }

        internal void UNetUpdate()
        {
            uint num = 0;
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                int dirtyChannel = this.m_NetworkBehaviours[i].GetDirtyChannel();
                if (dirtyChannel != -1)
                {
                    num |= ((uint) 1) << dirtyChannel;
                }
            }
            if (num != 0)
            {
                for (int j = 0; j < NetworkServer.numChannels; j++)
                {
                    if ((num & (((int) 1) << j)) != 0)
                    {
                        s_UpdateWriter.StartMessage(8);
                        s_UpdateWriter.Write(this.netId);
                        bool flag = false;
                        for (int k = 0; k < this.m_NetworkBehaviours.Length; k++)
                        {
                            short position = s_UpdateWriter.Position;
                            NetworkBehaviour behaviour2 = this.m_NetworkBehaviours[k];
                            if (behaviour2.GetDirtyChannel() != j)
                            {
                                behaviour2.OnSerialize(s_UpdateWriter, false);
                            }
                            else
                            {
                                if (behaviour2.OnSerialize(s_UpdateWriter, false))
                                {
                                    behaviour2.ClearAllDirtyBits();
                                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 8, behaviour2.GetType().Name, 1);
                                    flag = true;
                                }
                                if (((s_UpdateWriter.Position - position) > NetworkServer.maxPacketSize) && LogFilter.logWarn)
                                {
                                    Debug.LogWarning(string.Concat(new object[] { "Large state update of ", s_UpdateWriter.Position - position, " bytes for netId:", this.netId, " from script:", behaviour2 }));
                                }
                            }
                        }
                        if (flag)
                        {
                            s_UpdateWriter.FinishMessage();
                            NetworkServer.SendWriterToReady(base.gameObject, s_UpdateWriter, j);
                        }
                    }
                }
            }
        }

        internal void UpdateClientServer(bool isClientFlag, bool isServerFlag)
        {
            this.m_IsClient |= isClientFlag;
            this.m_IsServer |= isServerFlag;
        }

        /// <summary>
        /// <para>Unique identifier used to find the source assets when server spawns the on clients.</para>
        /// </summary>
        public NetworkHash128 assetId
        {
            get
            {
                if (!this.m_AssetId.IsValid())
                {
                    this.SetupIDs();
                }
                return this.m_AssetId;
            }
        }

        /// <summary>
        /// <para>The client that has authority for this object. This will be null if no client has authority.</para>
        /// </summary>
        public NetworkConnection clientAuthorityOwner =>
            this.m_ClientAuthorityOwner;

        /// <summary>
        /// <para>The connection associated with this NetworkIdentity. This is only valid for player objects on the server.</para>
        /// </summary>
        public NetworkConnection connectionToClient =>
            this.m_ConnectionToClient;

        /// <summary>
        /// <para>The UConnection associated with this NetworkIdentity. This is only valid for player objects on a local client.</para>
        /// </summary>
        public NetworkConnection connectionToServer =>
            this.m_ConnectionToServer;

        /// <summary>
        /// <para>This returns true if this object is the authoritative version of the object in the distributed network application.</para>
        /// </summary>
        public bool hasAuthority =>
            this.m_HasAuthority;

        /// <summary>
        /// <para>Returns true if running as a client and this object was spawned by a server.</para>
        /// </summary>
        public bool isClient =>
            this.m_IsClient;

        /// <summary>
        /// <para>This returns true if this object is the one that represents the player on the local machine.</para>
        /// </summary>
        public bool isLocalPlayer =>
            this.m_IsLocalPlayer;

        /// <summary>
        /// <para>Returns true if running as a server, which spawned the object.</para>
        /// </summary>
        public bool isServer
        {
            get
            {
                if (!this.m_IsServer)
                {
                    return false;
                }
                return (NetworkServer.active && this.m_IsServer);
            }
        }

        /// <summary>
        /// <para>.localPlayerAuthority means that the client of the "owning" player has authority over their own player object.</para>
        /// </summary>
        public bool localPlayerAuthority
        {
            get => 
                this.m_LocalPlayerAuthority;
            set
            {
                this.m_LocalPlayerAuthority = value;
            }
        }

        /// <summary>
        /// <para>Unique identifier for this particular object instance, used for tracking objects between networked clients and the server.</para>
        /// </summary>
        public NetworkInstanceId netId =>
            this.m_NetId;

        /// <summary>
        /// <para>The set of network connections (players) that can see this object.</para>
        /// </summary>
        public ReadOnlyCollection<NetworkConnection> observers
        {
            get
            {
                if (this.m_Observers == null)
                {
                    return null;
                }
                return new ReadOnlyCollection<NetworkConnection>(this.m_Observers);
            }
        }

        /// <summary>
        /// <para>The id of the player associated with this GameObject.</para>
        /// </summary>
        public short playerControllerId =>
            this.m_PlayerId;

        /// <summary>
        /// <para>A unique identifier for NetworkIdentity objects within a scene.</para>
        /// </summary>
        public NetworkSceneId sceneId =>
            this.m_SceneId;

        /// <summary>
        /// <para>Flag to make this object only exist when the game is running as a server (or host).</para>
        /// </summary>
        public bool serverOnly
        {
            get => 
                this.m_ServerOnly;
            set
            {
                this.m_ServerOnly = value;
            }
        }

        /// <summary>
        /// <para>The delegate type for the clientAuthorityCallback.</para>
        /// </summary>
        /// <param name="conn">The network connection that is gaining or losing authority.</param>
        /// <param name="uv">The object whose client authority status is being changed.</param>
        /// <param name="authorityState">The new state of client authority of the object for the connection.</param>
        public delegate void ClientAuthorityCallback(NetworkConnection conn, NetworkIdentity uv, bool authorityState);
    }
}

