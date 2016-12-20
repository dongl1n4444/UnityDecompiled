namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>The details of a network message received by a client or server on a network connection.</para>
    /// </summary>
    public class NetworkMessage
    {
        /// <summary>
        /// <para>The transport layer channel the message was sent on.</para>
        /// </summary>
        public int channelId;
        /// <summary>
        /// <para>The connection the message was recieved on.</para>
        /// </summary>
        public NetworkConnection conn;
        /// <summary>
        /// <para>The size of the largest message in bytes that can be sent on a NetworkConnection.</para>
        /// </summary>
        public const int MaxMessageSize = 0xffff;
        /// <summary>
        /// <para>The id of the message type of the message.</para>
        /// </summary>
        public short msgType;
        /// <summary>
        /// <para>A NetworkReader object that contains the contents of the message.</para>
        /// </summary>
        public NetworkReader reader;

        /// <summary>
        /// <para>Returns a string with the numeric representation of each byte in the payload.</para>
        /// </summary>
        /// <param name="payload">Network message payload to dump.</param>
        /// <param name="sz">Length of payload in bytes.</param>
        /// <returns>
        /// <para>Dumped info from payload.</para>
        /// </returns>
        public static string Dump(byte[] payload, int sz)
        {
            string str = "[";
            for (int i = 0; i < sz; i++)
            {
                str = str + payload[i] + " ";
            }
            return (str + "]");
        }

        public TMsg ReadMessage<TMsg>() where TMsg: MessageBase, new()
        {
            TMsg local = Activator.CreateInstance<TMsg>();
            local.Deserialize(this.reader);
            return local;
        }

        public void ReadMessage<TMsg>(TMsg msg) where TMsg: MessageBase
        {
            msg.Deserialize(this.reader);
        }
    }
}

