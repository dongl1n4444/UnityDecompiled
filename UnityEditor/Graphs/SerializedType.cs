namespace UnityEditor.Graphs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class SerializedType
    {
        public static bool CanAssignFromGenericType(string serializedTypeString, Type t)
        {
            SerializedTypeData data = SplitTypeString(serializedTypeString);
            if (!data.isGeneric)
            {
                return false;
            }
            if (t.IsGenericType)
            {
                if ((data.typeName == "T") || (data.typeName == "T[]"))
                {
                    return false;
                }
                Type[] genericArguments = t.GetGenericArguments();
                if (genericArguments.Length != 1)
                {
                    return false;
                }
                if (genericArguments[0].IsGenericType)
                {
                    return false;
                }
                return (t.GetGenericTypeDefinition() == FromString(data).GetGenericTypeDefinition());
            }
            return ((data.typeName == "T") || (data.typeName == "T[]"));
        }

        public static Type FromString(string serializedTypeString)
        {
            if (string.IsNullOrEmpty(serializedTypeString) || IsGeneric(serializedTypeString))
            {
                return null;
            }
            return Type.GetType(SplitTypeString(serializedTypeString).typeName, true);
        }

        private static Type FromString(SerializedTypeData data)
        {
            return Type.GetType(data.typeName, true);
        }

        public static Type GenericType(Type t)
        {
            if (t.IsArray)
            {
                return t.GetElementType();
            }
            if (!t.IsGenericType)
            {
                return t;
            }
            Type[] genericArguments = t.GetGenericArguments();
            if (genericArguments.Length != 1)
            {
                throw new ArgumentException("Internal error: got generic type with more than one generic argument.");
            }
            return genericArguments[0];
        }

        public static string GetFullName(Type t)
        {
            if (!t.IsGenericType)
            {
                return SafeTypeName(t);
            }
            if (t.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new ArgumentException("Internal error: got unsupported generic type");
            }
            return string.Format("System.Collections.Generic.List<{0}>", SafeTypeName(t.GetGenericArguments()[0]));
        }

        public static bool IsBaseTypeGeneric(string serializedTypeString)
        {
            if (string.IsNullOrEmpty(serializedTypeString))
            {
                return false;
            }
            SerializedTypeData data = SplitTypeString(serializedTypeString);
            return (data.isGeneric || (data.genericTypeName != string.Empty));
        }

        public static bool IsGeneric(string serializedTypeString)
        {
            if (string.IsNullOrEmpty(serializedTypeString))
            {
                return false;
            }
            return (serializedTypeString[serializedTypeString.Length - 1] == '1');
        }

        public static bool IsListType(Type t)
        {
            return typeof(IList).IsAssignableFrom(t);
        }

        public static string ResetGenericArgumentType(string serializedTypeString)
        {
            if (string.IsNullOrEmpty(serializedTypeString))
            {
                throw new ArgumentException("Cannot reset generic argument type for null type.");
            }
            SerializedTypeData data = SplitTypeString(serializedTypeString);
            if (string.IsNullOrEmpty(data.genericTypeName))
            {
                throw new ArgumentException("Cannot reset generic argument type, previous generic type unknown.");
            }
            data.typeName = data.genericTypeName;
            data.isGeneric = true;
            data.genericTypeName = string.Empty;
            return ToString(data);
        }

        private static string SafeTypeName(Type type)
        {
            return ((type.FullName == null) ? null : type.FullName.Replace('+', '.'));
        }

        public static string SetGenericArgumentType(string serializedTypeString, Type type)
        {
            if (!IsGeneric(serializedTypeString))
            {
                if (IsBaseTypeGeneric(serializedTypeString))
                {
                    throw new ArgumentException("Trying to set a different generic type. Reset old one first.");
                }
                throw new ArgumentException("Trying to set generic argument type for non generic type.");
            }
            SerializedTypeData data = SplitTypeString(serializedTypeString);
            data.genericTypeName = data.typeName;
            data.isGeneric = false;
            switch (data.typeName)
            {
                case "T":
                    data.typeName = ToShortTypeName(type);
                    break;

                case "T[]":
                    data.typeName = ToShortTypeName(type.MakeArrayType());
                    break;

                default:
                {
                    Type[] typeArguments = new Type[] { type };
                    data.typeName = ToShortTypeName(Type.GetType(data.typeName, true).GetGenericTypeDefinition().MakeGenericType(typeArguments));
                    break;
                }
            }
            return ToString(data);
        }

        private static SerializedTypeData SplitTypeString(string serializedTypeString)
        {
            SerializedTypeData data;
            if (string.IsNullOrEmpty(serializedTypeString))
            {
                throw new ArgumentException("Cannot parse serialized type string, it is empty.");
            }
            data.isGeneric = IsGeneric(serializedTypeString);
            data.typeName = serializedTypeString.Substring(0, serializedTypeString.IndexOf('#'));
            int index = serializedTypeString.IndexOf('#', data.typeName.Length + 1);
            data.genericTypeName = serializedTypeString.Substring(data.typeName.Length + 1, (index - data.typeName.Length) - 1);
            return data;
        }

        private static string StripAllFromTypeNameString(string str, string toStrip)
        {
            for (int i = str.IndexOf(toStrip); i != -1; i = str.IndexOf(toStrip, i))
            {
                str = StripTypeNameString(str, i);
            }
            return str;
        }

        private static string StripTypeNameString(string str, int index)
        {
            int num = index + 1;
            while (((num < str.Length) && (str[num] != ',')) && (str[num] != ']'))
            {
                num++;
            }
            return str.Remove(index, num - index);
        }

        private static string ToShortTypeName(Type t)
        {
            string assemblyQualifiedName = t.AssemblyQualifiedName;
            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                return string.Empty;
            }
            return StripAllFromTypeNameString(StripAllFromTypeNameString(StripAllFromTypeNameString(assemblyQualifiedName, ", Version"), ", Culture"), ", PublicKeyToken");
        }

        public static string ToString(Type t)
        {
            SerializedTypeData data = new SerializedTypeData();
            if (t == null)
            {
                return string.Empty;
            }
            data.typeName = string.Empty;
            data.isGeneric = t.ContainsGenericParameters;
            if (data.isGeneric && t.IsGenericType)
            {
                data.typeName = ToShortTypeName(t.GetGenericTypeDefinition());
            }
            else if (data.isGeneric && t.IsArray)
            {
                data.typeName = "T[]";
            }
            else if (data.isGeneric)
            {
                data.typeName = "T";
            }
            else
            {
                data.typeName = ToShortTypeName(t);
            }
            return ToString(data);
        }

        private static string ToString(SerializedTypeData data)
        {
            string[] textArray1 = new string[] { data.typeName, "#", data.genericTypeName, "#", !data.isGeneric ? "0" : "1" };
            return string.Concat(textArray1);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SerializedTypeData
        {
            public string typeName;
            public string genericTypeName;
            public bool isGeneric;
        }
    }
}

