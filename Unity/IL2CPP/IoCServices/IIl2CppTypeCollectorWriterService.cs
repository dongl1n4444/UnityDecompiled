namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;

    public interface IIl2CppTypeCollectorWriterService
    {
        void Add(TypeReference type, [Optional, DefaultParameterValue(0)] int attrs);
        int GetIndex(TypeReference type, [Optional, DefaultParameterValue(0)] int attrs);
        int GetOrCreateIndex(TypeReference type, [Optional, DefaultParameterValue(0)] int attrs);
    }
}

