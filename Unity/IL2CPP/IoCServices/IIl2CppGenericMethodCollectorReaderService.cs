namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public interface IIl2CppGenericMethodCollectorReaderService
    {
        uint GetIndex(MethodReference method);
        bool HasIndex(MethodReference method);

        IDictionary<MethodReference, uint> Items { get; }
    }
}

