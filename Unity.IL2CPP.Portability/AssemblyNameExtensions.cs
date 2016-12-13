using System;
using System.Configuration.Assemblies;
using System.Globalization;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class AssemblyNameExtensions
	{
		public static CultureInfo GetCultureInfoPortable(this AssemblyName assembly)
		{
			return assembly.CultureInfo;
		}

		public static AssemblyHashAlgorithm GetHashAlgorithmPortable(this AssemblyName assembly)
		{
			return assembly.HashAlgorithm;
		}

		public static StrongNameKeyPair GetKeyPairPortable(this AssemblyName assembly)
		{
			return assembly.KeyPair;
		}
	}
}
