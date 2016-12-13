using System;
using System.IO;
using System.Text;

namespace Unity.IL2CPP
{
	public class InMemoryCodeWriter : CppCodeWriter
	{
		private readonly MemoryStream _memoryStream;

		public const int DefaultMemoryStreamCapacity = 4096;

		public InMemoryCodeWriter() : this(new MemoryStream(4096))
		{
		}

		private InMemoryCodeWriter(MemoryStream stream) : base(new StreamWriter(stream, new UTF8Encoding(false)))
		{
			this._memoryStream = stream;
		}

		public string GetSourceCodeString()
		{
			return Encoding.UTF8.GetString(this._memoryStream.ToArray());
		}
	}
}
