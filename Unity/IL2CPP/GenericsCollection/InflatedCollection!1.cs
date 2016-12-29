namespace Unity.IL2CPP.GenericsCollection
{
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Common;

    public class InflatedCollection<T>
    {
        private readonly HashSet<T> _items;

        public InflatedCollection(IEqualityComparer<T> comparer)
        {
            this._items = new HashSet<T>(comparer);
        }

        public virtual bool Add(T item) => 
            this._items.Add(item);

        public int Count =>
            this._items.Count;

        public ReadOnlyHashSet<T> Items =>
            this._items.AsReadOnly<T>();
    }
}

