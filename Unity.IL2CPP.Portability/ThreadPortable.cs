using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;

namespace Unity.IL2CPP.Portability
{
	public static class ThreadPortable
	{
		public static int CurrentContextIdPortable
		{
			get
			{
				return Thread.CurrentContext.ContextID;
			}
		}

		public static bool HasCurrentContext
		{
			get
			{
				return Thread.CurrentContext != null;
			}
		}

		public static Context CurrentContext
		{
			get
			{
				return Thread.CurrentContext;
			}
		}
	}
}
