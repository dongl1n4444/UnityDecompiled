namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

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

