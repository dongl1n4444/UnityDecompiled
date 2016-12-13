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
using System.Text;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[Serializable]
	public class Property
	{
		internal const char kListSeparator = '↓';

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private string m_TypeString = string.Empty;

		[SerializeField]
		private string m_Value = string.Empty;

		[SerializeField]
		private List<UnityEngine.Object> m_RefValues;

		[NonSerialized]
		private bool m_HaveCachedDeserializedResult;

		[NonSerialized]
		private string m_CachedLastDeserializedString;

		[NonSerialized]
		private object m_CachedLastDeserializedValue;

		public Type type
		{
			get
			{
				return SerializedType.FromString(this.m_TypeString);
			}
			set
			{
				this.m_TypeString = SerializedType.ToString(value);
			}
		}

		public string typeString
		{
			get
			{
				return this.m_TypeString;
			}
		}

		public bool isGeneric
		{
			get
			{
				return SerializedType.IsGeneric(this.m_TypeString);
			}
		}

		private List<UnityEngine.Object> refValues
		{
			get
			{
				List<UnityEngine.Object> arg_1C_0;
				if ((arg_1C_0 = this.m_RefValues) == null)
				{
					arg_1C_0 = (this.m_RefValues = new List<UnityEngine.Object>());
				}
				return arg_1C_0;
			}
		}

		public bool hasValue
		{
			get
			{
				return (!this.isSceneReferenceType) ? (!string.IsNullOrEmpty(this.stringValue)) : (this.refValues.Count != 0);
			}
		}

		public bool hasDefaultValue
		{
			get
			{
				object value = this.value;
				return value == null || value.Equals(Property.TryGetDefaultValue(this.type));
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public bool isIList
		{
			get
			{
				return typeof(IList).IsAssignableFrom(this.type);
			}
		}

		public bool isSceneReferenceType
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_TypeString) && !this.isGeneric && Property.IsSceneReferenceType(this.type);
			}
		}

		public int elementCount
		{
			get
			{
				int result;
				if (!this.hasValue)
				{
					result = 0;
				}
				else if (this.isSceneReferenceType)
				{
					result = this.refValues.Count;
				}
				else
				{
					result = this.stringValue.Count((char t) => t == '↓');
				}
				return result;
			}
		}

		public Type elementType
		{
			get
			{
				Type result;
				if (this.isSceneReferenceType)
				{
					if (this.type.IsArray)
					{
						result = this.type.GetElementType();
					}
					else if (this.type.IsGenericType && this.type.GetGenericTypeDefinition() == typeof(List<>))
					{
						result = this.type.GetGenericArguments()[0];
					}
					else
					{
						result = this.type;
					}
				}
				else
				{
					result = ((!this.type.IsArray) ? this.type.GetGenericArguments()[0] : this.type.GetElementType());
				}
				return result;
			}
		}

		public string stringValue
		{
			get
			{
				return this.m_Value;
			}
		}

		public CodeExpression codeExpression
		{
			get
			{
				Type type = this.type;
				if (typeof(UnityEngine.Object).IsAssignableFrom(type))
				{
					throw new ArgumentException("Trying to get a code expression for Object type");
				}
				object value = this.value;
				CodeExpression result;
				if (value == null)
				{
					result = this.GetDefaultCodeExpression();
				}
				else
				{
					result = ((!this.isIList) ? this.ConvertSingleValueToCodeExpression(value) : this.ConvertArrayToCodeExpression(value));
				}
				return result;
			}
		}

		public object value
		{
			get
			{
				object cachedLastDeserializedValue;
				if (this.m_HaveCachedDeserializedResult && this.m_CachedLastDeserializedString == this.m_Value)
				{
					cachedLastDeserializedValue = this.m_CachedLastDeserializedValue;
				}
				else
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
						this.m_CachedLastDeserializedValue = ((!this.isIList) ? this.ConvertToSingleValue() : this.ConvertToListOrArray());
					}
					this.m_HaveCachedDeserializedResult = true;
					this.m_CachedLastDeserializedString = this.m_Value;
					cachedLastDeserializedValue = this.m_CachedLastDeserializedValue;
				}
				return cachedLastDeserializedValue;
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
					this.m_Value = ((value != null) ? this.ConvertFromListOrArray(value) : string.Empty);
				}
			}
		}

		public Property()
		{
			this.m_Name = string.Empty;
			this.m_TypeString = string.Empty;
		}

		public Property(string typeString, string name)
		{
			this.m_Name = name;
			this.m_TypeString = typeString;
			if (!this.isGeneric)
			{
				this.value = Property.TryGetDefaultValue(this.type);
			}
		}

		public Property(Type type, string name)
		{
			this.m_Name = name;
			this.type = type;
			if (!this.isGeneric)
			{
				this.value = Property.TryGetDefaultValue(type);
			}
		}

		public static bool IsSceneReferenceType(Type t)
		{
			if (t.IsArray)
			{
				t = t.GetElementType();
			}
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
			{
				t = t.GetGenericArguments()[0];
			}
			return typeof(UnityEngine.Object).IsAssignableFrom(t);
		}

		public void SetGenericArgumentType(Type type)
		{
			if (this.isGeneric)
			{
				this.m_TypeString = SerializedType.SetGenericArgumentType(this.m_TypeString, type);
				this.value = Property.TryGetDefaultValue(this.type);
			}
		}

		public void ResetGenericArgumentType()
		{
			this.m_TypeString = SerializedType.ResetGenericArgumentType(this.m_TypeString);
		}

		private static object TryGetDefaultValue(Type type)
		{
			return (!type.IsValueType) ? null : Activator.CreateInstance(type);
		}

		private object GetSceneReferenceValue()
		{
			object result;
			if (this.type.IsGenericType && this.type.GetGenericTypeDefinition() == typeof(List<>))
			{
				IList list = (IList)Activator.CreateInstance(this.type, new object[]
				{
					this.refValues.Count
				});
				foreach (UnityEngine.Object current in this.refValues)
				{
					list.Add((!(current == null)) ? current : null);
				}
				result = list;
			}
			else if (this.type.IsArray)
			{
				Array array = (Array)Activator.CreateInstance(this.type, new object[]
				{
					this.refValues.Count
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (this.refValues[i] != null)
					{
						array.SetValue(this.refValues[i], i);
					}
				}
				result = array;
			}
			else
			{
				result = ((this.refValues.Count != 0) ? this.refValues[0] : null);
			}
			return result;
		}

		private void SetSceneReferenceValue(object o)
		{
			this.refValues.Clear();
			if (o != null)
			{
				if (this.type.IsArray || (this.type.IsGenericType && this.type.GetGenericTypeDefinition() == typeof(List<>)))
				{
					IEnumerator enumerator = ((IList)o).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object current = enumerator.Current;
							this.refValues.Add((UnityEngine.Object)current);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
				else
				{
					this.refValues.Add((UnityEngine.Object)o);
				}
			}
		}

		private object ConvertToListOrArray()
		{
			object obj = Activator.CreateInstance(this.type, new object[]
			{
				this.elementCount
			});
			TypeConverter converter = Property.GetConverter(this.elementType);
			int num = 0;
			if (this.type.IsArray)
			{
				Array array = (Array)obj;
				if (!string.IsNullOrEmpty(this.m_Value))
				{
					string[] array2 = this.m_Value.Split(new char[]
					{
						'↓'
					});
					for (int i = 0; i < array2.Length; i++)
					{
						string text = array2[i];
						if (num == this.elementCount)
						{
							break;
						}
						array.SetValue(converter.ConvertFromString(text), num++);
					}
				}
			}
			else
			{
				MethodInfo method = this.type.GetMethod("Add");
				if (!string.IsNullOrEmpty(this.m_Value))
				{
					string[] array3 = this.m_Value.Split(new char[]
					{
						'↓'
					});
					for (int j = 0; j < array3.Length; j++)
					{
						string text2 = array3[j];
						if (num++ == this.elementCount)
						{
							break;
						}
						method.Invoke(obj, new object[]
						{
							converter.ConvertFromString(text2)
						});
					}
				}
			}
			return obj;
		}

		private object ConvertToSingleValue()
		{
			TypeConverter converter = Property.GetConverter(this.type);
			object result;
			if (typeof(TypeConverter) == converter.GetType() || !converter.IsValid(this.m_Value))
			{
				result = null;
			}
			else
			{
				result = converter.ConvertFromString(this.m_Value);
			}
			return result;
		}

		private string ConvertFromSingleValue(object o)
		{
			TypeConverter converter = Property.GetConverter(this.type);
			if (!converter.IsValid(o))
			{
				throw new ArgumentException();
			}
			return converter.ConvertToString(o);
		}

		private string ConvertFromListOrArray(object o)
		{
			TypeConverter converter = Property.GetConverter(this.elementType);
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerator enumerator = ((IEnumerable)o).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					stringBuilder.Append(converter.ConvertToString(current));
					stringBuilder.Append('↓');
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return stringBuilder.ToString();
		}

		private CodeExpression ConvertArrayToCodeExpression(object o)
		{
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerator enumerator = ((IEnumerable)o).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(',');
					}
					StringWriter stringWriter = new StringWriter();
					cSharpCodeProvider.GenerateCodeFromExpression(this.ConvertSingleValueToCodeExpression(current), stringWriter, new CodeGeneratorOptions());
					stringBuilder.AppendFormat(stringWriter.ToString(), new object[0]);
					stringWriter.Close();
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return new CodeSnippetExpression(string.Format("new {0} {{ {1} }}", SerializedType.GetFullName(this.type), stringBuilder));
		}

		private CodeExpression ConvertSingleValueToCodeExpression(object o)
		{
			CodeExpression result;
			if (Property.IsPrimitive(o.GetType()))
			{
				result = new CodePrimitiveExpression(o);
			}
			else
			{
				TypeConverter converter = Property.GetConverter(o.GetType());
				if (!converter.CanConvertTo(typeof(CodeExpression)))
				{
					result = this.GetDefaultCodeExpression();
				}
				else
				{
					result = (converter.ConvertTo(o, typeof(CodeExpression)) as CodeExpression);
				}
			}
			return result;
		}

		private CodeExpression GetDefaultCodeExpression()
		{
			CodeExpression result;
			if (this.type.IsValueType)
			{
				result = new CodeObjectCreateExpression(this.type, new CodeExpression[0]);
			}
			else
			{
				result = new CodePrimitiveExpression(null);
			}
			return result;
		}

		private static bool IsPrimitive(Type t)
		{
			return t.IsPrimitive || t == typeof(string);
		}

		public static bool ValidPropertyType(Type type)
		{
			if (typeof(IList).IsAssignableFrom(type))
			{
				if (type.IsArray && type.GetArrayRank() == 1)
				{
					type = type.GetElementType();
				}
				else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
				{
					type = type.GetGenericArguments()[0];
				}
			}
			bool result;
			if (Property.IsPrimitive(type))
			{
				result = true;
			}
			else
			{
				TypeConverter converter = Property.GetConverter(type);
				result = (converter != null && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(CodeExpression)));
			}
			return result;
		}

		public static object ConvertFromString(Type toType, string str)
		{
			object result;
			try
			{
				result = Property.GetConverter(toType).ConvertFromInvariantString(str);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private static TypeConverter GetConverter(Type t)
		{
			string name = t.Name;
			TypeConverter result;
			switch (name)
			{
			case "Vector2":
				result = new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y"
				});
				return result;
			case "Vector3":
				result = new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"z"
				});
				return result;
			case "Vector4":
				result = new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"z",
					"w"
				});
				return result;
			case "Color":
				result = new GenericFloatVarsTypeConverter(t, new string[]
				{
					"r",
					"g",
					"b",
					"a"
				});
				return result;
			case "Quaternion":
				result = new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"z",
					"w"
				});
				return result;
			case "Rect":
				result = new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"width",
					"height"
				});
				return result;
			case "AnimationCurve":
				result = new AnimationCurveTypeConverter(t);
				return result;
			}
			if (t.BaseType == typeof(Enum))
			{
				result = new EnumTypeConverter(t);
			}
			else
			{
				result = TypeDescriptor.GetConverter(t);
			}
			return result;
		}

		public void ChangeDataType(Type newDataType)
		{
			if (this.type != newDataType)
			{
				object value = this.value;
				this.type = newDataType;
				this.m_Value = string.Empty;
				this.m_RefValues = null;
				this.m_HaveCachedDeserializedResult = false;
				this.m_CachedLastDeserializedString = string.Empty;
				this.m_CachedLastDeserializedValue = null;
				object obj = Property.ConvertActualValueIfPossible(value, newDataType);
				if (obj == null && newDataType.IsValueType && !this.isGeneric)
				{
					this.value = Property.TryGetDefaultValue(this.type);
				}
				else
				{
					this.value = obj;
				}
			}
		}

		public static object ConvertActualValueIfPossible(object value, Type toType)
		{
			object result;
			if (value == null)
			{
				result = value;
			}
			else
			{
				Type type = value.GetType();
				if (type == toType)
				{
					result = value;
				}
				else if (Property.ConvertableUnityObjects(type, toType))
				{
					if (type == typeof(GameObject))
					{
						result = ((GameObject)value).GetComponent(toType);
					}
					else if (toType == typeof(GameObject))
					{
						result = ((UnityEngine.Component)value).gameObject;
					}
					else
					{
						result = ((UnityEngine.Component)value).GetComponent(toType);
					}
				}
				else
				{
					if (toType.IsAssignableFrom(typeof(Vector3)))
					{
						if (typeof(UnityEngine.Component).IsAssignableFrom(type))
						{
							result = ((UnityEngine.Component)value).transform.position;
							return result;
						}
						if (typeof(GameObject).IsAssignableFrom(type))
						{
							result = ((GameObject)value).transform.position;
							return result;
						}
					}
					if (toType == typeof(string))
					{
						result = value.ToString();
					}
					else
					{
						try
						{
							result = Convert.ChangeType(value, toType);
						}
						catch (Exception)
						{
							result = null;
						}
					}
				}
			}
			return result;
		}

		public static bool ConvertableUnityObjects(Type t1, Type t2)
		{
			return (typeof(UnityEngine.Component).IsAssignableFrom(t1) || typeof(GameObject).IsAssignableFrom(t1)) && (typeof(UnityEngine.Component).IsAssignableFrom(t2) || typeof(GameObject).IsAssignableFrom(t2));
		}
	}
}
