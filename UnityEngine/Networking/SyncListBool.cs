namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>A list of booleans that will be synchronized from server to clients.</para>
    /// </summary>
    public class SyncListBool : SyncList<bool>
    {
        protected override bool DeserializeItem(NetworkReader reader) => 
            reader.ReadBoolean();

        [Obsolete("ReadReference is now used instead")]
        public static SyncListBool ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListBool @bool = new SyncListBool();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                @bool.AddInternal(reader.ReadBoolean());
            }
            return @bool;
        }

        /// <summary>
        /// <para>An internal function used for serializing SyncList member variables.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="syncList"></param>
        public static void ReadReference(NetworkReader reader, SyncListBool syncList)
        {
            ushort num = reader.ReadUInt16();
            syncList.Clear();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                syncList.AddInternal(reader.ReadBoolean());
            }
        }

        protected override void SerializeItem(NetworkWriter writer, bool item)
        {
            writer.Write(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListBool items)
        {
            writer.Write((ushort) items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                writer.Write(items[i]);
            }
        }
    }
}

