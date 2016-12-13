using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public class ReadOnlyHashSet<T> : IEnumerable<T>, IEnumerable
	{
		private readonly HashSet<T> _set;

		public int Count
		{
			get
			{
				return this._set.Count;
			}
		}

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

		public ReadOnlyHashSet(IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			this._set = new HashSet<T>(values, comparer);
		}

		public ReadOnlyHashSet(IEqualityComparer<T> comparer)
		{
			this._set = new HashSet<T>(comparer);
		}

		public bool Contains(T item)
		{
			return this._set.Contains(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._set.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
