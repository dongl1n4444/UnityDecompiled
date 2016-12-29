namespace Unity.PackageManager
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using Unity.PackageManager.Ivy;

    internal class DownloaderTask : Task
    {
        private long bytes;
        private WebRequest request;
        private string streamName;
        private readonly Uri uri;

        public DownloaderTask(Uri uri)
        {
            base.Name = "Downloader Task";
            base.ProgressMessage = "Downloading";
            string str = uri.Segments[uri.Segments.Length - 1];
            if (str == "/")
            {
                str = "temp";
            }
            this.streamName = Path.Combine(base.LocalPath.LocalPath, str);
            this.uri = uri;
        }

        public DownloaderTask(IvyArtifact artifact)
        {
            base.Name = "Downloader Task";
            base.ProgressMessage = "Downloading";
            this.streamName = Path.Combine(base.LocalPath.LocalPath, artifact.Filename);
            this.uri = artifact.Url;
        }

        public DownloaderTask(Uri uri, Guid id) : base(id)
        {
            base.Name = "Downloader Task";
            base.ProgressMessage = "Downloading";
            string str = uri.Segments[uri.Segments.Length - 1];
            if (str == "/")
            {
                str = "temp";
            }
            this.streamName = Path.Combine(base.LocalPath.LocalPath, str);
            this.uri = uri;
        }

        public DownloaderTask(IvyArtifact artifact, Guid id) : base(id)
        {
            base.Name = "Downloader Task";
            base.ProgressMessage = "Downloading";
            this.streamName = Path.Combine(base.LocalPath.LocalPath, artifact.Filename);
            this.uri = artifact.Url;
        }

        protected override void TaskFinishing(bool success)
        {
            base.TaskFinishing(success);
        }

        protected override bool TaskRunning()
        {
            bool flag;
            if (base.Restarted && (this.bytes > 0L))
            {
                Console.WriteLine("Resuming download of {0} to {1}", this.uri, this.streamName);
            }
            else
            {
                Console.WriteLine("Downloading {0} to {1}", this.uri, this.streamName);
            }
            base.Result = this.streamName;
            using (WebResponse response = this.request.GetResponseWithoutException())
            {
                <TaskRunning>c__AnonStorey0 storey = new <TaskRunning>c__AnonStorey0 {
                    $this = this
                };
                if (response == null)
                {
                    return false;
                }
                if ((base.Restarted && (this.bytes > 0L)) && (response is HttpWebResponse))
                {
                    if (((HttpWebResponse) response).StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                    {
                        this.UpdateProgress(1f);
                        return true;
                    }
                    if (((HttpWebResponse) response).StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }
                }
                storey.respSize = response.ContentLength;
                if (base.Restarted && (this.bytes > 0L))
                {
                    this.UpdateProgress((float) (this.bytes / storey.respSize));
                    if (this.bytes == storey.respSize)
                    {
                        return true;
                    }
                }
                using (Stream stream = response.GetResponseStream())
                {
                    using (Stream stream2 = new FileStream(this.streamName, FileMode.Append))
                    {
                        if (base.CancelRequested)
                        {
                            return false;
                        }
                        flag = Utils.Copy(stream, stream2, 0x2000, storey.respSize, new Func<long, long, bool>(storey, (IntPtr) this.<>m__0), 100);
                    }
                }
            }
            return flag;
        }

        protected override bool TaskStarting()
        {
            if (!base.TaskStarting())
            {
                return false;
            }
            if (base.Restarted)
            {
                FileInfo info = new FileInfo(Path.Combine(base.LocalPath.LocalPath, this.streamName));
                if (info.Exists && (info.Length > 0L))
                {
                    this.bytes = info.Length;
                }
                else if (info.Length == 0L)
                {
                    info.Delete();
                }
            }
            this.request = WebRequest.Create(this.uri);
            if (this.request is HttpWebRequest)
            {
                ((HttpWebRequest) this.request).UserAgent = "Unity PackageManager v" + Unity.PackageManager.PackageManager.Instance.Version;
                if (this.bytes > 0L)
                {
                    ((HttpWebRequest) this.request).AddRange((int) this.bytes);
                }
            }
            this.request.Method = "GET";
            this.request.Timeout = 0xbb8;
            return true;
        }

        [CompilerGenerated]
        private sealed class <TaskRunning>c__AnonStorey0
        {
            internal DownloaderTask $this;
            internal long respSize;

            internal bool <>m__0(long totalRead, long timeToFinish)
            {
                this.$this.EstimatedDuration = timeToFinish;
                this.$this.UpdateProgress(((float) totalRead) / ((float) this.respSize));
                return !this.$this.CancelRequested;
            }
        }
    }
}

