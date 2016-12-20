namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>Network message classes should be derived from this class. These message classes can then be sent using the various Send functions of NetworkConnection, NetworkClient and NetworkServer.</para>
    /// </summary>
    public abstract class MessageBase
    {
        protected MessageBase()
        {
        }

        /// <summary>
        /// <para>This method is used to populate a message object from a NetworkReader stream.</para>
        /// </summary>
        /// <param name="reader">Stream to read from.</param>
        public virtual void Deserialize(NetworkReader reader)
        {
        }

        /// <summary>
        /// <para>The method is used to populate a NetworkWriter stream from a message object.</para>
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        public virtual void Serialize(NetworkWriter writer)
        {
        }
    }
}

