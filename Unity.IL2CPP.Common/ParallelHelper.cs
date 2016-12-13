using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unity.IL2CPP.Common
{
	public static class ParallelHelper
	{
		public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> action, Func<bool, bool> shouldForceSerialCallback = null, bool loadBalance = true, bool betaTag = false)
		{
			if (shouldForceSerialCallback != null && shouldForceSerialCallback(betaTag))
			{
				foreach (TSource current in source)
				{
					action(current);
				}
			}
			else
			{
				ParallelHelper.ForEachParallel<TSource>(source, action, loadBalance);
			}
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> func, Func<bool, bool> shouldForceSerialCallback = null, bool loadBalance = true, bool betaTag = false)
		{
			IEnumerable<TResult> result;
			if (shouldForceSerialCallback != null && shouldForceSerialCallback(betaTag))
			{
				result = source.Select(func);
			}
			else
			{
				result = ParallelHelper.SelectParallel<TSource, TResult>(source, func, loadBalance);
			}
			return result;
		}

		private static void ForEachParallel<TSource>(IEnumerable<TSource> source, Action<TSource> action, bool loadBalance = true)
		{
			if (loadBalance)
			{
				OrderablePartitioner<TSource> source2 = Partitioner.Create<TSource>(source.ToList<TSource>(), true);
				Parallel.ForEach<TSource>(source2, action);
			}
			else
			{
				Parallel.ForEach<TSource>(source, action);
			}
		}

		private static IEnumerable<TResult> SelectParallel<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> func, bool loadBalance)
		{
			List<TResult> results = new List<TResult>();
			if (loadBalance)
			{
				OrderablePartitioner<TSource> source2 = Partitioner.Create<TSource>(source.ToList<TSource>(), true);
				Parallel.ForEach<TSource>(source2, delegate(TSource item)
				{
					TResult item2 = func(item);
					object results = results;
					lock (results)
					{
						results.Add(item2);
					}
				});
			}
			else
			{
				Parallel.ForEach<TSource>(source, delegate(TSource item)
				{
					TResult item2 = func(item);
					object results = results;
					lock (results)
					{
						results.Add(item2);
					}
				});
			}
			return results;
		}
	}
}
