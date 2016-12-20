namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.StringLiterals;

    public class MetadataUsageWriter : MetadataWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<string, uint>, uint> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, uint>, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, MetadataUsage>, string> <>f__am$cache2;
        [Inject]
        public static IIl2CppFieldReferenceCollectorWriterService Il2CppFieldReferenceCollector;
        [Inject]
        public static IIl2CppMethodReferenceCollectorWriterService Il2CppMethodReferenceCollector;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static IMetadataUsageCollectorReaderService MetadataUsageCollector;
        [Inject]
        public static IStringLiteralCollection StringLiteralCollection;

        public MetadataUsageWriter(CppCodeWriter writer) : base(writer)
        {
        }

        private static uint GetEncodedMetadataUsageIndex(uint index, Il2CppMetadataUsage type)
        {
            return ((((uint) type) << 0x1d) | index);
        }

        private List<KeyValuePair<uint, uint>> GetValues(MetadataUsage metadataUsage, Dictionary<string, uint> items, IMetadataCollection metadataCollection)
        {
            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>(metadataUsage.UsageCount);
            foreach (TypeReference reference in metadataUsage.GetIl2CppTypes())
            {
                list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeIl2CppType(reference))], GetEncodedMetadataUsageIndex((uint) Il2CppTypeCollector.GetOrCreateIndex(reference, 0), Il2CppMetadataUsage.Il2CppType)));
            }
            foreach (TypeReference reference2 in metadataUsage.GetTypeInfos())
            {
                list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeTypeInfo(reference2))], GetEncodedMetadataUsageIndex((uint) Il2CppTypeCollector.GetOrCreateIndex(reference2, 0), Il2CppMetadataUsage.Il2CppClass)));
            }
            foreach (MethodReference reference3 in metadataUsage.GetInflatedMethods())
            {
                list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeMethodInfo(reference3))], Il2CppMethodReferenceCollector.GetOrCreateIndex(reference3, metadataCollection)));
            }
            foreach (FieldReference reference4 in metadataUsage.GetFieldInfos())
            {
                list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeFieldInfo(reference4))], GetEncodedMetadataUsageIndex(Il2CppFieldReferenceCollector.GetOrCreateIndex(reference4), Il2CppMetadataUsage.FieldInfo)));
            }
            foreach (string str in metadataUsage.GetStringLiterals())
            {
                int index = StringLiteralCollection.GetIndex(str);
                list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForStringLiteralIdentifier(str))], GetEncodedMetadataUsageIndex((uint) index, Il2CppMetadataUsage.StringLiteral)));
            }
            return list;
        }

        public TableInfo WriteMetadataUsage(IMetadataCollection metadataCollection, List<KeyValuePair<uint, uint>> usagePairs1, List<KeyValuePair<uint, uint>> usageLists)
        {
            <WriteMetadataUsage>c__AnonStorey0 storey = new <WriteMetadataUsage>c__AnonStorey0 {
                metadataCollection = metadataCollection,
                $this = this
            };
            base.Writer.AddCodeGenIncludes();
            storey.items = new Dictionary<string, uint>(MetadataUsageCollector.UsageCount);
            foreach (TypeReference reference in MetadataUsageCollector.GetIl2CppTypes())
            {
                base.Writer.WriteStatement("const Il2CppType* " + MetadataWriter.Naming.ForRuntimeIl2CppType(reference));
                storey.items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeIl2CppType(reference)), (uint) storey.items.Count);
            }
            foreach (TypeReference reference2 in MetadataUsageCollector.GetTypeInfos())
            {
                base.Writer.WriteStatement("Il2CppClass* " + MetadataWriter.Naming.ForRuntimeTypeInfo(reference2));
                storey.items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeTypeInfo(reference2)), (uint) storey.items.Count);
            }
            foreach (MethodReference reference3 in MetadataUsageCollector.GetInflatedMethods())
            {
                base.Writer.WriteStatement("const MethodInfo* " + MetadataWriter.Naming.ForRuntimeMethodInfo(reference3));
                storey.items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeMethodInfo(reference3)), (uint) storey.items.Count);
            }
            foreach (FieldReference reference4 in MetadataUsageCollector.GetFieldInfos())
            {
                base.Writer.WriteStatement("FieldInfo* " + MetadataWriter.Naming.ForRuntimeFieldInfo(reference4));
                storey.items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeFieldInfo(reference4)), (uint) storey.items.Count);
            }
            foreach (string str in MetadataUsageCollector.GetStringLiterals())
            {
                base.Writer.WriteStatement("Il2CppCodeGenString* " + MetadataWriter.Naming.ForStringLiteralIdentifier(str));
                storey.items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForStringLiteralIdentifier(str)), (uint) storey.items.Count);
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<string, uint>, uint>(null, (IntPtr) <WriteMetadataUsage>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<string, uint>, string>(null, (IntPtr) <WriteMetadataUsage>m__1);
            }
            base.Writer.WriteArrayInitializer("extern void** const", "g_MetadataUsages", Enumerable.Select<KeyValuePair<string, uint>, string>(Enumerable.OrderBy<KeyValuePair<string, uint>, uint>(storey.items, <>f__am$cache0), <>f__am$cache1), false);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<KeyValuePair<string, MetadataUsage>, string>(null, (IntPtr) <WriteMetadataUsage>m__2);
            }
            KeyValuePair<string, MetadataUsage>[] source = Enumerable.ToArray<KeyValuePair<string, MetadataUsage>>(Enumerable.OrderBy<KeyValuePair<string, MetadataUsage>, string>(MetadataUsageCollector.GetUsages(), <>f__am$cache2));
            usagePairs1.AddRange(Enumerable.SelectMany<KeyValuePair<string, MetadataUsage>, KeyValuePair<uint, uint>>(source, new Func<KeyValuePair<string, MetadataUsage>, IEnumerable<KeyValuePair<uint, uint>>>(storey, (IntPtr) this.<>m__0)));
            int num = 0;
            uint key = 0;
            List<KeyValuePair<uint, uint>> collection = new List<KeyValuePair<uint, uint>>(source.Length);
            foreach (KeyValuePair<string, MetadataUsage> pair in source)
            {
                base.Writer.WriteStatement(string.Format("extern const uint32_t {0} = {1}", pair.Key, num++));
                collection.Add(new KeyValuePair<uint, uint>(key, (uint) pair.Value.UsageCount));
                key += (uint) pair.Value.UsageCount;
            }
            usageLists.AddRange(collection);
            return new TableInfo(source.Length, "extern void** const", "g_MetadataUsages");
        }

        [CompilerGenerated]
        private sealed class <WriteMetadataUsage>c__AnonStorey0
        {
            internal MetadataUsageWriter $this;
            internal Dictionary<string, uint> items;
            internal IMetadataCollection metadataCollection;

            internal IEnumerable<KeyValuePair<uint, uint>> <>m__0(KeyValuePair<string, MetadataUsage> item)
            {
                return this.$this.GetValues(item.Value, this.items, this.metadataCollection);
            }
        }

        private enum Il2CppMetadataUsage
        {
            Invalid,
            Il2CppClass,
            Il2CppType,
            MethodInfo,
            FieldInfo,
            StringLiteral,
            MethodRef
        }
    }
}

