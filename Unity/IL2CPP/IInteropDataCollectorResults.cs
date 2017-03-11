namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.ObjectModel;

    public interface IInteropDataCollectorResults
    {
        ReadOnlyCollection<InteropData> GetInteropData();
        int GetReversePInvokeWrapperIndex(MethodReference method);
        ReadOnlyCollection<MethodReference> GetReversePInvokeWrappers();
        ReadOnlyCollection<Tuple<TypeReference, string>> GetWindowsRuntimeTypesWithNames();
    }
}

