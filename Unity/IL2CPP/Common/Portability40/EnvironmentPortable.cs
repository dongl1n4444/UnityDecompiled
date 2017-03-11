namespace Unity.IL2CPP.Common.Portability40
{
    using System;

    public static class EnvironmentPortable
    {
        public static bool Is64BitOperatingSystemPortable =>
            Environment.Is64BitOperatingSystem;
    }
}

