namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;

    public interface IWindowsRuntimeProjections
    {
        IEnumerable<KeyValuePair<TypeDefinition, WindowsRuntimeProjectedCCWWriter>> GetAllNonGenericCCWMethodDefinitionsWriters();
        WindowsRuntimeProjectedCCWWriter GetCCWWriter(TypeDefinition type, bool typeDefinition);
        WindowsRuntimeProjectedMethodBodyWriter GetMethodBodyWriter(MethodDefinition method);
        IEnumerable<TypeDefinition> GetSupportedProjectedInterfacesCLR();
        bool IsSupportedProjectedInterfaceWindowsRuntime(TypeReference type);
        TypeDefinition ProjectToCLR(TypeDefinition type);
        TypeReference ProjectToCLR(TypeReference type);
        TypeDefinition ProjectToWindowsRuntime(TypeDefinition type);
        TypeReference ProjectToWindowsRuntime(TypeReference type);

        bool HasIEnumerableCCW { get; }
    }
}

