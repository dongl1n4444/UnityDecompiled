namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Rocks;
    using NiceIO;
    using System;
    using System.Collections.Generic;
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

    public sealed class MetadataCacheWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldReference, uint>, uint> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldReference, uint>, KeyValuePair<int, int>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, TypeReference> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<TableInfo, IEnumerable<string>> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<TableInfo, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<TableInfo, string> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<TypeDefinition, int, string> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheC;
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
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorReaderService RuntimeInvokerCollectorReader;
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IVirtualCallCollectorService VirtualCallCollector;

        private static void AddStreamAndRecordHeader(string name, Stream headerStream, Stream dataStream, Stream stream)
        {
            if ((dataStream.Position % 4L) != 0L)
            {
                throw new ArgumentException(string.Format("Data stream is not aligned to minimum alignment of {0}", 4), "dataStream");
            }
            if ((stream.Position % 4L) != 0L)
            {
                throw new ArgumentException(string.Format("Stream is not aligned to minimum alignment of {0}", 4), "stream");
            }
            StatsService.RecordMetadataStream(name, stream.Position);
            StreamExtensions.WriteLongAsInt(headerStream, 0x100L + dataStream.Position);
            StreamExtensions.WriteLongAsInt(headerStream, stream.Position);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.CopyTo(dataStream);
        }

        private static TypeReference BaseTypeFor(TypeDefinition type)
        {
            return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type).Resolve(type.Resolve().BaseType);
        }

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
                            throw new InvalidOperationException(string.Format("The packing size of {0} is not valid. Valid values are 0, 1, 2, 4, 8, 16, 32, 64, or 128.", packingSize));
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
            if (Unity.IL2CPP.Extensions.IsEnum(type))
            {
                return Unity.IL2CPP.Extensions.GetUnderlyingEnumType(type);
            }
            return type;
        }

        private static string FormatMethodTableEntry(MethodReference m, Dictionary<string, int> pointers)
        {
            int num;
            pointers.TryGetValue(MethodTables.MethodPointerFor(m), out num);
            object[] objArray1 = new object[] { "{ ", Il2CppGenericMethodCollector.GetIndex(m), ", ", num, "/*", MethodTables.MethodPointerFor(m), "*/, ", RuntimeInvokerCollectorReader.GetIndex(m), "/*", RuntimeInvokerCollectorReader.GetIndex(m), "*/}" };
            return string.Concat(objArray1);
        }

        private static CppCodeWriter GetMetadataCodeWriter(NPath outputDir, string tableName)
        {
            string[] append = new string[] { string.Format("Il2Cpp{0}Table.cpp", tableName) };
            SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append));
            IncludeWriter.WriteRegistrationIncludes(writer);
            return writer;
        }

        private static string InstanceSizeFor(TypeDefinition type)
        {
            if (Unity.IL2CPP.Extensions.IsInterface(type))
            {
                return "0";
            }
            return string.Format("sizeof ({0}){1}", Naming.ForType(type), !Unity.IL2CPP.Extensions.IsValueType(type) ? string.Empty : "+ sizeof (Il2CppObject)");
        }

        private static string OffsetOf(FieldDefinition field)
        {
            if (field.IsLiteral)
            {
                return "0";
            }
            if (Unity.IL2CPP.Extensions.IsThreadStatic(field))
            {
                return "THREAD_STATIC_FIELD_OFFSET";
            }
            if (field.DeclaringType.HasGenericParameters)
            {
                return "0";
            }
            if (Unity.IL2CPP.Extensions.IsNormalStatic(field))
            {
                return string.Format("{0}::{1}()", Naming.ForStaticFieldsStruct(field.DeclaringType), Naming.ForFieldOffsetGetter(field));
            }
            return string.Format("{0}::{1}(){2}", Naming.ForTypeNameOnly(field.DeclaringType), Naming.ForFieldOffsetGetter(field), !Unity.IL2CPP.Extensions.IsValueType(field.DeclaringType) ? "" : (" + static_cast<int32_t>(sizeof(" + Naming.ForType(TypeProvider.SystemObject) + "))"));
        }

        private static string Sizes(TypeDefinition type)
        {
            DefaultMarshalInfoWriter writer = MarshalDataCollector.MarshalInfoWriterFor(type, MarshalType.PInvoke, null, false, false, false, null);
            object[] args = new object[4];
            args[0] = !type.HasGenericParameters ? InstanceSizeFor(type) : "0";
            args[1] = !type.HasGenericParameters ? writer.NativeSize : "0";
            if (!type.HasGenericParameters && (<>f__am$cacheB == null))
            {
                <>f__am$cacheB = new Func<FieldDefinition, bool>(null, (IntPtr) <Sizes>m__B);
            }
            args[2] = (!Enumerable.Any<FieldDefinition>(type.Fields, <>f__am$cacheB) && !Unity.IL2CPP.Extensions.StoresNonFieldsInStaticFields(type)) ? "0" : string.Format("sizeof({0})", Naming.ForStaticFieldsStruct(type));
            if (!type.HasGenericParameters && (<>f__am$cacheC == null))
            {
                <>f__am$cacheC = new Func<FieldDefinition, bool>(null, (IntPtr) <Sizes>m__C);
            }
            args[3] = !Enumerable.Any<FieldDefinition>(type.Fields, <>f__am$cacheC) ? "0" : string.Format("sizeof({0})", Naming.ForThreadFieldsStruct(type));
            return string.Format("{0}, {1}, {2}, {3}", args);
        }

        internal static bool TypeDoesNotExceedMaximumRecursion(TypeReference type)
        {
            return (!type.IsGenericInstance || !GenericsUtilities.CheckForMaximumRecursion((GenericInstanceType) type));
        }

        internal static bool TypesDoNotExceedMaximumRecursion(IEnumerable<TypeReference> types)
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<TypeReference, bool>(null, (IntPtr) TypeDoesNotExceedMaximumRecursion);
            }
            return Enumerable.All<TypeReference>(types, <>f__mg$cache1);
        }

        public static TableInfo WriteFieldTable(CppCodeWriter writer, List<TableInfo> fieldTableInfos)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<TableInfo, bool>(null, (IntPtr) <WriteFieldTable>m__7);
            }
            foreach (TableInfo info in Enumerable.ToArray<TableInfo>(Enumerable.Where<TableInfo>(fieldTableInfos, <>f__am$cache7)))
            {
                object[] args = new object[] { info.Name, info.Count };
                writer.WriteLine("extern const int32_t {0}[{1}];", args);
            }
            IncludeWriter.WriteRegistrationIncludes(writer);
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Func<TableInfo, string>(null, (IntPtr) <WriteFieldTable>m__8);
            }
            return MetadataWriter.WriteTable<TableInfo>(writer, "extern const int32_t*", "g_FieldOffsetTable", fieldTableInfos, <>f__am$cache8);
        }

        public static void WriteMetadata(NPath outputDir, NPath dataFolder, InflatedCollectionCollector generics, TypeDefinition[] allTypeDefinitions, ICollection<AssemblyDefinition> usedAssemblies, MethodTables methodTables, IMetadataCollection metadataCollector, AttributeCollection attributeCollection, VTableBuilder vTableBuilder, IMethodCollectorResults methodCollector, UnresolvedVirtualsTablesInfo virtualCallTables)
        {
            TableInfo info;
            <WriteMetadata>c__AnonStorey0 storey = new <WriteMetadata>c__AnonStorey0 {
                methodTables = methodTables,
                metadataCollector = metadataCollector,
                attributeCollection = attributeCollection,
                methodCollector = methodCollector,
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
                <>f__am$cache0 = new Func<KeyValuePair<FieldReference, uint>, uint>(null, (IntPtr) <WriteMetadata>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<FieldReference, uint>, KeyValuePair<int, int>>(null, (IntPtr) <WriteMetadata>m__1);
            }
            storey.fieldRefs = Enumerable.ToArray<KeyValuePair<int, int>>(Enumerable.Select<KeyValuePair<FieldReference, uint>, KeyValuePair<int, int>>(Enumerable.OrderBy<KeyValuePair<FieldReference, uint>, uint>(FieldReferenceCollector.Fields, <>f__am$cache0), <>f__am$cache1));
            using (TinyProfiler.Section("GenericClasses", ""))
            {
                using (CppCodeWriter writer3 = GetMetadataCodeWriter(outputDir, "GenericClass"))
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = new Func<KeyValuePair<Il2CppTypeData, int>, bool>(null, (IntPtr) <WriteMetadata>m__2);
                    }
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = new Func<KeyValuePair<Il2CppTypeData, int>, TypeReference>(null, (IntPtr) <WriteMetadata>m__3);
                    }
                    TypeReference[] items = Enumerable.ToArray<TypeReference>(Enumerable.Distinct<TypeReference>(Enumerable.Select<KeyValuePair<Il2CppTypeData, int>, TypeReference>(Enumerable.Where<KeyValuePair<Il2CppTypeData, int>>(Il2CppTypeCollectorReader.Items, <>f__am$cache2), <>f__am$cache3), new Unity.IL2CPP.Common.TypeReferenceEqualityComparer()));
                    foreach (TypeReference reference in items)
                    {
                        writer3.WriteExternForGenericClass(reference);
                    }
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = new Func<TypeReference, string>(null, (IntPtr) <WriteMetadata>m__4);
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
            using (TinyProfiler.Section("GenericMethods", ""))
            {
                using (CppCodeWriter writer5 = GetMetadataCodeWriter(outputDir, "GenericMethod"))
                {
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = new Func<MethodReference, bool>(null, (IntPtr) <WriteMetadata>m__5);
                    }
                    MethodReference[] referenceArray3 = Enumerable.ToArray<MethodReference>(Enumerable.Where<MethodReference>(Il2CppGenericMethodCollector.Items.Keys, <>f__am$cache5));
                    source.Add(MetadataWriter.WriteTable<MethodReference>(writer5, "extern const Il2CppGenericMethodFunctionsDefinitions", "s_Il2CppGenericMethodFunctions", referenceArray3, new Func<MethodReference, string>(storey, (IntPtr) this.<>m__0)));
                }
            }
            using (TinyProfiler.Section("Il2CppTypes", ""))
            {
                string[] textArray3 = new string[] { "Il2CppTypeDefinitions.cpp" };
                using (SourceCodeWriter writer6 = new SourceCodeWriter(outputDir.Combine(textArray3)))
                {
                    source.Add(new Il2CppTypeWriter(writer6).WriteIl2CppTypeDefinitions(storey.metadataCollector));
                }
            }
            using (TinyProfiler.Section("GenericMethods", ""))
            {
                string[] textArray4 = new string[] { "Il2CppGenericMethodDefinitions.cpp" };
                using (SourceCodeWriter writer7 = new SourceCodeWriter(outputDir.Combine(textArray4)))
                {
                    source.Add(new Il2CppGenericMethodWriter(writer7).WriteIl2CppGenericMethodDefinitions(storey.metadataCollector));
                }
            }
            using (TinyProfiler.Section("CompilerCalculateTypeValues", ""))
            {
                using (CppCodeWriter writer8 = GetMetadataCodeWriter(outputDir, "CompilerCalculateTypeValues"))
                {
                    int num2 = 0;
                    List<TableInfo> fieldTableInfos = new List<TableInfo>();
                    int num3 = 0;
                    foreach (List<TypeDefinition> list3 in Unity.IL2CPP.Extensions.Chunk<TypeDefinition>(storey.metadataCollector.GetTypeInfos(), 100))
                    {
                        using (CppCodeWriter writer9 = GetMetadataCodeWriter(outputDir, "CompilerCalculateTypeValues_" + num2))
                        {
                            IncludeWriter.WriteRegistrationIncludes(writer9);
                            writer9.WriteClangWarningDisables();
                            foreach (TypeDefinition definition in list3)
                            {
                                writer9.AddIncludeForTypeDefinition(definition);
                                object[] args = new object[] { num3, Sizes(definition) };
                                writer9.WriteLine("extern const Il2CppTypeDefinitionSizes g_typeDefinitionSize{0} = {{ {1} }};", args);
                                if (<>f__mg$cache0 == null)
                                {
                                    <>f__mg$cache0 = new Func<FieldDefinition, string>(null, (IntPtr) OffsetOf);
                                }
                                fieldTableInfos.Add(MetadataWriter.WriteTable<FieldDefinition>(writer9, "extern const int32_t", "g_FieldOffsetTable" + num3, definition.Fields, <>f__mg$cache0));
                                num3++;
                            }
                            writer9.WriteClangWarningEnables();
                        }
                        num2++;
                    }
                    source.Add(WriteFieldTable(writer8, fieldTableInfos));
                    source.Add(WriteTypeDefinitionSizesTable(writer8, storey.metadataCollector, storey.attributeCollection));
                }
            }
            source.Add(info);
            using (TinyProfiler.Section("Registration", ""))
            {
                string[] textArray5 = new string[] { "Il2CppMetadataRegistration.cpp" };
                using (SourceCodeWriter writer10 = new SourceCodeWriter(outputDir.Combine(textArray5)))
                {
                    IncludeWriter.WriteRegistrationIncludes(writer10);
                    foreach (TableInfo info2 in source)
                    {
                        object[] objArray2 = new object[] { info2.Type, info2.Name };
                        writer10.WriteLine("{0} {1}[];", objArray2);
                    }
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = new Func<TableInfo, IEnumerable<string>>(null, (IntPtr) <WriteMetadata>m__6);
                    }
                    writer10.WriteStructInitializer("extern const Il2CppMetadataRegistration", "g_MetadataRegistration", Enumerable.SelectMany<TableInfo, string>(source, <>f__am$cache6));
                }
            }
            string[] textArray6 = new string[] { "Metadata" };
            NPath path = dataFolder.Combine(textArray6);
            path.CreateDirectory();
            string[] textArray7 = new string[] { "global-metadata.dat" };
            using (FileStream stream = new FileStream(path.Combine(textArray7).ToString(), FileMode.Create, FileAccess.Write))
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
                                    StreamExtensions.AlignTo(stream5, 4);
                                    AddStreamAndRecordHeader("String Literals", stream2, stream3, stream4);
                                    AddStreamAndRecordHeader("String Literal Data", stream2, stream3, stream5);
                                }
                            }
                        }
                        WriteMetadataToStream("Metadata Strings", stream2, stream3, new Action<MemoryStream>(storey.<>m__1));
                        WriteMetadataToStream("Events", stream2, stream3, new Action<MemoryStream>(storey.<>m__2));
                        WriteMetadataToStream("Properties", stream2, stream3, new Action<MemoryStream>(storey.<>m__3));
                        WriteMetadataToStream("Methods", stream2, stream3, new Action<MemoryStream>(storey.<>m__4));
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
                        WriteMetadataToStream("Field Refs", stream2, stream3, new Action<MemoryStream>(storey.<>m__18));
                        WriteMetadataToStream("Referenced Assemblies", stream2, stream3, new Action<MemoryStream>(storey.<>m__19));
                        WriteMetadataToStream("Attribute Types Ranges", stream2, stream3, new Action<MemoryStream>(storey.<>m__1A));
                        WriteMetadataToStream("Attribute Types", stream2, stream3, new Action<MemoryStream>(storey.<>m__1B));
                        WriteMetadataToStream("Unresolved Virtual Call Parameter Types", stream2, stream3, new Action<MemoryStream>(storey.<>m__1C));
                        WriteMetadataToStream("Unresolved Virtual Call Parameter Ranges", stream2, stream3, new Action<MemoryStream>(storey.<>m__1D));
                        StreamExtensions.WriteUInt(stream, 0xfab11baf);
                        StreamExtensions.WriteInt(stream, 0x16);
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
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Func<TypeDefinition, int, string>(null, (IntPtr) <WriteTypeDefinitionSizesTable>m__9);
            }
            string[] items = Enumerable.ToArray<string>(Enumerable.Select<TypeDefinition, string>(metadataCollector.GetTypeInfos(), <>f__am$cache9));
            foreach (string str in items)
            {
                object[] args = new object[] { str };
                writer.WriteLine("extern const Il2CppTypeDefinitionSizes {0};", args);
            }
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = new Func<string, string>(null, (IntPtr) <WriteTypeDefinitionSizesTable>m__A);
            }
            return MetadataWriter.WriteTable<string>(writer, "extern const Il2CppTypeDefinitionSizes*", "g_Il2CppTypeDefinitionSizesTable", items, <>f__am$cacheA);
        }

        [CompilerGenerated]
        private sealed class <WriteMetadata>c__AnonStorey0
        {
            private static Func<MethodDefinition, bool> <>f__am$cache0;
            private static Func<MethodDefinition, bool> <>f__am$cache1;
            internal AttributeCollection attributeCollection;
            internal KeyValuePair<int, int>[] fieldRefs;
            internal IMetadataCollection metadataCollector;
            internal IMethodCollectorResults methodCollector;
            internal MethodTables methodTables;
            internal List<KeyValuePair<uint, uint>> usageLists;
            internal List<KeyValuePair<uint, uint>> usagePairs;
            internal UnresolvedVirtualsTablesInfo virtualCallTables;
            internal VTableBuilder vTableBuilder;

            internal string <>m__0(MethodReference m)
            {
                return MetadataCacheWriter.FormatMethodTableEntry(m, this.methodTables.MethodPointers);
            }

            internal void <>m__1(MemoryStream stream)
            {
                byte[] buffer = Enumerable.ToArray<byte>(this.metadataCollector.GetStringData());
                stream.Write(buffer, 0, buffer.Length);
                StreamExtensions.AlignTo(stream, 4);
            }

            internal void <>m__10(MemoryStream stream)
            {
                foreach (uint num in this.metadataCollector.GetVTableMethods())
                {
                    StreamExtensions.WriteUInt(stream, num);
                }
            }

            internal void <>m__11(MemoryStream stream)
            {
                foreach (KeyValuePair<int, int> pair in this.metadataCollector.GetInterfaceOffsets())
                {
                    StreamExtensions.WriteInt(stream, pair.Key);
                    StreamExtensions.WriteInt(stream, pair.Value);
                }
            }

            internal void <>m__12(MemoryStream stream)
            {
                foreach (TypeDefinition definition in this.metadataCollector.GetTypeInfos())
                {
                    int count = 0;
                    int num2 = 0;
                    if (!definition.IsInterface || Unity.IL2CPP.Extensions.IsComOrWindowsRuntimeType(definition))
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
                            <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <>m__1E);
                        }
                        MethodDefinition[] definitionArray = Enumerable.ToArray<MethodDefinition>(Enumerable.Where<MethodDefinition>(definition.Methods, <>f__am$cache0));
                        length = definitionArray.Length;
                        if (length != 0)
                        {
                            methodIndex = this.metadataCollector.GetMethodIndex(definitionArray[0]);
                        }
                    }
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Namespace));
                    StreamExtensions.WriteUInt(stream, this.attributeCollection.GetIndex(definition));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition, 0));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.Il2CppTypeCollector.GetIndex(new ByReferenceType(definition), 0));
                    StreamExtensions.WriteInt(stream, (reference2 == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(reference2, 0));
                    StreamExtensions.WriteInt(stream, (type == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(type, 0));
                    StreamExtensions.WriteInt(stream, (reference3 == null) ? -1 : MetadataCacheWriter.Il2CppTypeCollector.GetIndex(reference3, 0));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetRGCTXEntriesStartIndex(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetRGCTXEntriesCount(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetGenericContainerIndex(definition));
                    if (Unity.IL2CPP.Extensions.IsDelegate(definition))
                    {
                    }
                    StreamExtensions.WriteInt(stream, (<>f__am$cache1 != null) ? -1 : this.methodCollector.GetWrapperForDelegateFromManagedToNativedIndex(Enumerable.Single<MethodDefinition>(definition.Methods, <>f__am$cache1)));
                    StreamExtensions.WriteInt(stream, this.methodCollector.GetTypeMarshalingFunctionsIndex(definition));
                    StreamExtensions.WriteInt(stream, this.methodCollector.GetCCWMarshalingFunctionIndex(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetGuidIndex(definition));
                    StreamExtensions.WriteUInt(stream, (uint) definition.Attributes);
                    StreamExtensions.WriteInt(stream, !definition.HasFields ? -1 : this.metadataCollector.GetFieldIndex(definition.Fields[0]));
                    StreamExtensions.WriteInt(stream, methodIndex);
                    StreamExtensions.WriteInt(stream, !definition.HasEvents ? -1 : this.metadataCollector.GetEventIndex(definition.Events[0]));
                    StreamExtensions.WriteInt(stream, !definition.HasProperties ? -1 : this.metadataCollector.GetPropertyIndex(definition.Properties[0]));
                    StreamExtensions.WriteInt(stream, !definition.HasNestedTypes ? -1 : this.metadataCollector.GetNestedTypesStartIndex(definition));
                    StreamExtensions.WriteInt(stream, !definition.HasInterfaces ? -1 : this.metadataCollector.GetInterfacesStartIndex(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetVTableMethodsStartIndex(definition));
                    StreamExtensions.WriteInt(stream, (definition.IsInterface && !Unity.IL2CPP.Extensions.IsComOrWindowsRuntimeType(definition)) ? -1 : this.metadataCollector.GetInterfaceOffsetsStartIndex(definition));
                    StreamExtensions.WriteIntAsUShort(stream, length);
                    StreamExtensions.WriteIntAsUShort(stream, definition.Properties.Count);
                    StreamExtensions.WriteIntAsUShort(stream, definition.Fields.Count);
                    StreamExtensions.WriteIntAsUShort(stream, definition.Events.Count);
                    StreamExtensions.WriteIntAsUShort(stream, definition.NestedTypes.Count);
                    StreamExtensions.WriteIntAsUShort(stream, count);
                    StreamExtensions.WriteIntAsUShort(stream, definition.Interfaces.Count);
                    StreamExtensions.WriteIntAsUShort(stream, num2);
                    int num5 = 0;
                    num5 |= ((definition.IsValueType == null) ? 0 : 1) << 0;
                    num5 |= ((definition.IsEnum == null) ? 0 : 1) << 1;
                    num5 |= ((Unity.IL2CPP.Extensions.HasFinalizer(definition) == null) ? 0 : 1) << 2;
                    num5 |= ((Unity.IL2CPP.Extensions.HasStaticConstructor(definition) == null) ? 0 : 1) << 3;
                    NativeType? nativeType = null;
                    num5 |= ((MarshalingUtils.IsBlittable(definition, nativeType, 0) == null) ? 0 : 1) << 4;
                    num5 |= ((Unity.IL2CPP.Extensions.IsComOrWindowsRuntimeType(definition) == null) ? 0 : 1) << 5;
                    int packingSize = TypeDefinitionWriter.FieldLayoutPackingSizeFor(definition);
                    if (packingSize != -1)
                    {
                        num5 |= ((int) MetadataCacheWriter.ConvertPackingSizeToCompressedEnum(packingSize)) << 6;
                    }
                    StreamExtensions.WriteInt(stream, num5);
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__13(MemoryStream stream)
            {
                foreach (KeyValuePair<int, uint> pair in this.metadataCollector.GetRGCTXEntries())
                {
                    StreamExtensions.WriteInt(stream, pair.Key);
                    StreamExtensions.WriteUInt(stream, pair.Value);
                }
            }

            internal void <>m__14(MemoryStream stream)
            {
                foreach (ModuleDefinition definition in this.metadataCollector.GetModules())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(Path.GetFileName(definition.FullyQualifiedName)));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetAssemblyIndex(definition.Assembly));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetTypeInfoIndex(definition.Types[0]));
                    StreamExtensions.WriteInt(stream, Enumerable.Count<TypeDefinition>(ModuleDefinitionRocks.GetAllTypes(definition)));
                    StreamExtensions.WriteInt(stream, (definition.Assembly.EntryPoint != null) ? this.metadataCollector.GetMethodIndex(definition.Assembly.EntryPoint) : -1);
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__15(MemoryStream stream)
            {
                foreach (AssemblyDefinition definition in this.metadataCollector.GetAssemblies())
                {
                    int num;
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetModuleIndex(definition.MainModule));
                    StreamExtensions.WriteInt(stream, (int) this.attributeCollection.GetIndex(definition));
                    int firstIndexInReferencedAssemblyTableForAssembly = this.metadataCollector.GetFirstIndexInReferencedAssemblyTableForAssembly(definition, out num);
                    StreamExtensions.WriteInt(stream, firstIndexInReferencedAssemblyTableForAssembly);
                    StreamExtensions.WriteInt(stream, num);
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name.Name));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name.Culture));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(Formatter.Stringify(definition.Name.Hash)));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(Formatter.Stringify(definition.Name.PublicKey)));
                    StreamExtensions.WriteUInt(stream, (uint) definition.Name.HashAlgorithm);
                    StreamExtensions.WriteInt(stream, definition.Name.Hash.Length);
                    StreamExtensions.WriteUInt(stream, (uint) definition.Name.Attributes);
                    StreamExtensions.WriteInt(stream, definition.Name.Version.Major);
                    StreamExtensions.WriteInt(stream, definition.Name.Version.Minor);
                    StreamExtensions.WriteInt(stream, definition.Name.Version.Build);
                    StreamExtensions.WriteInt(stream, definition.Name.Version.Revision);
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
                    StreamExtensions.WriteUInt(stream, pair.Key);
                    StreamExtensions.WriteUInt(stream, pair.Value);
                }
            }

            internal void <>m__17(MemoryStream stream)
            {
                foreach (KeyValuePair<uint, uint> pair in this.usagePairs)
                {
                    StreamExtensions.WriteUInt(stream, pair.Key);
                    StreamExtensions.WriteUInt(stream, pair.Value);
                }
            }

            internal void <>m__18(MemoryStream stream)
            {
                foreach (KeyValuePair<int, int> pair in this.fieldRefs)
                {
                    StreamExtensions.WriteInt(stream, pair.Key);
                    StreamExtensions.WriteInt(stream, pair.Value);
                }
            }

            internal void <>m__19(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetReferencedAssemblyIndiciesIntoAssemblyTable())
                {
                    StreamExtensions.WriteInt(stream, num);
                }
            }

            internal void <>m__1A(MemoryStream stream)
            {
                foreach (AttributeCollection.AttributeTypeRange range in this.attributeCollection.GetAttributeTypeRanges())
                {
                    StreamExtensions.WriteInt(stream, range.Start);
                    StreamExtensions.WriteInt(stream, range.Count);
                }
            }

            internal void <>m__1B(MemoryStream stream)
            {
                foreach (int num in this.attributeCollection.GetAttributeTypeIndices())
                {
                    StreamExtensions.WriteInt(stream, num);
                }
            }

            internal void <>m__1C(MemoryStream stream)
            {
                foreach (int num in this.virtualCallTables.SignatureTypesInfo)
                {
                    StreamExtensions.WriteInt(stream, num);
                }
            }

            internal void <>m__1D(MemoryStream stream)
            {
                foreach (Unity.IL2CPP.IoCServices.Range range in this.virtualCallTables.SignatureRangesInfo)
                {
                    StreamExtensions.WriteInt(stream, range.start);
                    StreamExtensions.WriteInt(stream, range.length);
                }
            }

            private static bool <>m__1E(MethodDefinition m)
            {
                return !Unity.IL2CPP.Extensions.IsStripped(m);
            }

            private static bool <>m__1F(MethodDefinition m)
            {
                return (m.Name == "Invoke");
            }

            internal void <>m__2(MemoryStream stream)
            {
                foreach (EventDefinition definition in this.metadataCollector.GetEvents())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.EventType, 0));
                    StreamExtensions.WriteInt(stream, (definition.AddMethod == null) ? -1 : (this.metadataCollector.GetMethodIndex(definition.AddMethod) - this.metadataCollector.GetMethodIndex(definition.DeclaringType.Methods[0])));
                    StreamExtensions.WriteInt(stream, (definition.RemoveMethod == null) ? -1 : (this.metadataCollector.GetMethodIndex(definition.RemoveMethod) - this.metadataCollector.GetMethodIndex(definition.DeclaringType.Methods[0])));
                    StreamExtensions.WriteInt(stream, (definition.InvokeMethod == null) ? -1 : (this.metadataCollector.GetMethodIndex(definition.InvokeMethod) - this.metadataCollector.GetMethodIndex(definition.DeclaringType.Methods[0])));
                    StreamExtensions.WriteInt(stream, (int) this.attributeCollection.GetIndex(definition));
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__3(MemoryStream stream)
            {
                foreach (PropertyDefinition definition in this.metadataCollector.GetProperties())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name));
                    StreamExtensions.WriteInt(stream, (definition.GetMethod == null) ? -1 : (this.metadataCollector.GetMethodIndex(definition.GetMethod) - this.metadataCollector.GetMethodIndex(definition.DeclaringType.Methods[0])));
                    StreamExtensions.WriteInt(stream, (definition.SetMethod == null) ? -1 : (this.metadataCollector.GetMethodIndex(definition.SetMethod) - this.metadataCollector.GetMethodIndex(definition.DeclaringType.Methods[0])));
                    StreamExtensions.WriteInt(stream, (int) definition.Attributes);
                    StreamExtensions.WriteInt(stream, (int) this.attributeCollection.GetIndex(definition));
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__4(MemoryStream stream)
            {
                foreach (MethodDefinition definition in this.metadataCollector.GetMethods())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetTypeInfoIndex(definition.DeclaringType));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.ReturnType, 0));
                    StreamExtensions.WriteInt(stream, !definition.HasParameters ? -1 : this.metadataCollector.GetParameterIndex(definition.Parameters[0]));
                    StreamExtensions.WriteInt(stream, (int) this.attributeCollection.GetIndex(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetGenericContainerIndex(definition));
                    StreamExtensions.WriteInt(stream, this.methodCollector.GetMethodIndex(definition));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.RuntimeInvokerCollectorReader.GetIndex(definition));
                    StreamExtensions.WriteInt(stream, this.methodCollector.GetReversePInvokeWrapperIndex(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetRGCTXEntriesStartIndex(definition));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetRGCTXEntriesCount(definition));
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                    StreamExtensions.WriteUShort(stream, (ushort) definition.Attributes);
                    StreamExtensions.WriteUShort(stream, (ushort) definition.ImplAttributes);
                    StreamExtensions.WriteUShort(stream, (ushort) this.vTableBuilder.IndexFor(definition));
                    StreamExtensions.WriteUShort(stream, (ushort) definition.Parameters.Count);
                }
            }

            internal void <>m__5(MemoryStream stream)
            {
                foreach (ParameterDefaultValue value2 in this.metadataCollector.GetParameterDefaultValues())
                {
                    StreamExtensions.WriteInt(stream, value2._parameterIndex);
                    StreamExtensions.WriteInt(stream, value2._typeIndex);
                    StreamExtensions.WriteInt(stream, value2._dataIndex);
                }
            }

            internal void <>m__6(MemoryStream stream)
            {
                foreach (FieldDefaultValue value2 in this.metadataCollector.GetFieldDefaultValues())
                {
                    StreamExtensions.WriteInt(stream, value2._fieldIndex);
                    StreamExtensions.WriteInt(stream, value2._typeIndex);
                    StreamExtensions.WriteInt(stream, value2._dataIndex);
                }
            }

            internal void <>m__7(MemoryStream stream)
            {
                byte[] buffer = Enumerable.ToArray<byte>(this.metadataCollector.GetDefaultValueData());
                stream.Write(buffer, 0, buffer.Length);
                StreamExtensions.AlignTo(stream, 4);
            }

            internal void <>m__8(MemoryStream stream)
            {
                foreach (FieldMarshaledSize size in this.metadataCollector.GetFieldMarshaledSizes())
                {
                    StreamExtensions.WriteInt(stream, size._fieldIndex);
                    StreamExtensions.WriteInt(stream, size._typeIndex);
                    StreamExtensions.WriteInt(stream, size._size);
                }
            }

            internal void <>m__9(MemoryStream stream)
            {
                foreach (ParameterDefinition definition in this.metadataCollector.GetParameters())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name));
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                    StreamExtensions.WriteInt(stream, (int) this.attributeCollection.GetIndex(definition, (MethodDefinition) definition.Method));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.ParameterType, (int) definition.Attributes));
                }
            }

            internal void <>m__A(MemoryStream stream)
            {
                foreach (FieldDefinition definition in this.metadataCollector.GetFields())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(definition.Name));
                    StreamExtensions.WriteInt(stream, MetadataCacheWriter.Il2CppTypeCollector.GetIndex(definition.FieldType, (int) definition.Attributes));
                    StreamExtensions.WriteInt(stream, (int) this.attributeCollection.GetIndex(definition));
                    StreamExtensions.WriteUInt(stream, definition.MetadataToken.ToUInt32());
                }
            }

            internal void <>m__B(MemoryStream stream)
            {
                foreach (GenericParameter parameter in this.metadataCollector.GetGenericParameters())
                {
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetGenericContainerIndex(parameter.Owner));
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetStringIndex(parameter.Name));
                    StreamExtensions.WriteShort(stream, (parameter.Constraints.Count <= 0) ? ((short) 0) : ((short) this.metadataCollector.GetGenericParameterConstraintsStartIndex(parameter)));
                    StreamExtensions.WriteShort(stream, (short) parameter.Constraints.Count);
                    StreamExtensions.WriteUShort(stream, (ushort) parameter.Position);
                    StreamExtensions.WriteUShort(stream, (ushort) parameter.Attributes);
                }
            }

            internal void <>m__C(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetGenericParameterConstraints())
                {
                    StreamExtensions.WriteInt(stream, num);
                }
            }

            internal void <>m__D(MemoryStream stream)
            {
                foreach (IGenericParameterProvider provider in this.metadataCollector.GetGenericContainers())
                {
                    StreamExtensions.WriteInt(stream, (provider.GenericParameterType != GenericParameterType.Method) ? this.metadataCollector.GetTypeInfoIndex((TypeDefinition) provider) : this.metadataCollector.GetMethodIndex((MethodDefinition) provider));
                    StreamExtensions.WriteInt(stream, provider.GenericParameters.Count);
                    StreamExtensions.WriteInt(stream, (provider.GenericParameterType != GenericParameterType.Method) ? 0 : 1);
                    StreamExtensions.WriteInt(stream, this.metadataCollector.GetGenericParameterIndex(provider.GenericParameters[0]));
                }
            }

            internal void <>m__E(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetNestedTypes())
                {
                    StreamExtensions.WriteInt(stream, num);
                }
            }

            internal void <>m__F(MemoryStream stream)
            {
                foreach (int num in this.metadataCollector.GetInterfaces())
                {
                    StreamExtensions.WriteInt(stream, num);
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

