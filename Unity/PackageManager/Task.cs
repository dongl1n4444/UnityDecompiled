namespace Unity.PackageManager
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Task : IDisposable, IAsyncResult
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WaitHandle <AsyncWaitHandle>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long <EstimatedDuration>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsCompleted>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsRunning>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsSuccessful>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <Order>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <Progress>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <ProgressMessage>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <Restarted>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object <Result>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <Shared>k__BackingField;
        private bool cancelRequested;
        private Guid jobId;
        private Uri localPath;
        private string name;
        private Action<Task, bool> taskFinishing;
        private Func<Task, bool> taskRunning;
        private Action<Task> taskStarting;
        protected bool transient;

        public event TaskFinishedHandler OnFinish;

        public event TaskProgressHandler OnProgress;

        public event TaskStartedHandler OnStart;

        protected Task()
        {
            this.name = "Generic Task";
            this.jobId = Guid.Empty;
            this.cancelRequested = false;
            this.transient = true;
            this.AsyncWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            this.ProgressMessage = "";
        }

        public Task(Guid id) : this()
        {
            this.jobId = id;
            this.Shared = true;
        }

        public Task(Action<Task> taskStarting, Func<Task, bool> taskRunning, Action<Task, bool> taskFinishing) : this()
        {
            this.taskStarting = taskStarting;
            this.taskRunning = taskRunning;
            this.taskFinishing = taskFinishing;
        }

        public virtual void Cancel()
        {
            this.cancelRequested = true;
            if (this.IsCompleted)
            {
                this.InvokeOnFinish(this, this.IsSuccessful);
            }
        }

        public virtual void CleanupArtifacts()
        {
            if (!this.Shared)
            {
                try
                {
                    if ((this.localPath != null) && Directory.Exists(this.localPath.LocalPath))
                    {
                        Directory.Delete(this.localPath.LocalPath, true);
                    }
                    this.localPath = null;
                }
                catch
                {
                    Console.WriteLine("Unable to clean up artifacts at {0}", this.localPath);
                }
            }
        }

        private void CleanupHandlers()
        {
            this.OnStart = null;
            this.OnProgress = null;
            this.OnFinish = null;
            this.taskStarting = null;
            this.taskRunning = null;
            this.taskFinishing = null;
        }

        public void Dispose()
        {
            this.CleanupHandlers();
        }

        protected void InvokeOnFinish(Task task, bool success)
        {
            if (this.OnFinish != null)
            {
                this.OnFinish(task, success);
            }
        }

        protected void InvokeOnProgress(Task task, float progress)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(task, progress);
            }
        }

        protected void InvokeOnStart(Task task)
        {
            if (this.OnStart != null)
            {
                this.OnStart(task);
            }
        }

        public virtual void Run()
        {
            this.IsRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunTask));
        }

        private void RunTask(object state)
        {
            bool success = false;
            try
            {
                if (!this.cancelRequested)
                {
                    success = this.TaskStarting();
                }
                if (!this.cancelRequested && success)
                {
                    success = this.TaskRunning();
                }
                if (this.cancelRequested)
                {
                    success = false;
                }
                this.TaskFinishing(success);
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Cancelling tasks, domain is going down");
                success = false;
            }
            catch (Exception exception)
            {
                Exception innerException = exception.InnerException;
                while ((innerException != null) && !(innerException is ThreadAbortException))
                {
                    innerException = innerException.InnerException;
                }
                if ((innerException != null) && (innerException is ThreadAbortException))
                {
                    Console.WriteLine("Cancelling tasks, domain is going down");
                }
                else
                {
                    Console.WriteLine("Aborting task {0}.{1}{2}", this.Name, Environment.NewLine, exception);
                }
            }
            this.IsSuccessful = success;
            this.IsCompleted = true;
            ((EventWaitHandle) this.AsyncWaitHandle).Set();
            if (this.transient)
            {
                this.CleanupHandlers();
            }
            this.CleanupArtifacts();
        }

        public virtual void Stop()
        {
            this.Stop(false);
        }

        public virtual void Stop(bool wait)
        {
            <Stop>c__AnonStorey0 storey = new <Stop>c__AnonStorey0();
            if (!this.IsRunning)
            {
                this.CleanupHandlers();
            }
            else
            {
                this.transient = true;
                Console.WriteLine("Shutting down " + this.name);
                this.Cancel();
                storey.signal = null;
                if (wait)
                {
                    storey.signal = new ManualResetEvent(false);
                    if (this.IsCompleted)
                    {
                        storey.signal.Set();
                    }
                    else
                    {
                        this.OnFinish += new TaskFinishedHandler(storey.<>m__0);
                    }
                    if (!storey.signal.WaitOne(500))
                    {
                        this.CleanupHandlers();
                        Console.WriteLine("{0} stopped.", this.Name);
                    }
                }
            }
        }

        protected virtual void TaskFinishing(bool success)
        {
            if (this.taskFinishing != null)
            {
                this.taskFinishing.Invoke(this, success);
            }
            this.InvokeOnFinish(this, success);
        }

        protected virtual bool TaskRunning()
        {
            if (this.taskRunning != null)
            {
                return this.taskRunning.Invoke(this);
            }
            return true;
        }

        protected virtual bool TaskStarting()
        {
            if (this.cancelRequested)
            {
                return false;
            }
            if (this.taskStarting != null)
            {
                this.taskStarting(this);
            }
            if (this.cancelRequested)
            {
                return false;
            }
            this.InvokeOnStart(this);
            if (this.cancelRequested)
            {
                return false;
            }
            return true;
        }

        protected virtual void UpdateProgress(float progress)
        {
            this.Progress = progress;
            this.InvokeOnProgress(this, progress);
        }

        public bool Wait(int milliseconds) => 
            this.AsyncWaitHandle.WaitOne(milliseconds);

        public WaitHandle AsyncWaitHandle { get; private set; }

        public bool CancelRequested =>
            this.cancelRequested;

        public long EstimatedDuration { get; protected set; }

        public bool IsCompleted { get; private set; }

        public bool IsRunning { virtual get; private set; }

        public bool IsSuccessful { get; private set; }

        public Guid JobId
        {
            get
            {
                if (this.jobId == Guid.Empty)
                {
                    this.jobId = Guid.NewGuid();
                }
                return this.jobId;
            }
            set
            {
                this.jobId = value;
            }
        }

        protected Uri LocalPath
        {
            get
            {
                if (this.localPath == null)
                {
                    this.localPath = new Uri(Path.Combine(Settings.downloadLocation, this.JobId.ToString()));
                    if (!Directory.Exists(this.localPath.LocalPath))
                    {
                        Directory.CreateDirectory(this.localPath.LocalPath);
                    }
                }
                return this.localPath;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal int Order { get; set; }

        public float Progress { get; protected set; }

        public string ProgressMessage { get; protected set; }

        public bool Restarted { get; set; }

        public object Result { get; set; }

        public bool Shared { get; set; }

        object IAsyncResult.AsyncState
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [CompilerGenerated]
        private sealed class <Stop>c__AnonStorey0
        {
            internal ManualResetEvent signal;

            internal void <>m__0(Task t, bool s)
            {
                this.signal.Set();
            }
        }
    }
}

