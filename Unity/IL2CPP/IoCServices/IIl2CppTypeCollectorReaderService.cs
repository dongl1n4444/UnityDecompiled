namespace Unity.IL2CPP.IoCServices
{
    using System.Collections.Generic;

    public interface IIl2CppTypeCollectorReaderService
    {
        IDictionary<Il2CppTypeData, int> Items { get; }
    }
}

