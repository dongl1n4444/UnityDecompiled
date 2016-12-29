namespace UnityScript.Lang
{
    using Boo.Lang;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class Array : CollectionBase, ICoercible
    {
        public Array()
        {
        }

        public Array(IEnumerable collection)
        {
            if (collection is string)
            {
                this.Add(collection);
            }
            else
            {
                this.AddRange(collection);
            }
        }

        public Array(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Expected: (capacity >= 0)", "capacity");
            }
            this.EnsureCapacity(capacity);
        }

        public Array(params object[] items)
        {
            if ((items.Length == 1) && (items[0] is IEnumerable))
            {
                object obj1 = items[0];
                if (!(obj1 is IEnumerable))
                {
                }
                this.AddRange((IEnumerable) RuntimeServices.Coerce(obj1, typeof(IEnumerable)));
            }
            else
            {
                this.AddRange(items);
            }
        }

        public void Add(object value)
        {
            this.InnerList.Add(value);
        }

        public void Add(object value, params object[] items)
        {
            this.AddImpl(value, items);
        }

        private int AddImpl(object value, IEnumerable items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            this.InnerList.Add(value);
            IEnumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                this.InnerList.Add(current);
            }
            return this.InnerList.Count;
        }

        public void AddRange(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            IEnumerator enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                this.InnerList.Add(current);
            }
        }

        public void clear()
        {
            this.Clear();
        }

        public override object Coerce(Type toType) => 
            (!toType.IsArray ? this : this.ToBuiltin(toType.GetElementType()));

        public UnityScript.Lang.Array concat(ICollection value, params object[] items) => 
            this.ConcatImpl(value, items);

        public UnityScript.Lang.Array Concat(ICollection value, params object[] items) => 
            this.ConcatImpl(value, items);

        private UnityScript.Lang.Array ConcatImpl(ICollection value, IEnumerable items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            UnityScript.Lang.Array array = new UnityScript.Lang.Array(this.InnerList);
            array.InnerList.AddRange(value);
            IEnumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (!(current is ICollection))
                {
                }
                ICollection c = (ICollection) RuntimeServices.Coerce(current, typeof(ICollection));
                array.InnerList.AddRange(c);
            }
            return array;
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity >= this.Count)
            {
                int num2 = 0;
                int num3 = capacity - this.Count;
                if (num3 < 0)
                {
                    throw new ArgumentOutOfRangeException("max");
                }
                while (num2 < num3)
                {
                    int num = num2;
                    num2++;
                    this.InnerList.Add(null);
                }
            }
        }

        public string join(string seperator) => 
            Builtins.join(this.InnerList, seperator);

        public string Join(string seperator) => 
            Builtins.join(this.InnerList, seperator);

        private int NormalizeIndex(int index) => 
            ((index < 0) ? (index + this.InnerList.Count) : index);

        protected override void OnValidate(object newValue)
        {
        }

        public static implicit operator UnityScript.Lang.Array(System.Array a) => 
            ((a != null) ? new UnityScript.Lang.Array(a as IEnumerable) : null);

        public static implicit operator UnityScript.Lang.Array(IEnumerable e) => 
            ((e != null) ? new UnityScript.Lang.Array(e) : null);

        public object pop() => 
            this.Pop();

        public object Pop()
        {
            int index = this.InnerList.Count - 1;
            object obj2 = this.InnerList[index];
            this.InnerList.RemoveAt(index);
            return obj2;
        }

        public int push(object value)
        {
            this.InnerList.Add(value);
            return this.InnerList.Count;
        }

        public int push(object value, params object[] items) => 
            this.AddImpl(value, items);

        public int Push(object value) => 
            this.push(value);

        public int Push(object value, params object[] items) => 
            this.AddImpl(value, items);

        public void remove(object obj)
        {
            this.Remove(obj);
        }

        public void Remove(object obj)
        {
            this.InnerList.Remove(obj);
        }

        public UnityScript.Lang.Array reverse()
        {
            this.Reverse();
            return this;
        }

        public UnityScript.Lang.Array Reverse()
        {
            this.InnerList.Reverse();
            return this;
        }

        public object shift() => 
            this.Shift();

        public object Shift()
        {
            int num = 0;
            object obj2 = this.InnerList[num];
            this.InnerList.RemoveAt(0);
            return obj2;
        }

        public UnityScript.Lang.Array slice(int start) => 
            this.Slice(start);

        public UnityScript.Lang.Array slice(int start, int end) => 
            this.Slice(start, end);

        public UnityScript.Lang.Array Slice(int start) => 
            this.Slice(start, this.InnerList.Count);

        public UnityScript.Lang.Array Slice(int start, int end)
        {
            int index = this.NormalizeIndex(start);
            int num2 = this.NormalizeIndex(end);
            return new UnityScript.Lang.Array(this.InnerList.GetRange(index, num2 - index));
        }

        public UnityScript.Lang.Array sort()
        {
            this.Sort();
            return this;
        }

        public void sort(Comparison comparison)
        {
            this.Sort(comparison);
        }

        public UnityScript.Lang.Array Sort()
        {
            this.InnerList.Sort();
            return this;
        }

        public void Sort(Comparison comparison)
        {
            this.InnerList.Sort(new ComparisonComparer(comparison));
        }

        public void splice(int index, int howmany, params object[] items)
        {
            this.SpliceImpl(index, howmany, items);
        }

        public void Splice(int index, int howmany, params object[] items)
        {
            this.SpliceImpl(index, howmany, items);
        }

        private void SpliceImpl(int index, int howmany, IEnumerable items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            if (howmany != 0)
            {
                this.InnerList.RemoveRange(index, howmany);
            }
            this.InnerList.InsertRange(index, (ICollection) items);
        }

        public System.Array ToBuiltin(Type type) => 
            this.InnerList.ToArray(type);

        public string toString() => 
            this.ToString();

        public override string ToString() => 
            this.Join(",");

        public int unshift(object value, params object[] items) => 
            this.UnshiftImpl(value, items);

        public int Unshift(object value, params object[] items) => 
            this.UnshiftImpl(value, items);

        private int UnshiftImpl(object value, IEnumerable items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            this.InnerList.InsertRange(0, (ICollection) items);
            this.InnerList.Insert(0, value);
            return this.InnerList.Count;
        }

        public object this[int index]
        {
            get => 
                this.InnerList[index];
            set
            {
                this.EnsureCapacity(index + 1);
                this.InnerList[index] = value;
            }
        }

        public int length
        {
            get => 
                this.Count;
            set
            {
                this.EnsureCapacity(value);
                if (value < this.Count)
                {
                    this.InnerList.RemoveRange(value, this.InnerList.Count - value);
                }
            }
        }

        [Serializable, CompilerGenerated]
        public delegate int Comparison(object lhs, object rhs);

        [Serializable]
        private class ComparisonComparer : IComparer
        {
            protected UnityScript.Lang.Array.Comparison _comparison;

            public ComparisonComparer(UnityScript.Lang.Array.Comparison comparison)
            {
                this._comparison = comparison;
            }

            public override int Compare(object lhs, object rhs) => 
                this._comparison(lhs, rhs);
        }
    }
}

