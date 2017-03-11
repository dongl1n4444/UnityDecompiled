namespace UnityEngine.Collections
{
    using System;

    internal sealed class NativeArrayDebugView<T> where T: struct
    {
        private NativeArray<T> array;

        public NativeArrayDebugView(NativeArray<T> array)
        {
            this.array = array;
        }

        public T[] Items =>
            this.array.ToArray();
    }
}

