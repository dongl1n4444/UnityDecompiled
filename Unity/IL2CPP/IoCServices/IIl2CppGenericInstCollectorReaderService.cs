namespace Unity.IL2CPP.IoCServices
{
    using System.Collections.Generic;

    public interface IIl2CppGenericInstCollectorReaderService
    {
        IDictionary<TypeReference[], uint> Items { get; }
    }
}

