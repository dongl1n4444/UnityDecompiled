using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.GenericsCollection;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public sealed class MethodTableWriter
	{
		[Inject]
		public static IIl2CppGenericMethodCollectorReaderService GenericMethodsCollection;

		[Inject]
		public static INamingService Naming;

		private readonly CppCodeWriter _writer;

		public MethodTableWriter(CppCodeWriter writer)
		{
			this._writer = writer;
		}

		public TableInfo Write(InflatedCollectionCollector generics, MethodTables methodTables, IMethodCollectorResults methodCollector)
		{
			IncludeWriter.WriteRegistrationIncludes(this._writer);
			MethodTableWriter.WriteIncludesFor(this._writer, generics);
			string[] array = (from value in methodTables.MethodPointers
			orderby value.Value
			select value into m
			select string.Concat(new object[]
			{
				m.Key,
				"/* ",
				m.Value,
				"*/"
			})).ToArray<string>();
			this._writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_Il2CppGenericMethodPointers", array, false);
			return new TableInfo(array.Length, "extern const Il2CppMethodPointer", "g_Il2CppGenericMethodPointers");
		}

		private static void WriteIncludesFor(CppCodeWriter writer, InflatedCollectionCollector generics)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (MethodReference current in from m in MethodTableWriter.GenericMethodsCollection.Items.Keys
			where !m.HasGenericParameters && !m.ContainsGenericParameters()
			select m)
			{
				string text = MethodTables.MethodPointerNameFor(current);
				if (text != MethodTableWriter.Naming.Null && hashSet.Add(text))
				{
					writer.WriteLine("extern \"C\" void {0} ();", new object[]
					{
						text
					});
				}
			}
		}
	}
}
