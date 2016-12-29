namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;
    using Unity.PackageManager.Ivy;
    using UnityEditorInternal;

    public class PackageManager : IDisposable
    {
        [CompilerGenerated]
        private static TaskFinishedHandler <>f__am$cache0;
        [CompilerGenerated]
        private static TaskFinishedHandler <>f__am$cache1;
        [CompilerGenerated]
        private static Func<IvyModule, bool> <>f__am$cache2;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, Installer> <Installers>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Indexer <LocalIndexer>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static bool <Ready>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Indexer <RemoteIndexer>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <UpdateCheckRequested>k__BackingField;
        private Unity.PackageManager.Database database;
        private static Unity.PackageManager.PackageManager instance;
        private IvyModule moduleInfo;
        private List<Action<Task>> registeredListeners;
        private object runLock;
        private bool running;

        public event Action<Task> OnTask
        {
            add
            {
                this.registeredListeners.Add(value);
            }
            remove
            {
                if (this.registeredListeners.Contains(value))
                {
                    this.registeredListeners.Remove(value);
                }
                else if (value == null)
                {
                    this.registeredListeners.Clear();
                }
            }
        }

        private PackageManager(PackageInfo packageInfo)
        {
            this.registeredListeners = new List<Action<Task>>();
            this.running = false;
            this.runLock = new object();
            this.database = new Unity.PackageManager.Database();
            this.RemoteIndexer = new Unity.PackageManager.RemoteIndexer();
            this.LocalIndexer = new Unity.PackageManager.LocalIndexer();
            if (packageInfo != null)
            {
                this.moduleInfo = IvyParser.ParseFile<IvyModule>(Path.Combine(packageInfo.basePath, "ivy.xml"));
                if (IvyParser.HasErrors)
                {
                    return;
                }
            }
            this.Initialize();
        }

        private PackageManager(IvyModule module)
        {
            this.registeredListeners = new List<Action<Task>>();
            this.running = false;
            this.runLock = new object();
            this.database = new Unity.PackageManager.Database();
            this.RemoteIndexer = new Unity.PackageManager.RemoteIndexer();
            this.LocalIndexer = new Unity.PackageManager.LocalIndexer();
            this.moduleInfo = module;
            this.Initialize();
        }

        internal void CheckForLocalUpdates()
        {
            Indexer localIndexer = null;
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    return;
                }
                localIndexer = this.LocalIndexer;
            }
            this.RunIndexer(localIndexer);
        }

        internal void CheckForRemoteUpdates()
        {
            Indexer remoteIndexer = null;
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    return;
                }
                remoteIndexer = this.RemoteIndexer;
            }
            this.UpdateCheckRequested = false;
            this.RunIndexer(remoteIndexer);
        }

        public static void CheckForUpdates()
        {
            if (instance != null)
            {
                instance.RequestUpdate();
            }
        }

        public void Dispose()
        {
            Stop(true);
        }

        internal void FireListeners(Task task)
        {
            foreach (Action<Task> action in this.registeredListeners)
            {
                action(task);
            }
        }

        private IEnumerable<IvyModule> GetCachedLocalModules() => 
            GetCachedModulesForIndex(this.LocalIndexer.IndexCache);

        private static IEnumerable<IvyModule> GetCachedModulesForIndex(string indexFile)
        {
            try
            {
                if (File.Exists(indexFile))
                {
                    return IvyParser.Deserialize<ModuleRepository>(File.ReadAllText(indexFile)).Modules;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error loading index cache {0}: {1}", indexFile, exception);
            }
            return new List<IvyModule>();
        }

        private IEnumerable<IvyModule> GetCachedRemoteModules() => 
            GetCachedModulesForIndex(this.RemoteIndexer.IndexCache);

        private void Initialize()
        {
            if (this.moduleInfo != null)
            {
                object[] arg = new object[] { this.moduleInfo.Name, this.moduleInfo.Info.Type, this.moduleInfo.Version, this.moduleInfo.UnityVersion };
                Console.WriteLine("Initializing {0} ({1}) v{2} for Unity v{3}", arg);
                Ready = true;
                this.moduleInfo.Loaded = true;
            }
            else
            {
                Console.WriteLine("Initializing Package Manager with no module information. This should only happen during testing.");
            }
            this.database.OnUpdateAvailable += new Action<IvyModule[]>(this.PackageUpdatesAvailable);
            this.database.RefreshLocalPackages(this.LocalIndexer.Packages);
            this.database.UpdatePackageState(this.moduleInfo);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (Task t, bool s) {
                    if (s)
                    {
                        Database.RefreshRemotePackages(((Indexer) t).Packages);
                    }
                };
            }
            this.RemoteIndexer.OnFinish += <>f__am$cache0;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = delegate (Task t, bool s) {
                    if (s)
                    {
                        Database.RefreshLocalPackages(((Indexer) t).Packages);
                    }
                };
            }
            this.LocalIndexer.OnFinish += <>f__am$cache1;
            this.Installers = new Dictionary<string, Installer>();
        }

        public static void Initialize(PackageInfo packageInfo)
        {
            if (instance == null)
            {
                instance = new Unity.PackageManager.PackageManager(packageInfo);
            }
        }

        public static void Initialize(IvyModule module)
        {
            if (instance == null)
            {
                instance = new Unity.PackageManager.PackageManager(module);
            }
        }

        private void PackageUpdatesAvailable(IvyModule[] packages)
        {
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    return;
                }
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<IvyModule, bool>(null, (IntPtr) <PackageUpdatesAvailable>m__3);
            }
            IvyModule package = Enumerable.FirstOrDefault<IvyModule>(packages, <>f__am$cache2);
            if (package != null)
            {
                foreach (Installer installer in this.Installers.Values)
                {
                    if (installer.Package.Info.FullName == package.Info.FullName)
                    {
                        return;
                    }
                }
                this.SetupPackageInstall(package).Run();
            }
        }

        private void RefreshLocalPackages()
        {
            Unity.PackageManager.Database database = null;
            Indexer localIndexer = null;
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    return;
                }
                database = this.database;
                localIndexer = this.LocalIndexer;
            }
            database.RefreshLocalPackages(localIndexer.Packages);
        }

        private void RefreshRemotePackages()
        {
            Unity.PackageManager.Database database = null;
            Indexer remoteIndexer = null;
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    return;
                }
                database = this.database;
                remoteIndexer = this.RemoteIndexer;
            }
            database.RefreshRemotePackages(remoteIndexer.Packages);
        }

        private void RequestUpdate()
        {
            if (this.running)
            {
                this.CheckForRemoteUpdates();
            }
            else
            {
                this.UpdateCheckRequested = true;
            }
        }

        private void ResumePendingTasks()
        {
            Task[] taskArray = TaskFactory.FromExisting(Settings.installLocation);
            foreach (Task task in taskArray)
            {
                if (task is Installer)
                {
                    this.Installers.Add(task.JobId.ToString(), task as Installer);
                    task.OnFinish += delegate (Task t, bool s) {
                        this.Installers.Remove(t.JobId.ToString());
                        if (s)
                        {
                            this.CheckForLocalUpdates();
                        }
                    };
                    this.FireListeners(task);
                    task.Run();
                }
            }
        }

        private void RunIndexer(Indexer indexer)
        {
            if (!indexer.IsRunning)
            {
                this.FireListeners(indexer);
                indexer.Run();
            }
        }

        internal Installer SetupPackageInstall(IvyModule package)
        {
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    return null;
                }
            }
            Installer installer2 = new Installer(package);
            this.Installers.Add(installer2.JobId.ToString(), installer2);
            installer2.OnFinish += delegate (Task t, bool s) {
                if (s || t.CancelRequested)
                {
                    if (this.Installers.ContainsKey(t.JobId.ToString()))
                    {
                        this.Installers.Remove(t.JobId.ToString());
                    }
                    try
                    {
                        if (s)
                        {
                            this.CheckForLocalUpdates();
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            };
            this.FireListeners(installer2);
            return installer2;
        }

        public static void Start()
        {
            if (instance == null)
            {
                instance = new Unity.PackageManager.PackageManager(null);
            }
            instance.StartInstance();
        }

        private void StartInstance()
        {
            bool flag = false;
            object runLock = this.runLock;
            lock (runLock)
            {
                if (!this.running)
                {
                    flag = true;
                    this.running = true;
                }
            }
            if (flag)
            {
                this.Installers = new Dictionary<string, Installer>();
                ThreadUtils.SetMainThread();
                Settings.CacheAllSettings();
                this.ResumePendingTasks();
                this.RefreshRemotePackages();
                if (this.UpdateCheckRequested && (!InternalEditorUtility.inBatchMode && (!Settings.teamcity || Settings.inTestMode)))
                {
                    this.CheckForRemoteUpdates();
                }
            }
        }

        public static void Stop(bool wait)
        {
            if (instance != null)
            {
                instance.StopInstance(wait);
            }
        }

        private void StopInstance(bool wait)
        {
            Indexer remoteIndexer = null;
            Indexer localIndexer = null;
            Service[] serviceArray = null;
            bool flag = false;
            object runLock = this.runLock;
            lock (runLock)
            {
                if (this.running)
                {
                    flag = true;
                    this.running = false;
                    remoteIndexer = this.RemoteIndexer;
                    localIndexer = this.LocalIndexer;
                    serviceArray = this.Installers.Values.ToArray<Installer>();
                    this.LocalIndexer = null;
                    this.RemoteIndexer = null;
                    this.Installers = null;
                }
            }
            if (flag)
            {
                if (localIndexer != null)
                {
                    localIndexer.Stop(wait);
                }
                if (remoteIndexer != null)
                {
                    remoteIndexer.Stop(wait);
                }
                if (serviceArray != null)
                {
                    foreach (Service service in serviceArray)
                    {
                        service.Stop(wait);
                    }
                }
                this.database.Dispose();
                this.database = null;
                instance = null;
            }
        }

        internal static Unity.PackageManager.Database Database
        {
            get
            {
                if (Instance == null)
                {
                    return null;
                }
                return Instance.database;
            }
        }

        internal Dictionary<string, Installer> Installers { get; private set; }

        public static Unity.PackageManager.PackageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Unity.PackageManager.PackageManager(null);
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        internal Indexer LocalIndexer { get; private set; }

        internal static bool Ready
        {
            [CompilerGenerated]
            get => 
                <Ready>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <Ready>k__BackingField = value;
            }
        }

        internal Indexer RemoteIndexer { get; private set; }

        internal bool UpdateCheckRequested { get; private set; }

        internal string Version =>
            ((this.moduleInfo == null) ? Assembly.GetExecutingAssembly().GetName().Version.ToString() : this.moduleInfo.Info.Version.ToString());
    }
}

