namespace Unity.TinyProfiling
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP.Portability;

    public class TinyProfiler
    {
        private static bool _started;
        private static NPath s_filename;
        private static string s_reportTitle;
        private static Thread s_startingThread;
        private static DateTime s_startTimeOfFirstSection;
        private static List<ThreadContext> s_threadContexts = new List<ThreadContext>();
        [ThreadStatic]
        private static Stack<int> ts_openSections;
        [ThreadStatic]
        private static List<TimedSection> ts_sections;

        public static ReadOnlyCollection<ThreadContext> CaptureSnapshot()
        {
            return new List<ThreadContext>(s_threadContexts).AsReadOnly();
        }

        private static void CloseSection(int index)
        {
            if (ts_openSections.Pop() != index)
            {
                throw new ArgumentException("TimedSection being closed is not the most recently opened");
            }
            TimedSection section = ts_sections[index];
            section.Duration = GetTimeOffset() - section.Start;
            ts_sections[index] = section;
            if ((ts_openSections.Count == 0) && (s_startingThread == Thread.CurrentThread))
            {
                Finish();
            }
        }

        public static void ConfigureOutput(NPath reportFileName, string reportTitle)
        {
            s_filename = reportFileName;
            s_reportTitle = reportTitle;
        }

        private static void Finish()
        {
            if (s_filename != null)
            {
                WriteGraph();
            }
            s_filename = null;
            _started = false;
            foreach (ThreadContext context in s_threadContexts)
            {
                context.OpenSections.Clear();
                context.Sections.Clear();
            }
        }

        public static double GetTimeForSection(string sectionLabel)
        {
            <GetTimeForSection>c__AnonStorey0 storey = new <GetTimeForSection>c__AnonStorey0 {
                sectionLabel = sectionLabel
            };
            TimedSection section = Enumerable.FirstOrDefault<TimedSection>(Enumerable.Where<TimedSection>(ts_sections, new Func<TimedSection, bool>(storey, (IntPtr) this.<>m__0)));
            if (section.Duration == 0.0)
            {
                throw new ArgumentException("TimedSection is not valid, or hasn't been closed.");
            }
            return section.Duration;
        }

        internal static double GetTimeOffset()
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - s_startTimeOfFirstSection);
            return span.TotalMilliseconds;
        }

        private static void InitializeProfilerForCurrentThread()
        {
            ts_sections = new List<TimedSection>(0x1388);
            ts_openSections = new Stack<int>(50);
            object obj2 = s_threadContexts;
            lock (obj2)
            {
                ThreadContext item = new ThreadContext {
                    OpenSections = ts_openSections,
                    Sections = ts_sections,
                    ThreadID = Thread.CurrentThread.ManagedThreadId,
                    ThreadName = Thread.CurrentThread.Name
                };
                s_threadContexts.Add(item);
            }
        }

        public static IDisposable Section(string label, [Optional, DefaultParameterValue("")] string details)
        {
            if (ts_sections == null)
            {
                InitializeProfilerForCurrentThread();
            }
            if (!_started)
            {
                Start();
            }
            int num = (ts_openSections.Count != 0) ? ts_openSections.Peek() : -1;
            TimedSection item = new TimedSection {
                Label = label,
                Details = details,
                Start = GetTimeOffset(),
                Parent = num
            };
            int count = ts_sections.Count;
            ts_sections.Add(item);
            ts_openSections.Push(count);
            return new TimedSectionHandle { index = count };
        }

        private static void Start()
        {
            _started = true;
            s_startingThread = Thread.CurrentThread;
            s_startTimeOfFirstSection = DateTime.Now;
        }

        private static void WriteGraph()
        {
            using (Stream stream = TypeExtensions.GetAssemblyPortable(typeof(TinyProfiler)).GetManifestResourceStream("TinyProfiler.SVGPan.js"))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                string[] append = new string[] { "SVGPan.js" };
                File.WriteAllBytes(s_filename.Parent.Combine(append).ToString(), buffer);
            }
            Console.WriteLine("Writing ProfileReport to: " + s_filename);
            s_filename.WriteAllText(new GraphMaker().MakeGraph(s_threadContexts, s_reportTitle));
        }

        [CompilerGenerated]
        private sealed class <GetTimeForSection>c__AnonStorey0
        {
            internal string sectionLabel;

            internal bool <>m__0(TinyProfiler.TimedSection s)
            {
                return (s.Label == this.sectionLabel);
            }
        }

        public class ThreadContext
        {
            public Stack<int> OpenSections;
            public List<TinyProfiler.TimedSection> Sections;
            public int ThreadID;
            public string ThreadName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TimedSection
        {
            public string Label;
            public string Details;
            public double Start;
            public double Duration;
            public int Parent;
            public string Summary
            {
                get
                {
                    object[] objArray1 = new object[] { this.Label, " ", this.Details, " (", this.Duration, "ms)" };
                    return string.Concat(objArray1);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TimedSectionHandle : IDisposable
        {
            internal int index;
            public void Dispose()
            {
                TinyProfiler.CloseSection(this.index);
            }
        }
    }
}

