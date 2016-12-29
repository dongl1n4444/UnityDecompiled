namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class Profiler
    {
        [ThreadStatic]
        private static int _currentLevel;
        [ThreadStatic]
        private static List<ProfilerSample> _data;
        [ThreadStatic]
        private static Stopwatch _stopwatch;

        public static void Dump()
        {
            foreach (ProfilerSample sample in _data)
            {
                for (int i = 0; i < sample.Level; i++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("{0} {1:F2} ms", sample.Name, (1000.0 * sample.Ticks) / ((double) Stopwatch.Frequency));
            }
        }

        public static void Init()
        {
            _data = new List<ProfilerSample>();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public static IDisposable Sample(string name) => 
            new Sampler(name);

        private class ProfilerSample
        {
            public int Level;
            public string Name;
            public long Ticks;
        }

        private class Sampler : IDisposable
        {
            private readonly Profiler.ProfilerSample _sample = new Profiler.ProfilerSample();

            public Sampler(string name)
            {
                Profiler.ProfilerSample sample = new Profiler.ProfilerSample {
                    Name = name,
                    Level = Profiler._currentLevel++,
                    Ticks = Profiler._stopwatch.ElapsedTicks
                };
                this._sample = sample;
                Profiler._data.Add(this._sample);
            }

            public void Dispose()
            {
                Profiler._currentLevel--;
                this._sample.Ticks = Profiler._stopwatch.ElapsedTicks - this._sample.Ticks;
            }
        }
    }
}

