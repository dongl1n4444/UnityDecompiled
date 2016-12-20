namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    [Serializable, DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ICollection
    {
        private readonly IDictionary<TKey, TValue> m_dictionary;
        [NonSerialized]
        private KeyCollection<TKey, TValue> m_keys;
        [NonSerialized]
        private object m_syncRoot;
        [NonSerialized]
        private ValueCollection<TKey, TValue> m_values;

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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return (key is TKey);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
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

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
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
            if ((index < 0) || (index > array.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((array.Length - index) < this.Count)
            {
                throw new ArgumentException();
            }
            KeyValuePair<TKey, TValue>[] pairArray = array as KeyValuePair<TKey, TValue>[];
            if (pairArray != null)
            {
                this.m_dictionary.CopyTo(pairArray, index);
            }
            else
            {
                DictionaryEntry[] entryArray = array as DictionaryEntry[];
                if (entryArray != null)
                {
                    foreach (KeyValuePair<TKey, TValue> pair in this.m_dictionary)
                    {
                        entryArray[index++] = new DictionaryEntry(pair.Key, pair.Value);
                    }
                }
                else
                {
                    object[] objArray = array as object[];
                    if (objArray == null)
                    {
                        throw new ArgumentException();
                    }
                    try
                    {
                        foreach (KeyValuePair<TKey, TValue> pair2 in this.m_dictionary)
                        {
                            objArray[index++] = new KeyValuePair<TKey, TValue>(pair2.Key, pair2.Value);
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException();
                    }
                }
            }
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
            return (ReadOnlyDictionary<TKey, TValue>.IsCompatibleKey(key) && this.ContainsKey((TKey) key));
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            IDictionary dictionary = this.m_dictionary as IDictionary;
            if (dictionary != null)
            {
                return dictionary.GetEnumerator();
            }
            return new DictionaryEnumerator<TKey, TValue>(this.m_dictionary);
        }

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.m_dictionary.TryGetValue(key, out value);
        }

        public int Count
        {
            get
            {
                return this.m_dictionary.Count;
            }
        }

        protected IDictionary<TKey, TValue> Dictionary
        {
            get
            {
                return this.m_dictionary;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.m_dictionary[key];
            }
        }

        public KeyCollection<TKey, TValue> Keys
        {
            get
            {
                if (this.m_keys == null)
                {
                    this.m_keys = new KeyCollection<TKey, TValue>(this.m_dictionary.Keys);
                }
                return this.m_keys;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return true;
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
                    ICollection dictionary = this.m_dictionary as ICollection;
                    if (dictionary != null)
                    {
                        this.m_syncRoot = dictionary.SyncRoot;
                    }
                    else
                    {
                        Interlocked.CompareExchange<object>(ref this.m_syncRoot, new object(), null);
                    }
                }
                return this.m_syncRoot;
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

        object IDictionary.this[object key]
        {
            get
            {
                if (ReadOnlyDictionary<TKey, TValue>.IsCompatibleKey(key))
                {
                    return this[(TKey) key];
                }
                return null;
            }
            set
            {
                throw new NotSupportedException();
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

        public ValueCollection<TKey, TValue> Values
        {
            get
            {
                if (this.m_values == null)
                {
                    this.m_values = new ValueCollection<TKey, TValue>(this.m_dictionary.Values);
                }
                return this.m_values;
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
        {
            private readonly IDictionary<TKey, TValue> m_dictionary;
            private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;
            public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
            {
                this.m_dictionary = dictionary;
                this.m_enumerator = this.m_dictionary.GetEnumerator();
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(this.m_enumerator.Current.Key, this.m_enumerator.Current.Value);
                }
            }
            public object Key
            {
                get
                {
                    return this.m_enumerator.Current.Key;
                }
            }
            public object Value
            {
                get
                {
                    return this.m_enumerator.Current.Value;
                }
            }
            public object Current
            {
                get
                {
                    return this.Entry;
                }
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

        [Serializable, DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, ICollection, IEnumerable<TKey>, IEnumerable
        {
            private readonly ICollection<TKey> m_collection;
            [NonSerialized]
            private object m_syncRoot;

            internal KeyCollection(ICollection<TKey> collection)
            {
                if (collection == null)
                {
                    throw new ArgumentNullException("collection");
                }
                this.m_collection = collection;
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                this.m_collection.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return this.m_collection.GetEnumerator();
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

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ReadOnlyDictionaryHelpers.CopyToNonGenericICollectionHelper<TKey>(this.m_collection, array, index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_collection.GetEnumerator();
            }

            public int Count
            {
                get
                {
                    return this.m_collection.Count;
                }
            }

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
        }

        [Serializable, DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : ICollection<TValue>, ICollection, IEnumerable<TValue>, IEnumerable
        {
            private readonly ICollection<TValue> m_collection;
            [NonSerialized]
            private object m_syncRoot;

            internal ValueCollection(ICollection<TValue> collection)
            {
                if (collection == null)
                {
                    throw new ArgumentNullException("collection");
                }
                this.m_collection = collection;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                this.m_collection.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return this.m_collection.GetEnumerator();
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

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ReadOnlyDictionaryHelpers.CopyToNonGenericICollectionHelper<TValue>(this.m_collection, array, index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_collection.GetEnumerator();
            }

            public int Count
            {
                get
                {
                    return this.m_collection.Count;
                }
            }

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
        }
    }
}

