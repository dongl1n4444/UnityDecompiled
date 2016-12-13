using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class Il2CppGenericMethodWriter : MetadataWriter
	{
		[Inject]
		public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;

		[Inject]
		public static IIl2CppGenericInstCollectorReaderService IIl2CppGenericInstCollector;

		public const string TableSizeVariableName = "g_Il2CppMethodSpecTableSize";

		public const string TableVariableName = "g_Il2CppMethodSpecTable";

		public Il2CppGenericMethodWriter(CppCodeWriter writer) : base(writer)
		{
		}

		public TableInfo WriteIl2CppGenericMethodDefinitions(IMetadataCollection metadataCollection)
		{
			base.Writer.AddCodeGenIncludes();
			return MetadataWriter.WriteTable<KeyValuePair<MethodReference, uint>>(base.Writer, "extern const Il2CppMethodSpec", "g_Il2CppMethodSpecTable", (from item in Il2CppGenericMethodWriter.Il2CppGenericMethodCollector.Items
			orderby item.Value
			select item).ToArray<KeyValuePair<MethodReference, uint>>(), (KeyValuePair<MethodReference, uint> item, int index) => this.FormatGenericMethod(item.Key, metadataCollection));
		}

		private string FormatGenericMethod(MethodReference method, IMetadataCollection metadataCollection)
		{
			return string.Format("{{ {0}, {1}, {2} }}", metadataCollection.GetMethodIndex(method.Resolve()), (int)((!method.DeclaringType.IsGenericInstance) ? 4294967295u : Il2CppGenericMethodWriter.IIl2CppGenericInstCollector.Items[((GenericInstanceType)method.DeclaringType).GenericArguments.ToArray()]), (int)((!method.IsGenericInstance) ? 4294967295u : Il2CppGenericMethodWriter.IIl2CppGenericInstCollector.Items[((GenericInstanceMethod)method).GenericArguments.ToArray()]));
		}
	}
}
