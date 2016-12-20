namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Configuration.Assemblies;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class AssemblyNameExtensions
    {
        [Extension]
        public static CultureInfo GetCultureInfoPortable(AssemblyName assembly)
        {
            return assembly.CultureInfo;
        }

        [Extension]
        public static AssemblyHashAlgorithm GetHashAlgorithmPortable(AssemblyName assembly)
        {
            return assembly.HashAlgorithm;
        }

        [Extension]
        public static StrongNameKeyPair GetKeyPairPortable(AssemblyName assembly)
        {
            return assembly.KeyPair;
        }
    }
}

