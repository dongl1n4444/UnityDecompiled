namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public static class GenericsUtilities
    {
        private const int MaximumEmittedGenericDepth = 7;

        public static bool CheckForMaximumRecursion(GenericInstanceType genericInstanceType) => 
            (RecursiveGenericDepthFor(genericInstanceType) >= 7);

        public static bool CheckForMaximumRecursionPlusTwo(GenericInstanceType genericInstanceType) => 
            (RecursiveGenericDepthFor(genericInstanceType) >= 9);

        public static bool IsGenericInstanceOfCompareExchange(MethodReference methodReference) => 
            (((methodReference.DeclaringType.Name == "Interlocked") && (methodReference.Name == "CompareExchange")) && methodReference.IsGenericInstance);

        public static bool IsGenericInstanceOfExchange(MethodReference methodReference) => 
            (((methodReference.DeclaringType.Name == "Interlocked") && (methodReference.Name == "Exchange")) && methodReference.IsGenericInstance);

        private static int MaximumDepthFor(int depth, TypeReference genericArgument, int maximumDepth)
        {
            if (genericArgument is GenericInstanceType)
            {
                int num = RecursiveGenericDepthFor(genericArgument as GenericInstanceType, depth);
                if (num > maximumDepth)
                {
                    maximumDepth = num;
                }
                return maximumDepth;
            }
            if (genericArgument is ArrayType)
            {
                int num2 = RecursiveGenericDepthFor(genericArgument as ArrayType, depth);
                if (num2 > maximumDepth)
                {
                    maximumDepth = num2;
                }
            }
            return maximumDepth;
        }

        public static int RecursiveGenericDepthFor(GenericInstanceType type)
        {
            if (type == null)
            {
                return 0;
            }
            return RecursiveGenericDepthFor(type, !type.HasGenericArguments ? 0 : 1);
        }

        private static int RecursiveGenericDepthFor(ArrayType type, int depth) => 
            (depth + MaximumDepthFor(depth, type.ElementType, 0));

        private static int RecursiveGenericDepthFor(GenericInstanceType type, int depth)
        {
            int maximumDepth = 0;
            foreach (TypeReference reference in type.GenericArguments)
            {
                maximumDepth = MaximumDepthFor(depth, reference, maximumDepth);
            }
            return (depth + maximumDepth);
        }
    }
}

