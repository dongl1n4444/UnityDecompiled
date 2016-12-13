using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Unity.IL2CPP.Common
{
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ICollection
	{
		[Serializable]
		private struct DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			private readonly IDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public DictionaryEntry Entry
			{
				get
				{
					KeyValuePair<TKey, TValue> current = this.m_enumerator.Current;
					object arg_31_0 = current.Key;
					KeyValuePair<TKey, TValue> current2 = this.m_enumerator.Current;
					return new DictionaryEntry(arg_31_0, current2.Value);
				}
			}

			public object Key
			{
				get
				{
					KeyValuePair<TKey, TValue> current = this.m_enumerator.Current;
					return current.Key;
				}
			}

			public object Value
			{
				get
				{
					KeyValuePair<TKey, TValue> current = this.m_enumerator.Current;
					return current.Value;
				}
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
				this.m_enumerator = this.m_dictionary.GetEnumerator();
			}

			public bool MoveNext()
			{
				return this.m_enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_enumerator.Reset();
			}
		}

		[DebuggerDisplay("Count = {Count}")]
		[Serializable]
		public sealed class KeyCollection : ICollection<TKey>, ICollection, IEnumerable<TKey>, IEnumerable
		{
			private readonly ICollection<TKey> m_collection;

			[NonSerialized]
			private object m_syncRoot;

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					if (this.m_syncRoot == null)
					{
						ICollection collection = this.m_collection as ICollection;
						if (collection != null)
						{
							this.m_syncRoot = collection.SyncRoot;
						}
						else
						{
							Interlocked.CompareExchange<object>(ref this.m_syncRoot, new object(), null);
						}
					}
					return this.m_syncRoot;
				}
			}

			public int Count
			{
				get
				{
					return this.m_collection.Count;
				}
			}

			internal KeyCollection(ICollection<TKey> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException("collection");
				}
				this.m_collection = collection;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return this.m_collection.Contains(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				this.m_collection.CopyTo(array, arrayIndex);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return this.m_collection.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.m_collection.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				ReadOnlyDictionaryHelpers.CopyToNonGenericICollectionHelper<TKey>(this.m_collection, array, index);
			}
		}

		[DebuggerDisplay("Count = {Count}")]
		[Serializable]
		public sealed class ValueCollection : ICollection<TValue>, ICollection, IEnumerable<TValue>, IEnumerable
		{
			private readonly ICollection<TValue> m_collection;

			[NonSerialized]
			private object m_syncRoot;

			bool ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					if (this.m_syncRoot == null)
					{
						ICollection collection = this.m_collection as ICollection;
						if (collection != null)
						{
							this.m_syncRoot = collection.SyncRoot;
						}
						else
						{
							Interlocked.CompareExchange<object>(ref this.m_syncRoot, new object(), null);
						}
					}
					return this.m_syncRoot;
				}
			}

			public int Count
			{
				get
				{
					return this.m_collection.Count;
				}
			}

			internal ValueCollection(ICollection<TValue> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException("collection");
				}
				this.m_collection = collection;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return this.m_collection.Contains(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				this.m_collection.CopyTo(array, arrayIndex);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return this.m_collection.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.m_collection.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				ReadOnlyDictionaryHelpers.CopyToNonGenericICollectionHelper<TValue>(this.m_collection, array, index);
			}
		}

		private readonly IDictionary<TKey, TValue> m_dictionary;

		[NonSerialized]
		private object m_syncRoot;

		[NonSerialized]
		private ReadOnlyDictionary<TKey, TValue>.KeyCollection m_keys;

		[NonSerialized]
		private ReadOnlyDictionary<TKey, TValue>.ValueCollection m_values;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return this.Values;
			}
		}

		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get
			{
				return this.m_dictionary[key];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return this.Values;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				object result;
				if (ReadOnlyDictionary<TKey, TValue>.IsCompatibleKey(key))
				{
					result = this[(TKey)((object)key)];
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this.m_syncRoot == null)
				{
					ICollection collection = this.m_dictionary as ICollection;
					if (collection != null)
					{
						this.m_syncRoot = collection.SyncRoot;
					}
					else
					{
						Interlocked.CompareExchange<object>(ref this.m_syncRoot, new object(), null);
					}
				}
				return this.m_syncRoot;
			}
		}

		protected IDictionary<TKey, TValue> Dictionary
		{
			get
			{
				return this.m_dictionary;
			}
		}

		public ReadOnlyDictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				if (this.m_keys == null)
				{
					this.m_keys = new ReadOnlyDictionary<TKey, TValue>.KeyCollection(this.m_dictionary.Keys);
				}
				return this.m_keys;
			}
		}

		public ReadOnlyDictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				if (this.m_values == null)
				{
					this.m_values = new ReadOnlyDictionary<TKey, TValue>.ValueCollection(this.m_dictionary.Values);
				}
				return this.m_values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.m_dictionary[key];
			}
		}

		public int Count
		{
			get
			{
				return this.m_dictionary.Count;
			}
		}

		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.m_dictionary = dictionary;
		}

		public bool ContainsKey(TKey key)
		{
			return this.m_dictionary.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.m_dictionary.TryGetValue(key, out value);
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.m_dictionary.Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.m_dictionary.CopyTo(array, arrayIndex);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.m_dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_dictionary.GetEnumerator();
		}

		private static bool IsCompatibleKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key is TKey;
		}

		void IDictionary.Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		void IDictionary.Clear()
		{
			throw new NotSupportedException();
		}

		bool IDictionary.Contains(object key)
		{
			return ReadOnlyDictionary<TKey, TValue>.IsCompatibleKey(key) && this.ContainsKey((TKey)((object)key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			IDictionary dictionary = this.m_dictionary as IDictionary;
			IDictionaryEnumerator result;
			if (dictionary != null)
			{
				result = dictionary.GetEnumerator();
			}
			else
			{
				result = new ReadOnlyDictionary<TKey, TValue>.DictionaryEnumerator(this.m_dictionary);
			}
			return result;
		}

		void IDictionary.Remove(object key)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException();
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException();
			}
			if (index < 0 || index > array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException();
			}
			KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
			if (array2 != null)
			{
				this.m_dictionary.CopyTo(array2, index);
			}
			else
			{
				DictionaryEntry[] array3 = array as DictionaryEntry[];
				if (array3 != null)
				{
					foreach (KeyValuePair<TKey, TValue> current in this.m_dictionary)
					{
						array3[index++] = new DictionaryEntry(current.Key, current.Value);
					}
				}
				else
				{
					object[] array4 = array as object[];
					if (array4 == null)
					{
						throw new ArgumentException();
					}
					try
					{
						foreach (KeyValuePair<TKey, TValue> current2 in this.m_dictionary)
						{
							array4[index++] = new KeyValuePair<TKey, TValue>(current2.Key, current2.Value);
						}
					}
					catch (ArrayTypeMismatchException)
					{
						throw new ArgumentException();
					}
				}
			}
		}
	}
}
