namespace Unity.IL2CPP.FileNaming
{
    using Mono.Cecil;
    using System;

    public interface IFileNameProvider
    {
        string ForMethodDeclarations(TypeReference type);
        string ForModule(ModuleDefinition module);
        string ForTypeDefinition(TypeReference type);
    }
}

