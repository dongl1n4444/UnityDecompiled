namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Configuration.Assemblies;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class AssemblyNameExtensions
    {
        public static CultureInfo GetCultureInfoPortable(this AssemblyName assembly) => 
            assembly.CultureInfo;

        public static AssemblyHashAlgorithm GetHashAlgorithmPortable(this AssemblyName assembly) => 
            assembly.HashAlgorithm;
    }
}

