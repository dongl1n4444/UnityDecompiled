using System;
using System.Collections.Generic;

namespace Unity.IL2CPP
{
	public class HashCodeCache<T>
	{
		private readonly HashSet<uint> _hashes = new HashSet<uint>();

		private readonly Dictionary<uint, uint> _lastUsedCollisionOffset = new Dictionary<uint, uint>();

		private readonly Dictionary<T, uint> _cache;

		private readonly Func<T, uint> _hashFunc;

		private readonly Action<uint> _onCollisionCallback;

		public int Count
		{
			get
			{
				return this._cache.Count;
			}
		}

		public HashCodeCache(Func<T, uint> hashFunc, Action<uint> onCollisionCallback) : this(hashFunc, onCollisionCallback, null)
		{
		}

		public HashCodeCache(Func<T, uint> hashFunc, Action<uint> onCollisionCallback, IEqualityComparer<T> comparer)
		{
			this._cache = ((comparer == null) ? new Dictionary<T, uint>() : new Dictionary<T, uint>(comparer));
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
			uint result;
			if (this._cache.TryGetValue(value, out num))
			{
				result = num;
			}
			else
			{
				uint num2 = this._hashFunc(value);
				uint num3 = num2;
				uint num4;
				if (this._lastUsedCollisionOffset.TryGetValue(num2, out num4))
				{
					num3 += num4;
				}
				uint num5 = 0u;
				while (this._hashes.Contains(num3))
				{
					num4 += 1u;
					num5 += 1u;
					num3 = num2 + num4;
				}
				if (num2 != num3)
				{
					this._onCollisionCallback(num5);
				}
				this._hashes.Add(num3);
				this._cache.Add(value, num);
				this._lastUsedCollisionOffset[num2] = num4 + 1u;
				result = num3;
			}
			return result;
		}
	}
}
