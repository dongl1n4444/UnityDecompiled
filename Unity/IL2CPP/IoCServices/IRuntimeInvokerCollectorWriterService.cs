namespace Unity.IL2CPP.IoCServices
{
    using NiceIO;
    using Unity.IL2CPP.Metadata;

    public interface IRuntimeInvokerCollectorWriterService
    {
        TableInfo Write(NPath path);
    }
}

