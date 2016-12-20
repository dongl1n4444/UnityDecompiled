namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>Information about another participant in the same network game.</para>
    /// </summary>
    public class PeerInfoMessage : MessageBase
    {
        /// <summary>
        /// <para>The IP address of the peer.</para>
        /// </summary>
        public string address;
        /// <summary>
        /// <para>The id of the NetworkConnection associated with the peer.</para>
        /// </summary>
        public int connectionId;
        /// <summary>
        /// <para>True if this peer is the host of the network game.</para>
        /// </summary>
        public bool isHost;
        /// <summary>
        /// <para>True if the peer if the same as the current client.</para>
        /// </summary>
        public bool isYou;
        /// <summary>
        /// <para>The players for this peer.</para>
        /// </summary>
        public PeerInfoPlayer[] playerIds;
        /// <summary>
        /// <para>The network port being used by the peer.</para>
        /// </summary>
        public int port;

        public override void Deserialize(NetworkReader reader)
        {
            this.connectionId = (int) reader.ReadPackedUInt32();
            this.address = reader.ReadString();
            this.port = (int) reader.ReadPackedUInt32();
            this.isHost = reader.ReadBoolean();
            this.isYou = reader.ReadBoolean();
            uint num = reader.ReadPackedUInt32();
            if (num > 0)
            {
                List<PeerInfoPlayer> list = new List<PeerInfoPlayer>();
                for (uint i = 0; i < num; i++)
                {
                    PeerInfoPlayer player;
                    player.netId = reader.ReadNetworkId();
                    player.playerControllerId = (short) reader.ReadPackedUInt32();
                    list.Add(player);
                }
                this.playerIds = list.ToArray();
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint) this.connectionId);
            writer.Write(this.address);
            writer.WritePackedUInt32((uint) this.port);
            writer.Write(this.isHost);
            writer.Write(this.isYou);
            if (this.playerIds == null)
            {
                writer.WritePackedUInt32(0);
            }
            else
            {
                writer.WritePackedUInt32((uint) this.playerIds.Length);
                for (int i = 0; i < this.playerIds.Length; i++)
                {
                    writer.Write(this.playerIds[i].netId);
                    writer.WritePackedUInt32((uint) this.playerIds[i].playerControllerId);
                }
            }
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { "PeerInfo conn:", this.connectionId, " addr:", this.address, ":", this.port, " host:", this.isHost, " isYou:", this.isYou };
            return string.Concat(objArray1);
        }
    }
}

