namespace SimpleJson.Reflection
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [GeneratedCode("reflection-utils", "1.0.0")]
    internal class ReflectionUtils
    {
        private static readonly object[] EmptyObjects = new object[0];

        public static Attribute GetAttribute(MemberInfo info, Type type)
        {
            if (((info == null) || (type == null)) || !Attribute.IsDefined(info, type))
            {
                return null;
            }
            return Attribute.GetCustomAttribute(info, type);
        }

        public static Attribute GetAttribute(Type objectType, Type attributeType)
        {
            if (((objectType == null) || (attributeType == null)) || !Attribute.IsDefined(objectType, attributeType))
            {
                return null;
            }
            return Attribute.GetCustomAttribute(objectType, attributeType);
        }

        public static ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
        {
            <GetConstructorByReflection>c__AnonStorey0 storey = new <GetConstructorByReflection>c__AnonStorey0 {
                constructorInfo = constructorInfo
            };
            return new ConstructorDelegate(storey.<>m__0);
        }

        public static ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
        {
            ConstructorInfo constructorInfo = GetConstructorInfo(type, argsType);
            return ((constructorInfo != null) ? GetConstructorByReflection(constructorInfo) : null);
        }

        public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
        {
            IEnumerable<ConstructorInfo> constructors = GetConstructors(type);
            foreach (ConstructorInfo info in constructors)
            {
                ParameterInfo[] parameters = info.GetParameters();
                if (argsType.Length == parameters.Length)
                {
                    int index = 0;
                    bool flag = true;
                    foreach (ParameterInfo info2 in info.GetParameters())
                    {
                        if (info2.ParameterType != argsType[index])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(Type type) => 
            type.GetConstructors();

        public static ConstructorDelegate GetContructor(ConstructorInfo constructorInfo) => 
            GetConstructorByReflection(constructorInfo);

        public static ConstructorDelegate GetContructor(Type type, params Type[] argsType) => 
            GetConstructorByReflection(type, argsType);

        public static IEnumerable<FieldInfo> GetFields(Type type) => 
            type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        public static Type[] GetGenericTypeArguments(Type type) => 
            type.GetGenericArguments();

        public static GetDelegate GetGetMethod(FieldInfo fieldInfo) => 
            GetGetMethodByReflection(fieldInfo);

        public static GetDelegate GetGetMethod(PropertyInfo propertyInfo) => 
            GetGetMethodByReflection(propertyInfo);

        public static GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
        {
            <GetGetMethodByReflection>c__AnonStorey2 storey = new <GetGetMethodByReflection>c__AnonStorey2 {
                fieldInfo = fieldInfo
            };
            return new GetDelegate(storey.<>m__0);
        }

        public static GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
        {
            <GetGetMethodByReflection>c__AnonStorey1 storey = new <GetGetMethodByReflection>c__AnonStorey1 {
                methodInfo = GetGetterMethodInfo(propertyInfo)
            };
            return new GetDelegate(storey.<>m__0);
        }

        public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo) => 
            propertyInfo.GetGetMethod(true);

        public static IEnumerable<PropertyInfo> GetProperties(Type type) => 
            type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        public static SetDelegate GetSetMethod(FieldInfo fieldInfo) => 
            GetSetMethodByReflection(fieldInfo);

        public static SetDelegate GetSetMethod(PropertyInfo propertyInfo) => 
            GetSetMethodByReflection(propertyInfo);

        public static SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
        {
            <GetSetMethodByReflection>c__AnonStorey4 storey = new <GetSetMethodByReflection>c__AnonStorey4 {
                fieldInfo = fieldInfo
            };
            return new SetDelegate(storey.<>m__0);
        }

        public static SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
        {
            <GetSetMethodByReflection>c__AnonStorey3 storey = new <GetSetMethodByReflection>c__AnonStorey3 {
                methodInfo = GetSetterMethodInfo(propertyInfo)
            };
            return new SetDelegate(storey.<>m__0);
        }

        public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo) => 
            propertyInfo.GetSetMethod(true);

        public static bool IsAssignableFrom(Type type1, Type type2) => 
            type1.IsAssignableFrom(type2);

        public static bool IsNullableType(Type type) => 
            (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));

        public static bool IsTypeDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return true;
            }
            if (!type.IsGenericType)
            {
                return false;
            }
            return (type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        public static bool IsTypeGenericeCollectionInterface(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            return (((genericTypeDefinition == typeof(IList<>)) || (genericTypeDefinition == typeof(ICollection<>))) || (genericTypeDefinition == typeof(IEnumerable<>)));
        }

        public static bool IsValueType(Type type) => 
            type.IsValueType;

        public static object ToNullableType(object obj, Type nullableType) => 
            ((obj != null) ? Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture) : null);

        [CompilerGenerated]
        private sealed class <GetConstructorByReflection>c__AnonStorey0
        {
            internal ConstructorInfo constructorInfo;

            internal object <>m__0(object[] args) => 
                this.constructorInfo.Invoke(args);
        }

        [CompilerGenerated]
        private sealed class <GetGetMethodByReflection>c__AnonStorey1
        {
            internal MethodInfo methodInfo;

            internal object <>m__0(object source) => 
                this.methodInfo.Invoke(source, ReflectionUtils.EmptyObjects);
        }

        [CompilerGenerated]
        private sealed class <GetGetMethodByReflection>c__AnonStorey2
        {
            internal FieldInfo fieldInfo;

            internal object <>m__0(object source) => 
                this.fieldInfo.GetValue(source);
        }

        [CompilerGenerated]
        private sealed class <GetSetMethodByReflection>c__AnonStorey3
        {
            internal MethodInfo methodInfo;

            internal void <>m__0(object source, object value)
            {
                object[] parameters = new object[] { value };
                this.methodInfo.Invoke(source, parameters);
            }
        }

        [CompilerGenerated]
        private sealed class <GetSetMethodByReflection>c__AnonStorey4
        {
            internal FieldInfo fieldInfo;

            internal void <>m__0(object source, object value)
            {
                this.fieldInfo.SetValue(source, value);
            }
        }

        public delegate object ConstructorDelegate(params object[] args);

        public delegate object GetDelegate(object source);

        public delegate void SetDelegate(object source, object value);

        public sealed class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
        {
            private Dictionary<TKey, TValue> _dictionary;
            private readonly object _lock;
            private readonly ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;

            public ThreadSafeDictionary(ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
            {
                this._lock = new object();
                this._valueFactory = valueFactory;
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void Add(TKey key, TValue value)
            {
                throw new NotImplementedException();
            }

            private TValue AddValue(TKey key)
            {
                TValue local = this._valueFactory(key);
                object obj2 = this._lock;
                lock (obj2)
                {
                    if (this._dictionary == null)
                    {
                        this._dictionary = new Dictionary<TKey, TValue>();
                        this._dictionary[key] = local;
                    }
                    else
                    {
                        TValue local2;
                        if (this._dictionary.TryGetValue(key, out local2))
                        {
                            return local2;
                        }
                        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._dictionary) {
                            [key] = local
                        };
                        this._dictionary = dictionary;
                    }
                }
                return local;
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(TKey key) => 
                this._dictionary.ContainsKey(key);

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            private TValue Get(TKey key)
            {
                TValue local2;
                if (this._dictionary == null)
                {
                    return this.AddValue(key);
                }
                if (!this._dictionary.TryGetValue(key, out local2))
                {
                    return this.AddValue(key);
                }
                return local2;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => 
                this._dictionary.GetEnumerator();

            public bool Remove(TKey key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this._dictionary.GetEnumerator();

            public bool TryGetValue(TKey key, out TValue value)
            {
                value = this[key];
                return true;
            }

            public int Count =>
                this._dictionary.Count;

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public TValue this[TKey key]
            {
                get => 
                    this.Get(key);
                set
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<TKey> Keys =>
                this._dictionary.Keys;

            public ICollection<TValue> Values =>
                this._dictionary.Values;
        }

        public delegate TValue ThreadSafeDictionaryValueFactory<TKey, TValue>(TKey key);
    }
}

