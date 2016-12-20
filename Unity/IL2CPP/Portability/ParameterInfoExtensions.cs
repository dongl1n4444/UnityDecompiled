namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class ParameterInfoExtensions
    {
        [Extension]
        public static bool GetIsLcidPortable(ParameterInfo parameterInfo)
        {
            return parameterInfo.IsLcid;
        }

        [Extension]
        public static int GetMetadataTokenPortable(ParameterInfo parameterInfo)
        {
            return parameterInfo.MetadataToken;
        }

        [Extension]
        public static object GetRawDefaultValuePortable(ParameterInfo parameterInfo)
        {
            return parameterInfo.RawDefaultValue;
        }
    }
}

