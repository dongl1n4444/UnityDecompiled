namespace Unity.IL2CPP.Com
{
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public class GuidWriter
    {
        private readonly CppCodeWriter _writer;
        [Inject]
        public static INamingService Naming;

        public GuidWriter(CppCodeWriter writer)
        {
            this._writer = writer;
        }

        public TableInfo WriteGuids(ICollection<TypeDefinition> types) => 
            MetadataWriter.WriteTable<TypeDefinition>(this._writer, "extern const Il2CppGuid*", "g_Guids", types, delegate (TypeDefinition t) {
                this._writer.AddIncludeForTypeDefinition(t);
                return $"&{Naming.ForTypeNameOnly(t)}::IID";
            });
    }
}

