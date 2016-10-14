using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Unity.IL2CPP
{
	internal class GenericParameterResolver
	{
		internal static TypeReference ResolveReturnTypeIfNeeded(MethodReference methodReference)
		{
			TypeReference result;
			if (methodReference.DeclaringType.IsArray && methodReference.Name == "Get")
			{
				result = methodReference.ReturnType;
			}
			else
			{
				GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
				GenericInstanceType genericInstanceType = methodReference.DeclaringType as GenericInstanceType;
				if (genericInstanceMethod == null && genericInstanceType == null)
				{
					result = methodReference.ReturnType;
				}
				else
				{
					result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, methodReference.ReturnType);
				}
			}
			return result;
		}

		internal static TypeReference ResolveFieldTypeIfNeeded(FieldReference fieldReference)
		{
			return GenericParameterResolver.ResolveIfNeeded(null, fieldReference.DeclaringType as GenericInstanceType, fieldReference.FieldType);
		}

		internal static TypeReference ResolveParameterTypeIfNeeded(MethodReference method, ParameterReference parameter)
		{
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
			TypeReference result;
			if (genericInstanceMethod == null && genericInstanceType == null)
			{
				result = parameter.ParameterType;
			}
			else
			{
				result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, parameter.ParameterType);
			}
			return result;
		}

		internal static TypeReference ResolveVariableTypeIfNeeded(MethodReference method, VariableReference variable)
		{
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
			TypeReference result;
			if (genericInstanceMethod == null && genericInstanceType == null)
			{
				result = variable.VariableType;
			}
			else
			{
				result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, variable.VariableType);
			}
			return result;
		}

		private static TypeReference ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance declaringGenericInstanceType, TypeReference parameterType)
		{
			ByReferenceType byReferenceType = parameterType as ByReferenceType;
			TypeReference result;
			if (byReferenceType != null)
			{
				result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, byReferenceType);
			}
			else
			{
				ArrayType arrayType = parameterType as ArrayType;
				if (arrayType != null)
				{
					result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, arrayType);
				}
				else
				{
					GenericInstanceType genericInstanceType = parameterType as GenericInstanceType;
					if (genericInstanceType != null)
					{
						result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, genericInstanceType);
					}
					else
					{
						GenericParameter genericParameter = parameterType as GenericParameter;
						if (genericParameter != null)
						{
							result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, genericParameter);
						}
						else
						{
							RequiredModifierType requiredModifierType = parameterType as RequiredModifierType;
							if (requiredModifierType != null && requiredModifierType.ContainsGenericParameters())
							{
								result = GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, requiredModifierType.ElementType);
							}
							else
							{
								if (parameterType.ContainsGenericParameters())
								{
									throw new Exception("Unexpected generic parameter.");
								}
								result = parameterType;
							}
						}
					}
				}
			}
			return result;
		}

		private static TypeReference ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, GenericParameter genericParameterElement)
		{
			return (genericParameterElement.MetadataType != MetadataType.MVar) ? genericInstanceType.GenericArguments[genericParameterElement.Position] : ((genericInstanceMethod == null) ? genericParameterElement : genericInstanceMethod.GenericArguments[genericParameterElement.Position]);
		}

		private static ArrayType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, ArrayType arrayType)
		{
			return new ArrayType(GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, arrayType.ElementType), arrayType.Rank);
		}

		private static ByReferenceType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, ByReferenceType byReferenceType)
		{
			return new ByReferenceType(GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, byReferenceType.ElementType));
		}

		private static GenericInstanceType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, GenericInstanceType genericInstanceType1)
		{
			GenericInstanceType result;
			if (!genericInstanceType1.ContainsGenericParameters())
			{
				result = genericInstanceType1;
			}
			else
			{
				GenericInstanceType genericInstanceType2 = new GenericInstanceType(genericInstanceType1.ElementType);
				foreach (TypeReference current in genericInstanceType1.GenericArguments)
				{
					if (!current.IsGenericParameter)
					{
						genericInstanceType2.GenericArguments.Add(GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, current));
					}
					else
					{
						GenericParameter genericParameter = (GenericParameter)current;
						GenericParameterType type = genericParameter.Type;
						if (type != GenericParameterType.Type)
						{
							if (type == GenericParameterType.Method)
							{
								if (genericInstanceMethod == null)
								{
									genericInstanceType2.GenericArguments.Add(genericParameter);
								}
								else
								{
									genericInstanceType2.GenericArguments.Add(genericInstanceMethod.GenericArguments[genericParameter.Position]);
								}
							}
						}
						else
						{
							if (genericInstanceType == null)
							{
								throw new NotSupportedException();
							}
							genericInstanceType2.GenericArguments.Add(genericInstanceType.GenericArguments[genericParameter.Position]);
						}
					}
				}
				result = genericInstanceType2;
			}
			return result;
		}
	}
}
