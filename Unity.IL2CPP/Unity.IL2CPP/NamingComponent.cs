using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class NamingComponent : INamingService, IDisposable
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IStatsService StatsService;

		private readonly Dictionary<string, string> CleanNamesCache = new Dictionary<string, string>();

		private readonly Dictionary<MethodReference, string> ForMethodNameOnlyCache = new Dictionary<MethodReference, string>(new MethodReferenceComparer());

		private readonly Dictionary<TypeReference, string> ForTypeNameOnlyCache = new Dictionary<TypeReference, string>(new TypeReferenceEqualityComparer());

		private readonly Dictionary<string, string> ForStringLiteralCache = new Dictionary<string, string>();

		private readonly MetadataNames MemberNames = new MetadataNames();

		private readonly HashCodeCache<TypeReference> _typeHashCache;

		private readonly HashCodeCache<MethodReference> _methodHashCache;

		private readonly HashCodeCache<string> _stringLiteralHashCache;

		private const string _uIntPtrPointerField = "_pointer";

		private const string _intPtrValueField = "m_value";

		private const string _thisParameterName = "__this";

		private const string _null = "NULL";

		private readonly StringBuilder CleanStringBuilder;

		[CompilerGenerated]
		private static Func<TypeReference, uint> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<MethodReference, uint> <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<string, uint> <>f__mg$cache2;

		public string ForIntPtrT
		{
			get
			{
				return "intptr_t";
			}
		}

		public string ForUIntPtrT
		{
			get
			{
				return "uintptr_t";
			}
		}

		public string UIntPtrPointerField
		{
			get
			{
				return "_pointer";
			}
		}

		public string IntPtrValueField
		{
			get
			{
				return "m_value";
			}
		}

		public string ThisParameterName
		{
			get
			{
				return "__this";
			}
		}

		public string Null
		{
			get
			{
				return "NULL";
			}
		}

		public NamingComponent()
		{
			if (NamingComponent.<>f__mg$cache0 == null)
			{
				NamingComponent.<>f__mg$cache0 = new Func<TypeReference, uint>(SemiUniqueStableTokenGenerator.GenerateFor);
			}
			this._typeHashCache = new HashCodeCache<TypeReference>(NamingComponent.<>f__mg$cache0, delegate(uint notUsed)
			{
				NamingComponent.StatsService.TypeHashCollisions++;
			}, new TypeReferenceEqualityComparer());
			if (NamingComponent.<>f__mg$cache1 == null)
			{
				NamingComponent.<>f__mg$cache1 = new Func<MethodReference, uint>(SemiUniqueStableTokenGenerator.GenerateFor);
			}
			this._methodHashCache = new HashCodeCache<MethodReference>(NamingComponent.<>f__mg$cache1, delegate(uint notUsed)
			{
				NamingComponent.StatsService.MethodHashCollisions++;
			}, new MethodReferenceComparer());
			if (NamingComponent.<>f__mg$cache2 == null)
			{
				NamingComponent.<>f__mg$cache2 = new Func<string, uint>(SemiUniqueStableTokenGenerator.GenerateFor);
			}
			this._stringLiteralHashCache = new HashCodeCache<string>(NamingComponent.<>f__mg$cache2, delegate(uint notUsed)
			{
				NamingComponent.StatsService.MethodHashCollisions++;
			});
			this.CleanStringBuilder = new StringBuilder();
			base..ctor();
		}

		public string ForType(TypeReference typeReference)
		{
			typeReference = this.RemoveModifiers(typeReference);
			return this.ForTypeNameOnly(typeReference);
		}

		public string ForTypeNameOnly(TypeReference type)
		{
			string text;
			string result;
			if (this.ForTypeNameOnlyCache.TryGetValue(type, out text))
			{
				result = text;
			}
			else
			{
				text = this.ForTypeNameInternal(type);
				this.ForTypeNameOnlyCache[type] = text;
				result = text;
			}
			return result;
		}

		private string GetWellKnownNameFor(TypeReference typeReference)
		{
			MetadataType metadataType = typeReference.MetadataType;
			switch (metadataType)
			{
			case MetadataType.IntPtr:
			{
				string result = "IntPtr_t";
				return result;
			}
			case MetadataType.UIntPtr:
			{
				string result = "UIntPtr_t";
				return result;
			}
			case (MetadataType)26:
			case MetadataType.FunctionPointer:
			{
				IL_25:
				string result;
				if (metadataType == MetadataType.String)
				{
					result = "String_t";
					return result;
				}
				TypeDefinition typeDefinition = typeReference.Resolve();
				if (typeDefinition != null && typeDefinition.Module != null && typeDefinition.Module.Name == "mscorlib.dll")
				{
					string fullName = typeReference.FullName;
					switch (fullName)
					{
					case "System.Array":
						result = "Il2CppArray";
						return result;
					case "System.Type":
						result = "Type_t";
						return result;
					case "System.Reflection.MemberInfo":
						result = "MemberInfo_t";
						return result;
					case "System.Reflection.MethodInfo":
						result = "MethodInfo_t";
						return result;
					case "System.Reflection.FieldInfo":
						result = "FieldInfo_t";
						return result;
					case "System.Reflection.PropertyInfo":
						result = "PropertyInfo_t";
						return result;
					case "System.Reflection.EventInfo":
						result = "EventInfo_t";
						return result;
					case "System.MonoType":
						result = "MonoType_t";
						return result;
					case "System.Reflection.MonoMethod":
						result = "MonoMethod_t";
						return result;
					case "System.Reflection.MonoGenericMethod":
						result = "MonoGenericMethod_t";
						return result;
					case "System.Reflection.MonoField":
						result = "MonoField_t";
						return result;
					case "System.Reflection.MonoProperty":
						result = "MonoProperty_t";
						return result;
					case "System.Reflection.MonoEvent":
						result = "MonoEvent_t";
						return result;
					}
				}
				if (typeReference.IsIActivationFactory())
				{
					result = "Il2CppIActivationFactory";
					return result;
				}
				if (typeReference.IsIl2CppComObject())
				{
					result = "Il2CppComObject";
					return result;
				}
				result = null;
				return result;
			}
			case MetadataType.Object:
			{
				string result = "Il2CppObject";
				return result;
			}
			}
			goto IL_25;
		}

		private string ForTypeNameInternal(TypeReference typeReference)
		{
			string wellKnownNameFor = this.GetWellKnownNameFor(typeReference);
			string result;
			if (wellKnownNameFor != null)
			{
				result = wellKnownNameFor;
			}
			else
			{
				result = this.Clean(typeReference.Name) + "_t" + this.GenerateUniqueTypePostFix(typeReference);
			}
			return result;
		}

		public string ForCustomAttributesCacheGenerator(TypeDefinition typeDefinition)
		{
			return string.Format("{0}_CustomAttributesCacheGenerator", this.ForTypeNameOnly(typeDefinition));
		}

		public string ForCustomAttributesCacheGenerator(FieldDefinition fieldDefinition)
		{
			return string.Format("{0}_{1}", this.ForCustomAttributesCacheGenerator(fieldDefinition.DeclaringType), this.Clean(fieldDefinition.Name));
		}

		public string ForCustomAttributesCacheGenerator(MethodDefinition methodDefinition)
		{
			return string.Format("{0}_{1}", this.ForCustomAttributesCacheGenerator(methodDefinition.DeclaringType), this.ForMethodNameOnly(methodDefinition));
		}

		public string ForCustomAttributesCacheGenerator(PropertyDefinition propertyDefinition)
		{
			return string.Format("{0}_{1}", this.ForCustomAttributesCacheGenerator(propertyDefinition.DeclaringType), this.ForPropertyInfo(propertyDefinition));
		}

		public string ForCustomAttributesCacheGenerator(EventDefinition eventDefinition)
		{
			return string.Format("{0}_{1}", this.ForCustomAttributesCacheGenerator(eventDefinition.DeclaringType), this.ForEventInfo(eventDefinition));
		}

		public string ForCustomAttributesCacheGenerator(ParameterDefinition parameterDefinition, MethodDefinition method)
		{
			return string.Format("{0}_{1}", this.ForCustomAttributesCacheGenerator(method), this.ForParameterName(parameterDefinition));
		}

		public string ForCustomAttributesCacheGenerator(AssemblyDefinition assemblyDefinition)
		{
			return string.Format("{0}_CustomAttributesCacheGenerator", this.ForAssembly(assemblyDefinition));
		}

		public TypeReference RemoveModifiers(TypeReference typeReference)
		{
			TypeReference typeReference2 = typeReference;
			while (typeReference2 != null)
			{
				PinnedType pinnedType = typeReference2 as PinnedType;
				if (pinnedType != null)
				{
					typeReference2 = pinnedType.ElementType;
				}
				else
				{
					RequiredModifierType requiredModifierType = typeReference2 as RequiredModifierType;
					if (requiredModifierType == null)
					{
						return typeReference2;
					}
					typeReference2 = requiredModifierType.ElementType;
				}
			}
			throw new Exception();
		}

		public string ForVariable(TypeReference variableType)
		{
			variableType = this.RemoveModifiers(variableType);
			ArrayType arrayType = variableType as ArrayType;
			PointerType pointerType = variableType as PointerType;
			ByReferenceType byReferenceType = variableType as ByReferenceType;
			string result;
			if (arrayType != null)
			{
				if (arrayType.Rank == 1)
				{
					result = string.Format("{0}*", this.ForType(arrayType));
				}
				else
				{
					if (arrayType.Rank <= 1)
					{
						throw new NotImplementedException("Invalid array rank");
					}
					result = string.Format("{0}*", this.ForType(arrayType));
				}
			}
			else if (pointerType != null)
			{
				result = this.ForVariable(pointerType.ElementType) + "*";
			}
			else if (byReferenceType != null)
			{
				result = this.ForVariable(byReferenceType.ElementType) + "*";
			}
			else
			{
				MetadataType metadataType = variableType.MetadataType;
				if (metadataType == MetadataType.Void)
				{
					result = "void";
				}
				else if (metadataType == MetadataType.Boolean)
				{
					result = "bool";
				}
				else if (metadataType == MetadataType.Single)
				{
					result = "float";
				}
				else if (metadataType == MetadataType.Double)
				{
					result = "double";
				}
				else if (metadataType == MetadataType.String)
				{
					result = this.ForType(variableType) + "*";
				}
				else if (metadataType == MetadataType.SByte)
				{
					result = "int8_t";
				}
				else if (metadataType == MetadataType.Byte)
				{
					result = "uint8_t";
				}
				else if (metadataType == MetadataType.Char)
				{
					result = "Il2CppChar";
				}
				else if (metadataType == MetadataType.Int16)
				{
					result = "int16_t";
				}
				else if (metadataType == MetadataType.UInt16)
				{
					result = "uint16_t";
				}
				else if (metadataType == MetadataType.Int32)
				{
					result = "int32_t";
				}
				else if (metadataType == MetadataType.UInt32)
				{
					result = "uint32_t";
				}
				else if (metadataType == MetadataType.Int64)
				{
					result = "int64_t";
				}
				else if (metadataType == MetadataType.UInt64)
				{
					result = "uint64_t";
				}
				else if (metadataType == MetadataType.IntPtr)
				{
					result = this.ForTypeNameOnly(variableType);
				}
				else if (variableType.Name == this.ForIntPtrT)
				{
					result = this.ForIntPtrT;
				}
				else if (variableType.Name == this.ForUIntPtrT)
				{
					result = this.ForUIntPtrT;
				}
				else
				{
					GenericParameter genericParameter = variableType as GenericParameter;
					if (genericParameter != null)
					{
						throw new ArgumentException("Generic parameter encountered as variable type", "variableType");
					}
					TypeDefinition typeDefinition = variableType.Resolve();
					if (typeDefinition.IsEnum)
					{
						FieldDefinition fieldDefinition = typeDefinition.Fields.Single((FieldDefinition f) => f.Name == "value__");
						result = this.ForVariable(fieldDefinition.FieldType);
					}
					else
					{
						GenericInstanceType genericInstanceType = variableType as GenericInstanceType;
						if (genericInstanceType != null)
						{
							if (variableType.Resolve().IsInterface)
							{
								result = this.ForType(variableType.Module.TypeSystem.Object) + "*";
							}
							else
							{
								result = string.Format("{0} {1}", this.ForTypeNameOnly(variableType), (!NamingComponent.NeedsAsterisk(variableType)) ? string.Empty : "*");
							}
						}
						else
						{
							result = this.ForVariableInternal(variableType);
						}
					}
				}
			}
			return result;
		}

		private string ForVariableInternal(TypeReference variableType)
		{
			RequiredModifierType requiredModifierType = variableType as RequiredModifierType;
			if (requiredModifierType != null)
			{
				variableType = requiredModifierType.ElementType;
			}
			string result;
			if (variableType.Resolve().IsInterface)
			{
				result = this.ForVariable(NamingComponent.TypeProvider.SystemObject);
			}
			else
			{
				result = string.Format("{0} {1}", this.ForType(variableType), (!NamingComponent.NeedsAsterisk(variableType)) ? string.Empty : "*");
			}
			return result;
		}

		private static TypeReference UnderlyingType(TypeReference type)
		{
			TypeSpecification typeSpecification = type as TypeSpecification;
			TypeReference result;
			if (typeSpecification != null)
			{
				result = NamingComponent.UnderlyingType(typeSpecification.ElementType);
			}
			else
			{
				result = type;
			}
			return result;
		}

		private static bool NeedsAsterisk(TypeReference type)
		{
			TypeReference typeReference = NamingComponent.UnderlyingType(type);
			return !typeReference.IsValueType() || type.IsByReference;
		}

		public string ForMethod(MethodReference method)
		{
			return this.ForMethodNameOnly(method);
		}

		public string ForMethodAdjustorThunk(MethodReference method)
		{
			return this.ForMethod(method) + "_AdjustorThunk";
		}

		public string ForMethodNameOnly(MethodReference method)
		{
			string text;
			string result;
			if (this.ForMethodNameOnlyCache.TryGetValue(method, out text))
			{
				result = text;
			}
			else
			{
				TypeResolver typeResolver = TypeResolver.For(method.DeclaringType, method);
				MethodReference methodReference = typeResolver.Resolve(method.Resolve());
				if (this.ForMethodNameOnlyCache.TryGetValue(methodReference, out text))
				{
					result = text;
				}
				else if (!TypeReferenceEqualityComparer.AreEqual(methodReference.DeclaringType, method.DeclaringType, TypeComparisonMode.Exact))
				{
					result = this.ForMethodNameOnly(methodReference);
				}
				else
				{
					string text2 = this.ForMethodInternal(method);
					this.ForMethodNameOnlyCache[method] = text2;
					result = text2;
				}
			}
			return result;
		}

		private string ForMethodInternal(MethodReference method)
		{
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Clean(method.DeclaringType.Name));
			stringBuilder.Append("_");
			stringBuilder.Append(this.Clean(method.Name));
			if (genericInstanceMethod != null)
			{
				foreach (TypeReference current in genericInstanceMethod.GenericArguments)
				{
					stringBuilder.Append("_Tis" + this.ForTypeNameOnly(current));
				}
			}
			stringBuilder.Append("_m");
			stringBuilder.Append(this.GenerateUniqueMethodPostFix(method));
			return stringBuilder.ToString();
		}

		private string ForTypeMangling(TypeReference typeReference)
		{
			string result;
			if (typeReference.IsGenericParameter)
			{
				GenericParameter genericParameter = (GenericParameter)typeReference;
				result = ((genericParameter.MetadataType != MetadataType.Var) ? "mgp" : "tgp") + genericParameter.Position;
			}
			else if (typeReference.IsArray)
			{
				ArrayType arrayType = (ArrayType)typeReference;
				result = this.ForTypeMangling(arrayType.ElementType) + "_arr" + arrayType.Rank;
			}
			else if (typeReference.IsGenericInstance)
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)typeReference;
				string text = this.ForTypeMangling(genericInstanceType.ElementType) + "_git_";
				foreach (TypeReference current in genericInstanceType.GenericArguments)
				{
					text = text + "_" + this.ForTypeMangling(current);
				}
				result = text;
			}
			else
			{
				string text2 = this.ForTypeNameOnly(typeReference);
				if (typeReference is ArrayType)
				{
					result = text2 + "_arr";
				}
				else if (typeReference is PointerType)
				{
					result = text2 + "_ptr";
				}
				else if (typeReference is ByReferenceType)
				{
					result = text2 + "_ref";
				}
				else
				{
					result = this.Clean(text2);
				}
			}
			return result;
		}

		public string ForVariableName(VariableReference variable)
		{
			return "V_" + variable.Index;
		}

		public string ForFieldPadding(FieldReference field)
		{
			return this.ForField(field) + "_OffsetPadding";
		}

		public int GetFieldIndex(FieldReference field, bool includeBase = false)
		{
			FieldDefinition fieldDefinition = field.Resolve();
			TypeDefinition typeDefinition = (fieldDefinition.DeclaringType.BaseType == null) ? fieldDefinition.DeclaringType : fieldDefinition.DeclaringType.BaseType.Resolve();
			int num = 0;
			while (includeBase && typeDefinition != null)
			{
				num += typeDefinition.Fields.Count;
				typeDefinition = ((typeDefinition.BaseType == null) ? null : typeDefinition.BaseType.Resolve());
			}
			Collection<FieldDefinition> fields = fieldDefinition.DeclaringType.Fields;
			for (int i = 0; i < fields.Count; i++)
			{
				if (fieldDefinition == fields[i])
				{
					return num + i;
				}
			}
			throw new InvalidOperationException(string.Format("Field {0} was not found on its definition {1}!", field.Name, fieldDefinition.DeclaringType.FullName));
		}

		public string ForField(FieldReference field)
		{
			return this.Clean(this.EscapeKeywords(field.Name)) + "_" + this.GetFieldIndex(field, true);
		}

		public string ForFieldOffsetGetter(FieldReference field)
		{
			return string.Format("get_offset_of_{0}_", this.Clean(field.Name)) + this.GetFieldIndex(field, true);
		}

		public string ForFieldGetter(FieldReference field)
		{
			return string.Format("get_{0}_", this.Clean(field.Name)) + this.GetFieldIndex(field, true);
		}

		public string ForFieldAddressGetter(FieldReference field)
		{
			return string.Format("get_address_of_{0}_", this.Clean(field.Name)) + this.GetFieldIndex(field, true);
		}

		public string ForFieldSetter(FieldReference field)
		{
			return string.Format("set_{0}_", this.Clean(field.Name)) + this.GetFieldIndex(field, true);
		}

		public string ForArrayItems()
		{
			return "m_Items";
		}

		public string ForArrayItemGetter()
		{
			return "GetAt";
		}

		public string ForArrayItemAddressGetter()
		{
			return "GetAddressAt";
		}

		public string ForArrayItemSetter()
		{
			return "SetAt";
		}

		public string ForArrayIndexType()
		{
			return "il2cpp_array_size_t";
		}

		public string ForArrayIndexName()
		{
			return "index";
		}

		private bool IsSafeCharacter(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
		}

		public string Clean(string name)
		{
			string result;
			if (this.CleanNamesCache.ContainsKey(name))
			{
				result = this.CleanNamesCache[name];
			}
			else
			{
				StringBuilder stringBuilder = this.CleanStringBuilder.Clear();
				char[] array = name.ToCharArray();
				for (int i = 0; i < array.Length; i++)
				{
					char c = array[i];
					if (this.IsSafeCharacter(c) || (this.IsAsciiDigit(c) && i != 0))
					{
						stringBuilder.Append(c);
					}
					else
					{
						ushort num = Convert.ToUInt16(c);
						if (num < 255)
						{
							if (num == 46 || num == 47 || num == 96 || num == 95)
							{
								stringBuilder.Append("_");
							}
							else
							{
								stringBuilder.AppendFormat("U{0:X2}", num);
							}
						}
						else if (num < 4095)
						{
							stringBuilder.AppendFormat("U{0:X3}", num);
						}
						else
						{
							stringBuilder.AppendFormat("U{0:X4}", num);
						}
					}
				}
				string text = stringBuilder.ToString();
				this.CleanNamesCache[name] = text;
				result = text;
			}
			return result;
		}

		private bool IsAsciiDigit(char c)
		{
			return c >= '0' && c <= '9';
		}

		public string ForFile(TypeDefinition type)
		{
			return this.ModuleNameToPrependString(type.Module.Name) + "_" + this.Clean(type.FullName);
		}

		public string ForInitializedTypeInfo(string argument)
		{
			return string.Format("InitializedTypeInfo({0})", argument);
		}

		private string ForTypeInfo(TypeReference typeReference)
		{
			return this.TypeMember(typeReference, this.MemberNames.Il2CppClass);
		}

		private string ForEventInfo(EventDefinition ev)
		{
			return this.TypeMember(ev.DeclaringType, this.Clean(this.EscapeKeywords(ev.Name)) + "_EventInfo");
		}

		private string ForFieldInfo(FieldReference field)
		{
			return this.TypeMember(field.DeclaringType, this.ForField(field) + "_FieldInfo");
		}

		public string ForIl2CppType(TypeReference type, int attrs = 0)
		{
			TypeReference nonPinnedAndNonByReferenceType = type.GetNonPinnedAndNonByReferenceType();
			string text = this.ForType(nonPinnedAndNonByReferenceType);
			GenericParameter genericParameter = nonPinnedAndNonByReferenceType as GenericParameter;
			if (genericParameter != null)
			{
				TypeDefinition typeDefinition = genericParameter.Owner as TypeDefinition;
				if (typeDefinition != null)
				{
					text = this.ForTypeNameOnly((TypeReference)genericParameter.Owner) + "_gp_" + genericParameter.Position;
				}
				else
				{
					text = this.ForMethodNameOnly((MethodReference)genericParameter.Owner) + "_gp_" + genericParameter.Position;
				}
			}
			return string.Concat(new object[]
			{
				text,
				"_",
				(!type.IsByReference) ? 0 : 1,
				"_",
				(!type.IsPinned) ? 0 : 1,
				"_",
				attrs
			});
		}

		public string ForGenericInst(IList<TypeReference> types)
		{
			string text = "GenInst";
			for (int i = 0; i < types.Count; i++)
			{
				text = text + "_" + this.ForIl2CppType(types[i], 0);
			}
			return text;
		}

		public string ForGenericClass(TypeReference type)
		{
			return this.TypeMember(type, this.MemberNames.GenericClass);
		}

		public string ForStaticFieldsStruct(TypeReference type)
		{
			return this.TypeMember(type, this.MemberNames.StaticFields);
		}

		public string ForThreadFieldsStruct(TypeReference type)
		{
			return this.TypeMember(type, this.MemberNames.ThreadStaticFields);
		}

		private string TypeMember(TypeReference type, string memberName)
		{
			string result;
			if (type.IsGenericParameter)
			{
				GenericParameter genericParameter = (GenericParameter)type;
				result = string.Format("{0}_{1}", string.Concat(new object[]
				{
					(genericParameter.Owner.GenericParameterType != GenericParameterType.Type) ? this.ForMethodNameOnly((MethodReference)genericParameter.Owner) : this.ForTypeNameOnly((TypeReference)genericParameter.Owner),
					"_gp_",
					this.Clean(genericParameter.Name),
					"_",
					genericParameter.Position
				}), memberName);
			}
			else
			{
				result = string.Format("{0}_{1}", this.ForType(type), memberName);
			}
			return result;
		}

		private string ForPropertyInfo(PropertyDefinition property)
		{
			return this.ForPropertyInfo(property, property.DeclaringType);
		}

		private string ForPropertyInfo(PropertyDefinition property, TypeReference declaringType)
		{
			string str = this.Clean(this.EscapeKeywords(property.Name));
			if (declaringType.Resolve().Properties.Count((PropertyDefinition p) => p.Name == property.Name) > 1)
			{
				str = str + "_" + (from param in property.Parameters
				select this.ForTypeMangling(param.ParameterType)).Aggregate((string buff, string s) => buff + "_" + s);
			}
			return this.TypeMember(declaringType, str + "_PropertyInfo");
		}

		public string ForDebugMethodInfo(MethodReference method)
		{
			return this.ForMethodInfoInternal(method, this.MemberNames.DebugMethodInfo);
		}

		public string ForDebugMethodInfoOffsetTable(MethodReference method)
		{
			return this.ForMethodInfoInternal(method, this.MemberNames.DebugMethodInfoOffsetTable);
		}

		public string ForDebugMethodLocalInfo(VariableDefinition variable, MethodReference method)
		{
			return this.ForMethodInfoInternal(method, string.Concat(new object[]
			{
				variable.Name,
				"_",
				variable.Index,
				"_DebugLocalInfo"
			}));
		}

		public string ForDebugLocalInfo(MethodReference method)
		{
			return this.ForMethodInfoInternal(method, "DebugLocalInfos");
		}

		private string ForMethodInfoInternal(MethodReference method, string suffix)
		{
			return this.ForMethodNameOnly(method) + "_" + suffix;
		}

		public string ForParameterName(ParameterReference parameterReference)
		{
			string name = (!string.IsNullOrEmpty(parameterReference.Name)) ? this.EscapeKeywords(parameterReference.Name) : "p";
			return this.Clean(name) + parameterReference.Index.ToString(CultureInfo.InvariantCulture);
		}

		public string ForCreateStringMethod(MethodReference method)
		{
			if (method.DeclaringType.Name != "String")
			{
				throw new Exception("method.DeclaringType.Name != \"String\"");
			}
			foreach (MethodDefinition current in from meth in method.DeclaringType.Resolve().Methods
			where meth.Name == "CreateString"
			select meth)
			{
				if (current.Parameters.Count == method.Parameters.Count)
				{
					bool flag = false;
					for (int i = 0; i < current.Parameters.Count; i++)
					{
						if (current.Parameters[i].ParameterType.FullName != method.Parameters[i].ParameterType.FullName)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						return this.ForMethodNameOnly(current);
					}
				}
			}
			throw new Exception(string.Format("Can't find proper CreateString : {0}", method.FullName));
		}

		public string ForDebugTypeInfos(TypeReference type)
		{
			return this.TypeMember(type, this.MemberNames.DebugTypeInfo);
		}

		public string ForArrayType(ArrayType type)
		{
			return this.ForTypeNameOnly(type) + "_ArrayType";
		}

		public string ForAssembly(AssemblyDefinition assembly)
		{
			return string.Format("g_{0}_Assembly", this.Clean(assembly.Name.Name));
		}

		public string ForAssemblyScope(AssemblyDefinition assembly, string symbol)
		{
			return string.Format("{0}_{1}", this.ForAssembly(assembly), symbol);
		}

		public string ModuleNameToPrependString(string name)
		{
			return this.Clean(name.Replace(".dll", "").Replace(".DLL", ""));
		}

		public bool IsSpecialArrayMethod(MethodReference methodReference)
		{
			return (methodReference.Name == "Set" || methodReference.Name == "Get" || methodReference.Name == "Address" || methodReference.Name == ".ctor") && methodReference.DeclaringType.IsArray;
		}

		public string ForImage(TypeDefinition type)
		{
			return string.Format("g_{0}_Image", this.Clean(type.Module.Name));
		}

		public string ForImage(ModuleDefinition module)
		{
			return string.Format("g_{0}_Image", this.Clean(module.Name));
		}

		private string EscapeKeywords(string fieldName)
		{
			return "___" + fieldName;
		}

		public string ForStringLiteralIdentifier(string literal)
		{
			string text;
			string result;
			if (this.ForStringLiteralCache.TryGetValue(literal, out text))
			{
				result = text;
			}
			else
			{
				string text2 = "_stringLiteral" + this.GenerateUniqueStringLiteralPostFix(literal);
				this.ForStringLiteralCache[literal] = text2;
				result = text2;
			}
			return result;
		}

		public string AddressOf(string value)
		{
			string result;
			if (value.StartsWith("*"))
			{
				result = value.Substring(1);
			}
			else
			{
				result = string.Format("&{0}", value);
			}
			return result;
		}

		public string ForRuntimeIl2CppType(TypeReference type)
		{
			return this.ForIl2CppType(type, 0) + "_var";
		}

		public string ForRuntimeTypeInfo(TypeReference type)
		{
			return this.ForTypeInfo(type) + "_var";
		}

		public string ForRuntimeMethodInfo(MethodReference method)
		{
			return this.ForMethodInfoInternal(method, this.MemberNames.MethodInfo) + "_var";
		}

		public string ForRuntimeFieldInfo(FieldReference field)
		{
			return this.ForFieldInfo(field) + "_var";
		}

		public string ForPadding(TypeDefinition typeDefinition)
		{
			return this.ForType(typeDefinition) + "__padding";
		}

		public string ForComTypeInterfaceFieldName(TypeReference interfaceType)
		{
			return this.ForInteropInterfaceVariable(interfaceType);
		}

		public string ForComTypeInterfaceFieldGetter(TypeReference interfaceType)
		{
			return "get_" + this.ForInteropInterfaceVariable(interfaceType);
		}

		public string ForInteropInterfaceVariable(TypeReference interfaceType)
		{
			string result;
			if (interfaceType.IsIActivationFactory())
			{
				result = "activationFactory";
			}
			else
			{
				string text = this.ForTypeNameOnly(interfaceType).TrimStart(new char[]
				{
					'_'
				});
				result = text.Substring(0, 2).ToLower() + text.Substring(2);
			}
			return result;
		}

		public string ForInteropHResultVariable()
		{
			return "hr";
		}

		public string ForInteropReturnValue()
		{
			return "returnValue";
		}

		public string ForComInterfaceReturnParameterName()
		{
			return "comReturnValue";
		}

		public string ForPInvokeFunctionPointerTypedef()
		{
			return "PInvokeFunc";
		}

		public string ForPInvokeFunctionPointerVariable()
		{
			return "il2cppPInvokeFunc";
		}

		public string ForDelegatePInvokeWrapper(TypeReference type)
		{
			return "DelegatePInvokeWrapper_" + this.ForType(type);
		}

		public string ForReversePInvokeWrapperMethod(MethodReference method)
		{
			return "ReversePInvokeWrapper_" + this.ForMethod(method);
		}

		public string ForIl2CppComObjectIdentityField()
		{
			return "identity";
		}

		private string GenerateUniqueTypePostFix(TypeReference typeReference)
		{
			return this._typeHashCache.GetUniqueHash(typeReference).ToString();
		}

		private string GenerateUniqueMethodPostFix(MethodReference methodReference)
		{
			return this._methodHashCache.GetUniqueHash(methodReference).ToString();
		}

		private string GenerateUniqueStringLiteralPostFix(string literal)
		{
			return this._stringLiteralHashCache.GetUniqueHash(literal).ToString();
		}

		public void Dispose()
		{
			this.CleanNamesCache.Clear();
			this.ForMethodNameOnlyCache.Clear();
			this.ForTypeNameOnlyCache.Clear();
			this.ForStringLiteralCache.Clear();
			this._typeHashCache.Clear();
			this._methodHashCache.Clear();
			this._stringLiteralHashCache.Clear();
		}
	}
}
