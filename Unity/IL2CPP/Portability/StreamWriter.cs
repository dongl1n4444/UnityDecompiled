namespace Unity.IL2CPP.Portability
{
    using System;
    using System.IO;
    using System.Text;

    public class StreamWriter : System.IO.StreamWriter
    {
        public StreamWriter(string path) : base(path)
        {
        }

        public StreamWriter(string path, bool append) : base(path, append)
        {
        }

        public StreamWriter(string path, Encoding encoding) : base(path, false, encoding)
        {
        }
    }
}

