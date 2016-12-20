namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>A list of unsigned integers that will be synchronized from server to clients.</para>
    /// </summary>
    public class SyncListUInt : SyncList<uint>
    {
        protected override uint DeserializeItem(NetworkReader reader)
        {
            return reader.ReadPackedUInt32();
        }

        [Obsolete("ReadReference is now used instead")]
        public static SyncListUInt ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListUInt num2 = new SyncListUInt();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                num2.AddInternal(reader.ReadPackedUInt32());
            }
            return num2;
        }

        /// <summary>
        /// <para>An internal function used for serializing SyncList member variables.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="syncList"></param>
        public static void ReadReference(NetworkReader reader, SyncListUInt syncList)
        {
            ushort num = reader.ReadUInt16();
            syncList.Clear();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                syncList.AddInternal(reader.ReadPackedUInt32());
            }
        }

        protected override void SerializeItem(NetworkWriter writer, uint item)
        {
            writer.WritePackedUInt32(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListUInt items)
        {
            writer.Write((ushort) items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                writer.WritePackedUInt32(items[i]);
            }
        }
    }
}

