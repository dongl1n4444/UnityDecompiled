namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>This is passed to handler funtions registered for the SYSTEM_REMOVE_PLAYER built-in message.</para>
    /// </summary>
    public class RemovePlayerMessage : MessageBase
    {
        /// <summary>
        /// <para>The player ID of the player object which should be removed.</para>
        /// </summary>
        public short playerControllerId;

        public override void Deserialize(NetworkReader reader)
        {
            this.playerControllerId = (short) reader.ReadUInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.playerControllerId);
        }
    }
}

