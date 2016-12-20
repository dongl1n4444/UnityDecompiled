namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;

    public interface IRuntimeInvokerCollectorReaderService
    {
        int GetIndex(MethodReference method);
    }
}

