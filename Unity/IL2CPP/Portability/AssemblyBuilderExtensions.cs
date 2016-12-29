namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public static class AssemblyBuilderExtensions
    {
        public static ModuleBuilder DefineDynamicModulePortable(this AssemblyBuilder assemblyBuilder, string name, string fileName) => 
            assemblyBuilder.DefineDynamicModule(name, fileName);

        public static void SaveILCodeI386Portable(this AssemblyBuilder assemblyBuilder, string assemblyFileName)
        {
            assemblyBuilder.Save(assemblyFileName, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        public static void SaveILCodeI386Required32BitPortable(this AssemblyBuilder assemblyBuilder, string assemblyFileName)
        {
            assemblyBuilder.Save(assemblyFileName, PortableExecutableKinds.Required32Bit | PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        public static void SetEntryPointForConsoleApplicationPortable(this AssemblyBuilder assemblyBuilder, MethodInfo entryMethod)
        {
            assemblyBuilder.SetEntryPoint(entryMethod, PEFileKinds.ConsoleApplication);
        }
    }
}

