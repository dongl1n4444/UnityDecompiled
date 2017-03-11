namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using Unity.IL2CPP.WindowsRuntime;

    public interface IWindowsRuntimeProjections
    {
        TypeDefinition GetNativeToManagedAdapterClassFor(TypeDefinition interfaceType);
        IProjectedComCallableWrapperMethodWriter GetProjectedComCallableWrapperMethodWriterFor(TypeDefinition type);
        TypeDefinition ProjectToCLR(TypeDefinition type);
        TypeReference ProjectToCLR(TypeReference type);
        TypeDefinition ProjectToWindowsRuntime(TypeDefinition type);
        TypeReference ProjectToWindowsRuntime(TypeReference type);
    }
}

