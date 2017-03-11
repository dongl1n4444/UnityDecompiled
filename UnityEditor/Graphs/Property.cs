namespace UnityEditor.Graphs
{
    using Microsoft.CSharp;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    [Serializable]
    public class Property
    {
        [CompilerGenerated]
        private static Func<char, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        internal const char kListSeparator = '↓';
        [NonSerialized]
        private string m_CachedLastDeserializedString;
        [NonSerialized]
        private object m_CachedLastDeserializedValue;
        [NonSerialized]
        private bool m_HaveCachedDeserializedResult;
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private List<UnityEngine.Object> m_RefValues;
        [SerializeField]
        private string m_TypeString;
        [SerializeField]
        private string m_Value;

        public Property()
        {
            this.m_TypeString = string.Empty;
            this.m_Value = string.Empty;
            this.m_Name = string.Empty;
            this.m_TypeString = string.Empty;
        }

        public Property(string typeString, string name)
        {
            this.m_TypeString = string.Empty;
            this.m_Value = string.Empty;
            this.m_Name = name;
            this.m_TypeString = typeString;
            if (!this.isGeneric)
            {
                this.value = TryGetDefaultValue(this.type);
            }
        }

        public Property(System.Type type, string name)
        {
            this.m_TypeString = string.Empty;
            this.m_Value = string.Empty;
            this.m_Name = name;
            this.type = type;
            if (!this.isGeneric)
            {
                this.value = TryGetDefaultValue(type);
            }
        }

        public void ChangeDataType(System.Type newDataType)
        {
            if (this.type != newDataType)
            {
                object obj2 = this.value;
                this.type = newDataType;
                this.m_Value = string.Empty;
                this.m_RefValues = null;
                this.m_HaveCachedDeserializedResult = false;
                this.m_CachedLastDeserializedString = string.Empty;
                this.m_CachedLastDeserializedValue = null;
                object obj3 = ConvertActualValueIfPossible(obj2, newDataType);
                if (((obj3 == null) && newDataType.IsValueType) && !this.isGeneric)
                {
                    this.value = TryGetDefaultValue(this.type);
                }
                else
                {
                    this.value = obj3;
                }
            }
        }

        public static bool ConvertableUnityObjects(System.Type t1, System.Type t2) => 
            ((typeof(UnityEngine.Component).IsAssignableFrom(t1) || typeof(GameObject).IsAssignableFrom(t1)) && (typeof(UnityEngine.Component).IsAssignableFrom(t2) || typeof(GameObject).IsAssignableFrom(t2)));

        public static object ConvertActualValueIfPossible(object value, System.Type toType)
        {
            if (value == null)
            {
                return value;
            }
            System.Type type = value.GetType();
            if (type == toType)
            {
                return value;
            }
            if (ConvertableUnityObjects(type, toType))
            {
                if (type == typeof(GameObject))
                {
                    return ((GameObject) value).GetComponent(toType);
                }
                if (toType == typeof(GameObject))
                {
                    return ((UnityEngine.Component) value).gameObject;
                }
                return ((UnityEngine.Component) value).GetComponent(toType);
            }
            if (toType.IsAssignableFrom(typeof(Vector3)))
            {
                if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                {
                    return ((UnityEngine.Component) value).transform.position;
                }
                if (typeof(GameObject).IsAssignableFrom(type))
                {
                    return ((GameObject) value).transform.position;
                }
            }
            if (toType == typeof(string))
            {
                return value.ToString();
            }
            try
            {
                return Convert.ChangeType(value, toType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private CodeExpression ConvertArrayToCodeExpression(object o)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            StringBuilder builder = new StringBuilder();
            IEnumerator enumerator = ((IEnumerable) o).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    if (builder.Length != 0)
                    {
                        builder.Append(',');
                    }
                    StringWriter writer = new StringWriter();
                    provider.GenerateCodeFromExpression(this.ConvertSingleValueToCodeExpression(current), writer, new CodeGeneratorOptions());
                    builder.AppendFormat(writer.ToString(), new object[0]);
                    writer.Close();
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
            return new CodeSnippetExpression($"new {SerializedType.GetFullName(this.type)} {{ {builder} }}");
        }

        private string ConvertFromListOrArray(object o)
        {
            TypeConverter converter = GetConverter(this.elementType);
            StringBuilder builder = new StringBuilder();
            IEnumerator enumerator = ((IEnumerable) o).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    builder.Append(converter.ConvertToString(current));
                    builder.Append('↓');
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
            return builder.ToString();
        }

        private string ConvertFromSingleValue(object o)
        {
            TypeConverter converter = GetConverter(this.type);
            if (!converter.IsValid(o))
            {
                throw new ArgumentException();
            }
            return converter.ConvertToString(o);
        }

        public static object ConvertFromString(System.Type toType, string str)
        {
            try
            {
                return GetConverter(toType).ConvertFromInvariantString(str);
            }
            catch
            {
                return null;
            }
        }

        private CodeExpression ConvertSingleValueToCodeExpression(object o)
        {
            if (IsPrimitive(o.GetType()))
            {
                return new CodePrimitiveExpression(o);
            }
            TypeConverter converter = GetConverter(o.GetType());
            if (!converter.CanConvertTo(typeof(CodeExpression)))
            {
                return this.GetDefaultCodeExpression();
            }
            return (converter.ConvertTo(o, typeof(CodeExpression)) as CodeExpression);
        }

        private object ConvertToListOrArray()
        {
            object[] args = new object[] { this.elementCount };
            object obj2 = Activator.CreateInstance(this.type, args);
            TypeConverter converter = GetConverter(this.elementType);
            int num = 0;
            if (this.type.IsArray)
            {
                Array array = (Array) obj2;
                if (!string.IsNullOrEmpty(this.m_Value))
                {
                    char[] separator = new char[] { '↓' };
                    foreach (string str in this.m_Value.Split(separator))
                    {
                        if (num == this.elementCount)
                        {
                            return obj2;
                        }
                        array.SetValue(converter.ConvertFromString(str), num++);
                    }
                }
                return obj2;
            }
            MethodInfo method = this.type.GetMethod("Add");
            if (!string.IsNullOrEmpty(this.m_Value))
            {
                char[] chArray2 = new char[] { '↓' };
                foreach (string str2 in this.m_Value.Split(chArray2))
                {
                    if (num++ == this.elementCount)
                    {
                        return obj2;
                    }
                    object[] parameters = new object[] { converter.ConvertFromString(str2) };
                    method.Invoke(obj2, parameters);
                }
            }
            return obj2;
        }

        private object ConvertToSingleValue()
        {
            TypeConverter converter = GetConverter(this.type);
            if ((typeof(TypeConverter) == converter.GetType()) || !converter.IsValid(this.m_Value))
            {
                return null;
            }
            return converter.ConvertFromString(this.m_Value);
        }

        private static TypeConverter GetConverter(System.Type t)
        {
            string name = t.Name;
            if (name != null)
            {
                int num;
                if (<>f__switch$map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(7) {
                        { 
                            "Vector2",
                            0
                        },
                        { 
                            "Vector3",
                            1
                        },
                        { 
                            "Vector4",
                            2
                        },
                        { 
                            "Color",
                            3
                        },
                        { 
                            "Quaternion",
                            4
                        },
                        { 
                            "Rect",
                            5
                        },
                        { 
                            "AnimationCurve",
                            6
                        }
                    };
                    <>f__switch$map0 = dictionary;
                }
                if (<>f__switch$map0.TryGetValue(name, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return new GenericFloatVarsTypeConverter(t, new string[] { "x", "y" });

                        case 1:
                            return new GenericFloatVarsTypeConverter(t, new string[] { "x", "y", "z" });

                        case 2:
                            return new GenericFloatVarsTypeConverter(t, new string[] { "x", "y", "z", "w" });

                        case 3:
                            return new GenericFloatVarsTypeConverter(t, new string[] { "r", "g", "b", "a" });

                        case 4:
                            return new GenericFloatVarsTypeConverter(t, new string[] { "x", "y", "z", "w" });

                        case 5:
                            return new GenericFloatVarsTypeConverter(t, new string[] { "x", "y", "width", "height" });

                        case 6:
                            return new AnimationCurveTypeConverter(t);
                    }
                }
            }
            if (t.BaseType == typeof(Enum))
            {
                return new EnumTypeConverter(t);
            }
            return TypeDescriptor.GetConverter(t);
        }

        private CodeExpression GetDefaultCodeExpression()
        {
            if (this.type.IsValueType)
            {
                return new CodeObjectCreateExpression(this.type, new CodeExpression[0]);
            }
            return new CodePrimitiveExpression(null);
        }

        private object GetSceneReferenceValue()
        {
            if (this.type.IsGenericType && (this.type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                object[] args = new object[] { this.refValues.Count };
                IList list = (IList) Activator.CreateInstance(this.type, args);
                foreach (UnityEngine.Object obj2 in this.refValues)
                {
                    list.Add((obj2 != null) ? obj2 : null);
                }
                return list;
            }
            if (this.type.IsArray)
            {
                object[] objArray2 = new object[] { this.refValues.Count };
                Array array = (Array) Activator.CreateInstance(this.type, objArray2);
                for (int i = 0; i < array.Length; i++)
                {
                    if (this.refValues[i] != null)
                    {
                        array.SetValue(this.refValues[i], i);
                    }
                }
                return array;
            }
            return ((this.refValues.Count != 0) ? this.refValues[0] : null);
        }

        private static bool IsPrimitive(System.Type t) => 
            (t.IsPrimitive || (t == typeof(string)));

        public static bool IsSceneReferenceType(System.Type t)
        {
            if (t.IsArray)
            {
                t = t.GetElementType();
            }
            if (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>)))
            {
                t = t.GetGenericArguments()[0];
            }
            return typeof(UnityEngine.Object).IsAssignableFrom(t);
        }

        public void ResetGenericArgumentType()
        {
            this.m_TypeString = SerializedType.ResetGenericArgumentType(this.m_TypeString);
        }

        public void SetGenericArgumentType(System.Type type)
        {
            if (this.isGeneric)
            {
                this.m_TypeString = SerializedType.SetGenericArgumentType(this.m_TypeString, type);
                this.value = TryGetDefaultValue(this.type);
            }
        }

        private void SetSceneReferenceValue(object o)
        {
            this.refValues.Clear();
            if (o != null)
            {
                if (this.type.IsArray || (this.type.IsGenericType && (this.type.GetGenericTypeDefinition() == typeof(List<>))))
                {
                    IEnumerator enumerator = ((IList) o).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            this.refValues.Add((UnityEngine.Object) current);
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
                    this.refValues.Add((UnityEngine.Object) o);
                }
            }
        }

        private static object TryGetDefaultValue(System.Type type) => 
            (!type.IsValueType ? null : Activator.CreateInstance(type));

        public static bool ValidPropertyType(System.Type type)
        {
            if (typeof(IList).IsAssignableFrom(type))
            {
                if (type.IsArray && (type.GetArrayRank() == 1))
                {
                    type = type.GetElementType();
                }
                else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            if (IsPrimitive(type))
            {
                return true;
            }
            TypeConverter converter = GetConverter(type);
            return ((((converter != null) && (converter.GetType() != typeof(TypeConverter))) && (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))) && converter.CanConvertTo(typeof(CodeExpression)));
        }

        public CodeExpression codeExpression
        {
            get
            {
                System.Type c = this.type;
                if (typeof(UnityEngine.Object).IsAssignableFrom(c))
                {
                    throw new ArgumentException("Trying to get a code expression for Object type");
                }
                object o = this.value;
                if (o == null)
                {
                    return this.GetDefaultCodeExpression();
                }
                return (!this.isIList ? this.ConvertSingleValueToCodeExpression(o) : this.ConvertArrayToCodeExpression(o));
            }
        }

        public int elementCount
        {
            get
            {
                if (!this.hasValue)
                {
                    return 0;
                }
                if (this.isSceneReferenceType)
                {
                    return this.refValues.Count;
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => t == '↓';
                }
                return Enumerable.Count<char>(this.stringValue, <>f__am$cache0);
            }
        }

        public System.Type elementType
        {
            get
            {
                if (this.isSceneReferenceType)
                {
                    if (this.type.IsArray)
                    {
                        return this.type.GetElementType();
                    }
                    if (this.type.IsGenericType && (this.type.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        return this.type.GetGenericArguments()[0];
                    }
                    return this.type;
                }
                return (!this.type.IsArray ? this.type.GetGenericArguments()[0] : this.type.GetElementType());
            }
        }

        public bool hasDefaultValue
        {
            get
            {
                object obj2 = this.value;
                return ((obj2 == null) || obj2.Equals(TryGetDefaultValue(this.type)));
            }
        }

        public bool hasValue =>
            (!this.isSceneReferenceType ? !string.IsNullOrEmpty(this.stringValue) : (this.refValues.Count != 0));

        public bool isGeneric =>
            SerializedType.IsGeneric(this.m_TypeString);

        public bool isIList =>
            typeof(IList).IsAssignableFrom(this.type);

        public bool isSceneReferenceType
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_TypeString))
                {
                    return false;
                }
                return (!this.isGeneric && IsSceneReferenceType(this.type));
            }
        }

        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }

        private List<UnityEngine.Object> refValues
        {
            get
            {
                List<UnityEngine.Object> refValues;
                if (this.m_RefValues != null)
                {
                    refValues = this.m_RefValues;
                }
                else
                {
                    refValues = this.m_RefValues = new List<UnityEngine.Object>();
                }
                return refValues;
            }
        }

        public string stringValue =>
            this.m_Value;

        public System.Type type
        {
            get => 
                SerializedType.FromString(this.m_TypeString);
            set
            {
                this.m_TypeString = SerializedType.ToString(value);
            }
        }

        public string typeString =>
            this.m_TypeString;

        public object value
        {
            get
            {
                if (!this.m_HaveCachedDeserializedResult || (this.m_CachedLastDeserializedString != this.m_Value))
                {
                    if (this.isGeneric)
                    {
                        this.m_CachedLastDeserializedValue = null;
                    }
                    else if (this.isSceneReferenceType)
                    {
                        this.m_CachedLastDeserializedValue = this.GetSceneReferenceValue();
                    }
                    else
                    {
                        this.m_CachedLastDeserializedValue = !this.isIList ? this.ConvertToSingleValue() : this.ConvertToListOrArray();
                    }
                    this.m_HaveCachedDeserializedResult = true;
                    this.m_CachedLastDeserializedString = this.m_Value;
                }
                return this.m_CachedLastDeserializedValue;
            }
            set
            {
                this.m_HaveCachedDeserializedResult = false;
                if (this.isSceneReferenceType)
                {
                    this.SetSceneReferenceValue(value);
                }
                else if (!this.isIList)
                {
                    this.m_Value = this.ConvertFromSingleValue(value);
                }
                else
                {
                    this.m_Value = (value != null) ? this.ConvertFromListOrArray(value) : string.Empty;
                }
            }
        }
    }
}

