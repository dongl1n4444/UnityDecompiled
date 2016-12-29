namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public static class ParallelHelper
    {
        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> action, Func<bool, bool> shouldForceSerialCallback = null, bool loadBalance = true, bool betaTag = false)
        {
            if ((shouldForceSerialCallback != null) && shouldForceSerialCallback.Invoke(betaTag))
            {
                foreach (TSource local in source)
                {
                    action(local);
                }
            }
            else
            {
                ForEachParallel<TSource>(source, action, loadBalance);
            }
        }

        private static void ForEachParallel<TSource>(IEnumerable<TSource> source, Action<TSource> action, bool loadBalance = true)
        {
            if (loadBalance)
            {
                Parallel.ForEach<TSource>(Partitioner.Create<TSource>(source.ToList<TSource>(), true), action);
            }
            else
            {
                Parallel.ForEach<TSource>(source, action);
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> func, Func<bool, bool> shouldForceSerialCallback = null, bool loadBalance = true, bool betaTag = false)
        {
            if ((shouldForceSerialCallback != null) && shouldForceSerialCallback.Invoke(betaTag))
            {
                return source.Select<TSource, TResult>(func);
            }
            return SelectParallel<TSource, TResult>(source, func, loadBalance);
        }

        private static IEnumerable<TResult> SelectParallel<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> func, bool loadBalance)
        {
            <SelectParallel>c__AnonStorey0<TSource, TResult> storey = new <SelectParallel>c__AnonStorey0<TSource, TResult> {
                func = func,
                results = new List<TResult>()
            };
            if (loadBalance)
            {
                Parallel.ForEach<TSource>(Partitioner.Create<TSource>(source.ToList<TSource>(), true), new Action<TSource>(storey.<>m__0));
            }
            else
            {
                Parallel.ForEach<TSource>(source, new Action<TSource>(storey.<>m__1));
            }
            return storey.results;
        }

        [CompilerGenerated]
        private sealed class <SelectParallel>c__AnonStorey0<TSource, TResult>
        {
            internal Func<TSource, TResult> func;
            internal List<TResult> results;

            internal void <>m__0(TSource item)
            {
                TResult local = this.func.Invoke(item);
                object results = this.results;
                lock (results)
                {
                    this.results.Add(local);
                }
            }

            internal void <>m__1(TSource item)
            {
                TResult local = this.func.Invoke(item);
                object results = this.results;
                lock (results)
                {
                    this.results.Add(local);
                }
            }
        }
    }
}

