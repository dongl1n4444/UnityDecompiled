using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Unity.IL2CPP.Portability
{
	public static class AppDomainPortable
	{
		public static IEnumerable<Assembly> GetAllAssembliesInCurrentAppDomainPortable()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public static Assembly CurrentDomainLoadPortable(string assemblyString)
		{
			return AppDomain.CurrentDomain.Load(assemblyString);
		}

		public static string GetCurrentDomainFriendlyNamePortable()
		{
			return AppDomain.CurrentDomain.FriendlyName;
		}

		public static AssemblyBuilder DefineDynamicAssemblyPortable(AssemblyName assemblyName, AssemblyBuilderAccess access, string dir)
		{
			return AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, access, dir);
		}
	}
}
