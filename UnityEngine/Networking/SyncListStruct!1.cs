namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>This class is used for lists of structs that are synchronized from the server to clients.</para>
    /// </summary>
    public class SyncListStruct<T> : SyncList<T> where T: struct
    {
        public void AddInternal(T item)
        {
            base.AddInternal(item);
        }

        protected override T DeserializeItem(NetworkReader reader)
        {
            return Activator.CreateInstance<T>();
        }

        public T GetItem(int i)
        {
            return base[i];
        }

        protected override void SerializeItem(NetworkWriter writer, T item)
        {
        }

        public ushort Count
        {
            get
            {
                return (ushort) base.Count;
            }
        }
    }
}

