namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>A utility class to send simple network messages that only contain an integer.</para>
    /// </summary>
    public class IntegerMessage : MessageBase
    {
        /// <summary>
        /// <para>The integer value to serialize.</para>
        /// </summary>
        public int value;

        public IntegerMessage()
        {
        }

        public IntegerMessage(int v)
        {
            this.value = v;
        }

        public override void Deserialize(NetworkReader reader)
        {
            this.value = (int) reader.ReadPackedUInt32();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint) this.value);
        }
    }
}

