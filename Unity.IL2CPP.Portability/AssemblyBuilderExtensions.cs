using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Unity.IL2CPP.Portability
{
	public static class AssemblyBuilderExtensions
	{
		public static void SetEntryPointForConsoleApplicationPortable(this AssemblyBuilder assemblyBuilder, MethodInfo entryMethod)
		{
			assemblyBuilder.SetEntryPoint(entryMethod, PEFileKinds.ConsoleApplication);
		}

		public static ModuleBuilder DefineDynamicModulePortable(this AssemblyBuilder assemblyBuilder, string name, string fileName)
		{
			return assemblyBuilder.DefineDynamicModule(name, fileName);
		}

		public static void SaveILCodeI386Portable(this AssemblyBuilder assemblyBuilder, string assemblyFileName)
		{
			assemblyBuilder.Save(assemblyFileName, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
		}

		public static void SaveILCodeI386Required32BitPortable(this AssemblyBuilder assemblyBuilder, string assemblyFileName)
		{
			assemblyBuilder.Save(assemblyFileName, PortableExecutableKinds.ILOnly | PortableExecutableKinds.Required32Bit, ImageFileMachine.I386);
		}
	}
}
