namespace Unity.PackageManager
{
    using ICSharpCode.SharpZipLib.Zip;
    using Mono.Unix.Native;
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Unity.PackageManager.Ivy;

    internal class UnzipperTask : Task
    {
        private readonly IvyArtifact artifact;
        private readonly Uri basePath;

        public UnzipperTask(Uri basePath, IvyArtifact artifact)
        {
            base.Name = "Unzipper Task";
            base.ProgressMessage = "Installing";
            this.basePath = basePath;
            this.artifact = artifact;
        }

        private void ExtractZipFile(string archive, string outFolder)
        {
            <ExtractZipFile>c__AnonStorey0 storey = new <ExtractZipFile>c__AnonStorey0 {
                $this = this
            };
            ZipFile file = null;
            base.EstimatedDuration = 1L;
            DateTime now = DateTime.Now;
            int num = 0;
            storey.totalBytes = 0L;
            try
            {
                FileStream stream = File.OpenRead(archive);
                file = new ZipFile(stream);
                IEnumerator enumerator = file.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ZipEntry current = (ZipEntry) enumerator.Current;
                        <ExtractZipFile>c__AnonStorey1 storey2 = new <ExtractZipFile>c__AnonStorey1 {
                            <>f__ref$0 = storey
                        };
                        if (!current.get_IsDirectory())
                        {
                            string str = current.get_Name();
                            Stream inputStream = file.GetInputStream(current);
                            if (str.EndsWith("ivy.xml"))
                            {
                                str = str.Replace("ivy.xml", "ivy-waiting-for-unzip-to-end");
                            }
                            string path = Path.Combine(outFolder, str);
                            string directoryName = Path.GetDirectoryName(path);
                            if (directoryName.Length > 0)
                            {
                                Directory.CreateDirectory(directoryName);
                            }
                            if (((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX)) && (current.get_ExternalFileAttributes() > 0))
                            {
                                Syscall.close(Syscall.open(path, OpenFlags.O_TRUNC | OpenFlags.O_CREAT, (FilePermissions) current.get_ExternalFileAttributes()));
                            }
                            storey2.targetFile = new FileInfo(path);
                            using (FileStream stream3 = storey2.targetFile.OpenWrite())
                            {
                                Utils.Copy(inputStream, stream3, 0x1000, storey2.targetFile.Length, new Func<long, long, bool>(storey2, (IntPtr) this.<>m__0), 100);
                            }
                            storey2.targetFile.LastWriteTime = current.get_DateTime();
                            num++;
                            storey.totalBytes += current.get_Size();
                            TimeSpan span = (TimeSpan) (DateTime.Now - now);
                            double num3 = span.TotalMilliseconds / ((double) num);
                            base.EstimatedDuration = Math.Max(1L, (long) ((stream.Length - storey.totalBytes) * num3));
                            this.UpdateProgress(((float) num) / ((float) file.get_Size()));
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        protected override bool TaskRunning()
        {
            this.ExtractZipFile(Path.Combine(this.basePath.LocalPath, this.artifact.Filename), Settings.installLocation);
            return base.TaskRunning();
        }

        [CompilerGenerated]
        private sealed class <ExtractZipFile>c__AnonStorey0
        {
            internal UnzipperTask $this;
            internal long totalBytes;
        }

        [CompilerGenerated]
        private sealed class <ExtractZipFile>c__AnonStorey1
        {
            internal UnzipperTask.<ExtractZipFile>c__AnonStorey0 <>f__ref$0;
            internal FileInfo targetFile;

            internal bool <>m__0(long totalRead, long timeToFinish)
            {
                this.<>f__ref$0.$this.EstimatedDuration = timeToFinish;
                this.<>f__ref$0.$this.UpdateProgress(((float) (this.<>f__ref$0.totalBytes + totalRead)) / ((float) this.targetFile.Length));
                return !this.<>f__ref$0.$this.CancelRequested;
            }
        }
    }
}

