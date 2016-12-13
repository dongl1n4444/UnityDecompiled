using System;
using System.IO;
using System.Text;

namespace Unity.IL2CPP.Portability
{
	public class StreamWriter : System.IO.StreamWriter
	{
		public StreamWriter(string path, Encoding encoding) : base(path, false, encoding)
		{
		}

		public StreamWriter(string path) : base(path)
		{
		}
	}
}
