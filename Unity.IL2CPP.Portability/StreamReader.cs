using System;
using System.IO;

namespace Unity.IL2CPP.Portability
{
	public class StreamReader : System.IO.StreamReader
	{
		public StreamReader(string path) : base(path)
		{
		}
	}
}
