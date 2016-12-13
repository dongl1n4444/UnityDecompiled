using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Metadata
{
	public class GenericClassWriter : MetadataWriter
	{
		public GenericClassWriter(CppCodeWriter writer) : base(writer)
		{
		}

		public void WriteDefinition(GenericInstanceType type, IMetadataCollection metadataCollection)
		{
			if (GenericsUtilities.CheckForMaximumRecursion(type))
			{
				base.Writer.WriteLine("Il2CppGenericClass {0} = {{ -1, {{ NULL, NULL }}, NULL }};", new object[]
				{
					MetadataWriter.Naming.ForGenericClass(type)
				});
			}
			else
			{
				base.Writer.WriteExternForIl2CppGenericInst(type.GenericArguments);
				base.WriteLine("Il2CppGenericClass {0} = {{ {1}, {{ &{2}, {3} }}, {4} }};", new object[]
				{
					MetadataWriter.Naming.ForGenericClass(type),
					metadataCollection.GetTypeInfoIndex(type.Resolve()),
					MetadataWriter.Naming.ForGenericInst(type.GenericArguments),
					MetadataWriter.Naming.Null,
					MetadataWriter.Naming.Null
				});
			}
		}
	}
}
