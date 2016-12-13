using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.Graphs
{
	public class SerializedType
	{
		private struct SerializedTypeData
		{
			public string typeName;

			public string genericTypeName;

			public bool isGeneric;
		}

		private static string StripTypeNameString(string str, int index)
		{
			int num = index + 1;
			while (num < str.Length && str[num] != ',' && str[num] != ']')
			{
				num++;
			}
			return str.Remove(index, num - index);
		}

		private static string StripAllFromTypeNameString(string str, string toStrip)
		{
			for (int num = str.IndexOf(toStrip); num != -1; num = str.IndexOf(toStrip, num))
			{
				str = SerializedType.StripTypeNameString(str, num);
			}
			return str;
		}

		private static string ToShortTypeName(Type t)
		{
			string text = t.AssemblyQualifiedName;
			string result;
			if (string.IsNullOrEmpty(text))
			{
				result = string.Empty;
			}
			else
			{
				text = SerializedType.StripAllFromTypeNameString(text, ", Version");
				text = SerializedType.StripAllFromTypeNameString(text, ", Culture");
				text = SerializedType.StripAllFromTypeNameString(text, ", PublicKeyToken");
				result = text;
			}
			return result;
		}

		private static string SafeTypeName(Type type)
		{
			return (type.FullName == null) ? null : type.FullName.Replace('+', '.');
		}

		private static SerializedType.SerializedTypeData SplitTypeString(string serializedTypeString)
		{
			if (string.IsNullOrEmpty(serializedTypeString))
			{
				throw new ArgumentException("Cannot parse serialized type string, it is empty.");
			}
			SerializedType.SerializedTypeData result;
			result.isGeneric = SerializedType.IsGeneric(serializedTypeString);
			result.typeName = serializedTypeString.Substring(0, serializedTypeString.IndexOf('#'));
			result.genericTypeName = serializedTypeString.Substring(result.typeName.Length + 1, serializedTypeString.IndexOf('#', result.typeName.Length + 1) - result.typeName.Length - 1);
			return result;
		}

		private static string ToString(SerializedType.SerializedTypeData data)
		{
			return string.Concat(new string[]
			{
				data.typeName,
				"#",
				data.genericTypeName,
				"#",
				(!data.isGeneric) ? "0" : "1"
			});
		}

		private static Type FromString(SerializedType.SerializedTypeData data)
		{
			return Type.GetType(data.typeName, true);
		}

		public static Type GenericType(Type t)
		{
			Type result;
			if (t.IsArray)
			{
				result = t.GetElementType();
			}
			else if (!t.IsGenericType)
			{
				result = t;
			}
			else
			{
				Type[] genericArguments = t.GetGenericArguments();
				if (genericArguments.Length != 1)
				{
					throw new ArgumentException("Internal error: got generic type with more than one generic argument.");
				}
				result = genericArguments[0];
			}
			return result;
		}

		public static bool IsListType(Type t)
		{
			return typeof(IList).IsAssignableFrom(t);
		}

		public static string GetFullName(Type t)
		{
			string result;
			if (!t.IsGenericType)
			{
				result = SerializedType.SafeTypeName(t);
			}
			else
			{
				if (t.GetGenericTypeDefinition() != typeof(List<>))
				{
					throw new ArgumentException("Internal error: got unsupported generic type");
				}
				result = string.Format("System.Collections.Generic.List<{0}>", SerializedType.SafeTypeName(t.GetGenericArguments()[0]));
			}
			return result;
		}

		public static string ToString(Type t)
		{
			SerializedType.SerializedTypeData data = default(SerializedType.SerializedTypeData);
			string result;
			if (t == null)
			{
				result = string.Empty;
			}
			else
			{
				data.typeName = string.Empty;
				data.isGeneric = t.ContainsGenericParameters;
				if (data.isGeneric && t.IsGenericType)
				{
					data.typeName = SerializedType.ToShortTypeName(t.GetGenericTypeDefinition());
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
					data.typeName = SerializedType.ToShortTypeName(t);
				}
				result = SerializedType.ToString(data);
			}
			return result;
		}

		public static Type FromString(string serializedTypeString)
		{
			Type result;
			if (string.IsNullOrEmpty(serializedTypeString) || SerializedType.IsGeneric(serializedTypeString))
			{
				result = null;
			}
			else
			{
				result = Type.GetType(SerializedType.SplitTypeString(serializedTypeString).typeName, true);
			}
			return result;
		}

		public static bool IsGeneric(string serializedTypeString)
		{
			return !string.IsNullOrEmpty(serializedTypeString) && serializedTypeString[serializedTypeString.Length - 1] == '1';
		}

		public static bool IsBaseTypeGeneric(string serializedTypeString)
		{
			bool result;
			if (string.IsNullOrEmpty(serializedTypeString))
			{
				result = false;
			}
			else
			{
				SerializedType.SerializedTypeData serializedTypeData = SerializedType.SplitTypeString(serializedTypeString);
				result = (serializedTypeData.isGeneric || serializedTypeData.genericTypeName != string.Empty);
			}
			return result;
		}

		public static string SetGenericArgumentType(string serializedTypeString, Type type)
		{
			if (SerializedType.IsGeneric(serializedTypeString))
			{
				SerializedType.SerializedTypeData data = SerializedType.SplitTypeString(serializedTypeString);
				data.genericTypeName = data.typeName;
				data.isGeneric = false;
				string typeName = data.typeName;
				if (typeName != null)
				{
					if (typeName == "T")
					{
						data.typeName = SerializedType.ToShortTypeName(type);
						goto IL_D9;
					}
					if (typeName == "T[]")
					{
						data.typeName = SerializedType.ToShortTypeName(type.MakeArrayType());
						goto IL_D9;
					}
				}
				data.typeName = SerializedType.ToShortTypeName(Type.GetType(data.typeName, true).GetGenericTypeDefinition().MakeGenericType(new Type[]
				{
					type
				}));
				IL_D9:
				return SerializedType.ToString(data);
			}
			if (SerializedType.IsBaseTypeGeneric(serializedTypeString))
			{
				throw new ArgumentException("Trying to set a different generic type. Reset old one first.");
			}
			throw new ArgumentException("Trying to set generic argument type for non generic type.");
		}

		public static string ResetGenericArgumentType(string serializedTypeString)
		{
			if (string.IsNullOrEmpty(serializedTypeString))
			{
				throw new ArgumentException("Cannot reset generic argument type for null type.");
			}
			SerializedType.SerializedTypeData data = SerializedType.SplitTypeString(serializedTypeString);
			if (string.IsNullOrEmpty(data.genericTypeName))
			{
				throw new ArgumentException("Cannot reset generic argument type, previous generic type unknown.");
			}
			data.typeName = data.genericTypeName;
			data.isGeneric = true;
			data.genericTypeName = string.Empty;
			return SerializedType.ToString(data);
		}

		public static bool CanAssignFromGenericType(string serializedTypeString, Type t)
		{
			SerializedType.SerializedTypeData data = SerializedType.SplitTypeString(serializedTypeString);
			bool result;
			if (!data.isGeneric)
			{
				result = false;
			}
			else if (t.IsGenericType)
			{
				if (data.typeName == "T" || data.typeName == "T[]")
				{
					result = false;
				}
				else
				{
					Type[] genericArguments = t.GetGenericArguments();
					result = (genericArguments.Length == 1 && !genericArguments[0].IsGenericType && t.GetGenericTypeDefinition() == SerializedType.FromString(data).GetGenericTypeDefinition());
				}
			}
			else
			{
				result = (data.typeName == "T" || data.typeName == "T[]");
			}
			return result;
		}
	}
}
