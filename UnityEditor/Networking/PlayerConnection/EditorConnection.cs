namespace UnityEditor.Networking.PlayerConnection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Runtime.Networking.PlayerConnection;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Handles the connection from the Editor to the Player.</para>
    /// </summary>
    [Serializable]
    public class EditorConnection : ScriptableSingleton<EditorConnection>, IEditorPlayerConnection
    {
        internal static IPlayerEditorConnectionNative connectionNative;
        [SerializeField]
        private List<int> m_connectedPlayers = new List<int>();
        [SerializeField]
        private bool m_hasDoneInitilizeThisEditorSession;
        [SerializeField]
        private PlayerEditorConnectionEvents m_PlayerEditorConnectionEvents = new PlayerEditorConnectionEvents();

        [RequiredByNativeCode]
        private static void ConnectedCallbackInternal(int playerId)
        {
            ScriptableSingleton<EditorConnection>.instance.m_connectedPlayers.Add(playerId);
            ScriptableSingleton<EditorConnection>.instance.m_PlayerEditorConnectionEvents.connectionEvent.Invoke(playerId);
        }

        [RequiredByNativeCode]
        private static void DisconnectedCallback(int playerId)
        {
            ScriptableSingleton<EditorConnection>.instance.m_connectedPlayers.Remove(playerId);
            ScriptableSingleton<EditorConnection>.instance.m_PlayerEditorConnectionEvents.disconnectionEvent.Invoke(playerId);
        }

        private IPlayerEditorConnectionNative GetEditorConnectionNativeApi()
        {
            IPlayerEditorConnectionNative connectionNative;
            if (EditorConnection.connectionNative != null)
            {
                connectionNative = EditorConnection.connectionNative;
            }
            else
            {
                connectionNative = new EditorConnectionInternal();
            }
            return connectionNative;
        }

        [RequiredByNativeCode]
        private static void MessageCallbackInternal(IntPtr data, ulong size, ulong guid, string messageId)
        {
            byte[] destination = null;
            if (size > 0L)
            {
                destination = new byte[size];
                Marshal.Copy(data, destination, 0, (int) size);
            }
            ScriptableSingleton<EditorConnection>.instance.m_PlayerEditorConnectionEvents.InvokeMessageIdSubscribers(new Guid(messageId), destination, (int) guid);
        }

        public void OnEnable()
        {
            if (!this.m_hasDoneInitilizeThisEditorSession)
            {
                this.m_hasDoneInitilizeThisEditorSession = true;
                this.GetEditorConnectionNativeApi().Initialize();
            }
        }

        public void Register(Guid messageId, UnityAction<MessageEventArgs> callback)
        {
            <Register>c__AnonStorey0 storey = new <Register>c__AnonStorey0 {
                messageId = messageId
            };
            if (storey.messageId == Guid.Empty)
            {
                throw new ArgumentException("Cant be Guid.Empty", "messageId");
            }
            if (!Enumerable.Any<PlayerEditorConnectionEvents.MessageTypeSubscribers>(this.m_PlayerEditorConnectionEvents.messageTypeSubscribers, new Func<PlayerEditorConnectionEvents.MessageTypeSubscribers, bool>(storey.<>m__0)))
            {
                this.GetEditorConnectionNativeApi().RegisterInternal(storey.messageId);
            }
            this.m_PlayerEditorConnectionEvents.AddAndCreate(storey.messageId).AddPersistentListener(callback, UnityEventCallState.EditorAndRuntime);
        }

        public void RegisterConnection(UnityAction<int> callback)
        {
            foreach (int num in this.m_connectedPlayers)
            {
                callback(num);
            }
            this.m_PlayerEditorConnectionEvents.connectionEvent.AddPersistentListener(callback, UnityEventCallState.EditorAndRuntime);
        }

        public void RegisterDisconnection(UnityAction<int> callback)
        {
            this.m_PlayerEditorConnectionEvents.disconnectionEvent.AddPersistentListener(callback, UnityEventCallState.EditorAndRuntime);
        }

        /// <summary>
        /// <para>Sends data to multiple or single Player(s).</para>
        /// </summary>
        /// <param name="messageId">Type ID of the message to send to the Player(s).</param>
        /// <param name="playerId">If set, the message will only send to the Player with this ID.</param>
        /// <param name="data"></param>
        public void Send(Guid messageId, byte[] data)
        {
            this.Send(messageId, data, 0);
        }

        /// <summary>
        /// <para>Sends data to multiple or single Player(s).</para>
        /// </summary>
        /// <param name="messageId">Type ID of the message to send to the Player(s).</param>
        /// <param name="playerId">If set, the message will only send to the Player with this ID.</param>
        /// <param name="data"></param>
        public void Send(Guid messageId, byte[] data, int playerId)
        {
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException("Cant be Guid.Empty", "messageId");
            }
            this.GetEditorConnectionNativeApi().SendMessage(messageId, data, playerId);
        }

        public void Unregister(Guid messageId, UnityAction<MessageEventArgs> callback)
        {
            <Unregister>c__AnonStorey1 storey = new <Unregister>c__AnonStorey1 {
                messageId = messageId
            };
            this.m_PlayerEditorConnectionEvents.UnregisterManagedCallback(storey.messageId, callback);
            if (!Enumerable.Any<PlayerEditorConnectionEvents.MessageTypeSubscribers>(this.m_PlayerEditorConnectionEvents.messageTypeSubscribers, new Func<PlayerEditorConnectionEvents.MessageTypeSubscribers, bool>(storey.<>m__0)))
            {
                this.GetEditorConnectionNativeApi().UnregisterInternal(storey.messageId);
            }
        }

        [CompilerGenerated]
        private sealed class <Register>c__AnonStorey0
        {
            internal Guid messageId;

            internal bool <>m__0(PlayerEditorConnectionEvents.MessageTypeSubscribers x) => 
                (x.MessageTypeId == this.messageId);
        }

        [CompilerGenerated]
        private sealed class <Unregister>c__AnonStorey1
        {
            internal Guid messageId;

            internal bool <>m__0(PlayerEditorConnectionEvents.MessageTypeSubscribers x) => 
                (x.MessageTypeId == this.messageId);
        }
    }
}

