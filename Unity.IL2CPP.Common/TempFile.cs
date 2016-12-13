using NiceIO;
using System;
using System.IO;

namespace Unity.IL2CPP.Common
{
	public sealed class TempFile : IDisposable
	{
		public readonly NPath Path;

		private TempFile(NPath path)
		{
			this.Path = path;
		}

		public static TempFile CreateRandom()
		{
			NPath nPath = TempDir.Il2CppTemporaryDirectoryRoot.Combine(new string[]
			{
				System.IO.Path.GetRandomFileName()
			});
			while (nPath.Exists(""))
			{
				nPath = TempDir.Il2CppTemporaryDirectoryRoot.Combine(new string[]
				{
					System.IO.Path.GetRandomFileName()
				});
			}
			return new TempFile(nPath);
		}

		public void Dispose()
		{
			if (this.Path.Exists(""))
			{
				this.Path.Delete(DeleteMode.Normal);
			}
		}
	}
}
