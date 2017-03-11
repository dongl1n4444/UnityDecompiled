namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    public static class CodeWriterExtensions
    {
        public static void WriteWriteBarrierIfNeeded(this CodeWriter writer, TypeReference valueType, string addressExpression, string valueExpression)
        {
            if (!valueType.IsValueType() && !valueType.IsPointer)
            {
                object[] args = new object[] { addressExpression, valueExpression };
                writer.WriteLine("Il2CppCodeGenWriteBarrier({0}, {1});", args);
            }
        }
    }
}

