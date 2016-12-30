namespace UnityEngine
{
    using System;

    internal interface IPlayerEditorConnectionNative
    {
        void Initialize();
        void RegisterInternal(Guid messageId);
        void SendMessage(Guid messageId, byte[] data, int playerId);
        void UnregisterInternal(Guid messageId);
    }
}

