namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions
{
    using Microsoft.Win32;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;

    public abstract class MsvcInstallation
    {
        private static Dictionary<System.Version, MsvcInstallation> _installations = new Dictionary<System.Version, MsvcInstallation>();
        [CompilerGenerated]
        private static Func<System.Version, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<System.Version, int> <>f__am$cache1;
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

        public abstract IEnumerable<NPath> GetIncludeDirectories();
        public static MsvcInstallation GetInstallation(System.Version version)
        {
            <GetInstallation>c__AnonStorey0 storey = new <GetInstallation>c__AnonStorey0 {
                version = version
            };
            System.Version version2 = Enumerable.FirstOrDefault<System.Version>(_installations.Keys, new Func<System.Version, bool>(storey, (IntPtr) this.<>m__0));
            if (version2 == null)
            {
                throw new Exception(string.Format("MSVC Installation version {0}.{1} is not installed on current machine!", storey.version.Major, storey.version.Minor));
            }
            return _installations[version2];
        }

        public static MsvcInstallation GetLatestInstalled()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<System.Version, int>(null, (IntPtr) <GetLatestInstalled>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<System.Version, int>(null, (IntPtr) <GetLatestInstalled>m__1);
            }
            System.Version version = Enumerable.FirstOrDefault<System.Version>(Enumerable.ThenByDescending<System.Version, int>(Enumerable.OrderByDescending<System.Version, int>(_installations.Keys, <>f__am$cache0), <>f__am$cache1));
            if (version == null)
            {
                throw new Exception("No MSVC installations were found on the machine!");
            }
            return _installations[version];
        }

        public abstract IEnumerable<NPath> GetLibDirectories(Unity.IL2CPP.Building.Architecture architecture, [Optional, DefaultParameterValue(null)] string sdkSubset);
        public virtual string GetPathEnvVariable(Unity.IL2CPP.Building.Architecture architecture)
        {
            NPath path;
            NPath path2 = null;
            if ((architecture is ARMv7Architecture) || (architecture is x86Architecture))
            {
                string[] textArray1 = new string[] { "VC", "bin" };
                path = this.VisualStudioDirectory.Combine(textArray1);
                if (this.SDKDirectory != null)
                {
                    string[] textArray2 = new string[] { "bin" };
                    string[] textArray3 = new string[] { "x86" };
                    path2 = this.SDKDirectory.Combine(textArray2).Combine(textArray3);
                }
                return string.Format("{0};{1}", path, path2);
            }
            string[] append = new string[] { "VC", "bin", "amd64" };
            path = this.VisualStudioDirectory.Combine(append);
            if (this.SDKDirectory != null)
            {
                string[] textArray5 = new string[] { "bin" };
                string[] textArray6 = new string[] { "x64" };
                path2 = this.SDKDirectory.Combine(textArray5).Combine(textArray6);
            }
            if (path2 != null)
            {
                return string.Format("{0};{1}", path, path2);
            }
            return path.ToString();
        }

        public virtual IEnumerable<NPath> GetPlatformMetadataReferences()
        {
            throw new NotSupportedException(string.Format("{0} does not support platform metadata", base.GetType().Name));
        }

        public NPath GetSDKToolPath(string toolName)
        {
            string[] append = new string[] { "bin" };
            NPath path = this.SDKDirectory.Combine(append);
            Unity.IL2CPP.Building.Architecture bestThisMachineCanRun = Unity.IL2CPP.Building.Architecture.BestThisMachineCanRun;
            if (bestThisMachineCanRun is x86Architecture)
            {
                string[] textArray2 = new string[] { "x86", toolName };
                return path.Combine(textArray2);
            }
            if (!(bestThisMachineCanRun is x64Architecture))
            {
                throw new NotSupportedException("Can't find MSVC tool for " + bestThisMachineCanRun);
            }
            string[] textArray3 = new string[] { "x64", toolName };
            return path.Combine(textArray3);
        }

        public virtual NPath GetUnionMetadataDirectory()
        {
            throw new NotSupportedException(string.Format("{0} does not support union metadata", base.GetType().Name));
        }

        protected static NPath GetVisualStudioInstallationFolder(System.Version version)
        {
            if (PlatformUtils.IsWindows())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}.{1}_Config", version.Major, version.Minor));
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

        public NPath GetVSToolPath(Unity.IL2CPP.Building.Architecture architecture, string toolName)
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
            throw new NotSupportedException(string.Format("{0} does not support windows metadata", base.GetType().Name));
        }

        public virtual bool HasMetadataDirectories()
        {
            return false;
        }

        protected bool HasCppSDK
        {
            get
            {
                return ((this.SDKDirectory != null) && this.SDKDirectory.Exists(""));
            }
        }

        protected NPath SDKDirectory { get; set; }

        public abstract IEnumerable<Type> SupportedArchitectures { get; }

        public System.Version Version { get; set; }

        protected NPath VisualStudioDirectory { get; set; }

        [CompilerGenerated]
        private sealed class <GetInstallation>c__AnonStorey0
        {
            internal Version version;

            internal bool <>m__0(Version k)
            {
                return (k == this.version);
            }
        }
    }
}

