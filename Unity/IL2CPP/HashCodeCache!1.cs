namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;

    public class HashCodeCache<T>
    {
        private readonly Dictionary<T, uint> _cache;
        private readonly HashSet<uint> _hashes;
        private readonly Func<T, uint> _hashFunc;
        private readonly Dictionary<uint, uint> _lastUsedCollisionOffset;
        private readonly Action<uint> _onCollisionCallback;

        public HashCodeCache(Func<T, uint> hashFunc, Action<uint> onCollisionCallback) : this(hashFunc, onCollisionCallback, null)
        {
        }

        public HashCodeCache(Func<T, uint> hashFunc, Action<uint> onCollisionCallback, IEqualityComparer<T> comparer)
        {
            this._hashes = new HashSet<uint>();
            this._lastUsedCollisionOffset = new Dictionary<uint, uint>();
            this._cache = (comparer == null) ? new Dictionary<T, uint>() : new Dictionary<T, uint>(comparer);
            this._hashFunc = hashFunc;
            this._onCollisionCallback = onCollisionCallback;
        }

        public void Clear()
        {
            this._hashes.Clear();
            this._cache.Clear();
            this._lastUsedCollisionOffset.Clear();
        }

        public uint GetUniqueHash(T value)
        {
            uint num;
            uint num5;
            if (this._cache.TryGetValue(value, out num))
            {
                return num;
            }
            uint key = this._hashFunc.Invoke(value);
            uint item = key;
            if (this._lastUsedCollisionOffset.TryGetValue(key, out num5))
            {
                item += num5;
            }
            uint num6 = 0;
            while (this._hashes.Contains(item))
            {
                num5++;
                num6++;
                item = key + num5;
            }
            if (key != item)
            {
                this._onCollisionCallback(num6);
            }
            this._hashes.Add(item);
            this._cache.Add(value, num);
            this._lastUsedCollisionOffset[key] = num5 + 1;
            return item;
        }

        public int Count
        {
            get
            {
                return this._cache.Count;
            }
        }
    }
}

