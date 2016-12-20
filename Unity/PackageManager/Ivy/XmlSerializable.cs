namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Unity.DataContract;
    using Unity.PackageManager.IvyInternal;

    public class XmlSerializable : IXmlSerializable
    {
        private static Dictionary<string, KeyValuePair<Func<object, object>, Action<object, object>>> attributeMappings = new Dictionary<string, KeyValuePair<Func<object, object>, Action<object, object>>>();
        private static object cacheLock = new object();
        private static readonly PackageVersion compatibilityVersion = new PackageVersion("4.5.0b2");
        private static List<string> processed = new List<string>();
        private static Dictionary<Type, XmlSerializer> serializerCache = new Dictionary<Type, XmlSerializer>();
        private static Dictionary<string, Type> typeMappings = new Dictionary<string, Type>();

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        internal static XmlSerializer GetSerializer(Type type)
        {
            object cacheLock = XmlSerializable.cacheLock;
            lock (cacheLock)
            {
                if (!serializerCache.ContainsKey(type))
                {
                    serializerCache.Add(type, new XmlSerializer(type));
                }
                return serializerCache[type];
            }
        }

        private void ProcessType()
        {
            <ProcessType>c__AnonStorey3 storey = new <ProcessType>c__AnonStorey3 {
                type = base.GetType()
            };
            object processed = XmlSerializable.processed;
            lock (processed)
            {
                if (!XmlSerializable.processed.Contains(storey.type.FullName))
                {
                    string str = storey.type.FullName + ".";
                    MemberInfo[] infoArray2 = storey.type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    for (int i = 0; i < infoArray2.Length; i++)
                    {
                        <ProcessType>c__AnonStorey2 storey2 = new <ProcessType>c__AnonStorey2 {
                            memberInfo = infoArray2[i]
                        };
                        if ((storey2.memberInfo.MemberType == MemberTypes.Field) || (storey2.memberInfo.MemberType == MemberTypes.Property))
                        {
                            string key = null;
                            object[] customAttributes = storey2.memberInfo.GetCustomAttributes(false);
                            foreach (object obj3 in customAttributes)
                            {
                                if (!(obj3 is XmlIgnoreAttribute))
                                {
                                    if (obj3 is XmlElementAttribute)
                                    {
                                        key = ((XmlElementAttribute) obj3).ElementName;
                                        break;
                                    }
                                    if (obj3 is XmlAttributeAttribute)
                                    {
                                        key = ((XmlAttributeAttribute) obj3).AttributeName;
                                        break;
                                    }
                                }
                            }
                            if (key != null)
                            {
                                key = str + key;
                                if (!attributeMappings.ContainsKey(key))
                                {
                                    if (storey2.memberInfo.MemberType == MemberTypes.Field)
                                    {
                                        <ProcessType>c__AnonStorey0 storey3 = new <ProcessType>c__AnonStorey0 {
                                            field = storey.type.GetField(storey2.memberInfo.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                        };
                                        attributeMappings.Add(key, new KeyValuePair<Func<object, object>, Action<object, object>>(new Func<object, object>(storey3, (IntPtr) this.<>m__0), new Action<object, object>(storey3, (IntPtr) this.<>m__1)));
                                    }
                                    else if (storey2.memberInfo.MemberType == MemberTypes.Property)
                                    {
                                        <ProcessType>c__AnonStorey1 storey4 = new <ProcessType>c__AnonStorey1 {
                                            <>f__ref$3 = storey,
                                            <>f__ref$2 = storey2,
                                            prop = storey.type.GetProperty(storey2.memberInfo.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                        };
                                        attributeMappings.Add(key, new KeyValuePair<Func<object, object>, Action<object, object>>(new Func<object, object>(storey4, (IntPtr) this.<>m__0), new Action<object, object>(storey4, (IntPtr) this.<>m__1)));
                                    }
                                }
                            }
                        }
                    }
                    XmlSerializable.processed.Add(storey.type.FullName);
                }
            }
        }

        private void ProcessTypes()
        {
            object typeMappings = XmlSerializable.typeMappings;
            lock (typeMappings)
            {
                if (XmlSerializable.typeMappings.Count <= 0)
                {
                    Type[] types = Assembly.GetAssembly(base.GetType()).GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.Namespace == "Unity.PackageManager.IvyInternal")
                        {
                            object[] customAttributes = type.GetCustomAttributes(false);
                            foreach (object obj3 in customAttributes)
                            {
                                string key = null;
                                string str2 = null;
                                if (obj3 is XmlTypeAttribute)
                                {
                                    key = ((XmlTypeAttribute) obj3).TypeName;
                                    str2 = ((XmlTypeAttribute) obj3).Namespace;
                                }
                                else if (obj3 is XmlRootAttribute)
                                {
                                    key = ((XmlRootAttribute) obj3).ElementName;
                                    str2 = ((XmlRootAttribute) obj3).Namespace;
                                }
                                else
                                {
                                    continue;
                                }
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    key = str2 + "." + key;
                                }
                                XmlSerializable.typeMappings.Add(key, type);
                            }
                        }
                    }
                }
            }
        }

        public void ReadXml(XmlReader reader)
        {
            this.ProcessTypes();
            this.ProcessType();
            bool isEmptyElement = reader.IsEmptyElement;
            reader.MoveToContent();
            if (reader.HasAttributes)
            {
                string str = base.GetType().FullName + ".";
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    KeyValuePair<Func<object, object>, Action<object, object>> pair;
                    reader.MoveToAttribute(i);
                    string localName = reader.LocalName;
                    string str3 = reader.Value;
                    if (localName == "packageType")
                    {
                        switch (str3)
                        {
                            case "PlaybackEngines":
                                str3 = "PlaybackEngine";
                                break;

                            case "EditorExtension":
                                str3 = "UnityExtension";
                                break;
                        }
                    }
                    if (attributeMappings.TryGetValue(str + localName, out pair))
                    {
                        pair.Value.Invoke(this, str3);
                    }
                }
            }
            if (reader.IsStartElement() && !reader.IsEmptyElement)
            {
                reader.ReadStartElement();
            }
            while (reader.IsStartElement() && !isEmptyElement)
            {
                Type type;
                KeyValuePair<Func<object, object>, Action<object, object>> pair2;
                string str4 = reader.LocalName;
                string key = str4;
                if (!typeMappings.TryGetValue(key, out type))
                {
                    key = base.GetType().Name + "." + key;
                    if (!typeMappings.TryGetValue(key, out type))
                    {
                        reader.Skip();
                        continue;
                    }
                }
                object obj2 = GetSerializer(type).Deserialize(reader);
                key = base.GetType().FullName + "." + str4;
                if (attributeMappings.TryGetValue(key, out pair2))
                {
                    pair2.Value.Invoke(this, obj2);
                }
            }
            if (!isEmptyElement)
            {
                reader.ReadEndElement();
            }
            else
            {
                reader.ReadStartElement();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            this.ProcessTypes();
            this.ProcessType();
            Type type = base.GetType();
            string str = type.FullName + ".";
            MemberInfo[] members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<string> list = new List<string>();
            Dictionary<string, XmlAttributeAttribute> dictionary = new Dictionary<string, XmlAttributeAttribute>();
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
            foreach (MemberInfo info in members)
            {
                if ((info.MemberType == MemberTypes.Field) || (info.MemberType == MemberTypes.Property))
                {
                    string elementName = null;
                    object[] customAttributes = info.GetCustomAttributes(false);
                    foreach (object obj2 in customAttributes)
                    {
                        if (!(obj2 is XmlIgnoreAttribute))
                        {
                            if (obj2 is XmlElementAttribute)
                            {
                                XmlElementAttribute attribute = (XmlElementAttribute) obj2;
                                elementName = attribute.ElementName;
                                if (attribute.Order > 0)
                                {
                                    if (list.Count < attribute.Order)
                                    {
                                        list.AddRange(new string[attribute.Order - list.Count]);
                                    }
                                    list[attribute.Order - 1] = str + elementName;
                                }
                                else
                                {
                                    list.Add(str + elementName);
                                }
                                dictionary2.Add(str + elementName, elementName);
                                break;
                            }
                            if (obj2 is XmlAttributeAttribute)
                            {
                                XmlAttributeAttribute attribute2 = (XmlAttributeAttribute) obj2;
                                dictionary.Add(str + attribute2.AttributeName, attribute2);
                                object[] objArray3 = info.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                                if (objArray3.Length > 0)
                                {
                                    dictionary3.Add(str + attribute2.AttributeName, ((DefaultValueAttribute) objArray3[0]).Value);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, XmlAttributeAttribute> pair in dictionary)
            {
                if (!attributeMappings.ContainsKey(pair.Key))
                {
                    continue;
                }
                KeyValuePair<Func<object, object>, Action<object, object>> pair2 = attributeMappings[pair.Key];
                object obj3 = pair2.Key.Invoke(this);
                if (obj3 == null)
                {
                    continue;
                }
                string str3 = obj3.ToString();
                object obj4 = null;
                if (dictionary3.TryGetValue(pair.Key, out obj4) && (obj3 == obj4))
                {
                    continue;
                }
                if (obj3 is bool)
                {
                    if (!((bool) obj3))
                    {
                        continue;
                    }
                    str3 = str3.ToLower();
                }
                if ((obj3 is IvyPackageType) && (this is Unity.PackageManager.IvyInternal.IvyInfo))
                {
                    PackageVersion version = new PackageVersion(((Unity.PackageManager.IvyInternal.IvyInfo) this).UnityVersion);
                    if (version <= compatibilityVersion)
                    {
                        switch (((IvyPackageType) obj3))
                        {
                            case IvyPackageType.PlaybackEngine:
                                str3 = "PlaybackEngines";
                                break;

                            case IvyPackageType.UnityExtension:
                                goto Label_0301;
                        }
                    }
                }
                goto Label_030F;
            Label_0301:
                str3 = "EditorExtension";
            Label_030F:
                writer.WriteAttributeString((pair.Value.Namespace == null) ? "" : "e", pair.Value.AttributeName, pair.Value.Namespace, str3);
            }
            foreach (string str4 in list)
            {
                if (attributeMappings.ContainsKey(str4))
                {
                    KeyValuePair<Func<object, object>, Action<object, object>> pair3 = attributeMappings[str4];
                    object o = pair3.Key.Invoke(this);
                    if (o != null)
                    {
                        if (o is List<Unity.PackageManager.IvyInternal.IvyRepository>)
                        {
                            IEnumerator enumerator = (o as IList).GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    object current = enumerator.Current;
                                    GetSerializer(current.GetType()).Serialize(writer, current, IvyParser.Namespaces);
                                }
                            }
                            finally
                            {
                                IDisposable disposable = enumerator as IDisposable;
                                if (disposable != null)
                                {
                                    disposable.Dispose();
                                }
                            }
                        }
                        else
                        {
                            GetSerializer(o.GetType()).Serialize(writer, o, IvyParser.Namespaces);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessType>c__AnonStorey0
        {
            internal FieldInfo field;

            internal object <>m__0(object who)
            {
                return this.field.GetValue(who);
            }

            internal void <>m__1(object who, object what)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(this.field.FieldType);
                object obj2 = what;
                if ((obj2 != null) && converter.CanConvertFrom(obj2.GetType()))
                {
                    obj2 = converter.ConvertFrom(what);
                }
                this.field.SetValue(who, obj2);
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessType>c__AnonStorey1
        {
            internal XmlSerializable.<ProcessType>c__AnonStorey2 <>f__ref$2;
            internal XmlSerializable.<ProcessType>c__AnonStorey3 <>f__ref$3;
            internal PropertyInfo prop;

            internal object <>m__0(object who)
            {
                return this.prop.GetGetMethod(true).Invoke(who, null);
            }

            internal void <>m__1(object who, object what)
            {
                if (!(what is IList) && typeof(IList).IsAssignableFrom(this.prop.PropertyType))
                {
                    if (what != null)
                    {
                        IList list = (IList) this.prop.GetGetMethod(true).Invoke(who, null);
                        if ((list == null) && this.<>f__ref$2.memberInfo.Name.StartsWith("xml"))
                        {
                            PropertyInfo property = this.<>f__ref$3.type.GetProperty(this.<>f__ref$2.memberInfo.Name.Substring(3), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                            if (property != null)
                            {
                                list = (IList) property.GetGetMethod(true).Invoke(who, null);
                            }
                        }
                        if (list != null)
                        {
                            list.Add(what);
                        }
                    }
                }
                else
                {
                    object[] parameters = new object[] { what };
                    this.prop.GetSetMethod(true).Invoke(who, parameters);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessType>c__AnonStorey2
        {
            internal MemberInfo memberInfo;
        }

        [CompilerGenerated]
        private sealed class <ProcessType>c__AnonStorey3
        {
            internal Type type;
        }
    }
}

