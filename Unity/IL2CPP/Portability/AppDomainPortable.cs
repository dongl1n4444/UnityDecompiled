namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class AppDomainPortable
    {
        public static Assembly CurrentDomainLoadPortable(string assemblyString) => 
            AppDomain.CurrentDomain.Load(assemblyString);

        public static AssemblyBuilder DefineDynamicAssemblyPortable(AssemblyName assemblyName, AssemblyBuilderAccess access, string dir) => 
            AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, access, dir);

        public static IEnumerable<Assembly> GetAllAssembliesInCurrentAppDomainPortable() => 
            AppDomain.CurrentDomain.GetAssemblies();

        public static string GetCurrentDomainFriendlyNamePortable() => 
            AppDomain.CurrentDomain.FriendlyName;
    }
}

