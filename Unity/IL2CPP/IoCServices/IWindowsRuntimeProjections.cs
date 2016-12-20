namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;

    public interface IWindowsRuntimeProjections
    {
        TypeReference ProjectToCLR(TypeReference type);
        TypeReference ProjectToWindowsRuntime(TypeReference type);
    }
}

