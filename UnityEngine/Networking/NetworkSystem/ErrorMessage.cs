namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>This is passed to handler functions registered for the SYSTEM_ERROR built-in message.</para>
    /// </summary>
    public class ErrorMessage : MessageBase
    {
        /// <summary>
        /// <para>The error code.</para>
        /// </summary>
        public int errorCode;

        public override void Deserialize(NetworkReader reader)
        {
            this.errorCode = reader.ReadUInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.errorCode);
        }
    }
}

