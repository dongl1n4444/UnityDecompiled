using System;

namespace Unity.IL2CPP
{
	internal class BlockWriter : IDisposable
	{
		private readonly CodeWriter _writer;

		private readonly bool _semicolon;

		public BlockWriter(CodeWriter writer, bool semicolon = false)
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
