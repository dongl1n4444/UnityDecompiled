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
        private string <CurrentArch>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <TizenSdkRootDir>k__BackingField;
        private string CompilerVersion = "llvm-3.6";
        private const string CurrentSDKVersion = "tizen-2.4.0r1";
        private const string Platform = "tizen-2.4";
        private string Rootstrap = "";

        public TizenSDKUtilities(NPath sdkRootPath, Unity.IL2CPP.Common.Architecture architecture)
        {
            if (!sdkRootPath.Exists(""))
            {
                throw new ArgumentException("Tizen Studio path does not exist: " + sdkRootPath);
            }
            this.TizenSdkRootDir = sdkRootPath;
            if (architecture is ARMv7Architecture)
            {
                this.CurrentArch = "arm";
                this.Rootstrap = "mobile-2.4-device.core";
            }
            else if (architecture is x86Architecture)
            {
                this.CurrentArch = "i386";
                this.Rootstrap = "mobile-2.4-emulator.core";
            }
            else if (architecture is ARM64Architecture)
            {
                this.CurrentArch = "aarch64";
                this.Rootstrap = "mobile-3.0-device64.core";
            }
            else
            {
                if (!(architecture is x64Architecture))
                {
                    throw new NotSupportedException("Unknown architecture: " + architecture);
                }
                this.CurrentArch = "x86_64";
                this.Rootstrap = "mobile-3.0-emulator64.core";
            }
        }

        public string GetCompilerPrefix()
        {
            string[] append = new string[] { "tools", this.CompilerVersion, "bin", "clang" };
            return this.TizenSdkRootDir.Combine(append).ToString();
        }

        public NPath GetLinkerPath()
        {
            string[] append = new string[] { "tools", this.CompilerVersion, "bin", "clang++" };
            return this.TizenSdkRootDir.Combine(append);
        }

        public string GetSysroot()
        {
            string[] append = new string[] { "platforms", "tizen-2.4", "mobile", "rootstraps", this.Rootstrap };
            return this.TizenSdkRootDir.Combine(append).ToString();
        }

        public string GetTargetFlags()
        {
            if (this.CurrentArch == "arm")
            {
                return "-march=armv7-a -mfpu=neon -mfloat-abi=softfp -mtune=cortex-a9";
            }
            if (this.CurrentArch == "i386")
            {
                return "-march=i686 -msse2 -mfpmath=sse";
            }
            return "";
        }

        public string GetToolchainFlags()
        {
            string[] textArray1 = new string[7];
            textArray1[0] = "-target ";
            textArray1[1] = this.CurrentArch;
            textArray1[2] = "-tizen-linux-gnueabi -gcc-toolchain ";
            string[] append = new string[] { "tools", this.CurrentArch + "-linux-gnueabi-gcc-4.9" };
            textArray1[3] = this.TizenSdkRootDir.Combine(append).ToString();
            textArray1[4] = " -ccc-gcc-name ";
            textArray1[5] = this.CurrentArch;
            textArray1[6] = "-linux-gnueabi-g++";
            return string.Concat(textArray1);
        }

        protected string CurrentArch { get; set; }

        public NPath TizenSdkRootDir { get; private set; }
    }
}

