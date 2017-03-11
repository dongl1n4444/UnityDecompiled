namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains.MsvcVersions.VisualStudioAPI;

    internal class Msvc15Installation : MsvcInstallation
    {
        private static readonly string _hostDirectory;
        private static readonly string _hostDirectoryNativeFolder;
        private readonly NPath _netfxsdkDir;
        private readonly List<NPath> _sdkBinDirectories;
        private readonly NPath _sdkUnionMetadataDirectory;
        private readonly string _sdkVersion;
        private static Dictionary<Type, VCPaths> _vcPaths;
        [CompilerGenerated]
        private static Func<NPath, NPath> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<NPath, NPath> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<NPath, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<ISetupPackageReference, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<ISetupPackageReference, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<ISetupPackageReference, bool> <>f__am$cache5;

        static Msvc15Installation()
        {
            Architecture bestThisMachineCanRun = Architecture.BestThisMachineCanRun;
            if (bestThisMachineCanRun is x64Architecture)
            {
                _hostDirectory = "HostX64";
                _hostDirectoryNativeFolder = "x64";
            }
            else
            {
                if (!(bestThisMachineCanRun is x86Architecture))
                {
                    throw new NotSupportedException($"Unknown host architecture: {bestThisMachineCanRun}");
                }
                _hostDirectory = "HostX86";
                _hostDirectoryNativeFolder = "x86";
            }
            _vcPaths = GetVCToolsPaths();
        }

        public Msvc15Installation() : base(new Version(15, 0))
        {
            this._sdkBinDirectories = new List<NPath>();
            base.SDKDirectory = WindowsSDKs.GetWindows10SDKDirectory(out this._sdkVersion);
            this._netfxsdkDir = WindowsSDKs.GetDotNetFrameworkSDKDirectory();
            if (base.SDKDirectory != null)
            {
                string[] append = new string[] { "bin" };
                NPath item = base.SDKDirectory.Combine(append);
                string[] textArray2 = new string[] { this._sdkVersion };
                NPath path2 = item.Combine(textArray2);
                if (path2.DirectoryExists(""))
                {
                    this._sdkBinDirectories.Add(path2);
                }
                this._sdkBinDirectories.Add(item);
                string[] textArray3 = new string[] { "UnionMetadata" };
                NPath path3 = base.SDKDirectory.Combine(textArray3);
                string[] textArray4 = new string[] { this._sdkVersion };
                NPath path4 = path3.Combine(textArray4);
                this._sdkUnionMetadataDirectory = !path4.DirectoryExists("") ? path3 : path4;
            }
        }

        private static void AddVCToolsForArchitecture(Architecture architecture, VCComponent component, string architectureFolder, Dictionary<Type, VCPaths> vcToolsPaths)
        {
            if (component != null)
            {
                string[] append = new string[] { "VC", "Tools", "MSVC", component.RawVersion };
                NPath path = component.InstallationPath.Combine(append);
                if (!path.DirectoryExists(""))
                {
                    string str = component.RawVersion.Substring(0, component.RawVersion.LastIndexOf('.'));
                    string[] textArray2 = new string[] { "VC", "Tools", "MSVC", str };
                    path = component.InstallationPath.Combine(textArray2);
                }
                string[] textArray3 = new string[] { "bin", _hostDirectory, architectureFolder };
                NPath toolsPath = path.Combine(textArray3);
                string[] textArray4 = new string[] { "include" };
                NPath includePath = path.Combine(textArray4);
                string[] textArray5 = new string[] { "lib", architectureFolder };
                NPath libPath = path.Combine(textArray5);
                if ((toolsPath.DirectoryExists("") && includePath.DirectoryExists("")) && libPath.DirectoryExists(""))
                {
                    vcToolsPaths.Add(architecture.GetType(), new VCPaths(toolsPath, includePath, libPath));
                }
            }
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> GetIncludeDirectories(Architecture architecture) => 
            new <GetIncludeDirectories>c__Iterator2 { 
                architecture = architecture,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<NPath> GetLibDirectories(Architecture architecture, string sdkSubset = null) => 
            new <GetLibDirectories>c__Iterator3 { 
                architecture = architecture,
                sdkSubset = sdkSubset,
                $this = this,
                $PC = -2
            };

        private static bool GetNextVSInstance(IEnumSetupInstances enumerator, out object vsInstance)
        {
            int pceltFetched = 0;
            object[] rgelt = new object[1];
            try
            {
                enumerator.Next(rgelt.Length, rgelt, out pceltFetched);
            }
            catch
            {
            }
            if (pceltFetched > 0)
            {
                vsInstance = rgelt[0];
                return true;
            }
            vsInstance = null;
            return false;
        }

        public override string GetPathEnvVariable(Architecture architecture)
        {
            ThrowIfArchitectureNotInstalled(architecture);
            List<NPath> source = new List<NPath>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = d => d.Combine(new string[] { "x64" });
            }
            source.AddRange(this._sdkBinDirectories.Select<NPath, NPath>(<>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = d => d.Combine(new string[] { "x86" });
            }
            source.AddRange(this._sdkBinDirectories.Select<NPath, NPath>(<>f__am$cache1));
            string[] append = new string[] { _hostDirectoryNativeFolder };
            source.Add(_vcPaths[architecture.GetType()].ToolsPath.Parent.Combine(append));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = p => p.ToString();
            }
            return source.Select<NPath, string>(<>f__am$cache2).AggregateWith(";");
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> GetPlatformMetadataReferences() => 
            new <GetPlatformMetadataReferences>c__Iterator0 { $PC = -2 };

        protected override IEnumerable<NPath> GetSDKBinDirectories() => 
            this._sdkBinDirectories;

        public override NPath GetUnionMetadataDirectory() => 
            this._sdkUnionMetadataDirectory;

        private static Dictionary<Type, VCPaths> GetVCToolsPaths()
        {
            Dictionary<Type, VCPaths> vcToolsPaths = new Dictionary<Type, VCPaths>();
            IEnumSetupInstances vSInstancesEnumerator = GetVSInstancesEnumerator();
            if (vSInstancesEnumerator != null)
            {
                object obj2;
                List<VCComponent> source = new List<VCComponent>();
                List<VCComponent> list2 = new List<VCComponent>();
                List<VCComponent> list3 = new List<VCComponent>();
                while (GetNextVSInstance(vSInstancesEnumerator, out obj2))
                {
                    <GetVCToolsPaths>c__AnonStorey4 storey = new <GetVCToolsPaths>c__AnonStorey4 {
                        instance2 = obj2 as ISetupInstance2
                    };
                    if (storey.instance2 != null)
                    {
                        try
                        {
                            if ((storey.instance2.GetState() & InstanceState.Local) == InstanceState.None)
                            {
                                continue;
                            }
                            ISetupPackageReference[] packages = storey.instance2.GetPackages();
                            if (<>f__am$cache3 == null)
                            {
                                <>f__am$cache3 = p => p.GetId() == $"Microsoft.VisualCpp.Tools.{_hostDirectory}.TargetX86";
                            }
                            source.AddRange(packages.Where<ISetupPackageReference>(<>f__am$cache3).Select<ISetupPackageReference, VCComponent>(new Func<ISetupPackageReference, VCComponent>(storey.<>m__0)));
                            if (<>f__am$cache4 == null)
                            {
                                <>f__am$cache4 = p => p.GetId() == $"Microsoft.VisualCpp.Tools.{_hostDirectory}.TargetX64";
                            }
                            list2.AddRange(packages.Where<ISetupPackageReference>(<>f__am$cache4).Select<ISetupPackageReference, VCComponent>(new Func<ISetupPackageReference, VCComponent>(storey.<>m__1)));
                            if (<>f__am$cache5 == null)
                            {
                                <>f__am$cache5 = p => p.GetId() == $"Microsoft.VisualCpp.Tools.{_hostDirectory}.TargetARM";
                            }
                            list3.AddRange(packages.Where<ISetupPackageReference>(<>f__am$cache5).Select<ISetupPackageReference, VCComponent>(new Func<ISetupPackageReference, VCComponent>(storey.<>m__2)));
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"Unexpected exception when trying to find Visual C++ directories: {exception}");
                        }
                    }
                }
                VCComponentVersionComparer comparer = new VCComponentVersionComparer();
                source.Sort(comparer);
                list2.Sort(comparer);
                list3.Sort(comparer);
                AddVCToolsForArchitecture(new x86Architecture(), source.LastOrDefault<VCComponent>(), "x86", vcToolsPaths);
                AddVCToolsForArchitecture(new x64Architecture(), list2.LastOrDefault<VCComponent>(), "x64", vcToolsPaths);
                AddVCToolsForArchitecture(new ARMv7Architecture(), list3.LastOrDefault<VCComponent>(), "ARM", vcToolsPaths);
            }
            return vcToolsPaths;
        }

        private static IEnumSetupInstances GetVSInstancesEnumerator()
        {
            SetupConfiguration configuration = null;
            try
            {
                configuration = new SetupConfiguration();
            }
            catch
            {
                return null;
            }
            try
            {
                return configuration.EnumInstances();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error in Msvc15Installation: failed to enumerate VS instances: {exception}");
                return null;
            }
        }

        public override NPath GetVSToolPath(Architecture architecture, string toolName)
        {
            ThrowIfArchitectureNotInstalled(architecture);
            string[] append = new string[] { toolName };
            return _vcPaths[architecture.GetType()].ToolsPath.Combine(append);
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> GetWindowsMetadataReferences() => 
            new <GetWindowsMetadataReferences>c__Iterator1 { 
                $this = this,
                $PC = -2
            };

        public override bool HasMetadataDirectories() => 
            true;

        private static void ThrowIfArchitectureNotInstalled(Architecture architecture)
        {
            if (!_vcPaths.ContainsKey(architecture.GetType()))
            {
                throw new NotSupportedException($"Visual Studio 2017 support for {architecture.Name} is not installed.");
            }
        }

        public static bool IsInstalled =>
            (_vcPaths.Count > 0);

        public override IEnumerable<Type> SupportedArchitectures =>
            _vcPaths.Keys;

        protected override NPath VisualStudioDirectory
        {
            get
            {
                throw new NotSupportedException("Msvc15Installation does not support VisualStudioDirectory property!");
            }
            set
            {
            }
        }

        [CompilerGenerated]
        private sealed class <GetIncludeDirectories>c__Iterator2 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc15Installation $this;
            internal NPath <includeDirectory>__0;
            internal Architecture architecture;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        Msvc15Installation.ThrowIfArchitectureNotInstalled(this.architecture);
                        this.$current = Msvc15Installation._vcPaths[this.architecture.GetType()].IncludePath;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0183;

                    case 1:
                    {
                        string[] append = new string[] { "Include" };
                        string[] textArray2 = new string[] { this.$this._sdkVersion };
                        this.<includeDirectory>__0 = this.$this.SDKDirectory.Combine(append).Combine(textArray2);
                        string[] textArray3 = new string[] { "shared" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray3);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0183;
                    }
                    case 2:
                    {
                        string[] textArray4 = new string[] { "um" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray4);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0183;
                    }
                    case 3:
                    {
                        string[] textArray5 = new string[] { "winrt" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray5);
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0183;
                    }
                    case 4:
                    {
                        string[] textArray6 = new string[] { "ucrt" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray6);
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_0183;
                    }
                    case 5:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0183:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Msvc15Installation.<GetIncludeDirectories>c__Iterator2 { 
                    $this = this.$this,
                    architecture = this.architecture
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <GetLibDirectories>c__Iterator3 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc15Installation $this;
            internal NPath <libDirectory>__0;
            internal NPath <vcLibPath>__0;
            internal Architecture architecture;
            internal string sdkSubset;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                    {
                        Msvc15Installation.ThrowIfArchitectureNotInstalled(this.architecture);
                        string[] append = new string[] { "Lib" };
                        string[] textArray2 = new string[] { this.$this._sdkVersion };
                        this.<libDirectory>__0 = this.$this.SDKDirectory.Combine(append).Combine(textArray2);
                        if (this.architecture is x86Architecture)
                        {
                            string[] textArray3 = new string[] { "um", "x86" };
                            this.$current = this.<libDirectory>__0.Combine(textArray3);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                        }
                        else if (this.architecture is x64Architecture)
                        {
                            string[] textArray6 = new string[] { "um", "x64" };
                            this.$current = this.<libDirectory>__0.Combine(textArray6);
                            if (!this.$disposing)
                            {
                                this.$PC = 4;
                            }
                        }
                        else
                        {
                            if (!(this.architecture is ARMv7Architecture))
                            {
                                throw new NotSupportedException($"Architecture {this.architecture} is not supported by MsvcToolChain!");
                            }
                            string[] textArray9 = new string[] { "um", "arm" };
                            this.$current = this.<libDirectory>__0.Combine(textArray9);
                            if (!this.$disposing)
                            {
                                this.$PC = 7;
                            }
                        }
                        goto Label_03DB;
                    }
                    case 1:
                    {
                        string[] textArray4 = new string[] { "ucrt", "x86" };
                        this.$current = this.<libDirectory>__0.Combine(textArray4);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_03DB;
                    }
                    case 2:
                    {
                        if (this.$this._netfxsdkDir == null)
                        {
                            break;
                        }
                        string[] textArray5 = new string[] { "lib", "um", "x86" };
                        this.$current = this.$this._netfxsdkDir.Combine(textArray5);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_03DB;
                    }
                    case 3:
                    case 6:
                    case 9:
                        break;

                    case 4:
                    {
                        string[] textArray7 = new string[] { "ucrt", "x64" };
                        this.$current = this.<libDirectory>__0.Combine(textArray7);
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_03DB;
                    }
                    case 5:
                    {
                        if (this.$this._netfxsdkDir == null)
                        {
                            break;
                        }
                        string[] textArray8 = new string[] { "lib", "um", "x64" };
                        this.$current = this.$this._netfxsdkDir.Combine(textArray8);
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_03DB;
                    }
                    case 7:
                    {
                        string[] textArray10 = new string[] { "ucrt", "arm" };
                        this.$current = this.<libDirectory>__0.Combine(textArray10);
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_03DB;
                    }
                    case 8:
                    {
                        if (this.$this._netfxsdkDir == null)
                        {
                            break;
                        }
                        string[] textArray11 = new string[] { "lib", "um", "arm" };
                        this.$current = this.$this._netfxsdkDir.Combine(textArray11);
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_03DB;
                    }
                    case 10:
                        this.$PC = -1;
                        goto Label_03D9;

                    default:
                        goto Label_03D9;
                }
                this.<vcLibPath>__0 = Msvc15Installation._vcPaths[this.architecture.GetType()].LibPath;
                this.$current = (this.sdkSubset == null) ? this.<vcLibPath>__0 : this.<vcLibPath>__0.Combine(new string[] { this.sdkSubset });
                if (!this.$disposing)
                {
                    this.$PC = 10;
                }
                goto Label_03DB;
            Label_03D9:
                return false;
            Label_03DB:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Msvc15Installation.<GetLibDirectories>c__Iterator3 { 
                    $this = this.$this,
                    architecture = this.architecture,
                    sdkSubset = this.sdkSubset
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <GetPlatformMetadataReferences>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                    {
                        Msvc15Installation.ThrowIfArchitectureNotInstalled(new x86Architecture());
                        string[] append = new string[] { "store", "references", "platform.winmd" };
                        this.$current = Msvc15Installation._vcPaths[typeof(x86Architecture)].LibPath.Combine(append);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Msvc15Installation.<GetPlatformMetadataReferences>c__Iterator0();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <GetVCToolsPaths>c__AnonStorey4
        {
            internal ISetupInstance2 instance2;

            internal Msvc15Installation.VCComponent <>m__0(ISetupPackageReference p) => 
                new Msvc15Installation.VCComponent(p, this.instance2);

            internal Msvc15Installation.VCComponent <>m__1(ISetupPackageReference p) => 
                new Msvc15Installation.VCComponent(p, this.instance2);

            internal Msvc15Installation.VCComponent <>m__2(ISetupPackageReference p) => 
                new Msvc15Installation.VCComponent(p, this.instance2);
        }

        [CompilerGenerated]
        private sealed class <GetWindowsMetadataReferences>c__Iterator1 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc15Installation $this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                    {
                        string[] append = new string[] { "windows.winmd" };
                        this.$current = this.$this.GetUnionMetadataDirectory().Combine(append);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Msvc15Installation.<GetWindowsMetadataReferences>c__Iterator1 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        private class VCComponent
        {
            public readonly NPath InstallationPath;
            public readonly string RawVersion;
            public readonly System.Version Version;

            public VCComponent(ISetupPackageReference package, ISetupInstance2 setupInstance)
            {
                this.InstallationPath = setupInstance.GetInstallationPath().ToNPath();
                this.RawVersion = package.GetVersion();
                this.Version = null;
                try
                {
                    this.Version = new System.Version(package.GetVersion());
                }
                catch
                {
                }
            }
        }

        private class VCComponentVersionComparer : IComparer<Msvc15Installation.VCComponent>
        {
            public int Compare(Msvc15Installation.VCComponent x, Msvc15Installation.VCComponent y)
            {
                if (x.Version == y.Version)
                {
                    return 0;
                }
                if (x.Version == null)
                {
                    return -1;
                }
                if (y.Version == null)
                {
                    return 1;
                }
                return ((x.Version >= y.Version) ? 1 : -1);
            }
        }

        private class VCPaths
        {
            public readonly NPath IncludePath;
            public readonly NPath LibPath;
            public readonly NPath ToolsPath;

            public VCPaths(NPath toolsPath, NPath includePath, NPath libPath)
            {
                this.ToolsPath = toolsPath;
                this.IncludePath = includePath;
                this.LibPath = libPath;
            }
        }
    }
}

