namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>This is a utility class for simple network messages that contain only a string.</para>
    /// </summary>
    public class StringMessage : MessageBase
    {
        /// <summary>
        /// <para>The string that will be serialized.</para>
        /// </summary>
        public string value;

        public StringMessage()
        {
        }

        public StringMessage(string v)
        {
            this.value = v;
        }

        public override void Deserialize(NetworkReader reader)
        {
            this.value = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.value);
        }
    }
}

