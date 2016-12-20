namespace Unity.IL2CPP
{
    using System;
    using System.IO;
    using System.Text;

    public class InMemoryCodeWriter : CppCodeWriter
    {
        private readonly MemoryStream _memoryStream;
        public const int DefaultMemoryStreamCapacity = 0x1000;

        public InMemoryCodeWriter() : this(new MemoryStream(0x1000))
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

