namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>This is a list of strings that will be synchronized from the server to clients.</para>
    /// </summary>
    public sealed class SyncListString : SyncList<string>
    {
        protected override string DeserializeItem(NetworkReader reader)
        {
            return reader.ReadString();
        }

        [Obsolete("ReadReference is now used instead")]
        public static SyncListString ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListString str = new SyncListString();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                str.AddInternal(reader.ReadString());
            }
            return str;
        }

        /// <summary>
        /// <para>An internal function used for serializing SyncList member variables.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="syncList"></param>
        public static void ReadReference(NetworkReader reader, SyncListString syncList)
        {
            ushort num = reader.ReadUInt16();
            syncList.Clear();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                syncList.AddInternal(reader.ReadString());
            }
        }

        protected override void SerializeItem(NetworkWriter writer, string item)
        {
            writer.Write(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListString items)
        {
            writer.Write((ushort) items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                writer.Write(items[i]);
            }
        }
    }
}

