namespace Unity.IL2CPP.Building
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class ParallelFor
    {
        public static void Run<D>(D[] data, Action<D> action)
        {
            <Run>c__AnonStorey0<D> storey = new <Run>c__AnonStorey0<D> {
                data = data,
                action = action,
                nextJobToTake = 0
            };
            List<Thread> list = new List<Thread>();
            storey.exceptions = new List<Exception>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                list.Add(new Thread(new ParameterizedThreadStart(storey.<>m__0)));
            }
            foreach (Thread thread in list)
            {
                thread.Start();
            }
            foreach (Thread thread2 in list)
            {
                thread2.Join();
            }
            object exceptions = storey.exceptions;
            lock (exceptions)
            {
                if (storey.exceptions.Count > 0)
                {
                    throw new AggregateException(storey.exceptions);
                }
            }
        }

        public static IEnumerable<T> RunWithResult<D, T>(D[] data, Func<D, T> action)
        {
            <RunWithResult>c__AnonStorey1<D, T> storey = new <RunWithResult>c__AnonStorey1<D, T> {
                data = data,
                action = action,
                nextJobToTake = 0
            };
            storey.results = new T[storey.data.Length];
            storey.exceptions = new List<Exception>();
            List<Thread> list = new List<Thread>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                list.Add(new Thread(new ParameterizedThreadStart(storey.<>m__0)));
            }
            foreach (Thread thread in list)
            {
                thread.Start();
            }
            foreach (Thread thread2 in list)
            {
                thread2.Join();
            }
            object exceptions = storey.exceptions;
            lock (exceptions)
            {
                if (storey.exceptions.Count > 0)
                {
                    throw new AggregateException(storey.exceptions);
                }
            }
            return storey.results;
        }

        [CompilerGenerated]
        private sealed class <Run>c__AnonStorey0<D>
        {
            internal Action<D> action;
            internal D[] data;
            internal List<Exception> exceptions;
            internal int nextJobToTake;

            internal void <>m__0(object o)
            {
                try
                {
                    int num;
                Label_0002:
                    num = Interlocked.Increment(ref this.nextJobToTake) - 1;
                    if (num < this.data.Length)
                    {
                        this.action(this.data[num]);
                        goto Label_0002;
                    }
                }
                catch (Exception exception)
                {
                    object exceptions = this.exceptions;
                    lock (exceptions)
                    {
                        this.exceptions.Add(exception);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RunWithResult>c__AnonStorey1<D, T>
        {
            internal Func<D, T> action;
            internal D[] data;
            internal List<Exception> exceptions;
            internal int nextJobToTake;
            internal T[] results;

            internal void <>m__0(object o)
            {
                try
                {
                    while (true)
                    {
                        int index = Interlocked.Increment(ref this.nextJobToTake) - 1;
                        if (index >= this.data.Length)
                        {
                            return;
                        }
                        D arg = this.data[index];
                        T local2 = this.action(arg);
                        object results = this.results;
                        lock (results)
                        {
                            this.results[index] = local2;
                        }
                    }
                }
                catch (Exception exception)
                {
                    object exceptions = this.exceptions;
                    lock (exceptions)
                    {
                        this.exceptions.Add(exception);
                    }
                }
            }
        }
    }
}

