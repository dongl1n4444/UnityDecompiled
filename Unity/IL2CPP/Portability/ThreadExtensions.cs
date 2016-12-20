namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Extension]
    public static class ThreadExtensions
    {
        [Extension]
        public static CultureInfo GetCurrentCulturePortable(Thread thread)
        {
            return thread.CurrentCulture;
        }

        [Extension]
        public static CultureInfo GetCurrentUICulturePortable(Thread thread)
        {
            return thread.CurrentUICulture;
        }

        [Extension]
        public static void InterruptPortable(Thread thread)
        {
            thread.Interrupt();
        }

        [Extension, Obsolete]
        public static void ResumePortable(Thread thread)
        {
            thread.Resume();
        }

        [Extension]
        public static void SetCurrentCulturePortable(Thread thread, CultureInfo info)
        {
            thread.CurrentCulture = info;
        }

        [Extension]
        public static void SetCurrentUICulturePortable(Thread thread, CultureInfo info)
        {
            thread.CurrentUICulture = info;
        }

        [Extension, Obsolete]
        public static void SuspendPortable(Thread thread)
        {
            thread.Suspend();
        }
    }
}

