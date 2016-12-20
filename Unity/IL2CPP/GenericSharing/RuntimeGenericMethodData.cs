namespace Unity.IL2CPP.GenericSharing
{
    using Mono.Cecil;
    using System;

    public class RuntimeGenericMethodData : RuntimeGenericData
    {
        public MethodReference Data;
        public MethodReference GenericMethod;

        public RuntimeGenericMethodData(RuntimeGenericContextInfo infoType, MethodReference data, MethodReference genericMethod) : base(infoType)
        {
            this.Data = data;
            this.GenericMethod = genericMethod;
        }
    }
}

