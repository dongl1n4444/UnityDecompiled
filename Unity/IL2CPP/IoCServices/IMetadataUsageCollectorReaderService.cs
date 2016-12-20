namespace Unity.IL2CPP.IoCServices
{
    using System;
    using System.Collections.Generic;

    public interface IMetadataUsageCollectorReaderService
    {
        IEnumerable<FieldReference> GetFieldInfos();
        IEnumerable<TypeReference> GetIl2CppTypes();
        IEnumerable<MethodReference> GetInflatedMethods();
        IEnumerable<string> GetStringLiterals();
        IEnumerable<TypeReference> GetTypeInfos();
        IEnumerable<KeyValuePair<string, MetadataUsage>> GetUsages();

        int UsageCount { get; }
    }
}

