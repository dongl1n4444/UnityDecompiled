namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;

    public class GenericClassWriter : MetadataWriter
    {
        public GenericClassWriter(CppCodeWriter writer) : base(writer)
        {
        }

        public void WriteDefinition(GenericInstanceType type, IMetadataCollection metadataCollection)
        {
            if (GenericsUtilities.CheckForMaximumRecursion(type))
            {
                object[] args = new object[] { MetadataWriter.Naming.ForGenericClass(type) };
                base.Writer.WriteLine("Il2CppGenericClass {0} = {{ -1, {{ NULL, NULL }}, NULL }};", args);
            }
            else
            {
                base.Writer.WriteExternForIl2CppGenericInst(type.GenericArguments);
                object[] objArray2 = new object[] { MetadataWriter.Naming.ForGenericClass(type), metadataCollection.GetTypeInfoIndex(type.Resolve()), MetadataWriter.Naming.ForGenericInst(type.GenericArguments), MetadataWriter.Naming.Null, MetadataWriter.Naming.Null };
                base.WriteLine("Il2CppGenericClass {0} = {{ {1}, {{ &{2}, {3} }}, {4} }};", objArray2);
            }
        }
    }
}

