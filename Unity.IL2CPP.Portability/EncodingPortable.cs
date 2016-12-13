using System;
using System.Text;

namespace Unity.IL2CPP.Portability
{
	public static class EncodingPortable
	{
		public static Encoding GetDefaultPortable()
		{
			return Encoding.Default;
		}
	}
}
