namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;

    public class NetCoreInstall
    {
        private static readonly NetCoreSdkInstall _currentPlatform;
        private const string NetCoreVersion = "1.0.0-preview3-004056";
        public const string TopLevelDirectoryName = "NetCore";

        static NetCoreInstall()
        {
            if (PlatformUtils.IsWindows())
            {
                _currentPlatform = NetCoreSdkInstall.CreateWindows();
            }
            else if (PlatformUtils.IsOSX())
            {
                _currentPlatform = NetCoreSdkInstall.CreateOSX();
            }
            else
            {
                _currentPlatform = NetCoreSdkInstall.CreateLinux();
            }
        }

        public static bool Available =>
            Root.Exists("");

        public static NPath DotNetExe =>
            _currentPlatform.DotNetExe;

        public static NPath Root
        {
            get
            {
                if (UnitySourceCode.Available)
                {
                    string[] textArray1 = new string[] { "External", "NetCore", "builds" };
                    return UnitySourceCode.Paths.UnityRoot.Combine(textArray1);
                }
                string[] append = new string[] { "NetCore" };
                return Il2CppDependencies.Root.Combine(append);
            }
        }

        public static NPath RunCsc =>
            _currentPlatform.RunCsc;

        public static NPath SdkRoot =>
            _currentPlatform.SdkRoot;

        public class NetCoreSdkInstall
        {
            private readonly string _arch;
            private readonly bool _isWindowsPlatform;
            private readonly string _platformName;

            public NetCoreSdkInstall(string platformName, string arch) : this(platformName, arch, platformName == "win")
            {
            }

            private NetCoreSdkInstall(string platformName, string arch, bool isWindowsPlatform)
            {
                this._platformName = platformName;
                this._arch = arch;
                this._isWindowsPlatform = isWindowsPlatform;
            }

            public static NetCoreInstall.NetCoreSdkInstall CreateLinux() => 
                new NetCoreInstall.NetCoreSdkInstall("ubuntu", "x64");

            public static NetCoreInstall.NetCoreSdkInstall CreateOSX() => 
                new NetCoreInstall.NetCoreSdkInstall("osx", "x64");

            public static NetCoreInstall.NetCoreSdkInstall CreateWindows() => 
                new NetCoreInstall.NetCoreSdkInstall("win", "x64");

            public NPath DotNetExe
            {
                get
                {
                    if (this._isWindowsPlatform)
                    {
                        string[] textArray1 = new string[] { "dotnet.exe" };
                        return this.SdkRoot.Combine(textArray1).FileMustExist();
                    }
                    string[] append = new string[] { "dotnet" };
                    return this.SdkRoot.Combine(append).FileMustExist();
                }
            }

            public string PlatformName =>
                this._platformName;

            public NPath RunCsc
            {
                get
                {
                    string[] append = new string[] { "sdk", "1.0.0-preview3-004056", !this._isWindowsPlatform ? "RunCsc.sh" : "RunCsc.cmd" };
                    return this.SdkRoot.Combine(append).FileMustExist();
                }
            }

            public NPath SdkRoot =>
                this.SdkRootNoAssert.DirectoryMustExist();

            public NPath SdkRootNoAssert
            {
                get
                {
                    string[] append = new string[] { $"dotnet-dev-{this.PlatformName}-{this._arch}.{"1.0.0-preview3-004056"}" };
                    return NetCoreInstall.Root.Combine(append);
                }
            }
        }
    }
}

