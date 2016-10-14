using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Com
{
	public class GuidWriter
	{
		[Inject]
		public static INamingService Naming;

		private readonly CppCodeWriter _writer;

		public GuidWriter(CppCodeWriter writer)
		{
			this._writer = writer;
		}

		public TableInfo WriteGuids(ICollection<TypeDefinition> types)
		{
			return MetadataWriter.WriteTable<TypeDefinition>(this._writer, "extern const Il2CppGuid*", "g_Guids", types, delegate(TypeDefinition t)
			{
				this._writer.AddIncludeForTypeDefinition(t);
				return string.Format("&{0}::IID", GuidWriter.Naming.ForTypeNameOnly(t));
			});
		}
	}
}
