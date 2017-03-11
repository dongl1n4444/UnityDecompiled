namespace UnityEngine.Networking.PlayerConnection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    internal class PlayerEditorConnectionEvents
    {
        [SerializeField]
        public ConnectionChangeEvent connectionEvent = new ConnectionChangeEvent();
        [SerializeField]
        public ConnectionChangeEvent disconnectionEvent = new ConnectionChangeEvent();
        [SerializeField]
        public List<MessageTypeSubscribers> messageTypeSubscribers = new List<MessageTypeSubscribers>();

        public UnityEvent<MessageEventArgs> AddAndCreate(Guid messageId)
        {
            <AddAndCreate>c__AnonStorey1 storey = new <AddAndCreate>c__AnonStorey1 {
                messageId = messageId
            };
            MessageTypeSubscribers item = Enumerable.SingleOrDefault<MessageTypeSubscribers>(this.messageTypeSubscribers, new Func<MessageTypeSubscribers, bool>(storey.<>m__0));
            if (item == null)
            {
                item = new MessageTypeSubscribers {
                    MessageTypeId = storey.messageId,
                    messageCallback = new MessageEvent()
                };
                this.messageTypeSubscribers.Add(item);
            }
            item.subscriberCount++;
            return item.messageCallback;
        }

        public void InvokeMessageIdSubscribers(Guid messageId, byte[] data, int playerId)
        {
            <InvokeMessageIdSubscribers>c__AnonStorey0 storey = new <InvokeMessageIdSubscribers>c__AnonStorey0 {
                messageId = messageId
            };
            IEnumerable<MessageTypeSubscribers> source = Enumerable.Where<MessageTypeSubscribers>(this.messageTypeSubscribers, new Func<MessageTypeSubscribers, bool>(storey.<>m__0));
            if (!source.Any<MessageTypeSubscribers>())
            {
                Debug.LogError("No actions found for messageId: " + storey.messageId);
            }
            else
            {
                MessageEventArgs args = new MessageEventArgs {
                    playerId = playerId,
                    data = data
                };
                foreach (MessageTypeSubscribers subscribers in source)
                {
                    subscribers.messageCallback.Invoke(args);
                }
            }
        }

        public void UnregisterManagedCallback(Guid messageId, UnityAction<MessageEventArgs> callback)
        {
            <UnregisterManagedCallback>c__AnonStorey2 storey = new <UnregisterManagedCallback>c__AnonStorey2 {
                messageId = messageId
            };
            MessageTypeSubscribers item = Enumerable.SingleOrDefault<MessageTypeSubscribers>(this.messageTypeSubscribers, new Func<MessageTypeSubscribers, bool>(storey.<>m__0));
            if (item != null)
            {
                item.subscriberCount--;
                item.messageCallback.RemoveListener(callback);
                if (item.subscriberCount <= 0)
                {
                    this.messageTypeSubscribers.Remove(item);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AddAndCreate>c__AnonStorey1
        {
            internal Guid messageId;

            internal bool <>m__0(PlayerEditorConnectionEvents.MessageTypeSubscribers x) => 
                (x.MessageTypeId == this.messageId);
        }

        [CompilerGenerated]
        private sealed class <InvokeMessageIdSubscribers>c__AnonStorey0
        {
            internal Guid messageId;

            internal bool <>m__0(PlayerEditorConnectionEvents.MessageTypeSubscribers x) => 
                (x.MessageTypeId == this.messageId);
        }

        [CompilerGenerated]
        private sealed class <UnregisterManagedCallback>c__AnonStorey2
        {
            internal Guid messageId;

            internal bool <>m__0(PlayerEditorConnectionEvents.MessageTypeSubscribers x) => 
                (x.MessageTypeId == this.messageId);
        }

        [Serializable]
        public class ConnectionChangeEvent : UnityEvent<int>
        {
        }

        [Serializable]
        public class MessageEvent : UnityEvent<MessageEventArgs>
        {
        }

        [Serializable]
        public class MessageTypeSubscribers
        {
            [SerializeField]
            private string m_messageTypeId;
            public PlayerEditorConnectionEvents.MessageEvent messageCallback = new PlayerEditorConnectionEvents.MessageEvent();
            public int subscriberCount = 0;

            public Guid MessageTypeId
            {
                get => 
                    new Guid(this.m_messageTypeId);
                set
                {
                    this.m_messageTypeId = value.ToString();
                }
            }
        }
    }
}

