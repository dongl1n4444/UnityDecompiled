namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ParameterInfoExtensions
    {
        public static bool GetIsLcidPortable(this ParameterInfo parameterInfo) => 
            parameterInfo.IsLcid;

        public static int GetMetadataTokenPortable(this ParameterInfo parameterInfo) => 
            parameterInfo.MetadataToken;

        public static object GetRawDefaultValuePortable(this ParameterInfo parameterInfo) => 
            parameterInfo.RawDefaultValue;
    }
}

