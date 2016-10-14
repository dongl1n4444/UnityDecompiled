using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.Com;
using Unity.IL2CPP.Debugger;
using Unity.IL2CPP.FileNaming;
using Unity.IL2CPP.GenericsCollection;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;
using Unity.TinyProfiling;

namespace Unity.IL2CPP
{
	public class SourceWriter
	{
		private readonly NPath _outputDir;

		private readonly VTableBuilder _vTableBuilder;

		private readonly IDebuggerSupport _debuggerSupport;

		[Inject]
		public static IRuntimeInvokerCollectorWriterService RuntimeInvokerCollectorWriter;

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		[Inject]
		public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[CompilerGenerated]
		private static Func<ArrayType, ModuleDefinition> <>f__mg$cache0;

		public SourceWriter(VTableBuilder vTableBuilder, IDebuggerSupport debuggerSupport, NPath outputDir)
		{
			this._vTableBuilder = vTableBuilder;
			this._debuggerSupport = debuggerSupport;
			this._outputDir = outputDir;
		}

		public void Write(AssemblyDefinition assemblyDefinition, InflatedCollectionCollector allGenerics, NPath outputDir, TypeDefinition[] typeList, AttributeCollection attributeCollection, MethodCollector methodCollector, IMetadataCollection metadataCollection)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyDefinition.MainModule.FullyQualifiedName);
			using (TinyProfiler.Section("Code", ""))
			{
				TypeDefinition[] array = (from t in typeList
				where !t.HasGenericParameters && (!t.IsInterface || t.IsComOrWindowsRuntimeInterface())
				select t).ToArray<TypeDefinition>();
				using (TinyProfiler.Section("Types", "Declarations"))
				{
					TypeDefinition[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						TypeDefinition type = array2[i];
						this.WriteTypeDefinitionFor(this._outputDir, type);
					}
				}
				using (TinyProfiler.Section("Methods", "Declarations"))
				{
					TypeDefinition[] array3 = array;
					for (int j = 0; j < array3.Length; j++)
					{
						TypeDefinition type2 = array3[j];
						this.WriteMethodDeclarationsFor(this._outputDir, (MethodDefinition m) => !m.HasGenericParameters, type2);
					}
				}
				using (TinyProfiler.Section("Methods", "Definitions"))
				{
					this.WriteMethodSourceFiles(this._outputDir, fileNameWithoutExtension, array, methodCollector, metadataCollection);
				}
			}
		}

		internal void WriteGenerics(ReadOnlyInflatedCollectionCollector genericsCollectionCollector, IEnumerable<TypeDefinition> allTypeDefinitions, IMetadataCollection metadataCollection)
		{
			ParallelHelper.ForEach<Action>(new List<Action>
			{
				delegate
				{
					using (TinyProfiler.Section("GenericInstanceTypes Definitions", ""))
					{
						foreach (GenericInstanceType current in genericsCollectionCollector.TypeDeclarations)
						{
							this.WriteTypeDefinitionFor(this._outputDir, current);
						}
					}
				},
				delegate
				{
					using (TinyProfiler.Section("GenericInstanceTypes Methods", ""))
					{
						foreach (GenericInstanceType current in from t in genericsCollectionCollector.TypeMethodDeclarations
						where !t.IsInterface() || t.IsComOrWindowsRuntimeInterface()
						select t)
						{
							this.WriteMethodDeclarationsFor(this._outputDir, (MethodDefinition m) => !m.HasGenericParameters, current);
						}
					}
				},
				delegate
				{
					using (TinyProfiler.Section("Arrays Definitions", ""))
					{
						IEnumerable<ArrayType> arg_3B_0 = genericsCollectionCollector.Arrays;
						if (SourceWriter.<>f__mg$cache0 == null)
						{
							SourceWriter.<>f__mg$cache0 = new Func<ArrayType, ModuleDefinition>(ArrayUtilities.ModuleDefinitionForElementTypeOf);
						}
						foreach (IGrouping<ModuleDefinition, ArrayType> current in arg_3B_0.GroupBy(SourceWriter.<>f__mg$cache0))
						{
							NPath filename = this._outputDir.Combine(new string[]
							{
								FileNameProvider.Instance.ForModule(current.Key) + "_ArrayTypes.h"
							});
							using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(filename))
							{
								sourceCodeWriter.WriteLine("#pragma once");
								sourceCodeWriter.AddIncludeForTypeDefinition(SourceWriter.TypeProvider.Corlib.MainModule.GetType("System.Array"));
								foreach (ArrayType current2 in current)
								{
									sourceCodeWriter.AddIncludeOrExternForTypeDefinition(current2.ElementType);
									new TypeDefinitionWriter().WriteArrayTypeDefinition(current2, sourceCodeWriter);
								}
							}
						}
					}
				},
				delegate
				{
					using (TinyProfiler.Section("EmptyTypes Definitions", ""))
					{
						foreach (GenericInstanceType current in genericsCollectionCollector.EmptyTypes)
						{
							this.WriteTypeDefinitionFor(this._outputDir, current);
						}
					}
				},
				delegate
				{
					using (TinyProfiler.Section("GenericInstanceMethods", ""))
					{
						this.WriteGenericMethods(genericsCollectionCollector, metadataCollection);
					}
					using (TinyProfiler.Section("GenericInstanceTypes", ""))
					{
						this.WriteMethodSourceFiles(this._outputDir, "Generics", genericsCollectionCollector.Types, new NullMethodCollector(), metadataCollection);
					}
				},
				new Action(this.WriteGeneratedCodeGen)
			}, delegate(Action s)
			{
				s();
			}, true, false);
		}

		public static void WriteCollectedMetadata(InflatedCollectionCollector genericsCollectionCollector, ICollection<AssemblyDefinition> usedAssemblies, NPath outputDir, NPath dataFolder, IMetadataCollection metadataCollection, AttributeCollection attributeCollection, VTableBuilder vTableBuilder, IMethodCollectorResults methodCollector)
		{
			TableInfo attributeGeneratorTable;
			using (TinyProfiler.Section("Attributes", ""))
			{
				using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppAttributes.cpp"
				})))
				{
					sourceCodeWriter.AddCodeGenIncludes();
					attributeGeneratorTable = new AttributesSupport(sourceCodeWriter, attributeCollection).WriteAttributes(usedAssemblies);
				}
			}
			TableInfo guidTable;
			using (TinyProfiler.Section("Guids", ""))
			{
				using (SourceCodeWriter sourceCodeWriter2 = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppGuids.cpp"
				})))
				{
					sourceCodeWriter2.AddCodeGenIncludes();
					guidTable = new GuidWriter(sourceCodeWriter2).WriteGuids(metadataCollection.GetTypesWithGuids());
				}
			}
			MethodTables methodTables = MethodTables.CollectMethodTables(from item in SourceWriter.Il2CppGenericMethodCollector.Items
			select item.Key);
			using (TinyProfiler.Section("WriteCodeRegistration", ""))
			{
				CodeRegistrationWriter.WriteCodeRegistration(outputDir, methodCollector, genericsCollectionCollector, methodTables, attributeGeneratorTable, guidTable);
			}
			using (TinyProfiler.Section("WriteMetadata", ""))
			{
				MetadataCacheWriter.WriteMetadata(outputDir, dataFolder, genericsCollectionCollector, null, usedAssemblies, methodTables, metadataCollection, attributeCollection, vTableBuilder, methodCollector);
			}
		}

		private void WriteGeneratedCodeGen()
		{
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(this._outputDir.Combine(new string[]
			{
				"GeneratedCodeGen.h"
			})))
			{
				sourceCodeWriter.WriteLine("#pragma once");
				string[] array = new string[]
				{
					"System.Object",
					"System.Array",
					"System.String",
					"System.Type",
					"System.IntPtr",
					"System.Exception",
					"System.RuntimeTypeHandle",
					"System.RuntimeFieldHandle",
					"System.RuntimeArgumentHandle",
					"System.RuntimeMethodHandle",
					"System.Text.StringBuilder",
					"System.MulticastDelegate",
					"System.Reflection.MethodBase"
				};
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string fullName = array2[i];
					TypeDefinition type = SourceWriter.TypeProvider.Corlib.MainModule.GetType(fullName);
					if (type.IsValueType() || type.Name == "Array")
					{
						sourceCodeWriter.AddIncludeForTypeDefinition(type);
					}
					else
					{
						sourceCodeWriter.AddForwardDeclaration(type);
					}
					sourceCodeWriter.WriteLine("typedef {0} Il2CppCodeGen{1};", new object[]
					{
						SourceWriter.Naming.ForType(type),
						type.Name
					});
				}
			}
		}

		private void WriteGenericMethods(ReadOnlyInflatedCollectionCollector genericsCollectionCollector, IMetadataCollection metadataCollection)
		{
			int num = 0;
			NullMethodCollector methodCollector = new NullMethodCollector();
			IOrderedEnumerable<GenericInstanceMethod> foo = from mr in genericsCollectionCollector.Methods
			orderby mr.FullName
			select mr;
			foreach (List<GenericInstanceMethod> current in foo.Chunk(1000))
			{
				NPath filename = this._outputDir.Combine(new string[]
				{
					"GenericMethods" + num++ + ".cpp"
				});
				using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(filename))
				{
					foreach (GenericInstanceMethod current2 in current)
					{
						this.WriteGenericMethodDefinition(sourceCodeWriter, current2, methodCollector, metadataCollection);
					}
				}
			}
		}

		private void WriteGenericMethodDefinition(CppCodeWriter writer, GenericInstanceMethod method, IMethodCollector methodCollector, IMetadataCollection metadataCollection)
		{
			writer.AddStdInclude("cstring");
			writer.AddStdInclude("string.h");
			writer.AddStdInclude("stdio.h");
			writer.AddStdInclude("cmath");
			writer.AddStdInclude("limits");
			writer.AddStdInclude("assert.h");
			if (DebuggerOptions.Enabled)
			{
				writer.AddInclude("il2cpp-debugger.h");
			}
			writer.AddInclude("codegen/il2cpp-codegen.h");
			writer.AddIncludesForMethodDeclaration(method);
			writer.AddIncludeForTypeDefinition(method.DeclaringType);
			MethodWriter methodWriter = new MethodWriter(method.DeclaringType, writer, this._vTableBuilder);
			methodWriter.WriteMethodDefinition(method, methodCollector);
		}

		private void WriteMethodSourceFiles(NPath outputDirectory, string fileName, IEnumerable<TypeReference> typeList, IMethodCollector methodCollector, IMetadataCollection metadataCollection)
		{
			int num = 0;
			IOrderedEnumerable<TypeReference> foo = from tr in typeList
			orderby tr.FullName
			select tr;
			foreach (List<TypeReference> current in foo.Chunk(100))
			{
				string text = string.Concat(new object[]
				{
					"Bulk_",
					fileName,
					"_",
					num++,
					".cpp"
				});
				using (TinyProfiler.Section("WriteBulkMethods", text))
				{
					using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDirectory.Combine(new string[]
					{
						text
					})))
					{
						sourceCodeWriter.AddInclude("class-internals.h");
						sourceCodeWriter.AddInclude("codegen/il2cpp-codegen.h");
						sourceCodeWriter.AddStdInclude("cstring");
						sourceCodeWriter.AddStdInclude("string.h");
						sourceCodeWriter.AddStdInclude("stdio.h");
						sourceCodeWriter.AddStdInclude("cmath");
						sourceCodeWriter.AddStdInclude("limits");
						sourceCodeWriter.AddStdInclude("assert.h");
						sourceCodeWriter.AddIncludeForTypeDefinition(SourceWriter.TypeProvider.SystemArray);
						this._debuggerSupport.WriteDebugMetadataIncludes(sourceCodeWriter);
						this._debuggerSupport.WriteDebugIncludes(sourceCodeWriter);
						sourceCodeWriter.WriteClangWarningDisables();
						foreach (TypeReference current2 in current)
						{
							sourceCodeWriter.AddIncludeForTypeDefinition(current2);
							sourceCodeWriter.AddIncludeForMethodDeclarations(current2);
							SourceWriter.WriteComTypeDeclaration(sourceCodeWriter, current2);
							new MethodWriter(current2, sourceCodeWriter, this._vTableBuilder).WriteMethodDefinitions(methodCollector);
						}
						sourceCodeWriter.WriteClangWarningEnables();
					}
				}
			}
		}

		private static void WriteComTypeDeclaration(SourceCodeWriter writer, TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition.IsImport && !typeDefinition.IsInterface)
			{
				writer.WriteCommentedLine(type.FullName);
				writer.WriteStatement(Emit.Assign("const Il2CppGuid " + SourceWriter.Naming.ForTypeNameOnly(type) + "::CLSID", SourceWriter.ConvertGuidToInitializer(typeDefinition.GetGuid())));
			}
			else if (type.IsComOrWindowsRuntimeInterface())
			{
				writer.WriteCommentedLine(type.FullName);
				writer.WriteStatement(Emit.Assign("const Il2CppGuid " + SourceWriter.Naming.ForTypeNameOnly(type) + "::IID", SourceWriter.ConvertGuidToInitializer(type.GetGuid())));
			}
			if (!typeDefinition.IsComOrWindowsRuntimeType())
			{
				if (typeDefinition.Interfaces.Any((TypeReference i) => i.IsComOrWindowsRuntimeInterface()))
				{
					new CCWWriter(typeDefinition).WriteMethodDefinitions(writer);
				}
			}
		}

		private void WriteMethodDeclarationsFor(NPath outputDirectory, Func<MethodDefinition, bool> filter, TypeDefinition type)
		{
			NPath filename = outputDirectory.Combine(new string[]
			{
				FileNameProvider.Instance.ForMethodDeclarations(type)
			});
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(filename))
			{
				sourceCodeWriter.AddStdInclude("stdint.h");
				sourceCodeWriter.AddStdInclude("assert.h");
				sourceCodeWriter.AddStdInclude("exception");
				sourceCodeWriter.AddInclude("codegen/il2cpp-codegen.h");
				MethodWriter methodWriter = new MethodWriter(type, sourceCodeWriter, this._vTableBuilder);
				methodWriter.WriteMethodDeclarationsFor(filter);
			}
		}

		private void WriteMethodDeclarationsFor(NPath outputDirectory, Func<MethodDefinition, bool> filter, GenericInstanceType type)
		{
			NPath filename = outputDirectory.Combine(new string[]
			{
				FileNameProvider.Instance.ForMethodDeclarations(type)
			});
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(filename))
			{
				sourceCodeWriter.AddStdInclude("stdint.h");
				sourceCodeWriter.AddStdInclude("assert.h");
				sourceCodeWriter.AddStdInclude("exception");
				sourceCodeWriter.AddInclude("codegen/il2cpp-codegen.h");
				MethodWriter methodWriter = new MethodWriter(type, sourceCodeWriter, this._vTableBuilder);
				methodWriter.WriteMethodDeclarationsFor(filter);
			}
		}

		private void WriteTypeDefinitionFor(NPath outputDirectory, TypeReference type)
		{
			if (!type.IsInterface() || type.IsComOrWindowsRuntimeInterface())
			{
				NPath filename = outputDirectory.Combine(new string[]
				{
					FileNameProvider.Instance.ForTypeDefinition(type)
				});
				using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(filename))
				{
					sourceCodeWriter.AddStdInclude("stdint.h");
					if (type.IsSystemArray())
					{
						sourceCodeWriter.WriteLine("struct Il2CppArrayBounds;");
					}
					if (!type.IsComOrWindowsRuntimeInterface())
					{
						new TypeDefinitionWriter().WriteTypeDefinitionFor(type, sourceCodeWriter);
					}
					else
					{
						new ComInterfaceWriter(sourceCodeWriter).WriteComInterfaceFor(type);
					}
				}
			}
		}

		private static string ConvertGuidToInitializer(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			uint num = BitConverter.ToUInt32(array, 0);
			ushort num2 = BitConverter.ToUInt16(array, 4);
			ushort num3 = BitConverter.ToUInt16(array, 6);
			return '{' + string.Format(" 0x{0:x}, 0x{1:x}, 0x{2:x}, 0x{3:x}, 0x{4:x}, 0x{5:x}, 0x{6:x}, 0x{7:x}, 0x{8:x}, 0x{9:x}, 0x{10:x} ", new object[]
			{
				num,
				num2,
				num3,
				array[8],
				array[9],
				array[10],
				array[11],
				array[12],
				array[13],
				array[14],
				array[15]
			}) + '}';
		}
	}
}
