namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal abstract class Service : Task
    {
        private bool paused;
        private object runLock;
        private bool running;
        protected Queue<Task> tasks;

        protected Service()
        {
            this.tasks = new Queue<Task>();
            this.runLock = new object();
            base.transient = false;
            base.Name = "Service Task";
            base.ProgressMessage = "Installing";
        }

        protected Service(Guid id) : base(id)
        {
            this.tasks = new Queue<Task>();
            this.runLock = new object();
            base.transient = false;
            base.Name = "Service Task";
            base.ProgressMessage = "Installing";
        }

        public override void Cancel()
        {
            base.Cancel();
            Queue<Task> tasks = this.tasks;
            if (tasks != null)
            {
                foreach (Task task in tasks.ToArray())
                {
                    if (task != null)
                    {
                        task.Cancel();
                    }
                }
                this.paused = true;
            }
        }

        public void Cancel(bool enableResume)
        {
            this.Cancel();
            this.paused = enableResume;
        }

        public override void CleanupArtifacts()
        {
            base.CleanupArtifacts();
            if (!this.paused)
            {
                string path = Path.Combine(Settings.installLocation, "task-installer-" + base.JobId);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        protected void HookupChildTask(Task task)
        {
            this.HookupChildTask(task, null);
        }

        protected void HookupChildTask(Task task, string name)
        {
            if (name != null)
            {
                task.Name = name;
            }
            task.Order = this.tasks.Count;
            task.Restarted = base.Restarted;
            this.tasks.Enqueue(task);
        }

        public override void Run()
        {
            object runLock = this.runLock;
            lock (runLock)
            {
                this.running = true;
            }
            base.Run();
        }

        protected override void TaskFinishing(bool success)
        {
            object runLock = this.runLock;
            lock (runLock)
            {
                this.running = false;
            }
            if (!success)
            {
                base.ProgressMessage = "Installation or download error, please retry.";
            }
            base.TaskFinishing(success);
        }

        protected override bool TaskRunning()
        {
            if (!base.TaskRunning())
            {
                return false;
            }
            if (this.tasks.Count != 0)
            {
                bool cancelRequested = base.CancelRequested;
                bool isSuccessful = true;
                Task task = null;
                int num = 0x2710;
                int milliseconds = 300;
                while (((this.tasks.Count > 0) && !cancelRequested) && isSuccessful)
                {
                    long num3 = 0L;
                    long estimatedDuration = 0L;
                    isSuccessful = false;
                    if (base.CancelRequested)
                    {
                        break;
                    }
                    task = this.tasks.Dequeue();
                    Unity.PackageManager.PackageManager.Instance.FireListeners(task);
                    task.OnProgress += new TaskProgressHandler(this.TaskUpdateProgress);
                    task.Run();
                    while (!base.CancelRequested && !task.Wait(milliseconds))
                    {
                        if (base.CancelRequested)
                        {
                            task.Cancel();
                            break;
                        }
                        num3 += milliseconds;
                        estimatedDuration = task.EstimatedDuration;
                        if ((num3 > num) && (estimatedDuration <= 0L))
                        {
                            task.Cancel();
                            this.Cancel();
                        }
                    }
                    task.OnProgress -= new TaskProgressHandler(this.TaskUpdateProgress);
                    cancelRequested |= task.CancelRequested | base.CancelRequested;
                    isSuccessful = task.IsSuccessful;
                    if (!isSuccessful)
                    {
                        Console.WriteLine("Task failed: {0}", task.Name);
                    }
                }
                if (cancelRequested || !isSuccessful)
                {
                    return false;
                }
                if (task != null)
                {
                    base.Result = task.Result;
                }
            }
            return true;
        }

        protected virtual void TaskUpdateProgress(Task task, float progress)
        {
            this.UpdateProgress(progress);
        }

        public override bool IsRunning
        {
            get
            {
                object runLock = this.runLock;
                lock (runLock)
                {
                    return this.running;
                }
            }
        }
    }
}

