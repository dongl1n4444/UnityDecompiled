namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;

    public class TizenSDKUtilities
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static NPath <SDKDirectory>k__BackingField;
        private const string CompilerVersion = "arm-linux-gnueabi-gcc-4.6";
        private const string CurrentSDKVersion = "tizen-2.4.0";
        private const string Platform = "tizen-2.4";
        private const string Rootstrap = "mobile-2.4-device.core";

        static TizenSDKUtilities()
        {
            if (PlatformUtils.IsLinux())
            {
                SDKDirectory = new NPath(NPath.HomeDirectory + "/tizen-2.4.0-linux");
            }
            else
            {
                if (!PlatformUtils.IsOSX())
                {
                    throw new NotSupportedException("Building Tizen on Windows is not supported.");
                }
                SDKDirectory = new NPath(NPath.HomeDirectory + "/tizen-2.4.0-macosx");
            }
        }

        public static string GetCompilerPrefix()
        {
            string[] append = new string[] { "tools", "arm-linux-gnueabi-gcc-4.6", "bin", "arm-linux-gnueabi-" };
            return SDKDirectory.Combine(append).ToString();
        }

        public static NPath GetLinkerPath()
        {
            string[] append = new string[] { "tools", "arm-linux-gnueabi-gcc-4.6", "bin", "arm-linux-gnueabi-g++" };
            return SDKDirectory.Combine(append);
        }

        public static string GetSysroot()
        {
            string[] append = new string[] { "platforms", "tizen-2.4", "mobile", "rootstraps", "mobile-2.4-device.core" };
            return SDKDirectory.Combine(append).ToString();
        }

        protected static NPath SDKDirectory
        {
            [CompilerGenerated]
            get => 
                <SDKDirectory>k__BackingField;
            [CompilerGenerated]
            set
            {
                <SDKDirectory>k__BackingField = value;
            }
        }
    }
}

