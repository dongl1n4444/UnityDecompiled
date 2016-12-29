namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public static class ThreadExtensions
    {
        public static CultureInfo GetCurrentCulturePortable(this Thread thread) => 
            thread.CurrentCulture;

        public static CultureInfo GetCurrentUICulturePortable(this Thread thread) => 
            thread.CurrentUICulture;

        public static void InterruptPortable(this Thread thread)
        {
            thread.Interrupt();
        }

        [Obsolete]
        public static void ResumePortable(this Thread thread)
        {
            thread.Resume();
        }

        public static void SetCurrentCulturePortable(this Thread thread, CultureInfo info)
        {
            thread.CurrentCulture = info;
        }

        public static void SetCurrentUICulturePortable(this Thread thread, CultureInfo info)
        {
            thread.CurrentUICulture = info;
        }

        [Obsolete]
        public static void SuspendPortable(this Thread thread)
        {
            thread.Suspend();
        }
    }
}

