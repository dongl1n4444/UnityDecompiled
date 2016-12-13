using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.GenericsCollection
{
	internal static class Inflater
	{
		public static TypeReference InflateType(GenericContext context, TypeReference typeReference)
		{
			TypeReference typeReference2 = Inflater.InflateTypeWithoutException(context, typeReference);
			if (typeReference2 == null)
			{
				throw new Exception();
			}
			return typeReference2;
		}

		public static GenericInstanceType InflateType(GenericContext context, TypeDefinition typeDefinition)
		{
			return Inflater.ConstructGenericType(context, typeDefinition, typeDefinition.GenericParameters);
		}

		public static GenericInstanceType InflateType(GenericContext context, GenericInstanceType genericInstanceType)
		{
			return Inflater.ConstructGenericType(context, genericInstanceType.Resolve(), genericInstanceType.GenericArguments);
		}

		public static TypeReference InflateTypeWithoutException(GenericContext context, TypeReference typeReference)
		{
			GenericParameter genericParameter = typeReference as GenericParameter;
			TypeReference result;
			if (genericParameter != null)
			{
				TypeReference typeReference2 = (genericParameter.Type != GenericParameterType.Type) ? context.Method.GenericArguments[genericParameter.Position] : context.Type.GenericArguments[genericParameter.Position];
				TypeReference typeReference3 = typeReference2;
				result = typeReference3;
			}
			else
			{
				GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
				if (genericInstanceType != null)
				{
					result = Inflater.InflateType(context, genericInstanceType);
				}
				else
				{
					ArrayType arrayType = typeReference as ArrayType;
					if (arrayType != null)
					{
						result = new ArrayType(Inflater.InflateType(context, arrayType.ElementType), arrayType.Rank);
					}
					else
					{
						PointerType pointerType = typeReference as PointerType;
						if (pointerType != null)
						{
							result = new PointerType(Inflater.InflateType(context, pointerType.ElementType));
						}
						else
						{
							result = typeReference.Resolve();
						}
					}
				}
			}
			return result;
		}

		private static GenericInstanceType ConstructGenericType(GenericContext context, TypeDefinition typeDefinition, IEnumerable<TypeReference> genericArguments)
		{
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeDefinition);
			foreach (TypeReference current in genericArguments)
			{
				genericInstanceType.GenericArguments.Add(Inflater.InflateType(context, current));
			}
			return genericInstanceType;
		}

		public static GenericInstanceMethod InflateMethod(GenericContext context, MethodDefinition methodDefinition)
		{
			TypeReference typeReference = methodDefinition.DeclaringType;
			if (typeReference.Resolve().HasGenericParameters)
			{
				typeReference = Inflater.InflateType(context, methodDefinition.DeclaringType);
			}
			return Inflater.ConstructGenericMethod(context, typeReference, methodDefinition, methodDefinition.GenericParameters);
		}

		public static GenericInstanceMethod InflateMethod(GenericContext context, GenericInstanceMethod genericInstanceMethod)
		{
			GenericInstanceType genericInstanceType = genericInstanceMethod.DeclaringType as GenericInstanceType;
			TypeReference declaringType = (genericInstanceType == null) ? Inflater.InflateType(context, genericInstanceMethod.DeclaringType) : Inflater.InflateType(context, genericInstanceType);
			return Inflater.ConstructGenericMethod(context, declaringType, genericInstanceMethod.Resolve(), genericInstanceMethod.GenericArguments);
		}

		private static GenericInstanceMethod ConstructGenericMethod(GenericContext context, TypeReference declaringType, MethodDefinition method, IEnumerable<TypeReference> genericArguments)
		{
			MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, declaringType)
			{
				HasThis = method.HasThis
			};
			foreach (GenericParameter current in method.GenericParameters)
			{
				methodReference.GenericParameters.Add(new GenericParameter(current.Name, methodReference));
			}
			foreach (ParameterDefinition current2 in method.Parameters)
			{
				methodReference.Parameters.Add(new ParameterDefinition(current2.Name, current2.Attributes, current2.ParameterType));
			}
			if (methodReference.Resolve() == null)
			{
				throw new Exception();
			}
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(methodReference);
			foreach (TypeReference current3 in genericArguments)
			{
				genericInstanceMethod.GenericArguments.Add(Inflater.InflateType(context, current3));
			}
			return genericInstanceMethod;
		}
	}
}
