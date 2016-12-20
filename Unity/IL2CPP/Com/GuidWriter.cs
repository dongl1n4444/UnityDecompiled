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

        public TableInfo WriteGuids(ICollection<TypeDefinition> types)
        {
            return MetadataWriter.WriteTable<TypeDefinition>(this._writer, "extern const Il2CppGuid*", "g_Guids", types, new Func<TypeDefinition, string>(this, (IntPtr) this.<WriteGuids>m__0));
        }
    }
}

