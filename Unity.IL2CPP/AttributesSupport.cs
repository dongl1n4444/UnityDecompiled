using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public class AttributesSupport
	{
		private readonly CppCodeWriter _writer;

		private readonly AttributeCollection _collection;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public static IGenericSharingAnalysisService GenericSharing;

		[CompilerGenerated]
		private static Func<CustomAttributeArgument, bool> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache2;

		[CompilerGenerated]
		private static Func<CustomAttributeArgument, bool> <>f__mg$cache3;

		[CompilerGenerated]
		private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache4;

		[CompilerGenerated]
		private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache5;

		public AttributesSupport(CppCodeWriter writer, AttributeCollection collection)
		{
			this._writer = writer;
			this._collection = collection;
		}

		public TableInfo WriteAttributes(IEnumerable<AssemblyDefinition> assemblyDefinitions)
		{
			foreach (AssemblyDefinition current in assemblyDefinitions)
			{
				this.WriteCustomAttributesCacheGeneratorFor(current);
				foreach (TypeDefinition current2 in current.MainModule.GetAllTypes())
				{
					this.WriteCustomAttributesCacheGenerators(current2);
				}
			}
			return MetadataWriter.WriteTable<string>(this._writer, "extern const CustomAttributesCacheGenerator", "g_AttributeGenerators", this._collection.GetEntries(), (string a) => a);
		}

		private void WriteCustomAttributesCacheGenerators(TypeDefinition type)
		{
			this.WriteCustomAttributesCacheGeneratorFor(type);
			foreach (FieldDefinition current in type.Fields)
			{
				this.WriteCustomAttributesCacheGeneratorFor(current);
			}
			foreach (MethodDefinition current2 in type.Methods)
			{
				this.WriteCustomAttributesCacheGeneratorFor(current2);
				foreach (ParameterDefinition current3 in current2.Parameters)
				{
					this.WriteCustomAttributesCacheGeneratorFor(current3, current2);
				}
			}
			foreach (PropertyDefinition current4 in type.Properties)
			{
				this.WriteCustomAttributesCacheGeneratorFor(current4);
			}
			foreach (EventDefinition current5 in type.Events)
			{
				this.WriteCustomAttributesCacheGeneratorFor(current5);
			}
		}

		private void WriteCustomAttributesCacheGeneratorFor(TypeDefinition typeDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(typeDefinition), typeDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(FieldDefinition fieldDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(fieldDefinition), fieldDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(MethodDefinition methodDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(methodDefinition), methodDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(PropertyDefinition propertyDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(propertyDefinition), propertyDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(EventDefinition eventDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(eventDefinition), eventDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(ParameterDefinition parameterDefinition, MethodDefinition methodDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(parameterDefinition, methodDefinition), parameterDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(AssemblyDefinition assemblyDefinition)
		{
			this.WriteCustomAttributesCacheGeneratorFor(AttributesSupport.Naming.ForCustomAttributesCacheGenerator(assemblyDefinition), assemblyDefinition);
		}

		private void WriteCustomAttributesCacheGeneratorFor(string name, ICustomAttributeProvider customAttributeProvider)
		{
			CustomAttribute[] constructibleCustomAttributes = customAttributeProvider.GetConstructibleCustomAttributes().ToArray<CustomAttribute>();
			if (constructibleCustomAttributes.Length != 0)
			{
				this._collection.Add(name, constructibleCustomAttributes);
				CustomAttribute[] constructibleCustomAttributes2 = constructibleCustomAttributes;
				for (int i = 0; i < constructibleCustomAttributes2.Length; i++)
				{
					CustomAttribute customAttribute = constructibleCustomAttributes2[i];
					this._writer.AddIncludeForTypeDefinition(customAttribute.AttributeType);
					this._writer.AddIncludeForMethodDeclarations(customAttribute.AttributeType);
					foreach (TypeReference current in AttributesSupport.ExtractTypeReferencesFromCustomAttributeArguments(customAttribute.ConstructorArguments))
					{
						if (current != null)
						{
							this._writer.AddIncludeForTypeDefinition(current);
						}
					}
					foreach (TypeReference current2 in AttributesSupport.ExtractTypeReferencesFromCustomAttributeArguments(from f in customAttribute.Fields
					select f.Argument))
					{
						if (current2 != null)
						{
							this._writer.AddIncludeForTypeDefinition(current2);
						}
					}
					foreach (TypeReference current3 in AttributesSupport.ExtractTypeReferencesFromCustomAttributeArguments(from p in customAttribute.Properties
					select p.Argument))
					{
						if (current3 != null)
						{
							this._writer.AddIncludeForTypeDefinition(current3);
						}
					}
					TypeDefinition attributeType = customAttribute.AttributeType.Resolve();
					List<PropertyDefinition> source = AttributesSupport.GatherPropertiesFromTypeAndBaseTypes(attributeType);
					using (Collection<CustomAttributeNamedArgument>.Enumerator enumerator4 = customAttribute.Properties.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							CustomAttributeNamedArgument property = enumerator4.Current;
							MethodDefinition setMethod = source.First((PropertyDefinition p) => p.Name == property.Name).SetMethod;
							this._writer.AddIncludeForMethodDeclarations(setMethod.DeclaringType);
						}
					}
				}
				MethodWriter.WriteMethodWithMetadataInitialization(this._writer, string.Format("static void {0}(CustomAttributesCache* cache)", name), name, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
				{
					AttributesSupport.WriteMethodBody(bodyWriter, constructibleCustomAttributes, new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage));
				}, name);
			}
		}

		private static void WriteMethodBody(CppCodeWriter writer, IEnumerable<CustomAttribute> customAttributes, DefaultRuntimeMetadataAccess metadataAccess)
		{
			int num = 0;
			foreach (CustomAttribute current in customAttributes)
			{
				writer.BeginBlock();
				AttributesSupport.DeclareTempLocals(writer, current, metadataAccess);
				TypeDefinition typeDefinition = current.AttributeType.Resolve();
				writer.WriteLine("{0} tmp = ({0})cache->attributes[{1}];", new object[]
				{
					AttributesSupport.Naming.ForVariable(typeDefinition),
					num
				});
				writer.WriteLine("{0}({1}, {2});", new object[]
				{
					AttributesSupport.Naming.ForMethodNameOnly(current.Constructor),
					AttributesSupport.CustomAttributeConstructorFormattedArgumentsFor(current, metadataAccess),
					metadataAccess.HiddenMethodInfo(current.Constructor)
				});
				List<FieldDefinition> source = AttributesSupport.GatherFieldsFromTypeAndBaseTypes(typeDefinition);
				using (Collection<CustomAttributeNamedArgument>.Enumerator enumerator2 = current.Fields.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CustomAttributeNamedArgument fieldArgument = enumerator2.Current;
						writer.WriteLine("tmp->{0}({1});", new object[]
						{
							AttributesSupport.Naming.ForFieldSetter(source.First((FieldDefinition p) => p.Name == fieldArgument.Name)),
							AttributesSupport.FormatAttributeValue(fieldArgument.Argument, AttributesSupport.TempName(fieldArgument), metadataAccess)
						});
					}
				}
				List<PropertyDefinition> source2 = AttributesSupport.GatherPropertiesFromTypeAndBaseTypes(typeDefinition);
				using (Collection<CustomAttributeNamedArgument>.Enumerator enumerator3 = current.Properties.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						CustomAttributeNamedArgument propertyArgument = enumerator3.Current;
						MethodDefinition setMethod = source2.First((PropertyDefinition p) => p.Name == propertyArgument.Name).SetMethod;
						writer.WriteLine("{0}(tmp, {1}, {2});", new object[]
						{
							AttributesSupport.Naming.ForMethodNameOnly(setMethod),
							AttributesSupport.FormatAttributeValue(propertyArgument.Argument, AttributesSupport.TempName(propertyArgument), metadataAccess),
							metadataAccess.HiddenMethodInfo(setMethod)
						});
					}
				}
				writer.EndBlock(false);
				num++;
			}
		}

		private static List<PropertyDefinition> GatherPropertiesFromTypeAndBaseTypes(TypeDefinition attributeType)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			for (TypeDefinition typeDefinition = attributeType; typeDefinition != null; typeDefinition = ((typeDefinition.BaseType == null) ? null : typeDefinition.BaseType.Resolve()))
			{
				list.AddRange(typeDefinition.Properties);
			}
			return list;
		}

		private static List<FieldDefinition> GatherFieldsFromTypeAndBaseTypes(TypeDefinition attributeType)
		{
			List<FieldDefinition> list = new List<FieldDefinition>();
			for (TypeDefinition typeDefinition = attributeType; typeDefinition != null; typeDefinition = ((typeDefinition.BaseType == null) ? null : typeDefinition.BaseType.Resolve()))
			{
				list.AddRange(typeDefinition.Fields);
			}
			return list;
		}

		private static void DeclareTempLocals(CppCodeWriter writer, CustomAttribute attribute, IRuntimeMetadataAccess metadataAccess)
		{
			AttributesSupport.DeclareTempLocalsForBoxing(writer, attribute, metadataAccess);
			AttributesSupport.DeclareTempLocalsForArrays(writer, attribute, metadataAccess);
		}

		private static void DeclareTempLocalsForBoxing(CppCodeWriter writer, CustomAttribute attribute, IRuntimeMetadataAccess metadataAccess)
		{
			IEnumerable<CustomAttributeArgument> arg_25_0 = attribute.ConstructorArguments;
			if (AttributesSupport.<>f__mg$cache0 == null)
			{
				AttributesSupport.<>f__mg$cache0 = new Func<CustomAttributeArgument, bool>(AttributesSupport.ValueNeedsBoxing);
			}
			foreach (CustomAttributeArgument current in arg_25_0.Where(AttributesSupport.<>f__mg$cache0))
			{
				AttributesSupport.WriteStoreValueInTempLocal(writer, AttributesSupport.TempName(current, attribute), (CustomAttributeArgument)current.Value, metadataAccess);
			}
			IEnumerable<CustomAttributeNamedArgument> arg_97_0 = attribute.Fields;
			if (AttributesSupport.<>f__mg$cache1 == null)
			{
				AttributesSupport.<>f__mg$cache1 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueNeedsBoxing);
			}
			foreach (CustomAttributeNamedArgument current2 in arg_97_0.Where(AttributesSupport.<>f__mg$cache1))
			{
				AttributesSupport.WriteStoreValueInTempLocal(writer, AttributesSupport.TempName(current2), (CustomAttributeArgument)current2.Argument.Value, metadataAccess);
			}
			IEnumerable<CustomAttributeNamedArgument> arg_111_0 = attribute.Properties;
			if (AttributesSupport.<>f__mg$cache2 == null)
			{
				AttributesSupport.<>f__mg$cache2 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueNeedsBoxing);
			}
			foreach (CustomAttributeNamedArgument current3 in arg_111_0.Where(AttributesSupport.<>f__mg$cache2))
			{
				AttributesSupport.WriteStoreValueInTempLocal(writer, AttributesSupport.TempName(current3), (CustomAttributeArgument)current3.Argument.Value, metadataAccess);
			}
		}

		private static bool ValueNeedsBoxing(CustomAttributeNamedArgument namedArgument)
		{
			return AttributesSupport.ValueNeedsBoxing(namedArgument.Argument);
		}

		private static bool ValueNeedsBoxing(CustomAttributeArgument argument)
		{
			bool result;
			if (argument.Type.MetadataType != MetadataType.Object)
			{
				result = false;
			}
			else
			{
				CustomAttributeArgument customAttributeArgument = (CustomAttributeArgument)argument.Value;
				result = (customAttributeArgument.Type.MetadataType != MetadataType.String && (customAttributeArgument.Type.MetadataType != MetadataType.Class || !(customAttributeArgument.Value is TypeReference)));
			}
			return result;
		}

		private static string TempName(CustomAttributeNamedArgument argument)
		{
			return "_tmp_" + argument.Name;
		}

		private static string TempName(CustomAttributeArgument argument, CustomAttribute attribute)
		{
			return "_tmp_" + attribute.ConstructorArguments.IndexOf(argument);
		}

		private static void WriteStoreValueInTempLocal(CppCodeWriter writer, string variableName, CustomAttributeArgument argument, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("{0} {1} = {2};", new object[]
			{
				AttributesSupport.StorageTypeFor(argument.Value),
				variableName,
				AttributesSupport.FormatForAssignment(argument.Value, metadataAccess)
			});
		}

		private static void DeclareTempLocalsForArrays(CppCodeWriter writer, CustomAttribute attribute, IRuntimeMetadataAccess metadataAccess)
		{
			IEnumerable<CustomAttributeArgument> arg_25_0 = attribute.ConstructorArguments;
			if (AttributesSupport.<>f__mg$cache3 == null)
			{
				AttributesSupport.<>f__mg$cache3 = new Func<CustomAttributeArgument, bool>(AttributesSupport.ValueIsArray);
			}
			foreach (CustomAttributeArgument current in arg_25_0.Where(AttributesSupport.<>f__mg$cache3))
			{
				AttributesSupport.WriteStoreArrayInTempLocal(writer, AttributesSupport.TempName(current, attribute), current, metadataAccess);
			}
			IEnumerable<CustomAttributeNamedArgument> arg_8C_0 = attribute.Fields;
			if (AttributesSupport.<>f__mg$cache4 == null)
			{
				AttributesSupport.<>f__mg$cache4 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueIsArray);
			}
			foreach (CustomAttributeNamedArgument current2 in arg_8C_0.Where(AttributesSupport.<>f__mg$cache4))
			{
				AttributesSupport.WriteStoreArrayInTempLocal(writer, AttributesSupport.TempName(current2), current2.Argument, metadataAccess);
			}
			IEnumerable<CustomAttributeNamedArgument> arg_F8_0 = attribute.Properties;
			if (AttributesSupport.<>f__mg$cache5 == null)
			{
				AttributesSupport.<>f__mg$cache5 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueIsArray);
			}
			foreach (CustomAttributeNamedArgument current3 in arg_F8_0.Where(AttributesSupport.<>f__mg$cache5))
			{
				AttributesSupport.WriteStoreArrayInTempLocal(writer, AttributesSupport.TempName(current3), current3.Argument, metadataAccess);
			}
		}

		private static bool ValueIsArray(CustomAttributeNamedArgument namedArgument)
		{
			return AttributesSupport.ValueIsArray(namedArgument.Argument);
		}

		private static bool ValueIsArray(CustomAttributeArgument argument)
		{
			return argument.Type.MetadataType == MetadataType.Array;
		}

		private static void WriteStoreArrayInTempLocal(CppCodeWriter writer, string varName, CustomAttributeArgument attributeValue, IRuntimeMetadataAccess metadataAccess)
		{
			CustomAttributeArgument[] array = (CustomAttributeArgument[])attributeValue.Value;
			ArrayType arrayType = (ArrayType)attributeValue.Type;
			if (array == null)
			{
				writer.WriteLine(Statement.Expression(Emit.Assign(string.Format("{0} {1}", AttributesSupport.Naming.ForVariable(arrayType), varName), "NULL")));
			}
			else
			{
				writer.WriteLine(Statement.Expression(Emit.Assign(string.Format("{0} {1}", AttributesSupport.Naming.ForVariable(arrayType), varName), Emit.NewSZArray(arrayType, array.Length, metadataAccess))));
				TypeReference elementType = arrayType.ElementType;
				if (elementType.MetadataType == MetadataType.Object)
				{
					AttributesSupport.WriteInitializeObjectArray(writer, varName, elementType, array, metadataAccess);
				}
				else
				{
					AttributesSupport.WriteInitializeArray(writer, varName, elementType, array, metadataAccess);
				}
			}
		}

		private static void WriteInitializeObjectArray(CppCodeWriter writer, string varName, TypeReference elementType, IEnumerable<CustomAttributeArgument> arguments, IRuntimeMetadataAccess metadataAccess)
		{
			int num = 0;
			foreach (CustomAttributeArgument current in arguments)
			{
				CustomAttributeArgument argument = current;
				if (argument.Value is CustomAttributeArgument)
				{
					argument = (CustomAttributeArgument)argument.Value;
				}
				if (argument.Type.MetadataType == MetadataType.String)
				{
					AttributesSupport.WriteStoreArrayElement(writer, varName, elementType, num, AttributesSupport.FormatForAssignment(argument.Value, metadataAccess));
				}
				else
				{
					string text = varName + "_" + num;
					AttributesSupport.WriteStoreValueInTempLocal(writer, text, argument, metadataAccess);
					AttributesSupport.WriteStoreArrayElement(writer, varName, elementType, num, Emit.Box(argument.Type, text, metadataAccess));
				}
				num++;
			}
		}

		private static void WriteInitializeArray(CppCodeWriter writer, string varName, TypeReference elementType, IEnumerable<CustomAttributeArgument> arguments, IRuntimeMetadataAccess metadataAccess)
		{
			int num = 0;
			foreach (CustomAttributeArgument current in arguments)
			{
				AttributesSupport.WriteStoreArrayElement(writer, varName, elementType, num++, AttributesSupport.FormatForAssignment(current.Value, metadataAccess));
			}
		}

		private static void WriteStoreArrayElement(CppCodeWriter writer, string varName, TypeReference elementType, int index, string value)
		{
			writer.WriteLine("{0};", new object[]
			{
				Emit.StoreArrayElement(varName, index.ToString(), value, false)
			});
		}

		private static string StorageTypeFor(object value)
		{
			string result;
			if (value is bool)
			{
				result = "bool";
			}
			else if (value is byte)
			{
				result = "uint8_t";
			}
			else if (value is sbyte)
			{
				result = "int8_t";
			}
			else if (value is char)
			{
				result = "Il2CppChar";
			}
			else if (value is short)
			{
				result = "int16_t";
			}
			else if (value is ushort)
			{
				result = "uint16_t";
			}
			else if (value is int)
			{
				result = "int32_t";
			}
			else if (value is uint)
			{
				result = "uint32_t";
			}
			else if (value is long)
			{
				result = "int64_t";
			}
			else if (value is ulong)
			{
				result = "uint64_t";
			}
			else if (value is float)
			{
				result = "float";
			}
			else if (value is double)
			{
				result = "double";
			}
			else if (value is TypeReference)
			{
				result = "Il2CppCodeGenType*";
			}
			else
			{
				if (value is Array)
				{
					throw new NotSupportedException("IL2CPP does not support attributes with arguments that are arrays of arrays.");
				}
				throw new ArgumentException("Unsupported CustomAttribute value of type " + value.GetType().FullName);
			}
			return result;
		}

		private static string FormatForAssignment(object value, IRuntimeMetadataAccess metadataAccess)
		{
			string result;
			if (value == null)
			{
				result = AttributesSupport.Naming.Null;
			}
			else if (value is bool)
			{
				result = value.ToString().ToLower();
			}
			else if (value is byte)
			{
				result = value.ToString().ToLower() + "U";
			}
			else if (value is sbyte)
			{
				result = value.ToString().ToLower();
			}
			else if (value is char)
			{
				result = "'" + value + "'";
			}
			else if (value is short)
			{
				result = value.ToString().ToLower();
			}
			else if (value is ushort)
			{
				result = value.ToString().ToLower() + "U";
			}
			else if (value is int)
			{
				result = value.ToString().ToLower();
			}
			else if (value is uint)
			{
				result = value.ToString().ToLower() + "U";
			}
			else if (value is long)
			{
				result = value.ToString().ToLower();
			}
			else if (value is ulong)
			{
				result = value.ToString().ToLower() + "U";
			}
			else if (value is float)
			{
				result = Formatter.StringRepresentationFor((float)value);
			}
			else if (value is double)
			{
				result = Formatter.StringRepresentationFor((double)value);
			}
			else if (value is string)
			{
				result = AttributesSupport.StringWrapperFor((string)value);
			}
			else
			{
				if (!(value is TypeReference))
				{
					throw new ArgumentException("Unsupported CustomAttribute value of type " + value.GetType().FullName);
				}
				result = AttributesSupport.TypeWrapperFor((TypeReference)value, metadataAccess);
			}
			return result;
		}

		private static string StringWrapperFor(string value)
		{
			string result;
			if (value == null)
			{
				result = AttributesSupport.Naming.Null;
			}
			else
			{
				if (value.Contains("Microsoft") && value.Contains("Visual") && value.Contains("\0ae"))
				{
					value = value.Replace("\0ae", "®");
				}
				result = string.Format("il2cpp_codegen_string_new_wrapper(\"{0}\")", Formatter.EscapeString(value));
			}
			return result;
		}

		private static string TypeWrapperFor(TypeReference typeReference, IRuntimeMetadataAccess metadataAccess)
		{
			string result;
			if (typeReference == null)
			{
				result = AttributesSupport.Naming.Null;
			}
			else
			{
				result = string.Format("il2cpp_codegen_type_get_object({0})", metadataAccess.Il2CppTypeFor(typeReference));
			}
			return result;
		}

		private static string CustomAttributeConstructorFormattedArgumentsFor(CustomAttribute attribute, DefaultRuntimeMetadataAccess metadataAccess)
		{
			return (from a in attribute.ConstructorArguments
			select AttributesSupport.FormatAttributeValue(a, AttributesSupport.TempName(a, attribute), metadataAccess)).Aggregate("tmp", (string buff, string s) => buff + ", " + s);
		}

		private static string FormatAttributeValue(CustomAttributeArgument argument, string tempLocalName, DefaultRuntimeMetadataAccess metadataAccess)
		{
			MetadataType metadataType = argument.Type.MetadataType;
			if (argument.Type.MetadataType == MetadataType.Class && argument.Type.IsEnum())
			{
				metadataType = MetadataType.ValueType;
			}
			string result;
			if (argument.Type.IsEnum())
			{
				TypeReference underlyingEnumType = argument.Type.GetUnderlyingEnumType();
				result = AttributesSupport.FormatAttributeValue(underlyingEnumType.MetadataType, MetadataUtils.ChangePrimitiveType(argument.Value, underlyingEnumType), tempLocalName, metadataAccess);
			}
			else
			{
				result = AttributesSupport.FormatAttributeValue(metadataType, argument.Value, tempLocalName, metadataAccess);
			}
			return result;
		}

		private static string FormatAttributeValue(MetadataType metadataType, object argumentValue, string tempLocalName, DefaultRuntimeMetadataAccess metadataAccess)
		{
			switch (metadataType)
			{
			case MetadataType.Boolean:
			case MetadataType.SByte:
			case MetadataType.Byte:
			case MetadataType.Int16:
			case MetadataType.UInt16:
			{
				string result = argumentValue.ToString().ToLower();
				return result;
			}
			case MetadataType.Char:
			{
				string result = Formatter.FormatChar((char)argumentValue);
				return result;
			}
			case MetadataType.Int32:
			case MetadataType.UInt32:
			case MetadataType.Int64:
			case MetadataType.UInt64:
			{
				string result = argumentValue.ToString().ToLower() + "LL";
				return result;
			}
			case MetadataType.Single:
			{
				string result = Formatter.StringRepresentationFor((float)argumentValue);
				return result;
			}
			case MetadataType.Double:
			{
				string result = Formatter.StringRepresentationFor((double)argumentValue);
				return result;
			}
			case MetadataType.String:
			{
				string result = AttributesSupport.StringWrapperFor((string)argumentValue);
				return result;
			}
			case MetadataType.Class:
			{
				string result = AttributesSupport.TypeWrapperFor((TypeReference)argumentValue, metadataAccess);
				return result;
			}
			case MetadataType.Array:
			{
				string result = tempLocalName;
				return result;
			}
			case MetadataType.Object:
			{
				CustomAttributeArgument customAttributeArgument = (CustomAttributeArgument)argumentValue;
				string result;
				if (customAttributeArgument.Type.MetadataType == MetadataType.String)
				{
					result = AttributesSupport.StringWrapperFor((string)customAttributeArgument.Value);
					return result;
				}
				if (customAttributeArgument.Type.MetadataType == MetadataType.Class && customAttributeArgument.Value is TypeReference)
				{
					result = AttributesSupport.TypeWrapperFor((TypeReference)customAttributeArgument.Value, metadataAccess);
					return result;
				}
				result = Emit.Box(customAttributeArgument.Type, tempLocalName, metadataAccess);
				return result;
			}
			}
			throw new NotSupportedException("Unsupported constructor argument metadata type: " + metadataType);
		}

		[DebuggerHidden]
		private static IEnumerable<TypeReference> ExtractTypeReferencesFromCustomAttributeArguments(IEnumerable<CustomAttributeArgument> arguments)
		{
			AttributesSupport.<ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator0 <ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator = new AttributesSupport.<ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator0();
			<ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator.arguments = arguments;
			AttributesSupport.<ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator0 expr_0E = <ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
