namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class AssemblyBuilderExtensions
    {
        [Extension]
        public static ModuleBuilder DefineDynamicModulePortable(AssemblyBuilder assemblyBuilder, string name, string fileName)
        {
            return assemblyBuilder.DefineDynamicModule(name, fileName);
        }

        [Extension]
        public static void SaveILCodeI386Portable(AssemblyBuilder assemblyBuilder, string assemblyFileName)
        {
            assemblyBuilder.Save(assemblyFileName, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        [Extension]
        public static void SaveILCodeI386Required32BitPortable(AssemblyBuilder assemblyBuilder, string assemblyFileName)
        {
            assemblyBuilder.Save(assemblyFileName, PortableExecutableKinds.Required32Bit | PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        [Extension]
        public static void SetEntryPointForConsoleApplicationPortable(AssemblyBuilder assemblyBuilder, MethodInfo entryMethod)
        {
            assemblyBuilder.SetEntryPoint(entryMethod, PEFileKinds.ConsoleApplication);
        }
    }
}

