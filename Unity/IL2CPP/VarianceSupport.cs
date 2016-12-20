namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public sealed class VarianceSupport
    {
        [Inject]
        public static INamingService Naming;

        public static string Apply(TypeReference leftType, TypeReference rightType)
        {
            if (leftType.FullName == rightType.FullName)
            {
                return string.Empty;
            }
            return ("(" + Naming.ForVariable(leftType) + ")");
        }

        public static bool IsNeededForConversion(TypeReference leftType, TypeReference rightType)
        {
            if (leftType.IsFunctionPointer || rightType.IsFunctionPointer)
            {
                return false;
            }
            if ((leftType.FullName != rightType.FullName) && (leftType.IsByReference & rightType.IsPointer))
            {
                return true;
            }
            if (leftType.IsByReference || rightType.IsByReference)
            {
                return false;
            }
            if (leftType.FullName == rightType.FullName)
            {
                return false;
            }
            return (leftType.IsArray || rightType.IsArray);
        }
    }
}

