using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class ParameterInfoExtensions
	{
		public static bool GetIsLcidPortable(this ParameterInfo parameterInfo)
		{
			return parameterInfo.IsLcid;
		}

		public static object GetRawDefaultValuePortable(this ParameterInfo parameterInfo)
		{
			return parameterInfo.RawDefaultValue;
		}

		public static int GetMetadataTokenPortable(this ParameterInfo parameterInfo)
		{
			return parameterInfo.MetadataToken;
		}
	}
}
