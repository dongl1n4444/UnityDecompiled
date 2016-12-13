using Mono.Cecil;
using System;

namespace Unity.IL2CPP
{
	public static class GenericsUtilities
	{
		private const int MaximumEmittedGenericDepth = 7;

		public static bool CheckForMaximumRecursion(GenericInstanceType genericInstanceType)
		{
			return GenericsUtilities.RecursiveGenericDepthFor(genericInstanceType) >= 7;
		}

		public static bool CheckForMaximumRecursionPlusTwo(GenericInstanceType genericInstanceType)
		{
			return GenericsUtilities.RecursiveGenericDepthFor(genericInstanceType) >= 9;
		}

		public static int RecursiveGenericDepthFor(GenericInstanceType type)
		{
			int result;
			if (type == null)
			{
				result = 0;
			}
			else
			{
				result = GenericsUtilities.RecursiveGenericDepthFor(type, (!type.HasGenericArguments) ? 0 : 1);
			}
			return result;
		}

		private static int RecursiveGenericDepthFor(GenericInstanceType type, int depth)
		{
			int num = 0;
			foreach (TypeReference current in type.GenericArguments)
			{
				num = GenericsUtilities.MaximumDepthFor(depth, current, num);
			}
			return depth + num;
		}

		private static int RecursiveGenericDepthFor(ArrayType type, int depth)
		{
			return depth + GenericsUtilities.MaximumDepthFor(depth, type.ElementType, 0);
		}

		private static int MaximumDepthFor(int depth, TypeReference genericArgument, int maximumDepth)
		{
			if (genericArgument is GenericInstanceType)
			{
				int num = GenericsUtilities.RecursiveGenericDepthFor(genericArgument as GenericInstanceType, depth);
				if (num > maximumDepth)
				{
					maximumDepth = num;
				}
			}
			else if (genericArgument is ArrayType)
			{
				int num2 = GenericsUtilities.RecursiveGenericDepthFor(genericArgument as ArrayType, depth);
				if (num2 > maximumDepth)
				{
					maximumDepth = num2;
				}
			}
			return maximumDepth;
		}

		public static bool IsGenericInstanceOfCompareExchange(MethodReference methodReference)
		{
			return methodReference.DeclaringType.Name == "Interlocked" && methodReference.Name == "CompareExchange" && methodReference.IsGenericInstance;
		}

		public static bool IsGenericInstanceOfExchange(MethodReference methodReference)
		{
			return methodReference.DeclaringType.Name == "Interlocked" && methodReference.Name == "Exchange" && methodReference.IsGenericInstance;
		}
	}
}
