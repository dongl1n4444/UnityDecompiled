namespace Unity.IL2CPP.Common
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ArrayView<T>
    {
        private readonly T[] _array;
        private readonly int _offset;
        private readonly int _length;
        public ArrayView(T[] array, int offset, int length)
        {
            this._array = array;
            this._offset = offset;
            this._length = length;
            if (offset < 0)
            {
                throw new ArgumentException($"offset must be greater than or equal to zero. It was {offset}.");
            }
            if (length < 0)
            {
                throw new ArgumentException($"length must be greater than or equal to zero. It was {length}.");
            }
            if ((offset + length) > array.Length)
            {
                throw new ArgumentException($"ArrayView cannot extend beyond the end of the array. Array length was {this._array.Length}; offset was {offset}; length was {length}.");
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= this._length)
                {
                    throw new IndexOutOfRangeException($"Attempted to access ArrayView out of its bounds. ArrayView length was {this._length}; index was {index}.");
                }
                return this._array[this._offset + index];
            }
        }
        public int Length =>
            this._length;
    }
}

