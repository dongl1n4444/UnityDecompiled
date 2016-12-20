namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>This is passed to handler funtions registered for the AddPlayer built-in message.</para>
    /// </summary>
    public class AddPlayerMessage : MessageBase
    {
        /// <summary>
        /// <para>The extra message data included in the AddPlayerMessage.</para>
        /// </summary>
        public byte[] msgData;
        /// <summary>
        /// <para>The size of the extra message data included in the AddPlayerMessage.</para>
        /// </summary>
        public int msgSize;
        /// <summary>
        /// <para>The playerId of the new player.</para>
        /// </summary>
        public short playerControllerId;

        public override void Deserialize(NetworkReader reader)
        {
            this.playerControllerId = (short) reader.ReadUInt16();
            this.msgData = reader.ReadBytesAndSize();
            if (this.msgData == null)
            {
                this.msgSize = 0;
            }
            else
            {
                this.msgSize = this.msgData.Length;
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.playerControllerId);
            writer.WriteBytesAndSize(this.msgData, this.msgSize);
        }
    }
}

