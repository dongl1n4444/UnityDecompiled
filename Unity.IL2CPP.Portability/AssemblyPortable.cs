using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class AssemblyPortable
	{
		public static Assembly GetExecutingAssemblyPortable()
		{
			return Assembly.GetCallingAssembly();
		}

		public static Assembly GetAssemblyPortable(Type type)
		{
			return type.GetAssemblyPortable();
		}

		public static Assembly LoadPortable(string partialName)
		{
			return Assembly.Load(partialName);
		}

		public static Assembly LoadFilePortable(string path)
		{
			return Assembly.LoadFile(path);
		}

		public static Assembly LoadFromPortable(string assemblyFile)
		{
			return Assembly.LoadFrom(assemblyFile);
		}
	}
}
