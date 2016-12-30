namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.Common;

    public interface IWindowsRuntimeProjectionsInitializer
    {
        void Initialize(ModuleDefinition mscorlib, DotNetProfile dotNetProfile);

        bool HasIEnumerableCCW { set; }
    }
}

