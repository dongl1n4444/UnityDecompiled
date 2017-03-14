namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;

    public interface IRuntimeImplementedMethodAdder
    {
        void RegisterMethod(MethodDefinition method, GetGenericSharingDataDelegate getGenericSharingData, WriteRuntimeImplementedMethodBodyDelegate writerMethodBody);
    }
}

