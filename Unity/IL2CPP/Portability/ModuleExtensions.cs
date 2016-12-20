namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class ModuleExtensions
    {
        [Extension]
        public static int GetMetadataTokenPortable(Module module)
        {
            return module.MetadataToken;
        }

        [Extension]
        public static Type[] GetTypesPortable(Module module)
        {
            return module.GetTypes();
        }
    }
}

