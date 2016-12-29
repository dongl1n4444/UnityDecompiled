namespace Unity.PackageManager
{
    using System;
    using System.IO;
    using Unity.PackageManager.Ivy;

    internal class Installer : Service
    {
        private IvyModule package;

        public Installer(IvyModule package)
        {
            this.package = package;
            base.Name = "Installer Task";
            string path = Path.Combine(Settings.installLocation, "task-installer-" + base.JobId);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, package.ToString());
        }

        public Installer(IvyModule package, Guid id) : base(id)
        {
            this.package = package;
            base.Name = "Installer Task";
            base.Restarted = true;
        }

        public static Installer FromExisting(string path)
        {
            IvyModule package = IvyParser.ParseFile<IvyModule>(path);
            if (package == null)
            {
                return null;
            }
            string g = Path.GetFileName(path).Substring("task-installer-".Length);
            return new Installer(package, new Guid(g));
        }

        protected override bool TaskStarting()
        {
            if (!base.TaskStarting())
            {
                return false;
            }
            IvyArtifact artifact = this.package.GetArtifact(ArtifactType.Package);
            if (artifact == null)
            {
                return false;
            }
            base.HookupChildTask(new DownloaderTask(artifact.MD5Uri, base.JobId), "MD5 Downloader Task");
            base.HookupChildTask(new DownloaderTask(artifact, base.JobId), "Package Downloader Task");
            base.HookupChildTask(new BinaryVerifier(base.LocalPath, artifact), "Checksum Verifier Task");
            base.HookupChildTask(new UnzipperTask(base.LocalPath, artifact));
            base.HookupChildTask(new ZipVerifier(Path.Combine(base.LocalPath.LocalPath, artifact.Filename), Settings.installLocation));
            return true;
        }

        protected override void TaskUpdateProgress(Task task, float progress)
        {
            float num = 0f;
            float num2 = 0f;
            switch (task.Order)
            {
                case 0:
                    num = 0.01f;
                    break;

                case 1:
                    num = 0.49f;
                    num2 = 0.01f;
                    break;

                case 2:
                    num = 0.1f;
                    num2 = 0.5f;
                    break;

                case 3:
                    num = 0.25f;
                    num2 = 0.6f;
                    break;

                case 4:
                    num = 0.15f;
                    num2 = 0.85f;
                    break;
            }
            base.ProgressMessage = task.ProgressMessage;
            base.TaskUpdateProgress(task, num2 + (progress * num));
        }

        public IvyModule Package =>
            this.package;
    }
}

