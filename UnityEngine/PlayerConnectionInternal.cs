namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal sealed class PlayerConnectionInternal : IPlayerEditorConnectionNative
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Initialize();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsConnected();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RegisterInternal(string messageId);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SendMessage(string messageId, byte[] data, int playerId);
        void IPlayerEditorConnectionNative.Initialize()
        {
            Initialize();
        }

        bool IPlayerEditorConnectionNative.IsConnected() => 
            IsConnected();

        void IPlayerEditorConnectionNative.RegisterInternal(Guid messageId)
        {
            RegisterInternal(messageId.ToString("N"));
        }

        void IPlayerEditorConnectionNative.SendMessage(Guid messageId, byte[] data, int playerId)
        {
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException("messageId must not be empty");
            }
            SendMessage(messageId.ToString("N"), data, playerId);
        }

        void IPlayerEditorConnectionNative.UnregisterInternal(Guid messageId)
        {
            UnregisterInternal(messageId.ToString("N"));
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UnregisterInternal(string messageId);
    }
}

