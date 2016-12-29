namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>A list of floats that will be synchronized from server to clients.</para>
    /// </summary>
    public sealed class SyncListFloat : SyncList<float>
    {
        protected override float DeserializeItem(NetworkReader reader) => 
            reader.ReadSingle();

        [Obsolete("ReadReference is now used instead")]
        public static SyncListFloat ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListFloat num2 = new SyncListFloat();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                num2.AddInternal(reader.ReadSingle());
            }
            return num2;
        }

        /// <summary>
        /// <para>An internal function used for serializing SyncList member variables.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="syncList"></param>
        public static void ReadReference(NetworkReader reader, SyncListFloat syncList)
        {
            ushort num = reader.ReadUInt16();
            syncList.Clear();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                syncList.AddInternal(reader.ReadSingle());
            }
        }

        protected override void SerializeItem(NetworkWriter writer, float item)
        {
            writer.Write(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListFloat items)
        {
            writer.Write((ushort) items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                writer.Write(items[i]);
            }
        }
    }
}

