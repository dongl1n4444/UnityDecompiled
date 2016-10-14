using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.Com;
using Unity.IL2CPP.GenericsCollection;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
using Unity.IL2CPP.Metadata;
using Unity.TinyProfiling;

namespace Unity.IL2CPP
{
	public class CodeRegistrationWriter
	{
		[Inject]
		public static INamingService Naming;

		[Inject]
		public static IRuntimeInvokerCollectorWriterService RuntimeInvokerCollectorWriter;

		[CompilerGenerated]
		private static Func<MethodReference, string> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<TypeDefinition, string> <>f__mg$cache1;

		public static void WriteCodeRegistration(NPath outputDir, IMethodCollectorResults methodCollector, InflatedCollectionCollector genericsCollectionCollector, MethodTables methodPointerTables, TableInfo attributeGeneratorTable, TableInfo guidTable)
		{
			TableInfo methodPointersTable = CodeRegistrationWriter.WriteMethodPointerTable(outputDir, methodCollector);
			TableInfo reversePInvokeWrappersTable = CodeRegistrationWriter.WriteReversePInvokeWrappersTable(outputDir, methodCollector);
			TableInfo delegateWrappersManagedToNativeTable = CodeRegistrationWriter.WriteDelegateWrappersManagedToNative(outputDir, methodCollector);
			TableInfo marshalingFunctionsTable = CodeRegistrationWriter.WriteMarshalingFunctions(outputDir, methodCollector);
			TableInfo ccwMarshalingFunctionsTable = CodeRegistrationWriter.WriteCCWMarshalingFunctions(outputDir, methodCollector);
			TableInfo genericMethodPointerTable = CodeRegistrationWriter.WriteGenericMethodPointerTable(outputDir, methodCollector, genericsCollectionCollector, methodPointerTables);
			TableInfo invokerTable = CodeRegistrationWriter.WriteInvokerTable(outputDir);
			CodeRegistrationWriter.WriteCodeRegistration(outputDir, invokerTable, methodPointersTable, reversePInvokeWrappersTable, delegateWrappersManagedToNativeTable, marshalingFunctionsTable, ccwMarshalingFunctionsTable, genericMethodPointerTable, attributeGeneratorTable, guidTable);
		}

		private static void WriteCodeRegistration(NPath outputDir, TableInfo invokerTable, TableInfo methodPointersTable, TableInfo reversePInvokeWrappersTable, TableInfo delegateWrappersManagedToNativeTable, TableInfo marshalingFunctionsTable, TableInfo ccwMarshalingFunctionsTable, TableInfo genericMethodPointerTable, TableInfo attributeGeneratorTable, TableInfo guidTable)
		{
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				"Il2CppCodeRegistration.cpp"
			})))
			{
				IncludeWriter.WriteRegistrationIncludes(sourceCodeWriter);
				if (methodPointersTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("extern const Il2CppMethodPointer g_MethodPointers[];");
				}
				if (reversePInvokeWrappersTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("extern const Il2CppMethodPointer g_ReversePInvokeWrapperPointers[];");
				}
				if (delegateWrappersManagedToNativeTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("{0} {1}[];", new object[]
					{
						delegateWrappersManagedToNativeTable.Type,
						delegateWrappersManagedToNativeTable.Name
					});
				}
				if (marshalingFunctionsTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("{0} {1}[];", new object[]
					{
						marshalingFunctionsTable.Type,
						marshalingFunctionsTable.Name
					});
				}
				if (ccwMarshalingFunctionsTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("{0} {1}[];", new object[]
					{
						ccwMarshalingFunctionsTable.Type,
						ccwMarshalingFunctionsTable.Name
					});
				}
				if (genericMethodPointerTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("extern const Il2CppMethodPointer g_Il2CppGenericMethodPointers[];");
				}
				if (invokerTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("extern const InvokerMethod {0}[];", new object[]
					{
						invokerTable.Name
					});
				}
				if (attributeGeneratorTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("{0} {1}[];", new object[]
					{
						attributeGeneratorTable.Type,
						attributeGeneratorTable.Name
					});
				}
				if (guidTable.Count > 0)
				{
					sourceCodeWriter.WriteLine("{0} {1}[];", new object[]
					{
						guidTable.Type,
						guidTable.Name
					});
				}
				sourceCodeWriter.WriteStructInitializer("const Il2CppCodeRegistration", "g_CodeRegistration", new string[]
				{
					methodPointersTable.Count.ToString(CultureInfo.InvariantCulture),
					(methodPointersTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : methodPointersTable.Name,
					reversePInvokeWrappersTable.Count.ToString(CultureInfo.InvariantCulture),
					(reversePInvokeWrappersTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : reversePInvokeWrappersTable.Name,
					delegateWrappersManagedToNativeTable.Count.ToString(CultureInfo.InvariantCulture),
					(delegateWrappersManagedToNativeTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : delegateWrappersManagedToNativeTable.Name,
					marshalingFunctionsTable.Count.ToString(CultureInfo.InvariantCulture),
					(marshalingFunctionsTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : marshalingFunctionsTable.Name,
					ccwMarshalingFunctionsTable.Count.ToString(CultureInfo.InvariantCulture),
					(ccwMarshalingFunctionsTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : ccwMarshalingFunctionsTable.Name,
					genericMethodPointerTable.Count.ToString(CultureInfo.InvariantCulture),
					(genericMethodPointerTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : genericMethodPointerTable.Name,
					invokerTable.Count.ToString(CultureInfo.InvariantCulture),
					(invokerTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : invokerTable.Name,
					attributeGeneratorTable.Count.ToString(CultureInfo.InvariantCulture),
					(attributeGeneratorTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : attributeGeneratorTable.Name,
					guidTable.Count.ToString(CultureInfo.InvariantCulture),
					(guidTable.Count <= 0) ? CodeRegistrationWriter.Naming.Null : guidTable.Name
				});
				sourceCodeWriter.WriteLine("extern const Il2CppMetadataRegistration g_MetadataRegistration;");
				sourceCodeWriter.WriteStructInitializer("static const Il2CppCodeGenOptions", "s_Il2CppCodeGenOptions", new string[]
				{
					(!CodeGenOptions.EnablePrimitiveValueTypeGenericSharing) ? "false" : "true"
				});
				sourceCodeWriter.WriteLine("static void s_Il2CppCodegenRegistration()");
				sourceCodeWriter.BeginBlock();
				sourceCodeWriter.WriteLine("il2cpp_codegen_register (&g_CodeRegistration, &g_MetadataRegistration, &s_Il2CppCodeGenOptions);");
				sourceCodeWriter.EndBlock(false);
				sourceCodeWriter.WriteLine("static il2cpp::utils::RegisterRuntimeInitializeAndCleanup s_Il2CppCodegenRegistrationVariable (&s_Il2CppCodegenRegistration, NULL);");
			}
		}

		private static TableInfo WriteMethodPointerTable(NPath outputDir, IMethodCollectorResults methodCollector)
		{
			TableInfo result;
			using (TinyProfiler.Section("MethodPointerTable", "Il2CppMethodPointerTable.cpp"))
			{
				TableInfo empty = TableInfo.Empty;
				ReadOnlyCollection<MethodReference> methods = methodCollector.GetMethods();
				if (methods.Count > 0)
				{
					using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
					{
						"Il2CppMethodPointerTable.cpp"
					})))
					{
						sourceCodeWriter.AddCodeGenIncludes();
						foreach (MethodReference current in methods)
						{
							sourceCodeWriter.WriteLine("extern \"C\" void {0} ();", new object[]
							{
								MethodTables.MethodPointerNameFor(current)
							});
						}
						CppCodeWriter arg_CE_0 = sourceCodeWriter;
						string arg_CE_1 = "extern const Il2CppMethodPointer";
						string arg_CE_2 = "g_MethodPointers";
						IEnumerable<MethodReference> arg_C8_0 = methods;
						if (CodeRegistrationWriter.<>f__mg$cache0 == null)
						{
							CodeRegistrationWriter.<>f__mg$cache0 = new Func<MethodReference, string>(MethodTables.MethodPointerNameFor);
						}
						arg_CE_0.WriteArrayInitializer(arg_CE_1, arg_CE_2, arg_C8_0.Select(CodeRegistrationWriter.<>f__mg$cache0), false);
						empty = new TableInfo(methods.Count, "extern const Il2CppMethodPointer", "g_MethodPointers");
					}
				}
				result = empty;
			}
			return result;
		}

		private static TableInfo WriteReversePInvokeWrappersTable(NPath outputDir, IMethodCollectorResults methodCollector)
		{
			TableInfo result;
			using (TinyProfiler.Section("ReversePInvokerWrapperTable", "Il2CppReversePInvokeWrapperTable.cpp"))
			{
				TableInfo empty = TableInfo.Empty;
				ReadOnlyCollection<MethodReference> reversePInvokeWrappers = methodCollector.GetReversePInvokeWrappers();
				if (reversePInvokeWrappers.Count > 0)
				{
					using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
					{
						"Il2CppReversePInvokeWrapperTable.cpp"
					})))
					{
						sourceCodeWriter.AddCodeGenIncludes();
						foreach (MethodReference current in reversePInvokeWrappers)
						{
							sourceCodeWriter.AddIncludeForMethodDeclarations(current.DeclaringType);
						}
						sourceCodeWriter.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_ReversePInvokeWrapperPointers", from m in reversePInvokeWrappers
						select string.Format("reinterpret_cast<Il2CppMethodPointer>({0})", CodeRegistrationWriter.Naming.ForReversePInvokeWrapperMethod(m)), false);
						empty = new TableInfo(reversePInvokeWrappers.Count, "extern const Il2CppMethodPointer", "g_ReversePInvokeWrapperPointers");
					}
				}
				result = empty;
			}
			return result;
		}

		private static TableInfo WriteDelegateWrappersManagedToNative(NPath outputDir, IMethodCollectorResults methodCollector)
		{
			TableInfo result;
			using (TinyProfiler.Section("DelegateWrappersManagedToNative", "Il2CppDelegateWrappersManagedToNativeTable.cpp"))
			{
				TableInfo empty = TableInfo.Empty;
				ReadOnlyCollection<MethodReference> wrappersForDelegateFromManagedToNative = methodCollector.GetWrappersForDelegateFromManagedToNative();
				if (wrappersForDelegateFromManagedToNative.Count > 0)
				{
					using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
					{
						"Il2CppDelegateWrappersManagedToNativeTable.cpp"
					})))
					{
						sourceCodeWriter.AddCodeGenIncludes();
						foreach (MethodReference current in wrappersForDelegateFromManagedToNative)
						{
							sourceCodeWriter.WriteLine("extern \"C\" void {0} ();", new object[]
							{
								CodeRegistrationWriter.Naming.ForDelegatePInvokeWrapper(current.DeclaringType)
							});
						}
						sourceCodeWriter.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_DelegateWrappersManagedToNative", from wrapper in wrappersForDelegateFromManagedToNative
						select CodeRegistrationWriter.Naming.ForDelegatePInvokeWrapper(wrapper.DeclaringType), false);
						empty = new TableInfo(wrappersForDelegateFromManagedToNative.Count, "extern const Il2CppMethodPointer", "g_DelegateWrappersManagedToNative");
					}
				}
				result = empty;
			}
			return result;
		}

		private static TableInfo WriteMarshalingFunctions(NPath outputDir, IMethodCollectorResults methodCollector)
		{
			TableInfo result;
			using (TinyProfiler.Section("MarshalingFunctions", "Il2CppMarshalingFunctionsTable.cpp"))
			{
				TableInfo empty = TableInfo.Empty;
				ReadOnlyCollection<TypeDefinition> typeMarshalingFunctions = methodCollector.GetTypeMarshalingFunctions();
				if (typeMarshalingFunctions.Count > 0)
				{
					using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
					{
						"Il2CppMarshalingFunctionsTable.cpp"
					})))
					{
						sourceCodeWriter.AddCodeGenIncludes();
						foreach (TypeDefinition current in typeMarshalingFunctions)
						{
							DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(current, MarshalType.PInvoke, null, false, false);
							sourceCodeWriter.WriteLine("extern \"C\" void {0} ();", new object[]
							{
								defaultMarshalInfoWriter.MarshalToNativeFunctionName
							});
							sourceCodeWriter.WriteLine("extern \"C\" void {0} ();", new object[]
							{
								defaultMarshalInfoWriter.MarshalFromNativeFunctionName
							});
							sourceCodeWriter.WriteLine("extern \"C\" void {0} ();", new object[]
							{
								defaultMarshalInfoWriter.MarshalCleanupFunctionName
							});
						}
						CppCodeWriter arg_113_0 = sourceCodeWriter;
						string arg_113_1 = "extern const Il2CppMarshalingFunctions";
						string arg_113_2 = "g_MarshalingFunctions";
						IEnumerable<TypeDefinition> arg_10D_0 = typeMarshalingFunctions;
						if (CodeRegistrationWriter.<>f__mg$cache1 == null)
						{
							CodeRegistrationWriter.<>f__mg$cache1 = new Func<TypeDefinition, string>(CodeRegistrationWriter.FormatIl2CppMarshalingFunction);
						}
						arg_113_0.WriteArrayInitializer(arg_113_1, arg_113_2, arg_10D_0.Select(CodeRegistrationWriter.<>f__mg$cache1), true);
						empty = new TableInfo(typeMarshalingFunctions.Count, "extern const Il2CppMarshalingFunctions", "g_MarshalingFunctions");
					}
				}
				result = empty;
			}
			return result;
		}

		private static string FormatIl2CppMarshalingFunction(TypeDefinition marshalableType)
		{
			DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(marshalableType, MarshalType.PInvoke, null, false, false);
			return string.Format("{{ {0}, {1}, {2} }}", defaultMarshalInfoWriter.MarshalToNativeFunctionName, defaultMarshalInfoWriter.MarshalFromNativeFunctionName, defaultMarshalInfoWriter.MarshalCleanupFunctionName);
		}

		private static TableInfo WriteCCWMarshalingFunctions(NPath outputDir, IMethodCollectorResults methodCollector)
		{
			TableInfo result;
			using (TinyProfiler.Section("CCWMarshalingFunctions", "Il2CppCcwMarshalingFunctionsTable.cpp"))
			{
				TableInfo empty = TableInfo.Empty;
				ReadOnlyCollection<TypeDefinition> cCWMarshalingFunctions = methodCollector.GetCCWMarshalingFunctions();
				if (cCWMarshalingFunctions.Count > 0)
				{
					using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
					{
						"Il2CppCcwMarshalingFunctionsTable.cpp"
					})))
					{
						sourceCodeWriter.AddCodeGenIncludes();
						CCWWriter[] source = (from t in cCWMarshalingFunctions
						select new CCWWriter(t)).ToArray<CCWWriter>();
						foreach (string current in from w in source
						select w.FunctionName)
						{
							sourceCodeWriter.WriteLine("extern \"C\" void {0} ();", new object[]
							{
								current
							});
						}
						sourceCodeWriter.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_CcwMarshalingFunctions", from w in source
						select w.FunctionName, true);
						empty = new TableInfo(cCWMarshalingFunctions.Count, "extern const Il2CppMethodPointer", "g_CcwMarshalingFunctions");
					}
				}
				result = empty;
			}
			return result;
		}

		private static TableInfo WriteGenericMethodPointerTable(NPath outputDir, IMethodCollectorResults methodCollector, InflatedCollectionCollector genericsCollectionCollector, MethodTables methodPointerTables)
		{
			TableInfo result;
			using (TinyProfiler.Section("GenericMethodPointerTable", "Il2CppGenericMethodPointerTable.cpp"))
			{
				TableInfo tableInfo;
				using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
				{
					"Il2CppGenericMethodPointerTable.cpp"
				})))
				{
					tableInfo = new MethodTableWriter(sourceCodeWriter).Write(genericsCollectionCollector, methodPointerTables, methodCollector);
				}
				result = tableInfo;
			}
			return result;
		}

		private static TableInfo WriteInvokerTable(NPath outputDir)
		{
			TableInfo result;
			using (TinyProfiler.Section("InvokerTable", "Il2CppInvokerTable.cpp"))
			{
				result = CodeRegistrationWriter.RuntimeInvokerCollectorWriter.Write(outputDir.Combine(new string[]
				{
					"Il2CppInvokerTable.cpp"
				}));
			}
			return result;
		}
	}
}
