namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.PackageManager.Ivy;

    internal class LocalIndexer : Indexer
    {
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache0;

        public LocalIndexer()
        {
            base.Name = "Local Indexer";
            base.cachePath = Locator.installLocation;
            base.cacheName = "index-local.xml";
            if (!Settings.teamcity)
            {
                this.LoadFromCache();
            }
            if (Locator.Completed)
            {
                base.FlushCache();
            }
            IEnumerable<object> source = Locator.QueryAllModules();
            if (source.Any<object>())
            {
                IEnumerable<IvyModule> second = source.Cast<IvyModule>();
                IEnumerable<IvyModule> first = base.Packages.Intersect<IvyModule>(second);
                foreach (IvyModule module in first)
                {
                    foreach (IvyModule module2 in second)
                    {
                        if (module == module2)
                        {
                            module.BasePath = module2.BasePath;
                            break;
                        }
                    }
                }
                base.Packages = first.Union<IvyModule>(second.Except<IvyModule>(first)).ToList<IvyModule>();
                this.CacheResult();
            }
            else if (!base.Packages.Any<IvyModule>())
            {
                List<IvyModule> list = new List<IvyModule>();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<Assembly, bool>(null, (IntPtr) <LocalIndexer>m__0);
                }
                foreach (Assembly assembly in Enumerable.Where<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache0))
                {
                    string path = Path.Combine(Path.GetDirectoryName(assembly.Location), "ivy.xml");
                    if (File.Exists(path))
                    {
                        IvyModule item = IvyParser.ParseFile<IvyModule>(path);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                }
                base.Packages = list;
                this.CacheResult();
            }
        }

        [CompilerGenerated]
        private static bool <LocalIndexer>m__0(Assembly a) => 
            (GetAssemblyLocationSafe(a) != null);

        private static string GetAssemblyLocationSafe(Assembly a)
        {
            try
            {
                return a.Location;
            }
            catch
            {
            }
            return null;
        }

        protected override bool TaskStarting()
        {
            <TaskStarting>c__AnonStorey1 storey = new <TaskStarting>c__AnonStorey1 {
                $this = this
            };
            if (!base.TaskStarting())
            {
                return false;
            }
            storey.task1 = new Task(null, new Func<Task, bool>(storey, (IntPtr) this.<>m__0), null);
            storey.task1.Name = "Locator Task";
            Task task = new Task(null, new Func<Task, bool>(storey, (IntPtr) this.<>m__1), null) {
                Name = "Package List Task"
            };
            base.HookupChildTask(storey.task1);
            base.HookupChildTask(task);
            return true;
        }

        [CompilerGenerated]
        private sealed class <TaskStarting>c__AnonStorey1
        {
            internal LocalIndexer $this;
            internal Task task1;

            internal bool <>m__0(Task _)
            {
                <TaskStarting>c__AnonStorey0 storey = new <TaskStarting>c__AnonStorey0 {
                    <>f__ref$1 = this,
                    scanDone = new ManualResetEvent(false)
                };
                Locator.Scan(Path.Combine(Settings.editorInstallPath, "PackageManager"), Settings.unityVersionPath, new Func<bool>(storey, (IntPtr) this.<>m__0), new Action(storey, (IntPtr) this.<>m__1));
                while (!storey.scanDone.WaitOne(10))
                {
                    this.$this.UpdateProgress(0f);
                }
                return true;
            }

            internal bool <>m__1(Task t)
            {
                IEnumerable<object> enumerable = Locator.QueryAllModules();
                List<IvyModule> list = new List<IvyModule>();
                foreach (object obj2 in enumerable)
                {
                    list.Add(obj2 as IvyModule);
                }
                this.$this.Packages = list;
                this.$this.CacheResult();
                return this.task1.IsSuccessful;
            }

            private sealed class <TaskStarting>c__AnonStorey0
            {
                internal LocalIndexer.<TaskStarting>c__AnonStorey1 <>f__ref$1;
                internal ManualResetEvent scanDone;

                internal bool <>m__0() => 
                    !this.<>f__ref$1.$this.CancelRequested;

                internal void <>m__1()
                {
                    this.scanDone.Set();
                }
            }
        }
    }
}

