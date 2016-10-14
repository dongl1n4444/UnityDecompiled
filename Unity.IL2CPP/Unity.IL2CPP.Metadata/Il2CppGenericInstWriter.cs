using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class Il2CppGenericInstWriter : MetadataWriter
	{
		[Inject]
		public static IIl2CppGenericInstCollectorReaderService Il2CppGenericInstCollector;

		[Inject]
		public static IIl2CppTypeCollectorWriterService TypeCollector;

		public const string TableVariableName = "g_Il2CppGenericInstTable";

		public Il2CppGenericInstWriter(CppCodeWriter writer) : base(writer)
		{
		}

		public TableInfo WriteIl2CppGenericInstDefinitions()
		{
			base.Writer.AddCodeGenIncludes();
			foreach (TypeReference[] current in from item in Il2CppGenericInstWriter.Il2CppGenericInstCollector.Items
			select item.Key)
			{
				for (int i = 0; i < current.Length; i++)
				{
					base.Writer.WriteExternForIl2CppType(current[i]);
				}
				string arg_BD_1 = "static const Il2CppType* {0}[] = {{ {1} }};";
				object[] expr_7A = new object[2];
				expr_7A[0] = MetadataWriter.Naming.ForGenericInst(current) + "_Types";
				expr_7A[1] = current.Select((TypeReference t) => MetadataWriter.TypeRepositoryTypeFor(t, 0)).AggregateWithComma();
				base.WriteLine(arg_BD_1, expr_7A);
				base.WriteLine("extern const Il2CppGenericInst {0} = {{ {1}, {2} }};", new object[]
				{
					MetadataWriter.Naming.ForGenericInst(current),
					current.Length,
					MetadataWriter.Naming.ForGenericInst(current) + "_Types"
				});
			}
			return MetadataWriter.WriteTable<KeyValuePair<TypeReference[], uint>>(base.Writer, "extern const Il2CppGenericInst* const", "g_Il2CppGenericInstTable", (from item in Il2CppGenericInstWriter.Il2CppGenericInstCollector.Items
			orderby item.Value
			select item).ToArray<KeyValuePair<TypeReference[], uint>>(), (KeyValuePair<TypeReference[], uint> item) => "&" + MetadataWriter.Naming.ForGenericInst(item.Key));
		}
	}
}
