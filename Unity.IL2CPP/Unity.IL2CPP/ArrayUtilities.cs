using Mono.Cecil;
using System;

namespace Unity.IL2CPP
{
	public class ArrayUtilities
	{
		public static TypeReference ArrayElementTypeOf(TypeReference typeReference)
		{
			ArrayType arrayType = typeReference as ArrayType;
			TypeReference result;
			if (arrayType != null)
			{
				result = arrayType.ElementType;
			}
			else
			{
				TypeSpecification typeSpecification = typeReference as TypeSpecification;
				if (typeSpecification == null)
				{
					throw new ArgumentException(string.Format("{0} is not an array type", typeReference.FullName), "typeReference");
				}
				result = ArrayUtilities.ArrayElementTypeOf(typeSpecification.ElementType);
			}
			return result;
		}

		internal static ModuleDefinition ModuleDefinitionForElementTypeOf(ArrayType arrayType)
		{
			return arrayType.ElementType.Resolve().Module;
		}
	}
}
