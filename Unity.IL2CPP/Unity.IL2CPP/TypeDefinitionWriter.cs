using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.IL2CPP.Com;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP
{
	public class TypeDefinitionWriter
	{
		private enum LayoutMode
		{
			Managed,
			Native
		}

		private enum FieldType
		{
			Instance,
			Static,
			ThreadStatic
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct FieldWriteInstruction
		{
			public FieldDefinition Field
			{
				get;
				private set;
			}

			public string FieldName
			{
				get;
				private set;
			}

			public string FieldTypeName
			{
				get;
				private set;
			}

			public TypeReference FieldType
			{
				get;
				private set;
			}

			public FieldWriteInstruction(FieldDefinition field, string fieldTypeName, TypeReference fieldType)
			{
				this = default(TypeDefinitionWriter.FieldWriteInstruction);
				this.Field = field;
				this.FieldName = TypeDefinitionWriter.Naming.ForField(field);
				this.FieldTypeName = fieldTypeName;
				this.FieldType = fieldType;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ComFieldWriteInstruction
		{
			public TypeReference InterfaceType
			{
				get;
				private set;
			}

			public bool IsStatic
			{
				get;
				private set;
			}

			public ComFieldWriteInstruction(TypeReference interfaceType, bool isStatic)
			{
				this = default(TypeDefinitionWriter.ComFieldWriteInstruction);
				this.InterfaceType = interfaceType;
				this.IsStatic = isStatic;
			}
		}

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		private const char kArrayFirstIndexName = 'i';

		public void WriteTypeDefinitionFor(TypeReference type, CppCodeWriter writer)
		{
			TypeDefinition typeDefinition = type.Resolve();
			this.CollectIncludes(writer, type, typeDefinition);
			ErrorInformation.CurrentlyProcessing.Type = typeDefinition;
			if (CodeGenOptions.EnableErrorMessageTest)
			{
				ErrorTypeAndMethod.ThrowIfIsErrorType(type.Resolve());
			}
			TypeDefinition typeDefinition2 = type as TypeDefinition;
			if (typeDefinition2 == null || !typeDefinition2.HasGenericParameters)
			{
				writer.WriteClangWarningDisables();
				writer.WriteLine();
				if (type.IsSystemObject())
				{
					writer.WriteLine("struct Il2CppClass;");
				}
				writer.WriteCommentedLine(type.FullName);
				bool flag = TypeDefinitionWriter.NeedsPackingForManaged(typeDefinition) && !typeDefinition.IsExplicitLayout;
				if (flag)
				{
					writer.WriteLine("#pragma pack(push, tp, {0})", new object[]
					{
						TypeDefinitionWriter.FieldLayoutPackingSizeFor(typeDefinition)
					});
				}
				int num = TypeDefinitionWriter.AlignmentSizeFor(typeDefinition);
				string text = "";
				if (num != 1)
				{
					text = "ALIGN_TYPE(" + num + ")";
				}
				if (type.IsSystemObject() || type.IsSystemArray() || type.IsIl2CppComObject())
				{
					writer.AddInclude("object-internals.h");
				}
				else
				{
					writer.WriteLine("struct {0} {1} {2}", new object[]
					{
						text,
						TypeDefinitionWriter.Naming.ForTypeNameOnly(type),
						this.GetBaseTypeDeclaration(type)
					});
					writer.BeginBlock();
					TypeDefinitionWriter.WriteCLSID(writer, typeDefinition);
					TypeDefinitionWriter.WriteFieldsWithAccessors(writer, type, TypeDefinitionWriter.NeedsPackingForManaged(typeDefinition), TypeDefinitionWriter.FieldType.Instance);
					writer.EndBlock(true);
				}
				if (flag)
				{
					writer.WriteLine("#pragma pack(pop, tp)");
				}
				if (typeDefinition.Fields.Any((FieldDefinition f) => f.IsNormalStatic()) || typeDefinition.StoresNonFieldsInStaticFields())
				{
					writer.WriteLine();
					writer.WriteLine("struct {0}", new object[]
					{
						TypeDefinitionWriter.Naming.ForStaticFieldsStruct(type)
					});
					writer.BeginBlock();
					TypeDefinitionWriter.WriteFieldsWithAccessors(writer, type, false, TypeDefinitionWriter.FieldType.Static);
					writer.EndBlock(true);
				}
				if (typeDefinition.Fields.Any((FieldDefinition f) => f.IsThreadStatic()))
				{
					writer.WriteLine();
					writer.WriteLine("struct {0}", new object[]
					{
						TypeDefinitionWriter.Naming.ForThreadFieldsStruct(type)
					});
					writer.BeginBlock();
					TypeDefinitionWriter.WriteFieldsWithAccessors(writer, type, false, TypeDefinitionWriter.FieldType.ThreadStatic);
					writer.EndBlock(true);
				}
				writer.WriteLine();
				writer.WriteClangWarningEnables();
				TypeDefinitionWriter.WriteNativeStructDefinitions(type, writer);
				if (!typeDefinition.IsComOrWindowsRuntimeType())
				{
					if (typeDefinition.Interfaces.Any((TypeReference i) => i.IsComOrWindowsRuntimeInterface()))
					{
						new CCWWriter(typeDefinition).WriteTypeDefinition(writer);
					}
				}
			}
		}

		public void WriteArrayTypeDefinition(TypeReference type, CppCodeWriter writer)
		{
			if (!(type is ArrayType))
			{
				throw new ArgumentException("ArrayType expected", "type");
			}
			ErrorInformation.CurrentlyProcessing.Type = type.Resolve();
			if (CodeGenOptions.EnableErrorMessageTest)
			{
				ErrorTypeAndMethod.ThrowIfIsErrorType(type.Resolve());
			}
			writer.WriteCommentedLine(type.FullName);
			writer.WriteLine("struct {0} {1}", new object[]
			{
				TypeDefinitionWriter.Naming.ForTypeNameOnly(type),
				this.GetBaseTypeDeclaration(type)
			});
			writer.BeginBlock();
			TypeDefinitionWriter.WriteArrayFieldsWithAccessors(writer, (ArrayType)type);
			writer.EndBlock(true);
			TypeDefinitionWriter.WriteNativeStructDefinitions(type, writer);
		}

		private static void WriteNativeStructDefinitions(TypeReference type, CppCodeWriter writer)
		{
			MarshalType[] array = new MarshalType[]
			{
				MarshalType.PInvoke,
				MarshalType.COM
			};
			for (int i = 0; i < array.Length; i++)
			{
				MarshalType marshalType = array[i];
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type, marshalType, null, false, false);
				defaultMarshalInfoWriter.WriteNativeStructDefinition(writer);
			}
		}

		private void CollectIncludes(CppCodeWriter writer, TypeReference type, TypeDefinition typeDefinition)
		{
			if (!type.HasGenericParameters)
			{
				if (type is ArrayType)
				{
					writer.AddIncludeForTypeDefinition(TypeDefinitionWriter.TypeProvider.Corlib.MainModule.GetType("System.Array"));
				}
				else
				{
					TypeResolver typeResolver = new TypeResolver(type as GenericInstanceType);
					TypeReference typeReference = typeResolver.Resolve(typeDefinition.BaseType);
					if (typeReference != null)
					{
						writer.AddIncludeForTypeDefinition(typeReference);
					}
					foreach (FieldDefinition current in typeDefinition.Fields)
					{
						writer.AddIncludesForTypeReference(typeResolver.Resolve(current.FieldType), false);
					}
					foreach (FieldDefinition current2 in typeDefinition.Fields)
					{
						PointerType pointerType = current2.FieldType as PointerType;
						if (pointerType != null)
						{
							writer.AddForwardDeclaration(typeResolver.Resolve(pointerType.ElementType));
						}
					}
					foreach (TypeReference current3 in type.GetActivationFactoryTypes())
					{
						writer.AddForwardDeclaration(current3);
					}
					if (typeDefinition.IsDelegate())
					{
						MethodDefinition[] array;
						if (!typeDefinition.IsWindowsRuntime)
						{
							MethodDefinition[] expr_17E = new MethodDefinition[3];
							expr_17E[0] = typeDefinition.Methods.Single((MethodDefinition m) => m.Name == "Invoke");
							expr_17E[1] = typeDefinition.Methods.Single((MethodDefinition m) => m.Name == "BeginInvoke");
							expr_17E[2] = typeDefinition.Methods.Single((MethodDefinition m) => m.Name == "EndInvoke");
							array = expr_17E;
						}
						else
						{
							MethodDefinition[] expr_20E = new MethodDefinition[1];
							expr_20E[0] = typeDefinition.Methods.Single((MethodDefinition m) => m.Name == "Invoke");
							array = expr_20E;
						}
						MethodDefinition[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							MethodDefinition methodDefinition = array2[i];
							writer.AddIncludesForTypeReference(typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(methodDefinition)), false);
							foreach (ParameterDefinition current4 in methodDefinition.Parameters)
							{
								TypeReference typeReference2 = typeResolver.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(methodDefinition, current4));
								writer.AddIncludesForTypeReference(typeReference2, false);
								if (typeReference2.IsByReference)
								{
									ByReferenceType byReferenceType = (ByReferenceType)typeReference2;
									if (byReferenceType.ElementType.IsValueType())
									{
										typeReference2 = byReferenceType.ElementType;
									}
								}
								if (typeReference2.IsValueType())
								{
									writer.AddIncludeForTypeDefinition(typeReference2);
								}
							}
						}
					}
				}
			}
		}

		internal static bool NeedsPackingForManaged(TypeDefinition typeDefinition)
		{
			return TypeDefinitionWriter.NeedsPacking(typeDefinition, TypeDefinitionWriter.LayoutMode.Managed);
		}

		internal static bool NeedsPackingForNative(TypeDefinition typeDefinition)
		{
			return TypeDefinitionWriter.NeedsPacking(typeDefinition, TypeDefinitionWriter.LayoutMode.Native);
		}

		private static bool NeedsPacking(TypeDefinition typeDefinition, TypeDefinitionWriter.LayoutMode layoutMode)
		{
			return typeDefinition.IsExplicitLayout || ((layoutMode != TypeDefinitionWriter.LayoutMode.Managed || MarshalingUtils.IsBlittable(typeDefinition, null)) && (typeDefinition.IsSequentialLayout && typeDefinition.PackingSize != 0) && typeDefinition.PackingSize != -1);
		}

		internal static int FieldLayoutPackingSizeFor(TypeDefinition typeDefinition)
		{
			int result;
			if (typeDefinition.IsExplicitLayout)
			{
				result = 1;
			}
			else
			{
				result = (int)typeDefinition.PackingSize;
			}
			return result;
		}

		internal static int AlignmentPackingSizeFor(TypeDefinition typeDefinition)
		{
			return (int)typeDefinition.PackingSize;
		}

		internal static bool NeedsPadding(TypeDefinition typeDefinition)
		{
			return typeDefinition.ClassSize > 0;
		}

		internal static int AlignmentSizeFor(TypeDefinition typeDefinition)
		{
			int result;
			foreach (CustomAttribute current in typeDefinition.CustomAttributes)
			{
				if (current.AttributeType.FullName.Contains("IL2CPPStructAlignment"))
				{
					foreach (CustomAttributeNamedArgument current2 in current.Fields)
					{
						if (current2.Name == "Align")
						{
							result = (int)current2.Argument.Value;
							return result;
						}
					}
				}
			}
			result = 1;
			return result;
		}

		private static void WriteCLSID(CppCodeWriter writer, TypeDefinition type)
		{
			if (type.IsImport)
			{
				TypeDefinitionWriter.WriteAccessSpecifier(writer, "public");
				writer.WriteLine("static const Il2CppGuid CLSID;");
				writer.WriteLine();
			}
		}

		private static bool FieldMatches(FieldDefinition field, TypeDefinitionWriter.FieldType fieldType)
		{
			bool result;
			if (fieldType == TypeDefinitionWriter.FieldType.Static)
			{
				result = field.IsNormalStatic();
			}
			else if (fieldType == TypeDefinitionWriter.FieldType.ThreadStatic)
			{
				result = field.IsThreadStatic();
			}
			else
			{
				result = !field.IsStatic;
			}
			return result;
		}

		private static void WriteFieldsWithAccessors(CppCodeWriter writer, TypeReference type, bool needsPacking, TypeDefinitionWriter.FieldType fieldType = TypeDefinitionWriter.FieldType.Instance)
		{
			TypeDefinition typeDefinition = type.Resolve();
			List<TypeDefinitionWriter.FieldWriteInstruction> fieldWriteInstructions = TypeDefinitionWriter.MakeFieldWriteInstructionsForType(writer, type, typeDefinition, fieldType);
			List<TypeDefinitionWriter.ComFieldWriteInstruction> list = TypeDefinitionWriter.MakeComFieldWriteInstructionsForType(writer, type, typeDefinition, fieldType);
			TypeDefinitionWriter.WriteAccessSpecifier(writer, "public");
			if (fieldType == TypeDefinitionWriter.FieldType.Instance)
			{
				using (new TypeDefinitionPaddingWriter(writer, typeDefinition))
				{
					TypeDefinitionWriter.WriteFields(writer, typeDefinition, needsPacking, fieldType, fieldWriteInstructions, list);
				}
			}
			else
			{
				TypeDefinitionWriter.WriteFields(writer, typeDefinition, needsPacking, fieldType, fieldWriteInstructions, list);
			}
			writer.WriteLine();
			TypeDefinitionWriter.WriteAccessSpecifier(writer, "public");
			TypeDefinitionWriter.WriteFieldGettersAndSetters(writer, type, fieldWriteInstructions);
			TypeDefinitionWriter.WriteComFieldGetters(writer, type, list);
		}

		private static void WriteArrayFieldsWithAccessors(CppCodeWriter writer, ArrayType arrayType)
		{
			TypeReference elementType = arrayType.ElementType;
			string elementTypeName = TypeDefinitionWriter.Naming.ForVariable(elementType);
			TypeDefinitionWriter.WriteAccessSpecifier(writer, "public");
			writer.WriteLine("ALIGN_FIELD (8) {0} {1}[1];", new object[]
			{
				TypeDefinitionWriter.Naming.ForVariable(arrayType.ElementType),
				TypeDefinitionWriter.Naming.ForArrayItems()
			});
			writer.WriteLine();
			TypeDefinitionWriter.WriteAccessSpecifier(writer, "public");
			TypeDefinitionWriter.WriteArrayAccessors(writer, elementType, elementTypeName);
			if (arrayType.Rank > 1)
			{
				TypeDefinitionWriter.WriteArrayAccessorsForMultiDimensionalArray(writer, arrayType.Rank, elementType, elementTypeName);
			}
		}

		private static void WriteArrayAccessors(CppCodeWriter writer, TypeReference elementType, string elementTypeName)
		{
			writer.WriteLine("inline {0} {1}({2} {3}) const {{ return {4}[{3}]; }}", new object[]
			{
				elementTypeName,
				TypeDefinitionWriter.Naming.ForArrayItemGetter(),
				TypeDefinitionWriter.Naming.ForArrayIndexType(),
				TypeDefinitionWriter.Naming.ForArrayIndexName(),
				TypeDefinitionWriter.Naming.ForArrayItems()
			});
			writer.WriteLine("inline {0}* {1}({2} {3}) {{ return {4} + {3}; }}", new object[]
			{
				elementTypeName,
				TypeDefinitionWriter.Naming.ForArrayItemAddressGetter(),
				TypeDefinitionWriter.Naming.ForArrayIndexType(),
				TypeDefinitionWriter.Naming.ForArrayIndexName(),
				TypeDefinitionWriter.Naming.ForArrayItems()
			});
			writer.WriteLine("inline void {0}({1} {2}, {3} value)", new object[]
			{
				TypeDefinitionWriter.Naming.ForArrayItemSetter(),
				TypeDefinitionWriter.Naming.ForArrayIndexType(),
				TypeDefinitionWriter.Naming.ForArrayIndexName(),
				elementTypeName
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("{0}[{1}] = value;", new object[]
				{
					TypeDefinitionWriter.Naming.ForArrayItems(),
					TypeDefinitionWriter.Naming.ForArrayIndexName()
				});
				writer.WriteWriteBarrierIfNeeded(elementType, string.Format("{0} + {1}", TypeDefinitionWriter.Naming.ForArrayItems(), TypeDefinitionWriter.Naming.ForArrayIndexName()), "value");
			}
		}

		private static void WriteArrayAccessorsForMultiDimensionalArray(CppCodeWriter writer, int rank, TypeReference elementType, string elementTypeName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = TypeDefinitionWriter.BuildArrayIndexParameters(stringBuilder, rank);
			string block = TypeDefinitionWriter.BuildArrayIndexCalculation(stringBuilder, rank);
			writer.WriteLine("inline {0} {1}({2}) const", new object[]
			{
				elementTypeName,
				TypeDefinitionWriter.Naming.ForArrayItemGetter(),
				text
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(block);
				writer.WriteLine("return {0}[{1}];", new object[]
				{
					TypeDefinitionWriter.Naming.ForArrayItems(),
					TypeDefinitionWriter.Naming.ForArrayIndexName()
				});
			}
			writer.WriteLine("inline {0}* {1}({2})", new object[]
			{
				elementTypeName,
				TypeDefinitionWriter.Naming.ForArrayItemAddressGetter(),
				text
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(block);
				writer.WriteLine("return {0} + {1};", new object[]
				{
					TypeDefinitionWriter.Naming.ForArrayItems(),
					TypeDefinitionWriter.Naming.ForArrayIndexName()
				});
			}
			writer.WriteLine("inline void {0}({1}, {2} value)", new object[]
			{
				TypeDefinitionWriter.Naming.ForArrayItemSetter(),
				text,
				elementTypeName
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(block);
				writer.WriteLine("{0}[{1}] = value;", new object[]
				{
					TypeDefinitionWriter.Naming.ForArrayItems(),
					TypeDefinitionWriter.Naming.ForArrayIndexName()
				});
				writer.WriteWriteBarrierIfNeeded(elementType, string.Format("{0} + {1}", TypeDefinitionWriter.Naming.ForArrayItems(), TypeDefinitionWriter.Naming.ForArrayIndexName()), "value");
			}
		}

		private static string BuildArrayIndexParameters(StringBuilder stringBuilder, int rank)
		{
			stringBuilder.Clear();
			char c = (char)(105 + rank);
			for (char c2 = 'i'; c2 < c; c2 += '\u0001')
			{
				stringBuilder.AppendFormat("{0} {1}", TypeDefinitionWriter.Naming.ForArrayIndexType(), c2);
				if (c2 != c - '\u0001')
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		private static string BuildArrayIndexCalculation(StringBuilder stringBuilder, int rank)
		{
			stringBuilder.Clear();
			stringBuilder.AppendFormat("{0} {1} = ", TypeDefinitionWriter.Naming.ForArrayIndexType(), TypeDefinitionWriter.Naming.ForArrayIndexName());
			for (int i = 0; i < rank - 2; i++)
			{
				stringBuilder.Append('(');
			}
			for (int j = 0; j < rank; j++)
			{
				stringBuilder.Append((char)(105 + j));
				if (j != 0 && j != rank - 1)
				{
					stringBuilder.Append(')');
				}
				if (j != rank - 1)
				{
					stringBuilder.AppendFormat(" * bounds[{0}].length + ", j + 1);
				}
			}
			stringBuilder.Append(';');
			return stringBuilder.ToString();
		}

		private static List<TypeDefinitionWriter.FieldWriteInstruction> MakeFieldWriteInstructionsForType(CppCodeWriter writer, TypeReference type, TypeDefinition typeDefinition, TypeDefinitionWriter.FieldType fieldType)
		{
			List<TypeDefinitionWriter.FieldWriteInstruction> list = new List<TypeDefinitionWriter.FieldWriteInstruction>();
			TypeResolver typeResolver = TypeResolver.For(type);
			foreach (FieldDefinition current in from f in typeDefinition.Fields
			where TypeDefinitionWriter.FieldMatches(f, fieldType)
			select f)
			{
				string fieldTypeName;
				TypeReference typeReference;
				if (current.DeclaringType.FullName == "System.Delegate" && current.Name == "method_ptr")
				{
					fieldTypeName = "Il2CppMethodPointer";
					typeReference = current.FieldType;
				}
				else
				{
					FieldReference field = new FieldReference(current.Name, current.FieldType, type);
					typeReference = typeResolver.ResolveFieldType(field);
					fieldTypeName = TypeDefinitionWriter.Naming.ForVariable(typeReference);
				}
				list.Add(new TypeDefinitionWriter.FieldWriteInstruction(current, fieldTypeName, typeReference));
			}
			return list;
		}

		private static List<TypeDefinitionWriter.ComFieldWriteInstruction> MakeComFieldWriteInstructionsForType(CppCodeWriter writer, TypeReference type, TypeDefinition typeDefinition, TypeDefinitionWriter.FieldType fieldType)
		{
			List<TypeDefinitionWriter.ComFieldWriteInstruction> result;
			if (!typeDefinition.IsComOrWindowsRuntimeType())
			{
				result = new List<TypeDefinitionWriter.ComFieldWriteInstruction>();
			}
			else
			{
				TypeReference[] arg_5B_0;
				if (fieldType == TypeDefinitionWriter.FieldType.Static)
				{
					arg_5B_0 = typeDefinition.GetActivationFactoryTypes().ToArray<TypeReference>();
				}
				else
				{
					arg_5B_0 = (from i in typeDefinition.Interfaces
					where i.IsComOrWindowsRuntimeInterface()
					select i).ToArray<TypeReference>();
				}
				TypeReference[] array = arg_5B_0;
				List<TypeDefinitionWriter.ComFieldWriteInstruction> list = new List<TypeDefinitionWriter.ComFieldWriteInstruction>(array.Length);
				TypeResolver typeResolver = TypeResolver.For(type);
				bool flag = false;
				TypeReference[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					TypeReference typeReference = array2[j];
					if (typeReference.IsIActivationFactory())
					{
						flag = true;
					}
					list.Add(new TypeDefinitionWriter.ComFieldWriteInstruction(typeResolver.Resolve(typeReference), fieldType == TypeDefinitionWriter.FieldType.Static));
				}
				if (!flag && fieldType == TypeDefinitionWriter.FieldType.Static && list.Count > 0)
				{
					list.Insert(0, new TypeDefinitionWriter.ComFieldWriteInstruction(TypeDefinitionWriter.TypeProvider.IActivationFactoryTypeReference, true));
				}
				result = list;
			}
			return result;
		}

		private static void WriteFields(CppCodeWriter writer, TypeDefinition typeDefinition, bool needsPacking, TypeDefinitionWriter.FieldType fieldType, List<TypeDefinitionWriter.FieldWriteInstruction> fieldWriteInstructions, List<TypeDefinitionWriter.ComFieldWriteInstruction> comFieldWriteInstructions)
		{
			bool flag = typeDefinition.IsExplicitLayout && fieldType == TypeDefinitionWriter.FieldType.Instance;
			if (flag)
			{
				writer.WriteLine("union");
				writer.BeginBlock();
			}
			foreach (TypeDefinitionWriter.FieldWriteInstruction current in fieldWriteInstructions)
			{
				TypeDefinitionWriter.WriteFieldInstruction(writer, typeDefinition, needsPacking, flag, current, false);
				if (flag)
				{
					TypeDefinitionWriter.WriteFieldInstruction(writer, typeDefinition, false, true, current, true);
				}
			}
			if (flag)
			{
				writer.EndBlock(true);
			}
			foreach (TypeDefinitionWriter.ComFieldWriteInstruction current2 in comFieldWriteInstructions)
			{
				writer.WriteCommentedLine(string.Format("Cached pointer to {0}", current2.InterfaceType.FullName));
				writer.WriteLine("{0}* {1};", new object[]
				{
					TypeDefinitionWriter.Naming.ForTypeNameOnly(current2.InterfaceType),
					TypeDefinitionWriter.Naming.ForComTypeInterfaceFieldName(current2.InterfaceType)
				});
			}
		}

		private static void WriteFieldInstruction(CppCodeWriter writer, TypeDefinition typeDefinition, bool needsPacking, bool explicitLayout, TypeDefinitionWriter.FieldWriteInstruction instruction, bool forAlignmentOnly = false)
		{
			int num = TypeDefinitionWriter.AlignmentPackingSizeFor(typeDefinition);
			bool flag = needsPacking || (forAlignmentOnly && num != -1 && num != 0);
			string str = (!forAlignmentOnly) ? string.Empty : "_forAlignmentOnly";
			if (explicitLayout)
			{
				if (flag)
				{
					writer.WriteLine("#pragma pack(push, tp, {0})", new object[]
					{
						(!forAlignmentOnly) ? TypeDefinitionWriter.FieldLayoutPackingSizeFor(typeDefinition) : num
					});
				}
				writer.WriteLine("struct");
				writer.BeginBlock();
				int offset = instruction.Field.Offset;
				if (offset > 0)
				{
					writer.WriteLine("char {0}[{1}];", new object[]
					{
						TypeDefinitionWriter.Naming.ForFieldPadding(instruction.Field) + str,
						offset
					});
				}
			}
			if (!forAlignmentOnly)
			{
				writer.WriteCommentedLine(instruction.Field.FullName);
			}
			writer.Write(string.Format("{0} {1}", instruction.FieldTypeName, instruction.FieldName + str));
			writer.WriteLine(";");
			if (explicitLayout)
			{
				writer.EndBlock(true);
				if (flag)
				{
					writer.WriteLine("#pragma pack(pop, tp)");
				}
			}
		}

		private static void WriteFieldGettersAndSetters(CppCodeWriter writer, TypeReference declaringType, List<TypeDefinitionWriter.FieldWriteInstruction> fieldWriteInstructions)
		{
			for (int i = 0; i < fieldWriteInstructions.Count; i++)
			{
				TypeDefinitionWriter.FieldWriteInstruction fieldWriteInstruction = fieldWriteInstructions[i];
				writer.WriteLine("inline static int32_t {0}() {{ return static_cast<int32_t>(offsetof({1}, {2})); }}", new object[]
				{
					TypeDefinitionWriter.Naming.ForFieldOffsetGetter(fieldWriteInstruction.Field),
					TypeDefinitionWriter.GetDeclaringTypeStructName(declaringType, fieldWriteInstruction.Field),
					TypeDefinitionWriter.Naming.ForField(fieldWriteInstruction.Field)
				});
				writer.WriteLine("inline {0} {1}() const {{ return {2}; }}", new object[]
				{
					fieldWriteInstruction.FieldTypeName,
					TypeDefinitionWriter.Naming.ForFieldGetter(fieldWriteInstruction.Field),
					fieldWriteInstruction.FieldName
				});
				writer.WriteLine("inline {0}* {1}() {{ return &{2}; }}", new object[]
				{
					fieldWriteInstruction.FieldTypeName,
					TypeDefinitionWriter.Naming.ForFieldAddressGetter(fieldWriteInstruction.Field),
					fieldWriteInstruction.FieldName
				});
				writer.WriteLine("inline void {0}({1} value)", new object[]
				{
					TypeDefinitionWriter.Naming.ForFieldSetter(fieldWriteInstruction.Field),
					fieldWriteInstruction.FieldTypeName
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("{0} = value;", new object[]
					{
						fieldWriteInstruction.FieldName
					});
					writer.WriteWriteBarrierIfNeeded(fieldWriteInstruction.FieldType, TypeDefinitionWriter.Naming.AddressOf(fieldWriteInstruction.FieldName), "value");
				}
				if (i != fieldWriteInstructions.Count - 1)
				{
					writer.WriteLine();
				}
			}
		}

		private static void WriteComFieldGetters(CppCodeWriter writer, TypeReference declaringType, List<TypeDefinitionWriter.ComFieldWriteInstruction> fieldWriteInstructions)
		{
			for (int i = 0; i < fieldWriteInstructions.Count; i++)
			{
				TypeDefinitionWriter.ComFieldWriteInstruction comFieldWriteInstruction = fieldWriteInstructions[i];
				TypeReference interfaceType = comFieldWriteInstruction.InterfaceType;
				string text = TypeDefinitionWriter.Naming.ForTypeNameOnly(interfaceType);
				string text2 = TypeDefinitionWriter.Naming.ForComTypeInterfaceFieldName(interfaceType);
				string text3 = TypeDefinitionWriter.Naming.ForInteropReturnValue();
				if (i != 0)
				{
					writer.WriteLine();
				}
				writer.AddIncludeForTypeDefinition(interfaceType);
				writer.WriteLine("inline {0}* {1}()", new object[]
				{
					text,
					TypeDefinitionWriter.Naming.ForComTypeInterfaceFieldGetter(interfaceType)
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("{0}* {1} = {2};", new object[]
					{
						text,
						text3,
						text2
					});
					writer.WriteLine("if ({0} == NULL)", new object[]
					{
						text3
					});
					using (new BlockWriter(writer, false))
					{
						if (comFieldWriteInstruction.IsStatic && interfaceType.IsIActivationFactory())
						{
							writer.WriteLine("il2cpp::utils::StringView<Il2CppNativeChar> className(IL2CPP_NATIVE_STRING(\"{0}\"));", new object[]
							{
								declaringType.FullName
							});
							writer.WriteStatement(Emit.Assign(text3, "il2cpp_codegen_windows_runtime_get_activation_factory(className)"));
						}
						else
						{
							string arg = (!comFieldWriteInstruction.IsStatic) ? TypeDefinitionWriter.Naming.ForIl2CppComObjectIdentityField() : Emit.Call(TypeDefinitionWriter.Naming.ForComTypeInterfaceFieldGetter(TypeDefinitionWriter.TypeProvider.IActivationFactoryTypeReference));
							string left = string.Format("const il2cpp_hresult_t {0}", TypeDefinitionWriter.Naming.ForInteropHResultVariable());
							string right = string.Format("{0}->QueryInterface({1}::IID, reinterpret_cast<void**>(&{2}))", arg, text, text3);
							writer.WriteStatement(Emit.Assign(left, right));
							writer.WriteStatement("il2cpp_codegen_com_raise_exception_if_failed(hr)");
						}
						writer.WriteLine();
						writer.WriteLine("if (il2cpp_codegen_atomic_compare_exchange_pointer<{0}>({1}, {2}, {3}) != NULL)", new object[]
						{
							text,
							TypeDefinitionWriter.Naming.AddressOf(text2),
							text3,
							TypeDefinitionWriter.Naming.Null
						});
						using (new BlockWriter(writer, false))
						{
							writer.WriteLine("{0}->Release();", new object[]
							{
								text3
							});
							writer.WriteStatement(Emit.Assign(text3, text2));
						}
					}
					writer.WriteLine("return {0};", new object[]
					{
						text3
					});
				}
			}
		}

		private static string GetDeclaringTypeStructName(TypeReference declaringType, FieldReference field)
		{
			string result;
			if (field.IsThreadStatic())
			{
				result = TypeDefinitionWriter.Naming.ForThreadFieldsStruct(declaringType);
			}
			else if (field.IsNormalStatic())
			{
				result = TypeDefinitionWriter.Naming.ForStaticFieldsStruct(declaringType);
			}
			else
			{
				result = TypeDefinitionWriter.Naming.ForTypeNameOnly(declaringType);
			}
			return result;
		}

		private string GetBaseTypeDeclaration(TypeReference type)
		{
			string result;
			if (type.IsArray)
			{
				result = string.Format(" : public " + TypeDefinitionWriter.Naming.ForType(TypeDefinitionWriter.TypeProvider.Corlib.MainModule.GetType("System.Array")), new object[0]);
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				if (typeDefinition.BaseType != null && typeDefinition.BaseType.FullName != "System.Enum" && (typeDefinition.BaseType.FullName != "System.ValueType" || typeDefinition.FullName == "System.Enum"))
				{
					TypeResolver typeResolver = TypeResolver.For(type);
					result = string.Format(" : public " + TypeDefinitionWriter.Naming.ForType(typeResolver.Resolve(typeDefinition.BaseType)), new object[0]);
				}
				else
				{
					result = string.Empty;
				}
			}
			return result;
		}

		private static void WriteAccessSpecifier(CppCodeWriter writer, string accessSpecifier)
		{
			writer.Dedent();
			writer.WriteLine("{0}:", new object[]
			{
				accessSpecifier
			});
			writer.Indent();
		}
	}
}
