using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class ModuleExtensions
	{
		public static Type[] GetTypesPortable(this Module module)
		{
			return module.GetTypes();
		}

		public static int GetMetadataTokenPortable(this Module module)
		{
			return module.MetadataToken;
		}
	}
}
