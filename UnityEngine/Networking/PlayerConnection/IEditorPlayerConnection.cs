namespace UnityEngine.Networking.PlayerConnection
{
    using System;
    using UnityEngine.Events;

    internal interface IEditorPlayerConnection
    {
        void Register(Guid messageId, UnityAction<MessageEventArgs> callback);
        void RegisterConnection(UnityAction<int> callback);
        void RegisterDisconnection(UnityAction<int> callback);
        void Send(Guid messageId, byte[] data);
        void Unregister(Guid messageId, UnityAction<MessageEventArgs> callback);
    }
}

