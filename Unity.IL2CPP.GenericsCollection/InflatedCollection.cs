using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.GenericsCollection
{
	public class InflatedCollection<T>
	{
		private readonly HashSet<T> _items;

		public ReadOnlyHashSet<T> Items
		{
			get
			{
				return this._items.AsReadOnly<T>();
			}
		}

		public int Count
		{
			get
			{
				return this._items.Count;
			}
		}

		public InflatedCollection(IEqualityComparer<T> comparer)
		{
			this._items = new HashSet<T>(comparer);
		}

		public virtual bool Add(T item)
		{
			return this._items.Add(item);
		}
	}
}
