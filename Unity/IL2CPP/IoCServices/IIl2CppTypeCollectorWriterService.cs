﻿namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;

    public interface IIl2CppTypeCollectorWriterService
    {
        void Add(TypeReference type, int attrs = 0);
        int GetIndex(TypeReference type, int attrs = 0);
        int GetOrCreateIndex(TypeReference type, int attrs = 0);
    }
}

