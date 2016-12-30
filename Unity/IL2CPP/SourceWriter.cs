namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Com;
    using Unity.IL2CPP.Debugger;
    using Unity.IL2CPP.FileNaming;
    using Unity.IL2CPP.GenericsCollection;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.Symbols;
    using Unity.IL2CPP.WindowsRuntime;
    using Unity.TinyProfiling;

    public class SourceWriter
    {
        private readonly IDebuggerSupport _debuggerSupport;
        private readonly NPath _outputDir;
        private readonly VTableBuilder _vTableBuilder;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Action<Action> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, uint>, MethodReference> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<GenericInstanceMethod, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<InteropData, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<InteropData, TypeReference> <>f__am$cache7;
        [CompilerGenerated]
        private static Action<CppCodeWriter, TypeReference> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<ArrayType, ModuleDefinition> <>f__mg$cache0;
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorWriterService RuntimeInvokerCollectorWriter;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IVirtualCallCollectorService VirtualCallCollector;

        public SourceWriter(VTableBuilder vTableBuilder, IDebuggerSupport debuggerSupport, NPath outputDir)
        {
            this._vTableBuilder = vTableBuilder;
            this._debuggerSupport = debuggerSupport;
            this._outputDir = outputDir;
        }

        public void Write(AssemblyDefinition assemblyDefinition, ReadOnlyInflatedCollectionCollector allGenerics, NPath outputDir, TypeDefinition[] typeList, AttributeCollection attributeCollection, MethodCollector methodCollector, IInteropDataCollector interopDataCollector, IMetadataCollection metadataCollection, SymbolsCollector symbolsCollector)
        {
            string fileName = FileNameProvider.Instance.ForModule(assemblyDefinition.MainModule);
            using (TinyProfiler.Section("Code", ""))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => !t.HasGenericParameters && (!t.IsInterface || t.IsComOrWindowsRuntimeInterface());
                }
                TypeDefinition[] definitionArray = typeList.Where<TypeDefinition>(<>f__am$cache0).ToArray<TypeDefinition>();
                using (TinyProfiler.Section("Types", "Declarations"))
                {
                    foreach (TypeDefinition definition in definitionArray)
                    {
                        this.WriteTypeDefinitionFor(this._outputDir, definition, allGenerics, interopDataCollector);
                    }
                }
                using (TinyProfiler.Section("Methods", "Declarations"))
                {
                    foreach (TypeDefinition definition2 in definitionArray)
                    {
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = m => !m.HasGenericParameters;
                        }
                        this.WriteMethodDeclarationsFor(this._outputDir, <>f__am$cache1, definition2);
                    }
                }
                using (TinyProfiler.Section("Methods", "Definitions"))
                {
                    this.WriteMethodSourceFiles(this._outputDir, fileName, definitionArray, methodCollector, interopDataCollector, symbolsCollector, true);
                }
            }
        }

        public static void WriteCollectedMetadata(InflatedCollectionCollector genericsCollectionCollector, ICollection<AssemblyDefinition> usedAssemblies, NPath outputDir, NPath dataFolder, IMetadataCollection metadataCollection, AttributeCollection attributeCollection, VTableBuilder vTableBuilder, IMethodCollectorResults methodCollector, IInteropDataCollectorResults interopDataCollector)
        {
            TableInfo info;
            using (TinyProfiler.Section("Attributes", ""))
            {
                string[] append = new string[] { "Il2CppAttributes.cpp" };
                using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                {
                    writer.AddCodeGenIncludes();
                    info = new AttributesSupport(writer, attributeCollection).WriteAttributes(usedAssemblies);
                }
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = item => item.Key;
            }
            MethodTables methodPointerTables = MethodTables.CollectMethodTables(Il2CppGenericMethodCollector.Items.Select<KeyValuePair<MethodReference, uint>, MethodReference>(<>f__am$cache3));
            UnresolvedVirtualsTablesInfo virtualCallTables = VirtualCallCollector.WriteUnresolvedStubs(outputDir);
            using (TinyProfiler.Section("WriteCodeRegistration", ""))
            {
                CodeRegistrationWriter.WriteCodeRegistration(outputDir, methodCollector, interopDataCollector, genericsCollectionCollector, methodPointerTables, info, virtualCallTables);
            }
            using (TinyProfiler.Section("WriteMetadata", ""))
            {
                MetadataCacheWriter.WriteMetadata(outputDir, dataFolder, genericsCollectionCollector, null, usedAssemblies, methodPointerTables, metadataCollection, attributeCollection, vTableBuilder, methodCollector, interopDataCollector, virtualCallTables);
            }
        }

        public static void WriteComCallableWrappers(NPath outputDirectory, IInteropDataCollectorResults interopDataCollector)
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = interopData => interopData.HasCreateCCWFunction;
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = interopData => interopData.Type;
            }
            TypeReference[] items = interopDataCollector.GetInteropData().Where<InteropData>(<>f__am$cache6).Select<InteropData, TypeReference>(<>f__am$cache7).ToArray<TypeReference>();
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = delegate (CppCodeWriter writer, TypeReference type) {
                    if (type.IsArray)
                    {
                        new ArrayCCWWriter((ArrayType) type).Write(writer);
                    }
                    else if (type.Resolve().IsDelegate())
                    {
                        new DelegateCCWWriter(type).Write(writer);
                    }
                    else
                    {
                        new CCWWriter(type).Write(writer);
                    }
                    writer.WriteLine();
                    writer.WriteLine($"extern "C" Il2CppIManagedObjectHolder* {Naming.ForCreateComCallableWrapperFunction(type)}(Il2CppObject* obj)");
                    using (new BlockWriter(writer, false))
                    {
                        string str = Naming.ForComCallableWrapperClass(type);
                        writer.WriteLine($"void* memory = il2cpp::utils::Memory::Malloc(sizeof({str}));");
                        writer.WriteLine($"if (memory == {Naming.Null})");
                        using (new BlockWriter(writer, false))
                        {
                            writer.WriteLine("il2cpp_codegen_raise_out_of_memory_exception();");
                        }
                        writer.WriteLine();
                        writer.WriteLine($"return new(memory) {str}(obj);");
                    }
                };
            }
            WriteEqualSizedChunks<TypeReference>(outputDirectory, items, "Il2CppComCallableWrappers", 0x100000L, <>f__am$cache8);
        }

        private static void WriteEqualSizedChunks<T>(NPath outputDir, ICollection<T> items, string fileName, long chunkSize, Action<CppCodeWriter, T> writeItemAction)
        {
            InMemoryCodeWriter[] writerArray = new InMemoryCodeWriter[items.Count];
            List<SourceCodeWriter> list = new List<SourceCodeWriter>();
            int num = 0;
            foreach (T local in items)
            {
                InMemoryCodeWriter writer = writerArray[num++] = new InMemoryCodeWriter();
                writeItemAction(writer, local);
            }
            SourceCodeWriter item = null;
            long num2 = chunkSize;
            foreach (InMemoryCodeWriter writer4 in writerArray)
            {
                if ((num2 > chunkSize) || (item == null))
                {
                    string[] append = new string[] { $"{fileName}{list.Count}.cpp" };
                    item = new SourceCodeWriter(outputDir.Combine(append));
                    list.Add(item);
                    item.AddCodeGenIncludes();
                    num2 = 0L;
                }
                item.Write(writer4);
                num2 += writer4.Writer.BaseStream.Length;
                writer4.Dispose();
            }
            foreach (SourceCodeWriter writer5 in list)
            {
                writer5.Dispose();
            }
        }

        private void WriteGeneratedCodeGen()
        {
            string[] append = new string[] { "GeneratedCodeGen.h" };
            using (SourceCodeWriter writer = new SourceCodeWriter(this._outputDir.Combine(append)))
            {
                writer.WriteLine("#pragma once");
                string[] strArray = new string[] { "System.Object", "System.Array", "System.String", "System.Type", "System.IntPtr", "System.Exception", "System.RuntimeTypeHandle", "System.RuntimeFieldHandle", "System.RuntimeArgumentHandle", "System.RuntimeMethodHandle", "System.Text.StringBuilder", "System.MulticastDelegate", "System.Reflection.MethodBase" };
                foreach (string str in strArray)
                {
                    TypeDefinition type = TypeProvider.Corlib.MainModule.GetType(str);
                    if (type.IsValueType() || (type.Name == "Array"))
                    {
                        writer.AddIncludeForTypeDefinition(type);
                    }
                    else
                    {
                        writer.AddForwardDeclaration(type);
                    }
                    object[] args = new object[] { Naming.ForType(type), type.Name };
                    writer.WriteLine("typedef {0} Il2CppCodeGen{1};", args);
                }
            }
        }

        private void WriteGenericComDefinitions(ReadOnlyInflatedCollectionCollector genericsCollectionCollector, IInteropDataCollector interopDataCollector)
        {
            <WriteGenericComDefinitions>c__AnonStorey1 storey = new <WriteGenericComDefinitions>c__AnonStorey1 {
                interopDataCollector = interopDataCollector
            };
            WriteEqualSizedChunks<GenericInstanceType>(this._outputDir, genericsCollectionCollector.TypeDeclarations, "Il2CppGenericComDefinitions", 0x7d000L, new Action<CppCodeWriter, GenericInstanceType>(storey.<>m__0));
        }

        private void WriteGenericMethodDefinition(CppCodeWriter writer, GenericInstanceMethod method, IMethodCollector methodCollector)
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
            new MethodWriter(method.DeclaringType, writer, this._vTableBuilder).WriteMethodDefinition(method, methodCollector);
        }

        private void WriteGenericMethods(ReadOnlyInflatedCollectionCollector genericsCollectionCollector)
        {
            int num = 0;
            NullMethodCollector methodCollector = new NullMethodCollector();
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = mr => mr.FullName;
            }
            IOrderedEnumerable<GenericInstanceMethod> foo = genericsCollectionCollector.Methods.OrderBy<GenericInstanceMethod, string>(<>f__am$cache4);
            foreach (List<GenericInstanceMethod> list in foo.Chunk<GenericInstanceMethod>(0x3e8))
            {
                string[] append = new string[] { "GenericMethods" + num++ + ".cpp" };
                using (SourceCodeWriter writer = new SourceCodeWriter(this._outputDir.Combine(append)))
                {
                    foreach (GenericInstanceMethod method in list)
                    {
                        this.WriteGenericMethodDefinition(writer, method, methodCollector);
                    }
                }
            }
        }

        internal void WriteGenerics(ReadOnlyInflatedCollectionCollector genericsCollectionCollector, IEnumerable<TypeDefinition> allTypeDefinitions, IMethodCollector methodCollector, IInteropDataCollector interopDataCollector, IMetadataCollection metadataCollection, SymbolsCollector symbolsCollector)
        {
            <WriteGenerics>c__AnonStorey0 storey = new <WriteGenerics>c__AnonStorey0 {
                genericsCollectionCollector = genericsCollectionCollector,
                interopDataCollector = interopDataCollector,
                methodCollector = methodCollector,
                symbolsCollector = symbolsCollector,
                $this = this
            };
            List<Action> source = new List<Action> {
                new Action(storey.<>m__0),
                new Action(storey.<>m__1),
                new Action(storey.<>m__2),
                new Action(storey.<>m__3),
                new Action(storey.<>m__4),
                new Action(storey.<>m__5),
                new Action(this.WriteGeneratedCodeGen)
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = s => s();
            }
            ParallelHelper.ForEach<Action>(source, <>f__am$cache2, true, false);
        }

        private static void WriteMarshalingDefinitions(CppCodeWriter writer, TypeReference type, IInteropDataCollector interopDataCollector)
        {
            TypeDefinition definition = type.Resolve();
            if (definition.HasCLSID())
            {
                writer.AddIncludeForTypeDefinition(type);
                writer.WriteCommentedLine(type.FullName);
                writer.WriteStatement(Emit.Assign("const Il2CppGuid " + Naming.ForTypeNameOnly(type) + "::CLSID", definition.GetGuid().ToInitializer()));
            }
            else if (type.HasIID())
            {
                writer.AddIncludeForTypeDefinition(type);
                writer.WriteCommentedLine(type.FullName);
                writer.WriteStatement(Emit.Assign("const Il2CppGuid " + Naming.ForTypeNameOnly(type) + "::IID", type.GetGuid().ToInitializer()));
            }
            if (type.NeedsComCallableWrapper())
            {
                interopDataCollector.AddCCWMarshallingFunction(type);
            }
            foreach (MarshalType type2 in MarshalingUtils.GetMarshalTypesForMarshaledType(type))
            {
                MarshalDataCollector.MarshalInfoWriterFor(type, type2, null, false, false, false, null).WriteMarshalFunctionDefinitions(writer, interopDataCollector);
            }
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            foreach (MethodDefinition definition2 in definition.Methods)
            {
                MethodReference method = resolver.Resolve(definition2);
                if (!method.HasGenericParameters)
                {
                    MethodWriter.WriteMethodForDelegatePInvokeIfNeeded(writer, method, interopDataCollector);
                    if (ReversePInvokeMethodBodyWriter.IsReversePInvokeWrapperNecessary(method))
                    {
                        MethodWriter.WriteReversePInvokeMethodDefinition(writer, method, interopDataCollector);
                    }
                }
            }
        }

        private void WriteMethodDeclarationsFor(NPath outputDirectory, Func<MethodDefinition, bool> filter, GenericInstanceType type)
        {
            string[] append = new string[] { FileNameProvider.Instance.ForMethodDeclarations(type) };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDirectory.Combine(append)))
            {
                writer.AddStdInclude("stdint.h");
                writer.AddStdInclude("assert.h");
                writer.AddStdInclude("exception");
                writer.AddInclude("codegen/il2cpp-codegen.h");
                new MethodWriter(type, writer, this._vTableBuilder).WriteMethodDeclarationsFor(filter);
            }
        }

        private void WriteMethodDeclarationsFor(NPath outputDirectory, Func<MethodDefinition, bool> filter, TypeDefinition type)
        {
            string[] append = new string[] { FileNameProvider.Instance.ForMethodDeclarations(type) };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDirectory.Combine(append)))
            {
                writer.AddStdInclude("stdint.h");
                writer.AddStdInclude("assert.h");
                writer.AddStdInclude("exception");
                writer.AddInclude("codegen/il2cpp-codegen.h");
                new MethodWriter(type, writer, this._vTableBuilder).WriteMethodDeclarationsFor(filter);
            }
        }

        private void WriteMethodSourceFiles(NPath outputDirectory, string fileName, IEnumerable<TypeReference> typeList, IMethodCollector methodCollector, IInteropDataCollector interopDataCollector, SymbolsCollector symbolsCollector, bool writeMarshalingDefinitions)
        {
            int num = 0;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = tr => tr.FullName;
            }
            IOrderedEnumerable<TypeReference> foo = typeList.OrderBy<TypeReference, string>(<>f__am$cache5);
            foreach (List<TypeReference> list in foo.Chunk<TypeReference>(100))
            {
                object[] objArray1 = new object[] { "Bulk_", fileName, "_", num++, ".cpp" };
                string details = string.Concat(objArray1);
                using (TinyProfiler.Section("WriteBulkMethods", details))
                {
                    string[] textArray1 = new string[] { details };
                    using (SourceCodeWriter writer = new SourceCodeWriter(outputDirectory.Combine(textArray1)))
                    {
                        writer.AddInclude("class-internals.h");
                        writer.AddInclude("codegen/il2cpp-codegen.h");
                        writer.AddStdInclude("cstring");
                        writer.AddStdInclude("string.h");
                        writer.AddStdInclude("stdio.h");
                        writer.AddStdInclude("cmath");
                        writer.AddStdInclude("limits");
                        writer.AddStdInclude("assert.h");
                        writer.AddIncludeForTypeDefinition(TypeProvider.SystemArray);
                        this._debuggerSupport.WriteDebugMetadataIncludes(writer);
                        this._debuggerSupport.WriteDebugIncludes(writer);
                        writer.WriteClangWarningDisables();
                        foreach (TypeReference reference in list)
                        {
                            writer.AddIncludeForTypeDefinition(reference);
                            writer.AddIncludeForMethodDeclarations(reference);
                            if (writeMarshalingDefinitions)
                            {
                                WriteMarshalingDefinitions(writer, reference, interopDataCollector);
                            }
                            new MethodWriter(reference, writer, this._vTableBuilder).WriteMethodDefinitions(methodCollector);
                        }
                        writer.WriteClangWarningEnables();
                    }
                }
                string[] append = new string[] { details };
                symbolsCollector.CollectLineNumberInformation(outputDirectory.Combine(append));
            }
        }

        private void WriteTypeDefinitionFor(NPath outputDirectory, TypeReference type, ReadOnlyInflatedCollectionCollector genericsCollectionCollector, IInteropDataCollector interopDataCollector)
        {
            if (!type.IsInterface() || type.IsComOrWindowsRuntimeInterface())
            {
                string[] append = new string[] { FileNameProvider.Instance.ForTypeDefinition(type) };
                using (SourceCodeWriter writer = new SourceCodeWriter(outputDirectory.Combine(append)))
                {
                    writer.AddStdInclude("stdint.h");
                    if (type.IsSystemArray())
                    {
                        writer.WriteLine("struct Il2CppArrayBounds;");
                    }
                    if (!type.IsComOrWindowsRuntimeInterface())
                    {
                        new TypeDefinitionWriter().WriteTypeDefinitionFor(type, writer, interopDataCollector);
                    }
                    else
                    {
                        new ComInterfaceWriter(writer).WriteComInterfaceFor(type, genericsCollectionCollector, interopDataCollector);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WriteGenericComDefinitions>c__AnonStorey1
        {
            internal IInteropDataCollector interopDataCollector;

            internal void <>m__0(CppCodeWriter writer, GenericInstanceType type)
            {
                SourceWriter.WriteMarshalingDefinitions(writer, type, this.interopDataCollector);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteGenerics>c__AnonStorey0
        {
            internal SourceWriter $this;
            private static Func<GenericInstanceType, bool> <>f__am$cache0;
            private static Func<MethodDefinition, bool> <>f__am$cache1;
            internal ReadOnlyInflatedCollectionCollector genericsCollectionCollector;
            internal IInteropDataCollector interopDataCollector;
            internal IMethodCollector methodCollector;
            internal SymbolsCollector symbolsCollector;

            internal void <>m__0()
            {
                using (TinyProfiler.Section("GenericInstanceTypes Definitions", ""))
                {
                    foreach (GenericInstanceType type in this.genericsCollectionCollector.TypeDeclarations)
                    {
                        this.$this.WriteTypeDefinitionFor(this.$this._outputDir, type, this.genericsCollectionCollector, this.interopDataCollector);
                    }
                }
            }

            internal void <>m__1()
            {
                using (TinyProfiler.Section("GenericInstanceTypes Methods", ""))
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = t => !t.IsInterface() || t.IsComOrWindowsRuntimeInterface();
                    }
                    foreach (GenericInstanceType type in this.genericsCollectionCollector.TypeMethodDeclarations.Where<GenericInstanceType>(<>f__am$cache0))
                    {
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = m => !m.HasGenericParameters;
                        }
                        this.$this.WriteMethodDeclarationsFor(this.$this._outputDir, <>f__am$cache1, type);
                    }
                }
            }

            internal void <>m__2()
            {
                using (TinyProfiler.Section("Arrays Definitions", ""))
                {
                    if (SourceWriter.<>f__mg$cache0 == null)
                    {
                        SourceWriter.<>f__mg$cache0 = new Func<ArrayType, ModuleDefinition>(ArrayUtilities.ModuleDefinitionForElementTypeOf);
                    }
                    foreach (IGrouping<ModuleDefinition, ArrayType> grouping in this.genericsCollectionCollector.Arrays.GroupBy<ArrayType, ModuleDefinition>(SourceWriter.<>f__mg$cache0))
                    {
                        string[] append = new string[] { FileNameProvider.Instance.ForModule(grouping.Key) + "_ArrayTypes.h" };
                        using (SourceCodeWriter writer = new SourceCodeWriter(this.$this._outputDir.Combine(append)))
                        {
                            writer.AddIncludeForTypeDefinition(SourceWriter.TypeProvider.Corlib.MainModule.GetType("System.Array"));
                            foreach (ArrayType type in grouping)
                            {
                                writer.AddIncludeOrExternForTypeDefinition(type.ElementType);
                                new TypeDefinitionWriter().WriteArrayTypeDefinition(type, writer, this.interopDataCollector);
                            }
                        }
                    }
                }
            }

            internal void <>m__3()
            {
                using (TinyProfiler.Section("EmptyTypes Definitions", ""))
                {
                    foreach (GenericInstanceType type in this.genericsCollectionCollector.EmptyTypes)
                    {
                        this.$this.WriteTypeDefinitionFor(this.$this._outputDir, type, this.genericsCollectionCollector, this.interopDataCollector);
                    }
                }
            }

            internal void <>m__4()
            {
                using (TinyProfiler.Section("GenericInstanceMethods", ""))
                {
                    this.$this.WriteGenericMethods(this.genericsCollectionCollector);
                }
                using (TinyProfiler.Section("GenericInstanceTypes", ""))
                {
                    this.$this.WriteMethodSourceFiles(this.$this._outputDir, "Generics", (IEnumerable<TypeReference>) this.genericsCollectionCollector.Types, this.methodCollector, this.interopDataCollector, this.symbolsCollector, false);
                }
            }

            internal void <>m__5()
            {
                using (TinyProfiler.Section("GenericComDefinitions", ""))
                {
                    this.$this.WriteGenericComDefinitions(this.genericsCollectionCollector, this.interopDataCollector);
                }
            }

            private static bool <>m__6(GenericInstanceType t) => 
                (!t.IsInterface() || t.IsComOrWindowsRuntimeInterface());

            private static bool <>m__7(MethodDefinition m) => 
                !m.HasGenericParameters;
        }
    }
}

