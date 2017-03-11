namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ReadOnlyHashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private readonly HashSet<T> _set;

        public ReadOnlyHashSet()
        {
            this._set = new HashSet<T>();
        }

        public ReadOnlyHashSet(HashSet<T> set)
        {
            this._set = set;
        }

        public ReadOnlyHashSet(IEnumerable<T> values)
        {
            this._set = new HashSet<T>(values);
        }

        public ReadOnlyHashSet(IEqualityComparer<T> comparer)
        {
            this._set = new HashSet<T>(comparer);
        }

        public ReadOnlyHashSet(IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            this._set = new HashSet<T>(values, comparer);
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item) => 
            this._set.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
            {
                throw new ArgumentException("arrayIndex", $"Array index must be greater than or equal to zero. It was {arrayIndex}.");
            }
            if ((arrayIndex > array.Length) || ((array.Length - arrayIndex) < this._set.Count))
            {
                throw new ArgumentException($"Array was too small to fit whole set. array.Length was {array.Length}; arrayIndex was {arrayIndex}; set size was {this._set.Count}.");
            }
            foreach (T local in this._set)
            {
                array[arrayIndex++] = local;
            }
        }

        public IEnumerator<T> GetEnumerator() => 
            this._set.GetEnumerator();

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count =>
            this._set.Count;

        public bool IsReadOnly =>
            true;
    }
}

