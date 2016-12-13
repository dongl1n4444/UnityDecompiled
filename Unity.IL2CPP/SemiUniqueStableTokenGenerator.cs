using Mono.Cecil;
using System;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP
{
	internal static class SemiUniqueStableTokenGenerator
	{
		internal static uint GenerateFor(TypeReference type)
		{
			return (uint)TypeReferenceEqualityComparer.GetHashCodeFor(type);
		}

		internal static uint GenerateFor(MethodReference method)
		{
			string text = method.Module + "_" + method.FullName;
			return (uint)text.GetHashCode();
		}

		internal static uint GenerateFor(string literal)
		{
			return (uint)literal.GetHashCode();
		}
	}
}
