namespace Unity.IL2CPP.Portability
{
    using System;

    public static class PortabilityUtilities
    {
        public static bool IsWindows()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                    return true;
            }
            return false;
        }
    }
}

