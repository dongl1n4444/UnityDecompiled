using System;
using System.Globalization;
using System.Threading;

namespace Unity.IL2CPP.Portability
{
	public static class ThreadExtensions
	{
		[Obsolete]
		public static void SuspendPortable(this Thread thread)
		{
			thread.Suspend();
		}

		[Obsolete]
		public static void ResumePortable(this Thread thread)
		{
			thread.Resume();
		}

		public static void InterruptPortable(this Thread thread)
		{
			thread.Interrupt();
		}

		public static CultureInfo GetCurrentCulturePortable(this Thread thread)
		{
			return thread.CurrentCulture;
		}

		public static CultureInfo GetCurrentUICulturePortable(this Thread thread)
		{
			return thread.CurrentUICulture;
		}

		public static void SetCurrentCulturePortable(this Thread thread, CultureInfo info)
		{
			thread.CurrentCulture = info;
		}

		public static void SetCurrentUICulturePortable(this Thread thread, CultureInfo info)
		{
			thread.CurrentUICulture = info;
		}
	}
}
