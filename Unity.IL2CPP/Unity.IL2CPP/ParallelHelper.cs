using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP
{
	public static class ParallelHelper
	{
		public static bool ForceSerialByDefault
		{
			get
			{
				return !PlatformUtils.RunningWithMono() || true;
			}
		}

		public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> action, bool loadBalance = true, bool betaTag = false)
		{
			Unity.IL2CPP.Common.ParallelHelper.ForEach<TSource>(source, action, (bool b) => CodeGenOptions.ForceSerial || (b && CodeGenOptions.ForceSerialBetaOnly), loadBalance, betaTag);
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> func, bool loadBalance = true, bool betaTag = false)
		{
			return Unity.IL2CPP.Common.ParallelHelper.Select<TSource, TResult>(source, func, (bool b) => CodeGenOptions.ForceSerial || (b && CodeGenOptions.ForceSerialBetaOnly), loadBalance, betaTag);
		}
	}
}
