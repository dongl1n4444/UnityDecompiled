namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.DataContract;
    using Unity.PackageManager.Ivy;

    internal class Database : IDisposable
    {
        [CompilerGenerated]
        private static Func<IGrouping<string, IvyModule>, IvyModule> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<IvyModule, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<IvyModule, PackageVersion> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<IvyModule, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<IGrouping<string, IvyModule>, IEnumerable<IvyModule>> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<IvyModule, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<IvyModule, PackageVersion> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<IvyModule, string> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<IGrouping<string, IvyModule>, IvyModule> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<IvyModule, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<IvyModule, bool> <>f__am$cacheA;
        private ILookup<string, IvyModule> localPackagesByName;
        private IEnumerable<IvyModule> packagesNewOrUpdated = Enumerable.Empty<IvyModule>();
        private ILookup<string, IvyModule> remotePackagesByName;

        public event Action<IvyModule[]> OnUpdateAvailable;

        public event Action<IvyModule[]> OnUpdateLocal;

        public IEnumerable<IvyModule> AllVersionsOfPackage(string packageName)
        {
            IEnumerable<IvyModule> first = Enumerable.Empty<IvyModule>();
            if (this.localPackagesByName != null)
            {
                first = first.Union<IvyModule>(this.localPackagesByName[packageName]);
            }
            if (this.remotePackagesByName != null)
            {
                first = first.Union<IvyModule>(this.remotePackagesByName[packageName]);
            }
            return first;
        }

        private void CleanupHandlers()
        {
            this.OnUpdateAvailable = null;
        }

        public void Dispose()
        {
            this.CleanupHandlers();
        }

        public void RefreshLocalPackages(IEnumerable<IvyModule> packages)
        {
            if (this.localPackagesByName != null)
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = x => x;
                }
                IvyModule[] moduleArray2 = Enumerable.SelectMany<IGrouping<string, IvyModule>, IvyModule>(this.localPackagesByName, <>f__am$cache4).ToArray<IvyModule>();
                for (int i = 0; i < moduleArray2.Length; i++)
                {
                    <RefreshLocalPackages>c__AnonStorey0 storey = new <RefreshLocalPackages>c__AnonStorey0 {
                        old = moduleArray2[i]
                    };
                    Enumerable.Any<IvyModule>(packages, new Func<IvyModule, bool>(storey.<>m__0));
                }
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = x => x.UnityVersion.IsCompatibleWith(Settings.unityVersion);
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = p => p.Version;
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = p => p.Name;
            }
            this.localPackagesByName = Enumerable.ToLookup<IvyModule, string>(Enumerable.OrderByDescending<IvyModule, PackageVersion>(Enumerable.Where<IvyModule>(packages, <>f__am$cache5), <>f__am$cache6), <>f__am$cache7);
            this.RefreshNewPackages();
            if (this.OnUpdateLocal != null)
            {
                this.OnUpdateLocal(this.NewestLocalPackages.ToArray<IvyModule>());
            }
        }

        private void RefreshNewPackages()
        {
            if (this.remotePackagesByName != null)
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = g => g.First<IvyModule>();
                }
                this.packagesNewOrUpdated = Enumerable.Where<IvyModule>(Enumerable.Select<IGrouping<string, IvyModule>, IvyModule>(this.remotePackagesByName, <>f__am$cache8), delegate (IvyModule e) {
                    IvyModule module = this.localPackagesByName[e.Name].FirstOrDefault<IvyModule>();
                    return (module == null) || (e.Version > module.Version);
                });
                if (this.OnUpdateAvailable != null)
                {
                    this.OnUpdateAvailable(this.packagesNewOrUpdated.ToArray<IvyModule>());
                }
            }
        }

        public void RefreshRemotePackages(IEnumerable<IvyModule> packages)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => x.UnityVersion.IsCompatibleWith(Settings.unityVersion);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = p => p.Version;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = p => p.Name;
            }
            this.remotePackagesByName = Enumerable.ToLookup<IvyModule, string>(Enumerable.OrderByDescending<IvyModule, PackageVersion>(Enumerable.Where<IvyModule>(packages, <>f__am$cache1), <>f__am$cache2), <>f__am$cache3);
            this.RefreshNewPackages();
        }

        public void SelectPackage(PackageInfo package)
        {
            <SelectPackage>c__AnonStorey3 storey = new <SelectPackage>c__AnonStorey3 {
                package = package
            };
            IGrouping<string, IvyModule> grouping = Enumerable.FirstOrDefault<IGrouping<string, IvyModule>>(this.localPackagesByName, new Func<IGrouping<string, IvyModule>, bool>(storey.<>m__0));
            if (grouping != null)
            {
                foreach (IvyModule module in grouping)
                {
                    module.Selected = module == storey.package;
                }
            }
        }

        public void SelectPackage(IvyModule module)
        {
            <SelectPackage>c__AnonStorey4 storey = new <SelectPackage>c__AnonStorey4 {
                module = module
            };
            IGrouping<string, IvyModule> grouping = Enumerable.FirstOrDefault<IGrouping<string, IvyModule>>(this.localPackagesByName, new Func<IGrouping<string, IvyModule>, bool>(storey.<>m__0));
            if (grouping != null)
            {
                foreach (IvyModule module2 in grouping)
                {
                    module2.Selected = module2 == storey.module;
                }
            }
        }

        public void UpdatePackageState(PackageInfo package)
        {
            <UpdatePackageState>c__AnonStorey1 storey = new <UpdatePackageState>c__AnonStorey1 {
                package = package
            };
            IGrouping<string, IvyModule> grouping = Enumerable.FirstOrDefault<IGrouping<string, IvyModule>>(this.localPackagesByName, new Func<IGrouping<string, IvyModule>, bool>(storey.<>m__0));
            if (grouping != null)
            {
                Enumerable.Any<IvyModule>(grouping, new Func<IvyModule, bool>(storey.<>m__1));
            }
        }

        public void UpdatePackageState(IvyModule package)
        {
            <UpdatePackageState>c__AnonStorey2 storey = new <UpdatePackageState>c__AnonStorey2 {
                package = package
            };
            IGrouping<string, IvyModule> grouping = Enumerable.FirstOrDefault<IGrouping<string, IvyModule>>(this.localPackagesByName, new Func<IGrouping<string, IvyModule>, bool>(storey.<>m__0));
            if (grouping != null)
            {
                Enumerable.Any<IvyModule>(grouping, new Func<IvyModule, bool>(storey.<>m__1));
            }
        }

        public IEnumerable<IvyModule> NewestLocalPackages
        {
            get
            {
                if (this.localPackagesByName == null)
                {
                    return Enumerable.Empty<IvyModule>();
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = delegate (IGrouping<string, IvyModule> g) {
                        if (<>f__am$cache9 == null)
                        {
                            <>f__am$cache9 = h => h.Selected;
                        }
                        if (Enumerable.Any<IvyModule>(g, <>f__am$cache9))
                        {
                        }
                        return (<>f__am$cacheA != null) ? g.First<IvyModule>() : Enumerable.First<IvyModule>(g, <>f__am$cacheA);
                    };
                }
                return Enumerable.Select<IGrouping<string, IvyModule>, IvyModule>(this.localPackagesByName, <>f__am$cache0);
            }
        }

        public IEnumerable<IvyModule> NewOrUpdatedPackages =>
            this.packagesNewOrUpdated;

        [CompilerGenerated]
        private sealed class <RefreshLocalPackages>c__AnonStorey0
        {
            internal IvyModule old;

            internal bool <>m__0(IvyModule p)
            {
                if (this.old == p)
                {
                    p.Loaded = this.old.Loaded;
                    return true;
                }
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <SelectPackage>c__AnonStorey3
        {
            internal PackageInfo package;

            internal bool <>m__0(IGrouping<string, IvyModule> p) => 
                (p.Key == this.package.packageName);
        }

        [CompilerGenerated]
        private sealed class <SelectPackage>c__AnonStorey4
        {
            internal IvyModule module;

            internal bool <>m__0(IGrouping<string, IvyModule> p) => 
                (p.Key == this.module.Name);
        }

        [CompilerGenerated]
        private sealed class <UpdatePackageState>c__AnonStorey1
        {
            internal PackageInfo package;

            internal bool <>m__0(IGrouping<string, IvyModule> n) => 
                (n.Key == this.package.packageName);

            internal bool <>m__1(IvyModule m)
            {
                if (m == this.package)
                {
                    m.Loaded = this.package.loaded;
                    return true;
                }
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <UpdatePackageState>c__AnonStorey2
        {
            internal IvyModule package;

            internal bool <>m__0(IGrouping<string, IvyModule> n) => 
                (n.Key == this.package.Name);

            internal bool <>m__1(IvyModule m)
            {
                if (m == this.package)
                {
                    m.Loaded = this.package.Loaded;
                    return true;
                }
                return false;
            }
        }
    }
}

