namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;
    using Unity.PackageManager.Ivy;

    public class EditorModule : IPackageManagerModule, IEditorModule, IDisposable
    {
        [CompilerGenerated]
        private static Func<IvyModule, PackageInfo> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PackageInfo <moduleInfo>k__BackingField;

        public void CheckForUpdates()
        {
            Unity.PackageManager.PackageManager.CheckForUpdates();
        }

        public void Dispose()
        {
            Unity.PackageManager.PackageManager.Stop(false);
        }

        private static IEnumerable<PackageInfo> GetNewestLocalPackagesOfType(PackageType type)
        {
            <GetNewestLocalPackagesOfType>c__AnonStorey0 storey = new <GetNewestLocalPackagesOfType>c__AnonStorey0 {
                type = type
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<IvyModule, PackageInfo>(null, (IntPtr) <GetNewestLocalPackagesOfType>m__0);
            }
            return Enumerable.Select<IvyModule, PackageInfo>(Enumerable.Where<IvyModule>(Unity.PackageManager.PackageManager.Database.NewestLocalPackages, new Func<IvyModule, bool>(storey, (IntPtr) this.<>m__0)), <>f__am$cache0);
        }

        public void Initialize()
        {
            Unity.PackageManager.PackageManager.Initialize(this.moduleInfo);
        }

        public void LoadPackage(PackageInfo package)
        {
            package.loaded = true;
            Unity.PackageManager.PackageManager.Database.UpdatePackageState(package);
        }

        public void SelectPackage(PackageInfo package)
        {
            Unity.PackageManager.PackageManager.Database.SelectPackage(package);
            Unity.PackageManager.PackageManager.Instance.LocalIndexer.CacheResult();
        }

        public void Shutdown(bool wait)
        {
            Unity.PackageManager.PackageManager.Stop(wait);
        }

        public string editorInstallPath
        {
            get => 
                Settings.editorInstallPath;
            set
            {
                Settings.editorInstallPath = value;
            }
        }

        public PackageInfo moduleInfo { get; set; }

        public IEnumerable<PackageInfo> playbackEngines =>
            GetNewestLocalPackagesOfType(PackageType.PlaybackEngine);

        public IEnumerable<PackageInfo> unityExtensions =>
            GetNewestLocalPackagesOfType(PackageType.UnityExtension);

        public string unityVersion
        {
            get => 
                Settings.unityVersion.ToString();
            set
            {
                Settings.unityVersion = new PackageVersion(value);
            }
        }

        public UpdateMode updateMode
        {
            get => 
                Settings.updateMode;
            set
            {
                Settings.updateMode = value;
            }
        }

        [CompilerGenerated]
        private sealed class <GetNewestLocalPackagesOfType>c__AnonStorey0
        {
            internal PackageType type;

            internal bool <>m__0(IvyModule m) => 
                (m.Info.Type == this.type);
        }
    }
}

