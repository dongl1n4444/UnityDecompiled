using System;
using System.IO;
using System.Reflection;

namespace Unity.IL2CPP.Common
{
	public static class ResourceExtraction
	{
		public static byte[] Extract(string filename, Assembly assembly)
		{
			byte[] result;
			using (Stream manifestResourceStream = assembly.GetManifestResourceStream(filename))
			{
				if (manifestResourceStream == null)
				{
					result = null;
				}
				else
				{
					byte[] array = new byte[manifestResourceStream.Length];
					manifestResourceStream.Read(array, 0, array.Length);
					result = array;
				}
			}
			return result;
		}
	}
}
