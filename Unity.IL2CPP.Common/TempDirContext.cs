using NiceIO;
using System;

namespace Unity.IL2CPP.Common
{
	public sealed class TempDirContext : IDisposable
	{
		public readonly NPath Directory;

		private readonly bool _noClean;

		internal TempDirContext(NPath directory, bool noClean)
		{
			this.Directory = directory;
			this._noClean = noClean;
		}

		public void Dispose()
		{
			if (!this._noClean && this.Directory.Exists(""))
			{
				TempDir.ForgivingCleanDirectory(this.Directory);
			}
		}
	}
}
