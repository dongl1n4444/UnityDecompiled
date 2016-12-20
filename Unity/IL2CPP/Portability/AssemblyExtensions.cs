namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class AssemblyExtensions
    {
        [Extension]
        public static string GetCodeBasePortable(Assembly assembly)
        {
            return assembly.CodeBase;
        }

        [Extension]
        public static object[] GetCustomAttributesPortable(Assembly assembly, Type attributeType, bool inherit)
        {
            return assembly.GetCustomAttributes(attributeType, inherit);
        }

        [Extension]
        public static Type[] GetExportedTypesPortable(Assembly assembly)
        {
            return assembly.GetExportedTypes();
        }

        [Extension]
        public static Module[] GetModulesPortable(Assembly assembly)
        {
            return assembly.GetModules();
        }

        [Extension]
        public static AssemblyName[] GetReferencedAssembliesPortable(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies();
        }

        [Extension]
        public static Type[] GetTypesPortable(Assembly assembly)
        {
            return assembly.GetTypes();
        }

        [Extension]
        public static bool InGlobalAssemblyCachePortable(Assembly assembly)
        {
            return assembly.GlobalAssemblyCache;
        }
    }
}

