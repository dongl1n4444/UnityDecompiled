using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.IL2CPP
{
	internal class Profiler
	{
		private class Sampler : IDisposable
		{
			private readonly Profiler.ProfilerSample _sample = new Profiler.ProfilerSample();

			public Sampler(string name)
			{
				this._sample = new Profiler.ProfilerSample
				{
					Name = name,
					Level = Profiler._currentLevel++,
					Ticks = Profiler._stopwatch.ElapsedTicks
				};
				Profiler._data.Add(this._sample);
			}

			public void Dispose()
			{
				Profiler._currentLevel--;
				this._sample.Ticks = Profiler._stopwatch.ElapsedTicks - this._sample.Ticks;
			}
		}

		private class ProfilerSample
		{
			public string Name;

			public int Level;

			public long Ticks;
		}

		[ThreadStatic]
		private static List<Profiler.ProfilerSample> _data;

		[ThreadStatic]
		private static int _currentLevel;

		[ThreadStatic]
		private static Stopwatch _stopwatch;

		public static void Init()
		{
			Profiler._data = new List<Profiler.ProfilerSample>();
			Profiler._stopwatch = new Stopwatch();
			Profiler._stopwatch.Start();
		}

		public static IDisposable Sample(string name)
		{
			return new Profiler.Sampler(name);
		}

		public static void Dump()
		{
			foreach (Profiler.ProfilerSample current in Profiler._data)
			{
				for (int i = 0; i < current.Level; i++)
				{
					Console.Write(" ");
				}
				Console.WriteLine("{0} {1:F2} ms", current.Name, 1000.0 * (double)current.Ticks / (double)Stopwatch.Frequency);
			}
		}
	}
}
