namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ModuleExtensions
    {
        public static int GetMetadataTokenPortable(this Module module) => 
            module.MetadataToken;

        public static Type[] GetTypesPortable(this Module module) => 
            module.GetTypes();
    }
}

