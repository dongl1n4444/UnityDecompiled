using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class AssemblyExtensions
	{
		public static bool InGlobalAssemblyCachePortable(this Assembly assembly)
		{
			return assembly.GlobalAssemblyCache;
		}

		public static string GetCodeBasePortable(this Assembly assembly)
		{
			return assembly.CodeBase;
		}

		public static AssemblyName[] GetReferencedAssembliesPortable(this Assembly assembly)
		{
			return assembly.GetReferencedAssemblies();
		}

		public static Module[] GetModulesPortable(this Assembly assembly)
		{
			return assembly.GetModules();
		}

		public static Type[] GetTypesPortable(this Assembly assembly)
		{
			return assembly.GetTypes();
		}

		public static Type[] GetExportedTypesPortable(this Assembly assembly)
		{
			return assembly.GetExportedTypes();
		}

		public static object[] GetCustomAttributesPortable(this Assembly assembly, Type attributeType, bool inherit)
		{
			return assembly.GetCustomAttributes(attributeType, inherit);
		}
	}
}
