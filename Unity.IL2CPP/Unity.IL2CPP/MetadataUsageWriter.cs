using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;
using Unity.IL2CPP.StringLiterals;

namespace Unity.IL2CPP
{
	public class MetadataUsageWriter : MetadataWriter
	{
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

		[Inject]
		public static IMetadataUsageCollectorReaderService MetadataUsageCollector;

		[Inject]
		public static IStringLiteralCollection StringLiteralCollection;

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		[Inject]
		public static IIl2CppMethodReferenceCollectorWriterService Il2CppMethodReferenceCollector;

		[Inject]
		public static IIl2CppFieldReferenceCollectorWriterService Il2CppFieldReferenceCollector;

		public MetadataUsageWriter(CppCodeWriter writer) : base(writer)
		{
		}

		public TableInfo WriteMetadataUsage(IMetadataCollection metadataCollection, List<KeyValuePair<uint, uint>> usagePairs1, List<KeyValuePair<uint, uint>> usageLists)
		{
			base.Writer.AddCodeGenIncludes();
			Dictionary<string, uint> items = new Dictionary<string, uint>(MetadataUsageWriter.MetadataUsageCollector.UsageCount);
			foreach (TypeReference current in MetadataUsageWriter.MetadataUsageCollector.GetIl2CppTypes())
			{
				base.Writer.WriteStatement("const Il2CppType* " + MetadataWriter.Naming.ForRuntimeIl2CppType(current));
				items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeIl2CppType(current)), (uint)items.Count);
			}
			foreach (TypeReference current2 in MetadataUsageWriter.MetadataUsageCollector.GetTypeInfos())
			{
				base.Writer.WriteStatement("Il2CppClass* " + MetadataWriter.Naming.ForRuntimeTypeInfo(current2));
				items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeTypeInfo(current2)), (uint)items.Count);
			}
			foreach (MethodReference current3 in MetadataUsageWriter.MetadataUsageCollector.GetInflatedMethods())
			{
				base.Writer.WriteStatement("const MethodInfo* " + MetadataWriter.Naming.ForRuntimeMethodInfo(current3));
				items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeMethodInfo(current3)), (uint)items.Count);
			}
			foreach (FieldReference current4 in MetadataUsageWriter.MetadataUsageCollector.GetFieldInfos())
			{
				base.Writer.WriteStatement("FieldInfo* " + MetadataWriter.Naming.ForRuntimeFieldInfo(current4));
				items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeFieldInfo(current4)), (uint)items.Count);
			}
			foreach (string current5 in MetadataUsageWriter.MetadataUsageCollector.GetStringLiterals())
			{
				base.Writer.WriteStatement("Il2CppCodeGenString* " + MetadataWriter.Naming.ForStringLiteralIdentifier(current5));
				items.Add(MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForStringLiteralIdentifier(current5)), (uint)items.Count);
			}
			base.Writer.WriteArrayInitializer("extern void** const", "g_MetadataUsages", from item in items
			orderby item.Value
			select Emit.Cast("void**", item.Key), false);
			KeyValuePair<string, MetadataUsage>[] array = (from item in MetadataUsageWriter.MetadataUsageCollector.GetUsages()
			orderby item.Key
			select item).ToArray<KeyValuePair<string, MetadataUsage>>();
			usagePairs1.AddRange(array.SelectMany((KeyValuePair<string, MetadataUsage> item) => this.GetValues(item.Value, items, metadataCollection)));
			int num = 0;
			uint num2 = 0u;
			List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>(array.Length);
			KeyValuePair<string, MetadataUsage>[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				KeyValuePair<string, MetadataUsage> keyValuePair = array2[i];
				base.Writer.WriteStatement(string.Format("extern const uint32_t {0} = {1}", keyValuePair.Key, num++));
				list.Add(new KeyValuePair<uint, uint>(num2, (uint)keyValuePair.Value.UsageCount));
				num2 += (uint)keyValuePair.Value.UsageCount;
			}
			usageLists.AddRange(list);
			return new TableInfo(array.Length, "extern void** const", "g_MetadataUsages");
		}

		private List<KeyValuePair<uint, uint>> GetValues(MetadataUsage metadataUsage, Dictionary<string, uint> items, IMetadataCollection metadataCollection)
		{
			List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>(metadataUsage.UsageCount);
			foreach (TypeReference current in metadataUsage.GetIl2CppTypes())
			{
				list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeIl2CppType(current))], MetadataUsageWriter.GetEncodedMetadataUsageIndex((uint)MetadataUsageWriter.Il2CppTypeCollector.GetOrCreateIndex(current, 0), MetadataUsageWriter.Il2CppMetadataUsage.Il2CppType)));
			}
			foreach (TypeReference current2 in metadataUsage.GetTypeInfos())
			{
				list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeTypeInfo(current2))], MetadataUsageWriter.GetEncodedMetadataUsageIndex((uint)MetadataUsageWriter.Il2CppTypeCollector.GetOrCreateIndex(current2, 0), MetadataUsageWriter.Il2CppMetadataUsage.Il2CppClass)));
			}
			foreach (MethodReference current3 in metadataUsage.GetInflatedMethods())
			{
				list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeMethodInfo(current3))], MetadataUsageWriter.Il2CppMethodReferenceCollector.GetOrCreateIndex(current3, metadataCollection)));
			}
			foreach (FieldReference current4 in metadataUsage.GetFieldInfos())
			{
				list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForRuntimeFieldInfo(current4))], MetadataUsageWriter.GetEncodedMetadataUsageIndex(MetadataUsageWriter.Il2CppFieldReferenceCollector.GetOrCreateIndex(current4), MetadataUsageWriter.Il2CppMetadataUsage.FieldInfo)));
			}
			foreach (string current5 in metadataUsage.GetStringLiterals())
			{
				int index = MetadataUsageWriter.StringLiteralCollection.GetIndex(current5);
				list.Add(new KeyValuePair<uint, uint>(items[MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForStringLiteralIdentifier(current5))], MetadataUsageWriter.GetEncodedMetadataUsageIndex((uint)index, MetadataUsageWriter.Il2CppMetadataUsage.StringLiteral)));
			}
			return list;
		}

		private static uint GetEncodedMetadataUsageIndex(uint index, MetadataUsageWriter.Il2CppMetadataUsage type)
		{
			return (uint)((uint)type << 29 | (MetadataUsageWriter.Il2CppMetadataUsage)index);
		}
	}
}
