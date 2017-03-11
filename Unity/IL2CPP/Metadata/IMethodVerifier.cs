namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;

    public interface IMethodVerifier
    {
        bool MethodExists(MethodReference method);
    }
}

