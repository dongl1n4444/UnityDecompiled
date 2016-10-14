using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP
{
	internal class MethodDefinitionSignatureComparer : IEqualityComparer<MethodDefinition>
	{
		public bool Equals(MethodDefinition x, MethodDefinition y)
		{
			bool result;
			if (x.HasGenericParameters != y.HasGenericParameters)
			{
				result = false;
			}
			else if (x.FullName != y.FullName)
			{
				result = false;
			}
			else if (x.Module == y.Module)
			{
				result = true;
			}
			else if (x.Module == null || y.Module == null)
			{
				result = false;
			}
			else if (x.Module.Assembly == y.Module.Assembly)
			{
				result = true;
			}
			else
			{
				AssemblyDefinitionComparer assemblyDefinitionComparer = new AssemblyDefinitionComparer();
				result = assemblyDefinitionComparer.Equals(x.Module.Assembly, y.Module.Assembly);
			}
			return result;
		}

		public int GetHashCode(MethodDefinition obj)
		{
			return obj.FullName.GetHashCode();
		}
	}
}
