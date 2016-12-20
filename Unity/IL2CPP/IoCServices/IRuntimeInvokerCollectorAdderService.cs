namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;

    public interface IRuntimeInvokerCollectorAdderService
    {
        string Add(MethodReference method);
    }
}

