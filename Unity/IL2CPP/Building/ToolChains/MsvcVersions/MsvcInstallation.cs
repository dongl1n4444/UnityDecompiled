namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions
{
    using Microsoft.Win32;
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
    using Unity.IL2CPP.Common;

    public abstract class MsvcInstallation
    {
        private static Dictionary<System.Version, MsvcInstallation> _installations = new Dictionary<System.Version, MsvcInstallation>();
        [CompilerGenerated]
        private static Func<NPath, NPath> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<NPath, NPath> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<NPath, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<System.Version, int> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<System.Version, int> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<System.Version, int> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<System.Version, int> <>f__am$cache6;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <SDKDirectory>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private System.Version <Version>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <VisualStudioDirectory>k__BackingField;

        static MsvcInstallation()
        {
            System.Version version = new System.Version(10, 0);
            System.Version version2 = new System.Version(12, 0);
            System.Version version3 = new System.Version(14, 0);
            System.Version key = new System.Version(15, 0);
            NPath visualStudioInstallationFolder = GetVisualStudioInstallationFolder(version);
            NPath visualStudioDir = GetVisualStudioInstallationFolder(version2);
            NPath path3 = GetVisualStudioInstallationFolder(version3);
            if (visualStudioInstallationFolder != null)
            {
                Msvc10Installation installation = new Msvc10Installation(visualStudioInstallationFolder);
                if (installation.HasCppSDK)
                {
                    _installations.Add(version, installation);
                }
            }
            if (visualStudioDir != null)
            {
                Msvc12Installation installation2 = new Msvc12Installation(visualStudioDir);
                if (installation2.HasCppSDK)
                {
                    _installations.Add(version2, installation2);
                }
            }
            if (path3 != null)
            {
                Msvc14Installation installation3 = new Msvc14Installation(path3);
                if (installation3.HasCppSDK)
                {
                    _installations.Add(version3, installation3);
                }
            }
            if (Msvc15Installation.IsInstalled)
            {
                Msvc15Installation installation4 = new Msvc15Installation();
                if (installation4.HasCppSDK)
                {
                    _installations.Add(key, installation4);
                }
            }
        }

        protected MsvcInstallation(System.Version visualStudioVersion)
        {
            this.VisualStudioDirectory = GetVisualStudioInstallationFolder(visualStudioVersion);
            this.Version = visualStudioVersion;
        }

        protected MsvcInstallation(System.Version visualStudioVersion, NPath visualStudioDir)
        {
            this.VisualStudioDirectory = visualStudioDir;
            this.Version = visualStudioVersion;
        }

        public abstract IEnumerable<NPath> GetIncludeDirectories(Unity.IL2CPP.Building.Architecture architecture);
        public static MsvcInstallation GetLatestInstallationAtLeast(System.Version version)
        {
            <GetLatestInstallationAtLeast>c__AnonStorey1 storey = new <GetLatestInstallationAtLeast>c__AnonStorey1 {
                version = version
            };
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = v => v.Major;
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = v => v.Minor;
            }
            System.Version version2 = _installations.Keys.OrderByDescending<System.Version, int>(<>f__am$cache5).ThenByDescending<System.Version, int>(<>f__am$cache6).Where<System.Version>(new Func<System.Version, bool>(storey.<>m__0)).FirstOrDefault<System.Version>();
            if (version2 == null)
            {
                throw new Exception($"MSVC Installation version {storey.version.Major}.{storey.version.Minor} or later is not installed on current machine!");
            }
            return _installations[version2];
        }

        public static MsvcInstallation GetLatestInstalled()
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = k => k.Major;
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = k => k.Minor;
            }
            System.Version version = _installations.Keys.OrderByDescending<System.Version, int>(<>f__am$cache3).ThenByDescending<System.Version, int>(<>f__am$cache4).FirstOrDefault<System.Version>();
            if (version == null)
            {
                throw new Exception("No MSVC installations were found on the machine!");
            }
            return _installations[version];
        }

        public abstract IEnumerable<NPath> GetLibDirectories(Unity.IL2CPP.Building.Architecture architecture, string sdkSubset = null);
        public virtual string GetPathEnvVariable(Unity.IL2CPP.Building.Architecture architecture)
        {
            List<NPath> source = new List<NPath>();
            if ((architecture is ARMv7Architecture) || (architecture is x86Architecture))
            {
                string[] append = new string[] { "VC", "bin" };
                source.Add(this.VisualStudioDirectory.Combine(append));
                if (this.SDKDirectory != null)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = d => d.Combine(new string[] { "x86" });
                    }
                    source.AddRange(this.GetSDKBinDirectories().Select<NPath, NPath>(<>f__am$cache0));
                }
            }
            else
            {
                string[] textArray2 = new string[] { "VC", "bin", "amd64" };
                source.Add(this.VisualStudioDirectory.Combine(textArray2));
                if (this.SDKDirectory != null)
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = d => d.Combine(new string[] { "x64" });
                    }
                    source.AddRange(this.GetSDKBinDirectories().Select<NPath, NPath>(<>f__am$cache1));
                }
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = p => p.ToString();
            }
            return source.Select<NPath, string>(<>f__am$cache2).AggregateWith(";");
        }

        public virtual IEnumerable<NPath> GetPlatformMetadataReferences()
        {
            throw new NotSupportedException($"{base.GetType().Name} does not support platform metadata");
        }

        [DebuggerHidden]
        protected virtual IEnumerable<NPath> GetSDKBinDirectories() => 
            new <GetSDKBinDirectories>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        public virtual NPath GetSDKToolPath(string toolName)
        {
            string str;
            Unity.IL2CPP.Building.Architecture bestThisMachineCanRun = Unity.IL2CPP.Building.Architecture.BestThisMachineCanRun;
            if (bestThisMachineCanRun is x86Architecture)
            {
                str = "x86";
            }
            else
            {
                if (!(bestThisMachineCanRun is x64Architecture))
                {
                    throw new NotSupportedException($"Invalid host architecture: {bestThisMachineCanRun.GetType().Name}");
                }
                str = "x64";
            }
            foreach (NPath path in this.GetSDKBinDirectories())
            {
                string[] append = new string[] { str, toolName };
                NPath path2 = path.Combine(append);
                if (path2.FileExists(""))
                {
                    return path2;
                }
            }
            throw new NotSupportedException("Can't find MSVC tool for " + bestThisMachineCanRun);
        }

        public virtual NPath GetUnionMetadataDirectory()
        {
            throw new NotSupportedException($"{base.GetType().Name} does not support union metadata");
        }

        protected static NPath GetVisualStudioInstallationFolder(System.Version version)
        {
            if (PlatformUtils.IsWindows())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey($"SOFTWARE\Microsoft\VisualStudio\{version.Major}.{version.Minor}_Config");
                if (key != null)
                {
                    string str = (string) key.GetValue("InstallDir");
                    if (!string.IsNullOrEmpty(str))
                    {
                        return new NPath(str).Parent.Parent;
                    }
                }
            }
            return null;
        }

        public virtual NPath GetVSToolPath(Unity.IL2CPP.Building.Architecture architecture, string toolName)
        {
            string[] append = new string[] { "VC", "bin" };
            NPath path = this.VisualStudioDirectory.Combine(append);
            if (architecture is x86Architecture)
            {
                string[] textArray2 = new string[] { toolName };
                return path.Combine(textArray2);
            }
            if (architecture is x64Architecture)
            {
                string[] textArray3 = new string[] { "amd64", toolName };
                return path.Combine(textArray3);
            }
            if (!(architecture is ARMv7Architecture))
            {
                throw new NotSupportedException("Can't find MSVC tool for " + architecture);
            }
            string[] textArray4 = new string[] { "x86_arm", toolName };
            return path.Combine(textArray4);
        }

        public virtual IEnumerable<NPath> GetWindowsMetadataReferences()
        {
            throw new NotSupportedException($"{base.GetType().Name} does not support windows metadata");
        }

        public virtual bool HasMetadataDirectories() => 
            false;

        protected bool HasCppSDK =>
            ((this.SDKDirectory != null) && this.SDKDirectory.Exists(""));

        protected NPath SDKDirectory { get; set; }

        public abstract IEnumerable<Type> SupportedArchitectures { get; }

        public System.Version Version { get; set; }

        protected virtual NPath VisualStudioDirectory { get; set; }

        [CompilerGenerated]
        private sealed class <GetLatestInstallationAtLeast>c__AnonStorey1
        {
            internal Version version;

            internal bool <>m__0(Version v) => 
                (v >= this.version);
        }

        [CompilerGenerated]
        private sealed class <GetSDKBinDirectories>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal MsvcInstallation $this;

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
                        string[] append = new string[] { "bin" };
                        this.$current = this.$this.SDKDirectory.Combine(append);
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
                return new MsvcInstallation.<GetSDKBinDirectories>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

