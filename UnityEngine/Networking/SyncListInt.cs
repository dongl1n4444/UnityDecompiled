namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>A list of integers that will be synchronized from server to clients.</para>
    /// </summary>
    public class SyncListInt : SyncList<int>
    {
        protected override int DeserializeItem(NetworkReader reader) => 
            ((int) reader.ReadPackedUInt32());

        [Obsolete("ReadReference is now used instead")]
        public static SyncListInt ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListInt num2 = new SyncListInt();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                num2.AddInternal((int) reader.ReadPackedUInt32());
            }
            return num2;
        }

        /// <summary>
        /// <para>An internal function used for serializing SyncList member variables.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="syncList"></param>
        public static void ReadReference(NetworkReader reader, SyncListInt syncList)
        {
            ushort num = reader.ReadUInt16();
            syncList.Clear();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                syncList.AddInternal((int) reader.ReadPackedUInt32());
            }
        }

        protected override void SerializeItem(NetworkWriter writer, int item)
        {
            writer.WritePackedUInt32((uint) item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListInt items)
        {
            writer.Write((ushort) items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                writer.WritePackedUInt32(items[i]);
            }
        }
    }
}

