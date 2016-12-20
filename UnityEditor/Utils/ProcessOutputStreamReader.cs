namespace UnityEditor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class ProcessOutputStreamReader
    {
        private readonly Func<bool> hostProcessExited;
        internal List<string> lines;
        private readonly StreamReader stream;
        private Thread thread;

        internal ProcessOutputStreamReader(Process p, StreamReader stream) : this(new Func<bool>(storey, (IntPtr) this.<>m__0), stream)
        {
            <ProcessOutputStreamReader>c__AnonStorey0 storey = new <ProcessOutputStreamReader>c__AnonStorey0 {
                p = p
            };
        }

        internal ProcessOutputStreamReader(Func<bool> hostProcessExited, StreamReader stream)
        {
            this.hostProcessExited = hostProcessExited;
            this.stream = stream;
            this.lines = new List<string>();
            this.thread = new Thread(new ThreadStart(this.ThreadFunc));
            this.thread.Start();
        }

        internal string[] GetOutput()
        {
            if (this.hostProcessExited.Invoke())
            {
                this.thread.Join();
            }
            object lines = this.lines;
            lock (lines)
            {
                return this.lines.ToArray();
            }
        }

        private void ThreadFunc()
        {
            if (!this.hostProcessExited.Invoke())
            {
                while (true)
                {
                    if (this.stream.BaseStream == null)
                    {
                        break;
                    }
                    string item = this.stream.ReadLine();
                    if (item == null)
                    {
                        break;
                    }
                    object lines = this.lines;
                    lock (lines)
                    {
                        this.lines.Add(item);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessOutputStreamReader>c__AnonStorey0
        {
            internal Process p;

            internal bool <>m__0()
            {
                return this.p.HasExited;
            }
        }
    }
}

