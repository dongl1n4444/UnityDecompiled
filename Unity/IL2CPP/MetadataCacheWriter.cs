namespace Unity.IL2CPP
{
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
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.Metadata.Fields;
    using Unity.IL2CPP.StringLiterals;
    using Unity.TinyProfiling;

    public sealed class MetadataCacheWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldReference, uint>, uint> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldReference, uint>, KeyValuePair<int, int>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, TypeReference> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, int> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, TypeReference> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<TableInfo, IEnumerable<string>> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldReference, uint>, uint> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldReference, uint>, FieldReference> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<TableInfo, bool> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<TableInfo, string> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<TypeDefinition, int, string> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cacheF;
        [CompilerGenerated]
        private static Func<FieldDefinition, string> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__mg$cache1;
        [Inject]
        public static IIl2CppFieldReferenceCollectorReaderService FieldReferenceCollector;
        [Inject]
        public static IIl2CppGenericInstCollectorReaderService Il2CppGenericInstCollector;
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static IIl2CppTypeCollectorReaderService Il2CppTypeCollectorReader;
        private const int kMinimumStreamAlignment = 4;
        [Inject]
        public static IMetadataUsageCollectorReaderService MetadataUsageCollector;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorReaderService RuntimeInvokerCollectorReader;
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static IStringLiteralCollection StringLiterals;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IVirtualCallCollectorService VirtualCallCollector;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        private static void AddStreamAndRecordHeader(string name, Stream headerStream, Stream dataStream, Stream stream)
        {
            if ((dataStream.Position % 4L) != 0L)
            {
                throw new ArgumentException($"Data stream is not aligned to minimum alignment of {4}", "dataStream");
            }
            if ((stream.Position % 4L) != 0L)
            {
                throw new ArgumentException($"Stream is not aligned to minimum alignment of {4}", "stream");
            }
            StatsService.RecordMetadataStream(name, stream.Position);
            headerStream.WriteLongAsInt(0x108L + dataStream.Position);
            headerStream.WriteLongAsInt(stream.Position);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.CopyTo(dataStream);
        }

        private static TypeReference BaseTypeFor(TypeDefinition type) => 
            Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type).Resolve(type.Resolve().BaseType);

        private static PackingSize ConvertPackingSizeToCompressedEnum(int packingSize)
        {
            switch (packingSize)
            {
                case 0:
                    return PackingSize.Zero;

                case 1:
                    return PackingSize.One;

                case 2:
                    return PackingSize.Two;

                case 4:
                    return PackingSize.Four;

                case 8:
                    return PackingSize.Eight;
            }
            if (packingSize != 0x10)
            {
                if (packingSize != 0x20)
                {
                    if (packingSize != 0x40)
                    {
                        if (packingSize != 0x80)
                        {
                            throw new InvalidOperationException($"The packing size of {packingSize} is not valid. Valid values are 0, 1, 2, 4, 8, 16, 32, 64, or 128.");
                        }
                        return PackingSize.OneHundredTwentyEight;
                    }
                    return PackingSize.SixtyFour;
                }
            }
            else
            {
                return PackingSize.Sixteen;
            }
            return PackingSize.ThirtyTwo;
        }

        private static TypeReference DeclaringTypeFor(TypeDefinition type)
        {
            if (!type.IsNested)
            {
                return null;
            }
            return type.DeclaringType;
        }

        private static TypeReference ElementTypeFor(TypeDefinition type)
        {
            if (type.IsEnum())
            {
                return type.GetUnderlyingEnumType();
            }
            return type;
        }

        private static MethodDefinition FirstNotStrippedMethodOf(TypeDefinition type)
        {
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = m => !m.IsStripped();
            }
            return type.Methods.First<MethodDefinition>(<>f__am$cacheB);
        }

        private static string FormatMethodTableEntry(MethodReference m, Dictionary<string, int> pointers)
        {
            int num;
            pointers.TryGetValue(MethodTables.MethodPointerFor(m), out num);
            object[] objArray1 = new object[] { "{ ", Il2CppGenericMethodCollector.GetIndex(m), ", ", num, "/*", MethodTables.MethodPointerFor(m), "*/, ", RuntimeInvokerCollectorReader.GetIndex(m), "/*", RuntimeInvokerCollectorReader.GetIndex(m), "*/}" };
            return string.Concat(objArray1);
        }

        internal static MethodReference[] GenericMethodTableEntries(ICollection<MethodReference> methodReferences)
        {
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = m => (!m.HasGenericParameters && !m.ContainsGenericParameters()) && TypeDoesNotExceedMaximumRecursion(m.DeclaringType);
            }
            return methodReferences.Where<MethodReference>(<>f__am$cacheA).ToArray<MethodReference>();
        }

        private static CppCodeWriter GetMetadataCodeWriter(NPath outputDir, string tableName)
        {
            string[] append = new string[] { $"Il2Cpp{tableName}Table.cpp" };
            SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append));
            IncludeWriter.WriteRegistrationIncludes(writer);
            return writer;
        }

        private static string InstanceSizeFor(TypeDefinition type)
        {
            if (type.IsInterface())
            {
                return "0";
            }
            return $"sizeof ({Naming.ForType(type)}){(!type.IsValueType() ? string.Empty : "+ sizeof (Il2CppObject)")}";
        }

        private static bool IsAvailableAndNotStripped(MethodDefinition method) => 
            ((method != null) && !method.IsStripped());

        private static string OffsetOf(FieldDefinition field)
        {
            if (field.IsLiteral)
            {
                return "0";
            }
            if (field.IsThreadStatic())
            {
                return "THREAD_STATIC_FIELD_OFFSET";
            }
            if (field.DeclaringType.HasGenericParameters)
            {
                return "0";
            }
            if (field.IsNormalStatic())
            {
                return $"{Naming.ForStaticFieldsStruct(field.DeclaringType)}::{Naming.ForFieldOffsetGetter(field)}()";
            }
            return $"{Naming.ForTypeNameOnly(field.DeclaringType)}::{Naming.ForFieldOffsetGetter(field)}(){(!field.DeclaringType.IsValueType() ? "" : (" + static_cast<int32_t>(sizeof(" + Naming.ForType(TypeProvider.SystemObject) + "))"))}";
        }

        private static string Sizes(TypeDefinition type)
        {
            DefaultMarshalInfoWriter writer = MarshalDataCollector.MarshalInfoWriterFor(type, MarshalType.PInvoke, null, false, false, false, null);
            object[] args = new object[4];
            args[0] = !type.HasGenericParameters ? InstanceSizeFor(type) : "0";
            args[1] = !type.HasGenericParameters ? writer.NativeSize : "0";
            if (!type.HasGenericParameters && (<>f__am$cache10 == null))
            {
                <>f__am$cache10 = f => f.IsNormalStatic();
            }
            args[2] = (!type.Fields.Any<FieldDefinition>(<>f__am$cache10) && !type.StoresNonFieldsInStaticFields()) ? "0" : $"sizeof({Naming.ForStaticFieldsStruct(type)})";
            if (!type.HasGenericParameters && (<>f__am$cache11 == null))
            {
                <>f__am$cache11 = f => f.IsThreadStatic();
            }
            args[3] = !type.Fields.Any<FieldDefinition>(<>f__am$cache11) ? "0" : $"sizeof({Naming.ForThreadFieldsStruct(type)})";
            return string.Format("{0}, {1}, {2}, {3}", args);
        }

        internal static bool TypeDoesNotExceedMaximumRecursion(TypeReference type) => 
            (!type.IsGenericInstance || !GenericsUtilities.CheckForMaximumRecursion((GenericInstanceType) type));

        internal static bool TypesDoNotExceedMaximumRecursion(IEnumerable<TypeReference> types)
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<TypeReference, bool>(MetadataCacheWriter.TypeDoesNotExceedMaximumRecursion);
            }
            return types.All<TypeReference>(<>f__mg$cache1);
        }

        public static TableInfo WriteFieldTable(CppCodeWriter writer, List<TableInfo> fieldTableInfos)
        {
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = item => item.Count > 0;
            }
            foreach (TableInfo info in fieldTableInfos.Where<TableInfo>(<>f__am$cacheC).ToArray<TableInfo>())
            {
                object[] args = new object[] { info.Name, info.Count };
                writer.WriteLine("extern const int32_t {0}[{1}];", args);
            }
            IncludeWriter.WriteRegistrationIncludes(writer);
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = table => (table.Count <= 0) ? Naming.Null : table.Name;
            }
            return MetadataWriter.WriteTable<TableInfo>(writer, "extern const int32_t*", "g_FieldOffsetTable", fieldTableInfos, <>f__am$cacheD);
        }

        public static void WriteMetadata(NPath outputDir, NPath dataFolder, TypeDefinition[] allTypeDefinitions, ICollection<AssemblyDefinition> usedAssemblies, MethodTables methodTables, IMetadataCollection metadataCollector, AttributeCollection attributeCollection, VTableBuilder vTableBuilder, IMethodCollectorResults methodCollector, IInteropDataCollectorResults interopDataCollector, UnresolvedVirtualsTablesInfo virtualCallTables)
        {
            TableInfo info;
            <WriteMetadata>c__AnonStorey0 storey = new <WriteMetadata>c__AnonStorey0 {
                methodTables = methodTables,
                metadataCollector = metadataCollector,
                attributeCollection = attributeCollection,
                methodCollector = methodCollector,
                interopDataCollector = interopDataCollector,
                vTableBuilder = vTableBuilder,
                virtualCallTables = virtualCallTables
            };
            List<TableInfo> source = new List<TableInfo>();
            storey.usagePairs = new List<KeyValuePair<uint, uint>>();
            storey.usageLists = new List<KeyValuePair<uint, uint>>();
            string[] append = new string[] { "Il2CppMetadataUsage.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                info = new MetadataUsageWriter(writer).WriteMetadataUsage(storey.metadataCollector, storey.usagePairs, storey.usageLists);
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = item => item.Value;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = item => new KeyValuePair<int, int>(Il2CppTypeCollector.GetIndex(item.Key.DeclaringType, 0), Naming.GetFieldIndex(item.Key, false));
            }
            storey.fieldRefs = FieldReferenceCollector.Fields.OrderBy<KeyValuePair<FieldReference, uint>, uint>(<>f__am$cache0).Select<KeyValuePair<FieldReference, uint>, KeyValuePair<int, int>>(<>f__am$cache1).ToArray<KeyValuePair<int, int>>();
            using (TinyProfiler.Section("GenericClasses", ""))
            {
                using (CppCodeWriter writer3 = GetMetadataCodeWriter(outputDir, "GenericClass"))
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = t => t.Key.Type.IsGenericInstance;
                    }
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = t => t.Key.Type;
                    }
                    TypeReference[] items = Il2CppTypeCollectorReader.Items.Where<KeyValuePair<Il2CppTypeData, int>>(<>f__am$cache2).Select<KeyValuePair<Il2CppTypeData, int>, TypeReference>(<>f__am$cache3).Distinct<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer()).ToArray<TypeReference>();
                    foreach (TypeReference reference in items)
                    {
                        writer3.WriteExternForGenericClass(reference);
                    }
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = t => "&" + Naming.ForGenericClass(t);
                    }
                    source.Add(MetadataWriter.WriteTable<TypeReference>(writer3, "extern Il2CppGenericClass* const", "s_Il2CppGenericTypes", items, <>f__am$cache4));
                }
            }
            using (TinyProfiler.Section("GenericInst2", ""))
            {
                string[] textArray2 = new string[] { "Il2CppGenericInstDefinitions.cpp" };
                using (SourceCodeWriter writer4 = new SourceCodeWriter(outputDir.Combine(textArray2)))
                {
                    source.Add(new Il2CppGenericInstWriter(writer4).WriteIl2CppGenericInstDefinitions());
                }
            }
            if (CodeGenOptions.MonoRuntime)
            {
                string[] textArray3 = new string[] { "Il2CppMonoGenericInstDefinitions.cpp" };
                using (SourceCodeWriter writer5 = new SourceCodeWriter(outputDir.Combine(textArray3)))
                {
                    new MonoGenericInstMetadataWriter(writer5).WriteMonoMetadataForGenericInstances();
                }
            }
            using (TinyProfiler.Section("GenericMethods", ""))
            {
                using (CppCodeWriter writer6 = GetMetadataCodeWriter(outputDir, "GenericMethod"))
                {
                    MethodReference[] referenceArray3 = GenericMethodTableEntries(Il2CppGenericMethodCollector.Items.Keys);
                    source.Add(MetadataWriter.WriteTable<MethodReference>(writer6, "extern const Il2CppGenericMethodFunctionsDefinitions", "s_Il2CppGenericMethodFunctions", referenceArray3, new Func<MethodReference, string>(storey.<>m__0)));
                }
            }
            using (TinyProfiler.Section("Il2CppTypes", ""))
            {
                string[] textArray4 = new string[] { "Il2CppTypeDefinitions.cpp" };
                using (SourceCodeWriter writer7 = new SourceCodeWriter(outputDir.Combine(textArray4)))
                {
                    source.Add(new Il2CppTypeWriter(writer7).WriteIl2CppTypeDefinitions(storey.metadataCollector));
                }
            }
            if (CodeGenOptions.MonoRuntime)
            {
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = kvp => kvp.Value;
                }
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = i => i.Key.Type;
                }
                MonoRuntimeDataWriter.WriteMonoMetadataForTypes(outputDir, Il2CppTypeCollectorReader.Items.OrderBy<KeyValuePair<Il2CppTypeData, int>, int>(<>f__am$cache5).Select<KeyValuePair<Il2CppTypeData, int>, TypeReference>(<>f__am$cache6).ToList<TypeReference>());
            }
            using (TinyProfiler.Section("GenericMethods", ""))
            {
                string[] textArray5 = new string[] { "Il2CppGenericMethodDefinitions.cpp" };
                using (SourceCodeWriter writer8 = new SourceCodeWriter(outputDir.Combine(textArray5)))
                {
                    source.Add(new Il2CppGenericMethodWriter(writer8).WriteIl2CppGenericMethodDefinitions(storey.metadataCollector));
                }
            }
            using (TinyProfiler.Section("CompilerCalculateTypeValues", ""))
            {
                using (CppCodeWriter writer9 = GetMetadataCodeWriter(outputDir, "CompilerCalculateTypeValues"))
                {
                    int num2 = 0;
                    List<TableInfo> fieldTableInfos = new List<TableInfo>();
                    int num3 = 0;
                    foreach (List<TypeDefinition> list3 in storey.metadataCollector.GetTypeInfos().Chunk<TypeDefinition>(100))
                    {
                        using (CppCodeWriter writer10 = GetMetadataCodeWriter(outputDir, "CompilerCalculateTypeValues_" + num2))
                        {
                            IncludeWriter.WriteRegistrationIncludes(writer10);
                            writer10.WriteClangWarningDisables();
                            foreach (TypeDefinition definition in list3)
                            {
                                writer10.AddIncludeForTypeDefinition(definition);
                                object[] args = new object[] { num3, Sizes(definition) };
                                writer10.WriteLine("extern const Il2CppTypeDefinitionSizes g_typeDefinitionSize{0} = {{ {1} }};", args);
                                if (<>f__mg$cache0 == null)
                                {
                                    <>f__mg$cache0 = new Func<FieldDefinition, string>(MetadataCacheWriter.OffsetOf);
                                }
                                fieldTableInfos.Add(MetadataWriter.WriteTable<FieldDefinition>(writer10, "extern const int32_t", "g_FieldOffsetTable" + num3, definition.Fields, <>f__mg$cache0));
                                num3++;
                            }
                            writer10.WriteClangWarningEnables();
                        }
                        num2++;
                    }
                    source.Add(WriteFieldTable(writer9, fieldTableInfos));
                    source.Add(WriteTypeDefinitionSizesTable(writer9, storey.metadataCollector, storey.attributeCollection));
                }
            }
            source.Add(info);
            using (TinyProfiler.Section("Registration", ""))
            {
                string[] textArray6 = new string[] { "Il2CppMetadataRegistration.cpp" };
                using (SourceCodeWriter writer11 = new SourceCodeWriter(outputDir.Combine(textArray6)))
                {
                    IncludeWriter.WriteRegistrationIncludes(writer11);
                    foreach (TableInfo info2 in source)
                    {
                        object[] objArray2 = new object[] { info2.Type, info2.Name };
                        writer11.WriteLine("{0} {1}[];", objArray2);
                    }
                    if (<>f__am$cache7 == null)
                    {
                        <>f__am$cache7 = table => new string[] { table.Count.ToString(CultureInfo.InvariantCulture), table.Name };
                    }
                    writer11.WriteStructInitializer("extern const Il2CppMetadataRegistration", "g_MetadataRegistration", source.SelectMany<TableInfo, string>(<>f__am$cache7));
                }
            }
            string[] textArray7 = new string[] { "Metadata" };
            NPath path = dataFolder.Combine(textArray7);
            path.CreateDirectory();
            string[] textArray8 = new string[] { "global-metadata.dat" };
            using (FileStream stream = new FileStream(path.Combine(textArray8).ToString(), FileMode.Create, FileAccess.Write))
            {
                using (MemoryStream stream2 = new MemoryStream())
                {
                    using (MemoryStream stream3 = new MemoryStream())
                    {
                        using (TinyProfiler.Section("StringLiteralWriter", ""))
                        {
                            using (MemoryStream stream4 = new MemoryStream())
                            {
                                using (MemoryStream stream5 = new MemoryStream())
                                {
                                    new StringLiteralWriter().Write(stream4, stream5);
                                    stream5.AlignTo(4);
                                    AddStreamAndRecordHeader("String Literals", stream2, stream3, stream4);
                                    AddStreamAndRecordHeader("String Literal Data", stream2, stream3, stream5);
                                }
                            }
                        }
                        WriteMetadataToStream("Metadata Strings", stream2, stream3, new Action<MemoryStream>(storey.<>m__1));
                        if (CodeGenOptions.MonoRuntime)
                        {
                            MonoRuntimeDataWriter.WriteMonoMetadataForStrings(outputDir, StringLiterals.GetStringMetadataTokens());
                        }
                        WriteMetadataToStream("Events", stream2, stream3, new Action<MemoryStream>(storey.<>m__2));
                        WriteMetadataToStream("Properties", stream2, stream3, new Action<MemoryStream>(storey.<>m__3));
                        WriteMetadataToStream("Methods", stream2, stream3, new Action<MemoryStream>(storey.<>m__4));
                        if (CodeGenOptions.MonoRuntime)
                        {
                            MonoRuntimeDataWriter.WriteMonoMetadataForMethods(outputDir, storey.metadataCollector.GetMethods().ToList<MethodDefinition>(), storey.interopDataCollector);
                        }
                        if (CodeGenOptions.MonoRuntime)
                        {
                            MonoRuntimeDataWriter.WriteMethodIndexTable(outputDir, storey.methodCollector, storey.metadataCollector);
                            MonoRuntimeDataWriter.WriteGenericMethodIndexTable(outputDir, storey.metadataCollector, storey.methodTables);
                        }
                        WriteMetadataToStream("Parameter Default Values", stream2, stream3, new Action<MemoryStream>(storey.<>m__5));
                        WriteMetadataToStream("Field Default Values", stream2, stream3, new Action<MemoryStream>(storey.<>m__6));
                        WriteMetadataToStream("Field and Parameter Default Values Data", stream2, stream3, new Action<MemoryStream>(storey.<>m__7));
                        WriteMetadataToStream("Field Marshaled Sizes", stream2, stream3, new Action<MemoryStream>(storey.<>m__8));
                        WriteMetadataToStream("Parameters", stream2, stream3, new Action<MemoryStream>(storey.<>m__9));
                        WriteMetadataToStream("Fields", stream2, stream3, new Action<MemoryStream>(storey.<>m__A));
                        WriteMetadataToStream("Generic Parameters", stream2, stream3, new Action<MemoryStream>(storey.<>m__B));
                        WriteMetadataToStream("Generic Parameter Constraints", stream2, stream3, new Action<MemoryStream>(storey.<>m__C));
                        WriteMetadataToStream("Generic Containers", stream2, stream3, new Action<MemoryStream>(storey.<>m__D));
                        WriteMetadataToStream("Nested Types", stream2, stream3, new Action<MemoryStream>(storey.<>m__E));
                        WriteMetadataToStream("Interfaces", stream2, stream3, new Action<MemoryStream>(storey.<>m__F));
                        WriteMetadataToStream("VTables", stream2, stream3, new Action<MemoryStream>(storey.<>m__10));
                        WriteMetadataToStream("Interface Offsets", stream2, stream3, new Action<MemoryStream>(storey.<>m__11));
                        WriteMetadataToStream("Type Definitions", stream2, stream3, new Action<MemoryStream>(storey.<>m__12));
                        WriteMetadataToStream("RGCTX Entries", stream2, stream3, new Action<MemoryStream>(storey.<>m__13));
                        WriteMetadataToStream("Images", stream2, stream3, new Action<MemoryStream>(storey.<>m__14));
                        WriteMetadataToStream("Assemblies", stream2, stream3, new Action<MemoryStream>(storey.<>m__15));
                        WriteMetadataToStream("Metadata Usage Lists", stream2, stream3, new Action<MemoryStream>(storey.<>m__16));
                        WriteMetadataToStream("Metadata Usage Pairs", stream2, stream3, new Action<MemoryStream>(storey.<>m__17));
                        if (CodeGenOptions.MonoRuntime)
                        {
                            MonoRuntimeDataWriter.WriteMetadataUsages(outputDir, storey.usagePairs, storey.usageLists);
                        }
                        WriteMetadataToStream("Field Refs", stream2, stream3, new Action<MemoryStream>(storey.<>m__18));
                        if (CodeGenOptions.MonoRuntime)
                        {
                            if (<>f__am$cache8 == null)
                            {
                                <>f__am$cache8 = item => item.Value;
                            }
                            if (<>f__am$cache9 == null)
                            {
                                <>f__am$cache9 = f => f.Key;
                            }
                            MonoRuntimeDataWriter.WriteMonoMetadataForFields(outputDir, FieldReferenceCollector.Fields.OrderBy<KeyValuePair<FieldReference, uint>, uint>(<>f__am$cache8).Select<KeyValuePair<FieldReference, uint>, FieldReference>(<>f__am$cache9).ToList<FieldReference>());
                        }
                        WriteMetadataToStream("Referenced Assemblies", stream2, stream3, new Action<MemoryStream>(storey.<>m__19));
                        WriteMetadataToStream("Attribute Types Ranges", stream2, stream3, new Action<MemoryStream>(storey.<>m__1A));
                        WriteMetadataToStream("Attribute Types", stream2, stream3, new Action<MemoryStream>(storey.<>m__1B));
                        WriteMetadataToStream("Unresolved Virtual Call Parameter Types", stream2, stream3, new Action<MemoryStream>(storey.<>m__1C));
                        WriteMetadataToStream("Unresolved Virtual Call Parameter Ranges", stream2, stream3, new Action<MemoryStream>(storey.<>m__1D));
                        WriteMetadataToStream("Windows Runtime type names", stream2, stream3, new Action<MemoryStream>(storey.<>m__1E));
                        stream.WriteUInt(0xfab11baf);
                        stream.WriteInt(0x17);
                        stream2.Seek(0L, SeekOrigin.Begin);
                        stream2.CopyTo(stream);
                        stream3.Seek(0L, SeekOrigin.Begin);
                        stream3.CopyTo(stream);
                    }
                }
            }
        }

        private static void WriteMetadataToStream(string name, MemoryStream headerStream, MemoryStream dataStream, Action<MemoryStream> callback)
        {
            using (TinyProfiler.Section(name, ""))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    callback(stream);
                    AddStreamAndRecordHeader(name, headerStream, dataStream, stream);
                }
            }
        }

        public static TableInfo WriteTypeDefinitionSizesTable(CppCodeWriter writer, IMetadataCollection metadataCollector, AttributeCollection attributeCollection)
        {
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = (type, index) => "g_typeDefinitionSize" + index;
            }
            string[] items = metadataCollector.GetTypeInfos().Select<TypeDefinition, string>(<>f__am$cacheE).ToArray<string>();
            foreach (string str in items)
            {
                object[] args = new object[] { str };
                writer.WriteLine("extern const Il2CppTypeDefinitionSizes {0};", args);
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = varName => Naming.AddressOf(varName);
            }
            return MetadataWriter.WriteTable<string>(writer, "extern const Il2CppTypeDefinitionSizes*", "g_Il2CppTypeDefinitionSizesTable", items, <>f__am$cacheF);
        }

        [CompilerGenerated]
        private sealed class <WriteMetadata>c__AnonStorey0
        {
            private static Func<MethodDefinition, bool> <>f__am$cache0;
            internal AttributeCollection attributeCollection;
            internal KeyValuePair<int, int>[] fieldRefs;
            internal IInteropDataCollectorResults interopDataCollector;
            internal IMetadataCollection metadataCollector;
            internal IMethodCollectorResults methodCollector;
            internal MethodTables methodTables;
            internal List<KeyValuePair<uint, uint>> usageLists;
            internal List<KeyValuePair<uint, uint>> usagePairs;
            internal UnresolvedVirtualsTablesInfo virtualCallTables;
            internal VTableBuilder vTableBuilder;

            internal string <>m__0(MethodReference m) => 
                MetadataCacheWriter.FormatMethodTableEntry(m, this.methodTables.MethodPointers);

            internal void <>m__1(MemoryStream stream)
            {
                byte[] buffer = this.metadataCollector.GetStringData().ToArray<byte>();
                stream.Write(buffer, 0, buffer.Length);
                stream.AlignTo(4);
            }

            internal void <>m__10(MemoryStream stream)
            {
                foreach (uint num in this.metadataCollector.GetVTableMethods())
                {
                    stream.WriteUInt(num);
                }
            }

            internal void <>m__11(MemoryStream stream)
            {
                foreach (KeyValuePair<int, int> pair in this.metadataCollector.GetInterfaceOffsets())
                {
                    stream.WriteInt(pair.Key);
                    stream.WriteInt(pair.Value);
                }
            }

            internal void <>m__12(MemoryStream stream)
            {
                foreach (TypeDefinition definition in this.metadataCollector.GetTypeInfos())
                {
                    int count = 0;
                    int num2 = 0;
                    if ((!definition.IsInterface || definition.IsComOrWindowsRuntimeType()) || (MetadataCacheWriter.WindowsRuntimeProjections.GetNativeToManagedAdapterClassFor(definition) != null))
                    {
                        VTable table = this.vTableBuilder.VTableFor(definition, null);
                        count = table.Slots.Count;
                        num2 = table.InterfaceOffsets.Count;
                    }
                    TypeReference type = MetadataCacheWriter.BaseTypeFor(definition);
                    TypeReference reference2 = MetadataCacheWriter.DeclaringTypeFor(definition);
                    TypeReference reference3 = MetadataCacheWriter.ElementTypeFor(definition);
                    int length = 0;
                    int methodIndex = -1;
                    if (definition.HasMethods)
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = m => !m.IsStripped();
                        }
                        MethodDefinition[] definitionArray = definition.Methods.Where<MethodDefinition>(<>f__am$cache0).ToArray<MethodDefinition>();
                        length = definitionArray.Length;
                        if (length != 0)
                        {
                            methodIndex = this.metadataCollector.GetMethodIndex(definitionArray[0]);
                        }
                    }
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name));
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Namespace));
                    stream.WriteUInt(this.attributeCollection.GetIndex(definition));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition, 0));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(new ByReferenceType(definition), 0));
                    stream.WriteInt((reference2 == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(reference2, 0));
                    stream.WriteInt((type == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(type, 0));
                    stream.WriteInt((reference3 == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(reference3, 0));
                    stream.WriteInt(this.metadataCollector.GetRGCTXEntriesStartIndex(definition));
                    stream.WriteInt(this.metadataCollector.GetRGCTXEntriesCount(definition));
                    stream.WriteInt(this.metadataCollector.GetGenericContainerIndex(definition));
                    stream.WriteUInt((uint) definition.Attributes);
                    stream.WriteInt(!definition.HasFields ? -1 : this.metadataCollector.GetFieldIndex(definition.Fields[0]));
                    stream.WriteInt(methodIndex);
                    stream.WriteInt(!definition.HasEvents ? -1 : this.metadataCollector.GetEventIndex(definition.Events[0]));
                    stream.WriteInt(!definition.HasProperties ? -1 : this.metadataCollector.GetPropertyIndex(definition.Properties[0]));
                    stream.WriteInt(!definition.HasNestedTypes ? -1 : this.metadataCollector.GetNestedTypesStartIndex(definition));
                    stream.WriteInt(!definition.HasInterfaces ? -1 : this.metadataCollector.GetInterfacesStartIndex(definition));
                    stream.WriteInt(this.metadataCollector.GetVTableMethodsStartIndex(definition));
                    stream.WriteInt((definition.IsInterface && !definition.IsComOrWindowsRuntimeType()) ? -1 : this.metadataCollector.GetInterfaceOffsetsStartIndex(definition));
                    stream.WriteIntAsUShort(length);
                    stream.WriteIntAsUShort(definition.Properties.Count);
                    stream.WriteIntAsUShort(definition.Fields.Count);
                    stream.WriteIntAsUShort(definition.Events.Count);
                    stream.WriteIntAsUShort(definition.NestedTypes.Count);
                    stream.WriteIntAsUShort(count);
                    stream.WriteIntAsUShort(definition.Interfaces.Count);
                    stream.WriteIntAsUShort(num2);
                    int num5 = 0;
                    num5 |= ((definition.IsValueType == null) ? 0 : 1) << 0;
                    num5 |= ((definition.IsEnum == null) ? 0 : 1) << 1;
                    num5 |= ((definition.HasFinalizer() == null) ? 0 : 1) << 2;
                    num5 |= ((definition.HasStaticConstructor() == null) ? 0 : 1) << 3;
                    NativeType? nativeType = null;
                    num5 |= ((MarshalingUtils.IsBlittable(definition, nativeType, 0, 0) == null) ? 0 : 1) << 4;
                    num5 |= ((definition.IsComOrWindowsRuntimeType() == null) ? 0 : 1) << 5;
                    int packingSize = TypeDefinitionWriter.FieldLayoutPackingSizeFor(definition);
                    if (packingSize != -1)
                    {
                        num5 |= ((int) MetadataCacheWriter.ConvertPackingSizeToCompressedEnum(packingSize)) << 6;
                    }
                    stream.WriteInt(num5);
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__13(MemoryStream stream)
            {
                foreach (RGCTXEntry entry in this.metadataCollector.GetRGCTXEntries())
                {
                    stream.WriteInt((int) entry.Type);
                    stream.WriteUInt(entry.TypeOrMethodMetadataIndex);
                }
            }

            internal void <>m__14(MemoryStream stream)
            {
                foreach (ModuleDefinition definition in this.metadataCollector.GetModules())
                {
                    if (definition.FileName == null)
                    {
                    }
                    stream.WriteInt(this.metadataCollector.GetStringIndex(Path.GetFileName(definition.Name)));
                    stream.WriteInt(this.metadataCollector.GetAssemblyIndex(definition.Assembly));
                    stream.WriteInt(this.metadataCollector.GetTypeInfoIndex(definition.Types[0]));
                    stream.WriteInt(definition.GetAllTypes().Count<TypeDefinition>());
                    stream.WriteInt((definition.Assembly.EntryPoint != null) ? this.metadataCollector.GetMethodIndex(definition.Assembly.EntryPoint) : -1);
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__15(MemoryStream stream)
            {
                foreach (AssemblyDefinition definition in this.metadataCollector.GetAssemblies())
                {
                    int num;
                    stream.WriteInt(this.metadataCollector.GetModuleIndex(definition.MainModule));
                    stream.WriteInt((int) this.attributeCollection.GetIndex(definition));
                    int firstIndexInReferencedAssemblyTableForAssembly = this.metadataCollector.GetFirstIndexInReferencedAssemblyTableForAssembly(definition, out num);
                    stream.WriteInt(firstIndexInReferencedAssemblyTableForAssembly);
                    stream.WriteInt(num);
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name.Name));
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name.Culture));
                    stream.WriteInt(this.metadataCollector.GetStringIndex(Formatter.Stringify(definition.Name.Hash)));
                    stream.WriteInt(this.metadataCollector.GetStringIndex(Formatter.Stringify(definition.Name.PublicKey)));
                    stream.WriteUInt((uint) definition.Name.HashAlgorithm);
                    stream.WriteInt(definition.Name.Hash.Length);
                    stream.WriteUInt((uint) definition.Name.Attributes);
                    stream.WriteInt(definition.Name.Version.Major);
                    stream.WriteInt(definition.Name.Version.Minor);
                    stream.WriteInt(definition.Name.Version.Build);
                    stream.WriteInt(definition.Name.Version.Revision);
                    byte[] buffer = (definition.Name.PublicKeyToken.Length <= 0) ? new byte[8] : definition.Name.PublicKeyToken;
                    foreach (byte num3 in buffer)
                    {
                        stream.WriteByte(num3);
                    }
                }
            }

            internal void <>m__16(MemoryStream stream)
            {
                foreach (KeyValuePair<uint, uint> pair in this.usageLists)
                {
                    stream.WriteUInt(pair.Key);
                    stream.WriteUInt(pair.Value);
                }
            }

            internal void <>m__17(MemoryStream stream)
            {
                foreach (KeyValuePair<uint, uint> pair in this.usagePairs)
                {
                    stream.WriteUInt(pair.Key);
                    stream.WriteUInt(pair.Value);
                }
            }

            internal void <>m__18(MemoryStream stream)
            {
                foreach (KeyValuePair<int, int> pair in this.fieldRefs)
                {
                    stream.WriteInt(pair.Key);
                    stream.WriteInt(pair.Value);
                }
            }

            internal void <>m__19(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetReferencedAssemblyIndiciesIntoAssemblyTable())
                {
                    stream.WriteInt(num);
                }
            }

            internal void <>m__1A(MemoryStream stream)
            {
                foreach (AttributeCollection.AttributeTypeRange range in this.attributeCollection.GetAttributeTypeRanges())
                {
                    stream.WriteInt(range.Start);
                    stream.WriteInt(range.Count);
                }
            }

            internal void <>m__1B(MemoryStream stream)
            {
                foreach (int num in this.attributeCollection.GetAttributeTypeIndices())
                {
                    stream.WriteInt(num);
                }
            }

            internal void <>m__1C(MemoryStream stream)
            {
                foreach (int num in this.virtualCallTables.SignatureTypesInfo)
                {
                    stream.WriteInt(num);
                }
            }

            internal void <>m__1D(MemoryStream stream)
            {
                foreach (Unity.IL2CPP.IoCServices.Range range in this.virtualCallTables.SignatureRangesInfo)
                {
                    stream.WriteInt(range.start);
                    stream.WriteInt(range.length);
                }
            }

            internal void <>m__1E(MemoryStream stream)
            {
                foreach (Tuple<TypeReference, string> tuple in this.interopDataCollector.GetWindowsRuntimeTypesWithNames())
                {
                    stream.WriteInt(this.metadataCollector.GetStringIndex(tuple.Item2));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(tuple.Item1, 0));
                }
            }

            private static bool <>m__1F(MethodDefinition m) => 
                !m.IsStripped();

            internal void <>m__2(MemoryStream stream)
            {
                foreach (EventDefinition definition in this.metadataCollector.GetEvents())
                {
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.EventType, 0));
                    stream.WriteInt(!MetadataCacheWriter.IsAvailableAndNotStripped(definition.AddMethod) ? -1 : (this.metadataCollector.GetMethodIndex(definition.AddMethod) - this.metadataCollector.GetMethodIndex(MetadataCacheWriter.FirstNotStrippedMethodOf(definition.DeclaringType))));
                    stream.WriteInt(!MetadataCacheWriter.IsAvailableAndNotStripped(definition.RemoveMethod) ? -1 : (this.metadataCollector.GetMethodIndex(definition.RemoveMethod) - this.metadataCollector.GetMethodIndex(MetadataCacheWriter.FirstNotStrippedMethodOf(definition.DeclaringType))));
                    stream.WriteInt(!MetadataCacheWriter.IsAvailableAndNotStripped(definition.InvokeMethod) ? -1 : (this.metadataCollector.GetMethodIndex(definition.InvokeMethod) - this.metadataCollector.GetMethodIndex(MetadataCacheWriter.FirstNotStrippedMethodOf(definition.DeclaringType))));
                    stream.WriteInt((int) this.attributeCollection.GetIndex(definition));
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__3(MemoryStream stream)
            {
                foreach (PropertyDefinition definition in this.metadataCollector.GetProperties())
                {
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name));
                    stream.WriteInt(!MetadataCacheWriter.IsAvailableAndNotStripped(definition.GetMethod) ? -1 : (this.metadataCollector.GetMethodIndex(definition.GetMethod) - this.metadataCollector.GetMethodIndex(MetadataCacheWriter.FirstNotStrippedMethodOf(definition.DeclaringType))));
                    stream.WriteInt(!MetadataCacheWriter.IsAvailableAndNotStripped(definition.SetMethod) ? -1 : (this.metadataCollector.GetMethodIndex(definition.SetMethod) - this.metadataCollector.GetMethodIndex(MetadataCacheWriter.FirstNotStrippedMethodOf(definition.DeclaringType))));
                    stream.WriteInt((int) definition.Attributes);
                    stream.WriteInt((int) this.attributeCollection.GetIndex(definition));
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__4(MemoryStream stream)
            {
                foreach (MethodDefinition definition in this.metadataCollector.GetMethods())
                {
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name));
                    stream.WriteInt(this.metadataCollector.GetTypeInfoIndex(definition.DeclaringType));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.ReturnType, 0));
                    stream.WriteInt(!definition.HasParameters ? -1 : this.metadataCollector.GetParameterIndex(definition.Parameters[0]));
                    stream.WriteInt((int) this.attributeCollection.GetIndex(definition));
                    stream.WriteInt(this.metadataCollector.GetGenericContainerIndex(definition));
                    stream.WriteInt(this.methodCollector.GetMethodIndex(definition));
                    stream.WriteInt(MetadataCacheWriter.RuntimeInvokerCollectorReader.GetIndex(definition));
                    stream.WriteInt(this.interopDataCollector.GetReversePInvokeWrapperIndex(definition));
                    stream.WriteInt(this.metadataCollector.GetRGCTXEntriesStartIndex(definition));
                    stream.WriteInt(this.metadataCollector.GetRGCTXEntriesCount(definition));
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                    stream.WriteUShort((ushort) definition.Attributes);
                    stream.WriteUShort((ushort) definition.ImplAttributes);
                    stream.WriteUShort((ushort) this.vTableBuilder.IndexFor(definition));
                    stream.WriteUShort((ushort) definition.Parameters.Count);
                }
            }

            internal void <>m__5(MemoryStream stream)
            {
                foreach (ParameterDefaultValue value2 in this.metadataCollector.GetParameterDefaultValues())
                {
                    stream.WriteInt(value2._parameterIndex);
                    stream.WriteInt(value2._typeIndex);
                    stream.WriteInt(value2._dataIndex);
                }
            }

            internal void <>m__6(MemoryStream stream)
            {
                foreach (FieldDefaultValue value2 in this.metadataCollector.GetFieldDefaultValues())
                {
                    stream.WriteInt(value2._fieldIndex);
                    stream.WriteInt(value2._typeIndex);
                    stream.WriteInt(value2._dataIndex);
                }
            }

            internal void <>m__7(MemoryStream stream)
            {
                byte[] buffer = this.metadataCollector.GetDefaultValueData().ToArray<byte>();
                stream.Write(buffer, 0, buffer.Length);
                stream.AlignTo(4);
            }

            internal void <>m__8(MemoryStream stream)
            {
                foreach (FieldMarshaledSize size in this.metadataCollector.GetFieldMarshaledSizes())
                {
                    stream.WriteInt(size._fieldIndex);
                    stream.WriteInt(size._typeIndex);
                    stream.WriteInt(size._size);
                }
            }

            internal void <>m__9(MemoryStream stream)
            {
                foreach (ParameterDefinition definition in this.metadataCollector.GetParameters())
                {
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name));
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                    stream.WriteInt((int) this.attributeCollection.GetIndex(definition, (MethodDefinition) definition.Method));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.ParameterType, (int) definition.Attributes));
                }
            }

            internal void <>m__A(MemoryStream stream)
            {
                foreach (FieldDefinition definition in this.metadataCollector.GetFields())
                {
                    stream.WriteInt(this.metadataCollector.GetStringIndex(definition.Name));
                    stream.WriteInt(MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.FieldType, (int) definition.Attributes));
                    stream.WriteInt((int) this.attributeCollection.GetIndex(definition));
                    stream.WriteUInt(definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__B(MemoryStream stream)
            {
                foreach (GenericParameter parameter in this.metadataCollector.GetGenericParameters())
                {
                    stream.WriteInt(this.metadataCollector.GetGenericContainerIndex(parameter.Owner));
                    stream.WriteInt(this.metadataCollector.GetStringIndex(parameter.Name));
                    stream.WriteShort((parameter.Constraints.Count <= 0) ? ((short) 0) : ((short) this.metadataCollector.GetGenericParameterConstraintsStartIndex(parameter)));
                    stream.WriteShort((short) parameter.Constraints.Count);
                    stream.WriteUShort((ushort) parameter.Position);
                    stream.WriteUShort((ushort) parameter.Attributes);
                }
            }

            internal void <>m__C(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetGenericParameterConstraints())
                {
                    stream.WriteInt(num);
                }
            }

            internal void <>m__D(MemoryStream stream)
            {
                foreach (IGenericParameterProvider provider in this.metadataCollector.GetGenericContainers())
                {
                    stream.WriteInt((provider.GenericParameterType != GenericParameterType.Method) ? this.metadataCollector.GetTypeInfoIndex((TypeDefinition) provider) : this.metadataCollector.GetMethodIndex((MethodDefinition) provider));
                    stream.WriteInt(provider.GenericParameters.Count);
                    stream.WriteInt((provider.GenericParameterType != GenericParameterType.Method) ? 0 : 1);
                    stream.WriteInt(this.metadataCollector.GetGenericParameterIndex(provider.GenericParameters[0]));
                }
            }

            internal void <>m__E(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetNestedTypes())
                {
                    stream.WriteInt(num);
                }
            }

            internal void <>m__F(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetInterfaces())
                {
                    stream.WriteInt(num);
                }
            }
        }

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
    }
}

