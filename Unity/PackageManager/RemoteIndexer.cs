namespace Unity.PackageManager
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;
    using Unity.PackageManager.Ivy;
    using UnityEditor.Modules;

    internal class RemoteIndexer : Indexer
    {
        public RemoteIndexer()
        {
            base.Name = "Remote Indexer";
            base.cacheName = "index-remote.xml";
            this.LoadFromCache();
        }

        public override void CacheResult()
        {
            try
            {
                File.Copy(Path.Combine(base.LocalPath.LocalPath, "ivy.xml"), base.IndexCache, true);
            }
            catch
            {
            }
        }

        protected override bool TaskStarting()
        {
            <TaskStarting>c__AnonStorey0 storey = new <TaskStarting>c__AnonStorey0 {
                $this = this
            };
            if (!base.TaskStarting())
            {
                return false;
            }
            if (!this.ValidateSettings())
            {
                return false;
            }
            Uri uri = new Uri(Settings.RepoUrl + "ivy.xml");
            DownloaderTask task = new DownloaderTask(uri) {
                Shared = true
            };
            storey.task1 = task;
            Task task2 = new Task(null, new Func<Task, bool>(storey.<>m__0), null);
            base.HookupChildTask(storey.task1);
            base.HookupChildTask(task2, "Verifier Task");
            return true;
        }

        private bool ValidateSettings()
        {
            Uri uri;
            string uriString = Settings.RepoUrl.ToString();
            if (uriString == null)
            {
                return false;
            }
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
            {
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <TaskStarting>c__AnonStorey0
        {
            internal RemoteIndexer $this;
            private static Func<IvyModule, bool> <>f__am$cache0;
            internal Task task1;

            internal bool <>m__0(Task t)
            {
                try
                {
                    string result = this.task1.Result as string;
                    if (!File.Exists(result))
                    {
                        return false;
                    }
                    IvyVerifier verifier = new IvyVerifier(new Uri(result));
                    if (!verifier.Verify())
                    {
                        return false;
                    }
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = module => (module.Info.Type != PackageType.PlaybackEngine) || ModuleManager.HaveLicenseForBuildTarget(module.Info.Module);
                    }
                    this.$this.Packages = Enumerable.Where<IvyModule>(((ModuleRepository) verifier.Result).Modules, <>f__am$cache0).ToList<IvyModule>();
                    this.$this.CacheResult();
                    this.task1.Shared = false;
                    this.task1.CleanupArtifacts();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private static bool <>m__1(IvyModule module) => 
                ((module.Info.Type != PackageType.PlaybackEngine) || ModuleManager.HaveLicenseForBuildTarget(module.Info.Module));
        }
    }
}

