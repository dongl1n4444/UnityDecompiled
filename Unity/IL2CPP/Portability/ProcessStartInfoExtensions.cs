namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class ProcessStartInfoExtensions
    {
        [Extension]
        public static void SetEnvironmentVariablePortable(ProcessStartInfo processStartInfo, KeyValuePair<string, string> envVar)
        {
            SetEnvironmentVariablePortable(processStartInfo, envVar.Key, envVar.Value);
        }

        [Extension]
        public static void SetEnvironmentVariablePortable(ProcessStartInfo processStartInfo, string key, string value)
        {
            processStartInfo.EnvironmentVariables[key] = value;
        }
    }
}

