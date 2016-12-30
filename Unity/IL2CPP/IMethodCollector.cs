namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public interface IMethodCollector
    {
        void AddMethod(MethodReference method);
    }
}

