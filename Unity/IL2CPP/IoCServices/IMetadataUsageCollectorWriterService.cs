namespace Unity.IL2CPP.IoCServices
{
    using System;
    using Unity.IL2CPP;

    public interface IMetadataUsageCollectorWriterService
    {
        void Add(string identifier, MetadataUsage usage);
    }
}

