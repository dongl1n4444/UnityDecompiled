namespace SimpleJson
{
    using SimpleJson.Reflection;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [GeneratedCode("simple-json", "1.0.0")]
    internal class PocoJsonSerializerStrategy : IJsonSerializerStrategy
    {
        internal static readonly Type[] ArrayConstructorParameterTypes = new Type[] { typeof(int) };
        internal IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;
        internal static readonly Type[] EmptyTypes = new Type[0];
        internal IDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>> GetCache;
        private static readonly string[] Iso8601Format = new string[] { @"yyyy-MM-dd\THH:mm:ss.FFFFFFF\Z", @"yyyy-MM-dd\THH:mm:ss\Z", @"yyyy-MM-dd\THH:mm:ssK" };
        internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;

        public PocoJsonSerializerStrategy()
        {
            this.ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, ReflectionUtils.ConstructorDelegate>(this.ContructorDelegateFactory));
            this.GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(this.GetterValueFactory));
            this.SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(this.SetterValueFactory));
        }

        internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key) => 
            ReflectionUtils.GetContructor(key, !key.IsArray ? EmptyTypes : ArrayConstructorParameterTypes);

        public virtual object DeserializeObject(object value, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            string str = value as string;
            if ((type == typeof(Guid)) && string.IsNullOrEmpty(str))
            {
                return new Guid();
            }
            if (value == null)
            {
                return null;
            }
            object source = null;
            if (str != null)
            {
                if (str.Length != 0)
                {
                    if ((type == typeof(DateTime)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(DateTime))))
                    {
                        return DateTime.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    if ((type == typeof(DateTimeOffset)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset))))
                    {
                        return DateTimeOffset.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    if ((type == typeof(Guid)) || (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid))))
                    {
                        return new Guid(str);
                    }
                    return str;
                }
                if (type == typeof(Guid))
                {
                    source = new Guid();
                }
                else if (ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid)))
                {
                    source = null;
                }
                else
                {
                    source = str;
                }
                if (!ReflectionUtils.IsNullableType(type) && (Nullable.GetUnderlyingType(type) == typeof(Guid)))
                {
                    return str;
                }
            }
            else if (value is bool)
            {
                return value;
            }
            bool flag = value is long;
            bool flag2 = value is double;
            if ((flag && (type == typeof(long))) || (flag2 && (type == typeof(double))))
            {
                return value;
            }
            if ((flag2 && (type != typeof(double))) || (flag && (type != typeof(long))))
            {
                source = !typeof(IConvertible).IsAssignableFrom(type) ? value : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else
            {
                IDictionary<string, object> dictionary = value as IDictionary<string, object>;
                if (dictionary != null)
                {
                    IDictionary<string, object> dictionary2 = dictionary;
                    if (ReflectionUtils.IsTypeDictionary(type))
                    {
                        Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
                        Type type2 = genericTypeArguments[0];
                        Type type3 = genericTypeArguments[1];
                        Type[] typeArguments = new Type[] { type2, type3 };
                        Type type4 = typeof(Dictionary<,>).MakeGenericType(typeArguments);
                        IDictionary dictionary3 = (IDictionary) this.ConstructorCache[type4](null);
                        foreach (KeyValuePair<string, object> pair in dictionary2)
                        {
                            dictionary3.Add(pair.Key, this.DeserializeObject(pair.Value, type3));
                        }
                        return dictionary3;
                    }
                    if (type == typeof(object))
                    {
                        return value;
                    }
                    source = this.ConstructorCache[type](null);
                    foreach (KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> pair2 in this.SetCache[type])
                    {
                        object obj4;
                        if (dictionary2.TryGetValue(pair2.Key, out obj4))
                        {
                            obj4 = this.DeserializeObject(obj4, pair2.Value.Key);
                            pair2.Value.Value(source, obj4);
                        }
                    }
                    return source;
                }
                IList<object> list = value as IList<object>;
                if (list == null)
                {
                    return source;
                }
                IList<object> list2 = list;
                IList list3 = null;
                if (type.IsArray)
                {
                    object[] args = new object[] { list2.Count };
                    list3 = (IList) this.ConstructorCache[type](args);
                    int num = 0;
                    foreach (object obj5 in list2)
                    {
                        list3[num++] = this.DeserializeObject(obj5, type.GetElementType());
                    }
                }
                else if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type))
                {
                    Type type5 = ReflectionUtils.GetGenericTypeArguments(type)[0];
                    Type[] typeArray2 = new Type[] { type5 };
                    Type type6 = typeof(List<>).MakeGenericType(typeArray2);
                    object[] objArray2 = new object[] { list2.Count };
                    list3 = (IList) this.ConstructorCache[type6](objArray2);
                    foreach (object obj6 in list2)
                    {
                        list3.Add(this.DeserializeObject(obj6, type5));
                    }
                }
                return list3;
            }
            if (ReflectionUtils.IsNullableType(type))
            {
                return ReflectionUtils.ToNullableType(source, type);
            }
            return source;
        }

        internal virtual IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            IDictionary<string, ReflectionUtils.GetDelegate> dictionary = new Dictionary<string, ReflectionUtils.GetDelegate>();
            foreach (PropertyInfo info in ReflectionUtils.GetProperties(type))
            {
                if (info.CanRead)
                {
                    MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(info);
                    if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
                    {
                        dictionary[this.MapClrMemberNameToJsonFieldName(info.Name)] = ReflectionUtils.GetGetMethod(info);
                    }
                }
            }
            foreach (FieldInfo info3 in ReflectionUtils.GetFields(type))
            {
                if (!info3.IsStatic && info3.IsPublic)
                {
                    dictionary[this.MapClrMemberNameToJsonFieldName(info3.Name)] = ReflectionUtils.GetGetMethod(info3);
                }
            }
            return dictionary;
        }

        protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName) => 
            clrPropertyName;

        protected virtual object SerializeEnum(Enum p) => 
            Convert.ToDouble(p, CultureInfo.InvariantCulture);

        internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
            foreach (PropertyInfo info in ReflectionUtils.GetProperties(type))
            {
                if (info.CanWrite)
                {
                    MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(info);
                    if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
                    {
                        dictionary[this.MapClrMemberNameToJsonFieldName(info.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(info.PropertyType, ReflectionUtils.GetSetMethod(info));
                    }
                }
            }
            foreach (FieldInfo info3 in ReflectionUtils.GetFields(type))
            {
                if ((!info3.IsInitOnly && !info3.IsStatic) && info3.IsPublic)
                {
                    dictionary[this.MapClrMemberNameToJsonFieldName(info3.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(info3.FieldType, ReflectionUtils.GetSetMethod(info3));
                }
            }
            return dictionary;
        }

        protected virtual bool TrySerializeKnownTypes(object input, out object output)
        {
            bool flag = true;
            if (input is DateTime)
            {
                DateTime time = (DateTime) input;
                output = time.ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
                return flag;
            }
            if (input is DateTimeOffset)
            {
                DateTimeOffset offset = (DateTimeOffset) input;
                output = offset.ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
                return flag;
            }
            if (input is Guid)
            {
                output = ((Guid) input).ToString("D");
                return flag;
            }
            if (input is Uri)
            {
                output = input.ToString();
                return flag;
            }
            Enum p = input as Enum;
            if (p != 0)
            {
                output = this.SerializeEnum(p);
                return flag;
            }
            flag = false;
            output = null;
            return flag;
        }

        public virtual bool TrySerializeNonPrimitiveObject(object input, out object output) => 
            (this.TrySerializeKnownTypes(input, out output) || this.TrySerializeUnknownTypes(input, out output));

        protected virtual bool TrySerializeUnknownTypes(object input, out object output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            output = null;
            Type type = input.GetType();
            if (type.FullName == null)
            {
                return false;
            }
            IDictionary<string, object> dictionary = new JsonObject();
            IDictionary<string, ReflectionUtils.GetDelegate> dictionary2 = this.GetCache[type];
            foreach (KeyValuePair<string, ReflectionUtils.GetDelegate> pair in dictionary2)
            {
                if (pair.Value != null)
                {
                    string key = this.MapClrMemberNameToJsonFieldName(pair.Key);
                    dictionary.Add(key, pair.Value(input));
                }
            }
            output = dictionary;
            return true;
        }
    }
}

