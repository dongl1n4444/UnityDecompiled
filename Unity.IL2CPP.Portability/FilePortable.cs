using System;
using System.IO;

namespace Unity.IL2CPP.Portability
{
	public static class FilePortable
	{
		public static void ReplacePortable(string sourceFileName, string destinationFileName, string destinationBackupFileName)
		{
			File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
		}
	}
}
