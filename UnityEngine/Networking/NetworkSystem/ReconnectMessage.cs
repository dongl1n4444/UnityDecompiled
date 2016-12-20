namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>This network message is used when a client reconnect to the new host of a game.</para>
    /// </summary>
    public class ReconnectMessage : MessageBase
    {
        /// <summary>
        /// <para>Additional data.</para>
        /// </summary>
        public byte[] msgData;
        /// <summary>
        /// <para>Size of additional data.</para>
        /// </summary>
        public int msgSize;
        /// <summary>
        /// <para>The networkId of this player on the old host.</para>
        /// </summary>
        public NetworkInstanceId netId;
        /// <summary>
        /// <para>This client's connectionId on the old host.</para>
        /// </summary>
        public int oldConnectionId;
        /// <summary>
        /// <para>The playerControllerId of the player that is rejoining.</para>
        /// </summary>
        public short playerControllerId;

        public override void Deserialize(NetworkReader reader)
        {
            this.oldConnectionId = (int) reader.ReadPackedUInt32();
            this.playerControllerId = (short) reader.ReadPackedUInt32();
            this.netId = reader.ReadNetworkId();
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
            writer.WritePackedUInt32((uint) this.oldConnectionId);
            writer.WritePackedUInt32((uint) this.playerControllerId);
            writer.Write(this.netId);
            writer.WriteBytesAndSize(this.msgData, this.msgSize);
        }
    }
}

