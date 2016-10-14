using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public sealed class VarianceSupport
	{
		[Inject]
		public static INamingService Naming;

		public static bool IsNeededForConversion(TypeReference leftType, TypeReference rightType)
		{
			return !leftType.IsFunctionPointer && !rightType.IsFunctionPointer && !leftType.IsByReference && !rightType.IsByReference && !(leftType.FullName == rightType.FullName) && (leftType.IsArray || rightType.IsArray);
		}

		public static string Apply(TypeReference leftType, TypeReference rightType)
		{
			string result;
			if (leftType.FullName == rightType.FullName)
			{
				result = string.Empty;
			}
			else
			{
				result = "(" + VarianceSupport.Naming.ForVariable(leftType) + ")";
			}
			return result;
		}
	}
}
