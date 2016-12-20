namespace Unity.IL2CPP
{
    using System;
    using System.Runtime.InteropServices;

    internal class BlockWriter : IDisposable
    {
        private readonly bool _semicolon;
        private readonly CodeWriter _writer;

        public BlockWriter(CodeWriter writer, [Optional, DefaultParameterValue(false)] bool semicolon)
        {
            this._writer = writer;
            this._semicolon = semicolon;
            writer.BeginBlock();
        }

        public void Dispose()
        {
            this._writer.EndBlock(this._semicolon);
        }
    }
}

