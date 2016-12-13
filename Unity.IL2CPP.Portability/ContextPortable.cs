using System;
using System.Runtime.Remoting.Contexts;

namespace Unity.IL2CPP.Portability
{
	public static class ContextPortable
	{
		public static int DefaultContextIdPortable
		{
			get
			{
				return Context.DefaultContext.ContextID;
			}
		}
	}
}
