namespace UnityEngine.Runtime.Networking.PlayerConnection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Used for handling the network connection from the Player to the Editor.</para>
    /// </summary>
    [Serializable]
    public class PlayerConnection : ScriptableObject, IEditorPlayerConnection
    {
        internal static IPlayerEditorConnectionNative connectionNative;
        [SerializeField]
        private List<int> m_connectedPlayers = new List<int>();
        private bool m_IsInitilized;
        [SerializeField]
        private PlayerEditorConnectionEvents m_PlayerEditorConnectionEvents = new PlayerEditorConnectionEvents();
        private static UnityEngine.Runtime.Networking.PlayerConnection.PlayerConnection s_Instance;

        [RequiredByNativeCode]
        private static void ConnectedCallbackInternal(int playerId)
        {
            instance.m_connectedPlayers.Add(playerId);
            instance.m_PlayerEditorConnectionEvents.connectionEvent.Invoke(playerId);
        }

        private static UnityEngine.Runtime.Networking.PlayerConnection.PlayerConnection CreateInstance()
        {
            s_Instance = ScriptableObject.CreateInstance<UnityEngine.Runtime.Networking.PlayerConnection.PlayerConnection>();
            s_Instance.hideFlags = HideFlags.HideAndDontSave;
            return s_Instance;
        }

        [RequiredByNativeCode]
        private static void DisconnectedCallback(int playerId)
        {
            instance.m_PlayerEditorConnectionEvents.disconnectionEvent.Invoke(playerId);
        }

        private IPlayerEditorConnectionNative GetConnectionNativeApi()
        {
            IPlayerEditorConnectionNative connectionNative;
            if (UnityEngine.Runtime.Networking.PlayerConnection.PlayerConnection.connectionNative != null)
            {
                connectionNative = UnityEngine.Runtime.Networking.PlayerConnection.PlayerConnection.connectionNative;
            }
            else
            {
                connectionNative = new PlayerConnectionInternal();
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
            instance.m_PlayerEditorConnectionEvents.InvokeMessageIdSubscribers(new Guid(messageId), destination, (int) guid);
        }

        public void OnEnable()
        {
            if (!this.m_IsInitilized)
            {
                this.m_IsInitilized = true;
                this.GetConnectionNativeApi().Initialize();
            }
        }

        public void Register(Guid messageId, UnityAction<MessageEventArgs> callback)
        {
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException("Cant be Guid.Empty", "messageId");
            }
            if (!this.m_PlayerEditorConnectionEvents.messageTypeSubscribers.Any<PlayerEditorConnectionEvents.MessageTypeSubscribers>())
            {
                this.GetConnectionNativeApi().RegisterInternal(messageId);
            }
            this.m_PlayerEditorConnectionEvents.AddAndCreate(messageId).AddListener(callback);
        }

        public void RegisterConnection(UnityAction<int> callback)
        {
            foreach (int num in this.m_connectedPlayers)
            {
                callback(num);
            }
            this.m_PlayerEditorConnectionEvents.connectionEvent.AddListener(callback);
        }

        public void RegisterDisconnection(UnityAction<int> callback)
        {
            this.m_PlayerEditorConnectionEvents.disconnectionEvent.AddListener(callback);
        }

        /// <summary>
        /// <para>Sends data to the Editor.</para>
        /// </summary>
        /// <param name="messageId">The type ID of the message that is sent to the Editor.</param>
        /// <param name="data"></param>
        public void Send(Guid messageId, byte[] data)
        {
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException("Cant be Guid.Empty", "messageId");
            }
            this.GetConnectionNativeApi().SendMessage(messageId, data, 0);
        }

        public void Unregister(Guid messageId, UnityAction<MessageEventArgs> callback)
        {
            <Unregister>c__AnonStorey0 storey = new <Unregister>c__AnonStorey0 {
                messageId = messageId
            };
            this.m_PlayerEditorConnectionEvents.UnregisterManagedCallback(storey.messageId, callback);
            if (!Enumerable.Any<PlayerEditorConnectionEvents.MessageTypeSubscribers>(this.m_PlayerEditorConnectionEvents.messageTypeSubscribers, new Func<PlayerEditorConnectionEvents.MessageTypeSubscribers, bool>(storey.<>m__0)))
            {
                this.GetConnectionNativeApi().UnregisterInternal(storey.messageId);
            }
        }

        /// <summary>
        /// <para>Singleton instance.</para>
        /// </summary>
        public static UnityEngine.Runtime.Networking.PlayerConnection.PlayerConnection instance
        {
            get
            {
                if (s_Instance == null)
                {
                    return CreateInstance();
                }
                return s_Instance;
            }
        }

        [CompilerGenerated]
        private sealed class <Unregister>c__AnonStorey0
        {
            internal Guid messageId;

            internal bool <>m__0(PlayerEditorConnectionEvents.MessageTypeSubscribers x) => 
                (x.MessageTypeId == this.messageId);
        }
    }
}

