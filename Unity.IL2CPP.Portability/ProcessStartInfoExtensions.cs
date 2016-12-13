using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.IL2CPP.Portability
{
	public static class ProcessStartInfoExtensions
	{
		public static void SetEnvironmentVariablePortable(this ProcessStartInfo processStartInfo, KeyValuePair<string, string> envVar)
		{
			processStartInfo.SetEnvironmentVariablePortable(envVar.Key, envVar.Value);
		}

		public static void SetEnvironmentVariablePortable(this ProcessStartInfo processStartInfo, string key, string value)
		{
			processStartInfo.EnvironmentVariables[key] = value;
		}
	}
}
