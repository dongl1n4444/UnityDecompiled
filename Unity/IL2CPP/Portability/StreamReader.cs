namespace Unity.IL2CPP.Portability
{
    using System;
    using System.IO;

    public class StreamReader : System.IO.StreamReader
    {
        public StreamReader(string path) : base(path)
        {
        }
    }
}

