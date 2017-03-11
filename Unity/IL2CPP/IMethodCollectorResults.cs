namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.ObjectModel;

    public interface IMethodCollectorResults
    {
        int GetMethodIndex(MethodReference method);
        ReadOnlyCollection<MethodReference> GetMethods();
    }
}

