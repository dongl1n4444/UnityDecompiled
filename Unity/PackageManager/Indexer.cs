namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Unity.DataContract;
    using Unity.PackageManager.Ivy;

    internal class Indexer : Service
    {
        protected string cacheName;
        protected string cachePath;
        protected List<IvyModule> packages = new List<IvyModule>();
        private static UpdateMode updateMode = Settings.updateMode;

        public Indexer()
        {
            string[] parts = new string[] { Locator.installLocation, Settings.unityVersionPath };
            this.cachePath = Utils.BuildPath(parts);
            this.cacheName = "index-cache.xml";
        }

        public virtual void CacheResult()
        {
            ModuleRepository repository = new ModuleRepository();
            if (this.packages != null)
            {
                repository.Modules.AddRange(this.packages);
            }
            try
            {
                DirectoryInfo tempDirectory = Utils.GetTempDirectory();
                File.Copy(repository.WriteIvyFile(tempDirectory.FullName, null), this.IndexCache, true);
                tempDirectory.Delete(true);
            }
            catch
            {
            }
        }

        public void FlushCache()
        {
            try
            {
                if (File.Exists(this.IndexCache))
                {
                    File.Delete(this.IndexCache);
                }
            }
            catch
            {
            }
        }

        public virtual void LoadFromCache()
        {
            try
            {
                if (File.Exists(this.IndexCache))
                {
                    ModuleRepository repository = IvyParser.Deserialize<ModuleRepository>(File.ReadAllText(this.IndexCache));
                    this.Packages = repository.Modules;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error loading index cache {0}: {1}\n{2}", this.IndexCache, exception, IvyParser.ErrorException);
            }
        }

        public string IndexCache
        {
            get
            {
                return Path.Combine(this.cachePath, this.cacheName);
            }
        }

        public List<IvyModule> Packages
        {
            get
            {
                return this.packages;
            }
            set
            {
                this.packages = value;
                base.Result = value;
            }
        }
    }
}

