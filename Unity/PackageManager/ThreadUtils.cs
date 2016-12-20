namespace Unity.PackageManager
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class ThreadUtils
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static int <MainThread>k__BackingField;

        public static void SetMainThread()
        {
            MainThread = Thread.CurrentThread.ManagedThreadId;
        }

        public static bool InMainThread
        {
            get
            {
                return ((MainThread == 0) || (Thread.CurrentThread.ManagedThreadId == MainThread));
            }
        }

        public static int MainThread
        {
            [CompilerGenerated]
            get
            {
                return <MainThread>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <MainThread>k__BackingField = value;
            }
        }
    }
}

