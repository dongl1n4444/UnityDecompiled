namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.GenericsCollection;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
    using Unity.IL2CPP.Metadata;
    using Unity.TinyProfiling;

    public class CodeRegistrationWriter
    {
        [CompilerGenerated]
        private static Func<MethodReference, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodReference, string> <>f__mg$cache0;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorWriterService RuntimeInvokerCollectorWriter;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public static void WriteCodeRegistration(NPath outputDir, IMethodCollectorResults methodCollector, IInteropDataCollectorResults interopDataCollector, InflatedCollectionCollector genericsCollectionCollector, MethodTables methodPointerTables, TableInfo attributeGeneratorTable, UnresolvedVirtualsTablesInfo virtualCallTables)
        {
            TableInfo methodPointersTable = WriteMethodPointerTable(outputDir, methodCollector);
            TableInfo reversePInvokeWrappersTable = WriteReversePInvokeWrappersTable(outputDir, interopDataCollector);
            TableInfo genericMethodPointerTable = WriteGenericMethodPointerTable(outputDir, methodCollector, genericsCollectionCollector, methodPointerTables);
            TableInfo invokerTable = WriteInvokerTable(outputDir);
            TableInfo interopDataTable = WriteInteropDataTable(outputDir, interopDataCollector);
            WriteCodeRegistration(outputDir, invokerTable, methodPointersTable, reversePInvokeWrappersTable, genericMethodPointerTable, attributeGeneratorTable, virtualCallTables, interopDataTable);
        }

        private static void WriteCodeRegistration(NPath outputDir, TableInfo invokerTable, TableInfo methodPointersTable, TableInfo reversePInvokeWrappersTable, TableInfo genericMethodPointerTable, TableInfo attributeGeneratorTable, UnresolvedVirtualsTablesInfo virtualCallTables, TableInfo interopDataTable)
        {
            string[] append = new string[] { "Il2CppCodeRegistration.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                IncludeWriter.WriteRegistrationIncludes(writer);
                if (methodPointersTable.Count > 0)
                {
                    writer.WriteLine("extern const Il2CppMethodPointer g_MethodPointers[];");
                }
                if (reversePInvokeWrappersTable.Count > 0)
                {
                    writer.WriteLine("extern const Il2CppMethodPointer g_ReversePInvokeWrapperPointers[];");
                }
                if (genericMethodPointerTable.Count > 0)
                {
                    writer.WriteLine("extern const Il2CppMethodPointer g_Il2CppGenericMethodPointers[];");
                }
                if (invokerTable.Count > 0)
                {
                    object[] args = new object[] { invokerTable.Name };
                    writer.WriteLine("extern const InvokerMethod {0}[];", args);
                }
                if (attributeGeneratorTable.Count > 0)
                {
                    object[] objArray2 = new object[] { attributeGeneratorTable.Type, attributeGeneratorTable.Name };
                    writer.WriteLine("{0} {1}[];", objArray2);
                }
                if (virtualCallTables.MethodPointersInfo.Count > 0)
                {
                    object[] objArray3 = new object[] { virtualCallTables.MethodPointersInfo.Type, virtualCallTables.MethodPointersInfo.Name };
                    writer.WriteLine("{0} {1}[];", objArray3);
                }
                if (interopDataTable.Count > 0)
                {
                    object[] objArray4 = new object[] { interopDataTable.Type, interopDataTable.Name };
                    writer.WriteLine("{0} {1}[];", objArray4);
                }
                string[] values = new string[] { methodPointersTable.Count.ToString(CultureInfo.InvariantCulture), (methodPointersTable.Count <= 0) ? Naming.Null : methodPointersTable.Name, reversePInvokeWrappersTable.Count.ToString(CultureInfo.InvariantCulture), (reversePInvokeWrappersTable.Count <= 0) ? Naming.Null : reversePInvokeWrappersTable.Name, genericMethodPointerTable.Count.ToString(CultureInfo.InvariantCulture), (genericMethodPointerTable.Count <= 0) ? Naming.Null : genericMethodPointerTable.Name, invokerTable.Count.ToString(CultureInfo.InvariantCulture), (invokerTable.Count <= 0) ? Naming.Null : invokerTable.Name, attributeGeneratorTable.Count.ToString(CultureInfo.InvariantCulture), (attributeGeneratorTable.Count <= 0) ? Naming.Null : attributeGeneratorTable.Name, virtualCallTables.MethodPointersInfo.Count.ToString(CultureInfo.InvariantCulture), (virtualCallTables.MethodPointersInfo.Count <= 0) ? Naming.Null : virtualCallTables.MethodPointersInfo.Name, interopDataTable.Count.ToString(CultureInfo.InvariantCulture), (interopDataTable.Count <= 0) ? Naming.Null : interopDataTable.Name };
                writer.WriteStructInitializer("const Il2CppCodeRegistration", "g_CodeRegistration", values);
                writer.WriteLine("extern const Il2CppMetadataRegistration g_MetadataRegistration;");
                string[] textArray3 = new string[] { !CodeGenOptions.EnablePrimitiveValueTypeGenericSharing ? "false" : "true" };
                writer.WriteStructInitializer("static const Il2CppCodeGenOptions", "s_Il2CppCodeGenOptions", textArray3);
                writer.WriteLine("static void s_Il2CppCodegenRegistration()");
                writer.BeginBlock();
                writer.WriteLine("il2cpp_codegen_register (&g_CodeRegistration, &g_MetadataRegistration, &s_Il2CppCodeGenOptions);");
                writer.EndBlock(false);
                writer.WriteLine("static il2cpp::utils::RegisterRuntimeInitializeAndCleanup s_Il2CppCodegenRegistrationVariable (&s_Il2CppCodegenRegistration, NULL);");
            }
        }

        private static TableInfo WriteGenericMethodPointerTable(NPath outputDir, IMethodCollectorResults methodCollector, InflatedCollectionCollector genericsCollectionCollector, MethodTables methodPointerTables)
        {
            using (TinyProfiler.Section("GenericMethodPointerTable", "Il2CppGenericMethodPointerTable.cpp"))
            {
                string[] append = new string[] { "Il2CppGenericMethodPointerTable.cpp" };
                using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                {
                    return new MethodTableWriter(writer).Write(genericsCollectionCollector, methodPointerTables, methodCollector);
                }
            }
        }

        private static TableInfo WriteInteropDataTable(NPath outputDir, IInteropDataCollectorResults interopDataCollector)
        {
            using (TinyProfiler.Section("InteropDataTable", "Il2CppInteropDataTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<InteropData> interopData = interopDataCollector.GetInteropData();
                if (interopData.Count > 0)
                {
                    <WriteInteropDataTable>c__AnonStorey0 storey = new <WriteInteropDataTable>c__AnonStorey0();
                    string[] append = new string[] { "Il2CppInteropDataTable.cpp" };
                    storey.writer = new SourceCodeWriter(outputDir.Combine(append));
                    try
                    {
                        storey.writer.AddCodeGenIncludes();
                        storey.writer.WriteArrayInitializer("extern Il2CppInteropData", "g_Il2CppInteropData", interopData.Select<InteropData, string>(new Func<InteropData, string>(storey.<>m__0)), true);
                        empty = new TableInfo(interopData.Count, "extern Il2CppInteropData", "g_Il2CppInteropData");
                    }
                    finally
                    {
                        if (storey.writer != null)
                        {
                            storey.writer.Dispose();
                        }
                    }
                }
                return empty;
            }
        }

        private static TableInfo WriteInvokerTable(NPath outputDir)
        {
            using (TinyProfiler.Section("InvokerTable", "Il2CppInvokerTable.cpp"))
            {
                string[] append = new string[] { "Il2CppInvokerTable.cpp" };
                return RuntimeInvokerCollectorWriter.Write(outputDir.Combine(append));
            }
        }

        private static TableInfo WriteMethodPointerTable(NPath outputDir, IMethodCollectorResults methodCollector)
        {
            using (TinyProfiler.Section("MethodPointerTable", "Il2CppMethodPointerTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<MethodReference> methods = methodCollector.GetMethods();
                if (methods.Count > 0)
                {
                    string[] append = new string[] { "Il2CppMethodPointerTable.cpp" };
                    using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                    {
                        writer.AddCodeGenIncludes();
                        foreach (MethodReference reference in methods)
                        {
                            object[] args = new object[] { MethodTables.MethodPointerNameFor(reference) };
                            writer.WriteLine("extern \"C\" void {0} ();", args);
                        }
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new Func<MethodReference, string>(MethodTables.MethodPointerNameFor);
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_MethodPointers", methods.Select<MethodReference, string>(<>f__mg$cache0), false);
                        empty = new TableInfo(methods.Count, "extern const Il2CppMethodPointer", "g_MethodPointers");
                    }
                }
                return empty;
            }
        }

        private static TableInfo WriteReversePInvokeWrappersTable(NPath outputDir, IInteropDataCollectorResults interopDataCollector)
        {
            using (TinyProfiler.Section("ReversePInvokerWrapperTable", "Il2CppReversePInvokeWrapperTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<MethodReference> reversePInvokeWrappers = interopDataCollector.GetReversePInvokeWrappers();
                if (reversePInvokeWrappers.Count > 0)
                {
                    string[] append = new string[] { "Il2CppReversePInvokeWrapperTable.cpp" };
                    using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                    {
                        writer.AddCodeGenIncludes();
                        foreach (MethodReference reference in reversePInvokeWrappers)
                        {
                            writer.AddIncludeForMethodDeclarations(reference.DeclaringType);
                        }
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = m => $"reinterpret_cast<Il2CppMethodPointer>({Naming.ForReversePInvokeWrapperMethod(m)})";
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_ReversePInvokeWrapperPointers", reversePInvokeWrappers.Select<MethodReference, string>(<>f__am$cache0), false);
                        empty = new TableInfo(reversePInvokeWrappers.Count, "extern const Il2CppMethodPointer", "g_ReversePInvokeWrapperPointers");
                    }
                }
                return empty;
            }
        }

        [CompilerGenerated]
        private sealed class <WriteInteropDataTable>c__AnonStorey0
        {
            internal SourceCodeWriter writer;

            internal string <>m__0(InteropData data)
            {
                string @null = CodeRegistrationWriter.Naming.Null;
                string marshalToNativeFunctionName = CodeRegistrationWriter.Naming.Null;
                string marshalFromNativeFunctionName = CodeRegistrationWriter.Naming.Null;
                string marshalCleanupFunctionName = CodeRegistrationWriter.Naming.Null;
                string str5 = CodeRegistrationWriter.Naming.Null;
                string str6 = CodeRegistrationWriter.Naming.Null;
                string str7 = CodeRegistrationWriter.Naming.ForIl2CppType(data.Type, 0);
                if (data.HasDelegatePInvokeWrapperMethod)
                {
                    @null = CodeRegistrationWriter.Naming.ForDelegatePInvokeWrapper(data.Type);
                    this.writer.WriteLine($"extern "C" void {@null}();");
                }
                if (data.HasPInvokeMarshalingFunctions)
                {
                    DefaultMarshalInfoWriter writer = MarshalDataCollector.MarshalInfoWriterFor(data.Type, MarshalType.PInvoke, null, false, false, false, null);
                    marshalToNativeFunctionName = writer.MarshalToNativeFunctionName;
                    marshalFromNativeFunctionName = writer.MarshalFromNativeFunctionName;
                    marshalCleanupFunctionName = writer.MarshalCleanupFunctionName;
                    this.writer.WriteLine($"extern "C" void {marshalToNativeFunctionName}(void* managedStructure, void* marshaledStructure);");
                    this.writer.WriteLine($"extern "C" void {marshalFromNativeFunctionName}(void* marshaledStructure, void* managedStructure);");
                    this.writer.WriteLine($"extern "C" void {marshalCleanupFunctionName}(void* marshaledStructure);");
                }
                if (data.HasCreateCCWFunction)
                {
                    str5 = CodeRegistrationWriter.Naming.ForCreateComCallableWrapperFunction(data.Type);
                    this.writer.WriteLine($"extern "C" Il2CppIManagedObjectHolder* {str5}(Il2CppObject* obj);");
                }
                if (data.HasGuid)
                {
                    if (data.Type.Resolve().HasCLSID())
                    {
                        str6 = $"&{CodeRegistrationWriter.Naming.ForTypeNameOnly(data.Type)}::CLSID";
                    }
                    else if (data.Type.HasIID())
                    {
                        str6 = $"&{CodeRegistrationWriter.Naming.ForTypeNameOnly(data.Type)}::IID";
                    }
                    else
                    {
                        TypeReference type = CodeRegistrationWriter.WindowsRuntimeProjections.ProjectToWindowsRuntime(data.Type);
                        if (type.IsWindowsRuntimeDelegate())
                        {
                            str6 = $"&{CodeRegistrationWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(data.Type)}::IID";
                        }
                        else
                        {
                            if (type == data.Type)
                            {
                                throw new InvalidOperationException($"InteropData says type ('{data.Type.FullName}') has a GUID, but no GUID could be found for it.");
                            }
                            this.writer.AddIncludeForTypeDefinition(type);
                            str6 = $"&{CodeRegistrationWriter.Naming.ForTypeNameOnly(type)}::IID";
                        }
                    }
                    this.writer.AddIncludeForTypeDefinition(data.Type);
                }
                this.writer.WriteLine($"extern const Il2CppType {str7};");
                return $"{{ {@null}, {marshalToNativeFunctionName}, {marshalFromNativeFunctionName}, {marshalCleanupFunctionName}, {str5}, {str6}, &{str7} }} /* {data.Type.FullName} */";
            }
        }
    }
}

