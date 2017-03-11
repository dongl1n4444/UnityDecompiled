namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>Base class which should be inherited by scripts which contain networking functionality.</para>
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity)), AddComponentMenu("")]
    public class NetworkBehaviour : MonoBehaviour
    {
        private const float k_DefaultSendInterval = 0.1f;
        private float m_LastSendTime;
        private NetworkIdentity m_MyView;
        private uint m_SyncVarDirtyBits;
        private bool m_SyncVarGuard;
        private static Dictionary<int, Invoker> s_CmdHandlerDelegates = new Dictionary<int, Invoker>();

        /// <summary>
        /// <para>This clears all the dirty bits that were set on this script by SetDirtyBits();</para>
        /// </summary>
        public void ClearAllDirtyBits()
        {
            this.m_LastSendTime = Time.time;
            this.m_SyncVarDirtyBits = 0;
        }

        internal bool ContainsCommandDelegate(int cmdHash) => 
            s_CmdHandlerDelegates.ContainsKey(cmdHash);

        internal static void DumpInvokers()
        {
            Debug.Log("DumpInvokers size:" + s_CmdHandlerDelegates.Count);
            foreach (KeyValuePair<int, Invoker> pair in s_CmdHandlerDelegates)
            {
                Debug.Log(string.Concat(new object[] { "  Invoker:", pair.Value.invokeClass, ":", pair.Value.invokeFunction.GetMethodName(), " ", pair.Value.invokeType, " ", pair.Key }));
            }
        }

        internal static string GetCmdHashCmdName(int cmdHash) => 
            GetCmdHashPrefixName(cmdHash, "InvokeCmd");

        internal static string GetCmdHashEventName(int cmdHash) => 
            GetCmdHashPrefixName(cmdHash, "InvokeSyncEvent");

        internal static string GetCmdHashHandlerName(int cmdHash)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return cmdHash.ToString();
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            return (invoker.invokeType + ":" + invoker.invokeFunction.GetMethodName());
        }

        internal static string GetCmdHashListName(int cmdHash) => 
            GetCmdHashPrefixName(cmdHash, "InvokeSyncList");

        private static string GetCmdHashPrefixName(int cmdHash, string prefix)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return cmdHash.ToString();
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            string methodName = invoker.invokeFunction.GetMethodName();
            if (methodName.IndexOf(prefix) > -1)
            {
                methodName = methodName.Substring(prefix.Length);
            }
            return methodName;
        }

        internal static string GetCmdHashRpcName(int cmdHash) => 
            GetCmdHashPrefixName(cmdHash, "InvokeRpc");

        internal int GetDirtyChannel()
        {
            if (((Time.time - this.m_LastSendTime) > this.GetNetworkSendInterval()) && (this.m_SyncVarDirtyBits != 0))
            {
                return this.GetNetworkChannel();
            }
            return -1;
        }

        internal static string GetInvoker(int cmdHash)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return null;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            return invoker.DebugString();
        }

        private static bool GetInvokerForHash(int cmdHash, UNetInvokeType invokeType, out System.Type invokeClass, out CmdDelegate invokeFunction)
        {
            Invoker invoker = null;
            if (!s_CmdHandlerDelegates.TryGetValue(cmdHash, out invoker))
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("GetInvokerForHash hash:" + cmdHash + " not found");
                }
                invokeClass = null;
                invokeFunction = null;
                return false;
            }
            if (invoker == null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("GetInvokerForHash hash:" + cmdHash + " invoker null");
                }
                invokeClass = null;
                invokeFunction = null;
                return false;
            }
            if (invoker.invokeType != invokeType)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("GetInvokerForHash hash:" + cmdHash + " mismatched invokeType");
                }
                invokeClass = null;
                invokeFunction = null;
                return false;
            }
            invokeClass = invoker.invokeClass;
            invokeFunction = invoker.invokeFunction;
            return true;
        }

        internal static bool GetInvokerForHashClientRpc(int cmdHash, out System.Type invokeClass, out CmdDelegate invokeFunction) => 
            GetInvokerForHash(cmdHash, UNetInvokeType.ClientRpc, out invokeClass, out invokeFunction);

        internal static bool GetInvokerForHashCommand(int cmdHash, out System.Type invokeClass, out CmdDelegate invokeFunction) => 
            GetInvokerForHash(cmdHash, UNetInvokeType.Command, out invokeClass, out invokeFunction);

        internal static bool GetInvokerForHashSyncEvent(int cmdHash, out System.Type invokeClass, out CmdDelegate invokeFunction) => 
            GetInvokerForHash(cmdHash, UNetInvokeType.SyncEvent, out invokeClass, out invokeFunction);

        internal static bool GetInvokerForHashSyncList(int cmdHash, out System.Type invokeClass, out CmdDelegate invokeFunction) => 
            GetInvokerForHash(cmdHash, UNetInvokeType.SyncList, out invokeClass, out invokeFunction);

        /// <summary>
        /// <para>This virtual function is used to specify the QoS channel to use for SyncVar updates for this script.</para>
        /// </summary>
        /// <returns>
        /// <para>The QoS channel for this script.</para>
        /// </returns>
        public virtual int GetNetworkChannel() => 
            0;

        /// <summary>
        /// <para>This virtual function is used to specify the send interval to use for SyncVar updates for this script.</para>
        /// </summary>
        /// <returns>
        /// <para>The time in seconds between updates.</para>
        /// </returns>
        public virtual float GetNetworkSendInterval() => 
            0.1f;

        /// <summary>
        /// <para>Manually invoke a Command.</para>
        /// </summary>
        /// <param name="cmdHash">Hash of the Command name.</param>
        /// <param name="reader">Parameters to pass to the command.</param>
        /// <returns>
        /// <para>Returns true if successful.</para>
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeCommand(int cmdHash, NetworkReader reader) => 
            this.InvokeCommandDelegate(cmdHash, reader);

        internal bool InvokeCommandDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.Command)
            {
                return false;
            }
            if ((base.GetType() != invoker.invokeClass) && !base.GetType().IsSubclassOf(invoker.invokeClass))
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        /// <summary>
        /// <para>Manually invoke an RPC function.</para>
        /// </summary>
        /// <param name="cmdHash">Hash of the RPC name.</param>
        /// <param name="reader">Parameters to pass to the RPC function.</param>
        /// <returns>
        /// <para>Returns true if successful.</para>
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeRPC(int cmdHash, NetworkReader reader) => 
            this.InvokeRpcDelegate(cmdHash, reader);

        internal bool InvokeRpcDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.ClientRpc)
            {
                return false;
            }
            if ((base.GetType() != invoker.invokeClass) && !base.GetType().IsSubclassOf(invoker.invokeClass))
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        /// <summary>
        /// <para>Manually invoke a SyncEvent.</para>
        /// </summary>
        /// <param name="cmdHash">Hash of the SyncEvent name.</param>
        /// <param name="reader">Parameters to pass to the SyncEvent.</param>
        /// <returns>
        /// <para>Returns true if successful.</para>
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeSyncEvent(int cmdHash, NetworkReader reader) => 
            this.InvokeSyncEventDelegate(cmdHash, reader);

        internal bool InvokeSyncEventDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.SyncEvent)
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeSyncList(int cmdHash, NetworkReader reader) => 
            this.InvokeSyncListDelegate(cmdHash, reader);

        internal bool InvokeSyncListDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.SyncList)
            {
                return false;
            }
            if (base.GetType() != invoker.invokeClass)
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        /// <summary>
        /// <para>Callback used by the visibility system to determine if an observer (player) can see this object.</para>
        /// </summary>
        /// <param name="conn">Network connection of a player.</param>
        /// <returns>
        /// <para>True if the player can see this object.</para>
        /// </returns>
        public virtual bool OnCheckObserver(NetworkConnection conn) => 
            true;

        /// <summary>
        /// <para>Virtual function to override to receive custom serialization data. The corresponding function to send serialization data is OnSerialize().</para>
        /// </summary>
        /// <param name="reader">Reader to read from the stream.</param>
        /// <param name="initialState">True if being sent initial state.</param>
        public virtual void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (!initialState)
            {
                reader.ReadPackedUInt32();
            }
        }

        /// <summary>
        /// <para>This is invoked on clients when the server has caused this object to be destroyed.</para>
        /// </summary>
        public virtual void OnNetworkDestroy()
        {
        }

        public virtual bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize) => 
            false;

        /// <summary>
        /// <para>Virtual function to override to send custom serialization data. The corresponding function to send serialization data is OnDeserialize().</para>
        /// </summary>
        /// <param name="writer">Writer to use to write to the stream.</param>
        /// <param name="initialState">If this is being called to send initial state.</param>
        /// <returns>
        /// <para>True if data was written.</para>
        /// </returns>
        public virtual bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (!initialState)
            {
                writer.WritePackedUInt32(0);
            }
            return false;
        }

        /// <summary>
        /// <para>Callback used by the visibility system for objects on a host.</para>
        /// </summary>
        /// <param name="vis">New visibility state.</param>
        public virtual void OnSetLocalVisibility(bool vis)
        {
        }

        /// <summary>
        /// <para>This is invoked on behaviours that have authority, based on context and NetworkIdentity.localPlayerAuthority.</para>
        /// </summary>
        public virtual void OnStartAuthority()
        {
        }

        /// <summary>
        /// <para>Called on every NetworkBehaviour when it is activated on a client.</para>
        /// </summary>
        public virtual void OnStartClient()
        {
        }

        /// <summary>
        /// <para>Called when the local player object has been set up.</para>
        /// </summary>
        public virtual void OnStartLocalPlayer()
        {
        }

        /// <summary>
        /// <para>This is invoked for NetworkBehaviour objects when they become active on the server.</para>
        /// </summary>
        public virtual void OnStartServer()
        {
        }

        /// <summary>
        /// <para>This is invoked on behaviours when authority is removed.</para>
        /// </summary>
        public virtual void OnStopAuthority()
        {
        }

        /// <summary>
        /// <para>An internal method called on client objects to resolve GameObject references.</para>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PreStartClient()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterCommandDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.Command,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterCommandDelegate hash:", cmdHash, " ", func.GetMethodName() }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterEventDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.SyncEvent,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterEventDelegate hash:", cmdHash, " ", func.GetMethodName() }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterRpcDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.ClientRpc,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterRpcDelegate hash:", cmdHash, " ", func.GetMethodName() }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterSyncListDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.SyncList,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterSyncListDelegate hash:", cmdHash, " ", func.GetMethodName() }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendCommandInternal(NetworkWriter writer, int channelId, string cmdName)
        {
            if (!this.isLocalPlayer && !this.hasAuthority)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Trying to send command for object without authority.");
                }
            }
            else if (ClientScene.readyConnection == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Send command attempted with no client running [client=" + this.connectionToServer + "].");
                }
            }
            else
            {
                writer.FinishMessage();
                ClientScene.readyConnection.SendWriter(writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 5, cmdName, 1);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendEventInternal(NetworkWriter writer, int channelId, string eventName)
        {
            if (!NetworkServer.active)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("SendEvent no server?");
                }
            }
            else
            {
                writer.FinishMessage();
                NetworkServer.SendWriterToReady(base.gameObject, writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 7, eventName, 1);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendRPCInternal(NetworkWriter writer, int channelId, string rpcName)
        {
            if (!this.isServer)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ClientRpc call on un-spawned object");
                }
            }
            else
            {
                writer.FinishMessage();
                NetworkServer.SendWriterToReady(base.gameObject, writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 2, rpcName, 1);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendTargetRPCInternal(NetworkConnection conn, NetworkWriter writer, int channelId, string rpcName)
        {
            if (!this.isServer)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("TargetRpc call on un-spawned object");
                }
            }
            else
            {
                writer.FinishMessage();
                conn.SendWriter(writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 2, rpcName, 1);
            }
        }

        /// <summary>
        /// <para>Used to set the behaviour as dirty, so that a network update will be sent for the object.</para>
        /// </summary>
        /// <param name="dirtyBit">Bit mask to set.</param>
        public void SetDirtyBit(uint dirtyBit)
        {
            this.m_SyncVarDirtyBits |= dirtyBit;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SetSyncVar<T>(T value, ref T fieldValue, uint dirtyBit)
        {
            bool flag = false;
            if (value == null)
            {
                if (((T) fieldValue) != null)
                {
                    flag = true;
                }
            }
            else
            {
                flag = !value.Equals((T) fieldValue);
            }
            if (flag)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "SetSyncVar ", base.GetType().Name, " bit [", dirtyBit, "] ", (T) fieldValue, "->", value }));
                }
                this.SetDirtyBit(dirtyBit);
                fieldValue = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SetSyncVarGameObject(GameObject newGameObject, ref GameObject gameObjectField, uint dirtyBit, ref NetworkInstanceId netIdField)
        {
            if (!this.m_SyncVarGuard)
            {
                NetworkInstanceId netId = new NetworkInstanceId();
                if (newGameObject != null)
                {
                    NetworkIdentity component = newGameObject.GetComponent<NetworkIdentity>();
                    if (component != null)
                    {
                        netId = component.netId;
                        if (netId.IsEmpty() && LogFilter.logWarn)
                        {
                            Debug.LogWarning("SetSyncVarGameObject GameObject " + newGameObject + " has a zero netId. Maybe it is not spawned yet?");
                        }
                    }
                }
                NetworkInstanceId id2 = new NetworkInstanceId();
                if (gameObjectField != null)
                {
                    id2 = gameObjectField.GetComponent<NetworkIdentity>().netId;
                }
                if (netId != id2)
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log(string.Concat(new object[] { "SetSyncVar GameObject ", base.GetType().Name, " bit [", dirtyBit, "] netfieldId:", id2, "->", netId }));
                    }
                    this.SetDirtyBit(dirtyBit);
                    gameObjectField = newGameObject;
                    netIdField = netId;
                }
            }
        }

        /// <summary>
        /// <para>The NetworkConnection associated with this NetworkIdentity. This is only valid for player objects on the server.</para>
        /// </summary>
        public NetworkConnection connectionToClient =>
            this.myView.connectionToClient;

        /// <summary>
        /// <para>The NetworkConnection associated with this NetworkIdentity. This is only valid for player objects on the server.</para>
        /// </summary>
        public NetworkConnection connectionToServer =>
            this.myView.connectionToServer;

        /// <summary>
        /// <para>This returns true if this object is the authoritative version of the object in the distributed network application.</para>
        /// </summary>
        public bool hasAuthority =>
            this.myView.hasAuthority;

        /// <summary>
        /// <para>Returns true if running as a client and this object was spawned by a server.</para>
        /// </summary>
        public bool isClient =>
            this.myView.isClient;

        /// <summary>
        /// <para>This returns true if this object is the one that represents the player on the local machine.</para>
        /// </summary>
        public bool isLocalPlayer =>
            this.myView.isLocalPlayer;

        /// <summary>
        /// <para>Returns true if this object is active on an active server.</para>
        /// </summary>
        public bool isServer =>
            this.myView.isServer;

        /// <summary>
        /// <para>This value is set on the NetworkIdentity and is accessible here for convenient access for scripts.</para>
        /// </summary>
        public bool localPlayerAuthority =>
            this.myView.localPlayerAuthority;

        private NetworkIdentity myView
        {
            get
            {
                if (this.m_MyView == null)
                {
                    this.m_MyView = base.GetComponent<NetworkIdentity>();
                    if ((this.m_MyView == null) && LogFilter.logError)
                    {
                        Debug.LogError("There is no NetworkIdentity on this object. Please add one.");
                    }
                    return this.m_MyView;
                }
                return this.m_MyView;
            }
        }

        /// <summary>
        /// <para>The unique network Id of this object.</para>
        /// </summary>
        public NetworkInstanceId netId =>
            this.myView.netId;

        /// <summary>
        /// <para>The id of the player associated with the behaviour.</para>
        /// </summary>
        public short playerControllerId =>
            this.myView.playerControllerId;

        protected uint syncVarDirtyBits =>
            this.m_SyncVarDirtyBits;

        protected bool syncVarHookGuard
        {
            get => 
                this.m_SyncVarGuard;
            set
            {
                this.m_SyncVarGuard = value;
            }
        }

        /// <summary>
        /// <para>Delegate for Command functions.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reader"></param>
        public delegate void CmdDelegate(NetworkBehaviour obj, NetworkReader reader);

        /// <summary>
        /// <para>Delegate for Event functions.</para>
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="reader"></param>
        protected delegate void EventDelegate(List<Delegate> targets, NetworkReader reader);

        protected class Invoker
        {
            public System.Type invokeClass;
            public NetworkBehaviour.CmdDelegate invokeFunction;
            public NetworkBehaviour.UNetInvokeType invokeType;

            public string DebugString()
            {
                object[] objArray1 = new object[] { this.invokeType, ":", this.invokeClass, ":", this.invokeFunction.GetMethodName() };
                return string.Concat(objArray1);
            }
        }

        protected enum UNetInvokeType
        {
            Command,
            ClientRpc,
            SyncEvent,
            SyncList
        }
    }
}

