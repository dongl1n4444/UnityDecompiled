namespace UnityScript.Lang
{
    using Boo.Lang;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections;

    [Serializable]
    public class UnityRuntimeServices
    {
        [NonSerialized]
        public static IEnumerator EmptyEnumerator;
        [NonSerialized]
        protected static Type EnumeratorType;
        [NonSerialized]
        public static readonly bool Initialized;

        private static void $static_initializer$()
        {
            EmptyEnumerator = new object[0].GetEnumerator();
            EnumeratorType = typeof(IEnumerator);
        }

        static UnityRuntimeServices()
        {
            $static_initializer$();
            RuntimeServices.RegisterExtensions(typeof(Extensions));
            Initialized = true;
        }

        public static IEnumerator GetEnumerator(object obj)
        {
            IEnumerator enumerator;
            if (IsValueTypeArray(obj) || (obj is UnityScript.Lang.Array))
            {
                if (!(obj is IList))
                {
                }
                return ((obj != null) ? new ListUpdateableEnumerator((IList) RuntimeServices.Coerce(obj, typeof(IList))) : EmptyEnumerator);
            }
            IEnumerable enumerable = obj as IEnumerable;
            return ((enumerable == null) ? (((enumerator = obj as IEnumerator) == null) ? RuntimeServices.GetEnumerable(obj).GetEnumerator() : enumerator) : enumerable.GetEnumerator());
        }

        public static object GetProperty(object target, string name)
        {
            if (!Initialized)
            {
                throw new AssertionFailedException("Initialized");
            }
            try
            {
                return RuntimeServices.GetProperty(target, name);
            }
            catch (MissingMemberException)
            {
                if (target.GetType().IsValueType)
                {
                    throw;
                }
                return ExpandoServices.GetExpandoProperty(target, name);
            }
        }

        public static Type GetTypeOf(object o)
        {
            return ((o != null) ? o.GetType() : null);
        }

        public static object Invoke(object target, string name, object[] args, Type scriptBaseType)
        {
            if (!Initialized)
            {
                throw new AssertionFailedException("Initialized");
            }
            object obj2 = RuntimeServices.Invoke(target, name, args);
            return ((obj2 != null) ? (IsGenerator(obj2) ? (target.GetType().IsSubclassOf(scriptBaseType) ? (!IsStaticMethod(target.GetType(), name, args) ? RuntimeServices.Invoke(target, "StartCoroutine", new object[] { obj2 }) : obj2) : obj2) : obj2) : null);
        }

        public static bool IsGenerator(object obj)
        {
            Type c = obj.GetType();
            return ((c != EnumeratorType) ? (!EnumeratorType.IsAssignableFrom(c) ? typeof(AbstractGenerator).IsAssignableFrom(c) : true) : true);
        }

        public static bool IsStaticMethod(Type type, string name, object[] args)
        {
            try
            {
                return type.GetMethod(name).IsStatic;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool IsValueTypeArray(object obj)
        {
            return ((obj is System.Array) ? obj.GetType().GetElementType().IsValueType : false);
        }

        public static void PropagateValueTypeChanges(ValueTypeChange[] changes)
        {
            int index = 0;
            ValueTypeChange[] changeArray = changes;
            int length = changeArray.Length;
            while (index < length)
            {
                if (!changeArray[index].Propagate())
                {
                    break;
                }
                index++;
            }
        }

        public static object SetProperty(object target, string name, object value)
        {
            if (!Initialized)
            {
                throw new AssertionFailedException("Initialized");
            }
            try
            {
                return RuntimeServices.SetProperty(target, name, value);
            }
            catch (MissingMemberException)
            {
                if (target.GetType().IsValueType)
                {
                    throw;
                }
                return ExpandoServices.SetExpandoProperty(target, name, value);
            }
        }

        public static void Update(IEnumerator e, object newValue)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e is ListUpdateableEnumerator)
            {
                ((ListUpdateableEnumerator) e).Update(newValue);
            }
        }

        [Serializable]
        public class MemberValueTypeChange : UnityRuntimeServices.ValueTypeChange
        {
            public string Member;

            public MemberValueTypeChange(object target, string member, object value) : base(target, value)
            {
                this.Member = member;
            }

            public override bool Propagate()
            {
                if (!this.IsValid)
                {
                    bool flag;
                    return flag;
                }
                try
                {
                    RuntimeServices.SetProperty(base.Target, this.Member, base.Value);
                }
                catch (MissingFieldException)
                {
                    return false;
                }
                return true;
            }
        }

        [Serializable]
        public class SliceValueTypeChange : UnityRuntimeServices.ValueTypeChange
        {
            public object Index;

            public SliceValueTypeChange(object target, object index, object value) : base(target, value)
            {
                this.Index = index;
            }

            public override bool Propagate()
            {
                bool flag;
                if (!this.IsValid)
                {
                    return flag;
                }
                IList target = base.Target as IList;
                if (target != null)
                {
                    target[RuntimeServices.UnboxInt32(this.Index)] = base.Value;
                    return flag;
                }
                try
                {
                    object[] args = new object[] { this.Index, base.Value };
                    RuntimeServices.SetSlice(base.Target, string.Empty, args);
                }
                catch (MissingFieldException)
                {
                    return false;
                }
                return true;
            }
        }

        [Serializable]
        public abstract class ValueTypeChange
        {
            public object Target;
            public object Value;

            public ValueTypeChange(object target, object value)
            {
                this.Target = target;
                this.Value = value;
            }

            public abstract override bool Propagate();

            public bool IsValid
            {
                get
                {
                    return (this.Value is ValueType);
                }
            }
        }
    }
}

