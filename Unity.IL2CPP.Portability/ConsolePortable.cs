using System;
using System.Text;

namespace Unity.IL2CPP.Portability
{
	public static class ConsolePortable
	{
		public static void SetInputEncodingPortable(Encoding encoding)
		{
			Console.InputEncoding = encoding;
		}

		public static void SetOutputEncodingPortable(Encoding encoding)
		{
			Console.OutputEncoding = encoding;
		}
	}
}
