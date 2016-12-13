using Mono.Cecil;
using System;

namespace Unity.IL2CPP.GenericsCollection
{
	internal static class ArrayRegistration
	{
		public static bool ShouldForce2DArrayFor(TypeDefinition type)
		{
			return type.MetadataType == MetadataType.Single;
		}
	}
}
