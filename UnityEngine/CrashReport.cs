namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Holds data for a single application crash event and provides access to all gathered crash reports.</para>
    /// </summary>
    public sealed class CrashReport
    {
        [CompilerGenerated]
        private static Comparison<CrashReport> <>f__mg$cache0;
        private readonly string id;
        private static List<CrashReport> internalReports;
        private static object reportsLock = new object();
        /// <summary>
        /// <para>Crash report data as formatted text.</para>
        /// </summary>
        public readonly string text;
        /// <summary>
        /// <para>Time, when the crash occured.</para>
        /// </summary>
        public readonly DateTime time;

        private CrashReport(string id, DateTime time, string text)
        {
            this.id = id;
            this.time = time;
            this.text = text;
        }

        private static int Compare(CrashReport c1, CrashReport c2)
        {
            long ticks = c1.time.Ticks;
            long num2 = c2.time.Ticks;
            if (ticks > num2)
            {
                return 1;
            }
            if (ticks < num2)
            {
                return -1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern string GetReportData(string id, out double secondsSinceUnixEpoch);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern string[] GetReports();
        private static void PopulateReports()
        {
            object reportsLock = CrashReport.reportsLock;
            lock (reportsLock)
            {
                if (internalReports == null)
                {
                    string[] reports = GetReports();
                    internalReports = new List<CrashReport>(reports.Length);
                    foreach (string str in reports)
                    {
                        double num2;
                        string reportData = GetReportData(str, out num2);
                        DateTime time = new DateTime(0x7b2, 1, 1).AddSeconds(num2);
                        internalReports.Add(new CrashReport(str, time, reportData));
                    }
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new Comparison<CrashReport>(CrashReport.Compare);
                    }
                    internalReports.Sort(<>f__mg$cache0);
                }
            }
        }

        /// <summary>
        /// <para>Remove report from available reports list.</para>
        /// </summary>
        public void Remove()
        {
            if (RemoveReport(this.id))
            {
                object reportsLock = CrashReport.reportsLock;
                lock (reportsLock)
                {
                    internalReports.Remove(this);
                }
            }
        }

        /// <summary>
        /// <para>Remove all reports from available reports list.</para>
        /// </summary>
        public static void RemoveAll()
        {
            foreach (CrashReport report in reports)
            {
                report.Remove();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern bool RemoveReport(string id);

        /// <summary>
        /// <para>Returns last crash report, or null if no reports are available.</para>
        /// </summary>
        public static CrashReport lastReport
        {
            get
            {
                PopulateReports();
                object reportsLock = CrashReport.reportsLock;
                lock (reportsLock)
                {
                    if (internalReports.Count > 0)
                    {
                        return internalReports[internalReports.Count - 1];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// <para>Returns all currently available reports in a new array.</para>
        /// </summary>
        public static CrashReport[] reports
        {
            get
            {
                PopulateReports();
                object reportsLock = CrashReport.reportsLock;
                lock (reportsLock)
                {
                    return internalReports.ToArray();
                }
            }
        }
    }
}

