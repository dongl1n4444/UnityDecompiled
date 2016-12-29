namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class AssemblyExtensions
    {
        public static string GetCodeBasePortable(this Assembly assembly) => 
            assembly.CodeBase;

        public static object[] GetCustomAttributesPortable(this Assembly assembly, Type attributeType, bool inherit) => 
            assembly.GetCustomAttributes(attributeType, inherit);

        public static Type[] GetExportedTypesPortable(this Assembly assembly) => 
            assembly.GetExportedTypes();

        public static Module[] GetModulesPortable(this Assembly assembly) => 
            assembly.GetModules();

        public static AssemblyName[] GetReferencedAssembliesPortable(this Assembly assembly) => 
            assembly.GetReferencedAssemblies();

        public static Type[] GetTypesPortable(this Assembly assembly) => 
            assembly.GetTypes();

        public static bool InGlobalAssemblyCachePortable(this Assembly assembly) => 
            assembly.GlobalAssemblyCache;
    }
}

