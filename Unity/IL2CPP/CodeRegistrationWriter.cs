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
        private static Func<MethodReference, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TypeDefinition, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodReference, string> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<TypeDefinition, string> <>f__mg$cache1;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorWriterService RuntimeInvokerCollectorWriter;

        private static string FormatIl2CppMarshalingFunction(TypeDefinition marshalableType)
        {
            DefaultMarshalInfoWriter writer = MarshalDataCollector.MarshalInfoWriterFor(marshalableType, MarshalType.PInvoke, null, false, false, false, null);
            return $"{{ {writer.MarshalToNativeFunctionName}, {writer.MarshalFromNativeFunctionName}, {writer.MarshalCleanupFunctionName} }}";
        }

        private static TableInfo WriteCCWMarshalingFunctions(NPath outputDir, IMethodCollectorResults methodCollector)
        {
            using (TinyProfiler.Section("CCWMarshalingFunctions", "Il2CppCcwMarshalingFunctionsTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<TypeDefinition> cCWMarshalingFunctions = methodCollector.GetCCWMarshalingFunctions();
                if (cCWMarshalingFunctions.Count > 0)
                {
                    string[] append = new string[] { "Il2CppCcwMarshalingFunctionsTable.cpp" };
                    using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                    {
                        writer.AddCodeGenIncludes();
                        foreach (TypeDefinition definition in cCWMarshalingFunctions)
                        {
                            object[] args = new object[] { Naming.ForCreateComCallableWrapperFunction(definition) };
                            writer.WriteLine("extern \"C\" void {0} ();", args);
                        }
                        if (<>f__am$cache2 == null)
                        {
                            <>f__am$cache2 = new Func<TypeDefinition, string>(null, (IntPtr) <WriteCCWMarshalingFunctions>m__2);
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_CcwMarshalingFunctions", cCWMarshalingFunctions.Select<TypeDefinition, string>(<>f__am$cache2), true);
                        empty = new TableInfo(cCWMarshalingFunctions.Count, "extern const Il2CppMethodPointer", "g_CcwMarshalingFunctions");
                    }
                }
                return empty;
            }
        }

        public static void WriteCodeRegistration(NPath outputDir, IMethodCollectorResults methodCollector, InflatedCollectionCollector genericsCollectionCollector, MethodTables methodPointerTables, TableInfo attributeGeneratorTable, TableInfo guidTable, UnresolvedVirtualsTablesInfo virtualCallTables)
        {
            TableInfo methodPointersTable = WriteMethodPointerTable(outputDir, methodCollector);
            TableInfo reversePInvokeWrappersTable = WriteReversePInvokeWrappersTable(outputDir, methodCollector);
            TableInfo delegateWrappersManagedToNativeTable = WriteDelegateWrappersManagedToNative(outputDir, methodCollector);
            TableInfo marshalingFunctionsTable = WriteMarshalingFunctions(outputDir, methodCollector);
            TableInfo ccwMarshalingFunctionsTable = WriteCCWMarshalingFunctions(outputDir, methodCollector);
            TableInfo genericMethodPointerTable = WriteGenericMethodPointerTable(outputDir, methodCollector, genericsCollectionCollector, methodPointerTables);
            TableInfo invokerTable = WriteInvokerTable(outputDir);
            WriteCodeRegistration(outputDir, invokerTable, methodPointersTable, reversePInvokeWrappersTable, delegateWrappersManagedToNativeTable, marshalingFunctionsTable, ccwMarshalingFunctionsTable, genericMethodPointerTable, attributeGeneratorTable, guidTable, virtualCallTables);
        }

        private static void WriteCodeRegistration(NPath outputDir, TableInfo invokerTable, TableInfo methodPointersTable, TableInfo reversePInvokeWrappersTable, TableInfo delegateWrappersManagedToNativeTable, TableInfo marshalingFunctionsTable, TableInfo ccwMarshalingFunctionsTable, TableInfo genericMethodPointerTable, TableInfo attributeGeneratorTable, TableInfo guidTable, UnresolvedVirtualsTablesInfo virtualCallTables)
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
                if (delegateWrappersManagedToNativeTable.Count > 0)
                {
                    object[] args = new object[] { delegateWrappersManagedToNativeTable.Type, delegateWrappersManagedToNativeTable.Name };
                    writer.WriteLine("{0} {1}[];", args);
                }
                if (marshalingFunctionsTable.Count > 0)
                {
                    object[] objArray2 = new object[] { marshalingFunctionsTable.Type, marshalingFunctionsTable.Name };
                    writer.WriteLine("{0} {1}[];", objArray2);
                }
                if (ccwMarshalingFunctionsTable.Count > 0)
                {
                    object[] objArray3 = new object[] { ccwMarshalingFunctionsTable.Type, ccwMarshalingFunctionsTable.Name };
                    writer.WriteLine("{0} {1}[];", objArray3);
                }
                if (genericMethodPointerTable.Count > 0)
                {
                    writer.WriteLine("extern const Il2CppMethodPointer g_Il2CppGenericMethodPointers[];");
                }
                if (invokerTable.Count > 0)
                {
                    object[] objArray4 = new object[] { invokerTable.Name };
                    writer.WriteLine("extern const InvokerMethod {0}[];", objArray4);
                }
                if (attributeGeneratorTable.Count > 0)
                {
                    object[] objArray5 = new object[] { attributeGeneratorTable.Type, attributeGeneratorTable.Name };
                    writer.WriteLine("{0} {1}[];", objArray5);
                }
                if (guidTable.Count > 0)
                {
                    object[] objArray6 = new object[] { guidTable.Type, guidTable.Name };
                    writer.WriteLine("{0} {1}[];", objArray6);
                }
                if (virtualCallTables.MethodPointersInfo.Count > 0)
                {
                    object[] objArray7 = new object[] { virtualCallTables.MethodPointersInfo.Type, virtualCallTables.MethodPointersInfo.Name };
                    writer.WriteLine("{0} {1}[];", objArray7);
                }
                string[] values = new string[] { 
                    methodPointersTable.Count.ToString(CultureInfo.InvariantCulture), (methodPointersTable.Count <= 0) ? Naming.Null : methodPointersTable.Name, reversePInvokeWrappersTable.Count.ToString(CultureInfo.InvariantCulture), (reversePInvokeWrappersTable.Count <= 0) ? Naming.Null : reversePInvokeWrappersTable.Name, delegateWrappersManagedToNativeTable.Count.ToString(CultureInfo.InvariantCulture), (delegateWrappersManagedToNativeTable.Count <= 0) ? Naming.Null : delegateWrappersManagedToNativeTable.Name, marshalingFunctionsTable.Count.ToString(CultureInfo.InvariantCulture), (marshalingFunctionsTable.Count <= 0) ? Naming.Null : marshalingFunctionsTable.Name, ccwMarshalingFunctionsTable.Count.ToString(CultureInfo.InvariantCulture), (ccwMarshalingFunctionsTable.Count <= 0) ? Naming.Null : ccwMarshalingFunctionsTable.Name, genericMethodPointerTable.Count.ToString(CultureInfo.InvariantCulture), (genericMethodPointerTable.Count <= 0) ? Naming.Null : genericMethodPointerTable.Name, invokerTable.Count.ToString(CultureInfo.InvariantCulture), (invokerTable.Count <= 0) ? Naming.Null : invokerTable.Name, attributeGeneratorTable.Count.ToString(CultureInfo.InvariantCulture), (attributeGeneratorTable.Count <= 0) ? Naming.Null : attributeGeneratorTable.Name,
                    guidTable.Count.ToString(CultureInfo.InvariantCulture), (guidTable.Count <= 0) ? Naming.Null : guidTable.Name, virtualCallTables.MethodPointersInfo.Count.ToString(CultureInfo.InvariantCulture), (virtualCallTables.MethodPointersInfo.Count <= 0) ? Naming.Null : virtualCallTables.MethodPointersInfo.Name
                };
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

        private static TableInfo WriteDelegateWrappersManagedToNative(NPath outputDir, IMethodCollectorResults methodCollector)
        {
            using (TinyProfiler.Section("DelegateWrappersManagedToNative", "Il2CppDelegateWrappersManagedToNativeTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<MethodReference> wrappersForDelegateFromManagedToNative = methodCollector.GetWrappersForDelegateFromManagedToNative();
                if (wrappersForDelegateFromManagedToNative.Count > 0)
                {
                    string[] append = new string[] { "Il2CppDelegateWrappersManagedToNativeTable.cpp" };
                    using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                    {
                        writer.AddCodeGenIncludes();
                        foreach (MethodReference reference in wrappersForDelegateFromManagedToNative)
                        {
                            object[] args = new object[] { Naming.ForDelegatePInvokeWrapper(reference.DeclaringType) };
                            writer.WriteLine("extern \"C\" void {0} ();", args);
                        }
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = new Func<MethodReference, string>(null, (IntPtr) <WriteDelegateWrappersManagedToNative>m__1);
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_DelegateWrappersManagedToNative", wrappersForDelegateFromManagedToNative.Select<MethodReference, string>(<>f__am$cache1), false);
                        empty = new TableInfo(wrappersForDelegateFromManagedToNative.Count, "extern const Il2CppMethodPointer", "g_DelegateWrappersManagedToNative");
                    }
                }
                return empty;
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

        private static TableInfo WriteInvokerTable(NPath outputDir)
        {
            using (TinyProfiler.Section("InvokerTable", "Il2CppInvokerTable.cpp"))
            {
                string[] append = new string[] { "Il2CppInvokerTable.cpp" };
                return RuntimeInvokerCollectorWriter.Write(outputDir.Combine(append));
            }
        }

        private static TableInfo WriteMarshalingFunctions(NPath outputDir, IMethodCollectorResults methodCollector)
        {
            using (TinyProfiler.Section("MarshalingFunctions", "Il2CppMarshalingFunctionsTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<TypeDefinition> typeMarshalingFunctions = methodCollector.GetTypeMarshalingFunctions();
                if (typeMarshalingFunctions.Count > 0)
                {
                    string[] append = new string[] { "Il2CppMarshalingFunctionsTable.cpp" };
                    using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                    {
                        writer.AddCodeGenIncludes();
                        foreach (TypeDefinition definition in typeMarshalingFunctions)
                        {
                            DefaultMarshalInfoWriter writer2 = MarshalDataCollector.MarshalInfoWriterFor(definition, MarshalType.PInvoke, null, false, false, false, null);
                            object[] args = new object[] { writer2.MarshalToNativeFunctionName };
                            writer.WriteLine("extern \"C\" void {0} ();", args);
                            object[] objArray2 = new object[] { writer2.MarshalFromNativeFunctionName };
                            writer.WriteLine("extern \"C\" void {0} ();", objArray2);
                            object[] objArray3 = new object[] { writer2.MarshalCleanupFunctionName };
                            writer.WriteLine("extern \"C\" void {0} ();", objArray3);
                        }
                        if (<>f__mg$cache1 == null)
                        {
                            <>f__mg$cache1 = new Func<TypeDefinition, string>(null, (IntPtr) FormatIl2CppMarshalingFunction);
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMarshalingFunctions", "g_MarshalingFunctions", typeMarshalingFunctions.Select<TypeDefinition, string>(<>f__mg$cache1), true);
                        empty = new TableInfo(typeMarshalingFunctions.Count, "extern const Il2CppMarshalingFunctions", "g_MarshalingFunctions");
                    }
                }
                return empty;
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
                            <>f__mg$cache0 = new Func<MethodReference, string>(null, (IntPtr) MethodTables.MethodPointerNameFor);
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_MethodPointers", methods.Select<MethodReference, string>(<>f__mg$cache0), false);
                        empty = new TableInfo(methods.Count, "extern const Il2CppMethodPointer", "g_MethodPointers");
                    }
                }
                return empty;
            }
        }

        private static TableInfo WriteReversePInvokeWrappersTable(NPath outputDir, IMethodCollectorResults methodCollector)
        {
            using (TinyProfiler.Section("ReversePInvokerWrapperTable", "Il2CppReversePInvokeWrapperTable.cpp"))
            {
                TableInfo empty = TableInfo.Empty;
                ReadOnlyCollection<MethodReference> reversePInvokeWrappers = methodCollector.GetReversePInvokeWrappers();
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
                            <>f__am$cache0 = new Func<MethodReference, string>(null, (IntPtr) <WriteReversePInvokeWrappersTable>m__0);
                        }
                        writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_ReversePInvokeWrapperPointers", reversePInvokeWrappers.Select<MethodReference, string>(<>f__am$cache0), false);
                        empty = new TableInfo(reversePInvokeWrappers.Count, "extern const Il2CppMethodPointer", "g_ReversePInvokeWrapperPointers");
                    }
                }
                return empty;
            }
        }
    }
}

