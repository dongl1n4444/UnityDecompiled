namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.DataContract;
    using Unity.PackageManager.Ivy;

    public class Locator
    {
        [CompilerGenerated]
        private static Func<InternalPackageInfo, PackageInfo> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<InternalPackageInfo, object> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<InternalPackageInfo, PackageVersion> <>f__am$cache2;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static bool <Completed>k__BackingField;
        private const int kMaxLevels = 4;
        private static string m_ModuleLocation;
        private static bool s_Cancelled;
        private static string s_InstallLocation;
        private static Func<bool> s_ScanningCallback;
        private static List<InternalPackageInfo> s_Tree = new List<InternalPackageInfo>();

        public static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            if (paths.Length == 1)
            {
                return paths[0];
            }
            StringBuilder builder = new StringBuilder(paths[0]);
            for (int i = 1; i < paths.Length; i++)
            {
                builder.AppendFormat("{0}{1}", Path.DirectorySeparatorChar, paths[i]);
            }
            return builder.ToString();
        }

        public static PackageInfo GetPackageManager(string unityVersion)
        {
            <GetPackageManager>c__AnonStorey0 storey = new <GetPackageManager>c__AnonStorey0 {
                version = new PackageVersion(unityVersion)
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<InternalPackageInfo, PackageVersion>(null, (IntPtr) <GetPackageManager>m__2);
            }
            InternalPackageInfo info = Enumerable.FirstOrDefault<InternalPackageInfo>(Enumerable.OrderByDescending<InternalPackageInfo, PackageVersion>(Enumerable.Where<InternalPackageInfo>(s_Tree, new Func<InternalPackageInfo, bool>(storey, (IntPtr) this.<>m__0)), <>f__am$cache2));
            if (info != null)
            {
                return info.package;
            }
            return null;
        }

        private static InternalPackageInfo Parse(string moduleFile)
        {
            if (moduleFile == null)
            {
                return null;
            }
            IvyModule module = IvyParser.ParseFile<IvyModule>(moduleFile);
            if (IvyParser.HasErrors)
            {
                Console.WriteLine("Error parsing module description from {0}. {1}", moduleFile, IvyParser.ErrorMessage);
                return null;
            }
            return new InternalPackageInfo { 
                module = module,
                package = module.ToPackageInfo()
            };
        }

        public static IEnumerable<PackageInfo> QueryAll()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<InternalPackageInfo, PackageInfo>(null, (IntPtr) <QueryAll>m__0);
            }
            return Enumerable.Select<InternalPackageInfo, PackageInfo>(s_Tree, <>f__am$cache0);
        }

        public static IEnumerable<object> QueryAllModules()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<InternalPackageInfo, object>(null, (IntPtr) <QueryAllModules>m__1);
            }
            return Enumerable.Select<InternalPackageInfo, object>(s_Tree, <>f__am$cache1);
        }

        public static void Scan(string editorInstallPath, string unityVersion)
        {
            Completed = false;
            PackageVersion version = new PackageVersion(unityVersion);
            s_Cancelled = false;
            s_Tree = new List<InternalPackageInfo>();
            if (Directory.Exists(editorInstallPath))
            {
                ScanDirectory(editorInstallPath, editorInstallPath, 0);
            }
            string[] paths = new string[] { installLocation, string.Format("{0}.{1}", version.major, version.minor) };
            string path = CombinePaths(paths);
            if (Directory.Exists(path))
            {
                ScanDirectory(path, path, 0);
            }
            Completed = true;
        }

        public static void Scan(string[] scanPaths, string unityVersion)
        {
            Completed = false;
            PackageVersion version = new PackageVersion(unityVersion);
            s_Cancelled = false;
            s_Tree = new List<InternalPackageInfo>();
            foreach (string str in scanPaths)
            {
                if ((str != null) && Directory.Exists(str))
                {
                    ScanDirectory(str, str, 0);
                }
            }
            string[] paths = new string[] { installLocation, string.Format("{0}.{1}", version.major, version.minor) };
            string path = CombinePaths(paths);
            if (Directory.Exists(path))
            {
                ScanDirectory(path, path, 0);
            }
            Completed = true;
        }

        public static void Scan(string editorInstallPath, string unityVersion, Func<bool> scanInProgressCallback, Action scanDoneCallback)
        {
            s_ScanningCallback = scanInProgressCallback;
            Scan(editorInstallPath, unityVersion);
            if (scanDoneCallback != null)
            {
                scanDoneCallback.Invoke();
            }
        }

        private static void ScanDirectory(string rootPath, string path, int level)
        {
            if ((UserWantsToContinue() && (level <= 4)) && ((path != null) && (rootPath != null)))
            {
                if (File.Exists(Path.Combine(path, moduleFile)))
                {
                    try
                    {
                        string[] paths = new string[] { path, moduleFile };
                        InternalPackageInfo item = Parse(CombinePaths(paths));
                        if (item != null)
                        {
                            s_Tree.Add(item);
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        string[] textArray2 = new string[] { path, moduleFile };
                        Console.WriteLine("Error parsing module from {0}. {1}", CombinePaths(textArray2), exception.Message);
                        return;
                    }
                }
                string[] directories = Directory.GetDirectories(path);
                if (directories.Length != 0)
                {
                    foreach (string str in directories)
                    {
                        ScanDirectory(rootPath, str, level + 1);
                    }
                }
            }
        }

        private static bool UserWantsToContinue()
        {
            if (s_ScanningCallback != null)
            {
                s_Cancelled = !s_ScanningCallback.Invoke();
                if (s_Cancelled)
                {
                    s_ScanningCallback = null;
                }
            }
            return !s_Cancelled;
        }

        public static bool Completed
        {
            [CompilerGenerated]
            get
            {
                return <Completed>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <Completed>k__BackingField = value;
            }
        }

        public static string installLocation
        {
            get
            {
                if (s_InstallLocation == null)
                {
                    if (isLinux)
                    {
                        s_InstallLocation = Path.Combine(moduleLocation, "unity3d");
                    }
                    else
                    {
                        s_InstallLocation = Path.Combine(moduleLocation, "Unity");
                    }
                }
                return s_InstallLocation;
            }
            set
            {
                s_InstallLocation = value;
            }
        }

        private static bool isLinux
        {
            get
            {
                return ((Environment.OSVersion.Platform == PlatformID.Unix) && Directory.Exists("/proc"));
            }
        }

        private static string moduleFile
        {
            get
            {
                return "ivy.xml";
            }
        }

        public static string moduleLocation
        {
            get
            {
                if (m_ModuleLocation == null)
                {
                    switch (Environment.OSVersion.Platform)
                    {
                        case PlatformID.Unix:
                        case PlatformID.MacOSX:
                            m_ModuleLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                            goto Label_004D;
                    }
                    m_ModuleLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                }
            Label_004D:
                return m_ModuleLocation;
            }
            set
            {
                m_ModuleLocation = value;
            }
        }

        private static bool teamcity
        {
            get
            {
                return (Environment.GetEnvironmentVariable("UNITY_THISISABUILDMACHINE") == "1");
            }
        }

        [CompilerGenerated]
        private sealed class <GetPackageManager>c__AnonStorey0
        {
            internal PackageVersion version;

            internal bool <>m__0(InternalPackageInfo p)
            {
                return ((p.package.type == PackageType.PackageManager) && p.module.UnityVersion.IsCompatibleWith(this.version));
            }
        }
    }
}

