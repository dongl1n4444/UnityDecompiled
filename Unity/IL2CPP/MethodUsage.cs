namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public class MethodUsage
    {
        private readonly HashSet<MethodReference> _methods = new HashSet<MethodReference>(new MethodReferenceComparer());

        public void AddMethod(MethodReference method)
        {
            this._methods.Add(method);
        }

        public IEnumerable<MethodReference> GetMethods() => 
            this._methods;
    }
}

