namespace Unity.IL2CPP.GenericSharing
{
    using Mono.Cecil;
    using System;

    public class RuntimeGenericTypeData : RuntimeGenericData
    {
        public readonly TypeReference GenericType;

        public RuntimeGenericTypeData(RuntimeGenericContextInfo infoType, TypeReference genericType) : base(infoType)
        {
            this.GenericType = genericType;
        }
    }
}

