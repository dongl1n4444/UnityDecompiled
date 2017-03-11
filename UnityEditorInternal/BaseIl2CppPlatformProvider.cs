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
        private static Func<Unity.DataContract.PackageInfo, bool> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <libraryFolder>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuildTarget <target>k__BackingField;

        public BaseIl2CppPlatformProvider(BuildTarget target, string libraryFolder)
        {
            this.target = target;
            this.libraryFolder = libraryFolder;
        }

        public virtual Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder() => 
            null;

        public virtual INativeCompiler CreateNativeCompiler() => 
            null;

        private static Unity.DataContract.PackageInfo FindIl2CppPackage()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = e => e.name == "IL2CPP";
            }
            return Enumerable.FirstOrDefault<Unity.DataContract.PackageInfo>(ModuleManager.packageManager.unityExtensions, <>f__am$cache0);
        }

        protected string GetFileInPackageOrDefault(string path)
        {
            Unity.DataContract.PackageInfo info = FindIl2CppPackage();
            if (info == null)
            {
                return Path.Combine(this.libraryFolder, path);
            }
            string str2 = Path.Combine(info.basePath, path);
            return (File.Exists(str2) ? str2 : Path.Combine(this.libraryFolder, path));
        }

        protected string GetFolderInPackageOrDefault(string path)
        {
            Unity.DataContract.PackageInfo info = FindIl2CppPackage();
            if (info == null)
            {
                return Path.Combine(this.libraryFolder, path);
            }
            string str2 = Path.Combine(info.basePath, path);
            return (Directory.Exists(str2) ? str2 : Path.Combine(this.libraryFolder, path));
        }

        public virtual BuildReport buildReport =>
            null;

        public virtual bool developmentMode =>
            false;

        public virtual bool emitNullChecks =>
            true;

        public virtual bool enableArrayBoundsCheck =>
            true;

        public virtual bool enableDivideByZeroCheck =>
            false;

        public virtual bool enableStackTraces =>
            true;

        public virtual string il2CppFolder
        {
            get
            {
                Unity.DataContract.PackageInfo info = FindIl2CppPackage();
                if (info == null)
                {
                    return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, "il2cpp"));
                }
                return info.basePath;
            }
        }

        public virtual string[] includePaths =>
            new string[] { this.GetFolderInPackageOrDefault("bdwgc/include"), this.GetFolderInPackageOrDefault("libil2cpp/include") };

        public string libraryFolder { virtual get; private set; }

        public virtual string[] libraryPaths =>
            new string[] { this.GetFileInPackageOrDefault("bdwgc/lib/bdwgc." + this.staticLibraryExtension), this.GetFileInPackageOrDefault("libil2cpp/lib/libil2cpp." + this.staticLibraryExtension) };

        public virtual string moduleStrippingInformationFolder =>
            Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(EditorUserBuildSettings.activeBuildTarget, BuildOptions.CompressTextures), "Whitelists");

        public virtual string nativeLibraryFileName =>
            null;

        public virtual string staticLibraryExtension =>
            "a";

        public virtual bool supportsEngineStripping =>
            false;

        public BuildTarget target { virtual get; private set; }
    }
}

