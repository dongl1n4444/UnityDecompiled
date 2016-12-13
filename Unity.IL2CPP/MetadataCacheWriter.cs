using Mono.Cecil;
using Mono.Cecil.Rocks;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.GenericsCollection;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
using Unity.IL2CPP.Metadata;
using Unity.IL2CPP.Metadata.Fields;
using Unity.IL2CPP.StringLiterals;
using Unity.TinyProfiling;

namespace Unity.IL2CPP
{
	public sealed class MetadataCacheWriter
	{
		private enum PackingSize
		{
			Zero,
			One,
			Two,
			Four,
			Eight,
			Sixteen,
			ThirtyTwo,
			SixtyFour,
			OneHundredTwentyEight
		}

		[Inject]
		public static IRuntimeInvokerCollectorReaderService RuntimeInvokerCollectorReader;

		[Inject]
		public static IIl2CppGenericInstCollectorReaderService Il2CppGenericInstCollector;

		[Inject]
		public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		[Inject]
		public static IIl2CppTypeCollectorReaderService Il2CppTypeCollectorReader;

		[Inject]
		public static IIl2CppFieldReferenceCollectorReaderService FieldReferenceCollector;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static IStatsService StatsService;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IVirtualCallCollectorService VirtualCallCollector;

		private const int kMinimumStreamAlignment = 4;

		[CompilerGenerated]
		private static Func<FieldDefinition, string> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<TypeReference, bool> <>f__mg$cache1;

		public static void WriteMetadata(NPath outputDir, NPath dataFolder, InflatedCollectionCollector generics, TypeDefinition[] allTypeDefinitions, ICollection<AssemblyDefinition> usedAssemblies, MethodTables methodTables, IMetadataCollection metadataCollector, AttributeCollection attributeCollection, VTableBuilder vTableBuilder, IMethodCollectorResults methodCollector, UnresolvedVirtualsTablesInfo virtualCallTables)
		{
			List<TableInfo> list = new List<TableInfo>();
			List<KeyValuePair<uint, uint>> usagePairs = new List<KeyValuePair<uint, uint>>();
			List<KeyValuePair<uint, uint>> usageLists = new List<KeyValuePair<uint, uint>>();
			TableInfo item2;
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				"Il2CppMetadataUsage.cpp"
			})))
			{
				MetadataUsageWriter metadataUsageWriter = new MetadataUsageWriter(sourceCodeWriter);
				item2 = metadataUsageWriter.WriteMetadataUsage(metadataCollector, usagePairs, usageLists);
			}
			KeyValuePair<int, int>[] fieldRefs = (from item in MetadataCacheWriter.FieldReferenceCollector.Fields
			orderby item.Value
			select new KeyValuePair<int, int>(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(item.Key.DeclaringType, 0), MetadataCacheWriter.Naming.GetFieldIndex(item.Key, false))).ToArray<KeyValuePair<int, int>>();
			using (TinyProfiler.Section("GenericClasses", ""))
			{
				using (CppCodeWriter metadataCodeWriter = MetadataCacheWriter.GetMetadataCodeWriter(outputDir, "GenericClass"))
				{
					TypeReference[] array = (from t in MetadataCacheWriter.Il2CppTypeCollectorReader.Items
					where t.Key.Type.IsGenericInstance
					select t.Key.Type).Distinct(new TypeReferenceEqualityComparer()).ToArray<TypeReference>();
					TypeReference[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						TypeReference type = array2[i];
						metadataCodeWriter.WriteExternForGenericClass(type);
					}
					list.Add(MetadataWriter.WriteTable<TypeReference>(metadataCodeWriter, "extern Il2CppGenericClass* const", "s_Il2CppGenericTypes", array, (TypeReference t) => "&" + MetadataCacheWriter.Naming.ForGenericClass(t)));
				}
			}
			using (TinyProfiler.Section("GenericInst2", ""))
			{
				using (SourceCodeWriter sourceCodeWriter2 = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppGenericInstDefinitions.cpp"
				})))
				{
					list.Add(new Il2CppGenericInstWriter(sourceCodeWriter2).WriteIl2CppGenericInstDefinitions());
				}
			}
			using (TinyProfiler.Section("GenericMethods", ""))
			{
				using (CppCodeWriter metadataCodeWriter2 = MetadataCacheWriter.GetMetadataCodeWriter(outputDir, "GenericMethod"))
				{
					MethodReference[] items = (from m in MetadataCacheWriter.Il2CppGenericMethodCollector.Items.Keys
					where !m.HasGenericParameters && !m.ContainsGenericParameters() && MetadataCacheWriter.TypeDoesNotExceedMaximumRecursion(m.DeclaringType)
					select m).ToArray<MethodReference>();
					list.Add(MetadataWriter.WriteTable<MethodReference>(metadataCodeWriter2, "extern const Il2CppGenericMethodFunctionsDefinitions", "s_Il2CppGenericMethodFunctions", items, (MethodReference m) => MetadataCacheWriter.FormatMethodTableEntry(m, methodTables.MethodPointers)));
				}
			}
			using (TinyProfiler.Section("Il2CppTypes", ""))
			{
				using (SourceCodeWriter sourceCodeWriter3 = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppTypeDefinitions.cpp"
				})))
				{
					list.Add(new Il2CppTypeWriter(sourceCodeWriter3).WriteIl2CppTypeDefinitions(metadataCollector));
				}
			}
			using (TinyProfiler.Section("GenericMethods", ""))
			{
				using (SourceCodeWriter sourceCodeWriter4 = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppGenericMethodDefinitions.cpp"
				})))
				{
					list.Add(new Il2CppGenericMethodWriter(sourceCodeWriter4).WriteIl2CppGenericMethodDefinitions(metadataCollector));
				}
			}
			using (TinyProfiler.Section("CompilerCalculateTypeValues", ""))
			{
				using (CppCodeWriter metadataCodeWriter3 = MetadataCacheWriter.GetMetadataCodeWriter(outputDir, "CompilerCalculateTypeValues"))
				{
					int num = 0;
					List<TableInfo> list2 = new List<TableInfo>();
					int num2 = 0;
					foreach (List<TypeDefinition> current in metadataCollector.GetTypeInfos().Chunk(100))
					{
						using (CppCodeWriter metadataCodeWriter4 = MetadataCacheWriter.GetMetadataCodeWriter(outputDir, "CompilerCalculateTypeValues_" + num))
						{
							IncludeWriter.WriteRegistrationIncludes(metadataCodeWriter4);
							metadataCodeWriter4.WriteClangWarningDisables();
							foreach (TypeDefinition current2 in current)
							{
								metadataCodeWriter4.AddIncludeForTypeDefinition(current2);
								metadataCodeWriter4.WriteLine("extern const Il2CppTypeDefinitionSizes g_typeDefinitionSize{0} = {{ {1} }};", new object[]
								{
									num2,
									MetadataCacheWriter.Sizes(current2)
								});
								List<TableInfo> arg_4F2_0 = list2;
								CppCodeWriter arg_4ED_0 = metadataCodeWriter4;
								string arg_4ED_1 = "extern const int32_t";
								string arg_4ED_2 = "g_FieldOffsetTable" + num2;
								ICollection<FieldDefinition> arg_4ED_3 = current2.Fields;
								if (MetadataCacheWriter.<>f__mg$cache0 == null)
								{
									MetadataCacheWriter.<>f__mg$cache0 = new Func<FieldDefinition, string>(MetadataCacheWriter.OffsetOf);
								}
								arg_4F2_0.Add(MetadataWriter.WriteTable<FieldDefinition>(arg_4ED_0, arg_4ED_1, arg_4ED_2, arg_4ED_3, MetadataCacheWriter.<>f__mg$cache0));
								num2++;
							}
							metadataCodeWriter4.WriteClangWarningEnables();
						}
						num++;
					}
					list.Add(MetadataCacheWriter.WriteFieldTable(metadataCodeWriter3, list2));
					list.Add(MetadataCacheWriter.WriteTypeDefinitionSizesTable(metadataCodeWriter3, metadataCollector, attributeCollection));
				}
			}
			list.Add(item2);
			using (TinyProfiler.Section("Registration", ""))
			{
				using (SourceCodeWriter sourceCodeWriter5 = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppMetadataRegistration.cpp"
				})))
				{
					IncludeWriter.WriteRegistrationIncludes(sourceCodeWriter5);
					foreach (TableInfo current3 in list)
					{
						sourceCodeWriter5.WriteLine("{0} {1}[];", new object[]
						{
							current3.Type,
							current3.Name
						});
					}
					sourceCodeWriter5.WriteStructInitializer("extern const Il2CppMetadataRegistration", "g_MetadataRegistration", list.SelectMany((TableInfo table) => new string[]
					{
						table.Count.ToString(CultureInfo.InvariantCulture),
						table.Name
					}));
				}
			}
			NPath nPath = dataFolder.Combine(new string[]
			{
				"Metadata"
			});
			nPath.CreateDirectory();
			using (FileStream fileStream = new FileStream(nPath.Combine(new string[]
			{
				"global-metadata.dat"
			}).ToString(), FileMode.Create, FileAccess.Write))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						using (TinyProfiler.Section("StringLiteralWriter", ""))
						{
							using (MemoryStream memoryStream3 = new MemoryStream())
							{
								using (MemoryStream memoryStream4 = new MemoryStream())
								{
									new StringLiteralWriter().Write(memoryStream3, memoryStream4);
									memoryStream4.AlignTo(4);
									MetadataCacheWriter.AddStreamAndRecordHeader("String Literals", memoryStream, memoryStream2, memoryStream3);
									MetadataCacheWriter.AddStreamAndRecordHeader("String Literal Data", memoryStream, memoryStream2, memoryStream4);
								}
							}
						}
						MetadataCacheWriter.WriteMetadataToStream("Metadata Strings", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							byte[] array3 = metadataCollector.GetStringData().ToArray<byte>();
							stream.Write(array3, 0, array3.Length);
							stream.AlignTo(4);
						});
						MetadataCacheWriter.WriteMetadataToStream("Events", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (EventDefinition current4 in metadataCollector.GetEvents())
							{
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(current4.EventType, 0));
								stream.WriteInt((current4.AddMethod == null) ? -1 : (metadataCollector.GetMethodIndex(current4.AddMethod) - metadataCollector.GetMethodIndex(current4.DeclaringType.Methods[0])));
								stream.WriteInt((current4.RemoveMethod == null) ? -1 : (metadataCollector.GetMethodIndex(current4.RemoveMethod) - metadataCollector.GetMethodIndex(current4.DeclaringType.Methods[0])));
								stream.WriteInt((current4.InvokeMethod == null) ? -1 : (metadataCollector.GetMethodIndex(current4.InvokeMethod) - metadataCollector.GetMethodIndex(current4.DeclaringType.Methods[0])));
								stream.WriteInt((int)attributeCollection.GetIndex(current4));
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Properties", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (PropertyDefinition current4 in metadataCollector.GetProperties())
							{
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteInt((current4.GetMethod == null) ? -1 : (metadataCollector.GetMethodIndex(current4.GetMethod) - metadataCollector.GetMethodIndex(current4.DeclaringType.Methods[0])));
								stream.WriteInt((current4.SetMethod == null) ? -1 : (metadataCollector.GetMethodIndex(current4.SetMethod) - metadataCollector.GetMethodIndex(current4.DeclaringType.Methods[0])));
								stream.WriteInt((int)current4.Attributes);
								stream.WriteInt((int)attributeCollection.GetIndex(current4));
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Methods", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (MethodDefinition current4 in metadataCollector.GetMethods())
							{
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteInt(metadataCollector.GetTypeInfoIndex(current4.DeclaringType));
								stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(current4.ReturnType, 0));
								stream.WriteInt((!current4.HasParameters) ? -1 : metadataCollector.GetParameterIndex(current4.Parameters[0]));
								stream.WriteInt((int)attributeCollection.GetIndex(current4));
								stream.WriteInt(metadataCollector.GetGenericContainerIndex(current4));
								stream.WriteInt(methodCollector.GetMethodIndex(current4));
								stream.WriteInt(MetadataCacheWriter.RuntimeInvokerCollectorReader.GetIndex(current4));
								stream.WriteInt(methodCollector.GetReversePInvokeWrapperIndex(current4));
								stream.WriteInt(metadataCollector.GetRGCTXEntriesStartIndex(current4));
								stream.WriteInt(metadataCollector.GetRGCTXEntriesCount(current4));
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
								stream.WriteUShort((ushort)current4.Attributes);
								stream.WriteUShort((ushort)current4.ImplAttributes);
								stream.WriteUShort((ushort)vTableBuilder.IndexFor(current4));
								stream.WriteUShort((ushort)current4.Parameters.Count);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Parameter Default Values", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (ParameterDefaultValue current4 in metadataCollector.GetParameterDefaultValues())
							{
								stream.WriteInt(current4._parameterIndex);
								stream.WriteInt(current4._typeIndex);
								stream.WriteInt(current4._dataIndex);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Field Default Values", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (FieldDefaultValue current4 in metadataCollector.GetFieldDefaultValues())
							{
								stream.WriteInt(current4._fieldIndex);
								stream.WriteInt(current4._typeIndex);
								stream.WriteInt(current4._dataIndex);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Field and Parameter Default Values Data", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							byte[] array3 = metadataCollector.GetDefaultValueData().ToArray<byte>();
							stream.Write(array3, 0, array3.Length);
							stream.AlignTo(4);
						});
						MetadataCacheWriter.WriteMetadataToStream("Field Marshaled Sizes", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (FieldMarshaledSize current4 in metadataCollector.GetFieldMarshaledSizes())
							{
								stream.WriteInt(current4._fieldIndex);
								stream.WriteInt(current4._typeIndex);
								stream.WriteInt(current4._size);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Parameters", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (ParameterDefinition current4 in metadataCollector.GetParameters())
							{
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
								stream.WriteInt((int)attributeCollection.GetIndex(current4, (MethodDefinition)current4.Method));
								stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(current4.ParameterType, (int)current4.Attributes));
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Fields", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (FieldDefinition current4 in metadataCollector.GetFields())
							{
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(current4.FieldType, (int)current4.Attributes));
								stream.WriteInt((int)attributeCollection.GetIndex(current4));
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Generic Parameters", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (GenericParameter current4 in metadataCollector.GetGenericParameters())
							{
								stream.WriteInt(metadataCollector.GetGenericContainerIndex(current4.Owner));
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteShort((short)((current4.Constraints.Count <= 0) ? 0 : metadataCollector.GetGenericParameterConstraintsStartIndex(current4)));
								stream.WriteShort((short)current4.Constraints.Count);
								stream.WriteUShort((ushort)current4.Position);
								stream.WriteUShort((ushort)current4.Attributes);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Generic Parameter Constraints", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (int current4 in metadataCollector.GetGenericParameterConstraints())
							{
								stream.WriteInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Generic Containers", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (IGenericParameterProvider current4 in metadataCollector.GetGenericContainers())
							{
								stream.WriteInt((current4.GenericParameterType != GenericParameterType.Method) ? metadataCollector.GetTypeInfoIndex((TypeDefinition)current4) : metadataCollector.GetMethodIndex((MethodDefinition)current4));
								stream.WriteInt(current4.GenericParameters.Count);
								stream.WriteInt((current4.GenericParameterType != GenericParameterType.Method) ? 0 : 1);
								stream.WriteInt(metadataCollector.GetGenericParameterIndex(current4.GenericParameters[0]));
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Nested Types", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (int current4 in metadataCollector.GetNestedTypes())
							{
								stream.WriteInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Interfaces", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (int current4 in metadataCollector.GetInterfaces())
							{
								stream.WriteInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("VTables", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (uint current4 in metadataCollector.GetVTableMethods())
							{
								stream.WriteUInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Interface Offsets", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (KeyValuePair<int, int> current4 in metadataCollector.GetInterfaceOffsets())
							{
								stream.WriteInt(current4.Key);
								stream.WriteInt(current4.Value);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Type Definitions", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (TypeDefinition current4 in metadataCollector.GetTypeInfos())
							{
								int value = 0;
								int value2 = 0;
								if (!current4.IsInterface || current4.IsComOrWindowsRuntimeType())
								{
									VTable vTable = vTableBuilder.VTableFor(current4, null);
									value = vTable.Slots.Count;
									value2 = vTable.InterfaceOffsets.Count;
								}
								TypeReference typeReference = MetadataCacheWriter.BaseTypeFor(current4);
								TypeReference typeReference2 = MetadataCacheWriter.DeclaringTypeFor(current4);
								TypeReference typeReference3 = MetadataCacheWriter.ElementTypeFor(current4);
								int num3 = 0;
								int value3 = -1;
								if (current4.HasMethods)
								{
									MethodDefinition[] array3 = (from m in current4.Methods
									where !m.IsStripped()
									select m).ToArray<MethodDefinition>();
									num3 = array3.Length;
									if (num3 != 0)
									{
										value3 = metadataCollector.GetMethodIndex(array3[0]);
									}
								}
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name));
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Namespace));
								stream.WriteUInt(attributeCollection.GetIndex(current4));
								stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(current4, 0));
								stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(new ByReferenceType(current4), 0));
								stream.WriteInt((typeReference2 == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(typeReference2, 0));
								stream.WriteInt((typeReference == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(typeReference, 0));
								stream.WriteInt((typeReference3 == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(typeReference3, 0));
								stream.WriteInt(metadataCollector.GetRGCTXEntriesStartIndex(current4));
								stream.WriteInt(metadataCollector.GetRGCTXEntriesCount(current4));
								stream.WriteInt(metadataCollector.GetGenericContainerIndex(current4));
								int arg_221_1;
								if (current4.IsDelegate())
								{
									arg_221_1 = methodCollector.GetWrapperForDelegateFromManagedToNativedIndex(current4.Methods.Single((MethodDefinition m) => m.Name == "Invoke"));
								}
								else
								{
									arg_221_1 = -1;
								}
								stream.WriteInt(arg_221_1);
								stream.WriteInt(methodCollector.GetTypeMarshalingFunctionsIndex(current4));
								stream.WriteInt(methodCollector.GetCCWMarshalingFunctionIndex(current4));
								stream.WriteInt(metadataCollector.GetGuidIndex(current4));
								stream.WriteUInt((uint)current4.Attributes);
								stream.WriteInt((!current4.HasFields) ? -1 : metadataCollector.GetFieldIndex(current4.Fields[0]));
								stream.WriteInt(value3);
								stream.WriteInt((!current4.HasEvents) ? -1 : metadataCollector.GetEventIndex(current4.Events[0]));
								stream.WriteInt((!current4.HasProperties) ? -1 : metadataCollector.GetPropertyIndex(current4.Properties[0]));
								stream.WriteInt((!current4.HasNestedTypes) ? -1 : metadataCollector.GetNestedTypesStartIndex(current4));
								stream.WriteInt((!current4.HasInterfaces) ? -1 : metadataCollector.GetInterfacesStartIndex(current4));
								stream.WriteInt(metadataCollector.GetVTableMethodsStartIndex(current4));
								stream.WriteInt((current4.IsInterface && !current4.IsComOrWindowsRuntimeType()) ? -1 : metadataCollector.GetInterfaceOffsetsStartIndex(current4));
								stream.WriteIntAsUShort(num3);
								stream.WriteIntAsUShort(current4.Properties.Count);
								stream.WriteIntAsUShort(current4.Fields.Count);
								stream.WriteIntAsUShort(current4.Events.Count);
								stream.WriteIntAsUShort(current4.NestedTypes.Count);
								stream.WriteIntAsUShort(value);
								stream.WriteIntAsUShort(current4.Interfaces.Count);
								stream.WriteIntAsUShort(value2);
								int num4 = 0;
								num4 |= ((!current4.IsValueType) ? 0 : 1) << 0;
								num4 |= ((!current4.IsEnum) ? 0 : 1) << 1;
								num4 |= ((!current4.HasFinalizer()) ? 0 : 1) << 2;
								num4 |= ((!current4.HasStaticConstructor()) ? 0 : 1) << 3;
								num4 |= ((!MarshalingUtils.IsBlittable(current4, null, MarshalType.PInvoke)) ? 0 : 1) << 4;
								num4 |= ((!current4.IsComOrWindowsRuntimeType()) ? 0 : 1) << 5;
								int num5 = TypeDefinitionWriter.FieldLayoutPackingSizeFor(current4);
								if (num5 != -1)
								{
									num4 |= (int)((int)MetadataCacheWriter.ConvertPackingSizeToCompressedEnum(num5) << 6);
								}
								stream.WriteInt(num4);
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("RGCTX Entries", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (KeyValuePair<int, uint> current4 in metadataCollector.GetRGCTXEntries())
							{
								stream.WriteInt(current4.Key);
								stream.WriteUInt(current4.Value);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Images", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (ModuleDefinition current4 in metadataCollector.GetModules())
							{
								stream.WriteInt(metadataCollector.GetStringIndex(Path.GetFileName(current4.FullyQualifiedName)));
								stream.WriteInt(metadataCollector.GetAssemblyIndex(current4.Assembly));
								stream.WriteInt(metadataCollector.GetTypeInfoIndex(current4.Types[0]));
								stream.WriteInt(current4.GetAllTypes().Count<TypeDefinition>());
								stream.WriteInt((current4.Assembly.EntryPoint != null) ? metadataCollector.GetMethodIndex(current4.Assembly.EntryPoint) : -1);
								stream.WriteUInt(current4.MetadataToken.ToUInt32());
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Assemblies", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (AssemblyDefinition current4 in metadataCollector.GetAssemblies())
							{
								stream.WriteInt(metadataCollector.GetModuleIndex(current4.MainModule));
								stream.WriteInt((int)attributeCollection.GetIndex(current4));
								int value;
								int firstIndexInReferencedAssemblyTableForAssembly = metadataCollector.GetFirstIndexInReferencedAssemblyTableForAssembly(current4, out value);
								stream.WriteInt(firstIndexInReferencedAssemblyTableForAssembly);
								stream.WriteInt(value);
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name.Name));
								stream.WriteInt(metadataCollector.GetStringIndex(current4.Name.Culture));
								stream.WriteInt(metadataCollector.GetStringIndex(Formatter.Stringify(current4.Name.Hash)));
								stream.WriteInt(metadataCollector.GetStringIndex(Formatter.Stringify(current4.Name.PublicKey)));
								stream.WriteUInt((uint)current4.Name.HashAlgorithm);
								stream.WriteInt(current4.Name.Hash.Length);
								stream.WriteUInt((uint)current4.Name.Attributes);
								stream.WriteInt(current4.Name.Version.Major);
								stream.WriteInt(current4.Name.Version.Minor);
								stream.WriteInt(current4.Name.Version.Build);
								stream.WriteInt(current4.Name.Version.Revision);
								byte[] array3 = (current4.Name.PublicKeyToken.Length <= 0) ? new byte[8] : current4.Name.PublicKeyToken;
								byte[] array4 = array3;
								for (int j = 0; j < array4.Length; j++)
								{
									byte value2 = array4[j];
									stream.WriteByte(value2);
								}
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Metadata Usage Lists", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (KeyValuePair<uint, uint> current4 in usageLists)
							{
								stream.WriteUInt(current4.Key);
								stream.WriteUInt(current4.Value);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Metadata Usage Pairs", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (KeyValuePair<uint, uint> current4 in usagePairs)
							{
								stream.WriteUInt(current4.Key);
								stream.WriteUInt(current4.Value);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Field Refs", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							KeyValuePair<int, int>[] fieldRefs = fieldRefs;
							for (int j = 0; j < fieldRefs.Length; j++)
							{
								KeyValuePair<int, int> keyValuePair = fieldRefs[j];
								stream.WriteInt(keyValuePair.Key);
								stream.WriteInt(keyValuePair.Value);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Referenced Assemblies", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (int current4 in metadataCollector.GetReferencedAssemblyIndiciesIntoAssemblyTable())
							{
								stream.WriteInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Attribute Types Ranges", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (AttributeCollection.AttributeTypeRange current4 in attributeCollection.GetAttributeTypeRanges())
							{
								stream.WriteInt(current4.Start);
								stream.WriteInt(current4.Count);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Attribute Types", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (int current4 in attributeCollection.GetAttributeTypeIndices())
							{
								stream.WriteInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Unresolved Virtual Call Parameter Types", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (int current4 in virtualCallTables.SignatureTypesInfo)
							{
								stream.WriteInt(current4);
							}
						});
						MetadataCacheWriter.WriteMetadataToStream("Unresolved Virtual Call Parameter Ranges", memoryStream, memoryStream2, delegate(MemoryStream stream)
						{
							foreach (Range current4 in virtualCallTables.SignatureRangesInfo)
							{
								stream.WriteInt(current4.start);
								stream.WriteInt(current4.length);
							}
						});
						fileStream.WriteUInt(4205910959u);
						fileStream.WriteInt(22);
						memoryStream.Seek(0L, SeekOrigin.Begin);
						memoryStream.CopyTo(fileStream);
						memoryStream2.Seek(0L, SeekOrigin.Begin);
						memoryStream2.CopyTo(fileStream);
					}
				}
			}
		}

		private static void WriteMetadataToStream(string name, MemoryStream headerStream, MemoryStream dataStream, Action<MemoryStream> callback)
		{
			using (TinyProfiler.Section(name, ""))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					callback(memoryStream);
					MetadataCacheWriter.AddStreamAndRecordHeader(name, headerStream, dataStream, memoryStream);
				}
			}
		}

		private static TypeReference DeclaringTypeFor(TypeDefinition type)
		{
			TypeReference result;
			if (!type.IsNested)
			{
				result = null;
			}
			else
			{
				result = type.DeclaringType;
			}
			return result;
		}

		private static TypeReference BaseTypeFor(TypeDefinition type)
		{
			return TypeResolver.For(type).Resolve(type.Resolve().BaseType);
		}

		private static TypeReference ElementTypeFor(TypeDefinition type)
		{
			TypeReference result;
			if (type.IsEnum())
			{
				result = type.GetUnderlyingEnumType();
			}
			else
			{
				result = type;
			}
			return result;
		}

		private static void AddStreamAndRecordHeader(string name, Stream headerStream, Stream dataStream, Stream stream)
		{
			if (dataStream.Position % 4L != 0L)
			{
				throw new ArgumentException(string.Format("Data stream is not aligned to minimum alignment of {0}", 4), "dataStream");
			}
			if (stream.Position % 4L != 0L)
			{
				throw new ArgumentException(string.Format("Stream is not aligned to minimum alignment of {0}", 4), "stream");
			}
			MetadataCacheWriter.StatsService.RecordMetadataStream(name, stream.Position);
			headerStream.WriteLongAsInt(256L + dataStream.Position);
			headerStream.WriteLongAsInt(stream.Position);
			stream.Seek(0L, SeekOrigin.Begin);
			stream.CopyTo(dataStream);
		}

		internal static bool TypesDoNotExceedMaximumRecursion(IEnumerable<TypeReference> types)
		{
			if (MetadataCacheWriter.<>f__mg$cache1 == null)
			{
				MetadataCacheWriter.<>f__mg$cache1 = new Func<TypeReference, bool>(MetadataCacheWriter.TypeDoesNotExceedMaximumRecursion);
			}
			return types.All(MetadataCacheWriter.<>f__mg$cache1);
		}

		internal static bool TypeDoesNotExceedMaximumRecursion(TypeReference type)
		{
			return !type.IsGenericInstance || !GenericsUtilities.CheckForMaximumRecursion((GenericInstanceType)type);
		}

		private static string FormatMethodTableEntry(MethodReference m, Dictionary<string, int> pointers)
		{
			int num;
			pointers.TryGetValue(MethodTables.MethodPointerFor(m), out num);
			return string.Concat(new object[]
			{
				"{ ",
				MetadataCacheWriter.Il2CppGenericMethodCollector.GetIndex(m),
				", ",
				num,
				"/*",
				MethodTables.MethodPointerFor(m),
				"*/, ",
				MetadataCacheWriter.RuntimeInvokerCollectorReader.GetIndex(m),
				"/*",
				MetadataCacheWriter.RuntimeInvokerCollectorReader.GetIndex(m),
				"*/}"
			});
		}

		public static TableInfo WriteFieldTable(CppCodeWriter writer, List<TableInfo> fieldTableInfos)
		{
			TableInfo[] array = (from item in fieldTableInfos
			where item.Count > 0
			select item).ToArray<TableInfo>();
			for (int i = 0; i < array.Length; i++)
			{
				TableInfo tableInfo = array[i];
				writer.WriteLine("extern const int32_t {0}[{1}];", new object[]
				{
					tableInfo.Name,
					tableInfo.Count
				});
			}
			IncludeWriter.WriteRegistrationIncludes(writer);
			return MetadataWriter.WriteTable<TableInfo>(writer, "extern const int32_t*", "g_FieldOffsetTable", fieldTableInfos, (TableInfo table) => (table.Count <= 0) ? MetadataCacheWriter.Naming.Null : table.Name);
		}

		public static TableInfo WriteTypeDefinitionSizesTable(CppCodeWriter writer, IMetadataCollection metadataCollector, AttributeCollection attributeCollection)
		{
			string[] array = metadataCollector.GetTypeInfos().Select((TypeDefinition type, int index) => "g_typeDefinitionSize" + index).ToArray<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				writer.WriteLine("extern const Il2CppTypeDefinitionSizes {0};", new object[]
				{
					text
				});
			}
			return MetadataWriter.WriteTable<string>(writer, "extern const Il2CppTypeDefinitionSizes*", "g_Il2CppTypeDefinitionSizesTable", array, (string varName) => MetadataCacheWriter.Naming.AddressOf(varName));
		}

		private static string Sizes(TypeDefinition type)
		{
			DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type, MarshalType.PInvoke, null, false, false, false, null);
			string arg_114_0 = "{0}, {1}, {2}, {3}";
			object[] expr_19 = new object[4];
			expr_19[0] = ((!type.HasGenericParameters) ? MetadataCacheWriter.InstanceSizeFor(type) : "0");
			expr_19[1] = ((!type.HasGenericParameters) ? defaultMarshalInfoWriter.NativeSize : "0");
			int arg_B9_1 = 2;
			object arg_B9_2;
			if (!type.HasGenericParameters)
			{
				if (type.Fields.Any((FieldDefinition f) => f.IsNormalStatic()) || type.StoresNonFieldsInStaticFields())
				{
					arg_B9_2 = string.Format("sizeof({0})", MetadataCacheWriter.Naming.ForStaticFieldsStruct(type));
					goto IL_B9;
				}
			}
			arg_B9_2 = "0";
			IL_B9:
			expr_19[arg_B9_1] = arg_B9_2;
			int arg_113_1 = 3;
			object arg_113_2;
			if (!type.HasGenericParameters)
			{
				if (type.Fields.Any((FieldDefinition f) => f.IsThreadStatic()))
				{
					arg_113_2 = string.Format("sizeof({0})", MetadataCacheWriter.Naming.ForThreadFieldsStruct(type));
					goto IL_113;
				}
			}
			arg_113_2 = "0";
			IL_113:
			expr_19[arg_113_1] = arg_113_2;
			return string.Format(arg_114_0, expr_19);
		}

		private static string InstanceSizeFor(TypeDefinition type)
		{
			string result;
			if (type.IsInterface())
			{
				result = "0";
			}
			else
			{
				result = string.Format("sizeof ({0}){1}", MetadataCacheWriter.Naming.ForType(type), (!type.IsValueType()) ? string.Empty : "+ sizeof (Il2CppObject)");
			}
			return result;
		}

		private static string OffsetOf(FieldDefinition field)
		{
			string result;
			if (field.IsLiteral)
			{
				result = "0";
			}
			else if (field.IsThreadStatic())
			{
				result = "THREAD_STATIC_FIELD_OFFSET";
			}
			else if (field.DeclaringType.HasGenericParameters)
			{
				result = "0";
			}
			else if (field.IsNormalStatic())
			{
				result = string.Format("{0}::{1}()", MetadataCacheWriter.Naming.ForStaticFieldsStruct(field.DeclaringType), MetadataCacheWriter.Naming.ForFieldOffsetGetter(field));
			}
			else
			{
				result = string.Format("{0}::{1}(){2}", MetadataCacheWriter.Naming.ForTypeNameOnly(field.DeclaringType), MetadataCacheWriter.Naming.ForFieldOffsetGetter(field), (!field.DeclaringType.IsValueType()) ? "" : (" + static_cast<int32_t>(sizeof(" + MetadataCacheWriter.Naming.ForType(MetadataCacheWriter.TypeProvider.SystemObject) + "))"));
			}
			return result;
		}

		private static CppCodeWriter GetMetadataCodeWriter(NPath outputDir, string tableName)
		{
			SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				string.Format("Il2Cpp{0}Table.cpp", tableName)
			}));
			IncludeWriter.WriteRegistrationIncludes(sourceCodeWriter);
			return sourceCodeWriter;
		}

		private static MetadataCacheWriter.PackingSize ConvertPackingSizeToCompressedEnum(int packingSize)
		{
			switch (packingSize)
			{
			case 0:
			{
				MetadataCacheWriter.PackingSize result = MetadataCacheWriter.PackingSize.Zero;
				return result;
			}
			case 1:
			{
				MetadataCacheWriter.PackingSize result = MetadataCacheWriter.PackingSize.One;
				return result;
			}
			case 2:
			{
				MetadataCacheWriter.PackingSize result = MetadataCacheWriter.PackingSize.Two;
				return result;
			}
			case 3:
			case 5:
			case 6:
			case 7:
			{
				IL_2B:
				MetadataCacheWriter.PackingSize result;
				if (packingSize == 16)
				{
					result = MetadataCacheWriter.PackingSize.Sixteen;
					return result;
				}
				if (packingSize == 32)
				{
					result = MetadataCacheWriter.PackingSize.ThirtyTwo;
					return result;
				}
				if (packingSize == 64)
				{
					result = MetadataCacheWriter.PackingSize.SixtyFour;
					return result;
				}
				if (packingSize != 128)
				{
					throw new InvalidOperationException(string.Format("The packing size of {0} is not valid. Valid values are 0, 1, 2, 4, 8, 16, 32, 64, or 128.", packingSize));
				}
				result = MetadataCacheWriter.PackingSize.OneHundredTwentyEight;
				return result;
			}
			case 4:
			{
				MetadataCacheWriter.PackingSize result = MetadataCacheWriter.PackingSize.Four;
				return result;
			}
			case 8:
			{
				MetadataCacheWriter.PackingSize result = MetadataCacheWriter.PackingSize.Eight;
				return result;
			}
			}
			goto IL_2B;
		}
	}
}
