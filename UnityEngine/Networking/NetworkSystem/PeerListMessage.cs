namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>Internal UNET message for sending information about network peers to clients.</para>
    /// </summary>
    public class PeerListMessage : MessageBase
    {
        /// <summary>
        /// <para>The connectionId of this client on the old host.</para>
        /// </summary>
        public int oldServerConnectionId;
        /// <summary>
        /// <para>The list of participants in a networked game.</para>
        /// </summary>
        public PeerInfoMessage[] peers;

        public override void Deserialize(NetworkReader reader)
        {
            this.oldServerConnectionId = (int) reader.ReadPackedUInt32();
            int num = reader.ReadUInt16();
            this.peers = new PeerInfoMessage[num];
            for (int i = 0; i < this.peers.Length; i++)
            {
                PeerInfoMessage message = new PeerInfoMessage();
                message.Deserialize(reader);
                this.peers[i] = message;
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint) this.oldServerConnectionId);
            writer.Write((ushort) this.peers.Length);
            for (int i = 0; i < this.peers.Length; i++)
            {
                this.peers[i].Serialize(writer);
            }
        }
    }
}

