namespace Unity.IL2CPP
{
    using System;

    internal class DedentWriter : IDisposable
    {
        private readonly CodeWriter _writer;

        public DedentWriter(CodeWriter writer)
        {
            this._writer = writer;
            writer.Dedent(1);
        }

        public void Dispose()
        {
            this._writer.Indent(1);
        }
    }
}

