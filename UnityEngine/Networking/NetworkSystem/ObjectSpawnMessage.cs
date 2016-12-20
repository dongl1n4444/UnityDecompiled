namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    internal class ObjectSpawnMessage : MessageBase
    {
        public NetworkHash128 assetId;
        public NetworkInstanceId netId;
        public byte[] payload;
        public Vector3 position;
        public Quaternion rotation;

        public override void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.assetId = reader.ReadNetworkHash128();
            this.position = reader.ReadVector3();
            this.payload = reader.ReadBytesAndSize();
            uint num = 0x10;
            if ((reader.Length - reader.Position) >= num)
            {
                this.rotation = reader.ReadQuaternion();
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.assetId);
            writer.Write(this.position);
            writer.WriteBytesFull(this.payload);
            writer.Write(this.rotation);
        }
    }
}

