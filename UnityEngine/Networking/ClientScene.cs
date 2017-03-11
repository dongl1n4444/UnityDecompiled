namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    /// <summary>
    /// <para>A client manager which contains static client information and functions.</para>
    /// </summary>
    public class ClientScene
    {
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache0;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache1;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache10;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache11;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache12;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache2;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache3;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache4;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache5;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache6;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache7;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache8;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cache9;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cacheA;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cacheB;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cacheC;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cacheD;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cacheE;
        [CompilerGenerated]
        private static NetworkMessageDelegate <>f__mg$cacheF;
        /// <summary>
        /// <para>A constant ID used by the old host when it reconnects to the new host.</para>
        /// </summary>
        public const int ReconnectIdHost = 0;
        /// <summary>
        /// <para>An invalid reconnect Id.</para>
        /// </summary>
        public const int ReconnectIdInvalid = -1;
        private static ClientAuthorityMessage s_ClientAuthorityMessage = new ClientAuthorityMessage();
        private static bool s_IsReady;
        private static bool s_IsSpawnFinished;
        private static List<PlayerController> s_LocalPlayers = new List<PlayerController>();
        private static NetworkScene s_NetworkScene = new NetworkScene();
        private static ObjectDestroyMessage s_ObjectDestroyMessage = new ObjectDestroyMessage();
        private static ObjectSpawnFinishedMessage s_ObjectSpawnFinishedMessage = new ObjectSpawnFinishedMessage();
        private static ObjectSpawnMessage s_ObjectSpawnMessage = new ObjectSpawnMessage();
        private static ObjectSpawnSceneMessage s_ObjectSpawnSceneMessage = new ObjectSpawnSceneMessage();
        private static OwnerMessage s_OwnerMessage = new OwnerMessage();
        private static PeerInfoMessage[] s_Peers;
        private static List<PendingOwner> s_PendingOwnerIds = new List<PendingOwner>();
        private static NetworkConnection s_ReadyConnection;
        private static int s_ReconnectId = -1;
        private static Dictionary<NetworkSceneId, NetworkIdentity> s_SpawnableObjects;

        /// <summary>
        /// <para>This adds a player object for this client. This causes an AddPlayer message to be sent to the server, and NetworkManager.OnServerAddPlayer will be called. If an extra message was passed to AddPlayer, then OnServerAddPlayer will be called with a NetworkReader that contains the contents of the message.</para>
        /// </summary>
        /// <param name="readyConn">The connection to become ready for this client.</param>
        /// <param name="playerControllerId">The local player ID number.</param>
        /// <param name="extraMessage">An extra message object that can be passed to the server for this player.</param>
        /// <returns>
        /// <para>True if player was added.</para>
        /// </returns>
        public static bool AddPlayer(short playerControllerId) => 
            AddPlayer(null, playerControllerId);

        /// <summary>
        /// <para>This adds a player object for this client. This causes an AddPlayer message to be sent to the server, and NetworkManager.OnServerAddPlayer will be called. If an extra message was passed to AddPlayer, then OnServerAddPlayer will be called with a NetworkReader that contains the contents of the message.</para>
        /// </summary>
        /// <param name="readyConn">The connection to become ready for this client.</param>
        /// <param name="playerControllerId">The local player ID number.</param>
        /// <param name="extraMessage">An extra message object that can be passed to the server for this player.</param>
        /// <returns>
        /// <para>True if player was added.</para>
        /// </returns>
        public static bool AddPlayer(NetworkConnection readyConn, short playerControllerId) => 
            AddPlayer(readyConn, playerControllerId, null);

        /// <summary>
        /// <para>This adds a player object for this client. This causes an AddPlayer message to be sent to the server, and NetworkManager.OnServerAddPlayer will be called. If an extra message was passed to AddPlayer, then OnServerAddPlayer will be called with a NetworkReader that contains the contents of the message.</para>
        /// </summary>
        /// <param name="readyConn">The connection to become ready for this client.</param>
        /// <param name="playerControllerId">The local player ID number.</param>
        /// <param name="extraMessage">An extra message object that can be passed to the server for this player.</param>
        /// <returns>
        /// <para>True if player was added.</para>
        /// </returns>
        public static bool AddPlayer(NetworkConnection readyConn, short playerControllerId, MessageBase extraMessage)
        {
            PlayerController controller;
            if (playerControllerId < 0)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientScene::AddPlayer: playerControllerId of " + playerControllerId + " is negative");
                }
                return false;
            }
            if (playerControllerId > 0x20)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "ClientScene::AddPlayer: playerControllerId of ", playerControllerId, " is too high, max is ", 0x20 }));
                }
                return false;
            }
            if ((playerControllerId > 0x10) && LogFilter.logWarn)
            {
                Debug.LogWarning("ClientScene::AddPlayer: playerControllerId of " + playerControllerId + " is unusually high");
            }
            while (playerControllerId >= s_LocalPlayers.Count)
            {
                s_LocalPlayers.Add(new PlayerController());
            }
            if (readyConn == null)
            {
                if (!s_IsReady)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Must call AddPlayer() with a connection the first time to become ready.");
                    }
                    return false;
                }
            }
            else
            {
                s_IsReady = true;
                s_ReadyConnection = readyConn;
            }
            if (s_ReadyConnection.GetPlayerController(playerControllerId, out controller) && (controller.IsValid && (controller.gameObject != null)))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientScene::AddPlayer: playerControllerId of " + playerControllerId + " already in use.");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::AddPlayer() for ID ", playerControllerId, " called with connection [", s_ReadyConnection, "]" }));
            }
            if (!hasMigrationPending())
            {
                AddPlayerMessage msg = new AddPlayerMessage {
                    playerControllerId = playerControllerId
                };
                if (extraMessage != null)
                {
                    NetworkWriter writer = new NetworkWriter();
                    extraMessage.Serialize(writer);
                    msg.msgData = writer.ToArray();
                    msg.msgSize = writer.Position;
                }
                s_ReadyConnection.Send(0x25, msg);
            }
            else
            {
                return SendReconnectMessage(extraMessage);
            }
            return true;
        }

        private static void ApplySpawnPayload(NetworkIdentity uv, Vector3 position, byte[] payload, NetworkInstanceId netId, GameObject newGameObject)
        {
            if (!uv.gameObject.activeSelf)
            {
                uv.gameObject.SetActive(true);
            }
            uv.transform.position = position;
            if ((payload != null) && (payload.Length > 0))
            {
                NetworkReader reader = new NetworkReader(payload);
                uv.OnUpdateVars(reader, true);
            }
            if (newGameObject != null)
            {
                newGameObject.SetActive(true);
                uv.SetNetworkInstanceId(netId);
                SetLocalObject(netId, newGameObject);
                if (s_IsSpawnFinished)
                {
                    uv.OnStartClient();
                    CheckForOwner(uv);
                }
            }
        }

        private static void CheckForOwner(NetworkIdentity uv)
        {
            for (int i = 0; i < s_PendingOwnerIds.Count; i++)
            {
                PendingOwner owner = s_PendingOwnerIds[i];
                if (owner.netId == uv.netId)
                {
                    uv.SetConnectionToServer(s_ReadyConnection);
                    uv.SetLocalPlayer(owner.playerControllerId);
                    if (LogFilter.logDev)
                    {
                        Debug.Log("ClientScene::OnOwnerMessage - player=" + uv.gameObject.name);
                    }
                    if (s_ReadyConnection.connectionId < 0)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError("Owner message received on a local client.");
                        }
                    }
                    else
                    {
                        InternalAddPlayer(uv, owner.playerControllerId);
                        s_PendingOwnerIds.RemoveAt(i);
                    }
                    break;
                }
            }
        }

        internal static void ClearLocalPlayers()
        {
            s_LocalPlayers.Clear();
        }

        /// <summary>
        /// <para>This clears the registered spawn prefabs and spawn handler functions for this client.</para>
        /// </summary>
        public static void ClearSpawners()
        {
            NetworkScene.ClearSpawners();
        }

        /// <summary>
        /// <para>Create and connect a local client instance to the local server. This makes the client into a "host" - a client and server in the same process.</para>
        /// </summary>
        /// <returns>
        /// <para>A client object for communicating with the local server.</para>
        /// </returns>
        public static NetworkClient ConnectLocalServer()
        {
            LocalClient client = new LocalClient();
            NetworkServer.instance.ActivateLocalClientScene();
            client.InternalConnectLocalServer(true);
            return client;
        }

        /// <summary>
        /// <para>Destroys all networked objects on the client.</para>
        /// </summary>
        public static void DestroyAllClientObjects()
        {
            s_NetworkScene.DestroyAllClientObjects();
        }

        /// <summary>
        /// <para>This finds the local NetworkIdentity object with the specified network Id.</para>
        /// </summary>
        /// <param name="netId">The id of the networked object.</param>
        /// <returns>
        /// <para>The game object that matches the netId.</para>
        /// </returns>
        public static GameObject FindLocalObject(NetworkInstanceId netId) => 
            s_NetworkScene.FindLocalObject(netId);

        internal static bool GetPlayerController(short playerControllerId, out PlayerController player)
        {
            player = null;
            if (playerControllerId >= localPlayers.Count)
            {
                if (LogFilter.logWarn)
                {
                    Debug.Log("ClientScene::GetPlayer: no local player found for: " + playerControllerId);
                }
                return false;
            }
            if (localPlayers[playerControllerId] == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ClientScene::GetPlayer: local player is null for: " + playerControllerId);
                }
                return false;
            }
            player = localPlayers[playerControllerId];
            return (player.gameObject != null);
        }

        internal static string GetStringForAssetId(NetworkHash128 assetId)
        {
            GameObject obj2;
            SpawnDelegate delegate2;
            if (NetworkScene.GetPrefab(assetId, out obj2))
            {
                return obj2.name;
            }
            if (NetworkScene.GetSpawnHandler(assetId, out delegate2))
            {
                return delegate2.GetMethodName();
            }
            return "unknown";
        }

        internal static void HandleClientDisconnect(NetworkConnection conn)
        {
            if ((s_ReadyConnection == conn) && s_IsReady)
            {
                s_IsReady = false;
                s_ReadyConnection = null;
            }
        }

        private static bool hasMigrationPending() => 
            (s_ReconnectId != -1);

        internal static void InternalAddPlayer(NetworkIdentity view, short playerControllerId)
        {
            if (LogFilter.logDebug)
            {
                Debug.LogWarning("ClientScene::InternalAddPlayer: playerControllerId : " + playerControllerId);
            }
            if (playerControllerId >= s_LocalPlayers.Count)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ClientScene::InternalAddPlayer: playerControllerId higher than expected: " + playerControllerId);
                }
                while (playerControllerId >= s_LocalPlayers.Count)
                {
                    s_LocalPlayers.Add(new PlayerController());
                }
            }
            PlayerController player = new PlayerController {
                gameObject = view.gameObject,
                playerControllerId = playerControllerId,
                unetView = view
            };
            s_LocalPlayers[playerControllerId] = player;
            s_ReadyConnection.SetPlayerController(player);
        }

        private static void OnClientAuthority(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ClientAuthorityMessage>(s_ClientAuthorityMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnClientAuthority for  connectionId=", netMsg.conn.connectionId, " netId: ", s_ClientAuthorityMessage.netId }));
            }
            if (s_NetworkScene.GetNetworkIdentity(s_ClientAuthorityMessage.netId, out identity))
            {
                identity.HandleClientAuthority(s_ClientAuthorityMessage.authority);
            }
        }

        private static void OnLocalClientObjectDestroy(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<ObjectDestroyMessage>(s_ObjectDestroyMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnLocalObjectObjDestroy netId:" + s_ObjectDestroyMessage.netId);
            }
            s_NetworkScene.RemoveLocalObject(s_ObjectDestroyMessage.netId);
        }

        private static void OnLocalClientObjectHide(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectDestroyMessage>(s_ObjectDestroyMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnLocalObjectObjHide netId:" + s_ObjectDestroyMessage.netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectDestroyMessage.netId, out identity))
            {
                identity.OnSetLocalVisibility(false);
            }
        }

        private static void OnLocalClientObjectSpawn(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectSpawnMessage>(s_ObjectSpawnMessage);
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnMessage.netId, out identity))
            {
                identity.OnSetLocalVisibility(true);
            }
        }

        private static void OnLocalClientObjectSpawnScene(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectSpawnSceneMessage>(s_ObjectSpawnSceneMessage);
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnSceneMessage.netId, out identity))
            {
                identity.OnSetLocalVisibility(true);
            }
        }

        private static void OnObjectDestroy(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectDestroyMessage>(s_ObjectDestroyMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnObjDestroy netId:" + s_ObjectDestroyMessage.netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectDestroyMessage.netId, out identity))
            {
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 1, GetStringForAssetId(identity.assetId), 1);
                identity.OnNetworkDestroy();
                if (!NetworkScene.InvokeUnSpawnHandler(identity.assetId, identity.gameObject))
                {
                    if (identity.sceneId.IsEmpty())
                    {
                        UnityEngine.Object.Destroy(identity.gameObject);
                    }
                    else
                    {
                        identity.gameObject.SetActive(false);
                        s_SpawnableObjects[identity.sceneId] = identity;
                    }
                }
                s_NetworkScene.RemoveLocalObject(s_ObjectDestroyMessage.netId);
                identity.MarkForReset();
            }
            else if (LogFilter.logDebug)
            {
                Debug.LogWarning("Did not find target for destroy message for " + s_ObjectDestroyMessage.netId);
            }
        }

        private static void OnObjectSpawn(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<ObjectSpawnMessage>(s_ObjectSpawnMessage);
            if (!s_ObjectSpawnMessage.assetId.IsValid())
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("OnObjSpawn netId: " + s_ObjectSpawnMessage.netId + " has invalid asset Id");
                }
            }
            else
            {
                NetworkIdentity component;
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Client spawn handler instantiating [netId:", s_ObjectSpawnMessage.netId, " asset ID:", s_ObjectSpawnMessage.assetId, " pos:", s_ObjectSpawnMessage.position, "]" }));
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 3, GetStringForAssetId(s_ObjectSpawnMessage.assetId), 1);
                if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnMessage.netId, out component))
                {
                    ApplySpawnPayload(component, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.payload, s_ObjectSpawnMessage.netId, null);
                }
                else
                {
                    GameObject obj2;
                    if (NetworkScene.GetPrefab(s_ObjectSpawnMessage.assetId, out obj2))
                    {
                        GameObject newGameObject = UnityEngine.Object.Instantiate<GameObject>(obj2, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.rotation);
                        if (LogFilter.logDebug)
                        {
                            Debug.Log(string.Concat(new object[] { "Client spawn handler instantiating [netId:", s_ObjectSpawnMessage.netId, " asset ID:", s_ObjectSpawnMessage.assetId, " pos:", s_ObjectSpawnMessage.position, " rotation: ", s_ObjectSpawnMessage.rotation, "]" }));
                        }
                        component = newGameObject.GetComponent<NetworkIdentity>();
                        if (component == null)
                        {
                            if (LogFilter.logError)
                            {
                                Debug.LogError("Client object spawned for " + s_ObjectSpawnMessage.assetId + " does not have a NetworkIdentity");
                            }
                        }
                        else
                        {
                            component.Reset();
                            ApplySpawnPayload(component, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.payload, s_ObjectSpawnMessage.netId, newGameObject);
                        }
                    }
                    else
                    {
                        SpawnDelegate delegate2;
                        if (NetworkScene.GetSpawnHandler(s_ObjectSpawnMessage.assetId, out delegate2))
                        {
                            GameObject obj4 = delegate2(s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.assetId);
                            if (obj4 == null)
                            {
                                if (LogFilter.logWarn)
                                {
                                    Debug.LogWarning("Client spawn handler for " + s_ObjectSpawnMessage.assetId + " returned null");
                                }
                            }
                            else
                            {
                                component = obj4.GetComponent<NetworkIdentity>();
                                if (component == null)
                                {
                                    if (LogFilter.logError)
                                    {
                                        Debug.LogError("Client object spawned for " + s_ObjectSpawnMessage.assetId + " does not have a network identity");
                                    }
                                }
                                else
                                {
                                    component.Reset();
                                    component.SetDynamicAssetId(s_ObjectSpawnMessage.assetId);
                                    ApplySpawnPayload(component, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.payload, s_ObjectSpawnMessage.netId, obj4);
                                }
                            }
                        }
                        else if (LogFilter.logError)
                        {
                            Debug.LogError(string.Concat(new object[] { "Failed to spawn server object, did you forget to add it to the NetworkManager? assetId=", s_ObjectSpawnMessage.assetId, " netId=", s_ObjectSpawnMessage.netId }));
                        }
                    }
                }
            }
        }

        private static void OnObjectSpawnFinished(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<ObjectSpawnFinishedMessage>(s_ObjectSpawnFinishedMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("SpawnFinished:" + s_ObjectSpawnFinishedMessage.state);
            }
            if (s_ObjectSpawnFinishedMessage.state == 0)
            {
                PrepareToSpawnSceneObjects();
                s_IsSpawnFinished = false;
            }
            else
            {
                foreach (NetworkIdentity identity in objects.Values)
                {
                    if (!identity.isClient)
                    {
                        identity.OnStartClient();
                        CheckForOwner(identity);
                    }
                }
                s_IsSpawnFinished = true;
            }
        }

        private static void OnObjectSpawnScene(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectSpawnSceneMessage>(s_ObjectSpawnSceneMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "Client spawn scene handler instantiating [netId:", s_ObjectSpawnSceneMessage.netId, " sceneId:", s_ObjectSpawnSceneMessage.sceneId, " pos:", s_ObjectSpawnSceneMessage.position }));
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 10, "sceneId", 1);
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnSceneMessage.netId, out identity))
            {
                ApplySpawnPayload(identity, s_ObjectSpawnSceneMessage.position, s_ObjectSpawnSceneMessage.payload, s_ObjectSpawnSceneMessage.netId, identity.gameObject);
            }
            else
            {
                NetworkIdentity uv = SpawnSceneObject(s_ObjectSpawnSceneMessage.sceneId);
                if (uv == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Spawn scene object not found for " + s_ObjectSpawnSceneMessage.sceneId);
                    }
                }
                else
                {
                    if (LogFilter.logDebug)
                    {
                        Debug.Log(string.Concat(new object[] { "Client spawn for [netId:", s_ObjectSpawnSceneMessage.netId, "] [sceneId:", s_ObjectSpawnSceneMessage.sceneId, "] obj:", uv.gameObject.name }));
                    }
                    ApplySpawnPayload(uv, s_ObjectSpawnSceneMessage.position, s_ObjectSpawnSceneMessage.payload, s_ObjectSpawnSceneMessage.netId, uv.gameObject);
                }
            }
        }

        private static void OnOwnerMessage(NetworkMessage netMsg)
        {
            PlayerController controller;
            NetworkIdentity identity;
            netMsg.ReadMessage<OwnerMessage>(s_OwnerMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnOwnerMessage - connectionId=", netMsg.conn.connectionId, " netId: ", s_OwnerMessage.netId }));
            }
            if (netMsg.conn.GetPlayerController(s_OwnerMessage.playerControllerId, out controller))
            {
                controller.unetView.SetNotLocalPlayer();
            }
            if (s_NetworkScene.GetNetworkIdentity(s_OwnerMessage.netId, out identity))
            {
                identity.SetConnectionToServer(netMsg.conn);
                identity.SetLocalPlayer(s_OwnerMessage.playerControllerId);
                InternalAddPlayer(identity, s_OwnerMessage.playerControllerId);
            }
            else
            {
                PendingOwner item = new PendingOwner {
                    netId = s_OwnerMessage.netId,
                    playerControllerId = s_OwnerMessage.playerControllerId
                };
                s_PendingOwnerIds.Add(item);
            }
        }

        private static void OnRPCMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnRPCMessage hash:", cmdHash, " netId:", netId }));
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.HandleRPC(cmdHash, netMsg.reader);
            }
            else if (LogFilter.logWarn)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                object[] args = new object[] { netId, cmdHashHandlerName };
                Debug.LogWarningFormat("Could not find target object with netId:{0} for RPC call {1}", args);
            }
        }

        private static void OnSyncEventMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnSyncEventMessage " + netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.HandleSyncEvent(cmdHash, netMsg.reader);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for SyncEvent message for " + netId);
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 7, NetworkBehaviour.GetCmdHashHandlerName(cmdHash), 1);
        }

        private static void OnSyncListMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnSyncListMessage " + netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.HandleSyncList(cmdHash, netMsg.reader);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for SyncList message for " + netId);
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 9, NetworkBehaviour.GetCmdHashHandlerName(cmdHash), 1);
        }

        private static void OnUpdateVarsMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnUpdateVarsMessage ", netId, " channel:", netMsg.channelId }));
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.OnUpdateVars(netMsg.reader, false);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for sync message for " + netId);
            }
        }

        internal static void PrepareToSpawnSceneObjects()
        {
            s_SpawnableObjects = new Dictionary<NetworkSceneId, NetworkIdentity>();
            foreach (NetworkIdentity identity in UnityEngine.Resources.FindObjectsOfTypeAll<NetworkIdentity>())
            {
                if ((!identity.gameObject.activeSelf && (identity.gameObject.hideFlags != HideFlags.NotEditable)) && ((identity.gameObject.hideFlags != HideFlags.HideAndDontSave) && !identity.sceneId.IsEmpty()))
                {
                    s_SpawnableObjects[identity.sceneId] = identity;
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("ClientScene::PrepareSpawnObjects sceneId:" + identity.sceneId);
                    }
                }
            }
        }

        /// <summary>
        /// <para>Signal that the client connection is ready to enter the game.</para>
        /// </summary>
        /// <param name="conn">The client connection which is ready.</param>
        public static bool Ready(NetworkConnection conn)
        {
            if (s_IsReady)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("A connection has already been set as ready. There can only be one.");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::Ready() called with connection [" + conn + "]");
            }
            if (conn != null)
            {
                ReadyMessage msg = new ReadyMessage();
                conn.Send(0x23, msg);
                s_IsReady = true;
                s_ReadyConnection = conn;
                s_ReadyConnection.isReady = true;
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Ready() called with invalid connection object: conn=null");
            }
            return false;
        }

        internal static NetworkClient ReconnectLocalServer()
        {
            LocalClient client = new LocalClient();
            NetworkServer.instance.ActivateLocalClientScene();
            client.InternalConnectLocalServer(false);
            return client;
        }

        /// <summary>
        /// <para>Registers a prefab with the UNET spawning system.</para>
        /// </summary>
        /// <param name="prefab">A Prefab that will be spawned.</param>
        /// <param name="spawnHandler">A method to use as a custom spawnhandler on clients.</param>
        /// <param name="unspawnHandler">A method to use as a custom un-spawnhandler on clients.</param>
        /// <param name="newAssetId">An assetId to be assigned to this prefab. This allows a dynamically created game object to be registered for an already known asset Id.</param>
        public static void RegisterPrefab(GameObject prefab)
        {
            NetworkScene.RegisterPrefab(prefab);
        }

        /// <summary>
        /// <para>Registers a prefab with the UNET spawning system.</para>
        /// </summary>
        /// <param name="prefab">A Prefab that will be spawned.</param>
        /// <param name="spawnHandler">A method to use as a custom spawnhandler on clients.</param>
        /// <param name="unspawnHandler">A method to use as a custom un-spawnhandler on clients.</param>
        /// <param name="newAssetId">An assetId to be assigned to this prefab. This allows a dynamically created game object to be registered for an already known asset Id.</param>
        public static void RegisterPrefab(GameObject prefab, NetworkHash128 newAssetId)
        {
            NetworkScene.RegisterPrefab(prefab, newAssetId);
        }

        /// <summary>
        /// <para>Registers a prefab with the UNET spawning system.</para>
        /// </summary>
        /// <param name="prefab">A Prefab that will be spawned.</param>
        /// <param name="spawnHandler">A method to use as a custom spawnhandler on clients.</param>
        /// <param name="unspawnHandler">A method to use as a custom un-spawnhandler on clients.</param>
        /// <param name="newAssetId">An assetId to be assigned to this prefab. This allows a dynamically created game object to be registered for an already known asset Id.</param>
        public static void RegisterPrefab(GameObject prefab, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
        {
            NetworkScene.RegisterPrefab(prefab, spawnHandler, unspawnHandler);
        }

        /// <summary>
        /// <para>This is an advanced spawning funciotn that registers a custom assetId with the UNET spawning system.</para>
        /// </summary>
        /// <param name="assetId">Custom assetId string.</param>
        /// <param name="spawnHandler">A method to use as a custom spawnhandler on clients.</param>
        /// <param name="unspawnHandler">A method to use as a custom un-spawnhandler on clients.</param>
        public static void RegisterSpawnHandler(NetworkHash128 assetId, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
        {
            NetworkScene.RegisterSpawnHandler(assetId, spawnHandler, unspawnHandler);
        }

        internal static void RegisterSystemHandlers(NetworkClient client, bool localClient)
        {
            if (localClient)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new NetworkMessageDelegate(ClientScene.OnLocalClientObjectDestroy);
                }
                client.RegisterHandlerSafe(1, <>f__mg$cache0);
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new NetworkMessageDelegate(ClientScene.OnLocalClientObjectHide);
                }
                client.RegisterHandlerSafe(13, <>f__mg$cache1);
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new NetworkMessageDelegate(ClientScene.OnLocalClientObjectSpawn);
                }
                client.RegisterHandlerSafe(3, <>f__mg$cache2);
                if (<>f__mg$cache3 == null)
                {
                    <>f__mg$cache3 = new NetworkMessageDelegate(ClientScene.OnLocalClientObjectSpawnScene);
                }
                client.RegisterHandlerSafe(10, <>f__mg$cache3);
                if (<>f__mg$cache4 == null)
                {
                    <>f__mg$cache4 = new NetworkMessageDelegate(ClientScene.OnClientAuthority);
                }
                client.RegisterHandlerSafe(15, <>f__mg$cache4);
            }
            else
            {
                if (<>f__mg$cache5 == null)
                {
                    <>f__mg$cache5 = new NetworkMessageDelegate(ClientScene.OnObjectSpawn);
                }
                client.RegisterHandlerSafe(3, <>f__mg$cache5);
                if (<>f__mg$cache6 == null)
                {
                    <>f__mg$cache6 = new NetworkMessageDelegate(ClientScene.OnObjectSpawnScene);
                }
                client.RegisterHandlerSafe(10, <>f__mg$cache6);
                if (<>f__mg$cache7 == null)
                {
                    <>f__mg$cache7 = new NetworkMessageDelegate(ClientScene.OnObjectSpawnFinished);
                }
                client.RegisterHandlerSafe(12, <>f__mg$cache7);
                if (<>f__mg$cache8 == null)
                {
                    <>f__mg$cache8 = new NetworkMessageDelegate(ClientScene.OnObjectDestroy);
                }
                client.RegisterHandlerSafe(1, <>f__mg$cache8);
                if (<>f__mg$cache9 == null)
                {
                    <>f__mg$cache9 = new NetworkMessageDelegate(ClientScene.OnObjectDestroy);
                }
                client.RegisterHandlerSafe(13, <>f__mg$cache9);
                if (<>f__mg$cacheA == null)
                {
                    <>f__mg$cacheA = new NetworkMessageDelegate(ClientScene.OnUpdateVarsMessage);
                }
                client.RegisterHandlerSafe(8, <>f__mg$cacheA);
                if (<>f__mg$cacheB == null)
                {
                    <>f__mg$cacheB = new NetworkMessageDelegate(ClientScene.OnOwnerMessage);
                }
                client.RegisterHandlerSafe(4, <>f__mg$cacheB);
                if (<>f__mg$cacheC == null)
                {
                    <>f__mg$cacheC = new NetworkMessageDelegate(ClientScene.OnSyncListMessage);
                }
                client.RegisterHandlerSafe(9, <>f__mg$cacheC);
                if (<>f__mg$cacheD == null)
                {
                    <>f__mg$cacheD = new NetworkMessageDelegate(NetworkAnimator.OnAnimationClientMessage);
                }
                client.RegisterHandlerSafe(40, <>f__mg$cacheD);
                if (<>f__mg$cacheE == null)
                {
                    <>f__mg$cacheE = new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersClientMessage);
                }
                client.RegisterHandlerSafe(0x29, <>f__mg$cacheE);
                if (<>f__mg$cacheF == null)
                {
                    <>f__mg$cacheF = new NetworkMessageDelegate(ClientScene.OnClientAuthority);
                }
                client.RegisterHandlerSafe(15, <>f__mg$cacheF);
            }
            if (<>f__mg$cache10 == null)
            {
                <>f__mg$cache10 = new NetworkMessageDelegate(ClientScene.OnRPCMessage);
            }
            client.RegisterHandlerSafe(2, <>f__mg$cache10);
            if (<>f__mg$cache11 == null)
            {
                <>f__mg$cache11 = new NetworkMessageDelegate(ClientScene.OnSyncEventMessage);
            }
            client.RegisterHandlerSafe(7, <>f__mg$cache11);
            if (<>f__mg$cache12 == null)
            {
                <>f__mg$cache12 = new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerClientMessage);
            }
            client.RegisterHandlerSafe(0x2a, <>f__mg$cache12);
        }

        /// <summary>
        /// <para>Remove the specified player ID from the game.</para>
        /// </summary>
        /// <param name="playerControllerId">The local playerControllerId number to be removed.</param>
        /// <returns>
        /// <para>Returns true if the player was successfully destoyed and removed.</para>
        /// </returns>
        public static bool RemovePlayer(short playerControllerId)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::RemovePlayer() for ID ", playerControllerId, " called with connection [", s_ReadyConnection, "]" }));
            }
            if (s_ReadyConnection.GetPlayerController(playerControllerId, out controller))
            {
                RemovePlayerMessage msg = new RemovePlayerMessage {
                    playerControllerId = playerControllerId
                };
                s_ReadyConnection.Send(0x26, msg);
                s_ReadyConnection.RemovePlayerController(playerControllerId);
                s_LocalPlayers[playerControllerId] = new PlayerController();
                UnityEngine.Object.Destroy(controller.gameObject);
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Failed to find player ID " + playerControllerId);
            }
            return false;
        }

        /// <summary>
        /// <para>Send a reconnect message to the new host, used during host migration.</para>
        /// </summary>
        /// <param name="extraMessage">Any extra data to send.</param>
        /// <returns>
        /// <para>Returns true if the send succeeded.</para>
        /// </returns>
        public static bool SendReconnectMessage(MessageBase extraMessage)
        {
            if (!hasMigrationPending())
            {
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::AddPlayer reconnect " + s_ReconnectId);
            }
            if (s_Peers == null)
            {
                SetReconnectId(-1, null);
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientScene::AddPlayer: reconnecting, but no peers.");
                }
                return false;
            }
            for (int i = 0; i < s_Peers.Length; i++)
            {
                PeerInfoMessage message = s_Peers[i];
                if ((message.playerIds != null) && (message.connectionId == s_ReconnectId))
                {
                    for (int j = 0; j < message.playerIds.Length; j++)
                    {
                        ReconnectMessage msg = new ReconnectMessage {
                            oldConnectionId = s_ReconnectId,
                            netId = message.playerIds[j].netId,
                            playerControllerId = message.playerIds[j].playerControllerId
                        };
                        if (extraMessage != null)
                        {
                            NetworkWriter writer = new NetworkWriter();
                            extraMessage.Serialize(writer);
                            msg.msgData = writer.ToArray();
                            msg.msgSize = writer.Position;
                        }
                        s_ReadyConnection.Send(0x2f, msg);
                    }
                }
            }
            SetReconnectId(-1, null);
            return true;
        }

        public static void SetLocalObject(NetworkInstanceId netId, GameObject obj)
        {
            s_NetworkScene.SetLocalObject(netId, obj, s_IsSpawnFinished, false);
        }

        internal static void SetNotReady()
        {
            s_IsReady = false;
        }

        /// <summary>
        /// <para>Sets the Id that the ClientScene will use when reconnecting to a new host after host migration.</para>
        /// </summary>
        /// <param name="newReconnectId">The Id to use when reconnecting to a game.</param>
        /// <param name="peers">The set of known peers in the game. This may be null.</param>
        public static void SetReconnectId(int newReconnectId, PeerInfoMessage[] peers)
        {
            s_ReconnectId = newReconnectId;
            s_Peers = peers;
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::SetReconnectId: " + newReconnectId);
            }
        }

        internal static void Shutdown()
        {
            s_NetworkScene.Shutdown();
            s_LocalPlayers = new List<PlayerController>();
            s_PendingOwnerIds = new List<PendingOwner>();
            s_SpawnableObjects = null;
            s_ReadyConnection = null;
            s_IsReady = false;
            s_IsSpawnFinished = false;
            s_ReconnectId = -1;
            NetworkTransport.Shutdown();
            NetworkTransport.Init();
        }

        internal static NetworkIdentity SpawnSceneObject(NetworkSceneId sceneId)
        {
            if (s_SpawnableObjects.ContainsKey(sceneId))
            {
                NetworkIdentity identity = s_SpawnableObjects[sceneId];
                s_SpawnableObjects.Remove(sceneId);
                return identity;
            }
            return null;
        }

        /// <summary>
        /// <para>Removes a registered spawn prefab that was setup with ClientScene.RegisterPrefab.</para>
        /// </summary>
        /// <param name="prefab">The prefab to be removed from registration.</param>
        public static void UnregisterPrefab(GameObject prefab)
        {
            NetworkScene.UnregisterPrefab(prefab);
        }

        /// <summary>
        /// <para>Removes a registered spawn handler function that was registered with ClientScene.RegisterHandler().</para>
        /// </summary>
        /// <param name="assetId">The assetId for the handler to be removed for.</param>
        public static void UnregisterSpawnHandler(NetworkHash128 assetId)
        {
            NetworkScene.UnregisterSpawnHandler(assetId);
        }

        /// <summary>
        /// <para>A list of all players added to the game.</para>
        /// </summary>
        public static List<PlayerController> localPlayers =>
            s_LocalPlayers;

        /// <summary>
        /// <para>This is a dictionary of networked objects that have been spawned on the client.</para>
        /// </summary>
        public static Dictionary<NetworkInstanceId, NetworkIdentity> objects =>
            s_NetworkScene.localObjects;

        /// <summary>
        /// <para>This is a dictionary of the prefabs that are registered on the client with ClientScene.RegisterPrefab().</para>
        /// </summary>
        public static Dictionary<NetworkHash128, GameObject> prefabs =>
            NetworkScene.guidToPrefab;

        /// <summary>
        /// <para>Returns true when a client's connection has been set to ready.</para>
        /// </summary>
        public static bool ready =>
            s_IsReady;

        /// <summary>
        /// <para>The NetworkConnection object that is currently "ready". This is the connection to the server where objects are spawned from.</para>
        /// </summary>
        public static NetworkConnection readyConnection =>
            s_ReadyConnection;

        /// <summary>
        /// <para>The reconnectId to use when a client reconnects to the new host of a game after the old host was lost.</para>
        /// </summary>
        public static int reconnectId =>
            s_ReconnectId;

        /// <summary>
        /// <para>This is dictionary of the disabled NetworkIdentity objects in the scene that could be spawned by messages from the server.</para>
        /// </summary>
        public static Dictionary<NetworkSceneId, NetworkIdentity> spawnableObjects =>
            s_SpawnableObjects;

        [StructLayout(LayoutKind.Sequential)]
        private struct PendingOwner
        {
            public NetworkInstanceId netId;
            public short playerControllerId;
        }
    }
}

