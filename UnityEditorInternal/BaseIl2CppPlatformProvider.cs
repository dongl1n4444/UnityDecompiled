namespace UnityEditorInternal
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditor.Modules;

    internal class BaseIl2CppPlatformProvider : IIl2CppPlatformProvider
    {
        [CompilerGenerated]
        private static Func<PackageInfo, bool> <>f__am$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <libraryFolder>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuildTarget <target>k__BackingField;

        public BaseIl2CppPlatformProvider(BuildTarget target, string libraryFolder)
        {
            this.target = target;
            this.libraryFolder = libraryFolder;
        }

        public virtual Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder()
        {
            return null;
        }

        public virtual INativeCompiler CreateNativeCompiler()
        {
            return null;
        }

        private static PackageInfo FindIl2CppPackage()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<PackageInfo, bool>(null, (IntPtr) <FindIl2CppPackage>m__0);
            }
            return Enumerable.FirstOrDefault<PackageInfo>(ModuleManager.packageManager.unityExtensions, <>f__am$cache0);
        }

        protected string GetFileInPackageOrDefault(string path)
        {
            PackageInfo info = FindIl2CppPackage();
            if (info == null)
            {
                return Path.Combine(this.libraryFolder, path);
            }
            string str2 = Path.Combine(info.basePath, path);
            return (File.Exists(str2) ? str2 : Path.Combine(this.libraryFolder, path));
        }

        protected string GetFolderInPackageOrDefault(string path)
        {
            PackageInfo info = FindIl2CppPackage();
            if (info == null)
            {
                return Path.Combine(this.libraryFolder, path);
            }
            string str2 = Path.Combine(info.basePath, path);
            return (Directory.Exists(str2) ? str2 : Path.Combine(this.libraryFolder, path));
        }

        public virtual BuildReport buildReport
        {
            get
            {
                return null;
            }
        }

        public virtual bool developmentMode
        {
            get
            {
                return false;
            }
        }

        public virtual bool emitNullChecks
        {
            get
            {
                return true;
            }
        }

        public virtual bool enableArrayBoundsCheck
        {
            get
            {
                return true;
            }
        }

        public virtual bool enableDivideByZeroCheck
        {
            get
            {
                return false;
            }
        }

        public virtual bool enableStackTraces
        {
            get
            {
                return true;
            }
        }

        public virtual string il2CppFolder
        {
            get
            {
                PackageInfo info = FindIl2CppPackage();
                if (info == null)
                {
                    return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, "il2cpp"));
                }
                return info.basePath;
            }
        }

        public virtual string[] includePaths
        {
            get
            {
                return new string[] { this.GetFolderInPackageOrDefault("bdwgc/include"), this.GetFolderInPackageOrDefault("libil2cpp/include") };
            }
        }

        public string libraryFolder { virtual get; private set; }

        public virtual string[] libraryPaths
        {
            get
            {
                return new string[] { this.GetFileInPackageOrDefault("bdwgc/lib/bdwgc." + this.staticLibraryExtension), this.GetFileInPackageOrDefault("libil2cpp/lib/libil2cpp." + this.staticLibraryExtension) };
            }
        }

        public virtual bool loadSymbols
        {
            get
            {
                return false;
            }
        }

        public virtual string moduleStrippingInformationFolder
        {
            get
            {
                return Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(EditorUserBuildSettings.activeBuildTarget, BuildOptions.CompressTextures), "Whitelists");
            }
        }

        public virtual string nativeLibraryFileName
        {
            get
            {
                return null;
            }
        }

        public virtual string staticLibraryExtension
        {
            get
            {
                return "a";
            }
        }

        public virtual bool supportsEngineStripping
        {
            get
            {
                return false;
            }
        }

        public BuildTarget target { virtual get; private set; }
    }
}

